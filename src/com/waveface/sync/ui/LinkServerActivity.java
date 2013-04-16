package com.waveface.sync.ui;


import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.ui.LinkFragmentBase.onLoginFragmentChangedListener;


public class LinkServerActivity extends FragmentActivity implements
		LinkFragment.InstallFragmentListener, onLoginFragmentChangedListener {
	public final String TAG = LinkServerActivity.class.getSimpleName();

	/**
	 * @see android.app.Activity#onCreate(Bundle)
	 */
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		if ((getIntent().getFlags() & Intent.FLAG_ACTIVITY_BROUGHT_TO_FRONT) != 0) {
			// Activity was brought to front and not created,
			// Thus finishing this will get us to the last viewed activity
			finish();
			return;
		}
		getWindow().setBackgroundDrawable(null);
		setContentView(R.layout.entry_activity);
		if (Constant.PHONE) {
			setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);
		}
		if (savedInstanceState == null) {
			LinkFragment fragment = new LinkFragment();
			getSupportFragmentManager().beginTransaction()
					.add(R.id.entry_main, fragment, LinkFragment.class.getSimpleName()).commit();
		}
	}


	@Override
	public void onBackPressed() {
		int index = getSupportFragmentManager().getBackStackEntryCount()-1;
		if(index == -1) {
			super.onBackPressed();
		} else {
			FragmentManager.BackStackEntry backEntry=getSupportFragmentManager().getBackStackEntryAt(index);
		    String str=backEntry.getName();
		    Fragment fragment=getSupportFragmentManager().findFragmentByTag(str);
		    if(fragment instanceof LinkFragmentBase) {
		    	((LinkFragmentBase)fragment).onBackPressed();
		    } else {
				super.onBackPressed();
		    }
		}
	}

	private void showFragment(Fragment fragment, String tag, Bundle data) {
		if(data != null)
			fragment.setArguments(data);
		FragmentTransaction transaction = getSupportFragmentManager()
				.beginTransaction();
//		transaction.setCustomAnimations(R.anim.slide_in_right_left,
//				R.anim.slide_out_right_left, R.anim.slide_in_left_right,
//				R.anim.slide_out_left_right);
		transaction.replace(R.id.entry_main, fragment, tag);
		transaction.addToBackStack(tag);
		transaction.commit();
	}

	private void showFragment(Fragment fragment, String tag) {
		showFragment(fragment, tag, null);
	}

	@Override
	public void goBack(String id) {
		if (getSupportFragmentManager().getBackStackEntryCount() > 0) {
			getSupportFragmentManager().popBackStack();
		}
	}

	@Override
	public void goNext(String id) {
		if (WSServerFragment.class.getSimpleName().equals(id)) {
			showFragment(new BackupViewFragment(), BackupViewFragment.class.getSimpleName());
		}
		else if (BackupViewFragment.class.getSimpleName().equals(id)) {
			setResult(RESULT_OK);
			finish();
		}
		
	}

	@Override
	public void onSendEmail() {
		// TODO :Send Email
		
	}


	@Override
	public void onInstallNext() {
		showFragment(new WSServerFragment(), WSServerFragment.class.getSimpleName());
	}
}
