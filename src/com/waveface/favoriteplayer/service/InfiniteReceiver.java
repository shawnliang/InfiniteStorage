package com.waveface.favoriteplayer.service;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.text.TextUtils;
import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.RuntimeState;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.util.NetworkUtil;

public class InfiniteReceiver extends BroadcastReceiver {
	private static final String TAG = InfiniteReceiver.class.getSimpleName();
	@Override
	public void onReceive(Context context, Intent intent) {
		String action = intent.getAction();
		if(context!= null){ 
			context.startService(new Intent(context, InfiniteService.class));
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
						RuntimeState.setServerStatus(Constant.NETWORK_ACTION_WIFI_BROKEN);
				    	ServersLogic.updateAllBackedServerStatus(context,Constant.SERVER_OFFLINE);
						inte.putExtra(Constant.EXTRA_NETWROK_STATE, Constant.NETWORK_ACTION_WIFI_BROKEN);
					}
				}
	        	context.sendBroadcast(inte);
			}
		}
	}
}
