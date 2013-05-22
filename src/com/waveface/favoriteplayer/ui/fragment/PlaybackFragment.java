package com.waveface.favoriteplayer.ui.fragment;

import java.util.ArrayList;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.db.LabelFileTable;
import com.waveface.favoriteplayer.entity.OverviewData;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.event.DispatchKeyEvent;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.ui.FullScreenSlideShowActivity;
import com.waveface.favoriteplayer.ui.adapter.PlayerPagerAdapter;

import de.greenrobot.event.EventBus;

import android.content.Intent;
import android.database.Cursor;
import android.os.Bundle;
import android.os.Handler;
import android.support.v4.app.Fragment;
import android.support.v4.view.ViewPager;
import android.support.v4.view.ViewPager.OnPageChangeListener;
import android.util.Log;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnKeyListener;
import android.view.ViewGroup;

public class PlaybackFragment extends Fragment implements OnPageChangeListener {
	public static final String TAG = PlaybackFragment.class.getSimpleName(); 
	private ViewPager mPager;
	
	private ArrayList<OverviewData> mDatas;
	
	private static final int AUTO_SLIDE_SHOW_FIRST_DELAY_MILLIS = 5000;
	private static final int AUTO_SLIDE_SHOW_AFTER_CONTROL_DELAY_MILLIS = 15000;
	
	private Handler mSlideShowHandler = new Handler();
	private Runnable mSlideShowRunnable = new Runnable() {
		@Override
		public void run() {
			startSlideShow();
		}
	};
		
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		String labelId = null;
		

		ArrayList<ServerEntity> servers = ServersLogic.getBackupedServers(getActivity());
		ServerEntity pairedServer = servers.get(0);
		String serverUrl ="http://"+pairedServer.ip+":"+pairedServer.restPort;
		
		Cursor c = LabelDB.getAllLabels(getActivity());
		if(c.getCount() > 0) {
			c.moveToFirst();
			labelId = c.getString(0);
		}
		c.close();
		c = LabelDB.getLabelFilesByLabelId(getActivity(), labelId);
		
		mDatas = new ArrayList<OverviewData>(c.getCount());
		for(int i=0; i<c.getCount(); ++i) {
			c.moveToPosition(i);
			OverviewData data = new OverviewData();
			data.url = serverUrl + Constant.URL_IMAGE + "/" + 
					c.getString(c.getColumnIndex(LabelFileTable.COLUMN_FILE_ID)) + 
					Constant.URL_IMAGE_MEDIUM;
			mDatas.add(i, data);
		}
		c.close();
		
		View root = inflater.inflate(R.layout.fragment_playback, container, false);
		
		mPager = (ViewPager) root.findViewById(R.id.pager);
		mPager.setAdapter(new PlayerPagerAdapter(getActivity(), mDatas));
		mPager.setOnKeyListener(new OnKeyListener() {
			@Override
			public boolean onKey(View v, int keyCode, KeyEvent event) {
				if(event.getAction() == KeyEvent.ACTION_UP)
					EventBus.getDefault().post(new DispatchKeyEvent(keyCode));
				return true;
			}
		});
		mPager.setOnPageChangeListener(this);
		
		return root;
	}
	
	@Override
	public void onPause() {
		super.onPause();
		EventBus.getDefault().unregister(this);
		mSlideShowHandler.removeCallbacks(mSlideShowRunnable);
	}
	
	@Override
	public void onResume() {
		super.onResume();
		EventBus.getDefault().register(this);
		delaySlideShow(AUTO_SLIDE_SHOW_FIRST_DELAY_MILLIS);
	}
	

	public void onEvent(DispatchKeyEvent e) {
		Log.d(TAG, "e.keycode:" + e.keycode);
		mSlideShowHandler.removeCallbacks(mSlideShowRunnable);
		delaySlideShow(AUTO_SLIDE_SHOW_AFTER_CONTROL_DELAY_MILLIS);
		
		int currentPosition = mPager.getCurrentItem();

		switch(e.keycode) {
		case KeyEvent.KEYCODE_DPAD_LEFT:
		case KeyEvent.KEYCODE_MEDIA_REWIND:
			Log.d(TAG, "onEvent left");
			if(currentPosition > 0) {
				mPager.setCurrentItem(currentPosition-1, true);
			}
			break;
		case KeyEvent.KEYCODE_DPAD_RIGHT:
		case KeyEvent.KEYCODE_MEDIA_FAST_FORWARD:
			Log.d(TAG, "onEvent right");
			if(currentPosition < mDatas.size()-1) {
				mPager.setCurrentItem(currentPosition+1, true);
			}
			break;
		case KeyEvent.KEYCODE_ENTER:
		case KeyEvent.KEYCODE_MEDIA_PLAY:
			startSlideShow();
			break;
		case KeyEvent.KEYCODE_BACK:
//			getActivity().finish();
			break;
		}
	}
	
	private void startSlideShow() {
		Log.d(TAG, "startSlideShow");
		Intent intent = new Intent(getActivity(), FullScreenSlideShowActivity.class);
		Bundle data = new Bundle();
		data.putParcelableArrayList(Constant.ARGUMENT1, mDatas);
		data.putInt(Constant.ARGUMENT2, mPager.getCurrentItem());
		intent.putExtras(data);
		startActivityForResult(intent, 0);
	}
	
	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
		int position = data.getIntExtra(Constant.ARGUMENT1, -1);
		if(position != -1) {
			mPager.setCurrentItem(position);
		}
	}
		
	private void delaySlideShow(int delayMillis) {
		Log.d(TAG, "delaySlideShow:" + delayMillis);
		mSlideShowHandler.removeCallbacks(mSlideShowRunnable);
		mSlideShowHandler.postDelayed(mSlideShowRunnable, delayMillis);
	}

	@Override
	public void onPageScrollStateChanged(int arg0) {
		mSlideShowHandler.removeCallbacks(mSlideShowRunnable);
	}

	@Override
	public void onPageScrolled(int arg0, float arg1, int arg2) {
	}

	@Override
	public void onPageSelected(int arg0) {
		delaySlideShow(AUTO_SLIDE_SHOW_FIRST_DELAY_MILLIS);		
	}
}
