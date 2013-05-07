package com.waveface.sync.ui.preference;

import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.OnSharedPreferenceChangeListener;
import android.content.pm.PackageManager.NameNotFoundException;
import android.net.Uri;
import android.os.Bundle;
import android.os.Handler;
import android.preference.Preference;
import android.preference.PreferenceActivity;
import android.preference.PreferenceScreen;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.ui.AddServerActivity;

public class Preferences extends PreferenceActivity implements
		OnSharedPreferenceChangeListener {
	private static final String TAG = Preferences.class.getSimpleName();
	// SETTING
	private final static String KEY_CHANGE_PC = "changePc";
	// ABOUT
	private final static String KEY_SUPPORT = "support";
	private final static String KEY_VERSION = "version";

	private final static int MILLSECOND_FOR_REFRESH_INFO = 5000;

	// SETTING
	private Preference mChangePCPref;

	// ABOUT
	private Preference mSupportPref;

	private boolean mCanBackPress = true;

	// REQUEST
	public final static int REQUEST_CHANGE_PC = 100;
	public final static int REQUEST_STORAGE_PLAN = 101;
	public final static int REQUEST_PHOTO_AUTO_IMPORT = 102;
	public final static int REQUEST_WEB_SERVICE_IMPORT = 103;

	private final Handler mHandler = new Handler();

	private final Runnable mRunnable = new Runnable() {
		@Override
		public void run() {
			// displayConnectionSummary();
			// displayDocUsage();
			mHandler.postDelayed(this, MILLSECOND_FOR_REFRESH_INFO);
		}
	};

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		addPreferencesFromResource(R.xml.preferences);
		mChangePCPref = findPreference(KEY_CHANGE_PC);
		mChangePCPref.setSummary(this.getString(R.string.setting_current)+ServersLogic.getCurrentBackupedServerName(this));
		
		// ABOUT
		// SUPPORT
		mSupportPref = findPreference(KEY_SUPPORT);
		mSupportPref.setSummary("");
		// VERSION
		try {
			findPreference(KEY_VERSION)
					.setSummary(
							getPackageManager().getPackageInfo(getPackageName(), 0).versionName
					+ "(Build:"
						+ getPackageManager().getPackageInfo(getPackageName(), 0).versionCode
				     + ")");
		} catch (NameNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		mHandler.postDelayed(mRunnable, MILLSECOND_FOR_REFRESH_INFO);
	}

	@Override
	public void onDestroy() {
		super.onDestroy();
		if (mHandler != null && mRunnable != null) {
			mHandler.removeCallbacks(mRunnable);
		}
	}

	@Override
	protected void onStart() {
		super.onStart();
	}

	@Override
	protected void onStop() {
		super.onStop();
	}

	@Override
	public void onResume() {
		super.onDestroy();
		mCanBackPress = true;
	}

	@Override
	public boolean onPreferenceTreeClick(PreferenceScreen preferenceScreen,
			final Preference preference) {
		if (preference.getKey().equals(KEY_CHANGE_PC)) {
			if(ServersLogic.hasBackupedServers(this)){
				openDialog(this);
			}

		} else if (preference.getKey().equals(KEY_SUPPORT)) {
			mSupportPref.setEnabled(false);
			mCanBackPress = false;
			Intent intent = new Intent(Intent.ACTION_SENDTO,
					Uri.parse("mailto:"+this.getString(R.string.support_email)));
			startActivity(intent);
			mSupportPref.setEnabled(true);
		}
		return true;
	}

	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		switch (requestCode) {
		case REQUEST_CHANGE_PC:
			mChangePCPref.setSummary(this.getString(R.string.setting_current)+ServersLogic.getCurrentBackupedServerName(this));
			break;
		}
	}

	@Override
	public void onBackPressed() {
		finish();
//		if (mCanBackPress) {
//			super.onBackPressed();
//		} else {
//			Log.d(TAG, "CANN'T BACK TO TIMELINE");
//		}
	}

	@Override
	public void onSharedPreferenceChanged(SharedPreferences arg0, String arg1) {
		// TODO Auto-generated method stub

	}
	private void openDialog(Context context){
		String title = context.getString(R.string.setting_change_pc);
		String message = context.getString(R.string.add_pc_content);
		
		AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(context); 
		// set title
		alertDialogBuilder.setTitle(title);
		// set dialog message
		alertDialogBuilder
			.setMessage(message)
			.setCancelable(false)
			.setPositiveButton(R.string.btn_yes,new DialogInterface.OnClickListener() {
				public void onClick(DialogInterface dialog,int id) {
					choseAnewPC();
					dialog.cancel();
				}
			  })
			.setNegativeButton(R.string.btn_no,new DialogInterface.OnClickListener() {
				public void onClick(DialogInterface dialog,int id) {
					dialog.cancel();
				}
			  }); 
				
		alertDialogBuilder.create().show();
	}

	private void choseAnewPC(){
        Intent startIntent = new Intent(this, AddServerActivity.class);	                    	
        startActivityForResult(startIntent, Constant.REQUEST_CODE_ADD_SERVER);							
	}
}
