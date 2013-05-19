package com.waveface.favoriteplayer.ui.fragment;

import java.util.ArrayList;


import android.app.Activity;
import android.app.ProgressDialog;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Handler;
import android.support.v4.app.Fragment;
import android.text.TextUtils;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.RelativeLayout;
import android.widget.TextView;
import android.widget.Toast;

import com.google.analytics.tracking.android.EasyTracker;

//import com.waveface.mdns.DNSThread;
import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.RuntimeState;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.image.MediaStoreImage;
import com.waveface.favoriteplayer.logic.BackupLogic;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.service.InfiniteService;
import com.waveface.favoriteplayer.ui.FirstUseActivity;
import com.waveface.favoriteplayer.util.NetworkUtil;

public class SyncFragment extends Fragment implements OnClickListener {

	private String TAG = SyncFragment.class.getSimpleName();

	private Handler mHandler = new Handler();

	// UI
	private RelativeLayout mRLSetting;
	private ImageView mIvAddPc;
	private TextView tvConnectPC;

	

	private ProgressDialog mProgressDialog;
	

	private SharedPreferences mPrefs;
	private Editor mEditor;
	
//    private DNSThread dnsThread = null;

	private final static int IMAGE_HEIGHT = 110;
	private final static int IMAGE_WIDTH = 110;
	// DATA
	private ArrayList<ServerEntity> mPairedServers;

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		Log.d(TAG, "onCreateView");
		//START UP SERVICE
		new InvokeServiceTask().execute(new Void[]{});
		
		View root = inflater.inflate(R.layout.fragment_sync, container, false);

		mPrefs = getActivity().getSharedPreferences(Constant.PREFS_NAME,
				Context.MODE_PRIVATE);
		mEditor = mPrefs.edit();

		// SETTINGS
		ImageView iv = (ImageView) root.findViewById(R.id.ivSettings);
		iv.setOnClickListener(this);


		mRLSetting = (RelativeLayout) root.findViewById(R.id.rlSetting);

		mIvAddPc = (ImageView) root.findViewById(R.id.ivAddpc);
		mIvAddPc.setOnClickListener(this);
		
		tvConnectPC = (TextView) root.findViewById(R.id.tvConnectPC);

		return root;
	}

	@Override
	public void onActivityCreated(Bundle savedInstanceState) {
		super.onActivityCreated(savedInstanceState);
		refreshLayout();

		IntentFilter filter = new IntentFilter();
		filter.addAction(Constant.ACTION_BONJOUR_SERVER_MANUAL_PAIRING);
		filter.addAction(Constant.ACTION_BONJOUR_SERVER_AUTO_PAIRING);
		filter.addAction(Constant.ACTION_WEB_SOCKET_SERVER_CONNECTED);
		filter.addAction(Constant.ACTION_NETWORK_STATE_CHANGE);
		getActivity().registerReceiver(mReceiver, filter);

	
		// GET PAIRED SERVERS
//		firsttimeDispaly();

		boolean alarmEnable = mPrefs.getBoolean(
				Constant.PREF_BONJOUR_SERVER_ALRM_ENNABLED, false);
		if (alarmEnable == false) {
			BackupLogic.setAlarmWakeUpService(getActivity());
			mEditor.putBoolean(Constant.PREF_BONJOUR_SERVER_ALRM_ENNABLED, true)
					.commit();
		}
	}

	private void dismissProgress() {
		if ((mProgressDialog != null) && mProgressDialog.isShowing()) {
			mProgressDialog.dismiss();
		}
	}

	private void firsttimeDispaly() {
		//HAS PAIRED SERVER
		mPairedServers = ServersLogic.getBackupedServers(getActivity());
		if (mProgressDialog == null) {
			if (NetworkUtil.isWifiNetworkAvailable(getActivity())
					&& RuntimeState.OnWebSocketOpened == false) {
				if (mPairedServers.size() != 0) {
					RuntimeState.mAutoConnectMode = true;
					mProgressDialog = ProgressDialog.show(getActivity(), "",
							getString(R.string.auto_connect));
					mProgressDialog.setCancelable(true);
					mHandler.postDelayed(new Runnable() {
						public void run() {
							if (NetworkUtil
									.isWifiNetworkAvailable(getActivity())
									&& RuntimeState.OnWebSocketOpened == false) {
								dismissProgress();
								Toast.makeText(getActivity(),
										R.string.can_not_find_server,
										Toast.LENGTH_LONG).show();
							}
						}
					}, 10000);
				}
			}
		}
		//NO PAIRED SERVER
//		if (mPairedServers.size() == 0) {
//			Intent startIntent = new Intent(getActivity(),
//					FirstUseActivity.class);
//			startActivityForResult(startIntent,
//					Constant.REQUEST_CODE_OPEN_SERVER_CHOOSER);
//		}
	}

	private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			String action = intent.getAction();
			Log.d(TAG, "action:" + intent.getAction());
			if (Constant.ACTION_SCAN_FILE.equals(action)
					|| Constant.ACTION_WEB_SOCKET_SERVER_CONNECTED
							.equals(action)
					|| Constant.ACTION_FILE_DELETED.equals(action)) {
				refreshLayout();
			} else if (Constant.ACTION_BACKUP_START.equals(action)
					|| Constant.ACTION_BACKUP_DONE.equals(action)) {
				displayProgressingInfo();
			} else if (Constant.ACTION_NETWORK_STATE_CHANGE.equals(action)) {
				displayProgressingInfo();
			} 
			else if (Constant.ACTION_BONJOUR_SERVER_AUTO_PAIRING
					.equals(action)) {
				firsttimeDispaly();
				displayProgressingInfo();
			} else if (Constant.ACTION_UPLOADING_FILE.equals(action)) {
				displayProgressingInfo();
			}
		}
	};

	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
		if (requestCode == Constant.REQUEST_CODE_OPEN_SERVER_CHOOSER) {
			if (resultCode == Activity.RESULT_OK) {
				refreshLayout();
				// mAdapter.setData(ServersLogic.getBackupedServers(this));
			}
		} else if (requestCode == Constant.REQUEST_CODE_ADD_SERVER) {
			if (resultCode == Activity.RESULT_OK) {
				// mAdapter.setData(ServersLogic.getBackupedServers(this));
			}
		}
	}

	@Override
	public void onResume() {
		super.onResume();
		refreshLayout();
	}
    @Override
	public void onPause() {
        super.onPause();
        Log.v(TAG, "pause activity");
    }

	@Override
	public void onStart() {
		super.onStart();
		EasyTracker.getInstance().setContext(getActivity());
		EasyTracker.getTracker().sendView(TAG);
	};

	@Override
	public void onDestroy() {
		Log.d(TAG, "onDestroy");
		getActivity().unregisterReceiver(mReceiver);
		super.onDestroy();
	}

	public void refreshLayout() {
		mPairedServers = ServersLogic.getBackupedServers(getActivity());
		// REFRESH SETTING AREA
		if (ServersLogic.hasBackupedServers(getActivity())) {
			if(RuntimeState.OnWebSocketOpened){
			   tvConnectPC.setText(getActivity().getString(R.string.backup_linked,mPairedServers.get(0).serverName));
			}
			else{
				   tvConnectPC.setText(getActivity().getString(R.string.backup_wait_for_linked,mPairedServers.get(0).serverName));				
			}
			mIvAddPc.setVisibility(View.GONE);
		} else {
			tvConnectPC.setText(R.string.connect_a_pc);
			mIvAddPc.setVisibility(View.VISIBLE);
		}
	}

	public void displayProgressingInfo() {
	}

	@Override
	public void onClick(View v) {
		Intent startIntent = null;
		switch (v.getId()) {
		case R.id.ivAddpc:
			if (!ServersLogic.hasBackupedServers(getActivity())) {
				startIntent = new Intent(getActivity(), FirstUseActivity.class);
				startActivityForResult(startIntent,
						Constant.REQUEST_CODE_OPEN_SERVER_CHOOSER);
				EasyTracker.getTracker().sendEvent(Constant.CATEGORY_UI, Constant.ANALYTICS_ACTION_BTN_PRESS, Constant.ANALYTICS_LABEL_ADD_PC, null);
			}
			break;
		}
	}
	class InvokeServiceTask extends AsyncTask<Void,Void,Void>{
		@Override
		protected Void doInBackground(Void... params) {
			getActivity().startService(new Intent(getActivity(), InfiniteService.class));
			return null;
		}
	}
}
