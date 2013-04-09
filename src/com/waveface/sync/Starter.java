package com.waveface.sync;

import android.app.Application;
import android.content.Context;

import com.waveface.sync.logic.PhotoImport;
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
		super.onCreate();
		SYNC_CONTEXT = getApplicationContext();
		ScanImageFolder();
	}
	private void ScanImageFolder() {
		//Scan Image Folder
		String imageFolders = ImageUtil.findAlbums(SYNC_CONTEXT);
		if(imageFolders.length()>0){
			if(imageFolders.endsWith(",")){
				imageFolders = imageFolders.substring(0, imageFolders.length()-1);
			}
		}
		PhotoImport.addDefaultImportFolder(SYNC_CONTEXT, imageFolders.split(","), "P");
	}

}
