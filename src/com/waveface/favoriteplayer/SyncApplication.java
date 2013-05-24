package com.waveface.favoriteplayer;

import idv.jason.lib.imagemanager.ImageManager;

import java.io.File;

import android.app.Application;
import android.content.Context;
import android.content.Intent;
import android.os.Environment;

public class SyncApplication extends Application{
	private ImageManager mImageManager;
	
	public static SyncApplication getWavefacePlayerApplication(Context context) {
		return ((SyncApplication)context.getApplicationContext());
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

	public ImageManager getImageManager() {
		return mImageManager;
	}
	
	@Override
	public void onCreate() {
		super.onCreate();
		initialDirectory();
		mImageManager = ImageManager.getInstance(this);
		mImageManager.setDownloadPath(new File(Environment.getExternalStorageDirectory(), "FavoritePlayer/Images").getAbsolutePath());

		sendBroadcast(new Intent(Constant.ACTION_FAVORITE_PLAYER_ALARM));
	}
}
