package com.waveface.sync.util;

import java.util.HashMap;

import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.widget.RemoteViews;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.ui.CancelNotification;

public class SyncNotificationManager {
	private volatile static SyncNotificationManager mInstance;
	public static final String TAG = SyncNotificationManager.class.getSimpleName();

	private NotificationManager mNotificationManager = null;
	private Context mContext = null;
	private final HashMap<String, Notification> mNotifications;

	private SyncNotificationManager(Context context) {
		mContext = context;

		mNotificationManager = (NotificationManager) mContext
				.getSystemService(Context.NOTIFICATION_SERVICE);

		mNotifications = new HashMap<String, Notification>();
	}

	public static SyncNotificationManager getInstance(Context context) {
		if (mInstance == null) {
			synchronized (SyncNotificationManager.class) {
				if (mInstance == null) {
					mInstance = new SyncNotificationManager(context);
				}
			}
		}
		return mInstance;
	}

	public void cancelAll() {
		Log.d(TAG, "mNotificationManager is :" + mNotificationManager);
		if (mNotificationManager == null) {
			mNotificationManager = (NotificationManager) mContext
					.getSystemService(Context.NOTIFICATION_SERVICE);
		}
		mNotificationManager.cancelAll();
	}

	public void cancelNotification(String id) {
		if (mNotificationManager == null) {
			mNotificationManager = (NotificationManager) mContext
					.getSystemService(Context.NOTIFICATION_SERVICE);
		}
		mNotificationManager.cancel(id.hashCode());

		if (mNotifications.containsKey(id)) {
			mNotifications.remove(id);
		}
	}

	public Notification getNotication(String id) {
		if (mNotifications != null) {
			return mNotifications.get(id);
		} else
			return null;
	}
	public void removeNotication(String id) {
		if (mNotifications != null)
			mNotifications.remove(id);
	}


	public HashMap<String, Notification> getAllNotify() {
		return mNotifications;
	}

	public void createTextNotification(String id, String title, String content,
			Intent intent) {
		mNotificationManager.cancel(id.hashCode());
		if (intent == null)
			intent = new Intent();

		@SuppressWarnings("deprecation")
		Notification notification = new Notification(R.drawable.ic_launcher,
				title, System.currentTimeMillis());
		final PendingIntent pendingIntent = PendingIntent.getActivity(mContext,
				0, intent, 0);

		notification.flags |= Notification.FLAG_AUTO_CANCEL;
		notification.contentView = new RemoteViews(mContext.getPackageName(),
				R.layout.text_notification);
		notification.contentIntent = pendingIntent;
		notification.contentView.setImageViewResource(R.id.status_icon,
				R.drawable.ic_launcher);
		notification.contentView.setTextViewText(R.id.subject_text, title);
		notification.contentView.setTextViewText(R.id.content_text, content);
		mNotificationManager.notify(id.hashCode(), notification);
		mNotifications.put(id, notification);
	}

	public void createProgressNotification(String id, String title,
			String content) {
//		Intent intent = new Intent(mContext, CancelNotification.class);
		Intent intent = new Intent();
		intent.putExtra(Constant.EXTRA_NOTIFICATION_ID, id);
		final PendingIntent pendingIntent = PendingIntent.getActivity(mContext,
				0, intent, 0);
		createProgressNotification(id, title, content,0, pendingIntent);
	}

	public void createProgressNotification(String id, String title,
			String content,int progress) {
//		Intent intent = new Intent(mContext, CancelNotification.class);
		Intent intent = new Intent();
		
		intent.putExtra(Constant.EXTRA_NOTIFICATION_ID, id);
		final PendingIntent pendingIntent = PendingIntent.getActivity(mContext,
				0, intent, 0);
		createProgressNotification(id, title, content,progress, pendingIntent);
	}

	public void createProgressNotification(String id, String title,
			String content, PendingIntent pendingIntent) {

		createProgressNotification(id, title,content,0,pendingIntent);
	}
	public void createProgressNotification(String id, String title,
			String content,int progress, PendingIntent pendingIntent) {
		if (mNotifications.containsKey(id)) {
			mNotificationManager.cancel(id.hashCode());
			mNotifications.remove(id);
		}
		cancelNotification(id);

		Notification notification = new Notification(R.drawable.ic_launcher,
				title, System.currentTimeMillis());
		notification.flags |= Notification.FLAG_AUTO_CANCEL;
		notification.contentView = new RemoteViews(mContext.getPackageName(),
				R.layout.progress_notification);
		notification.contentIntent = pendingIntent;
		notification.contentView.setImageViewResource(R.id.status_icon,
				R.drawable.ic_launcher);
		notification.contentView.setTextViewText(R.id.status_text, content);
		notification.contentView.setProgressBar(R.id.status_progress, 100, progress,
				false);

		mNotificationManager.notify(id.hashCode(), notification);
		mNotifications.put(id, notification);
	}


	public void updateProgressNotification(String id, int progress) {
		if (mNotifications.containsKey(id)) {
			Notification notification = mNotifications.get(id);
			if (notification != null) {
				notification.contentView.setProgressBar(R.id.status_progress,
						100, progress, false);
				notification.contentView.setTextViewText(R.id.status_text,
						Integer.toString(progress) + "%");
				mNotificationManager.notify(id.hashCode(), notification);
			}
		}
	}
	public void updateProgressNotification(String id, String content,int progress) {
		if (mNotifications.containsKey(id)) {
			Notification notification = mNotifications.get(id);
			if (notification != null) {
				notification.contentView.setProgressBar(R.id.status_progress,
						100, progress, false);
				notification.contentView.setTextViewText(R.id.status_text,
						content);
				mNotificationManager.notify(id.hashCode(), notification);
			}
		}
	}

}
