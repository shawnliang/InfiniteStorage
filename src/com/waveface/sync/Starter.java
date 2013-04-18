package com.waveface.sync;

import java.io.File;

import android.app.Application;
import android.content.Context;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.pm.PackageManager.NameNotFoundException;
import android.os.Environment;

import com.crashlytics.android.Crashlytics;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.task.ScanTask;
import com.waveface.sync.util.ImageUtil;
import com.waveface.sync.util.Log;

public class Starter extends Application {
	private static final String TAG = Starter.class.getSimpleName();
	public static Context SYNC_CONTEXT;
	@Override
	public void onCreate() {
		Log.d(TAG,
				"NativeHeapAllocatedSize:"
						+ android.os.Debug.getNativeHeapAllocatedSize());
		boolean isDebuggable = (0 != (getApplicationInfo().flags & ApplicationInfo.FLAG_DEBUGGABLE));
		if(isDebuggable){
			setupCrashlytics();
		}
		
		super.onCreate();
		SYNC_CONTEXT = getApplicationContext();
		ServersLogic.setAllBackupedServersOffline(SYNC_CONTEXT);
		ServersLogic.purgeAllBonjourServer(this);
		new ScanTask(SYNC_CONTEXT).execute(new Void[]{});
		initialDirectory();
//		ScanImageFolder();
//		ScanVideoFolder();
	}
	private void ScanImageFolder() {
		//Scan Image Folder
		String imageFolders = ImageUtil.findFolders(SYNC_CONTEXT,Constant.TYPE_IMAGE);
		if(imageFolders.length()>0){
			if(imageFolders.endsWith(",")){
				imageFolders = imageFolders.substring(0, imageFolders.length()-1);
			}
		}
//		PhotoImport.addDefaultImportFolder(SYNC_CONTEXT, imageFolders.split(","), Constant.TYPE_IMAGE);
	}
	private void ScanVideoFolder() {
		//Scan Image Folder
		String videoFolders = ImageUtil.findFolders(SYNC_CONTEXT,Constant.TYPE_VIDEO);
		if(videoFolders.length()>0){
			if(videoFolders.endsWith(",")){
				videoFolders = videoFolders.substring(0, videoFolders.length()-1);
			}
		}
//		PhotoImport.addDefaultImportFolder(SYNC_CONTEXT, videoFolders.split(","), Constant.TYPE_VIDEO);
	}
	private void initialDirectory() {
		File dir = Environment.getExternalStorageDirectory();
		File imageFileDir = null;
		imageFileDir = new File(dir, Constant.APP_FOLDER);
		if (!imageFileDir.exists())
			imageFileDir.mkdir();
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
