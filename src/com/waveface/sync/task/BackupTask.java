package com.waveface.sync.task;

import android.content.Context;
import android.os.AsyncTask;
import android.text.TextUtils;

import com.waveface.sync.Constant;
import com.waveface.sync.RuntimeState;
import com.waveface.sync.logic.BackupLogic;

public class BackupTask extends AsyncTask<Void, Void, Void> {
	private Context mContext;

	public BackupTask(Context context) {
		mContext = context;
	}
	@Override
	protected Void doInBackground(Void... params) {
		if(!TextUtils.isEmpty(RuntimeState.mWebSocketServerId)){
			BackupLogic.backupFiles(mContext, RuntimeState.mWebSocketServerId);
			RuntimeState.isBackuping = false;
		}
		return null;
	}
	@Override
	protected void onPostExecute(Void entity) {
		super.onPostExecute(entity);
	}
}
