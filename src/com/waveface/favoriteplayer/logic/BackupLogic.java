package com.waveface.favoriteplayer.logic;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Date;
import java.util.Locale;
import java.util.TreeSet;

import org.jwebsocket.kit.WebSocketException;

import android.app.AlarmManager;
import android.app.PendingIntent;
import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.database.Cursor;
import android.net.Uri;
import android.os.SystemClock;
import android.provider.MediaStore;
import android.text.TextUtils;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.RuntimeState;
import com.waveface.favoriteplayer.db.PairedServersTable;
import com.waveface.favoriteplayer.entity.FileBackupEntity;
import com.waveface.favoriteplayer.util.FileUtil;
import com.waveface.favoriteplayer.util.Log;
import com.waveface.favoriteplayer.util.MediaFile;
import com.waveface.favoriteplayer.util.StringUtil;
import com.waveface.favoriteplayer.websocket.RuntimeWebClient;

public class BackupLogic {
	private static String TAG = BackupLogic.class.getSimpleName();
	public static String DATE_FORMAT = "yyyyMMddHHmmss";
	public static String ISO_DATE_FORMAT = "yyyy-MM-dd";

	public static void setAlarmWakeUpService(Context context) {
		AlarmManager alarmManager = (AlarmManager) context
				.getSystemService(Context.ALARM_SERVICE);
		// ORIGINAL
		// Calendar cal = Calendar.getInstance();
		// cal.setTimeInMillis(System.currentTimeMillis());
		// cal.add(Calendar.SECOND, 10);
		// int type = AlarmManager.RTC_WAKEUP;
		// long triggerTime = cal.getTimeInMillis();
		// Intent intent = new Intent(context, InfiniteReceiver.class);
		// NEW
		int type = AlarmManager.ELAPSED_REALTIME_WAKEUP;
		long triggerTime = SystemClock.elapsedRealtime();
		Intent intent = new Intent(Constant.ACTION_INFINITE_STORAGE_ALARM);
		PendingIntent sender = PendingIntent.getBroadcast(context, 0, intent,
				PendingIntent.FLAG_UPDATE_CURRENT);
		// alarmManager.setRepeating(type, triggerTime,interval, sender);
		alarmManager.setInexactRepeating(type, triggerTime,
				Constant.ALARM_INTERVAL, sender);
	}

	public static long getMaxIdFromMediaDB(Context context, int type) {
		long maxMediaId = 0;
		Uri mediaUri = null;
		switch (type) {
		case Constant.TYPE_IMAGE:
			mediaUri = MediaStore.Images.Media.EXTERNAL_CONTENT_URI;
			break;
		case Constant.TYPE_VIDEO:
			mediaUri = MediaStore.Video.Media.EXTERNAL_CONTENT_URI;
			break;
		case Constant.TYPE_AUDIO:
			mediaUri = MediaStore.Audio.Media.EXTERNAL_CONTENT_URI;
			break;
		}
		Cursor cursor = context.getContentResolver().query(mediaUri, 
				new String[] { MediaStore.Audio.Media._ID },
				null, null, MediaStore.Images.Media._ID + " DESC");
		maxMediaId = cursor.moveToFirst() ? cursor.getLong(cursor
				.getColumnIndex(MediaStore.Audio.Media._ID)) : -1;
		if(cursor!=null){
			cursor.close();
		}
		cursor = null;
		return maxMediaId;
	}
	public static long getModifiedTime(Context context, int type,long mediaId) {
		long dateModified = -1;
		long dateAdded = -1;		
		Uri mediaUri = null;
		switch (type) {
		case Constant.TYPE_VIDEO:
			mediaUri = MediaStore.Video.Media.EXTERNAL_CONTENT_URI;
			break;
		case Constant.TYPE_AUDIO:
			mediaUri = MediaStore.Audio.Media.EXTERNAL_CONTENT_URI;
			break;
		}
		Cursor cursor = context.getContentResolver().query(mediaUri, 
				new String[] 
						{ MediaStore.Video.Media.DATE_MODIFIED,
						MediaStore.Video.Media.DATE_ADDED,
						MediaStore.Video.Media.SIZE,
						MediaStore.Video.Media.DATA
						},
						MediaStore.Images.Media._ID+"=?", 
						new String[]{String.valueOf(mediaId)}, 
						null);
		if(cursor !=null && cursor.getCount()>0){
			cursor.moveToFirst(); 
			dateModified = cursor.getLong(0);
			dateAdded = cursor.getLong(1);		
			Log.d("InfiniteService", "dateAdded:"+dateAdded);
			Log.d("InfiniteService", "size:"+cursor.getLong(2));
			Log.d("InfiniteService", "real size:"+ new File(cursor.getString(3)).length());
			cursor.close();
		}
		cursor = null;
		return dateModified;
	}
	public static long getFileSizeFromDB(Context context, int type,long mediaId) {
		long fileSize = -1;
		Uri mediaUri = null;
		switch (type) {
		case Constant.TYPE_VIDEO:
			mediaUri = MediaStore.Video.Media.EXTERNAL_CONTENT_URI;
			break;
		case Constant.TYPE_AUDIO:
			mediaUri = MediaStore.Audio.Media.EXTERNAL_CONTENT_URI;
			break;
		}
		Cursor cursor = context.getContentResolver().query(mediaUri, 
				new String[] 
						{MediaStore.Video.Media.SIZE},
						MediaStore.Images.Media._ID+"=?", 
						new String[]{String.valueOf(mediaId)}, 
						null);
		if(cursor !=null && cursor.getCount()>0){
			cursor.moveToFirst(); 
			fileSize = cursor.getLong(0);
			cursor.close();
		}
		cursor = null;
		return fileSize;
	}
	public static String getServerBackupedId(Context context) {
		String serverId = null;
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = null;
		try {
			cursor = cr.query(PairedServersTable.CONTENT_URI, new String[] {
					PairedServersTable.COLUMN_SERVER_ID },
					PairedServersTable.COLUMN_STATUS + " NOT IN(?,?)",
					new String[] { Constant.SERVER_DENIED_BY_SERVER,
							   Constant.SERVER_DENIED_BY_CLIENT}, PairedServersTable.COLUMN_SERVER_ID+" LIMIT 1");
			if (cursor != null && cursor.getCount() > 0) {
				cursor.moveToFirst();
				serverId = cursor.getString(0);
			}
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			if (cursor != null) {
				cursor.close();
				cursor = null;
			}
		}
		return serverId;
	}


}
