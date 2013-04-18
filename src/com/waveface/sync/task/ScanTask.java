package com.waveface.sync.task;

import android.content.Context;
import android.content.Intent;
import android.os.AsyncTask;

import com.waveface.sync.Constant;
import com.waveface.sync.logic.BackupLogic;

public class ScanTask extends AsyncTask<Void, Void, Void> {
	private Context mContext;

	public ScanTask(Context context) {
		mContext = context;
	}
	@Override
	protected Void doInBackground(Void... params) {
		BackupLogic.scanFileForBackup(mContext, Constant.TYPE_AUDIO);
		BackupLogic.scanFileForBackup(mContext, Constant.TYPE_VIDEO);
    	Intent intent = new Intent(Constant.ACTION_SCAN_FILE);
        mContext.sendBroadcast(intent);
		BackupLogic.scanFileForBackup(mContext, Constant.TYPE_IMAGE);
        mContext.sendBroadcast(intent);
		return null;
	}
}
