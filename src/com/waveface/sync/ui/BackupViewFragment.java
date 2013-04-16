package com.waveface.sync.ui;

import android.content.Context;
import android.content.pm.ActivityInfo;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.CompoundButton.OnCheckedChangeListener;

import com.waveface.sync.Constant;
import com.waveface.sync.R;

public class BackupViewFragment extends LinkFragmentBase implements OnClickListener, OnCheckedChangeListener{
	public final String TAG = BackupViewFragment.class.getSimpleName();

	private boolean mImport = false;
	private ViewGroup mRootView;
	private boolean mHideBack = false;
    private CheckBox mImportCB ;
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
				R.layout.backupview, null);
		Button btn = (Button) mRootView.findViewById(R.id.btnConfirm);
		btn.setOnClickListener(this);
		return mRootView;
	}

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		if (Constant.PHONE) {
			getActivity().setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);
		}
	}

	@Override
	public void onDestroy() {
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
		mImport = arg1;
		mImportCB.setChecked(mImport);
		getActivity().getSharedPreferences(Constant.PREFS_NAME,Context.MODE_PRIVATE)
			.edit().putBoolean(Constant.PREF_AUTO_IMPORT_ENABLED, mImport).commit();
	}
}