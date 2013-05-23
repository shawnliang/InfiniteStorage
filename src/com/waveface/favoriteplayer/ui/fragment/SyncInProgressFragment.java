package com.waveface.favoriteplayer.ui.fragment;

import android.os.Bundle;
import android.os.Handler;
import android.support.v4.app.Fragment;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.event.DispatchKeyEvent;
import com.waveface.favoriteplayer.event.LabelImportedEvent;

import de.greenrobot.event.EventBus;

public class SyncInProgressFragment extends Fragment {
	private TextView mSyncContent;
	LabelImportedEvent mEvent;

	private Handler mHandler = new Handler();
	private Runnable mUpdateStatusRunnable = new Runnable() {

		@Override
		public void run() {
			long total = mEvent.singleTime
					* (mEvent.totalFile - mEvent.currentFile) / (60 * 1000);
			if (total > 0) {
				if(mSyncContent != null && getActivity() != null)
					mSyncContent.setText(String.format(getActivity().getResources()
						.getString(R.string.sync_content_one_more), Long
						.toString(total)));
			} else {
				if(mSyncContent != null && getActivity() != null)
					mSyncContent.setText(getActivity().getResources().getString(
						R.string.sync_content_one));
			}
		}
	};

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		EventBus.getDefault().register(this);
	}

	@Override
	public void onDestroy() {
		super.onDestroy();
		EventBus.getDefault().unregister(this);
	}

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		View root = inflater.inflate(R.layout.fragment_sync_in_progress,
				container, false);
		mSyncContent = (TextView) root.findViewById(R.id.sync_content);

		return root;
	}

	public void onEvent(LabelImportedEvent event) {
		if (event.status == LabelImportedEvent.STATUS_SYNCING) {
			mEvent = event;
			mHandler.post(mUpdateStatusRunnable);
		}
	}

	public void onEvent(DispatchKeyEvent e) {

		switch (e.keycode) {
		case KeyEvent.KEYCODE_BACK:
			getActivity().finish();
			break;
		}
	}

}
