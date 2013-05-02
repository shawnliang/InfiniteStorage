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
import android.graphics.Bitmap;
import android.graphics.Matrix;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Handler;
import android.text.TextUtils;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.RelativeLayout;
import android.widget.TextView;
import android.widget.Toast;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.RuntimeState;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.logic.BackupLogic;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.service.InfiniteService;
import com.waveface.sync.util.DeviceUtil;
import com.waveface.sync.util.ImageUtil;
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
    
    private Handler mHandler = new Handler();

    //UI
	private TextView mDevice;
	private TextView mTotalInfo;	
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
	private RelativeLayout mRLSetting;
	private ImageView mIvAddPc;
	private RelativeLayout rlBackupContent;
	private ImageView ivPC ;
	private ImageView ivFile ;	
	private ImageView ivPlay ;		
    private TextView tvBackupPC ;		    
    private TextView tvContent ;
    private TextView tvDetail ;
    private TextView tvLastBackupTime ;
    
	private ProgressDialog mProgressDialog;
    private MediaStoreImage mMediaImage;

	private SharedPreferences mPrefs ;
	private Editor mEditor ;
	
	private final static int IMAGE_HEIGHT = 110; 
	private final static int IMAGE_WIDTH = 110;
	//DATA 
	private ArrayList<ServerEntity> mPairedServers ;
	private PairedServersAdapter mAdapter ;
	

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		RuntimeState.isAppLaunching = true;
		
		Log.d(TAG, "onCreate");
		setContentView(R.layout.sync_main);
		
		mPrefs = getSharedPreferences(Constant.PREFS_NAME, Context.MODE_PRIVATE);
		mEditor = mPrefs.edit();
		
		mMediaImage = new MediaStoreImage(this,IMAGE_WIDTH,IMAGE_HEIGHT);
		
		mDevice = (TextView) this.findViewById(R.id.textDevice);
		mDevice.setText(DeviceUtil.getDeviceNameForDisplay(this));
		
		mTotalInfo = (TextView) this.findViewById(R.id.textTotolInfo);
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
		
		//progress Content Area
		rlBackupContent = (RelativeLayout) findViewById(R.id.rlBackupContent);
	    ivPC = (ImageView) findViewById(R.id.imagePC);
	    ivFile = (ImageView) findViewById(R.id.ivFile);
	    ivPlay = (ImageView) findViewById(R.id.ivPlay);	    
	    tvBackupPC = (TextView) findViewById(R.id.tvBackupPC);		    
	    tvContent = (TextView) findViewById(R.id.tvContent);
	    tvDetail = (TextView) findViewById(R.id.tvDetail);
	    tvLastBackupTime = (TextView) findViewById(R.id.tvLastBackupTime);
		
		//PAIRED SERVERS
//		ListView listview = (ListView) findViewById(R.id.listview);
//		ArrayList<ServerEntity> servers = ServersLogic.getBackupedServers(this);		
//		mAdapter = new PairedServersAdapter(this,servers);
//		listview.setAdapter(mAdapter);

	    mRLSetting = (RelativeLayout) findViewById(R.id.rlSetting);
	    
		mIvAddPc = (ImageView) findViewById(R.id.ivAddpc);
	    mIvAddPc.setOnClickListener(this);	    	    
		refreshLayout();

		IntentFilter filter = new IntentFilter();
		filter.addAction(Constant.ACTION_BONJOUR_SERVER_MANUAL_PAIRING);
		filter.addAction(Constant.ACTION_BONJOUR_SERVER_AUTO_PAIRING);		
		filter.addAction(Constant.ACTION_BACKUP_DONE);
		filter.addAction(Constant.ACTION_SCAN_FILE);		
		filter.addAction(Constant.ACTION_WS_SERVER_NOTIFY);
		filter.addAction(Constant.ACTION_WEB_SOCKET_SERVER_CONNECTED);
		filter.addAction(Constant.ACTION_NETWORK_STATE_CHANGE);
		filter.addAction(Constant.ACTION_UPLOADING_FILE);
		registerReceiver(mReceiver, filter);

		//GET PAIRED SERVERS
		firsttimeDispaly();
		
		boolean alarmEnable = mPrefs.getBoolean(Constant.PREF_BONJOUR_SERVER_ALRM_ENNABLED, false);
		if(alarmEnable == false){
			BackupLogic.setAlarmWakeUpService(this);
			mEditor.putBoolean(Constant.PREF_BONJOUR_SERVER_ALRM_ENNABLED, true).commit();
		}
		new StartServiceTask().execute(new Void[]{});
	}
	
	private void dismissProgress(){
		if ((mProgressDialog != null) && mProgressDialog.isShowing()) {
			mProgressDialog.dismiss();
		}		
	}
	private void firsttimeDispaly(){
		mPairedServers = ServersLogic.getBackupedServers(this);
		if (mProgressDialog == null ) {
			if(NetworkUtil.isWifiNetworkAvailable(this) && RuntimeState.OnWebSocketStation == false ){
				if(mPairedServers.size()!=0 ){
					RuntimeState.mAutoConnectMode = true ;
					mProgressDialog = ProgressDialog.show(this, "",
							getString(R.string.auto_connect));
					mProgressDialog.setCancelable(true);
			        mHandler.postDelayed(new Runnable() {
			            public void run() {
			            	if(NetworkUtil.isWifiNetworkAvailable(MainActivity.this) && RuntimeState.OnWebSocketStation == false ){
			            		dismissProgress();
			            		Toast.makeText(MainActivity.this, R.string.can_not_find_server, Toast.LENGTH_LONG).show();
			            	}
			            }
			         }, 10000);
				}
			}
		}		
		if(mPairedServers.size()==0 ){
            Intent startIntent = new Intent(MainActivity.this, FirstUseActivity.class);	                    	
            startActivityForResult(startIntent, Constant.REQUEST_CODE_OPEN_SERVER_CHOOSER);
		}
	}
	
	private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			String action = intent.getAction();
			Log.d(TAG, "action:" + intent.getAction());
			if (Constant.ACTION_SCAN_FILE.equals(action) || 
					Constant.ACTION_WEB_SOCKET_SERVER_CONNECTED.equals(action)) {
				refreshLayout();
			}
			else if (Constant.ACTION_BACKUP_DONE.equals(action)) {
				displayProgressingInfo();
			}
			else if (Constant.ACTION_NETWORK_STATE_CHANGE.equals(action)) {
				displayProgressingInfo();
			}
			else if(Constant.ACTION_WS_SERVER_NOTIFY.equals(action)){
				dismissProgress();
				String actionContent = intent.getStringExtra(Constant.EXTRA_WEB_SOCKET_EVENT_CONTENT);
				if(actionContent!=null){
//					if(actionContent.equals(Constant.WS_ACTION_BACKUP_INFO)){
//						//refreshServerStatus();
//					}
//					else if(actionContent.equals(Constant.WS_ACTION_ACCEPT)){
					if(actionContent.equals(Constant.WS_ACTION_ACCEPT)){
						if(RuntimeState.mAutoConnectMode){
							dismissProgress();
						}
					}
					else if(actionContent.equals(Constant.WS_ACTION_SOCKET_CLOSED)){
						displayProgressingInfo();
					}
				}
			}
			else if(Constant.ACTION_BONJOUR_SERVER_AUTO_PAIRING.equals(action)){
				firsttimeDispaly();
				displayProgressingInfo();
			}	
			else if(Constant.ACTION_UPLOADING_FILE.equals(action)){
				displayProgressingInfo();
				int state = intent.getIntExtra(Constant.EXTRA_BACKING_UP_FILE_STATE, -1);
				refreshFileContent(state);
			}
		}
	};

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == Constant.REQUEST_CODE_OPEN_SERVER_CHOOSER) {
            if (resultCode == RESULT_OK) {         
            	refreshLayout();
//            	mAdapter.setData(ServersLogic.getBackupedServers(this));
            }
        }
        else if(requestCode == Constant.REQUEST_CODE_ADD_SERVER){
            if (resultCode == RESULT_OK) {         
//            	mAdapter.setData(ServersLogic.getBackupedServers(this));
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
    	super.onDestroy();
    }
    public void refreshLayout(){
		String[] periods = BackupLogic.getFilePeriods(this);
		if(TextUtils.isEmpty(periods[0])){
			mNowPeriod.setText(R.string.file_scanning);
		}else{
			mNowPeriod.setText(getString(R.string.period,periods[0],periods[1]));
		}
		long totalCount = 0;
		long totalSize = 0;
		
		
		long[] datas = BackupLogic.getFileInfo(this, Constant.TYPE_IMAGE);
		mPhotoCount.setText(getString(R.string.photos, datas[0]));
		mPhotoSize.setText(StringUtil.byteCountToDisplaySize(datas[1]));
		totalCount += datas[0]; 
		totalSize += datas[1];
				
		datas = BackupLogic.getFileInfo(this, Constant.TYPE_VIDEO);
		mVideoCount.setText(getString(R.string.videos, datas[0]));
		mVideoSize.setText(StringUtil.byteCountToDisplaySize(datas[1]));
		totalCount += datas[0]; 
		totalSize += datas[1];

		
		datas = BackupLogic.getFileInfo(this, Constant.TYPE_AUDIO);
		mAudioCount.setText(getString(R.string.audios, datas[0]));
		mAudioSize.setText(StringUtil.byteCountToDisplaySize(datas[1]));
		totalCount += datas[0]; 
		totalSize += datas[1];

		mTotalInfo.setText(getString(R.string.total_info, totalCount,StringUtil.byteCountToDisplaySize(totalSize)));
		
		
		
		//REFRESH PROGRESS INFO
		displayProgressingInfo();
		
		//REFRESH SERVERS STATUS
		//refreshServerStatus();
		
		//REFRESH SETTING AREA
		if(ServersLogic.hasBackupedServers(this)){
			mRLSetting.setVisibility(View.GONE);
		}
		else{
			mRLSetting.setVisibility(View.VISIBLE);
		}
    }
    public void displayProgressingInfo(){
    	if(ServersLogic.hasBackupedServers(this)){
    		ArrayList<ServerEntity> servers = ServersLogic.getBackupedServers(this);
    		ServerEntity entity = servers.get(0);
    		String displayTime= StringUtil.displayLocalTime(entity.lastLocalBackupTime, StringUtil.DATE_FORMAT);
    		rlBackupContent.setVisibility(View.VISIBLE);
    		int[] counts = BackupLogic.getBackupProgressInfo(this, entity.serverId);
	    	if(RuntimeState.OnWebSocketStation){
			    tvBackupPC.setText(entity.serverName);			    
			    if(RuntimeState.isBackuping){
			    	ivPC.setImageResource(R.drawable.ic_processing);
				    tvContent.setText(getString(R.string.backup_progress,counts[0],counts[1]));					    
			    }
			    else {
			    	ivPC.setImageResource(R.drawable.ic_transfer);
			    	if(counts[0] == counts[1] && counts[1]!=0 ){
			    		tvContent.setText(getString(R.string.backup_done));
			    	}
			    	else {
			    		tvContent.setText(getString(R.string.backup_info_empty));
			    	}
		    	}
			}
	    	else{
	    		tvBackupPC.setText(entity.serverName);
	    		ivPC.setImageResource(R.drawable.ic_offline);
		    	
		    	if(counts[0]== counts[1] && counts[1]!=0 ){
		    		tvContent.setText(getString(R.string.backup_done));
		    	}
		    	else{
		    		tvContent.setText(getString(R.string.backup_uncompleted));
		    	}		    	
	    	}
	    	if(!TextUtils.isEmpty(displayTime)){
	    		tvLastBackupTime.setText( getString(R.string.backup_last_local_time,displayTime));
	    	}
		    tvDetail.setText(StringUtil.getFilename(RuntimeState.mFilename));
    	}
    	else{
    		rlBackupContent.setVisibility(View.INVISIBLE);
    	}
    	displayBackingUpImage();
    }
    
    public void refreshFileContent(int state){
    	switch(state){
			case Constant.JOB_START:
				displayProgressingInfo();
				break;
    		case Constant.FILE_START:
        		int[] counts = BackupLogic.getBackupProgressInfo(this, RuntimeState.mWebSocketServerId);
//    			int progress = (int) (counts[0]/ (float) counts[1] * 100);
//			    tvContent.setText(getString(R.string.backup_progress,counts[0],counts[1],progress));
			    tvContent.setText(getString(R.string.backup_progress,counts[0],counts[1]));
			    tvDetail.setText(StringUtil.getFilename(RuntimeState.mFilename));
			    displayBackingUpImage();
			    break;
    	}
    }
//    public void refreshServerStatus(){
//		mAdapter.setData(ServersLogic.getBackupedServers(this));
//		if(ServersLogic.hasBackupedServers(this)){
//			mRLSetting.setVisibility(View.GONE);
//		}
//    }

	private void displayBackingUpImage() {
		if(!TextUtils.isEmpty(RuntimeState.mFilename)){
			switch(RuntimeState.mFileType){
				case Constant.TYPE_AUDIO:
				case Constant.TYPE_VIDEO:
					ivPlay.setVisibility(View.VISIBLE);
					break;
				case Constant.TYPE_IMAGE:
					ivPlay.setVisibility(View.INVISIBLE);
					break;
			}
			if(RuntimeState.mFileType != Constant.TYPE_AUDIO){
				Bitmap b = mMediaImage.getBitmap(RuntimeState.mMediaID, RuntimeState.mFileType);
				if(b!=null){
					ivFile.setImageBitmap(b);
				}
				else{
					ivFile.setImageResource(R.drawable.ic_photos);
				}
			}
		}
		else{
			ivFile.setImageResource(R.drawable.ic_photos);
		}
	}


	@Override
	public void onClick(View v) {
		Intent startIntent = null;
 		switch(v.getId()){
			case R.id.ivAddpc:
				if(!ServersLogic.hasBackupedServers(this)){
		            startIntent = new Intent(MainActivity.this, FirstUseActivity.class);	                    	
		            startActivityForResult(startIntent, Constant.REQUEST_CODE_OPEN_SERVER_CHOOSER);
				}
//				else{
//		            startIntent = new Intent(MainActivity.this, AddServerActivity.class);	                    	
//		            startActivityForResult(startIntent, Constant.REQUEST_CODE_ADD_SERVER);					
//				}
				break;		
			case R.id.btnDeletePhoto:
	            startIntent = new Intent(MainActivity.this, CleanActivity.class);
	            startIntent.putExtra(Constant.BUNDLE_FILE_TYPE, Constant.TRANSFER_TYPE_IMAGE);
	            startActivityForResult(startIntent, Constant.REQUEST_CODE_CLEAN_STORAGE);
				break;
			case R.id.btnDeleteVideo:
	            startIntent = new Intent(MainActivity.this, CleanActivity.class);	                    	
	            startIntent.putExtra(Constant.BUNDLE_FILE_TYPE, Constant.TRANSFER_TYPE_VIDEO);
	            startActivityForResult(startIntent, Constant.REQUEST_CODE_CLEAN_STORAGE);
				break;
			case R.id.btnDeleteAudio:
	            startIntent = new Intent(MainActivity.this, CleanActivity.class);	          
	            startIntent.putExtra(Constant.BUNDLE_FILE_TYPE, Constant.TRANSFER_TYPE_AUDIO);
	            startActivityForResult(startIntent, Constant.REQUEST_CODE_CLEAN_STORAGE);
				break;
		}
	}
	class StartServiceTask extends AsyncTask<Void,Void,Void>{

		@Override
		protected Void doInBackground(Void... params) {
			startService(new Intent(MainActivity.this, InfiniteService.class)); 
			return null;
		}		
	}
}
