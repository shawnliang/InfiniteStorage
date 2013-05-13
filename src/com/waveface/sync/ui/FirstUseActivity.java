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
import com.waveface.sync.ui.fragment.FragmentBase;
import com.waveface.sync.ui.fragment.ServerChooserFragment;
import com.waveface.sync.ui.fragment.FragmentBase.onFragmentChangedListener;


public class FirstUseActivity extends FragmentActivity implements onFragmentChangedListener {
	public final String TAG = FirstUseActivity.class.getSimpleName();

	/**
	 * @see android.app.Activity#onCreate(Bundle)
	 */
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		if ((getIntent().getFlags() & Intent.FLAG_ACTIVITY_BROUGHT_TO_FRONT) != 0) {
			finish();
			return;
		}
		getWindow().setBackgroundDrawable(null);
		setContentView(R.layout.activity_base);
		if (Constant.PHONE) {
			setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);
		}
		if (savedInstanceState == null) {
			ServerChooserFragment fragment = new ServerChooserFragment();
			getSupportFragmentManager().beginTransaction()
					.add(R.id.entry_main, fragment, ServerChooserFragment.class.getSimpleName()).commit();
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
		    if(fragment instanceof FragmentBase) {
		    	((FragmentBase)fragment).onBackPressed();
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
		transaction.setCustomAnimations(R.anim.slide_in_right_left,
				R.anim.slide_out_right_left, R.anim.slide_in_left_right,
				R.anim.slide_out_left_right);
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
		// TODO Auto-generated method stub
		
	}

}
