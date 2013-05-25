package com.waveface.favoriteplayer.logic;

import idv.jason.lib.imagemanager.ImageManager;

import java.io.File;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.HashMap;

import org.jwebsocket.kit.WebSocketException;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.database.Cursor;
import android.os.Environment;

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
import com.waveface.favoriteplayer.task.DownloadLabelsTask;
import com.waveface.favoriteplayer.util.DeviceUtil;
import com.waveface.favoriteplayer.util.FileUtil;
import com.waveface.favoriteplayer.util.Log;
import com.waveface.favoriteplayer.websocket.RuntimeWebClient;
import com.waveface.service.HttpInvoker;

import de.greenrobot.event.EventBus;

public class DownloadLogic {

	private static final String TAG = DownloadLogic.class.getSimpleName();

	public static synchronized void downloadLabel(Context context, LabelEntity.Label label) {
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
				// TODO Auto-generated catch block
				e.printStackTrace();
			}

			// FileEntity
			FileEntity fileEntity = RuntimeState.GSON.fromJson(jsonOutput,
					FileEntity.class);
			LabelImportedEvent syncingEvent = new LabelImportedEvent(
					LabelImportedEvent.STATUS_SYNCING);
			LabelImportedEvent doneEvent = new LabelImportedEvent(
					LabelImportedEvent.STATUS_DONE);
			LabelDB.updateLabelInfo(context, label, fileEntity, true);
			File root = Environment.getExternalStorageDirectory();
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

					if (type.equals(Constant.FILE_TYPE_VIDEO)) {
						String url = restfulAPIURL + Constant.URL_IMAGE + "/"
								+ fileId + "/" + Constant.URL_IMAGE_ORIGIN;
						String fullFilename = root.getAbsolutePath()
								+ Constant.VIDEO_FOLDER + "/" + fileName;
						if (!FileUtil.isFileExisted(fullFilename)) {
							downloadVideo(fileId, fullFilename, url);
						}
					} else {
						String url = restfulAPIURL + Constant.URL_IMAGE + "/"
								+ fileId + Constant.URL_IMAGE_LARGE;
//						imageManager.getImageWithoutThread(url, null);
					}
					long time = System.currentTimeMillis();
					time = System.currentTimeMillis() - time;
					syncingEvent.singleTime = time;
					syncingEvent.currentFile++;
					EventBus.getDefault().post(syncingEvent);

					filecursor.moveToNext();
				}
				EventBus.getDefault().post(doneEvent);
				filecursor.close();
			}
		} else {
			LabelDB.updateLabel(context, label);
		}

	}

	public static void updateAllLabels(Context context, LabelEntity entity) {
		for (LabelEntity.Label label : entity.labels) {
			downloadLabel(context, label);
		}
		LabelImportedEvent doneEvent = new LabelImportedEvent(
				LabelImportedEvent.STATUS_DONE);
		EventBus.getDefault().post(doneEvent);
		subscribe(context);
	}

	public static void updateLabel(Context context,
			LabelEntity.Label labelEntity) {
		downloadLabel(context, labelEntity);
		// while(!wasSynced(context)){
		// //
		// downloadLabel(context,labelEntity);
		// }
		LabelImportedEvent doneEvent = new LabelImportedEvent(
				LabelImportedEvent.STATUS_DONE);
		EventBus.getDefault().post(doneEvent);
		subscribe(context);
	}

	public static boolean wasSynced(Context context) {
		// TODO: PREF MAS SEQ ID == DB SEQ MAX ID
		int dbSeq = 0;
		boolean wasSynced = false;
		SharedPreferences mPrefs = context.getSharedPreferences(
				Constant.PREFS_NAME, Context.MODE_PRIVATE);
		int prefSeq = mPrefs.getInt(Constant.PREF_SERVER_LABEL_SEQ, 0);
		Cursor cursor = LabelDB.getMAXSEQLabel(context);

		if (cursor != null && cursor.getCount() > 0) {
			cursor.moveToFirst();
			dbSeq = cursor.getInt(cursor.getColumnIndex(LabelTable.COLUMN_SEQ));
		}
		cursor.close();

		if (dbSeq >= prefSeq) {
			wasSynced = true;
		}
		return wasSynced;
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
		if(RuntimeState.OnWebSocketOpened==false)
			return;
		// sendSubcribe
		Cursor maxLabelcursor = LabelDB.getMAXSEQLabel(context);
		String labSeq = "0";
		if (maxLabelcursor != null && maxLabelcursor.getCount() > 0) {
			maxLabelcursor.moveToFirst();
			labSeq = maxLabelcursor.getString(maxLabelcursor
					.getColumnIndex(LabelTable.COLUMN_SEQ));
			// send broadcast label change
			context.sendBroadcast(new Intent(Constant.ACTION_LABEL_CHANGE));
		}

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
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

}
