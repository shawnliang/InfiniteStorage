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
import com.waveface.sync.RuntimeState;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.logic.BackupLogic;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.util.Log;
import com.waveface.sync.util.NetworkUtil;

public class InfiniteService extends Service{
	private static final String TAG = InfiniteService.class.getSimpleName();
	private Context mContext;
	private static SharedPreferences mPrefs ;
	private Editor mEditor;

	private WifiManager.MulticastLock mLock;
	private JmDNS mJMDNS = null;
    private ServiceListener mListener = null;
    private long mMDNSSetupTime;
 	
	//DATA 
	private ArrayList<ServerEntity> mPairedServers ;
	private String mCondidateServerId ;
	private String mCondidateWSLocation ;

	//TIMER
    private final int UPDATE_INTERVAL = 30 * 1000;
    private Timer BackupTimer = null;
	
	@Override
	public IBinder onBind(Intent intent) {
		return null;
	}

	@Override
	public int onStartCommand(Intent intent, int flags, int startId) {		
		if(BackupTimer!=null){
			return Service.START_NOT_STICKY;
		}
		BackupTimer = new Timer();
        BackupTimer.scheduleAtFixedRate(new TimerTask() {
            @Override
            public void run() {
            	if(NetworkUtil.isWifiNetworkAvailable(mContext)){
                	long fromTime = System.currentTimeMillis()-mMDNSSetupTime;
                	if(NetworkUtil.isWifiNetworkAvailable(mContext)
                			&& RuntimeState.isMDNSSetUped  
                			&& fromTime > (60*1000)
                			&& RuntimeState .OnWebSocketOpened == false){
                		    Log.d(TAG, "reset MDNS FOR WAIT FOR 1 Minutes");
    						releaseMDNS();
    						Log.d(TAG, "reset MDNS");
    						setupMDNS();
                	}                	            	
            	}
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

//		connectPCWithPairedServer();
		
		RuntimeState.mAutoConnectMode = ServersLogic.hasBackupedServers(this);
		setupMDNS();
		
		Log.d(TAG, "onCreate");
	}

	
	private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			String action = intent.getAction();
			if (Constant.ACTION_NETWORK_STATE_CHANGE.equals(action)) {
				String actionContent = intent.getStringExtra(Constant.EXTRA_NETWROK_STATE);
				if(actionContent!=null){
					if(actionContent.equals(Constant.NETWORK_ACTION_WIFI_BROKEN)){
						Log.d(TAG, "release MDNS");
						ServersLogic.purgeAllBonjourServer(mContext);
						releaseMDNS();
					}
					else if(actionContent.equals(Constant.NETWORK_ACTION_WIFI_CONNECTED)){
						if(NetworkUtil.isWifiNetworkAvailable(mContext) && RuntimeState.isMDNSSetUped== false){
							Log.d(TAG, "reset MDNS");
							setupMDNS();
						}
					}
				}
			}
		}
	};

	@Override
	public void onDestroy() {
 		unregisterReceiver(mReceiver);

 		if(BackupTimer!=null){
			BackupTimer.cancel();
		}
		releaseMDNS();  
		ServersLogic.purgeAllBonjourServer(this);
    	ServersLogic.updateAllBackedServerStatus(this,Constant.SERVER_OFFLINE);
		Log.d(TAG, "onDestroy");
		super.onDestroy();
	}

	private void releaseMDNS() {
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
	    	Log.e(TAG, th.getMessage());
	        // ignoring this exception, probably wakeLock was already released
	    }
	    finally{
	    	RuntimeState.isMDNSSetUped = false;
	    }
	}
	
	@Override
	public void onStart(Intent intent, int startid) {
		Log.d(TAG, "onStart");
	}

    private void setupMDNS() {
    	mMDNSSetupTime = System.currentTimeMillis();
        WifiManager wifi = (WifiManager) getSystemService(android.content.Context.WIFI_SERVICE);
        mLock = wifi.createMulticastLock("infiniteS");
//        lock.setReferenceCounted(true);
        mLock.acquire();
        try {
            mJMDNS = JmDNS.create();
            mJMDNS.addServiceListener(Constant.INFINTE_STORAGE,		mListener = new ServiceListener() {
                @SuppressWarnings("deprecation")
				@Override
                public void serviceResolved(ServiceEvent ev) {
                	ServiceInfo si = ev.getInfo();
                	if(!TextUtils.isEmpty(si.getPropertyString(Constant.PARAM_SERVER_ID))){
    	                final ServerEntity entity = new ServerEntity();
    	            	entity.serverName = si.getName();
    					entity.serverId = si.getPropertyString(Constant.PARAM_SERVER_ID);
    					entity.ip = si.getHostAddress();    					
    					entity.wsPort = si.getPropertyString(Constant.PARAM_WS_PORT);
    					if(TextUtils.isEmpty(entity.wsPort)){
    						entity.wsPort = "";
    					}
    					entity.notifyPort = si.getPropertyString(Constant.PARAM_NOTIFY_PORT);
       					if(TextUtils.isEmpty(entity.notifyPort)){
    						entity.notifyPort = "";
    					}
    					entity.restPort = si.getPropertyString(Constant.PARAM_REST_PORT);    					
       					if(TextUtils.isEmpty(entity.restPort)){
    						entity.restPort = "";
    					}
    					entity.wsLocation = "ws://"+si.getHostAddress()+":"+entity.wsPort;
    	                Log.d(TAG, "Resolved SERVER NAME:"+entity.serverName);
    	                ServersLogic.updateBonjourServer(mContext, entity);
    	        		mPairedServers = ServersLogic.getBackupedServers(mContext);
    	        		if(mPairedServers.size()!=0){
    	        			RuntimeState.mAutoConnectMode = true ;
    	        		}
	                	if(RuntimeState.mAutoConnectMode 
	                			&& NetworkUtil.isWifiNetworkAvailable(mContext) 
	                			&& RuntimeState.OnWebSocketOpened == false){
		                	if(ServersLogic.canPairedServers(mContext, entity.serverId)){
    	                		Intent intent = new Intent(Constant.ACTION_BONJOUR_SERVER_AUTO_PAIRING);
    		                	intent.putExtra(Constant.EXTRA_BONJOUR_AUTO_PAIR_STATUS, Constant.BONJOUR_PAIRING);
    		                	mContext.sendBroadcast(intent);
    		                	autoPairingConnect();
		                	}
	                	}
//    	                }
                	}
                }

                @Override
                public void serviceRemoved(ServiceEvent ev) {
                	ServiceInfo si = ev.getInfo();
    				//CHECK IF CONNECTING SERVER
    				String serverId = si.getPropertyString(Constant.PARAM_SERVER_ID);
    				 Log.d(TAG, "Remove SERVER NAME:"+si.getName());
    				if(serverId.equals(RuntimeState.mWebSocketServerId)){
    					RuntimeState.setServerStatus(Constant.BS_ACTION_SERVER_REMOVED);
    					ServersLogic.updateBackupedServerStatus(mContext, serverId, Constant.SERVER_OFFLINE);
    				}
    				ServersLogic.purgeBonjourServerByServerId(mContext, serverId);
                }

                @Override
                public void serviceAdded(ServiceEvent event) {
                	mJMDNS.requestServiceInfo(event.getType(), event.getName(), true, 1);
//                	mJMDNS.requestServiceInfo(event.getType(), event.getName(), 1);
                }
            });
            RuntimeState.isMDNSSetUped = true;
        } 
        catch (Exception e) {
        	RuntimeState.isMDNSSetUped = false;
        	Log.e(TAG, e.getMessage());
            return;
        }
    }
	private void connectPCWithPairedServer() {
		mPairedServers = ServersLogic.getBackupedServers(this);
		if(mPairedServers.size()>0){
			ServerEntity entity = mPairedServers.get(0);
			if(!TextUtils.isEmpty(entity.ip) && !TextUtils.isEmpty(entity.notifyPort)){
				//CONNECT FIRST FOR THREE TIME
				String wsLocation ="ws://"+entity.ip+":"+entity.notifyPort;
				int retryCount= 0;
				while(retryCount<3 &&NetworkUtil.isWifiNetworkAvailable(mContext) && RuntimeState.OnWebSocketOpened==false){
					ServersLogic.startWSServerConnect(this, wsLocation, entity.serverId,entity.serverName, entity.ip,entity.notifyPort,entity.restPort);
					try {
						Thread.sleep(50);
					} catch (InterruptedException e) {
						e.printStackTrace();
					}
					retryCount++;
				}
			}
		}
	}
	private void autoPairingConnect(){
		ServerEntity pairedServer = null;
		ServerEntity bonjourServer = null;		
		mPairedServers = ServersLogic.getBackupedServers(this);
		Log.d(TAG, "START PAIRING LOOP");
		for(int i = 0 ; i < mPairedServers.size();i++){
			pairedServer = mPairedServers.get(i);
			bonjourServer = ServersLogic.getBonjourServerByServerId(this, pairedServer.serverId);
			if(bonjourServer!=null && RuntimeState.OnWebSocketOpened == false){
				mCondidateServerId = pairedServer.serverId;
				mCondidateWSLocation = bonjourServer.wsLocation;
				Log.d(TAG, "PAIRING WITH "+pairedServer.serverName+","+bonjourServer.wsLocation);
				ServersLogic.startWSServerConnect(this, mCondidateWSLocation, mCondidateServerId,bonjourServer.serverName,bonjourServer.ip,bonjourServer.notifyPort,bonjourServer.restPort);
				while(!RuntimeState.OnWebSocketOpened){
					try {
						Thread.sleep(50);
					} catch (InterruptedException e) {
						e.printStackTrace();
					}
				}
			}
		}
	}
}
