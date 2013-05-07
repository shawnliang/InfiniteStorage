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
import android.database.ContentObserver;
import android.net.wifi.WifiManager;
import android.os.Handler;
import android.os.IBinder;
import android.provider.MediaStore;
import android.text.TextUtils;
import android.widget.Toast;

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
	
	//Observers
	private ImageTableObserver mCamera;
	private VideoTableObserver mVideo;
	private AudioTableObserver mAudio;
	
	@Override
	public IBinder onBind(Intent intent) {
		return null;
	}

	@Override
	public int onStartCommand(Intent intent, int flags, int startId) {
		if(timer==null){
			timer = new Timer();
		}
        timer.scheduleAtFixedRate(new TimerTask() {
            @Override
            public void run() {
        		//SCAN ALL FILES FOR THE FIRST TIME
            	if(RuntimeState.wasFirstTimeImportScanDone == false){
            		BackupLogic.scanAllFiles(mContext);
            		RuntimeState.wasFirstTimeImportScanDone = true ;
            		mPrefs.edit()
            			.putBoolean(Constant.PREF_FILE_IMPORT_FIRST_TIME_DONE, 
            					RuntimeState.wasFirstTimeImportScanDone)
            			.commit();
            	}
		    	String serverId = RuntimeState.mWebSocketServerId;
            	if(!TextUtils.isEmpty(serverId)
            			&& RuntimeState.wasFirstTimeImportScanDone
            			&& RuntimeState.canBackup(mContext) 
        				&& BackupLogic.needToBackup(mContext,serverId)){
					Log.d(TAG, "START BACKUP FILE");
					RuntimeState.setServerStatus(Constant.WS_ACTION_START_BACKUP);
					ServersLogic.updateCount(mContext, serverId);
					showSyncNotification(Constant.NOTIFICATION_BACK_UP_START);
					while(RuntimeState.isWebSocketAvaliable(mContext) && BackupLogic.needToBackup(mContext,serverId)){
						if(RuntimeState.isBackuping==false){
							Intent intent = new Intent(Constant.ACTION_UPLOADING_FILE);
							intent.putExtra(Constant.EXTRA_BACKING_UP_FILE_STATE, Constant.JOB_START);
							mContext.sendBroadcast(intent);
							RuntimeState.isBackuping = true;
						}
			    		BackupLogic.backupFiles(mContext, serverId);
			    	}
					RuntimeState.isBackuping = false;
					Intent intent = new Intent(Constant.ACTION_BACKUP_DONE);
					mContext.sendBroadcast(intent);
					RuntimeState.setServerStatus(Constant.WS_ACTION_END_BACKUP);
					showSyncNotification(Constant.NOTIFICATION_BACKED_UP);
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
		filter.addAction(Constant.ACTION_BACKUP_FILE);
		registerReceiver(mReceiver, filter);

		RuntimeState.mAutoConnectMode = ServersLogic.hasBackupedServers(this);
		setupMDNS();
		
		//Notification 
		mNotificationManager = SyncNotificationManager
				.getInstance(getApplicationContext());
		
		// register camera observer
		BackupLogic.setLastMediaSate(getApplicationContext());
		mCamera = new ImageTableObserver(new Handler());
	    getContentResolver().registerContentObserver(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, true, mCamera);
	    mVideo = new VideoTableObserver(new Handler());
	    getContentResolver().registerContentObserver(MediaStore.Video.Media.EXTERNAL_CONTENT_URI, true, mVideo);
	    mAudio = new AudioTableObserver(new Handler());
	    getContentResolver().registerContentObserver(MediaStore.Audio.Media.EXTERNAL_CONTENT_URI, true, mAudio);
		//SCAN ALL FILES FOR THE FIRST TIME
		RuntimeState.wasFirstTimeImportScanDone = 
				mPrefs.getBoolean(Constant.PREF_FILE_IMPORT_FIRST_TIME_DONE, false);
		Log.d(TAG, "onCreate");
	}

	private void showSyncNotification(int notifyCode) {
		mNotoficationId = mPrefs.getString(Constant.PREF_NOTIFICATION_ID, "");
		if(TextUtils.isEmpty(mNotoficationId)){
			mNotoficationId = System.currentTimeMillis()+"";
			mEditor.putString(Constant.PREF_NOTIFICATION_ID, mNotoficationId);
			mEditor.commit();
		}
		String content = null;
		int[] backupAndTotalCount = null;
		int progress = 0;
		if(notifyCode== Constant.NOTIFICATION_BACK_UP_START 
				|| notifyCode==  Constant.NOTIFICATION_BACK_UP_ON_PROGRESS){
			content = mContext.getString(R.string.notify_link_server,RuntimeState.mWebSocketServerName);
			backupAndTotalCount = BackupLogic.getBackupProgressInfo(mContext, RuntimeState.mWebSocketServerId);
			progress = (int) ((backupAndTotalCount[0])/ (float) backupAndTotalCount[1] * 100);
			content += " ( "+backupAndTotalCount[0]+" / "+backupAndTotalCount[1]+" ) , "+progress+"%";			
		}
		
		switch(notifyCode){
			case Constant.NOTIFICATION_BACK_UP_START:
				if(RuntimeState.OnWebSocketStation ){
					removeNotification();					
					mNotificationManager.createProgressNotification(
						mNotoficationId,
						mContext.getString(R.string.app_name),
						content,progress);
					RuntimeState.isNotificationShowing = true ;
				}

				break;
			case Constant.NOTIFICATION_BACK_UP_ON_PROGRESS:
				if(RuntimeState.OnWebSocketStation ){
					mNotificationManager.updateProgressNotification(mNotoficationId,
								content,progress);
					RuntimeState.isNotificationShowing = true ;
				}
				break;
			case Constant.NOTIFICATION_BACKED_UP:
				removeNotification();
				if(RuntimeState.isNotificationShowing == false ){
					int count = ServersLogic.getServerBackupedCountById(mContext, RuntimeState.mWebSocketServerId);		
					content = mContext.getString(R.string.notify_backup_status, count);				
					mNotificationManager.createTextNotification(
							mNotoficationId,
							mContext.getString(R.string.app_name),
							content,null);					
					RuntimeState.isNotificationShowing = true ;
				}	
				break;
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
						releaseMDNS();
					}
					else if(actionContent.equals(Constant.NETWORK_ACTION_WIFI_CONNECTED)){
						if(NetworkUtil.isWifiNetworkAvailable(mContext) && RuntimeState.isMDNSSetUped== false){
							Log.d(TAG, "reset MDNS");
							setupMDNS();
							if(!RuntimeState.isScaning){
								BackupLogic.scanAllFiles(mContext);
							}
						}
					}
				}
			}
			else if(Constant.ACTION_BACKUP_FILE.equals(action)){
				showSyncNotification(Constant.NOTIFICATION_BACK_UP_ON_PROGRESS);
			}
		}
	};

	@Override
	public void onDestroy() {
 		unregisterReceiver(mReceiver);
		getContentResolver().unregisterContentObserver(mCamera);
		getContentResolver().unregisterContentObserver(mVideo);
		getContentResolver().unregisterContentObserver(mAudio);

		if(timer!=null){
			timer.cancel();
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
    					if(!TextUtils.isEmpty(si.getPropertyString(Constant.PARAM_SERVER_OS))){
    						entity.serverOS = si.getPropertyString(Constant.PARAM_SERVER_OS);		
    					}
    					else{
    						entity.serverOS = "WINDOWS";
    					}
    	                entity.wsLocation = "ws://"+si.getHostAddress()+":"+si.getPort();
    	                Log.d(TAG, "Resolved SERVER NAME:"+entity.serverName);
    	                ServersLogic.updateBonjourServer(mContext, entity);
    	        		mPairedServers = ServersLogic.getBackupedServers(mContext);
    	        		if(mPairedServers.size()!=0){
    	        			RuntimeState.mAutoConnectMode = true ;
    	        		}
    	                if(RuntimeState.mAutoConnectMode == false && RuntimeState.OnWebSocketOpened == false ){	                    
//    	                    Intent intent = new Intent(Constant.ACTION_BONJOUR_SERVER_MANUAL_PAIRING);
//    	                    mContext.sendBroadcast(intent);
    	                }
    	                else{
    	                	if(NetworkUtil.isWifiNetworkAvailable(mContext) && RuntimeState.OnWebSocketOpened == false){
    		                	Intent intent = new Intent(Constant.ACTION_BONJOUR_SERVER_AUTO_PAIRING);
    		                	intent.putExtra(Constant.EXTRA_BONJOUR_AUTO_PAIR_STATUS, Constant.BONJOUR_PAIRING);
    		                	mContext.sendBroadcast(intent);
    		                	autoPairingConnect();
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
	//Observers
    class ImageTableObserver extends ContentObserver
    {
        private Handler mHandler;	

  	  public ImageTableObserver(Handler handler)
  	  {
  	    super(handler);
  	    mHandler = handler;
  	  }
  	  @Override
  	  public void onChange(boolean selfChange)
  	  {
  		long maxId = BackupLogic.getMaxIdFromMediaDB(mContext, Constant.TYPE_IMAGE);
  		Log.d(TAG, "Photo:MaxId:"+maxId+",RuntimeID:"+RuntimeState.maxImageId);
//  		Toast.makeText(getApplicationContext(), "Image:MaxId:"+maxId+",RuntimeID:"+RuntimeState.maxImageId, Toast.LENGTH_LONG).show();
  		if(maxId > RuntimeState.maxImageId && RuntimeState.isPhotoScaning == false){
  			BackupLogic.scanFileForBackup(mContext, Constant.TYPE_IMAGE);
	        RuntimeState.isPhotoScaning = true;
  			mHandler.postDelayed(new Runnable() {
  		        public void run() {
  		        	BackupLogic.scanFileForBackup(mContext, Constant.TYPE_IMAGE);
  		        	RuntimeState.isPhotoScaning = false;
  		        	}
  		        }, 1000);

  		}
  	  }
   }
    class VideoTableObserver extends ContentObserver
    {
      private Handler mHandler;	
	
  	  public VideoTableObserver(Handler handler)
  	  {
  	    super(handler);
  	    mHandler = handler;
  	  }
  	  @Override
  	  public void onChange(boolean selfChange)
  	  {
  		long maxId = BackupLogic.getMaxIdFromMediaDB(mContext, Constant.TYPE_VIDEO);
  		Log.d(TAG, "Video:MaxId:"+maxId+",RuntimeID:"+RuntimeState.maxVideoId);
  		if(maxId != RuntimeState.maxVideoId && RuntimeState.isVideoScaning == false){
//  			Toast.makeText(getApplicationContext(), "Video:MaxId:"+maxId+",RuntimeID:"+RuntimeState.maxVideoId, Toast.LENGTH_LONG).show();
  			if(BackupLogic.getFileSizeFromDB(mContext, Constant.TYPE_VIDEO, maxId)>0){  			  			
  				RuntimeState.isVideoScaning = true;
  				mHandler.postDelayed(new Runnable() {
		        public void run() {
		        	BackupLogic.scanFileForBackup(mContext, Constant.TYPE_VIDEO);
		        	RuntimeState.isVideoScaning = false;
		        	}
		        }, 3000);
  			}
  		}
  	  }
   }
    class AudioTableObserver extends ContentObserver
    {
      private Handler mHandler;	
  	  public AudioTableObserver(Handler handler)
  	  {
  	    super(handler);
  	    mHandler = handler;
  	  }
  	  @Override
  	  public void onChange(boolean selfChange)
  	  {
  		long maxId = BackupLogic.getMaxIdFromMediaDB(mContext, Constant.TYPE_AUDIO);
//  		Log.d(TAG, "Audioo:MaxId:"+maxId+",RuntimeID:"+RuntimeState.maxAudioId);
  		if(maxId > RuntimeState.maxAudioId && RuntimeState.isAudioScaning == false){  			
//  			Toast.makeText(getApplicationContext(), "Audioo:MaxId:"+maxId+",RuntimeID:"+RuntimeState.maxAudioId, Toast.LENGTH_LONG).show();
  			if(BackupLogic.getFileSizeFromDB(mContext, Constant.TYPE_AUDIO, maxId)>0){  			  			
	        	RuntimeState.isAudioScaning = true;
  				mHandler.postDelayed(new Runnable() {
		        public void run() {
		        	BackupLogic.scanFileForBackup(mContext, Constant.TYPE_AUDIO);
		        	RuntimeState.isAudioScaning = false;
		        	}
		        }, 2000);
  			}
  		}
  	  }
   }
}
