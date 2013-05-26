package com.waveface.favoriteplayer.ui;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentTransaction;
import android.text.TextUtils;
import android.util.Log;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.event.LabelImportedEvent;
import com.waveface.favoriteplayer.event.WebSocketEvent;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.task.DownloadLabelsTask;
import com.waveface.favoriteplayer.ui.fragment.FragmentBase;
import com.waveface.favoriteplayer.ui.fragment.SyncFragment;
import com.waveface.favoriteplayer.ui.fragment.SyncFragmentBase.onSyncFragmentChangedListener;
import com.waveface.favoriteplayer.ui.fragment.SyncInProgressFragment;

import de.greenrobot.event.EventBus;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 * 
 * @see SystemUiHider
 */
public class MainActivity extends FragmentActivity implements onSyncFragmentChangedListener{
	private String TAG = MainActivity.class.getSimpleName();
	private String mCurrentFragmentName = null;
//    private DNSThread dnsThread = null;
	
	private Handler mHander = new Handler();
	private Runnable mShowPlaybackRunnable = new Runnable() {
		
		@Override
		public void run() {
			startActivity(new Intent(MainActivity.this, MainTabActivity.class));
			finish();
//			if(OverviewFragment.class.getSimpleName().equals(mCurrentFragmentName) == false) {
//				FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
//				OverviewFragment overview = new OverviewFragment();
//				transaction.replace(R.id.container_content, overview, OverviewFragment.class.getSimpleName()).commit();
//				mCurrentFragmentName = OverviewFragment.class.getSimpleName();
//			}
//			VideoFragment videos = new VideoFragment();
//			FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
//			transaction.setCustomAnimations(android.R.anim.fade_in, android.R.anim.fade_out);
//			transaction.replace(R.id.container_content, videos, VideoFragment.class.getSimpleName()).commit();
//			mCurrentFragmentName = VideoFragment.class.getSimpleName();
			
//			PlaybackFragment photoJournal = new PlaybackFragment();
//			FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
//			transaction.setCustomAnimations(android.R.anim.fade_in, android.R.anim.fade_out);
//			transaction.replace(R.id.container_content, photoJournal, PlaybackFragment.class.getSimpleName()).commit();
//			mCurrentFragmentName = PlaybackFragment.class.getSimpleName();
		}
	};
	
	private Runnable mReplaceContentToSyncRunnable = new Runnable() {
		@Override
		public void run() {
			showSyncInProgressFragment(true);
			//new DownloadLabelsTask(MainActivity.this).execute(new Void[]{});
		}
		
	};
    
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		Log.d(TAG, "onCreate");
		EventBus.getDefault().register(this);
		setContentView(R.layout.activity_main);
		
		if(savedInstanceState == null) {
			FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
			transaction.setCustomAnimations(android.R.anim.fade_in, android.R.anim.fade_out);
			
			if(ServersLogic.hasBackupedServers(this) == false) {
				SyncFragment photoJournal = new SyncFragment();
				transaction.add(R.id.container_content, photoJournal, SyncFragment.class.getSimpleName()).commit();
				mCurrentFragmentName = SyncFragment.class.getSimpleName();
			} else {				
				showSyncInProgressFragment(false);
				new DownloadLabelsTask(this).execute(new Void[]{});
			}
		}
        sendBroadcast(new Intent(Constant.ACTION_FAVORITE_PLAYER_ALARM));
	}
	
	public void onEvent(LabelImportedEvent event) {
		if(event.status == LabelImportedEvent.STATUS_DONE) {
			mHander.post(mShowPlaybackRunnable);
		}
	}
	
	private void showSyncInProgressFragment(boolean replace) {
		FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
		transaction.setCustomAnimations(android.R.anim.fade_in, android.R.anim.fade_out);
		SyncInProgressFragment photoJournal = new SyncInProgressFragment();
		if(replace) {
			transaction.replace(R.id.container_content, photoJournal, SyncInProgressFragment.class.getSimpleName()).commit();
		} else {
			transaction.add(R.id.container_content, photoJournal, SyncInProgressFragment.class.getSimpleName()).commit();
		}
		mCurrentFragmentName = SyncInProgressFragment.class.getSimpleName();
	}
	
	public void onEvent(WebSocketEvent event) {
		if(event.status == WebSocketEvent.STATUS_CONNECT) {
			if(SyncFragment.class.getSimpleName().equals(mCurrentFragmentName)) {
				// since sync fragment is opening first use activity, can't do anything
			} else if(SyncInProgressFragment.class.getSimpleName().equals(mCurrentFragmentName)){
				//new DownloadLabelsTask(this).execute(new Void[]{});
			}
		}
	}
	
	@Override
	public void onBackPressed() {
		super.onBackPressed();
    	if(TextUtils.isEmpty(mCurrentFragmentName) == false) {
    		Fragment fragment = getSupportFragmentManager().findFragmentByTag(mCurrentFragmentName);
    		if(fragment != null && fragment instanceof FragmentBase) {
    			((FragmentBase)fragment).onBackPressed();
    		}
    	}
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
		if(SyncFragment.class.getSimpleName().equals(mCurrentFragmentName)) {
			mHander.post(mReplaceContentToSyncRunnable);
		}
	}

	@Override
	public void fail(String id) {
		
	}
}
