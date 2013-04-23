package com.waveface.sync.task;

import android.content.Context;
import android.os.AsyncTask;

import com.waveface.sync.Constant;
import com.waveface.sync.RuntimeState;
import com.waveface.sync.logic.BackupLogic;

public class ScanTask extends AsyncTask<Void, Void, Void> {
	private Context mContext;

	public ScanTask(Context context) {
		mContext = context;
	}
	@Override
	protected Void doInBackground(Void... params) {
		BackupLogic.scanAllFiles(mContext);
		return null;
	}
	@Override
	protected void onPostExecute(Void entity) {
		super.onPostExecute(entity);
	}
}
