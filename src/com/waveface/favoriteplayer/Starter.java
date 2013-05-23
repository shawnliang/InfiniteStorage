package com.waveface.favoriteplayer;

import java.io.File;

import android.app.Application;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.pm.PackageManager.NameNotFoundException;
import android.os.Environment;

import com.crashlytics.android.Crashlytics;

public class Starter extends Application {
	private static final String TAG = Starter.class.getSimpleName();	
	
	@Override
	public void onCreate() {
//		boolean isDebuggable = (0 != (getApplicationInfo().flags & ApplicationInfo.FLAG_DEBUGGABLE));
//		if(isDebuggable){
//			setupCrashlytics();
//		}
		
		initialDirectory();
		super.onCreate();
	}

	private void initialDirectory() {
		File dir = Environment.getExternalStorageDirectory();
		File fileDir = null;
		fileDir = new File(dir, Constant.APP_FOLDER);
		if (!fileDir.exists())
			fileDir.mkdir();
		fileDir = new File(dir, Constant.VIDEO_FOLDER);
		if (!fileDir.exists())
			fileDir.mkdir();
		
	}
	public void setupCrashlytics() {
		PackageManager packageManager = getPackageManager();
		String packageName = getPackageName();
		PackageInfo info = null;
		try {
			info = packageManager.getPackageInfo(packageName, 0);
		} catch (NameNotFoundException e) {
		}

		Crashlytics.start(this);
		if (info != null) {
			Crashlytics.setInt("android:versionCode", info.versionCode);
			Crashlytics.setString("android:versionName", info.versionName);
		} else {
			Crashlytics.setString("android:versionCode", "can not get");
			Crashlytics.setString("android:versionName", "can not get");
		}
	}
}
