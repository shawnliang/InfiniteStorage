package com.waveface.favoriteplayer.service;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Timer;
import java.util.TimerTask;

import org.jwebsocket.kit.WebSocketException;

import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.database.Cursor;
import android.net.wifi.WifiManager;
import android.os.AsyncTask;
import android.os.IBinder;
import android.text.TextUtils;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.RuntimeState;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.db.LabelTable;
import com.waveface.favoriteplayer.entity.ConnectForGTVEntity;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.event.WebSocketEvent;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.util.DeviceUtil;
import com.waveface.favoriteplayer.util.Log;
import com.waveface.favoriteplayer.util.NetworkUtil;
import com.waveface.favoriteplayer.websocket.RuntimeWebClient;
import com.waveface.jmdns.JMDNS;
import com.waveface.jmdns.ServiceEvent;
import com.waveface.jmdns.ServiceInfo;
import com.waveface.jmdns.ServiceListener;

import de.greenrobot.event.EventBus;


public class PlayerService extends Service{
	private static final String TAG = PlayerService.class.getSimpleName();
	private Context mContext;

    private long mMDNSSetupTime;
    
	//DATA 
	private ArrayList<ServerEntity> mPairedServers ;
	private String mCondidateServerId ;
	private String mCondidateWSLocation ;

	//TIMER
    private final int UPDATE_INTERVAL = 20 * 1000;

    private Timer SyncTimer = null;

    //TEST FOR MDNS
	private WifiManager.MulticastLock mLock;
	private JMDNS mJMDNS = null;
    private ServiceListener mListener = null;

    
	@Override
	public IBinder onBind(Intent intent) {
		return null;
	}

	@Override
	public int onStartCommand(Intent intent, int flags, int startId) {		
		if(SyncTimer!=null){
			return Service.START_NOT_STICKY;
		}
		SyncTimer = new Timer();
        SyncTimer.scheduleAtFixedRate(new TimerTask() {
            @Override
            public void run() {
            	if(RuntimeState.OnWebSocketOpened == false){
            		connectPCWithPairedServer();
            		autoPairingConnect();
            	}
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
            	//
            	if(NetworkUtil.isWifiNetworkAvailable(mContext)){
            		sendSubcribe();
            	}            	
            }
        }, 0, UPDATE_INTERVAL);
	    return Service.START_NOT_STICKY;
	}	
	
	@Override
	public void onCreate() {
		mContext = getApplicationContext();

		IntentFilter filter = new IntentFilter();
		filter.addAction(Constant.ACTION_NETWORK_STATE_CHANGE);	
		registerReceiver(mReceiver, filter);
		
		connectPCWithPairedServer();
		Log.d(TAG,"Wi-Fi-Network:"+NetworkUtil.isWifiNetworkAvailable(mContext));		
		RuntimeState.mAutoConnectMode = ServersLogic.hasBackupedServers(this);		
		new SetupMDNS().execute(new Void[]{});
//		setupMDNS();
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

 		if(SyncTimer!=null){
			SyncTimer.cancel();
		}
		releaseMDNS();  
		ServersLogic.purgeAllBonjourServer(this);
    	ServersLogic.updateAllBackedServerStatus(this,Constant.SERVER_OFFLINE);
		Log.d(TAG, "onDestroy");
		super.onDestroy();
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
            mJMDNS = JMDNS.create();
            mJMDNS.addServiceListener(Constant.INFINTE_STORAGE,	mListener = new ServiceListener() {
                @SuppressWarnings("deprecation")
				@Override
                public void serviceResolved(ServiceEvent ev) {
                	ServiceInfo si = ev.getInfo();
                	if(!TextUtils.isEmpty(si.getPropertyString(Constant.PARAM_SERVER_ID))){
    	                final ServerEntity entity = new ServerEntity();
    	            	entity.serverName = si.getName();
    	            	entity.ip = si.getHostAddress();
    					entity.serverId = si.getPropertyString(Constant.PARAM_SERVER_ID);
    					entity.wsPort = si.getPropertyString(Constant.PARAM_WS_PORT);
    					entity.notifyPort = si.getPropertyString(Constant.PARAM_NOTIFY_PORT);
    					entity.restPort = si.getPropertyString(Constant.PARAM_REST_PORT);
    	                entity.wsLocation = "ws://"+si.getHostAddress()+":"+si.getPort();
    	                Log.d(TAG, "Resolved SERVER NAME:"+entity.serverName);
    	                ServersLogic.updateBonjourServer(mContext, entity);
    	        		mPairedServers = ServersLogic.getPairedServer(mContext);
    	        		if(mPairedServers.size()!=0){
    	        			RuntimeState.mAutoConnectMode = true ;
    	        		}
	                	if(NetworkUtil.isWifiNetworkAvailable(mContext) 
	                			&& RuntimeState.OnWebSocketOpened == false){
		                	if(RuntimeState.mAutoConnectMode){
		                		if(ServersLogic.canPairedServers(mContext, entity.serverId)){
	    	                		Intent intent = new Intent(Constant.ACTION_BONJOUR_SERVER_AUTO_PAIRING);
	    		                	intent.putExtra(Constant.EXTRA_BONJOUR_AUTO_PAIR_STATUS, Constant.BONJOUR_PAIRING);
	    		                	mContext.sendBroadcast(intent);
	    		                	autoPairingConnect();
			                	}
		                	}
	                	}
                	}
                }

                @Override
                public void serviceRemoved(ServiceEvent ev) {
                	ServiceInfo si = ev.getInfo();
    				//CHECK IF CONNECTING SERVER
    				String serverId = si.getPropertyString(Constant.PARAM_SERVER_ID);
    				Log.d(TAG, "Remove SERVER NAME:"+si.getName());
    				if(serverId.equals(RuntimeState.mWebSocketServerId) && RuntimeState.OnWebSocketOpened == false){
    					RuntimeState.setServerStatus(Constant.BS_ACTION_SERVER_REMOVED);
    					ServersLogic.updateBackupedServerStatus(mContext, serverId, Constant.SERVER_OFFLINE);
    					ServersLogic.purgeBonjourServerByServerId(mContext, serverId);
    				}
                }

                @Override
                public void serviceAdded(ServiceEvent event) {
                	mJMDNS.requestServiceInfo(event.getType(), event.getName(), 1);
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

	private void connectPCWithPairedServer() {
		if(RuntimeState.OnWebSocketOpened)
			return;
		mPairedServers = ServersLogic.getPairedServer(this);
		if(mPairedServers.size()>0){
			ServerEntity entity = mPairedServers.get(0);
			if(!TextUtils.isEmpty(entity.ip) && !TextUtils.isEmpty(entity.notifyPort)){
				//CONNECT FIRST FOR THREE TIME
				int retryCount= 0;
				while(retryCount<3 &&NetworkUtil.isWifiNetworkAvailable(mContext) && RuntimeState.OnWebSocketOpened==false){
					ServersLogic.startWSServerConnect(this, 
							"ws://"+entity.ip+":"+entity.notifyPort, 
							entity.serverId,
							entity.serverName, 
							entity.ip,
							entity.notifyPort,
							entity.restPort
							,true);
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
		if(!NetworkUtil.isWifiNetworkAvailable(mContext))
			return ;
		ServerEntity pairedServer = null;
		ServerEntity bonjourServer = null;		
		mPairedServers = ServersLogic.getPairedServer(this);
		Log.d(TAG, "START PAIRING LOOP");
		for(int i = 0 ; i < mPairedServers.size();i++){
			pairedServer = mPairedServers.get(i);
			bonjourServer = ServersLogic.getBonjourServerByServerId(this, pairedServer.serverId);
			if(bonjourServer!=null && RuntimeState.OnWebSocketOpened == false){
				mCondidateServerId = pairedServer.serverId;
				mCondidateWSLocation = "ws://"+bonjourServer.ip+":"+bonjourServer.notifyPort;
				Log.d(TAG, "PAIRING WITH "+pairedServer.serverName+","+mCondidateWSLocation);
				ServersLogic.startWSServerConnect(this, 
						mCondidateWSLocation, 
						mCondidateServerId,
						bonjourServer.serverName,
						bonjourServer.ip,
						bonjourServer.notifyPort,
						bonjourServer.restPort
						,false);
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
	
	class SetupMDNS extends AsyncTask<Void,Void,Void>{
		@Override
		protected Void doInBackground(Void... params) {
			setupMDNS();
			return null;
		}	
	}
	
	public void sendSubcribe() {
		if(RuntimeState.OnWebSocketOpened){
			Cursor cursor = LabelDB.getMAXSEQLabel(mContext);
			EventBus.getDefault().post(new WebSocketEvent(WebSocketEvent.STATUS_CONNECT));
			String labSeq ="0";
			if(cursor!=null && cursor.getCount()>0){
				cursor.moveToFirst();
				labSeq=cursor.getString(cursor.getColumnIndex(LabelTable.COLUMN_SEQ));
			}
			
			ConnectForGTVEntity connectForGTV = new ConnectForGTVEntity();
			ConnectForGTVEntity.Connect  connect = new ConnectForGTVEntity.Connect();
			connect.deviceId=DeviceUtil.id(mContext);
			connect.deviceName = DeviceUtil
					.getDeviceNameForDisplay(mContext);
			connectForGTV.setConnect(connect);
			ConnectForGTVEntity.Subscribe subscribe = new ConnectForGTVEntity.Subscribe();
			subscribe.labels=true;
			subscribe.labels_from_seq = labSeq;
			connectForGTV.setSubscribe(subscribe);
			try {
				RuntimeWebClient.send(RuntimeState.GSON.toJson(connectForGTV));
			} catch (WebSocketException e) {
				e.printStackTrace();
			}
		}
		Log.v(TAG, "exit WorkerTimerTask.run()");
	}
}
