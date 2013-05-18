package com.waveface.sync.ui;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentTransaction;
import android.text.TextUtils;
import android.util.Log;

//import com.waveface.mdns.DNSThread;
import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.RuntimeState;
import com.waveface.sync.event.LabelImportedEvent;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.ui.fragment.FragmentBase;
import com.waveface.sync.ui.fragment.PlaybackFragment;
import com.waveface.sync.ui.fragment.SyncFragment;
import com.waveface.sync.ui.fragment.SyncInProgressFragment;

import de.greenrobot.event.EventBus;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 * 
 * @see SystemUiHider
 */
public class MainActivity extends FragmentActivity {
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
				SyncInProgressFragment photoJournal = new SyncInProgressFragment();
				transaction = getSupportFragmentManager().beginTransaction();
				transaction.add(R.id.container_content, photoJournal, SyncInProgressFragment.class.getSimpleName()).commit();
				mCurrentFragmentName = SyncInProgressFragment.class.getSimpleName();
			}
		}
		
        getWindow().setBackgroundDrawable(null);
	}
	
	public void onEvent(LabelImportedEvent event) {
		if(event.status == LabelImportedEvent.STATUS_DONE) {
			mHander.post(mShowPlaybackRunnable);
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

}