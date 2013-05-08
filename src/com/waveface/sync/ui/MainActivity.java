package com.waveface.sync.ui;

import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentTransaction;
import android.text.TextUtils;
import android.util.Log;
import android.view.KeyEvent;

import com.waveface.sync.R;
import com.waveface.sync.RuntimeState;
import com.waveface.sync.ui.fragment.FragmentBase;
import com.waveface.sync.ui.fragment.SlidingMenuFragment;
import com.waveface.sync.ui.fragment.SyncFragment;
import com.waveface.sync.ui.preference.Preferences;
import com.waveface.sync.ui.widget.SlidingMenu;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 * 
 * @see SystemUiHider
 */
public class MainActivity extends FragmentActivity {
	private String TAG = MainActivity.class.getSimpleName();
	private SlidingMenu mSlidingMenu;
	private String mCurrentFragmentName = null;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		Log.d(TAG, "onCreate");
		setContentView(R.layout.activity_main);
		RuntimeState.isAppLaunching = true;
		
		if(savedInstanceState == null) {
			FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
			SlidingMenuFragment menu = new SlidingMenuFragment();
			transaction.add(R.id.container_sliding_menu, menu).commit();

			SyncFragment photoJournal = new SyncFragment();
			transaction = getSupportFragmentManager().beginTransaction();
			transaction.add(R.id.container_content, photoJournal, SyncFragment.class.getSimpleName()).commit();
			mCurrentFragmentName = SyncFragment.class.getSimpleName();
		}
        

        mSlidingMenu = (SlidingMenu) findViewById(R.id.sliding_menu);
		
        getWindow().setBackgroundDrawable(null);
	}
	
	@Override
	public void onBackPressed() {
		super.onBackPressed();
    	if(mSlidingMenu.isMenuShowing())
    		mSlidingMenu.showContent();
    	else if(TextUtils.isEmpty(mCurrentFragmentName) == false) {
    		Fragment fragment = getSupportFragmentManager().findFragmentByTag(mCurrentFragmentName);
    		if(fragment != null && fragment instanceof FragmentBase) {
    			((FragmentBase)fragment).onBackPressed();
    		}
    	}
	}
	
	@Override
	protected void onDestroy() {
		super.onDestroy();
		RuntimeState.isAppLaunching = false;
	}
    
	@Override
	public boolean onKeyDown(int keycode, KeyEvent e) {
	    switch(keycode) {
	        case KeyEvent.KEYCODE_MENU:
 		    	Intent intent = new Intent(this,Preferences.class);
 		    	startActivity(intent);
	            return true;
	    }

	    return super.onKeyDown(keycode, e);
	}}
