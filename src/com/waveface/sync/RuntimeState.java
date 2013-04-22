package com.waveface.sync;

import com.google.gson.Gson;

public class RuntimeState{
	private static final String TAG = RuntimeState.class.getSimpleName();
	
	public static Gson GSON = new Gson();
	public static boolean OnWebSocketStation = false;
	public static boolean OnWebSocketOpened = false;
	
	public static boolean mAutoConnectMode = false;
	public static String mWebSocketServerId ;	
	
	public static boolean isScaning = false;
	public static boolean isBackuping = false;
	
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
			mWebSocketServerId = "";
		}
	}
}
