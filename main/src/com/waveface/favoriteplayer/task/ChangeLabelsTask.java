package com.waveface.favoriteplayer.task;

import java.util.ArrayList;
import java.util.HashMap;

import android.content.Context;
import android.database.Cursor;
import android.os.AsyncTask;
import com.waveface.exception.WammerServerException;
import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.RuntimeState;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.db.LabelTable;
import com.waveface.favoriteplayer.entity.LabelEntity;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.event.LabelChangeEvent;
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
		RuntimeState.setSyncing(false);
		super.onPostExecute(result);
	}

	@Override
	protected Void doInBackground(Void... params) {
		if (NetworkUtil.isWifiNetworkAvailable(mContext) == false)
			return null;
		// Find a different seq and server seq For All Label
		Cursor cursor = LabelDB.getUnsyncedLabel(mContext);
		while (cursor!=null && cursor.moveToFirst()) {
			syncLabelProcess(cursor);
			cursor.close();
			cursor = null;
			cursor = LabelDB.getUnsyncedLabel(mContext);
		}
		if (cursor!=null)
			cursor.close();
		return null;
	}

	private void syncLabelProcess(Cursor cursor) {
		String jsonOutput = null;
		String labelId = null;
		String coverUrl = null;
		String autoType = null;
		String serverSeq = null;		
		String onAir = null;

		ArrayList<ServerEntity> servers = ServersLogic
				.getPairedServer(mContext);
		ServerEntity pairedServer = servers.get(0);
		String restfulAPIURL = "http://" + pairedServer.ip + ":"
				+ pairedServer.restPort;
		String getLabelURL = restfulAPIURL + Constant.URL_GET_LABEL;
		HashMap<String, String> param = null;
					
		for(int i = 0 ; i < cursor.getCount();i++){
			labelId = cursor.getString(cursor
							.getColumnIndex(LabelTable.COLUMN_LABEL_ID));
			coverUrl = cursor.getString(cursor
							.getColumnIndex(LabelTable.COLUMN_COVER_URL));
			autoType = cursor.getString(cursor
							.getColumnIndex(LabelTable.COLUMN_AUTO_TYPE));
			onAir = cursor.getString(cursor
					.getColumnIndex(LabelTable.COLUMN_ON_AIR));
			serverSeq = cursor.getString(cursor
					.getColumnIndex(LabelTable.COLUMN_SERVER_SEQ));
			String seq = cursor.getString(cursor
					.getColumnIndex(LabelTable.COLUMN_SEQ));
			String displayStatus = cursor.getString(cursor
					.getColumnIndex(LabelTable.COLUMN_DISPLAY_STATUS));
			
			
			param = new HashMap<String, String>();
			param.put(Constant.PARAM_LABEL_ID, labelId);
			try {
				jsonOutput = HttpInvoker.executePost(getLabelURL, param,
						Constant.STATION_CONNECTION_TIMEOUT,
						Constant.STATION_CONNECTION_TIMEOUT);

				LabelEntity.Label labelEntity = RuntimeState.GSON.fromJson(
						jsonOutput, LabelEntity.Label.class);

				labelEntity.cover_url = coverUrl;
				labelEntity.auto_type = autoType;
				labelEntity.on_air = onAir;
				labelEntity.deleted = "false";

				DownloadLogic.updateLabel(mContext, labelEntity,serverSeq);
				EventBus.getDefault().post(
						new LabelChangeEvent(
								labelId,autoType));

			} catch (WammerServerException e) {
				e.printStackTrace();
			}
			cursor.moveToNext();
		}
	}
}
