package com.waveface.sync.ui;

import java.io.IOException;
import java.util.ArrayList;

import javax.jmdns.JmDNS;
import javax.jmdns.ServiceEvent;
import javax.jmdns.ServiceInfo;
import javax.jmdns.ServiceListener;

import org.jwebsocket.kit.WebSocketException;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.SharedPreferences;
import android.net.wifi.WifiManager;
import android.os.Bundle;
import android.os.Handler;
import android.text.TextUtils;
import android.widget.ListView;
import android.widget.TextView;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.RuntimePlayer;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.logic.FileBackup;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.task.BackupFilesTask;
import com.waveface.sync.util.DeviceUtil;
import com.waveface.sync.util.Log;
import com.waveface.sync.util.StringUtil;
import com.waveface.sync.util.SystemUiHider;
import com.waveface.sync.websocket.RuntimeWebClient;

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
    //UI
	private TextView mDevice;
	private TextView mNowPeriod;
	private TextView mPhotoCount;
	private TextView mPhotoSize;
	private TextView mVideoCount;
	private TextView mVideoSize;
	private TextView mAudioCount;
	private TextView mAudioSize;
	
	private static boolean mHasOpen = false;
	private static boolean mIsBackuping = false;
	
	//DATAS
	private ServersAdapter mAdapter ;
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.sync_main);
		mDevice = (TextView) this.findViewById(R.id.textDevice);
		String value = DeviceUtil.getEmailAccount(this);
		value = value.split("@")[0];
		String displayText = value +"-"+DeviceUtil.getDeviceName();
		mDevice.setText(displayText);
		mNowPeriod = (TextView) this.findViewById(R.id.textPeriod);
		String[] periods = FileBackup.getFilePeriods(this);
		if(TextUtils.isEmpty(periods[0])){
			mNowPeriod.setText(R.string.file_scanning);
		}else{
			mNowPeriod.setText(getString(R.string.period,periods[0],periods[1]));
		}
		
		long[] datas = FileBackup.getFileInfo(this, Constant.TYPE_IMAGE);
		mPhotoCount = (TextView) this.findViewById(R.id.textPhotoCount);
		mPhotoSize = (TextView) this.findViewById(R.id.textPhotoSize);
		mPhotoCount.setText(getString(R.string.photos, datas[0]));
		mPhotoSize.setText(StringUtil.byteCountToDisplaySize(datas[1]));
		//VIDEO
		datas = FileBackup.getFileInfo(this, Constant.TYPE_VIDEO);
		mVideoCount = (TextView) this.findViewById(R.id.textVideoCount);
		mVideoSize = (TextView) this.findViewById(R.id.textVideoSize);
		mVideoCount.setText(getString(R.string.videos, datas[0]));
		mVideoSize.setText(StringUtil.byteCountToDisplaySize(datas[1]));
		
		//AUDIO
		datas = FileBackup.getFileInfo(this, Constant.TYPE_AUDIO);
		mAudioCount = (TextView) this.findViewById(R.id.textAudioCount);
		mAudioSize = (TextView) this.findViewById(R.id.textAudioSize);
		mAudioCount.setText(getString(R.string.audios, datas[0]));
		mAudioSize.setText(StringUtil.byteCountToDisplaySize(datas[1]));
		
		ListView listview = (ListView) findViewById(R.id.listview);
		ArrayList<ServerEntity> servers = ServersLogic.getServers(this);		
		mAdapter = new ServersAdapter(this,servers);
		listview.setAdapter(mAdapter);
		
		IntentFilter filter = new IntentFilter();
		filter.addAction(Constant.ACTION_BACKUP_FILE);
		filter.addAction(Constant.ACTION_SCAN_FILE);		
		registerReceiver(mReceiver, filter);

        mHandler.postDelayed(new Runnable() {
            public void run() {
                setUp();
            }
            }, 100);
	}
	private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			String action = intent.getAction();
			Log.d(TAG, "action:" + intent.getAction());
			if (Constant.ACTION_BACKUP_FILE.equals(action)) {
				if(RuntimePlayer.isBackuping == false){
					RuntimePlayer.isBackuping = true;
					new BackupFilesTask(context).execute(new Void[]{});
				}
			}
			else if (Constant.ACTION_SCAN_FILE.equals(action)) {
			    refresh();
			}
		}
	};
    private void setUp() {
        WifiManager wifi = (WifiManager) getSystemService(android.content.Context.WIFI_SERVICE);
        lock = wifi.createMulticastLock("mylockthereturn");
        lock.setReferenceCounted(true);
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
					entity.serverOS = si.getPropertyString(Constant.PARAM_SERVER_OS);					
                    entity.wsLocation = "ws://"+si.getHostAddress()+":"+si.getPort();
                    Log.d(TAG, "SERVER NAME:"+entity.serverName);
                    if(RuntimePlayer.OnWebSocketOpened == false ){
	                    if(mHasOpen == false ){
		                    Intent intent = new Intent(MainActivity.this, BonjourServersActivity.class);
		                    intent.putExtra(Constant.PARAM_SERVER_DATA, entity);
		                    MainActivity.this.startActivityForResult(intent, Constant.REQUEST_CODE_OPEN_SERVER_CHOOSER);
		                    mHasOpen = true;
	                    }
	                    else{
	                        mHandler.postDelayed(new Runnable() {
	                            public void run() {
	    	                    	Intent intent = new Intent(Constant.ACTION_BONJOUR_MULTICAT_EVENT);
	    		                    intent.putExtra(Constant.PARAM_SERVER_DATA, entity);
	    		                    MainActivity.this.sendBroadcast(intent);
	                            }
	                            }, 500);

	                    }
                    }
                }

                @Override
                public void serviceRemoved(ServiceEvent ev) {
                	//TODO:
                	ServiceInfo si = ev.getInfo();
                    ServerEntity entity = new ServerEntity();
                	entity.serverName = si.getName();
					entity.serverId = si.getPropertyString(Constant.PARAM_SERVER_ID);
                }

                @Override
                public void serviceAdded(ServiceEvent event) {
                	//TODO:
                	jmdns.requestServiceInfo(event.getType(), event.getName(), 1);
                }
            });
        } catch (IOException e) {
            e.printStackTrace();
            return;
        }
    }
    public void startBackuping(ServerEntity entity){
	    //SETUP WS URL ANDLink to WS
	    boolean isConnected = false;
	    if(RuntimePlayer.OnWebSocketOpened == false){
	    	RuntimeWebClient.init(MainActivity.this);
	    	RuntimeWebClient.setURL(entity.wsLocation);
	    	try {
	    		RuntimeWebClient.open();
	    		//send connect cmd
	    		isConnected = true;
	    	} catch (WebSocketException e) {
	    		isConnected = false;
	    		e.printStackTrace();
	    	}
	    	finally{
	    		if(isConnected){
	    			RuntimePlayer.OnWebSocketOpened = true;
	    			Intent intent = new Intent(Constant.ACTION_BACKUP_FILE);
	    			sendBroadcast(intent);
	    		}
	    	}
	    }
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        // Check which request we're responding to
        if (requestCode == Constant.REQUEST_CODE_OPEN_SERVER_CHOOSER) {
            // Make sure the request was successful
            if (resultCode == RESULT_OK) {                
            	ServerEntity entity = (ServerEntity)data.getExtras().get(Constant.PARAM_RESULT);
            	Log.d(TAG, "WS:"+entity.wsLocation);
            	SharedPreferences prefs = getSharedPreferences(Constant.PREFS_NAME, Context.MODE_PRIVATE);
            	prefs.edit().putString(Constant.PREF_STATION_WEB_SOCKET_URL, entity.wsLocation).commit();
            	entity.status = Constant.SERVER_LINKING;
            	entity.startDatetime ="";
            	entity.endDatetime = "";
            	entity.Folder ="";
            	entity.freespace = "";
            	entity.photoCount ="";
            	entity.videoCount ="";
            	entity.audioCount ="";
            	int result = ServersLogic.updateServer(this, entity);
            	Log.d(TAG, "Update Server:"+result);
        		mAdapter.setData(ServersLogic.getServers(this));
            	startBackuping(entity);
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
		unregisterReceiver(mReceiver);
    	super.onDestroy();
    }
    public void refresh(){
 		mNowPeriod = (TextView) this.findViewById(R.id.textPeriod);
		String[] periods = FileBackup.getFilePeriods(this);
		if(TextUtils.isEmpty(periods[0])){
			mNowPeriod.setText(R.string.file_scanning);
		}else{
			mNowPeriod.setText(getString(R.string.period,periods[0],periods[1]));
		}
				
		long[] datas = FileBackup.getFileInfo(this, Constant.TYPE_IMAGE);
		mPhotoCount.setText(getString(R.string.photos, datas[0]));
		mPhotoSize.setText(StringUtil.byteCountToDisplaySize(datas[1]));
	
		datas = FileBackup.getFileInfo(this, Constant.TYPE_VIDEO);
		mVideoCount.setText(getString(R.string.videos, datas[0]));
		mVideoSize.setText(StringUtil.byteCountToDisplaySize(datas[1]));
		
		datas = FileBackup.getFileInfo(this, Constant.TYPE_AUDIO);
		mAudioCount.setText(getString(R.string.audios, datas[0]));
		mAudioSize.setText(StringUtil.byteCountToDisplaySize(datas[1]));

    }
}
