package com.waveface.sync.ui;

import java.util.HashMap;

import android.app.Activity;
import android.app.Notification;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;

import com.waveface.sync.R;
import com.waveface.sync.util.Log;
import com.waveface.sync.util.SyncNotificationManager;

public class CancelNotification extends Activity implements OnClickListener {
	private final String mNotificationId = null;
	public static final String TAG = CancelNotification.class.getSimpleName();
	private SyncNotificationManager mNotificationManager;

	@Override
	public void onCreate(Bundle bundle) {
		super.onCreate(bundle);
		this.finish();
	}

	@Override
	public void onClick(View v) {
		switch (v.getId()) {
//		case R.id.btn_yes:
//			if (mNotificationId != null) {
//				mNotificationManager = SyncNotificationManager
//						.getInstance(getApplicationContext());
//				if (mNotificationManager != null) {
//					HashMap<String, Notification> maps = mNotificationManager
//							.getAllNotify();
//					if (maps.containsKey(mNotificationId)) {
//						mNotificationManager
//								.cancelNotification(mNotificationId);
//						Notification notification = mNotificationManager
//								.getNotication(mNotificationId);
//						Log.d(TAG, "notification object:" + notification);
//						if (notification != null) {
//							notification.flags = notification.flags
//									| Notification.FLAG_AUTO_CANCEL;
//							notification.contentView.setViewVisibility(
//									R.drawable.ic_launcher, View.GONE);
//						}
//					}
//				}
//			} else {
//				mNotificationManager.cancelAll();
//			}
//			this.finish();
//			break;
//		case R.id.btn_no:
//			this.finish();
//			break;
		}
	}

}
