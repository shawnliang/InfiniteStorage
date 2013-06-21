package com.waveface.favoriteplayer.logic;

import idv.jason.lib.imagemanager.ImageManager;

import java.io.File;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.HashMap;

import org.jwebsocket.kit.WebSocketException;

import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.media.ThumbnailUtils;
import android.os.Environment;
import android.provider.MediaStore.Video.Thumbnails;
import android.text.TextUtils;

import com.waveface.exception.WammerServerException;
import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.RuntimeState;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.db.LabelFileView;
import com.waveface.favoriteplayer.db.LabelTable;
import com.waveface.favoriteplayer.entity.ConnectForGTVEntity;
import com.waveface.favoriteplayer.entity.FileEntity;
import com.waveface.favoriteplayer.entity.LabelEntity;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.event.LabelImportedEvent;
import com.waveface.favoriteplayer.util.DeviceUtil;
import com.waveface.favoriteplayer.util.FileUtil;
import com.waveface.favoriteplayer.util.Log;
import com.waveface.favoriteplayer.util.StringUtil;
import com.waveface.favoriteplayer.websocket.RuntimeWebClient;
import com.waveface.service.HttpInvoker;

import de.greenrobot.event.EventBus;

public class DownloadLogic {

	private static final String TAG = DownloadLogic.class.getSimpleName();

	public static synchronized void downloadLabel(Context context,
			LabelEntity.Label label, boolean autoDownload, boolean isChangeLabel) {
		HashMap<String, String> param = new HashMap<String, String>();
		String files = "";
		String jsonOutput = "";
		ArrayList<ServerEntity> servers = ServersLogic.getPairedServer(context);
		ServerEntity pairedServer = servers.get(0);
		String restfulAPIURL = "http://" + pairedServer.ip + ":"
				+ pairedServer.restPort;
		String getFileURL = restfulAPIURL + Constant.URL_GET_FILE;

		if (label.files.length > 0) {

			for (String f : label.files) {
				files += f + ",";
			}

			files = files.substring(0, files.length() - 1);
			param.put(Constant.PARAM_FILES, files.trim());

			try {
				jsonOutput = HttpInvoker.executePost(getFileURL, param,
						Constant.STATION_CONNECTION_TIMEOUT,
						Constant.STATION_CONNECTION_TIMEOUT);
			} catch (WammerServerException e) {
				e.printStackTrace();
			}

			// FileEntity
			FileEntity fileEntity = RuntimeState.GSON.fromJson(jsonOutput,
					FileEntity.class);
			LabelImportedEvent syncingEvent = new LabelImportedEvent(
					LabelImportedEvent.STATUS_SYNCING);
			LabelImportedEvent doneEvent = new LabelImportedEvent(
					LabelImportedEvent.STATUS_DONE);

			LabelDB.updateLabelInfo(context, label, fileEntity, isChangeLabel);

			String downloadFolder = FileUtil.getDownloadFolder(context);
			File root = null;
			if (TextUtils.isEmpty(downloadFolder)) {
				root = Environment.getExternalStorageDirectory();
			} else {
				root = new File(FileUtil.getDownloadFolder(context));
			}

			ImageManager imageManager = SyncApplication
					.getWavefacePlayerApplication(context).getImageManager();

			Cursor filecursor = LabelDB.getLabelFileViewByLabelId(context,
					label.label_id);
			if (filecursor != null && filecursor.getCount() > 0) {
				filecursor.moveToFirst();
				int count = filecursor.getCount();
				syncingEvent.totalFile = count;
				for (int j = 0; j < count; j++) {

					String type = filecursor.getString(filecursor
							.getColumnIndex(LabelFileView.COLUMN_TYPE));
					String fileId = filecursor.getString(filecursor
							.getColumnIndex(LabelFileView.COLUMN_FILE_ID));
					String fileName = filecursor.getString(filecursor
							.getColumnIndex(LabelFileView.COLUMN_FILE_NAME));
					Log.d(TAG, "filename:" + fileName);
					Log.d(TAG, "fileId:" + fileId);
					if (StringUtil.isAvaiableSpace(context,
							Constant.AVAIABLE_SPACE)) {
						if (type.equals(Constant.FILE_TYPE_VIDEO)) {
							String url = restfulAPIURL + Constant.URL_IMAGE
									+ "/" + fileId + "/"
									+ Constant.URL_IMAGE_ORIGIN;
							String fullFilename = root.getAbsolutePath()
									+ Constant.VIDEO_FOLDER + "/" + fileName;
							if (!FileUtil.isFileExisted(fullFilename)) {
								downloadVideo(fileId, fullFilename, url);
								Bitmap bmThumbnail = ThumbnailUtils
										.createVideoThumbnail(fullFilename,
												Thumbnails.MINI_KIND);
								imageManager.setBitmapToFile(bmThumbnail,
										fullFilename, null, false);
							}
							//check file in storage
							if(!FileUtil.isFileExisted(fullFilename)){
								LabelDB.updateFileStatus(context, fileId, Constant.FILE_STATUS_DELETE);
							}
						} else {
							String url = restfulAPIURL + Constant.URL_IMAGE
									+ "/" + fileId + Constant.URL_IMAGE_LARGE;
							imageManager.getImageWithoutThread(url, null);
						}
					} else {
						context.sendBroadcast(new Intent(
								Constant.ACTION_NOT_ENOUGH_SPACE));

					}
					long time = System.currentTimeMillis();
					time = System.currentTimeMillis() - time;
					syncingEvent.singleTime = time;
					syncingEvent.currentFile++;
					EventBus.getDefault().post(syncingEvent);

					filecursor.moveToNext();
				}
				if (autoDownload == false) {
					EventBus.getDefault().post(doneEvent);
				}
			}
			filecursor.close();
		} else {
			if (label.label_name.equals("TAG")) {
				if (label.files.length == 0) {
					LabelDB.removeLabelFileByLabelId(context, label.label_id);
				}
			}
			LabelDB.updateLabel(context, label);
		}

	}

	public static void updateAllLabels(Context context, LabelEntity entity) {
		ContentValues cv = null;
		ContentResolver cr = context.getContentResolver();
		for (LabelEntity.Label label : entity.labels) {
			downloadLabel(context, label, true, false);
			//Update Seq equals ServerSeq
			cv = new ContentValues();
			cv.put(LabelTable.COLUMN_SERVER_SEQ, label.seq);
			cr.update(LabelTable.CONTENT_URI, 
					cv, 
					LabelTable.COLUMN_LABEL_ID+"=?", 
					new String[]{label.label_id});
		}
		LabelImportedEvent doneEvent = new LabelImportedEvent(
				LabelImportedEvent.STATUS_DONE);
		EventBus.getDefault().post(doneEvent);
	}

	public static void updateLabel(Context context,
			LabelEntity.Label entity,String serverSeq) {
		downloadLabel(context, entity, false, true);
		//Update Seq equals ServerSeq
		ContentValues cv = new ContentValues();
		cv.put(LabelTable.COLUMN_SEQ, serverSeq);
		
		int result = context.getContentResolver().update(LabelTable.CONTENT_URI, 
				cv, 
				LabelTable.COLUMN_LABEL_ID+"=?", 
				new String[]{entity.label_id});
		Log.i(TAG, "Update SEQ to ["+serverSeq+"]:"+result);
	}

	public static void downloadVideo(String fileId, String fileName, String url) {
		InputStream is = null;
		try {
			is = HttpInvoker.getInputStreamFromUrl(url);
		} catch (WammerServerException e) {
			e.printStackTrace();
		}
		if (is != null) {
			FileUtil.downloadFile(is, fileName);
		}
	}

	public static void subscribe(Context context) {
		if (RuntimeState.OnWebSocketOpened == false)
			return;
		// sendSubcribe
		String labSeq = LabelDB.getMAXServerSeq(context);
		
		ConnectForGTVEntity connectForGTV = new ConnectForGTVEntity();
		ConnectForGTVEntity.Connect connect = new ConnectForGTVEntity.Connect();
		connect.deviceId = DeviceUtil.id(context);
		connect.deviceName = DeviceUtil.getDeviceNameForDisplay(context);
		connectForGTV.setConnect(connect);
		ConnectForGTVEntity.Subscribe subscribe = new ConnectForGTVEntity.Subscribe();
		subscribe.labels = true;
		subscribe.labels_from_seq = labSeq;
		connectForGTV.setSubscribe(subscribe);
		Log.d(TAG, "send message=" + RuntimeState.GSON.toJson(connectForGTV));

		try {
			RuntimeWebClient.send(RuntimeState.GSON.toJson(connectForGTV));
		} catch (WebSocketException e) {
			e.printStackTrace();
		}
	}
}
