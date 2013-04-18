package com.waveface.sync.service;

import java.io.IOException;
import java.util.ArrayList;

import javax.jmdns.JmDNS;
import javax.jmdns.ServiceEvent;
import javax.jmdns.ServiceInfo;
import javax.jmdns.ServiceListener;

import android.app.ProgressDialog;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.net.wifi.WifiManager;
import android.os.Handler;
import android.os.IBinder;
import android.text.TextUtils;
import android.widget.Toast;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.RuntimeConfig;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.ui.LinkServerActivity;
import com.waveface.sync.ui.MainActivity;
import com.waveface.sync.util.Log;
import com.waveface.sync.util.NetworkUtil;

public class BonjourService extends Service{
	private static final String TAG = BonjourService.class.getSimpleName();
	private Context mContext;
	private WifiManager.MulticastLock lock;
	private Handler mHandler = new Handler();
	private JmDNS jmdns = null;
    private ServiceListener mListener = null;
 	
    
	private static boolean mAutoConnectMode = false;	
	private static boolean mHasPopupFirstUse = false;

	//DATA 
	private ArrayList<ServerEntity> mPairedServers ;
	private ServerEntity mConnectedServer ;
	private String mCondidateServerId ;
	private String mCondidateWSLocation ;

	@Override
	public IBinder onBind(Intent intent) {
		return null;
	}

	@Override
	public int onStartCommand(Intent intent, int flags, int startId) {
		//TODO do something useful
	    return Service.START_NOT_STICKY;
	}	
	
	@Override
	public void onCreate() {
		mContext = getApplicationContext();
		mPairedServers = ServersLogic.getBackupedServers(this);
		if(NetworkUtil.isWifiNetworkAvailable(this)){
			if(mPairedServers.size()!=0){
				mAutoConnectMode = true ;
			}
		}
		multiCastSetUp();
		Toast.makeText(this, "My Service Created", Toast.LENGTH_LONG).show();
		Log.d(TAG, "onCreate");
	}

	@Override
	public void onDestroy() {
		Toast.makeText(this, "My Service Stopped", Toast.LENGTH_LONG).show();
		Log.d(TAG, "onDestroy");
	}
	
	@Override
	public void onStart(Intent intent, int startid) {
		Toast.makeText(this, "My Service Started", Toast.LENGTH_LONG).show();
		Log.d(TAG, "onStart");
	}
    private void multiCastSetUp() {
        WifiManager wifi = (WifiManager) getSystemService(android.content.Context.WIFI_SERVICE);
        lock = wifi.createMulticastLock("infiniteS");
//        lock.setReferenceCounted(true);
        lock.acquire();
        try {
            jmdns = JmDNS.create();
            jmdns.addServiceListener(Constant.INFINTE_STORAGE, mListener = new ServiceListener() {
                @Override
                public void serviceResolved(ServiceEvent ev) {
                	@SuppressWarnings("deprecation")
                	ServiceInfo si = ev.getInfo();
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
                    if(mAutoConnectMode == false && RuntimeConfig.OnWebSocketOpened == false ){	                    
	                    if(mHasPopupFirstUse == false ){
		                    Intent intent = new Intent(Constant.ACTION_BONJOUR_SERVER_MANUAL_PAIRING);
		                    mContext.sendBroadcast(intent);
	                    }
//	                    else{
//	                    	Intent intent = new Intent(Constant.ACTION_BONJOUR_MULTICAT_EVENT);
//	                    	mContext.sendBroadcast(intent);
//	                    }
                    }
                    else{
                    	Intent intent = new Intent(Constant.ACTION_BONJOUR_SERVER_AUTO_PAIRING);
                    	mContext.sendBroadcast(intent);
                    	mHandler.postDelayed(new Runnable() {
                            public void run() {
                            	autoPairConnect();  		                    
                            	}
                        }, 500);
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
                	jmdns.requestServiceInfo(event.getType(), event.getName(), 1);
                }
            });
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
		for(int i = 0 ; i < mPairedServers.size();i++){
			pairedServer = mPairedServers.get(i);
			bonjourServer = ServersLogic.getBonjourServerByServerId(this, pairedServer.serverId);
			if(bonjourServer!=null && RuntimeConfig.OnWebSocketOpened == false){
				mCondidateServerId = pairedServer.serverId;
				mCondidateWSLocation = bonjourServer.wsLocation;
				ServersLogic.startWSServerConnect(this, mCondidateWSLocation, mCondidateServerId);
				mConnectedServer = bonjourServer;
			}
		}
		if(RuntimeConfig.OnWebSocketOpened == false){
        	Intent intent = new Intent(Constant.ACTION_BONJOUR_SERVER_AUTO_PAIRING);
        	mContext.sendBroadcast(intent);			
		}
	}
	 
}
