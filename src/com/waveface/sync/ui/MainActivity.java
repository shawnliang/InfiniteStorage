package com.waveface.sync.ui;

import java.util.ArrayList;

import android.app.Activity;
import android.app.ProgressDialog;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.database.ContentObserver;
import android.os.Bundle;
import android.os.Handler;
import android.text.TextUtils;
import android.widget.ListView;
import android.widget.TextView;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.RuntimeConfig;
import com.waveface.sync.db.BackupedServersTable;
import com.waveface.sync.db.ImportFilesTable;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.logic.BackupLogic;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.service.BonjourService;
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
	
	private int mServerRefreshCount = 0; 
	private int mFileRefreshCount = 0; 
	
	//DATA 
	private ArrayList<ServerEntity> mPairedServers ;
	private ServersAdapter mAdapter ;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		RuntimeConfig.isAppRunning = true;
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
		filter.addAction(Constant.ACTION_BONJOUR_SERVER_MANUAL_PAIRING);
		filter.addAction(Constant.ACTION_BONJOUR_SERVER_AUTO_PAIRING);		
		filter.addAction(Constant.ACTION_BACKUP_DONE);
		filter.addAction(Constant.ACTION_WS_SERVER_NOTIFY);		
		filter.addAction(Constant.ACTION_WS_BROKEN_NOTIFY);
		registerReceiver(mReceiver, filter);

		//GET PAIRED SERVERS
		mPairedServers = ServersLogic.getBackupedServers(this);
		if(NetworkUtil.isWifiNetworkAvailable(this)){
			if(mPairedServers.size()!=0){
				RuntimeConfig.mAutoConnectMode = true ;
				mProgressDialog = ProgressDialog.show(this, "",
						getString(R.string.auto_connect));
				mProgressDialog.setCancelable(true);
			}
		}
		startService(new Intent(this, BonjourService.class));
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
//			mFileRefreshCount++;
//			if(mFileRefreshCount%10 == 1){
				refreshLayout();
//			}
		}
	}
	
	private class ServerObserver extends ContentObserver {
		public ServerObserver() {
			super(new Handler());
		}
		@Override
		public void onChange(boolean selfChange) {
//			mServerRefreshCount++;
//			if(mServerRefreshCount%10 == 1){
				refreshServerStatus();
//			}
		}
	}

	private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			String action = intent.getAction();
			Log.d(TAG, "action:" + intent.getAction());
			if (Constant.ACTION_BACKUP_DONE.equals(action)) {
				//TODO:???
			}
			else if(Constant.ACTION_WS_SERVER_NOTIFY.equals(action)){
				dismissProgress();
				String response = intent.getStringExtra(Constant.EXTRA_SERVER_NOTIFY_CONTENT);
				if(response!=null){
					if(response.equals(Constant.WS_ACTION_BACKUP_INFO)){
						//refreshServerStatus();
					}
					else if(response.equals(Constant.WS_ACTION_ACCEPT)){
						if(RuntimeConfig.mAutoConnectMode){
							dismissProgress();
							ServerEntity entity = (ServerEntity) intent.getExtras().get(Constant.EXTRA_SERVER_DATA);
							ServerEntity pairedServer = ServersLogic.getBonjourServerByServerId(MainActivity.this, RuntimeConfig.mWebSocketServerId);
							entity.serverId = pairedServer.serverId;
							entity.serverName = pairedServer.serverName;
							entity.serverOS = pairedServer.serverOS;
							entity.wsLocation = pairedServer.wsLocation;

							SharedPreferences prefs = getSharedPreferences(Constant.PREFS_NAME, Context.MODE_PRIVATE);
					    	Editor editor = prefs.edit();
					    	editor.putString(Constant.PREF_STATION_WEB_SOCKET_URL, entity.wsLocation);
					    	editor.putString(Constant.PREF_SERVER_ID,entity.serverId);
					    	editor.commit();
							ServersLogic.startBackuping(context, entity);
						}
					}
				}
			}
			else if(Constant.ACTION_WS_BROKEN_NOTIFY.equals(action)){
				//Update all Backuoed Server to offline
				ServersLogic.setAllBackupedServersOffline(context);
			}
			else if(Constant.ACTION_BONJOUR_SERVER_MANUAL_PAIRING.equals(action)){
                Intent startIntent = new Intent(MainActivity.this, LinkServerActivity.class);	                    	
                startActivityForResult(startIntent, Constant.REQUEST_CODE_OPEN_SERVER_CHOOSER);
			}
			else if(Constant.ACTION_BONJOUR_SERVER_AUTO_PAIRING.equals(action)){
				int status = intent.getIntExtra(Constant.EXTRA_BONJOUR_AUTO_PAIR_STATUS,Constant.BONJOUR_PAIRED);
				switch(status){
					case Constant.BONJOUR_PAIRED:
						dismissProgress();
						break;
				}
			}			
			refreshServerStatus();
		}
	};
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
    protected void onDestroy() {
    	ServersLogic.purgeAllBonjourServer(this);
		unregisterReceiver(mReceiver);
		getContentResolver().unregisterContentObserver(mImportFilesObserver);		
		getContentResolver().unregisterContentObserver(mServerObserver);
		RuntimeConfig.isAppRunning = false;
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
