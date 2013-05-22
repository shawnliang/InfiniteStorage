package com.waveface.favoriteplayer.task;

import idv.jason.lib.imagemanager.ImageManager;

import java.util.ArrayList;
import java.util.HashMap;

import android.content.Context;
import android.database.Cursor;
import android.os.AsyncTask;

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
		if(NetworkUtil.isWifiNetworkAvailable(mContext) == false)
			return null;
		ArrayList<ServerEntity> servers = ServersLogic
				.getBackupedServers(mContext);
		if(servers.size()==0)
			return null;
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
				
				
				Cursor cursor = LabelDB.getAllLabels(mContext);
				String labelId = null;
				String fileId = null;
				String type = null;
				if(cursor!=null && cursor.getCount()>0){
					int count = cursor.getCount();
					cursor.moveToFirst();
					for(int i = 0 ;  i<count;i++){
						labelId = cursor.getString(0);
						Cursor fileCursor = LabelDB.getFilesByLabelId(mContext, labelId,100);
						if(fileCursor!=null && fileCursor.getCount()>0){
							int filecount = fileCursor.getCount();
							fileCursor.moveToFirst();
							syncingEvent.totalFile = filecount;
							for(int j = 0 ;  j<filecount;j++){
								fileId = fileCursor.getString(fileCursor.getColumnIndex(LabelFileView.COLUMN_FILE_ID));
								type = fileCursor.getString(fileCursor.getColumnIndex(LabelFileView.COLUMN_TYPE));
								if(type.equals("0")){
									String url = restfulAPIURL + Constant.URL_IMAGE + "/"
											+ fileId + Constant.URL_IMAGE_LARGE;
									long time = System.currentTimeMillis();
									mImageManager.getImageWithoutThread(url, null);
									time = System.currentTimeMillis() - time;
									syncingEvent.singleTime = time;
									syncingEvent.currentFile++;
									EventBus.getDefault().post(syncingEvent);
								}
								else if(type.equals("1")){
									//Videos
									String url = restfulAPIURL + Constant.URL_IMAGE + "/"
											+ fileId + Constant.URL_IMAGE;
									Log.d(TAG, "Video url:"+url);
								}
								fileCursor.moveToNext();
							}
							EventBus.getDefault().post(doneEvent);
							fileCursor.close();
							fileCursor = null ;
						}
						cursor.moveToNext();
					}
				}
				cursor.close();
				cursor = null;
//				for (LabelEntity.Label label : entity.labels) {
//					if (label.files == null)
//						continue;
//					syncingEvent.totalFile = label.files.length;
//					for (int i = 0; i < label.files.length; ++i) {
//						String url = restfulAPIURL + Constant.URL_IMAGE + "/"
//								+ label.files[i] + Constant.URL_IMAGE_LARGE;
//						
//						long time = System.currentTimeMillis();
//						mImageManager.getImageWithoutThread(url, null);
//						time = System.currentTimeMillis() - time;
//						syncingEvent.singleTime = time;
//						syncingEvent.currentFile++;
//						EventBus.getDefault().post(syncingEvent);
//					}
//
//					EventBus.getDefault().post(doneEvent);
//				}
			}

		} catch (WammerServerException e) {
			e.printStackTrace();
			return null;
		}
		return null;
	}
}
