package com.waveface.favoriteplayer.util;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.NetworkInfo.State;
import android.net.wifi.SupplicantState;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;

public class NetworkUtil {
	private static final String TAG = NetworkUtil.class.getSimpleName();


	public static boolean isNetworkAvailable(Context context) {
		if(context == null){
			return false;
		}
		ConnectivityManager cm = (ConnectivityManager) context
				.getSystemService(Context.CONNECTIVITY_SERVICE);
		NetworkInfo netInfo = cm.getActiveNetworkInfo();
		if (netInfo!=null && netInfo.isConnected())
			return true;
		Log.w(TAG, "Internet Connection Not Present");
		return false;
	}

	public static boolean isWifiNetworkAvailable(Context context) {
		if(context == null){
			return false;
		}
		ConnectivityManager cm = (ConnectivityManager) context
				.getSystemService(Context.CONNECTIVITY_SERVICE);
		State wifi = cm.getNetworkInfo(ConnectivityManager.TYPE_WIFI).getState();
		if(wifi == State.CONNECTED ){
			return true;
		}
		else{
			Log.v(TAG, "Wi-Fi is Not Present");
			return false;
		}
	}

	public static void getWifiSSID(Context mContext) {
		WifiManager wifiManager = (WifiManager) mContext
				.getSystemService(Context.WIFI_SERVICE);
		if (wifiManager != null) {
			WifiInfo info = wifiManager.getConnectionInfo();
			if (info != null) {
				String ssid = info.getSSID();
				int IpAddress = info.getIpAddress();
				int LinkSpeed = info.getLinkSpeed();
				int NetworkId = info.getNetworkId();
				SupplicantState state = info.getSupplicantState();
				Log.d(TAG, "SSID:" + ssid + ",IpAddress:" + IpAddress
						+ ",LinkSpeed:" + LinkSpeed + ",NetworkId:" + NetworkId
						+ ",SupplicantState:" + state.name());
			}

		}
	}
}
