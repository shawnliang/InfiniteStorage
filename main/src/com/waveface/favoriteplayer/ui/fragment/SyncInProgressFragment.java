package com.waveface.favoriteplayer.ui.fragment;

import android.os.Bundle;
import android.os.Handler;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.event.LabelImportedEvent;

import de.greenrobot.event.EventBus;

public class SyncInProgressFragment extends Fragment {
	public static final String TAG = SyncInProgressFragment.class.getSimpleName();
	private TextView mSyncContent;
	LabelImportedEvent mEvent;
	
	private int mTotalFile;
	private int mOffset;

	private Handler mHandler = new Handler();
	private Runnable mUpdateStatusRunnable = new Runnable() {

		@Override
		public void run() {
			int current = mEvent.currentIndex + mOffset;
			Log.d(TAG, current + "/"  + mTotalFile );
			float total = (float)(((float)mTotalFile - current)/20.0);
			if(current > mTotalFile) {
				Log.d(TAG, "skip");
				return;
			} else if(current == mTotalFile || total < 0.5) {
				total = 0;
			}
			if (total > 1) {
				if(mSyncContent != null && getActivity() != null)
					mSyncContent.setText(String.format(getActivity().getResources()
						.getString(R.string.sync_content_one_more), Integer.toString(Math.round(total)))	);
			} else if(total < 0.5) {
				if(mSyncContent != null && getActivity() != null)
					mSyncContent.setText(getActivity().getResources().getString(
						R.string.sync_almost_ready));
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
		} else if(event.status == LabelImportedEvent.STATUS_SETTING) {
			if(event.totalFile != -1) {
				mTotalFile = event.totalFile;
			}
			if(event.offset != -1) {
				mOffset = event.offset;
			}
		}
	}

}
