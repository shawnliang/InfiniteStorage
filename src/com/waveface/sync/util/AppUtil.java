package com.waveface.sync.util;

import java.util.List;

import android.app.ActivityManager;
import android.app.ActivityManager.RunningAppProcessInfo;
import android.app.ActivityManager.RunningServiceInfo;
import android.content.Context;

public class AppUtil {
	public static boolean isThisServiceRunning(final Context context,
			final String servicePackageName) {
		final ActivityManager manager = (ActivityManager) context
				.getSystemService(Context.ACTIVITY_SERVICE);
		for (final RunningServiceInfo service : manager
				.getRunningServices(Integer.MAX_VALUE)) {
			if (servicePackageName.equals(service.service.getClassName())) {
				return true;
			}
		}
		return false;
	}

	public static boolean isThisApplicationRunning(final Context context,
			final String appPackage) {
		if (appPackage == null) {
			return false;
		}
		final ActivityManager manager = (ActivityManager) context
				.getSystemService(Context.ACTIVITY_SERVICE);
		final List<RunningAppProcessInfo> runningAppProcesses = manager
				.getRunningAppProcesses();
		for (final RunningAppProcessInfo app : runningAppProcesses) {
			if (appPackage.equals(app.processName)) {
				return true;
			}
		}
		return false;
	}
}
