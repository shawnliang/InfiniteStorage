package com.waveface.sync.ui;

import java.util.ArrayList;
import java.util.Calendar;

import android.app.Activity;
import android.app.AlarmManager;
import android.app.PendingIntent;
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
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.RuntimeState;
import com.waveface.sync.db.ImportFilesTable;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.logic.BackupLogic;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.service.InfiniteReceiver;
import com.waveface.sync.service.InfiniteService;
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
public class MainActivity extends Activity implements OnClickListener{
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
	private Button mDeletePhotoBtn;
	private Button mDeleteVideoBtn;
	private Button mDeleteAudioBtn;
	
	private ProgressDialog mProgressDialog;


	private SharedPreferences mPrefs ;
	private Editor mEditor ;

	//DATA 
	private ArrayList<ServerEntity> mPairedServers ;
	private ServersAdapter mAdapter ;
	

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		RuntimeState.isAppLaunching = true;
		
		Log.d(TAG, "onCreate");
		setContentView(R.layout.sync_main);
		
		mPrefs = getSharedPreferences(Constant.PREFS_NAME, Context.MODE_PRIVATE);
		mEditor = mPrefs.edit();
		
		mDevice = (TextView) this.findViewById(R.id.textDevice);
		mDevice.setText(DeviceUtil.getDeviceNameForDisplay(this));
		mNowPeriod = (TextView) this.findViewById(R.id.textPeriod);

		//PHOTO
		mPhotoCount = (TextView) this.findViewById(R.id.textPhotoCount);
		mPhotoSize = (TextView) this.findViewById(R.id.textPhotoSize);
		//VIDEO
		mVideoCount = (TextView) this.findViewById(R.id.textVideoCount);
		mVideoSize = (TextView) this.findViewById(R.id.textVideoSize);
		
		//AUDIO
		mAudioCount = (TextView) this.findViewById(R.id.textAudioCount);
		mAudioSize = (TextView) this.findViewById(R.id.textAudioSize);
		
		mDeletePhotoBtn = (Button) this.findViewById(R.id.btnDeletePhoto);
		mDeletePhotoBtn.setOnClickListener(this);
		mDeleteVideoBtn = (Button) this.findViewById(R.id.btnDeleteVideo);
		mDeleteVideoBtn.setOnClickListener(this);
		mDeleteAudioBtn = (Button) this.findViewById(R.id.btnDeleteAudio);
		mDeleteAudioBtn.setOnClickListener(this);
		
		ListView listview = (ListView) findViewById(R.id.listview);
		ArrayList<ServerEntity> servers = ServersLogic.getBackupedServers(this);		
		mAdapter = new ServersAdapter(this,servers);
		listview.setAdapter(mAdapter);

		refreshLayout();
		//RESGISTER OBSERVER
		mImportFilesObserver = new ImportFilesObserver();
		getContentResolver().registerContentObserver(ImportFilesTable.IMPORT_FILE_URI, false, mImportFilesObserver);
		
//		mServerObserver = new ServerObserver();
//		getContentResolver().registerContentObserver(BackupedServersTable.BACKUPED_SERVER_URI, false, mServerObserver);

		IntentFilter filter = new IntentFilter();
		filter.addAction(Constant.ACTION_BONJOUR_SERVER_MANUAL_PAIRING);
		filter.addAction(Constant.ACTION_BONJOUR_SERVER_AUTO_PAIRING);		
		filter.addAction(Constant.ACTION_BACKUP_DONE);
		filter.addAction(Constant.ACTION_WS_SERVER_NOTIFY);
		filter.addAction(Constant.ACTION_NETWORK_STATE_CHANGE);
		registerReceiver(mReceiver, filter);

		//GET PAIRED SERVERS
		displayProgress();
		
		boolean alarmEnable = mPrefs.getBoolean(Constant.PREF_BONJOUR_SERVER_ALRM_ENNABLED, false);
		if(alarmEnable == false){
			setAlarmWakeUpService(this);
			mEditor.putBoolean(Constant.PREF_BONJOUR_SERVER_ALRM_ENNABLED, true).commit();
		}
		startService(new Intent(MainActivity.this, InfiniteService.class)); 
		
	}
	
	private void dismissProgress(){
		if ((mProgressDialog != null) && mProgressDialog.isShowing()) {
			mProgressDialog.dismiss();
		}		
	}
	private void displayProgress(){
		mPairedServers = ServersLogic.getBackupedServers(this);
		if (mProgressDialog == null ) {
			if(NetworkUtil.isWifiNetworkAvailable(this) && RuntimeState.OnWebSocketStation == false ){
				if(mPairedServers.size()!=0 ){
					RuntimeState.mAutoConnectMode = true ;
					mProgressDialog = ProgressDialog.show(this, "",
							getString(R.string.auto_connect));
					mProgressDialog.setCancelable(true);
				}
			}
		}		
		if(mPairedServers.size()==0 ){
            Intent startIntent = new Intent(MainActivity.this, LinkServerActivity.class);	                    	
            startActivityForResult(startIntent, Constant.REQUEST_CODE_OPEN_SERVER_CHOOSER);
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
			if (Constant.ACTION_BACKUP_DONE.equals(action)) {
				//TODO:???
			}
			else if (Constant.ACTION_NETWORK_STATE_CHANGE.equals(action)) {
				//TODO:???
			}
			else if(Constant.ACTION_WS_SERVER_NOTIFY.equals(action)){
				dismissProgress();
				String actionContent = intent.getStringExtra(Constant.EXTRA_WEB_SOCKET_EVENT_CONTENT);
				if(actionContent!=null){
					if(actionContent.equals(Constant.WS_ACTION_BACKUP_INFO)){
						//refreshServerStatus();
					}
					else if(actionContent.equals(Constant.WS_ACTION_ACCEPT)){
						if(RuntimeState.mAutoConnectMode){
							dismissProgress();
						}
					}
					else if(actionContent.equals(Constant.WS_ACTION_SOCKET_CLOSED)){
					}
				}
			}
//			else if(Constant.ACTION_BONJOUR_SERVER_MANUAL_PAIRING.equals(action)){
//                Intent startIntent = new Intent(MainActivity.this, LinkServerActivity.class);	                    	
//                startActivityForResult(startIntent, Constant.REQUEST_CODE_OPEN_SERVER_CHOOSER);
//			}
			else if(Constant.ACTION_BONJOUR_SERVER_AUTO_PAIRING.equals(action)){
				displayProgress();
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
        Log.d(TAG, "onPause");
    }
    
    @Override
    protected void onResume() {
        super.onResume();
        Log.d(TAG, "onResume");
    }
        
    @Override
    protected void onDestroy() {
    	RuntimeState.isAppLaunching = false;
    	Log.d(TAG, "onDestroy");
 		unregisterReceiver(mReceiver);
		getContentResolver().unregisterContentObserver(mImportFilesObserver);		
//		getContentResolver().unregisterContentObserver(mServerObserver);
//		stopService(new Intent(MainActivity.this, InfiniteService.class)); 
    	super.onDestroy();
    }
    public void refreshLayout(){
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
    
    private void setAlarmWakeUpService(Context context){
    	AlarmManager alarmManager = (AlarmManager)context.getSystemService(Context.ALARM_SERVICE);
        Calendar cal = Calendar.getInstance();
        cal.setTimeInMillis(System.currentTimeMillis());
        cal.add(Calendar.SECOND, 2);
		long interval = 1 *30 * 1000;
		Intent intent = new Intent(context, InfiniteReceiver.class);
		PendingIntent sender = PendingIntent.getBroadcast(context, 0, intent, PendingIntent.FLAG_UPDATE_CURRENT);
		alarmManager.setRepeating(AlarmManager.RTC, cal.getTimeInMillis(),interval, sender);
    }

	@Override
	public void onClick(View v) {
		switch(v.getId()){
			case R.id.btnDeletePhoto:
				Toast.makeText(this, "Coming Soon!", Toast.LENGTH_LONG).show();
				break;
			case R.id.btnDeleteVideo:
				Toast.makeText(this, "Coming Soon!", Toast.LENGTH_LONG).show();
				break;
			case R.id.btnDeleteAudio:
				Toast.makeText(this, "Coming Soon!", Toast.LENGTH_LONG).show();
				break;
		}
	}
}
