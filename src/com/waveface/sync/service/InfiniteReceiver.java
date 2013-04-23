package com.waveface.sync.service;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.widget.Toast;

import com.waveface.sync.Constant;
import com.waveface.sync.RuntimeState;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.util.AppUtil;
import com.waveface.sync.util.NetworkUtil;

public class InfiniteReceiver extends BroadcastReceiver {
	private static final String TAG = InfiniteReceiver.class.getSimpleName();
	@Override
	public void onReceive(Context context, Intent intent) {
		RuntimeState.isServiceRunnng = 
				AppUtil.isThisServiceRunning(context,InfiniteService.class.getName());
//		Toast.makeText(context, "context is "+context+",isServiceRunnng:"+RuntimeState.isServiceRunnng, Toast.LENGTH_LONG).show();
		if(context!= null){ 
			 if(NetworkUtil.isWifiNetworkAvailable(context) 
					 && RuntimeState.isServiceRunnng == false){
				Toast.makeText(context, "START Bonjour Service ON WIFI", Toast.LENGTH_LONG).show();   
				context.startService(new Intent(context, InfiniteService.class));
			}
			else if(!NetworkUtil.isNetworkAvailable(context) 
					&& RuntimeState.isServiceRunnng){
			    Toast.makeText(context, "STOP Bonjour Service FOR WITHOUT NETWORK", Toast.LENGTH_LONG).show();   
				context.stopService(new Intent(context, InfiniteService.class));
			}
		}
		if(context!= null){
			if(RuntimeState.LastTimeNetworkState == Constant.NETWORK_UNAVAILABLE){
				if(NetworkUtil.isWifiNetworkAvailable(context)){
					RuntimeState.LastTimeNetworkState = Constant.NETWORK_WIFI;
					Intent inte = new Intent(Constant.ACTION_NETWORK_STATE_CHANGE);
					inte.putExtra(Constant.EXTRA_NETWROK_STATE, Constant.NETWORK_ACTION_WIFI_CONNECTED);
		        	context.sendBroadcast(inte);
		        	RuntimeState.LastTimeNetworkState = Constant.NETWORK_WIFI;
				}				
			}
			else if(RuntimeState.LastTimeNetworkState == Constant.NETWORK_WIFI){
				if(!NetworkUtil.isNetworkAvailable(context)){
					RuntimeState.LastTimeNetworkState = Constant.NETWORK_UNAVAILABLE;
					RuntimeState.setServerStatus(Constant.NETWORK_ACTION_BROKEN);
					ServersLogic.updateAllBackedServer(context);					
					Intent inte = new Intent(Constant.ACTION_NETWORK_STATE_CHANGE);
					inte.putExtra(Constant.EXTRA_NETWROK_STATE, Constant.NETWORK_ACTION_BROKEN);
		        	context.sendBroadcast(inte);
				}
			}
			
		}
	}
}
