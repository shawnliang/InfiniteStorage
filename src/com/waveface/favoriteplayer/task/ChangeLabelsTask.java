package com.waveface.favoriteplayer.task;

import java.util.ArrayList;
import java.util.HashMap;

import android.content.Context;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.database.Cursor;
import android.os.AsyncTask;
import android.text.TextUtils;

import com.waveface.exception.WammerServerException;
import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.RuntimeState;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.db.LabelTable;
import com.waveface.favoriteplayer.entity.LabelEntity;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.event.LabelImportedEvent;
import com.waveface.favoriteplayer.logic.DownloadLogic;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.util.NetworkUtil;
import com.waveface.service.HttpInvoker;

import de.greenrobot.event.EventBus;

public class ChangeLabelsTask extends AsyncTask<Void, Void, Void> {

	private Context mContext;

	public ChangeLabelsTask(Context context) {
		mContext = context;
	}

	@Override
	protected void onPostExecute(Void result) {
		
		super.onPostExecute(result);
	}

	@Override
	protected Void doInBackground(Void... params) {
		if (NetworkUtil.isWifiNetworkAvailable(mContext) == false)
			return null;
		
		
		SharedPreferences mPrefs = mContext.getSharedPreferences(
				Constant.PREFS_NAME, Context.MODE_PRIVATE);
		Editor mEditor = mPrefs.edit();
		
		// Find a different seq and server seq
		
		 Cursor diffSeqLabelCursor =  LabelDB.getDiffSeqLabel(mContext);
		
		 diffSeqLabelCursor.moveToFirst();
		  
		  
		  if(diffSeqLabelCursor.getCount()>0){
		 
			  String labelId =diffSeqLabelCursor.getColumnName(diffSeqLabelCursor.getColumnIndex(LabelTable.COLUMN_LABEL_ID));
			  String coverUrl =diffSeqLabelCursor.getColumnName(diffSeqLabelCursor.getColumnIndex(LabelTable.COLUMN_COVER_URL));
			  String autoType =diffSeqLabelCursor.getColumnName(diffSeqLabelCursor.getColumnIndex(LabelTable.COLUMN_AUTO_TYPE));
			  String onAir =diffSeqLabelCursor.getColumnName(diffSeqLabelCursor.getColumnIndex(LabelTable.COLUMN_ON_AIR));
			  ArrayList<ServerEntity> servers = ServersLogic
					.getPairedServer(mContext);
					ServerEntity pairedServer = servers.get(0);
					String restfulAPIURL = "http://" + pairedServer.ip
							+ ":" + pairedServer.restPort;
					String getLabelURL = restfulAPIURL
							+ Constant.URL_GET_LABEL;
					HashMap<String, String> param = new HashMap<String, String>();
					param.clear();
					param.put(Constant.PARAM_LABEL_ID,
							labelId);
					String jsonOutput;
					try {
						jsonOutput = HttpInvoker.executePost(getLabelURL,
								param, Constant.STATION_CONNECTION_TIMEOUT,
								Constant.STATION_CONNECTION_TIMEOUT);
						
						LabelEntity.Label labelEntity = RuntimeState.GSON
								.fromJson(jsonOutput,
										LabelEntity.Label.class);
					
						labelEntity.cover_url=coverUrl;
						labelEntity.auto_type=autoType;
						labelEntity.on_air=onAir;
						labelEntity.deleted="false";
						
//						RuntimeState.labelsHashSet.add(labelId);
//						mEditor.putStringSet(Constant.PREF_SERVER_CHANGE_LABELS, RuntimeState.labelsHashSet);
//						mEditor.commit();
						DownloadLogic.updateLabel(mContext, labelEntity);
				
			} catch (WammerServerException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
					diffSeqLabelCursor.close();
		 
		  }
		
		return null;
	}

}
