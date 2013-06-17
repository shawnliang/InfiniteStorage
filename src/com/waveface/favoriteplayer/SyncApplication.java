package com.waveface.favoriteplayer;

import idv.jason.lib.imagemanager.ImageManager;

import java.io.File;

import android.app.Application;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Environment;
import android.text.TextUtils;

import com.waveface.favoriteplayer.util.DeviceUtil;
import com.waveface.favoriteplayer.util.Log;

public class SyncApplication extends Application{
    public static final String TAG = SyncApplication.class.getSimpleName();
	private SharedPreferences mPrefs ;
    private String mDownloadFolder; 
	private ImageManager mImageManager;
	
	public static SyncApplication getWavefacePlayerApplication(Context context) {
		return ((SyncApplication)context.getApplicationContext());
	}
	
	private void initialDirectory() {
		String folder = null;
		String deviceName = DeviceUtil.getDeviceName();
		if (getPackageManager().hasSystemFeature("com.google.android.tv") && deviceName.equals("asus_google_cube")) {
			Log.d(TAG, "Running on Google TV!");
			Log.d(TAG, "deviceName:"+deviceName);
			File root = new File("/mnt/media");
			File[] files = root.listFiles();
			File firstLevel = null;
			for(int i =0 ; i < files.length;i++){
				firstLevel = files[i];
				if(firstLevel.isDirectory()){
					Log.d(TAG, "first dir:"+firstLevel.getAbsolutePath());
					File[] secondLevel = firstLevel.listFiles();
					if(secondLevel!=null){
						folder = firstLevel.getAbsolutePath();
						break;
//						for(int j =0 ; j < secondLevel.length;j++){
//							Log.d(TAG, "second dir:"+secondLevel[j].getAbsolutePath());					
//						}
					}
				}
			}
		}
		
		File dir = null;
		if( TextUtils.isEmpty(folder)){
			dir = Environment.getExternalStorageDirectory();
		}
		else{
			dir = new File(folder);
		}
		mPrefs.edit().putString(Constant.PREF_DOWNLOAD_FOLDER, dir.getAbsolutePath()).commit();
		Log.d(TAG, "Environment external dir:"+dir.getAbsolutePath());		
		File fileDir = null;
		fileDir = new File(dir, Constant.APP_FOLDER);
		if (!fileDir.exists())
			fileDir.mkdir();
		fileDir = new File(dir, Constant.VIDEO_FOLDER);
		if (!fileDir.exists())
			fileDir.mkdir();
		fileDir = new File(dir, Constant.IMAGE_FOLDER);
		if (!fileDir.exists())
			fileDir.mkdir();		
	}
	
	public ImageManager getImageManager() {
		return mImageManager;
	}
	
	@Override
	public void onCreate() {
		super.onCreate();
		mPrefs = getApplicationContext().getSharedPreferences(Constant.PREFS_NAME, Context.MODE_PRIVATE);
		initialDirectory();
		mImageManager = ImageManager.getInstance(this);
		mDownloadFolder = mPrefs.getString(Constant.PREF_DOWNLOAD_FOLDER, "");
		if(TextUtils.isEmpty(mDownloadFolder)){
			mPrefs.edit().putString(Constant.PREF_DOWNLOAD_FOLDER, Environment.getExternalStorageDirectory().getAbsolutePath()).commit();
			mImageManager.setDownloadPath(new File(Environment.getExternalStorageDirectory(), "Favorites/Images").getAbsolutePath());		
		}
		else{
			mImageManager.setDownloadPath(new File(new File(mDownloadFolder), "Favorites/Images").getAbsolutePath());			
		}
		sendBroadcast(new Intent(Constant.ACTION_FAVORITE_PLAYER_ALARM));
	}
}
