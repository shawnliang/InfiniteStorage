package com.waveface.sync.ui;

import java.io.IOException;
import java.util.ArrayList;

import javax.jmdns.JmDNS;
import javax.jmdns.ServiceEvent;
import javax.jmdns.ServiceInfo;
import javax.jmdns.ServiceListener;

import android.app.Activity;
import android.app.ProgressDialog;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.database.ContentObserver;
import android.net.wifi.WifiManager;
import android.os.Bundle;
import android.os.Handler;
import android.text.TextUtils;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.RuntimeConfig;
import com.waveface.sync.db.BackupedServersTable;
import com.waveface.sync.db.ImportFilesTable;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.logic.BackupLogic;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.task.BackupFilesTask;
import com.waveface.sync.util.DeviceUtil;
import com.waveface.sync.util.Log;
import com.waveface.sync.util.NetworkUtil;
import com.waveface.sync.util.StringUtil;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 * 
 * @see SystemUiHider
 */
public class MainActivity extends Activity {
	private String TAG = MainActivity.class.getSimpleName();
	//BONJOUR
    private WifiManager.MulticastLock lock;
    private Handler mHandler = new Handler();
    private JmDNS jmdns = null;
    private ServiceListener mListener = null;
    
	private ImportFilesObserver mImportFilesObserver;    
	private ServerObserver mServerObserver;
    //UI
	private TextView mDevice;
	private TextView mNowPeriod;
	private TextView mPhotoCount;
	private TextView mPhotoSize;
	private TextView mVideoCount;
	private TextView mVideoSize;
	private TextView mAudioCount;
	private TextView mAudioSize;
	private ProgressDialog mProgressDialog;
	
	//DATA 
	private ArrayList<ServerEntity> mPairedServers ;
	private ServerEntity mConnectedServer ;
	private String mCondidateServerId ;
	private String mCondidateWSLocation ;
	
	
	private static boolean mAutoConnectMode = false;	
	private static boolean mHasPopupFirstUse = false;
	
	//DATAS
	private ServersAdapter mAdapter ;
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.sync_main);
		mDevice = (TextView) this.findViewById(R.id.textDevice);
		mDevice.setText(DeviceUtil.getDeviceNameForDisplay(this));
		mNowPeriod = (TextView) this.findViewById(R.id.textPeriod);
		String[] periods = BackupLogic.getFilePeriods(this);
		if(TextUtils.isEmpty(periods[0])){
			mNowPeriod.setText(R.string.file_scanning);
		}else{
			mNowPeriod.setText(getString(R.string.period,periods[0],periods[1]));
		}
		
		long[] datas = BackupLogic.getFileInfo(this, Constant.TYPE_IMAGE);
		mPhotoCount = (TextView) this.findViewById(R.id.textPhotoCount);
		mPhotoSize = (TextView) this.findViewById(R.id.textPhotoSize);
		mPhotoCount.setText(getString(R.string.photos, datas[0]));
		mPhotoSize.setText(StringUtil.byteCountToDisplaySize(datas[1]));
		//VIDEO
		datas = BackupLogic.getFileInfo(this, Constant.TYPE_VIDEO);
		mVideoCount = (TextView) this.findViewById(R.id.textVideoCount);
		mVideoSize = (TextView) this.findViewById(R.id.textVideoSize);
		mVideoCount.setText(getString(R.string.videos, datas[0]));
		mVideoSize.setText(StringUtil.byteCountToDisplaySize(datas[1]));
		
		//AUDIO
		datas = BackupLogic.getFileInfo(this, Constant.TYPE_AUDIO);
		mAudioCount = (TextView) this.findViewById(R.id.textAudioCount);
		mAudioSize = (TextView) this.findViewById(R.id.textAudioSize);
		mAudioCount.setText(getString(R.string.audios, datas[0]));
		mAudioSize.setText(StringUtil.byteCountToDisplaySize(datas[1]));
		
		ListView listview = (ListView) findViewById(R.id.listview);
		ArrayList<ServerEntity> servers = ServersLogic.getBackupedServers(this);		
		mAdapter = new ServersAdapter(this,servers);
		listview.setAdapter(mAdapter);

		//RESGISTER OBSERVER
		mImportFilesObserver = new ImportFilesObserver();
		getContentResolver().registerContentObserver(ImportFilesTable.IMPORT_FILE_URI, false, mImportFilesObserver);
		
		mServerObserver = new ServerObserver();
		getContentResolver().registerContentObserver(BackupedServersTable.BACKUPED_SERVER_URI, false, mServerObserver);

		
		
		
		IntentFilter filter = new IntentFilter();
		filter.addAction(Constant.ACTION_BACKUP_FILE);
		filter.addAction(Constant.ACTION_BACKUP_DONE);
		filter.addAction(Constant.ACTION_WS_SERVER_NOTIFY);		
		filter.addAction(Constant.ACTION_WS_BROKEN_NOTIFY);
		registerReceiver(mReceiver, filter);

		//SETUP BONJOUR
		 multiCastSetUp();
		//GET PAIRED SERVERS
		mPairedServers = ServersLogic.getBackupedServers(this);
		if(NetworkUtil.isWifiNetworkAvailable(this)){
			if(mPairedServers.size()!=0){
				mAutoConnectMode = true ;
				mProgressDialog = ProgressDialog.show(this, "",
						getString(R.string.auto_connect));
				mProgressDialog.setCancelable(true);
			}
		}
		mHandler.postDelayed(new Runnable() {
            public void run() {
                multiCastSetUp();
            }
            }, 100);        
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
			dismissProgress();
		}
	}
	
	private void dismissProgress(){
		if ((mProgressDialog != null) && mProgressDialog.isShowing()) {
			mProgressDialog.dismiss();
		}		
	}
	private class ImportFilesObserver extends ContentObserver {
		public ImportFilesObserver() {
			super(new Handler());
		}
		@Override
		public void onChange(boolean selfChange) {
			refreshLayout();
		}
	}
	
	private class ServerObserver extends ContentObserver {
		public ServerObserver() {
			super(new Handler());
		}
		@Override
		public void onChange(boolean selfChange) {
			refreshServerStatus();
		}
	}

	private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			String action = intent.getAction();
			Log.d(TAG, "action:" + intent.getAction());
			if (Constant.ACTION_BACKUP_FILE.equals(action)) {
				if(RuntimeConfig.isBackuping == false){
					RuntimeConfig.isBackuping = true;
					new BackupFilesTask(context).execute(new Void[]{});
				}
			}
			else if (Constant.ACTION_BACKUP_DONE.equals(action)) {
				Toast.makeText(context, R.string.backuped_completed, Toast.LENGTH_LONG).show();
			}
			else if(Constant.ACTION_WS_SERVER_NOTIFY.equals(action)){
				dismissProgress();
				String response = intent.getStringExtra(Constant.EXTRA_SERVER_NOTIFY_CONTENT);
				if(response!=null){
					if(response.equals(Constant.WS_ACTION_BACKUP_INFO)){
						//refreshServerStatus();
					}
					else if(response.equals(Constant.WS_ACTION_ACCEPT)){
						if(mAutoConnectMode){
							dismissProgress();
							ServerEntity entity = (ServerEntity) intent.getExtras().get(Constant.EXTRA_SERVER_DATA);
							entity.serverId = mConnectedServer.serverId;
							entity.serverName = mConnectedServer.serverName;
							entity.serverOS = mConnectedServer.serverOS;
							entity.wsLocation = mConnectedServer.wsLocation;
							//bonjourServer = ServersLogic.getBonjourServerByServerId(MainActivity.this, mCondidateServerId);
							mConnectedServer = entity;
					    	SharedPreferences prefs = getSharedPreferences(Constant.PREFS_NAME, Context.MODE_PRIVATE);
					    	Editor editor = prefs.edit();
					    	editor.putString(Constant.PREF_STATION_WEB_SOCKET_URL, mConnectedServer.wsLocation);
					    	editor.putString(Constant.PREF_SERVER_ID,mConnectedServer.serverId);
					    	editor.commit();
							ServersLogic.startBackuping(context, mConnectedServer);
						}
					}
				}
			}
			else if(Constant.ACTION_WS_BROKEN_NOTIFY.equals(action)){
				//Update all Backuoed Server to offline
				ServersLogic.setAllBackupedServersOffline(context);
			}
			refreshServerStatus();
		}
	};
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
                    ServersLogic.updateBonjourServer(MainActivity.this, entity);
                    if(mAutoConnectMode == false){
	                    if(RuntimeConfig.OnWebSocketOpened == false ){
		                    if(mHasPopupFirstUse == false ){
			                    Intent intent = new Intent(MainActivity.this, LinkServerActivity.class);	                    	
			                    MainActivity.this.startActivityForResult(intent, Constant.REQUEST_CODE_OPEN_SERVER_CHOOSER);
			                    mHasPopupFirstUse = true; 
		                    }
		                    else{
    	                    	Intent intent = new Intent(Constant.ACTION_BONJOUR_MULTICAT_EVENT);
    		                    MainActivity.this.sendBroadcast(intent);
		                    }
	                    }
                    }
                    else{
                        mHandler.postDelayed(new Runnable() {
                            public void run() {
                            	autoPairConnect();  		                    
                            	}
                            }, 500);
                    }
                }

                @Override
                public void serviceRemoved(ServiceEvent ev) {
                	//TODO:
                	ServiceInfo si = ev.getInfo();
                    ServerEntity entity = new ServerEntity();
                	entity.serverName = si.getName();
					entity.serverId = si.getPropertyString(Constant.PARAM_SERVER_ID);
					ServersLogic.purgeBonjourServerByServerId(MainActivity.this, entity.serverId);
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
    public void refreshServerStatus(){
		mAdapter.setData(ServersLogic.getBackupedServers(this));
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        // Check which request we're responding to
        if (requestCode == Constant.REQUEST_CODE_OPEN_SERVER_CHOOSER) {
            // Make sure the request was successful
            if (resultCode == RESULT_OK) {                
            	mAdapter.setData(ServersLogic.getBackupedServers(this));
            }
        }
    }
    @Override
    protected void onPause() {
        super.onPause();
    }
    
    @Override
    protected void onResume() {
        super.onResume();
    }
    
	@Override
    protected void onStop() {
	if (jmdns != null) {
        if (mListener != null) {
            jmdns.removeServiceListener(Constant.INFINTE_STORAGE, mListener);
            mListener = null;
        }
        jmdns.unregisterAllServices();
        try {
            jmdns.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
        jmdns = null;
	}
    try {
    	lock.release();
    } catch (Throwable th) {
        // ignoring this exception, probably wakeLock was already released
    }    
	super.onStop();
}

    
    @Override
    protected void onDestroy() {
    	ServersLogic.purgeAllBonjourServer(this);
		unregisterReceiver(mReceiver);
		getContentResolver().unregisterContentObserver(mImportFilesObserver);		
		getContentResolver().unregisterContentObserver(mServerObserver);
    	super.onDestroy();
    }
    public void refreshLayout(){
 		mNowPeriod = (TextView) this.findViewById(R.id.textPeriod);
		String[] periods = BackupLogic.getFilePeriods(this);
		if(TextUtils.isEmpty(periods[0])){
			mNowPeriod.setText(R.string.file_scanning);
		}else{
			mNowPeriod.setText(getString(R.string.period,periods[0],periods[1]));
		}
				
		long[] datas = BackupLogic.getFileInfo(this, Constant.TYPE_IMAGE);
		mPhotoCount.setText(getString(R.string.photos, datas[0]));
		mPhotoSize.setText(StringUtil.byteCountToDisplaySize(datas[1]));
	
		datas = BackupLogic.getFileInfo(this, Constant.TYPE_VIDEO);
		mVideoCount.setText(getString(R.string.videos, datas[0]));
		mVideoSize.setText(StringUtil.byteCountToDisplaySize(datas[1]));
		
		datas = BackupLogic.getFileInfo(this, Constant.TYPE_AUDIO);
		mAudioCount.setText(getString(R.string.audios, datas[0]));
		mAudioSize.setText(StringUtil.byteCountToDisplaySize(datas[1]));
		//REFRESH SERVERS STATUS
		refreshServerStatus();
    }
}
