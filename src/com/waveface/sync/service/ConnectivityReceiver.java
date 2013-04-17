package com.waveface.sync.service;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;

import com.waveface.sync.Constant;
import com.waveface.sync.RuntimeConfig;
import com.waveface.sync.util.Log;
import com.waveface.sync.util.NetworkUtil;

public class ConnectivityReceiver extends BroadcastReceiver {
	private static final String TAG = ConnectivityReceiver.class.getSimpleName();
	@Override
	public void onReceive(Context context, Intent intent) {
		Log.d(TAG,"action: " + intent.getAction());
		Log.d(TAG,"LAST TIME NETWORK STATE: " + RuntimeConfig.NETWORK_STATE);
		NetworkInfo info = (NetworkInfo) intent
				.getParcelableExtra(ConnectivityManager.EXTRA_NETWORK_INFO);
		if (info != null && context != null && info.isAvailable()
				&& info.isConnected()) {
			if(context!= null && NetworkUtil.isWifiNetworkAvailable(context)){
				Log.d(TAG, "START WaltzService.mChangeSetTimer ON WIFI");
				RuntimeConfig.NETWORK_STATE = Constant.NETWORK_WIFI;
				Intent intent2 = new Intent(Constant.ACTION_NETWORK_STATE_CHANGED);
				context.sendBroadcast(intent2);
			}
			else if(context!= null && NetworkUtil.isNetworkAvailable(context)){
				Log.d(TAG, "START WaltzService.mChangeSetTimer ON 3G");
				RuntimeConfig.NETWORK_STATE = Constant.NETWORK_3G;
				Intent intent2 = new Intent(Constant.ACTION_NETWORK_STATE_CHANGED);
				context.sendBroadcast(intent2);
			}
		}
		else{
			if(RuntimeConfig.NETWORK_STATE != Constant.NETWORK_UNAVAILABLE){
				RuntimeConfig.NETWORK_STATE = Constant.NETWORK_UNAVAILABLE;
				Intent intent2 = new Intent(Constant.ACTION_NETWORK_STATE_CHANGED);
				context.sendBroadcast(intent2);
			}
		}
		Log.d(TAG,"NETWORK STATE AFTER CHANGE: " + RuntimeConfig.NETWORK_STATE);
	}
}