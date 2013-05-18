package com.waveface.sync.task;

import idv.jason.lib.imagemanager.ImageManager;

import java.util.ArrayList;
import java.util.HashMap;

import android.content.Context;
import android.database.Cursor;
import android.os.AsyncTask;

import com.waveface.exception.WammerServerException;
import com.waveface.service.HttpInvoker;
import com.waveface.sync.Constant;
import com.waveface.sync.RuntimeState;
import com.waveface.sync.SyncApplication;
import com.waveface.sync.db.LabelDB;
import com.waveface.sync.entity.FileEntity;
import com.waveface.sync.entity.LabelEntity;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.event.LabelImportedEvent;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.util.Log;

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

		ArrayList<ServerEntity> servers = ServersLogic
				.getBackupedServers(mContext);
		ServerEntity pairedServer = servers.get(0);
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
					Constant.CLOUD_CONNECTION_TIMEOUT,
					Constant.CLOUD_CONNECTION_TIMEOUT);

			Log.d(TAG, "jsonOutString =" + jsonOutput);
			entity = RuntimeState.GSON.fromJson(jsonOutput, LabelEntity.class);
			if (entity != null) {
				for (LabelEntity.Label label : entity.labels) {
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
								Constant.CLOUD_CONNECTION_TIMEOUT,
								Constant.CLOUD_CONNECTION_TIMEOUT);

						// FileEntity
						fileEntity = RuntimeState.GSON.fromJson(jsonOutput,
								FileEntity.class);
						Log.d(TAG, "file fileEntity =" + fileEntity);
						Log.d(TAG, "file jsonOutString =" + jsonOutput);

					}
					// update label info
					LabelDB.updateLabelInfo(mContext, label, fileEntity);
				}


				LabelImportedEvent syncingEvent = new LabelImportedEvent(LabelImportedEvent.STATUS_SYNCING);
				LabelImportedEvent doneEvent = new LabelImportedEvent(LabelImportedEvent.STATUS_DONE);
				
				for (LabelEntity.Label label : entity.labels) {
					if (label.files == null)
						continue;
					for (int i = 0; i < label.files.length; ++i) {
						String url = restfulAPIURL + Constant.URL_IMAGE + "/"
								+ label.files[i] + Constant.URL_IMAGE_LARGE;
						mImageManager.getImageWithoutThread(url, null);

						EventBus.getDefault().post(syncingEvent);
					}

					EventBus.getDefault().post(doneEvent);
				}
			}

		} catch (WammerServerException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			return null;
		}
		Cursor cursor = LabelDB.getAllLabels(mContext);
		String labelId = null;
		if (cursor != null && cursor.getCount() > 0) {
			cursor.moveToFirst();
			labelId = cursor.getString(0);

		}
		cursor = LabelDB.getLabelFilesByLabelId(mContext, labelId);
		if (cursor != null && cursor.getCount() > 0) {
			cursor.moveToFirst();
			int count = cursor.getCount();
			for (int i = 0; i < count; i++) {
				Log.d(TAG, "LABEL FILE TABLE Label ID:" + cursor.getString(0));
				Log.d(TAG, "LABEL FILE TABLE File ID:" + cursor.getString(1));
				cursor.moveToNext();
			}
			cursor.close();
		}

		cursor = LabelDB.getFilesByLabelId(mContext, labelId, 3);
		if (cursor != null && cursor.getCount() > 0) {
			cursor.moveToFirst();
			int count = cursor.getCount();
			for (int i = 0; i < count; i++) {
				Log.d(TAG, "Label ID:" + cursor.getString(0));
				Log.d(TAG, "File ID:" + cursor.getString(1));
				cursor.moveToNext();
			}
			cursor.close();
		}
		cursor = null;
		return null;
	}
}
