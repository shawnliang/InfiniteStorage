package com.waveface.favoriteplayer;

import idv.jason.lib.imagemanager.ImageManager;

import android.app.Application;
import android.content.Context;

public class SyncApplication extends Application{
	private ImageManager mImageManager;
	
	public static SyncApplication getWavefacePlayerApplication(Context context) {
		return ((SyncApplication)context.getApplicationContext());
	}
	
	public ImageManager getImageManager() {
		return mImageManager;
	}
	
	@Override
	public void onCreate() {
		super.onCreate();
		
		mImageManager = ImageManager.getInstance(this);
	}
}
