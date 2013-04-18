package com.waveface.sync.task;

import android.content.Context;
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
		BackupLogic.scanFileForBackup(mContext, Constant.TYPE_IMAGE);
		return null;
	}
}
