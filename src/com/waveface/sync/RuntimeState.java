package com.waveface.sync;

import android.content.Context;
import android.content.Intent;

import com.google.gson.Gson;
import com.waveface.sync.util.NetworkUtil;

public class RuntimeState{
	private static final String TAG = RuntimeState.class.getSimpleName();
	
	public static Gson GSON = new Gson();
	public static boolean OnWebSocketStation = false;
	public static boolean OnWebSocketOpened = false;
	
	public static boolean mAutoConnectMode = false;
	public static String mWebSocketServerId ;	
	public static String mWebSocketServerName ;	
	
	public static String mFilename ;	
	public static long mMediaID ;
	public static int mFileType ;	
	
	public static boolean isScaning = false;
	public static boolean isBackuping = false;

	public static boolean isServiceRunnng = false;
	public static boolean isAppLaunching = false;
	public static boolean isNotificationShowing = false;

	public static boolean isMDNSSetUped = false;
	
	
	public static int LastTimeNetworkState = 0;
		
	public static void setServerStatus(String action){
		if(action.equals(Constant.WS_ACTION_SOCKET_OPENED)){
			OnWebSocketOpened = true;			
			OnWebSocketStation = false;
		}
		else if(action.equals(Constant.WS_ACTION_CONNECT)){
			OnWebSocketOpened = true;			
			OnWebSocketStation = false;
		}
		else if(action.equals(Constant.WS_ACTION_ACCEPT)){
			OnWebSocketOpened = true;			
			OnWebSocketStation = true;
		}
		else if(action.equals(Constant.WS_ACTION_DENIED)){
			OnWebSocketOpened = false;			
			OnWebSocketStation = false;
			isBackuping = false;
			mWebSocketServerId = "";
		}
		else if(action.equals(Constant.WS_ACTION_WAIT_FOR_PAIR)){
			OnWebSocketOpened = true;			
			OnWebSocketStation = false;
			mWebSocketServerId = "";
		}
		else if(action.equals(Constant.WS_ACTION_BACKUP_INFO)){
			OnWebSocketOpened = true;			
			OnWebSocketStation = true;
		}
		else if(action.equals(Constant.WS_ACTION_SOCKET_CLOSED) 
				||action.equals(Constant.BS_ACTION_SERVER_REMOVED)
				||action.equals(Constant.NETWORK_ACTION_BROKEN) ){
			OnWebSocketStation = false;
			OnWebSocketOpened = false;
			isBackuping = false;
			mWebSocketServerId = "";
		}
		else if(action.equals(Constant.WS_ACTION_START_BACKUP)){
			isBackuping = true;			
		}
		else if(action.equals(Constant.WS_ACTION_END_BACKUP)){
			isBackuping = false;			
		}		
	}
	public static boolean isWebSocketAvaliable(Context context){
		if(NetworkUtil.isWifiNetworkAvailable(context) 
				&& RuntimeState.OnWebSocketOpened 
				&& RuntimeState.OnWebSocketStation){
			return true;
		}
		else{
			return false;
		}
	}	
	public static boolean canBackup(Context context){
		if(RuntimeState.isScaning == false
    			&& isWebSocketAvaliable(context) 
				&& RuntimeState.isBackuping == false){
			return true;
		}
		else{
			return false;
		}
	}	
	public static void FileBackedUp(Context context){
		Intent intent = new Intent(Constant.ACTION_BACKUP_FILE);
		context.sendBroadcast(intent);
	}
}
