package com.waveface.sync.task;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.AsyncTask;

import com.waveface.sync.Constant;
import com.waveface.sync.RuntimeConfig;
import com.waveface.sync.logic.BackupLogic;

public class BackupFilesTask extends AsyncTask<Void, Void, Void> {
	private static final String TAG = BackupFilesTask.class.getSimpleName();

	private Context mContext;

	public BackupFilesTask(Context context) {
		mContext = context;
	}
	@Override
	protected Void doInBackground(Void... params) {
	   	SharedPreferences prefs = mContext.getSharedPreferences(Constant.PREFS_NAME, Context.MODE_PRIVATE);    	
    	String serverId = prefs.getString(Constant.PREF_SERVER_ID, "");
    	while(BackupLogic.needToBackup(mContext,serverId) && BackupLogic.canBackup(mContext)){
    		BackupLogic.backupFiles(mContext, serverId);
    	}
		return null;
	}
	
	@Override
	protected void onPostExecute(Void entity) {
		RuntimeConfig.isBackuping = false;
		Intent intent = new Intent(Constant.ACTION_BACKUP_DONE);
		mContext.sendBroadcast(intent);
		super.onPostExecute(entity);
	}
	@Override
	protected void onPreExecute() {
		super.onPreExecute();
	}

}
