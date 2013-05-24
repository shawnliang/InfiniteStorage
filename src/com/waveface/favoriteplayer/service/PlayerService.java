package com.waveface.favoriteplayer.service;

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
import com.waveface.favoriteplayer.mdns.DNSThread;
import com.waveface.favoriteplayer.util.DeviceUtil;
import com.waveface.favoriteplayer.util.Log;
import com.waveface.favoriteplayer.util.NetworkUtil;
import com.waveface.favoriteplayer.websocket.RuntimeWebClient;

import de.greenrobot.event.EventBus;


public class PlayerService extends Service{
	private static final String TAG = PlayerService.class.getSimpleName();
	private Context mContext;

    private long mMDNSSetupTime;
    private DNSThread mDNSThread = null;
    
	//DATA 
	private ArrayList<ServerEntity> mPairedServers ;
	private String mCondidateServerId ;
	private String mCondidateWSLocation ;

	//TIMER
    private final int UPDATE_INTERVAL = 30 * 1000;
    private Timer BackupTimer = null;
    private Timer mWorkerTimer;
	private static final int WORKER_DELAY_SECONDS = 60;
	private static final int WORKER_PERIOD_SECONDS = 60;	
	
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
//    						releaseMDNS();
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

		IntentFilter filter = new IntentFilter();
		filter.addAction(Constant.ACTION_NETWORK_STATE_CHANGE);	
		registerReceiver(mReceiver, filter);
		
		connectPCWithPairedServer();
		Log.d(TAG,"Wi-Fi-Network:"+NetworkUtil.isWifiNetworkAvailable(mContext));		
		RuntimeState.mAutoConnectMode = ServersLogic.hasBackupedServers(this);		
		new SetupMDNS().execute(new Void[]{});
		Log.d(TAG, "onCreate");
		
		mWorkerTimer = new Timer();
		mWorkerTimer.schedule(new WorkerTimerTask(), 
				WORKER_DELAY_SECONDS * 1000, 
				WORKER_PERIOD_SECONDS * 1000);
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

	
	@Override
	public void onStart(Intent intent, int startid) {
		Log.d(TAG, "onStart");
	}

    private void setupMDNS() {
        if (mDNSThread != null) {
            Log.e(TAG, "DNS hread should be null!");
            mDNSThread.submitQuit();
            return;
        }
    	mDNSThread = new DNSThread(mContext);
    	mDNSThread.start();
    	mDNSThread.submitQuit();
    }
	private void releaseMDNS() {
	   	ServersLogic.purgeAllBonjourServer(mContext);
	   	if(mDNSThread!=null){
	   		mDNSThread = null;
	   	}
	}

	private void connectPCWithPairedServer() {
		if(RuntimeState.OnWebSocketOpened)
			return;
		mPairedServers = ServersLogic.getBackupedServers(this);
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
		mPairedServers = ServersLogic.getBackupedServers(this);
		Log.d(TAG, "START PAIRING LOOP");
		for(int i = 0 ; i < mPairedServers.size();i++){
			pairedServer = mPairedServers.get(i);
			bonjourServer = ServersLogic.getBonjourServerByServerId(this, pairedServer.serverId);
			if(bonjourServer!=null && RuntimeState.OnWebSocketOpened == false){
				mCondidateServerId = pairedServer.serverId;
				mCondidateWSLocation = bonjourServer.wsLocation;
				Log.d(TAG, "PAIRING WITH "+pairedServer.serverName+","+bonjourServer.wsLocation);
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
	
	class WorkerTimerTask extends TimerTask {

		@Override
		public void run() {
			Log.v(TAG, "enter WorkerTimerTask.run()");
			Log.d(TAG, "onCreateView");
			Cursor cursor = LabelDB.getMAXSEQLabel(mContext);
			EventBus.getDefault().post(new WebSocketEvent(WebSocketEvent.STATUS_CONNECT));

			String labelId = null;
			String labSeq ="0";
			if(cursor!=null && cursor.getCount()>0){
				cursor.moveToFirst();
				labelId = cursor.getString(cursor.getColumnIndex(LabelTable.COLUMN_LABEL_ID));
				labSeq=cursor.getString(cursor.getColumnIndex(LabelTable.COLUMN_SEQ));
				//send broadcast label change
				//context.sendBroadcast(new Intent(Constant.ACTION_LABEL_CHANGE));					
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
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			Log.v(TAG, "exit WorkerTimerTask.run()");
		}
	}
}
