package com.waveface.sync.ui.fragment;

import java.util.ArrayList;

import android.app.Activity;
import android.app.ProgressDialog;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.os.Handler;
import android.support.v4.app.Fragment;
import android.text.TextUtils;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.RelativeLayout;
import android.widget.TextView;
import android.widget.Toast;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.RuntimeState;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.image.MediaStoreImage;
import com.waveface.sync.logic.BackupLogic;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.ui.CleanActivity;
import com.waveface.sync.ui.FirstUseActivity;
import com.waveface.sync.ui.preference.Preferences;
import com.waveface.sync.util.DeviceUtil;
import com.waveface.sync.util.NetworkUtil;
import com.waveface.sync.util.StringUtil;

public class SyncFragment extends Fragment implements OnClickListener {

	private String TAG = SyncFragment.class.getSimpleName();

	private Handler mHandler = new Handler();

	// UI
	private TextView mDevice;
	private TextView mTotalInfo;
	private TextView mNowPeriod;
	private ImageView mPhotoImage;
	private TextView mPhotoCount;
	private TextView mPhotoSize;
	private ImageView mVideoImage;
	private TextView mVideoCount;
	private TextView mVideoSize;
	private ImageView mAudioImage;
	private TextView mAudioCount;
	private TextView mAudioSize;
	private Button mDeletePhotoBtn;
	private Button mDeleteVideoBtn;
	private Button mDeleteAudioBtn;
	private RelativeLayout mRLSetting;
	private ImageView mIvAddPc;
	private RelativeLayout rlBackupContent;
	private ImageView ivPC;
	private ImageView ivFile;
	private ImageView ivPlay;
	private TextView tvBackupPC;
	private TextView tvContent;
	private TextView tvDetail;
	private TextView tvLastBackupTime;

	private ProgressDialog mProgressDialog;
	private MediaStoreImage mMediaImage;

	private SharedPreferences mPrefs;
	private Editor mEditor;

	private final static int IMAGE_HEIGHT = 110;
	private final static int IMAGE_WIDTH = 110;
	// DATA
	private ArrayList<ServerEntity> mPairedServers;
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		Log.d(TAG, "onCreateView");
		View root = inflater.inflate(R.layout.fragment_sync, container, false);
		
		mPrefs = getActivity().getSharedPreferences(Constant.PREFS_NAME, Context.MODE_PRIVATE);
		mEditor = mPrefs.edit();

		mMediaImage = new MediaStoreImage(getActivity(), IMAGE_WIDTH, IMAGE_HEIGHT);

		mDevice = (TextView) root.findViewById(R.id.textDevice);
		mDevice.setText(DeviceUtil.getDeviceNameForDisplay(getActivity()));

		mTotalInfo = (TextView) root.findViewById(R.id.textTotolInfo);
		mNowPeriod = (TextView) root.findViewById(R.id.textPeriod);

		// SETTINGS
		ImageView iv = (ImageView) root.findViewById(R.id.ivSettings);
		iv.setOnClickListener(this);

		// PHOTO
		mPhotoImage = (ImageView) root.findViewById(R.id.imageView1);
		mPhotoImage.setOnClickListener(this);
		mPhotoCount = (TextView) root.findViewById(R.id.textPhotoCount);
		mPhotoSize = (TextView) root.findViewById(R.id.textPhotoSize);

		// VIDEO
		mVideoImage = (ImageView) root.findViewById(R.id.imageView2);
		mVideoImage.setOnClickListener(this);
		mVideoCount = (TextView) root.findViewById(R.id.textVideoCount);
		mVideoSize = (TextView) root.findViewById(R.id.textVideoSize);

		// AUDIO
		mAudioImage = (ImageView) root.findViewById(R.id.imageView3);
		mAudioImage.setOnClickListener(this);
		mAudioCount = (TextView) root.findViewById(R.id.textAudioCount);
		mAudioSize = (TextView) root.findViewById(R.id.textAudioSize);

		mDeletePhotoBtn = (Button) root.findViewById(R.id.btnDeletePhoto);
		mDeletePhotoBtn.setOnClickListener(this);
		mDeleteVideoBtn = (Button) root.findViewById(R.id.btnDeleteVideo);
		mDeleteVideoBtn.setOnClickListener(this);
		mDeleteAudioBtn = (Button) root.findViewById(R.id.btnDeleteAudio);
		mDeleteAudioBtn.setOnClickListener(this);

		// progress Content Area
		rlBackupContent = (RelativeLayout) root.findViewById(R.id.rlBackupContent);
		ivPC = (ImageView) root.findViewById(R.id.imagePC);
		ivFile = (ImageView) root.findViewById(R.id.ivFile);
		ivPlay = (ImageView) root.findViewById(R.id.ivPlay);
		tvBackupPC = (TextView) root.findViewById(R.id.tvBackupPC);
		tvContent = (TextView) root.findViewById(R.id.tvContent);
		tvDetail = (TextView) root.findViewById(R.id.tvDetail);
		tvLastBackupTime = (TextView) root.findViewById(R.id.tvLastBackupTime);

		// PAIRED SERVERS
		// ListView listview = (ListView) findViewById(R.id.listview);
		// ArrayList<ServerEntity> servers =
		// ServersLogic.getBackupedServers(this);
		// mAdapter = new PairedServersAdapter(this,servers);
		// listview.setAdapter(mAdapter);

		mRLSetting = (RelativeLayout) root.findViewById(R.id.rlSetting);

		mIvAddPc = (ImageView) root.findViewById(R.id.ivAddpc);
		mIvAddPc.setOnClickListener(this);
		return root;
	}
	
	@Override
	public void onActivityCreated(Bundle savedInstanceState) {
		super.onActivityCreated(savedInstanceState);
		refreshLayout();
		
		IntentFilter filter = new IntentFilter();
		filter.addAction(Constant.ACTION_BONJOUR_SERVER_MANUAL_PAIRING);
		filter.addAction(Constant.ACTION_BONJOUR_SERVER_AUTO_PAIRING);
		filter.addAction(Constant.ACTION_BACKUP_DONE);
		filter.addAction(Constant.ACTION_SCAN_FILE);
		filter.addAction(Constant.ACTION_FILE_DELETED);
		filter.addAction(Constant.ACTION_WS_SERVER_NOTIFY);
		filter.addAction(Constant.ACTION_WEB_SOCKET_SERVER_CONNECTED);
		filter.addAction(Constant.ACTION_NETWORK_STATE_CHANGE);
		filter.addAction(Constant.ACTION_UPLOADING_FILE);
		filter.addAction(Constant.ACTION_BACKUP_START);
		getActivity().registerReceiver(mReceiver, filter);

		// GET PAIRED SERVERS
		firsttimeDispaly();

		boolean alarmEnable = mPrefs.getBoolean(
				Constant.PREF_BONJOUR_SERVER_ALRM_ENNABLED, false);
		if (alarmEnable == false) {
			BackupLogic.setAlarmWakeUpService(getActivity());
			mEditor.putBoolean(Constant.PREF_BONJOUR_SERVER_ALRM_ENNABLED, true)
					.commit();
		}
//		new StartServiceTask().execute(new Void[] {});
		getActivity().sendBroadcast(new Intent(Constant.ACTION_INFINITE_STORAGE_ALARM));
	}

	private void dismissProgress() {
		if ((mProgressDialog != null) && mProgressDialog.isShowing()) {
			mProgressDialog.dismiss();
		}
	}

	private void firsttimeDispaly() {
		mPairedServers = ServersLogic.getBackupedServers(getActivity());
		if (mProgressDialog == null) {
			if (NetworkUtil.isWifiNetworkAvailable(getActivity())
					&& RuntimeState.OnWebSocketStation == false) {
				if (mPairedServers.size() != 0) {
					RuntimeState.mAutoConnectMode = true;
					mProgressDialog = ProgressDialog.show(getActivity(), "",
							getString(R.string.auto_connect));
					mProgressDialog.setCancelable(true);
					mHandler.postDelayed(new Runnable() {
						public void run() {
							if (NetworkUtil
									.isWifiNetworkAvailable(getActivity())
									&& RuntimeState.OnWebSocketStation == false) {
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
		if (mPairedServers.size() == 0) {
			Intent startIntent = new Intent(getActivity(),
					FirstUseActivity.class);
			startActivityForResult(startIntent,
					Constant.REQUEST_CODE_OPEN_SERVER_CHOOSER);
		}
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
			} else if (Constant.ACTION_BACKUP_START.equals(action) || Constant.ACTION_BACKUP_DONE.equals(action)) {
				displayProgressingInfo();
			} else if (Constant.ACTION_NETWORK_STATE_CHANGE.equals(action)) {
				displayProgressingInfo();
			} else if (Constant.ACTION_WS_SERVER_NOTIFY.equals(action)) {
				dismissProgress();
				String actionContent = intent
						.getStringExtra(Constant.EXTRA_WEB_SOCKET_EVENT_CONTENT);
				if (actionContent != null) {
					if (actionContent.equals(Constant.WS_ACTION_ACCEPT)) {
						if (RuntimeState.mAutoConnectMode) {
							dismissProgress();
						}
						displayProgressingInfo();
					} else if (actionContent
							.equals(Constant.WS_ACTION_SOCKET_CLOSED)) {
						displayProgressingInfo();
					} else if (actionContent.equals(Constant.WS_ACTION_DENIED)) {
						refreshLayout();
					}
				}
			} else if (Constant.ACTION_BONJOUR_SERVER_AUTO_PAIRING
					.equals(action)) {
				firsttimeDispaly();
				displayProgressingInfo();
			} else if (Constant.ACTION_UPLOADING_FILE.equals(action)) {
				displayProgressingInfo();
				int state = intent.getIntExtra(
						Constant.EXTRA_BACKING_UP_FILE_STATE, -1);
				refreshFileContent(state);
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
	public void onDestroy() {
		Log.d(TAG, "onDestroy");
		getActivity().unregisterReceiver(mReceiver);
		super.onDestroy();
	}

	public void refreshLayout() {
		String[] periods = BackupLogic.getFilePeriods(getActivity());
		if (TextUtils.isEmpty(periods[0])) {
			mNowPeriod.setText(R.string.file_scanning);
		} else {
			mNowPeriod.setText(getString(R.string.period, periods[0],
					periods[1]));
		}
		long totalCount = 0;
		long totalSize = 0;

		long[] datas = BackupLogic.getFileInfo(getActivity(), Constant.TYPE_IMAGE);
		mPhotoCount.setText(getString(R.string.photos, datas[0]));
		mPhotoSize.setText(StringUtil.byteCountToDisplaySize(datas[1]));
		totalCount += datas[0];
		totalSize += datas[1];

		datas = BackupLogic.getFileInfo(getActivity(), Constant.TYPE_VIDEO);
		mVideoCount.setText(getString(R.string.videos, datas[0]));
		mVideoSize.setText(StringUtil.byteCountToDisplaySize(datas[1]));
		totalCount += datas[0];
		totalSize += datas[1];

		datas = BackupLogic.getFileInfo(getActivity(), Constant.TYPE_AUDIO);
		mAudioCount.setText(getString(R.string.audios, datas[0]));
		mAudioSize.setText(StringUtil.byteCountToDisplaySize(datas[1]));
		totalCount += datas[0];
		totalSize += datas[1];

		mTotalInfo.setText(getString(R.string.total_info, totalCount,
				StringUtil.byteCountToDisplaySize(totalSize)));

		// REFRESH PROGRESS INFO
		displayProgressingInfo();

		// REFRESH SERVERS STATUS
		// refreshServerStatus();

		// REFRESH SETTING AREA
		if (ServersLogic.hasBackupedServers(getActivity())) {
			mRLSetting.setVisibility(View.GONE);
		} else {
			mRLSetting.setVisibility(View.VISIBLE);
		}
	}

	public void displayProgressingInfo() {
		if (ServersLogic.hasBackupedServers(getActivity())) {
			ArrayList<ServerEntity> servers = ServersLogic
					.getBackupedServers(getActivity());
			ServerEntity entity = servers.get(0);
			String displayTime = StringUtil.displayLocalTime(
					entity.lastLocalBackupTime, StringUtil.DATE_FORMAT);
			rlBackupContent.setVisibility(View.VISIBLE);
			int[] counts = BackupLogic.getBackupProgressInfo(getActivity(),
					entity.serverId);
			if (RuntimeState.OnWebSocketStation) {
				tvBackupPC.setText(entity.serverName);
				if (RuntimeState.isBackuping) {
					ivPC.setImageResource(R.drawable.ic_processing);
					tvContent.setText(getString(R.string.backup_progress,
							counts[0], counts[1]));
				} else {
					ivPC.setImageResource(R.drawable.ic_transfer);
					if (counts[0] == counts[1] && counts[1] != 0) {
						tvContent.setText(getString(R.string.backup_done));
					} else {
						tvContent
								.setText(getString(R.string.backup_info_empty));
					}
				}
			} else {
				tvBackupPC.setText(entity.serverName);
				ivPC.setImageResource(R.drawable.ic_offline);

				if (counts[0] == counts[1] && counts[1] != 0) {
					tvContent.setText(getString(R.string.backup_done));
				} else {
					tvContent.setText(getString(R.string.backup_uncompleted));
				}
			}
			if (!TextUtils.isEmpty(displayTime)) {
				tvLastBackupTime.setText(getString(
						R.string.backup_last_local_time, displayTime));
			}
			tvDetail.setText(StringUtil.getFilename(RuntimeState.mFilename));
		} else {
			rlBackupContent.setVisibility(View.INVISIBLE);
		}
		displayBackingUpImage();
	}

	public void refreshFileContent(int state) {
		switch (state) {
		case Constant.JOB_START:
			displayProgressingInfo();
			break;
		case Constant.FILE_START:
			int[] counts = BackupLogic.getBackupProgressInfo(getActivity(),
					RuntimeState.mWebSocketServerId);
			// int progress = (int) (counts[0]/ (float) counts[1] * 100);
			// tvContent.setText(getString(R.string.backup_progress,counts[0],counts[1],progress));
			tvContent.setText(getString(R.string.backup_progress, counts[0],
					counts[1]));
			tvDetail.setText(StringUtil.getFilename(RuntimeState.mFilename));
			displayBackingUpImage();
			break;
		}
	}

	// public void refreshServerStatus(){
	// mAdapter.setData(ServersLogic.getBackupedServers(this));
	// if(ServersLogic.hasBackupedServers(this)){
	// mRLSetting.setVisibility(View.GONE);
	// }
	// }

	private void displayBackingUpImage() {
		if (!TextUtils.isEmpty(RuntimeState.mFilename)) {
			switch (RuntimeState.mFileType) {
			case Constant.TYPE_AUDIO:
			case Constant.TYPE_VIDEO:
				ivPlay.setVisibility(View.VISIBLE);
				break;
			case Constant.TYPE_IMAGE:
				ivPlay.setVisibility(View.INVISIBLE);
				break;
			}
			if (RuntimeState.mFileType != Constant.TYPE_AUDIO) {
				
				try {
					Bitmap b = mMediaImage.getBitmap(RuntimeState.mMediaID,
							RuntimeState.mFileType);
					if (b != null) {
						ivFile.setImageBitmap(b);
					} else {
						ivFile.setImageResource(R.drawable.ic_photos);
					}
				} catch (Exception e) {
					e.printStackTrace();
					ivFile.setImageResource(R.drawable.ic_photos);
				}
			}
		} else {
			ivFile.setImageResource(R.drawable.ic_photos);
		}
	}

	@Override
	public void onClick(View v) {
		Intent startIntent = null;
		switch (v.getId()) {
		case R.id.ivSettings:
			Intent intent = new Intent(getActivity(), Preferences.class);
			startActivity(intent);
			break;
		case R.id.ivAddpc:
			if (!ServersLogic.hasBackupedServers(getActivity())) {
				startIntent = new Intent(getActivity(),
						FirstUseActivity.class);
				startActivityForResult(startIntent,
						Constant.REQUEST_CODE_OPEN_SERVER_CHOOSER);
			}
			// else{
			// startIntent = new Intent(MainActivity.this,
			// AddServerActivity.class);
			// startActivityForResult(startIntent,
			// Constant.REQUEST_CODE_ADD_SERVER);
			// }
			break;
		case R.id.btnDeletePhoto:
			startIntent = new Intent(getActivity(), CleanActivity.class);
			startIntent.putExtra(Constant.BUNDLE_FILE_TYPE,
					Constant.TRANSFER_TYPE_IMAGE);
			startActivityForResult(startIntent,
					Constant.REQUEST_CODE_CLEAN_STORAGE);
			break;
		case R.id.btnDeleteVideo:
			startIntent = new Intent(getActivity(), CleanActivity.class);
			startIntent.putExtra(Constant.BUNDLE_FILE_TYPE,
					Constant.TRANSFER_TYPE_VIDEO);
			startActivityForResult(startIntent,
					Constant.REQUEST_CODE_CLEAN_STORAGE);
			break;
		case R.id.btnDeleteAudio:
			startIntent = new Intent(getActivity(), CleanActivity.class);
			startIntent.putExtra(Constant.BUNDLE_FILE_TYPE,
					Constant.TRANSFER_TYPE_AUDIO);
			startActivityForResult(startIntent,
					Constant.REQUEST_CODE_CLEAN_STORAGE);
			break;
		// case R.id.imageView1:
		// //TO DISPLAY ALL IMAGES
		// startIntent = new Intent(MainActivity.this, ImageViewActivity.class);
		// startIntent.putExtra(Constant.BUNDLE_FILE_TYPE,
		// Constant.TRANSFER_TYPE_IMAGE);
		// startActivity(startIntent);
		// break;
		// case R.id.imageView2:
		// //TO DISPLAY ALL VIDEOS
		// startIntent = new Intent(MainActivity.this, ImageViewActivity.class);
		// startIntent.putExtra(Constant.BUNDLE_FILE_TYPE,
		// Constant.TRANSFER_TYPE_VIDEO);
		// startActivity(startIntent);
		// break;
		// case R.id.imageView3:
		// //TO DISPLAY ALL VIDEOS
		// startIntent = new Intent(MainActivity.this, ImageViewActivity.class);
		// startIntent.putExtra(Constant.BUNDLE_FILE_TYPE,
		// Constant.TRANSFER_TYPE_AUDIO);
		// startActivity(startIntent);
		// break;
		}
	}
}
