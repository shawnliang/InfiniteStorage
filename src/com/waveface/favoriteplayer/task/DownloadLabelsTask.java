package com.waveface.favoriteplayer.task;

import idv.jason.lib.imagemanager.ImageManager;

import java.io.File;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.HashMap;

import android.content.Context;
import android.database.Cursor;
import android.os.AsyncTask;
import android.os.Environment;
import android.text.TextUtils;

import com.waveface.exception.WammerServerException;
import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.RuntimeState;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.db.LabelFileView;
import com.waveface.favoriteplayer.entity.FileEntity;
import com.waveface.favoriteplayer.entity.LabelEntity;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.event.LabelImportedEvent;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.util.FileUtil;
import com.waveface.favoriteplayer.util.Log;
import com.waveface.favoriteplayer.util.NetworkUtil;
import com.waveface.service.HttpInvoker;

import de.greenrobot.event.EventBus;

public class DownloadLabelsTask extends AsyncTask<Void, Void, Void> {

	private static final String TAG = DownloadLabelsTask.class.getSimpleName();
	private ImageManager mImageManager;

	private Context mContext;

	public DownloadLabelsTask(Context context) {
		mContext = context;
		mImageManager = SyncApplication.getWavefacePlayerApplication(context)
				.getImageManager();
	}

	@Override
	protected Void doInBackground(Void... params) {
		if (NetworkUtil.isWifiNetworkAvailable(mContext) == false)
			return null;
		ArrayList<ServerEntity> servers = ServersLogic
				.getBackupedServers(mContext);
		if (servers.size() == 0)
			return null;
		ServerEntity pairedServer = servers.get(0);
		if(TextUtils.isEmpty(pairedServer.restPort)){
			pairedServer.restPort ="14005";
		}
		String restfulAPIURL = "http://" + pairedServer.ip + ":"
				+ pairedServer.restPort;
		String getAllLablesURL = restfulAPIURL + Constant.URL_GET_ALL_LABELS;
		String getFileURL = restfulAPIURL + Constant.URL_GET_FILE;
		HashMap<String, String> param = new HashMap<String, String>();
		LabelEntity entity = null;
		FileEntity fileEntity = null;
		String files = "";

		try {
			String jsonOutput = HttpInvoker.executePost(getAllLablesURL, param,
					Constant.STATION_CONNECTION_TIMEOUT,
					Constant.STATION_CONNECTION_TIMEOUT);

			Log.d(TAG, "Labels jsonOutString =" + jsonOutput);
			entity = RuntimeState.GSON.fromJson(jsonOutput, LabelEntity.class);
			if (entity != null) {
				for (LabelEntity.Label label : entity.labels) {
					files = "";
					if (label.files == null)
						continue;
					if (label.files.length > 0) {
						for (String f : label.files) {
							files += f + ",";
						}
						files = files.substring(0, files.length() - 1);
						param.clear();
						param.put(Constant.PARAM_FILES, files.trim());
						jsonOutput = HttpInvoker.executePost(getFileURL, param,
								Constant.STATION_CONNECTION_TIMEOUT,
								Constant.STATION_CONNECTION_TIMEOUT);

						// FileEntity
						fileEntity = RuntimeState.GSON.fromJson(jsonOutput,
								FileEntity.class);

						Log.d(TAG, "file fileEntity =" + fileEntity);
						Log.d(TAG, "file jsonOutString =" + jsonOutput);
						// update label info
						LabelDB.updateLabelInfo(mContext, label, fileEntity, false);
					}else{
						LabelDB.updateLabel(mContext, label);
					}
				}
				LabelImportedEvent syncingEvent = new LabelImportedEvent(
						LabelImportedEvent.STATUS_SYNCING);
				LabelImportedEvent doneEvent = new LabelImportedEvent(
						LabelImportedEvent.STATUS_DONE);
				
//				EventBus.getDefault().post(doneEvent);

				File root = Environment.getExternalStorageDirectory();
				Cursor cursor = LabelDB.getAllLabels(mContext);
				String labelId = null;
				if (cursor != null && cursor.getCount() > 0) {
					cursor.moveToFirst();
					labelId = cursor.getString(0);
					for (int i = 0; i < cursor.getCount(); i++) {
						Cursor filecursor = LabelDB.getLabelFileViewByLabelId(mContext,
								labelId);
						if (filecursor != null && filecursor.getCount() > 0) {
							filecursor.moveToFirst();
							int count = filecursor.getCount();
							syncingEvent.totalFile = count;
							for (int j = 0; j < count; j++) {

								String type = filecursor
										.getString(filecursor
												.getColumnIndex(LabelFileView.COLUMN_TYPE));
								String fileId = filecursor
										.getString(filecursor
												.getColumnIndex(LabelFileView.COLUMN_FILE_ID));
								String fileName = filecursor
										.getString(filecursor
												.getColumnIndex(LabelFileView.COLUMN_FILE_NAME));
								Log.d(TAG, "filename:" + fileName);
								Log.d(TAG, "fileId:" + fileId);
								
								if (type.equals(Constant.FILE_TYPE_VIDEO)) {
									String url = restfulAPIURL + Constant.URL_IMAGE + "/" + fileId
											+ "/" + Constant.URL_IMAGE_ORIGIN;
									String fullFilename = root.getAbsolutePath()
											+ Constant.VIDEO_FOLDER+ "/" + fileName;
									if(!FileUtil.isFileExisted(fullFilename)){
										downloadVideo(fileId, fullFilename,url);
									}
								} else {
									String url = restfulAPIURL
											+ Constant.URL_IMAGE + "/" + fileId
											+ Constant.URL_IMAGE_LARGE;
									
									mImageManager.getImageWithoutThread(url,
											null);
								}
								long time = System.currentTimeMillis();
								time = System.currentTimeMillis() - time;
								syncingEvent.singleTime = time;
								syncingEvent.currentFile++;
								EventBus.getDefault().post(syncingEvent);
								filecursor.moveToNext();
							}
							filecursor.close();
						}
					}
					EventBus.getDefault().post(doneEvent);
				}
				cursor = null;
			}

		} catch (WammerServerException e) {
			e.printStackTrace();
			return null;
		}

		return null;
	}
	public static void downloadVideo(String fileId, String fileName,
			String url) {
		InputStream is = null;
		try {
			is = HttpInvoker.getInputStreamFromUrl(url);
		} catch (WammerServerException e) {
			e.printStackTrace();
		}
		if(is!=null){
			FileUtil.downloadFile(is,fileName);
		}
	}
}
