package com.waveface.favoriteplayer.ui;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.support.v4.app.FragmentTransaction;
import android.util.Log;

import com.actionbarsherlock.app.SherlockFragmentActivity;
import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.event.LabelImportedEvent;
import com.waveface.favoriteplayer.event.ServerChooseEvent;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.ui.fragment.PickserverDialogFragment;
import com.waveface.favoriteplayer.ui.fragment.SyncFragmentBase.onSyncFragmentChangedListener;
import com.waveface.favoriteplayer.ui.fragment.SyncInProgressFragment;

import de.greenrobot.event.EventBus;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 * 
 * @see SystemUiHider
 */
public class MainActivity extends SherlockFragmentActivity implements onSyncFragmentChangedListener{
	private String TAG = MainActivity.class.getSimpleName();
	private boolean mActivityLaunch = false;
	
	private Handler mHander = new Handler();
	private Runnable mShowPlaybackRunnable = new Runnable() {
		
		@Override
		public void run() {
			startActivity(new Intent(MainActivity.this, MainTabActivity.class));
			finish();
		}
	};
	
	private Runnable mReplaceContentToSyncRunnable = new Runnable() {
		@Override
		public void run() {
			showSyncInProgressFragment();
		}
		
	};
    
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		Log.d(TAG, "onCreate");
		EventBus.getDefault().register(this);
		setContentView(R.layout.activity_main);
		
		if(savedInstanceState == null) {
			
			if(ServersLogic.hasBackupedServers(this) == false) {
				PickserverDialogFragment df = new PickserverDialogFragment();
				df.show(getSupportFragmentManager(), PickserverDialogFragment.class.getSimpleName());
			} else {
				mActivityLaunch = true;
				mHander.post(mShowPlaybackRunnable);
			}
		}
        sendBroadcast(new Intent(Constant.ACTION_FAVORITE_PLAYER_ALARM));
	}
	
	public void onEvent(LabelImportedEvent event) {
		if(event.status == LabelImportedEvent.STATUS_DONE && mActivityLaunch == false) {
			mActivityLaunch = true;
			mHander.post(mShowPlaybackRunnable);
		}
	}
	
	private void showSyncInProgressFragment() {
		FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
		transaction.setCustomAnimations(android.R.anim.fade_in, android.R.anim.fade_out);
		SyncInProgressFragment photoJournal = new SyncInProgressFragment();
		transaction.add(R.id.container_content, photoJournal, SyncInProgressFragment.class.getSimpleName()).commit();
	}
	
	public void onEvent(ServerChooseEvent event) {
		mHander.post(mReplaceContentToSyncRunnable);
	}
	
	@Override
	public void onBackPressed() {
    	finish();
	}
	
	@Override
	protected void onDestroy() {
		super.onDestroy();
		EventBus.getDefault().unregister(this);
	}
	@Override
	protected void onResume() {
		super.onResume();
	}

    @Override
	protected void onPause() {
        super.onPause();
        Log.v(TAG, "pause activity");
    }

	@Override
	public void done(String id) {
	}

	@Override
	public void fail(String id) {
		
	}
}
