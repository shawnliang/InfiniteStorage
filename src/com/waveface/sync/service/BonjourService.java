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
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.net.wifi.WifiManager;
import android.os.Handler;
import android.os.IBinder;
import android.text.TextUtils;

import com.waveface.sync.Constant;
import com.waveface.sync.RuntimeConfig;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.logic.BackupLogic;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.util.Log;
import com.waveface.sync.util.NetworkUtil;

public class BonjourService extends Service{
	private static final String TAG = BonjourService.class.getSimpleName();
	private Context mContext;
	private WifiManager.MulticastLock mLock;
	private Handler mHandler = new Handler();
	private JmDNS mJMDNS = null;
    private ServiceListener mListener = null;
 	
	//DATA 
	private ArrayList<ServerEntity> mPairedServers ;
	private String mCondidateServerId ;
	private String mCondidateWSLocation ;

	//TIMER
    private final int UPDATE_INTERVAL = 10 * 1000;
    private Timer timer = new Timer();
	
	@Override
	public IBinder onBind(Intent intent) {
		return null;
	}

	@Override
	public int onStartCommand(Intent intent, int flags, int startId) {
		//TODO do something useful
        timer.scheduleAtFixedRate(new TimerTask() {
            @Override
            public void run() {
			   	SharedPreferences prefs = mContext.getSharedPreferences(Constant.PREFS_NAME, Context.MODE_PRIVATE);    	
		    	String serverId = prefs.getString(Constant.PREF_SERVER_ID, "");
            	if(BackupLogic.needToBackup(mContext,serverId) && RuntimeConfig.isBackuping == false){
					Log.d(TAG, "START BACKUP FILE");
					RuntimeConfig.isBackuping = true;
			    	while(BackupLogic.needToBackup(mContext,serverId) && BackupLogic.canBackup(mContext)){
			    		BackupLogic.backupFiles(mContext, serverId);
			    	}
					Intent intent = new Intent(Constant.ACTION_BACKUP_DONE);
					mContext.sendBroadcast(intent);
				}
				RuntimeConfig.isBackuping = false;
                // Check if there are updates here and notify if true
            }
        }, 0, UPDATE_INTERVAL);
	    return Service.START_NOT_STICKY;
	}	
	
	@Override
	public void onCreate() {
		RuntimeConfig.hasServiceCreated = true;
		mContext = getApplicationContext();
		mListener = new ServiceListener() {
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
	        			RuntimeConfig.mAutoConnectMode = true ;
	        		}
	                if(RuntimeConfig.mAutoConnectMode == false && RuntimeConfig.OnWebSocketOpened == false ){	                    
	                    Intent intent = new Intent(Constant.ACTION_BONJOUR_SERVER_MANUAL_PAIRING);
	                    mContext.sendBroadcast(intent);
	                }
	                else{
	                	if(NetworkUtil.isWifiNetworkAvailable(mContext) && RuntimeConfig.OnWebSocketOpened == false){
		                	Intent intent = new Intent(Constant.ACTION_BONJOUR_SERVER_AUTO_PAIRING);
		                	intent.putExtra(Constant.EXTRA_BONJOUR_AUTO_PAIR_STATUS, Constant.BONJOUR_PAIRING);
		                	mContext.sendBroadcast(intent);
		                	mHandler.postDelayed(new Runnable() {
		                        public void run() {
		                        	autoPairConnect();  		                    
		                        	}
		                    }, 500);
	                	}
	                }
            	}
            }

            @Override
            public void serviceRemoved(ServiceEvent ev) {
            	ServiceInfo si = ev.getInfo();
                ServerEntity entity = new ServerEntity();
            	entity.serverName = si.getName();
				entity.serverId = si.getPropertyString(Constant.PARAM_SERVER_ID);
				ServersLogic.purgeBonjourServerByServerId(mContext, entity.serverId);
            }

            @Override
            public void serviceAdded(ServiceEvent event) {
            	//TODO:
            	mJMDNS.requestServiceInfo(event.getType(), event.getName(), 1);
            }
        };
		mPairedServers = ServersLogic.getBackupedServers(this);
		if(mPairedServers.size()!=0){
			RuntimeConfig.mAutoConnectMode = true ;
		}
		multiCastSetUp();		
		Log.d(TAG, "onCreate");
	}
	
	@Override
	public void onDestroy() {
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
		RuntimeConfig.hasServiceCreated = false;
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
            mJMDNS.addServiceListener(Constant.INFINTE_STORAGE,mListener);
        } 
        catch (IOException e) {
            e.printStackTrace();
            //TODO:BROKEN PIPE
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
			if(bonjourServer!=null && RuntimeConfig.OnWebSocketOpened == false){
				mCondidateServerId = pairedServer.serverId;
				mCondidateWSLocation = bonjourServer.wsLocation;
				ServersLogic.startWSServerConnect(this, mCondidateWSLocation, mCondidateServerId);
				while(RuntimeConfig.isPairing){
					try {
						Thread.sleep(500);
					} catch (InterruptedException e) {
						e.printStackTrace();
					}
				}
			}
			if(RuntimeConfig.isPaired){
				break;
			}
		}
    	Intent intent = new Intent(Constant.ACTION_BONJOUR_SERVER_AUTO_PAIRING);
    	intent.putExtra(Constant.EXTRA_BONJOUR_AUTO_PAIR_STATUS, Constant.BONJOUR_PAIRED);
    	mContext.sendBroadcast(intent);			
	}
	 
}
