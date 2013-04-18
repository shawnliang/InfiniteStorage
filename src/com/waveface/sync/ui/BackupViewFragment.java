package com.waveface.sync.ui;

import android.content.Context;
import android.content.SharedPreferences;
import android.content.pm.ActivityInfo;
import android.database.ContentObserver;
import android.os.Bundle;
import android.os.Handler;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.CompoundButton.OnCheckedChangeListener;
import android.widget.ProgressBar;
import android.widget.TextView;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.db.BackupedServersTable;
import com.waveface.sync.logic.BackupLogic;

public class BackupViewFragment extends LinkFragmentBase implements OnClickListener, OnCheckedChangeListener{
	public final String TAG = BackupViewFragment.class.getSimpleName();

	private ViewGroup mRootView;
	private boolean mHideBack = false;
//    private Handler mHandler = new Handler();
    private TextView mTvFileTransfer;
    private ProgressBar mProgressBar;
	private ServerObserver mContentObserver;
	
    public int getHeight() {
		return mRootView.getMeasuredHeight();
	}

	@Override
	public void onBackPressed() {
		if(mHideBack == false)
			mListener.goBack(TAG);
	}

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {

		mRootView = (ViewGroup) inflater.inflate(
				R.layout.firstuse_backup_view, null);
		Button btn = (Button) mRootView.findViewById(R.id.btnConfirm);
		btn.setOnClickListener(this);
		
		mTvFileTransfer = (TextView) mRootView.findViewById(R.id.tvFileTransfer);
		mProgressBar = (ProgressBar)mRootView.findViewById(R.id.pbBackup);
		refreshLayout();
		
//        mHandler.postDelayed(new Runnable() {
//            public void run() {
//            	refreshLayout();
//            	}
//            }, 300);

		return mRootView;
	}
	private class ServerObserver extends ContentObserver {
		public ServerObserver() {
			super(new Handler());
		}
		@Override
		public void onChange(boolean selfChange) {
			refreshLayout();
		}
	}

	public void refreshLayout(){
    	SharedPreferences prefs = getActivity().getSharedPreferences(Constant.PREFS_NAME, Context.MODE_PRIVATE);    	
    	String serverId = prefs.getString(Constant.PREF_SERVER_ID, "");
		int[] datas = BackupLogic.getBackupProgressInfo(getActivity(), serverId);
		String nowProgress = datas[0]+"/"+datas[1];
		mProgressBar.setMax(datas[1]);
		mProgressBar.setProgress(datas[0]);
		mTvFileTransfer.setText(getActivity().getString(R.string.file_transfering,nowProgress));
	}
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		if (Constant.PHONE) {
			getActivity().setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);
		}
		mContentObserver = new ServerObserver();
		getActivity().getContentResolver()
			.registerContentObserver(BackupedServersTable.BACKUPED_SERVER_URI, false, mContentObserver);
	}

	@Override
	public void onDestroy() {
		getActivity().getContentResolver().unregisterContentObserver(mContentObserver);
		super.onDestroy();
	}

	@Override
	public void onClick(View v) {
		int viewId = v.getId();
		switch (viewId) {
			case R.id.btnConfirm:
				mListener.goNext(TAG);
				break;
		}
	}
	public interface AutoImportListener {
		public void importNow();
		public void notImportNow();
	}
	@Override
	public void onCheckedChanged(CompoundButton arg0, boolean arg1) {
	}
}