package com.waveface.sync.service;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;

import com.waveface.sync.RuntimeConfig;
import com.waveface.sync.util.Log;
import com.waveface.sync.util.NetworkUtil;

public class ConnectivityReceiver extends BroadcastReceiver {
	private static final String TAG = ConnectivityReceiver.class.getSimpleName();
	@Override
	public void onReceive(Context context, Intent intent) {
		Log.d(TAG,"action: " + intent.getAction());
		if(context!= null && NetworkUtil.isWifiNetworkAvailable(context)){
			Log.d(TAG, "WIFI NETWORK");			
			if(RuntimeConfig.hasServiceCreated == false 
					&& RuntimeConfig.isAppRunning==false){
				Log.d(TAG, "START Bonjour Service ON WIFI");
				context.startService(new Intent(context, BonjourService.class));
			}
		}
		else if(context!= null && NetworkUtil.isNetworkAvailable(context)){
			Log.d(TAG, "3G NETWORK");
		}
		else{
			Log.d(TAG, "THERE'S NO NETWORK");
			if(RuntimeConfig.hasServiceCreated 
					&& RuntimeConfig.isAppRunning==false){
				Log.d(TAG, "START Bonjour Service WHEN NETWORK IS DOWNLOAD");
				context.stopService(new Intent(context, BonjourService.class));
			}
		}
	}
}