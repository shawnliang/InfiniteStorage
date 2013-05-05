package com.waveface.sync.service;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.text.TextUtils;
import android.util.Log;
import android.widget.Toast;

import com.waveface.sync.Constant;
import com.waveface.sync.RuntimeState;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.util.AppUtil;
import com.waveface.sync.util.NetworkUtil;
import com.waveface.sync.util.StringUtil;

public class InfiniteReceiver extends BroadcastReceiver {
	private static final String TAG = InfiniteReceiver.class.getSimpleName();
	@Override
	public void onReceive(Context context, Intent intent) {
		String action = intent.getAction();
//		RuntimeState.isServiceRunnng = 
//				AppUtil.isThisServiceRunning(context,InfiniteService.class.getName());
//		Toast.makeText(context, "action:"+action+",isServiceRunnng:"+RuntimeState.isServiceRunnng, Toast.LENGTH_LONG).show();
//		Log.d(TAG, "time:"+StringUtil.getLocalDate()+":action:"+action);
		if(context!= null){ 
//			 if(RuntimeState.isServiceRunnng == false){
//			Toast.makeText(context, "START Infinite Service", Toast.LENGTH_LONG).show();   
			context.startService(new Intent(context, InfiniteService.class));
//			}
			if(!TextUtils.isEmpty(action) && action.equals("android.net.conn.CONNECTIVITY_CHANGE")){
				Intent inte = new Intent(Constant.ACTION_NETWORK_STATE_CHANGE);

				if(RuntimeState.LastTimeNetworkState == Constant.NETWORK_UNAVAILABLE){
					if(NetworkUtil.isWifiNetworkAvailable(context)){
						RuntimeState.LastTimeNetworkState = Constant.NETWORK_WIFI;
						inte.putExtra(Constant.EXTRA_NETWROK_STATE, Constant.NETWORK_ACTION_WIFI_CONNECTED);
			        	RuntimeState.LastTimeNetworkState = Constant.NETWORK_WIFI;
					}				
				}
				else if(RuntimeState.LastTimeNetworkState == Constant.NETWORK_WIFI){
					if(!NetworkUtil.isWifiNetworkAvailable(context)){
						RuntimeState.LastTimeNetworkState = Constant.NETWORK_UNAVAILABLE;
						RuntimeState.setServerStatus(Constant.NETWORK_ACTION_BROKEN);
						ServersLogic.updateAllBackedServerOffline(context);					
						inte.putExtra(Constant.EXTRA_NETWROK_STATE, Constant.NETWORK_ACTION_BROKEN);
					}
				}
	        	context.sendBroadcast(inte);
			}
		}
	}
}
