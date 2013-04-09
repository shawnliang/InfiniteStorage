package com.waveface.sync.service;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.util.Log;

public class AlarmReceiver extends BroadcastReceiver {
	private static final String TAG = AlarmReceiver.class.getSimpleName();
	@Override
	public void onReceive(Context context, Intent intent) {
		Log.d(TAG, "ALERT FROM SYSTEM SERVICE");
//		try {
//			SharedPreferences prefs = context.getSharedPreferences(
//					Constant.PREFS_NAME, Context.MODE_PRIVATE);
//			int hour = prefs.getInt(Constant.PREF_AUTO_IMPORT_HOUR, -1);
//			if (PhotoImport.enableAutoImport(context)) {
//				if(RuntimePlayer.isImportProcessing == false && RuntimePlayer.OlderImportProcessing == false){
//				    if(prefs.getBoolean(Constant.PREF_AUTO_IMPORT_FIRST_TIME_DONE, false)){
//						Log.d(TAG, "INVOKE Import Task FOR NEW DATA");
//						Intent broadIntent = new Intent(Constant.ACTION_PHOTO_AUTO_IMPORTING);
//						context.sendBroadcast(broadIntent);
//						RuntimePlayer.isImportProcessing = true;
//						new InstantImportTask(context).execute(new Void[] {});
//				    }
//				}
//				if(hour!=-1){
//					PhotoImport.setImportAlarmCondition(context);
//				}
//				int[] dataCount = SyncStatusLogic.getUploadImageStatus(context);
//				if(HttpInvoker.isNetworkAvailable(context)){
//					if(SyncStatusLogic.needUploadImageThumb(dataCount)){
//						if(!RuntimePlayer.UploadImageQueueBackground){
//							RuntimePlayer.UploadImageQueueBackground = true;
//							new DummyImagUploadTask(context).execute(new Void[] {});
//						}
//					}
//				}
//			}
//		} catch (Exception e) {
//			e.printStackTrace();
//		}
	}
}
