package com.waveface.sync.service;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Timer;
import java.util.TimerTask;

import javax.jmdns.JmDNS;
import javax.jmdns.ServiceEvent;
import javax.jmdns.ServiceInfo;
import javax.jmdns.ServiceListener;

import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.net.wifi.WifiManager;
import android.os.IBinder;
import android.text.TextUtils;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.RuntimeState;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.logic.BackupLogic;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.util.Log;
import com.waveface.sync.util.NetworkUtil;
import com.waveface.sync.util.SyncNotificationManager;

public class InfiniteService extends Service{
	private static final String TAG = InfiniteService.class.getSimpleName();
	private Context mContext;
	private static SharedPreferences mPrefs ;
	private Editor mEditor;

	private WifiManager.MulticastLock mLock;
	private JmDNS mJMDNS = null;
    private ServiceListener mListener = null;
 	
	//DATA 
	private ArrayList<ServerEntity> mPairedServers ;
	private String mCondidateServerId ;
	private String mCondidateWSLocation ;

	//TIMER
    private final int UPDATE_INTERVAL = 30 * 1000;
    private Timer timer = new Timer();
    
    //
	private SyncNotificationManager mNotificationManager;
	private String mNotoficationId;
	private boolean mBackupNotoficationCreated;
	
	
	@Override
	public IBinder onBind(Intent intent) {
		return null;
	}

	@Override
	public int onStartCommand(Intent intent, int flags, int startId) {
        timer.scheduleAtFixedRate(new TimerTask() {
            @Override
            public void run() {
            	//SCAN FILES
            	BackupLogic.scanAllFiles(mContext);            	
		    	String serverId = RuntimeState.mWebSocketServerId;
//		    	if(RuntimeState.isBackuping){
//		    		displaySyncInfo(false);
//		    	}
            	if(!TextUtils.isEmpty(serverId)
            			&& RuntimeState.canBackup(mContext)
        				&& BackupLogic.needToBackup(mContext,serverId)){
					Log.d(TAG, "START BACKUP FILE");
					RuntimeState.setServerStatus(Constant.WS_ACTION_START_BACKUP);
					ServersLogic.updateCount(mContext, serverId);
					removeNotification();
					displaySyncInfo(false);
					while(RuntimeState.isWebSocketAvaliable(mContext) && BackupLogic.needToBackup(mContext,serverId)){
			    		BackupLogic.backupFiles(mContext, serverId);
			    	}
					Intent intent = new Intent(Constant.ACTION_BACKUP_DONE);
					mContext.sendBroadcast(intent);
					RuntimeState.setServerStatus(Constant.WS_ACTION_END_BACKUP);
					removeNotification();
					displaySyncInfo(true);
            	}
				// Check if there are updates here and notify if true
            }
        }, 0, UPDATE_INTERVAL);
	    return Service.START_NOT_STICKY;
	}	
	
	@Override
	public void onCreate() {
		mContext = getApplicationContext();
		mPrefs = mContext.getSharedPreferences(
				Constant.PREFS_NAME, Context.MODE_PRIVATE);
		mEditor = mPrefs.edit();

		IntentFilter filter = new IntentFilter();
		filter.addAction(Constant.ACTION_NETWORK_STATE_CHANGE);
		registerReceiver(mReceiver, filter);

		mPairedServers = ServersLogic.getBackupedServers(this);
		if(mPairedServers.size()!=0){
			RuntimeState.mAutoConnectMode = true ;
		}
		multiCastSetUp();
		//Notification 
		mNotificationManager = SyncNotificationManager
				.getInstance(getApplicationContext());
		//removeNotification(mContext);
		Log.d(TAG, "onCreate");
	}

	private void displaySyncInfo(boolean backupedCompleted) {
		mNotoficationId = mPrefs.getString(Constant.PREF_NOTIFICATION_ID, "");
		if(TextUtils.isEmpty(mNotoficationId)){
			mNotoficationId = System.currentTimeMillis()+"";
			mEditor.putString(Constant.PREF_NOTIFICATION_ID, mNotoficationId);
			mEditor.commit();
		}
		String content = null;
		if(!backupedCompleted && RuntimeState.OnWebSocketStation ){
			content = mContext.getString(R.string.notify_link_server,RuntimeState.mWebSocketServerName);
			mNotificationManager.createProgressNotification(mNotoficationId, content, content, 90);
			int[] backupAndTotalCount = BackupLogic.getBackupProgressInfo(mContext, RuntimeState.mWebSocketServerId);
			int progress = (int) ((backupAndTotalCount[0])/ (float) backupAndTotalCount[1] * 100);
			content += "("+backupAndTotalCount[0]+"/"+backupAndTotalCount[1]+"),"+progress+"%";
			
			if(!mBackupNotoficationCreated){
				mNotificationManager.createProgressNotification(
					mNotoficationId,
					mContext.getString(R.string.app_name),
					content,progress);
				mBackupNotoficationCreated = true ;
			}
			else{
				mNotificationManager.updateProgressNotification(mNotoficationId,
						content,progress);
			}

		}

		if(RuntimeState.isNotificationShowing == false ){
			if(backupedCompleted ){
				mBackupNotoficationCreated = false;
				int count = ServersLogic.getServerBackupedCountById(mContext, RuntimeState.mWebSocketServerId);		
				content = mContext.getString(R.string.notify_backup_status, count);				
				mNotificationManager.createTextNotification(
						mNotoficationId,
						mContext.getString(R.string.app_name),
						content,null);
			}
			RuntimeState.isNotificationShowing = true ;
		}	
	}
	
	private void removeNotification(){
		if(mContext == null){
			return;
		}
		if(mPrefs==null){
			mPrefs = mContext.getSharedPreferences(
					Constant.PREFS_NAME, Context.MODE_PRIVATE);
		}
		if(mNotificationManager == null){
			mNotificationManager = SyncNotificationManager
					.getInstance(mContext);
		}
		String notificationId = mPrefs.getString(Constant.PREF_NOTIFICATION_ID, "");
		if(!TextUtils.isEmpty(notificationId) && RuntimeState.isNotificationShowing){
			mNotificationManager.cancelNotification(notificationId);
			//mNotificationManager.cancelAll();
			RuntimeState.isNotificationShowing = false;
		}

//		mNotificationManager.cancelAll();
	}

	private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			String action = intent.getAction();
			Log.d(TAG, "action:" + intent.getAction());
			if (Constant.ACTION_NETWORK_STATE_CHANGE.equals(action)) {
				String actionContent = intent.getStringExtra(Constant.EXTRA_NETWROK_STATE);
				if(actionContent!=null){
					if(actionContent.equals(Constant.NETWORK_ACTION_BROKEN)){
						//TODO:?
					}
					else if(actionContent.equals(Constant.NETWORK_ACTION_WIFI_CONNECTED)){
						//TODO:?
					}
				}
			}
		}
	};

	@Override
	public void onDestroy() {
 		unregisterReceiver(mReceiver);
		if(timer!=null){
			timer.cancel();
		}
	   	ServersLogic.purgeAllBonjourServer(mContext);
		if (mJMDNS != null) {
	        if (mListener != null) {
	            mJMDNS.removeServiceListener(Constant.INFINTE_STORAGE, mListener);
	            mListener = null;
	        }
	        mJMDNS.unregisterAllServices();
	        try {
	            mJMDNS.close();
	        } catch (IOException e) {
	            e.printStackTrace();
	        }
	        mJMDNS = null;
		}
	    try {
	    	mLock.release();
	    } catch (Throwable th) {
	        // ignoring this exception, probably wakeLock was already released
	    }    
		Log.d(TAG, "onDestroy");
		super.onDestroy();
	}
	
	@Override
	public void onStart(Intent intent, int startid) {
		Log.d(TAG, "onStart");
	}

    private void multiCastSetUp() {
        WifiManager wifi = (WifiManager) getSystemService(android.content.Context.WIFI_SERVICE);
        mLock = wifi.createMulticastLock("infiniteS");
//        lock.setReferenceCounted(true);
        mLock.acquire();
        try {
            mJMDNS = JmDNS.create();
            mJMDNS.addServiceListener(Constant.INFINTE_STORAGE,		mListener = new ServiceListener() {
                @Override
                public void serviceResolved(ServiceEvent ev) {
                	@SuppressWarnings("deprecation")
                	ServiceInfo si = ev.getInfo();
                	if(!TextUtils.isEmpty(si.getPropertyString(Constant.PARAM_SERVER_ID))){
    	                final ServerEntity entity = new ServerEntity();
    	            	entity.serverName = si.getName();
    					entity.serverId = si.getPropertyString(Constant.PARAM_SERVER_ID);
    					if(!TextUtils.isEmpty(si.getPropertyString(Constant.PARAM_SERVER_OS))){
    						entity.serverOS = si.getPropertyString(Constant.PARAM_SERVER_OS);		
    					}
    					else{
    						entity.serverOS = "WINDOWS";
    					}
    	                entity.wsLocation = "ws://"+si.getHostAddress()+":"+si.getPort();
    	                Log.d(TAG, "SERVER NAME:"+entity.serverName);
    	                ServersLogic.updateBonjourServer(mContext, entity);
    	        		mPairedServers = ServersLogic.getBackupedServers(mContext);
    	        		if(mPairedServers.size()!=0){
    	        			RuntimeState.mAutoConnectMode = true ;
    	        		}
    	                if(RuntimeState.mAutoConnectMode == false && RuntimeState.OnWebSocketOpened == false ){	                    
    	                    Intent intent = new Intent(Constant.ACTION_BONJOUR_SERVER_MANUAL_PAIRING);
    	                    mContext.sendBroadcast(intent);
    	                }
    	                else{
    	                	if(NetworkUtil.isWifiNetworkAvailable(mContext) && RuntimeState.OnWebSocketOpened == false){
    		                	Intent intent = new Intent(Constant.ACTION_BONJOUR_SERVER_AUTO_PAIRING);
    		                	intent.putExtra(Constant.EXTRA_BONJOUR_AUTO_PAIR_STATUS, Constant.BONJOUR_PAIRING);
    		                	mContext.sendBroadcast(intent);
    		                	autoPairConnect();
    	                	}
    	                }
                	}
                }

                @Override
                public void serviceRemoved(ServiceEvent ev) {
                	ServiceInfo si = ev.getInfo();
    				//CHECK IF CONNECTING SERVER
    				String serverId = si.getPropertyString(Constant.PARAM_SERVER_ID);;
    				if(serverId.equals(RuntimeState.mWebSocketServerId)){
    					RuntimeState.setServerStatus(Constant.BS_ACTION_SERVER_REMOVED);
    					ServersLogic.updateBackupedServerStatus(mContext, serverId, Constant.SERVER_OFFLINE);
    				}
    				ServersLogic.purgeBonjourServerByServerId(mContext, serverId);
                }

                @Override
                public void serviceAdded(ServiceEvent event) {
                	//TODO:
                	mJMDNS.requestServiceInfo(event.getType(), event.getName(), 1);
                }
            });
        } 
        catch (IOException e) {
            e.printStackTrace();
            return;
        }
    }
	private void autoPairConnect(){
		ServerEntity pairedServer = null;
		ServerEntity bonjourServer = null;		
		mPairedServers = ServersLogic.getBackupedServers(this);
		for(int i = 0 ; i < mPairedServers.size();i++){
			pairedServer = mPairedServers.get(i);
			bonjourServer = ServersLogic.getBonjourServerByServerId(this, pairedServer.serverId);
			if(bonjourServer!=null && RuntimeState.OnWebSocketOpened == false){
				mCondidateServerId = pairedServer.serverId;
				mCondidateWSLocation = bonjourServer.wsLocation;
				ServersLogic.startWSServerConnect(this, mCondidateWSLocation, mCondidateServerId);
				while(RuntimeState.OnWebSocketOpened){
					try {
						Thread.sleep(50);
					} catch (InterruptedException e) {
						e.printStackTrace();
					}
				}
			}
			if(RuntimeState.OnWebSocketStation){
				break;
			}
		}
	}	 
}
