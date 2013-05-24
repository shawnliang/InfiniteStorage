package com.waveface.favoriteplayer.logic;

import java.io.File;

import android.app.AlarmManager;
import android.app.PendingIntent;
import android.content.ContentResolver;
import android.content.Context;
import android.content.Intent;
import android.database.Cursor;
import android.net.Uri;
import android.os.SystemClock;
import android.provider.MediaStore;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.db.PairedServersTable;
import com.waveface.favoriteplayer.util.Log;

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
		Intent intent = new Intent(Constant.ACTION_FAVORITE_PLAYER_ALARM);
		PendingIntent sender = PendingIntent.getBroadcast(context, 0, intent,
				PendingIntent.FLAG_UPDATE_CURRENT);
		// alarmManager.setRepeating(type, triggerTime,interval, sender);
		alarmManager.setInexactRepeating(type, triggerTime,
				Constant.ALARM_INTERVAL, sender);
	}

}
