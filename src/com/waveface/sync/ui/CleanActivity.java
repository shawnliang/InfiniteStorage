package com.waveface.sync.ui;

import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.os.Bundle;
import android.support.v4.app.FragmentActivity;
import android.widget.Toast;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.ui.FragmentBase.onFragmentChangedListener;

public class CleanActivity extends FragmentActivity 
	implements 
		CleanStorageFragment.CleanStorageListener,onFragmentChangedListener{
	public static final String TAG = CleanActivity.class.getSimpleName();

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		if ((getIntent().getFlags() & Intent.FLAG_ACTIVITY_BROUGHT_TO_FRONT) != 0) {
			finish();
			return;
		}
		getWindow().setBackgroundDrawable(null);
		setContentView(R.layout.entry_activity);
		if (Constant.PHONE) {
			setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);
		}
		if (savedInstanceState == null) {
			CleanStorageFragment fragment = new CleanStorageFragment();
			getSupportFragmentManager().beginTransaction()
					.add(R.id.entry_main, fragment, CleanStorageFragment.class.getSimpleName()).commit();
		}
	}

	@Override
	public void onSaveInstanceState(Bundle outState) {
		super.onSaveInstanceState(outState);
	}

	@Override
	protected void onDestroy() {
		super.onDestroy();
	}



	@Override
	public void keepThreeMonths() {
		// TODO Auto-generated method stub
		Toast.makeText(this, "KEEP ON WORKING THREE MONTH", Toast.LENGTH_LONG).show();
	}

	@Override
	public void keepSixMonths() {
		// TODO Auto-generated method stub
		Toast.makeText(this, "KEEP ON WORKING SIX MONTH", Toast.LENGTH_LONG).show();
		
	}

	@Override
	public void KeepSetting() {
		// TODO Auto-generated method stub
		Toast.makeText(this, "KEEP ON WORKING SETTING", Toast.LENGTH_LONG).show();		
	}

	@Override
	public void cancel() {
		finish();
	}

	@Override
	public void goBack(String id) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void goNext(String id) {
		// TODO Auto-generated method stub
		
	}
}
