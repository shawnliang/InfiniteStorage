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
import com.waveface.favoriteplayer.entity.LabelEntity;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.event.LabelImportedEvent;
import com.waveface.favoriteplayer.logic.DownloadLogic;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.util.NetworkUtil;
import com.waveface.service.HttpInvoker;

import de.greenrobot.event.EventBus;

public class DownloadLabelsTask extends AsyncTask<Void, Void, Void> {

	private Context mContext;

	public DownloadLabelsTask(Context context) {
		mContext = context;
	}

	@Override
	protected void onPostExecute(Void result) {
		RuntimeState.isDownloadingLabel = false;
		super.onPostExecute(result);
	}

	@Override
	protected Void doInBackground(Void... params) {
		if (NetworkUtil.isWifiNetworkAvailable(mContext) == false)
			return null;
		String ServerSeq = LabelDB.getMAXServerSeq(mContext);
//		Cursor cursor = LabelDB.getMAXSEQLabel(mContext);
//		if (cursor != null && cursor.getCount() > 0) {
		if (!TextUtils.isEmpty(ServerSeq)) {
			DownloadLogic.subscribe(mContext);
			LabelImportedEvent doneEvent = new LabelImportedEvent(
					LabelImportedEvent.STATUS_DONE);
			EventBus.getDefault().post(doneEvent);
			return null;
		}
//		cursor.close();
		SharedPreferences mPrefs = mContext.getSharedPreferences(
				Constant.PREFS_NAME, Context.MODE_PRIVATE);
		Editor mEditor = mPrefs.edit();
		ArrayList<ServerEntity> servers = ServersLogic
				.getPairedServer(mContext);
		if (servers.size() == 0)
			return null;
		ServerEntity pairedServer = servers.get(0);
		if (TextUtils.isEmpty(pairedServer.restPort)) {
			pairedServer.restPort = "14005";
		}
		String restfulAPIURL = "http://" + pairedServer.ip + ":"
				+ pairedServer.restPort;
		String getAllLablesURL = restfulAPIURL + Constant.URL_GET_ALL_LABELS;
		HashMap<String, String> param = new HashMap<String, String>();
		LabelEntity entity = null;

		// Mulit Label download
		String jsonOutput = null;
		try {
			jsonOutput = HttpInvoker.executePost(getAllLablesURL, param,
					Constant.STATION_CONNECTION_TIMEOUT,
					Constant.STATION_CONNECTION_TIMEOUT);
			entity = RuntimeState.GSON.fromJson(jsonOutput, LabelEntity.class);
			if (entity != null) {
				mEditor.putString(Constant.PREF_HOME_SHARING_STATUS, entity.home_sharing);
				mEditor.commit();
				DownloadLogic.updateAllLabels(mContext, entity);
			}
		} catch (WammerServerException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		mEditor.putInt(Constant.PREF_DOWNLOAD_LABEL_INIT_STATUS, 1);
		mEditor.commit();
		return null;
	}

}
