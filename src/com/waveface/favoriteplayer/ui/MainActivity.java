package com.waveface.favoriteplayer.ui;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentTransaction;
import android.text.TextUtils;
import android.util.Log;
import android.view.KeyEvent;

//import com.waveface.mdns.DNSThread;
import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.RuntimeState;
import com.waveface.favoriteplayer.event.DispatchKeyEvent;
import com.waveface.favoriteplayer.event.LabelImportedEvent;
import com.waveface.favoriteplayer.event.WebSocketEvent;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.task.DownloadLabelsTask;
import com.waveface.favoriteplayer.ui.fragment.FragmentBase;
import com.waveface.favoriteplayer.ui.fragment.PlaybackFragment;
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
			PlaybackFragment photoJournal = new PlaybackFragment();
			FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
			transaction = getSupportFragmentManager().beginTransaction();
			transaction.replace(R.id.container_content, photoJournal, PlaybackFragment.class.getSimpleName()).commit();
			mCurrentFragmentName = PlaybackFragment.class.getSimpleName();
		}
	};
	
	private Runnable mReplaceContentToSyncRunnable = new Runnable() {
		@Override
		public void run() {
			showSyncInProgressFragment(true);
			new DownloadLabelsTask(MainActivity.this).execute(new Void[]{});
		}
		
	};
    
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		Log.d(TAG, "onCreate");
		EventBus.getDefault().register(this);
		sendBroadcast(new Intent(Constant.ACTION_INFINITE_STORAGE_ALARM));
		setContentView(R.layout.activity_main);
		RuntimeState.isAppLaunching = true;
		
		if(savedInstanceState == null) {
			FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
			
			if(ServersLogic.hasBackupedServers(this) == false) {
				SyncFragment photoJournal = new SyncFragment();
				transaction = getSupportFragmentManager().beginTransaction();
				transaction.add(R.id.container_content, photoJournal, SyncFragment.class.getSimpleName()).commit();
				mCurrentFragmentName = SyncFragment.class.getSimpleName();
			} else {

				new DownloadLabelsTask(this).execute(new Void[]{});
				
				showSyncInProgressFragment(false);
			}
		}
		
        getWindow().setBackgroundDrawable(null);
        sendBroadcast(new Intent(Constant.ACTION_INFINITE_STORAGE_ALARM));
	}
	
	public void onEvent(LabelImportedEvent event) {
		if(event.status == LabelImportedEvent.STATUS_DONE) {
			mHander.post(mShowPlaybackRunnable);
		}
	}
	
	private void showSyncInProgressFragment(boolean replace) {
		FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
		SyncInProgressFragment photoJournal = new SyncInProgressFragment();
		transaction = getSupportFragmentManager().beginTransaction();
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
				new DownloadLabelsTask(this).execute(new Void[]{});
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
		RuntimeState.isAppLaunching = false;
		EventBus.getDefault().unregister(this);
	}
	@Override
	protected void onResume() {
		super.onResume();
//       if (dnsThread != null) {
//            Log.e(TAG, "DNS hread should be null!");
//            dnsThread.submitQuit();
//        }
//    	dnsThread = new DNSThread(this);
//    	dnsThread.start();
	
	}

    @Override
	protected void onPause() {
        super.onPause();
        Log.v(TAG, "pause activity");
                
//        if (dnsThread == null) {
//            Log.e(TAG, "netThread should not be null!");
//            return;
//        }
//        dnsThread.submitQuit();
//        dnsThread = null;
    }

	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {
		Log.d(TAG, "onKeyDown:"  + keyCode);
		EventBus.getDefault().post(new DispatchKeyEvent(keyCode));
		return true;
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
