package com.waveface.favoriteplayer;

import android.content.Context;

import com.google.gson.Gson;
import com.waveface.favoriteplayer.util.NetworkUtil;

public class RuntimeState{
	private static final String TAG = RuntimeState.class.getSimpleName();
	
	public static Gson GSON = new Gson();
	public static boolean OnWebSocketOpened = false;
	
	public static boolean mAutoConnectMode = false;
	public static String mWebSocketServerId ;	
	public static String mWebSocketServerName ;	
	
	public static String mFilename ;	
	public static long mMediaID ;
	public static String mFileDatetime ;		
	public static String mFileUpdatedDate ;	
	public static int mFileType ;	
	
	
	public static boolean isPhotoScaning = false;
	public static boolean isVideoScaning = false;
	public static boolean isAudioScaning = false;	
	public static boolean isScaning = false;
	public static boolean wasFirstTimeImportScanDone = false;	
	public static boolean isBackuping = false;
	public static boolean isSyncing = false;
	

	public static boolean isNotificationShowing = false;

	public static boolean isMDNSSetUped = false;
	
	public static long maxImageId = -1;
	public static long maxVideoId = -1;
	public static long maxAudioId = -1;
	
	public static int LastTimeNetworkState = 0;
		
	public static void setServerStatus(String action){
		if(action.equals(Constant.WS_ACTION_SOCKET_OPENED)){
			OnWebSocketOpened = true;			
		}
		else if(action.equals(Constant.WS_ACTION_CONNECT)){
			OnWebSocketOpened = true;			
		}
		else if(action.equals(Constant.WS_ACTION_ACCEPT)){
			OnWebSocketOpened = true;			
		}
		else if(action.equals(Constant.WS_ACTION_DENIED)){
			OnWebSocketOpened = false;			
			isBackuping = false;
			mWebSocketServerId = "";
		}
		else if(action.equals(Constant.WS_ACTION_WAIT_FOR_PAIR)){
			OnWebSocketOpened = true;			
			mWebSocketServerId = "";
		}
		else if(action.equals(Constant.WS_ACTION_BACKUP_INFO)){
			OnWebSocketOpened = true;			
		}
		else if(action.equals(Constant.WS_ACTION_SOCKET_CLOSED) 
				||action.equals(Constant.BS_ACTION_SERVER_REMOVED)
				||action.equals(Constant.NETWORK_ACTION_WIFI_BROKEN) ){
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
				&& RuntimeState.OnWebSocketOpened){
			return true;
		}
		else{
			return false;
		}
	}	
	public static boolean canBackup(Context context){
//		if(RuntimeState.isScaning == false
//    			&& isWebSocketAvaliable(context) 
//				&& RuntimeState.isBackuping == false){
		if(isWebSocketAvaliable(context) 
				&& RuntimeState.isBackuping == false){
			return true;
		}
		else{
			return false;
		}
	}	
	
}
