package com.waveface.favoriteplayer.service;

import java.io.IOException;
import java.lang.ref.WeakReference;
import java.math.BigInteger;
import java.net.Inet4Address;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.Timer;
import java.util.TimerTask;

import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.SharedPreferences;
import android.net.wifi.WifiManager;
import android.os.Handler;
import android.os.HandlerThread;
import android.os.IBinder;
import android.os.Looper;
import android.os.Message;
import android.os.Process;
import android.text.TextUtils;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.RuntimeState;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.logic.DownloadLogic;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.task.ChangeLabelsTask;
import com.waveface.favoriteplayer.task.InitDownloadLabelsTask;
import com.waveface.favoriteplayer.util.Log;
import com.waveface.favoriteplayer.util.NetworkUtil;
import com.waveface.jmdns.JMDNS;
import com.waveface.jmdns.ServiceEvent;
import com.waveface.jmdns.ServiceInfo;
import com.waveface.jmdns.ServiceListener;


public class PlayerService extends Service{
	private static final String TAG = PlayerService.class.getSimpleName();
	private Context mContext;

    private long mMDNSSetupTime = System.currentTimeMillis();
	//DATA 
	private ArrayList<ServerEntity> mPairedServers ;
	private String mCondidateServerId ;
	private String mCondidateWSLocation ;

	//TIMER
    private final int UPDATE_INTERVAL = 20 * 1000;

    private Timer SyncTimer = null;
    
    //JMDNS
	private WifiManager.MulticastLock mLock;
	private JMDNS mJMDNS = null;
    private ServiceListener mListener = null;

	private volatile ServiceHandler mServiceHandler;
	private static HandlerThread mBJThread;
	private static final int MSG_SETUP_BONJOUR = 1;
	private static final int MSG_RELEASE_BONJOUR = 2;	
	private static final int MSG_WEBSOCKET_CONNECT = 3;	

	private ServerEntity mCandidateServer = null;
	private SharedPreferences mPrefs ;
	private int mLableInitStatus = 0;

	
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
            	if(NetworkUtil.isWifiNetworkAvailable(mContext) ){
                	long fromTime = System.currentTimeMillis()-mMDNSSetupTime;
                	if(NetworkUtil.isWifiNetworkAvailable(mContext)
                			&& RuntimeState .OnWebSocketOpened == false
                			&& fromTime > (60*1000)){
                		    Log.d(TAG, "reset MDNS FOR WAIT FOR 1 Minutes");
    						finishBonjourPaired();
    						startupBonjourPaired();
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
		mLableInitStatus = mPrefs.getInt(
				Constant.PREF_DOWNLOAD_LABEL_INIT_STATUS, 0);
		IntentFilter filter = new IntentFilter();
		filter.addAction(Constant.ACTION_NETWORK_STATE_CHANGE);	
		filter.addAction(Constant.ACTION_WEB_SOCKET_SERVER_CONNECTED);
		filter.addAction(Constant.ACTION_WEB_SOCKET_SERVER_DISCONNECTED);
		filter.addAction(Constant.ACTION_LABEL_CHANGE_NOTIFICATION);
		registerReceiver(mReceiver, filter);
		
		connectPCWithPairedServer();
		RuntimeState.mAutoConnectMode = ServersLogic.hasBackupedServers(this);				
		if(RuntimeState.OnWebSocketOpened == false){
			startupBonjourPaired();
		}
		Log.d(TAG, "onCreate");		
	}

	public void startWebSocketConnect(boolean autoConnect){
		initServiceHandlerAndThread();
		try {
			if(RuntimeState.OnWebSocketOpened == false){
				Message.obtain(mServiceHandler,MSG_WEBSOCKET_CONNECT,autoConnect?1:0,0)
						.sendToTarget();
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	private void initServiceHandlerAndThread() {
		if(mBJThread==null){
			mBJThread = new HandlerThread(PlayerService.class.getSimpleName(), Process.THREAD_PRIORITY_BACKGROUND);
			mBJThread.start();
			mServiceHandler = new ServiceHandler(this, mBJThread.getLooper());
			Log.d(TAG, "start mServiceLooper");
		}
	}

	public void startupBonjourPaired(){
		initServiceHandlerAndThread();
		try {
			Message.obtain(mServiceHandler, MSG_SETUP_BONJOUR)
					.sendToTarget();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	public void finishBonjourPaired(){
		if(mBJThread!=null){
			try {
				Message.obtain(mServiceHandler, MSG_RELEASE_BONJOUR)
						.sendToTarget();
			} catch (Exception e) {
				e.printStackTrace();
			}
		}
		else{
			RuntimeState.isMDNSSetUped = false;
		}
	}

	
	private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			String action = intent.getAction();
			if (Constant.ACTION_NETWORK_STATE_CHANGE.equals(action)) {
				String actionContent = intent.getStringExtra(Constant.EXTRA_NETWROK_STATE);
				if(actionContent!=null){
					if(actionContent.equals(Constant.NETWORK_ACTION_WIFI_BROKEN)){
						if(RuntimeState.isMDNSSetUped== true){
							Log.d(TAG, "release MDNS by network broken");
							ServersLogic.purgeAllBonjourServer(mContext);
							finishBonjourPaired();
						}
					}
					else if(actionContent.equals(Constant.NETWORK_ACTION_WIFI_CONNECTED)){
						if(NetworkUtil.isWifiNetworkAvailable(mContext) && RuntimeState.isMDNSSetUped== false){
							if(ServersLogic.hasBackupedServers(mContext)){
								connectPCWithPairedServer();	
							}
							if(RuntimeState.OnWebSocketOpened == false){
								Log.d(TAG, "reset MDNS BY WI-Fi Network Recoverd");
								RuntimeState.isMDNSSetUped = false;
								startupBonjourPaired();
							}
						}
					}
					else if(actionContent.equals(Constant.ACTION_WEB_SOCKET_SERVER_DISCONNECTED)){
						//
					}
				}
			}
			else if (Constant.ACTION_WEB_SOCKET_SERVER_CONNECTED.equals(action)) {
				if(mLableInitStatus == 0){
					new InitDownloadLabelsTask(mContext).execute(new Void[]{});
				}
				DownloadLogic.subscribe(mContext);
			}else if(Constant.ACTION_LABEL_CHANGE_NOTIFICATION.equals(action)){
				mLableInitStatus = mPrefs.getInt(
						Constant.PREF_DOWNLOAD_LABEL_INIT_STATUS, 0);
				if(mLableInitStatus == 1 && RuntimeState.needToSync() == false){
					RuntimeState.setSyncing(true);
					new ChangeLabelsTask(mContext).execute(new Void[]{});
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
 		
		Log.d(TAG, "quit mServiceHandler");
		if(mServiceHandler!=null){
			mServiceHandler.getLooper().quit();
		}
		ServersLogic.purgeAllBonjourServer(this);
    	ServersLogic.updateAllBackedServerStatus(this,Constant.SERVER_OFFLINE);
		Log.d(TAG, "onDestroy");
		super.onDestroy();
	}

	
	@Override
	public void onStart(Intent intent, int startid) {
		Log.d(TAG, "onStart");
	}

	private Inet4Address getWifiIpAddressV4(WifiManager wifi) throws UnknownHostException {
    	int ip = wifi.getConnectionInfo().getIpAddress();
    	byte[] ipByte = BigInteger.valueOf(ip).toByteArray();
    	return (Inet4Address) Inet4Address.getByAddress(ipByte); 
	}
	
    private void setupMDNS() {
    	mMDNSSetupTime = System.currentTimeMillis();
        WifiManager wifi = (WifiManager) getSystemService(android.content.Context.WIFI_SERVICE);
        mLock = wifi.createMulticastLock("infiniteS");
//        lock.setReferenceCounted(true);
        mLock.acquire();
        try {
        	Inet4Address ipAddrv4 = getWifiIpAddressV4(wifi);
        	mJMDNS = JMDNS.create(ipAddrv4);
            mJMDNS.addServiceListener(Constant.INFINTE_STORAGE,	mListener = new ServiceListener() {
                @SuppressWarnings("deprecation")
				@Override
                public void serviceResolved(ServiceEvent ev) {
                	ServiceInfo si = ev.getInfo();
                	String homeSharing = "";
                	if(!TextUtils.isEmpty(si.getPropertyString(Constant.PARAM_SERVER_ID))){
    	                final ServerEntity entity = new ServerEntity();
    	            	entity.serverName = si.getName();
    	            	entity.ip = si.getHostAddress();
    					entity.serverId = si.getPropertyString(Constant.PARAM_SERVER_ID);
    	                homeSharing = si.getPropertyString(Constant.PARAM_SERVER_HOME_SHARING);
    	                if(!TextUtils.isEmpty(homeSharing) && homeSharing.equals("false")){
    	                	ServersLogic.purgeBonjourServerByServerId(mContext, entity.serverId);
    	                	return;
    	                }
    					
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
					entity.wsLocation = "ws://"+entity.ip+":"+entity.notifyPort;					
					mCandidateServer = entity;
					Log.d(TAG, "auto connect");
					startWebSocketConnect(true);	
					try {
						Thread.sleep(200);
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
				
				mCandidateServer = new ServerEntity();
				mCandidateServer.serverId = mCondidateServerId;
				mCandidateServer.wsLocation = mCondidateWSLocation;
				mCandidateServer.serverName = bonjourServer.serverName;
				mCandidateServer.ip = bonjourServer.ip;
				mCandidateServer.notifyPort = bonjourServer.notifyPort;
				mCandidateServer.restPort = bonjourServer.restPort;
				Log.d(TAG, "auto paired connect");
				startWebSocketConnect(false);
			}
		}
	}
	
	
	private final class ServiceHandler extends Handler {
		private WeakReference<PlayerService> mRef;
		public ServiceHandler(PlayerService service, Looper looper) {
			super(looper);
			mRef = new WeakReference<PlayerService>(service);
		}
		
		@Override
		public void handleMessage(Message msg){
			switch (msg.what) {
				case MSG_SETUP_BONJOUR:
					if(RuntimeState.isMDNSSetUped == false 
						&& RuntimeState.OnWebSocketOpened == false){
						setupMDNS();
					}
					break;
				case MSG_RELEASE_BONJOUR:
					if(RuntimeState.isMDNSSetUped == true){
						releaseMDNS();
					}
					break;
				case MSG_WEBSOCKET_CONNECT:					
					int autoConnect = msg.arg1;
					ServersLogic.startWSServerConnect(mContext, 
							mCandidateServer.wsLocation, 
							mCandidateServer.serverId,
							mCandidateServer.serverName,
							mCandidateServer.ip,
							mCandidateServer.notifyPort,
							mCandidateServer.restPort
							,autoConnect==1?true:false);
					break;
			}
		}
	}

}
