package com.waveface.sync.ui;

import java.io.IOException;

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
import android.widget.TextView;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.RuntimePlayer;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.logic.FileImport;
import com.waveface.sync.task.BackupTask;
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
    private Handler handler = new android.os.Handler();
    private JmDNS jmdns = null;
    private ServiceListener listener = null;
    //UI
	private TextView mDevice;
	private TextView mNowPeriod;
	private static boolean mHasOpen = false;
	private static boolean mIsBackuping = false;
	
	private static int OPEN_SERVER_CHOOSER = 1;
	
	
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
		String[] periods = FileImport.getFilePeriods(this);
		displayText = "From "+periods[0]+" to "+periods[1];
		mNowPeriod.setText(displayText);
		
		long[] datas = FileImport.getFileInfo(this, Constant.TYPE_IMAGE);
		TextView tv = (TextView) this.findViewById(R.id.textPhotoCount);
		displayText = "Total "+datas[0]+" photos.";
		tv.setText(displayText);
		tv = (TextView) this.findViewById(R.id.textPhotoSize);
		displayText = StringUtil.byteCountToDisplaySize(datas[1]);
		tv.setText(displayText);
	
		datas = FileImport.getFileInfo(this, Constant.TYPE_VIDEO);
		tv = (TextView) this.findViewById(R.id.textVideoCount);
		displayText = "Total "+datas[0]+" videos.";
		tv.setText(displayText);
		tv = (TextView) this.findViewById(R.id.textVideoSize);
		displayText = StringUtil.byteCountToDisplaySize(datas[1]);
		tv.setText(displayText);
		
		IntentFilter filter = new IntentFilter();
		filter.addAction(Constant.ACTION_BACKUP_FILE);
		registerReceiver(mReceiver, filter);

        handler.postDelayed(new Runnable() {
            public void run() {
                setUp();
            }
            }, 1000);
	}
	private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			String action = intent.getAction();
			Log.d(TAG, "action:" + intent.getAction());
			if (Constant.ACTION_BACKUP_FILE.equals(action)) {
				new BackupTask(context).execute(new Void[]{});
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
            jmdns.addServiceListener(Constant.INFINTE_STORAGE, listener = new ServiceListener() {
                @Override
                public void serviceResolved(ServiceEvent ev) {
                	@SuppressWarnings("deprecation")
                	ServiceInfo si = ev.getInfo();
                    ServerEntity entity = new ServerEntity();
                	entity.serverName = si.getName();
					entity.serverId = si.getPropertyString("server_id");
                    entity.wsLocation = "ws://"+si.getHostAddress()+":"+si.getPort();
                    if(RuntimePlayer.OnWebSocketOpened == false){
	                    if(mHasOpen == false ){
		                    Intent intent = new Intent(MainActivity.this, BonjourServersActivity.class);
		                    intent.putExtra("ServerDATA", entity);
		                    MainActivity.this.startActivityForResult(intent, OPEN_SERVER_CHOOSER);
		                    mHasOpen = true;
	                    }
	                    else{
		                    Intent intent = new Intent(Constant.ACTION_BONJOUR_MULTICAT_EVENT);
		                    intent.putExtra("ServerDATA", entity);
		                    MainActivity.this.sendBroadcast(intent);
	                    }
                    }
                }

                @Override
                public void serviceRemoved(ServiceEvent ev) {
                	//TODO:
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
        if (requestCode == OPEN_SERVER_CHOOSER) {
            // Make sure the request was successful
            if (resultCode == RESULT_OK) {                
            	ServerEntity entity = (ServerEntity)data.getExtras().get("result");
            	Log.d(TAG, "WS:"+entity.wsLocation);
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

}
