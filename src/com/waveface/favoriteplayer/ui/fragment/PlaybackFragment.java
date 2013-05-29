package com.waveface.favoriteplayer.ui.fragment;

import java.util.ArrayList;

import android.os.Bundle;
import android.os.Handler;
import android.support.v4.app.Fragment;
import android.support.v4.view.ViewPager;
import android.support.v4.view.ViewPager.OnPageChangeListener;
import android.util.Log;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.entity.PlaybackData;
import com.waveface.favoriteplayer.event.PhotoItemClickEvent;
import com.waveface.favoriteplayer.ui.adapter.PlayerPagerAdapter;

import de.greenrobot.event.EventBus;

public class PlaybackFragment extends Fragment implements OnPageChangeListener {
	public static final String TAG = PlaybackFragment.class.getSimpleName(); 
	private ViewPager mPager;
	private boolean mResume = false;
	
	private ArrayList<PlaybackData> mDatas = new ArrayList<PlaybackData>();
	
	private static final int AUTO_SLIDE_SHOW_FIRST_DELAY_MILLIS = 5000;
	private static final int AUTO_SLIDE_SHOW_AFTER_CONTROL_DELAY_MILLIS = 15000;
	
	private Handler mSlideShowHandler = new Handler();
	private Runnable mSlideShowRunnable = new Runnable() {
		@Override
		public void run() {
			Log.d(TAG, "start slide show");
			startSlideShow();
		}
	};
		
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		Bundle data = null;
		if(savedInstanceState == null) {
			data = getArguments();
		} else {
			data = savedInstanceState;
		}

		mDatas = data.getParcelableArrayList(Constant.ARGUMENT1);
		int position = data.getInt(Constant.ARGUMENT2);
		
		View root = inflater.inflate(R.layout.fragment_playback, container, false);
		
		mPager = (ViewPager) root.findViewById(R.id.pager);
		mPager.setAdapter(new PlayerPagerAdapter(getActivity(), mDatas));
		mPager.setOnPageChangeListener(this);
		
		mPager.setCurrentItem(position);
		
		return root;
	}
	
	@Override
	public void onSaveInstanceState(Bundle outState) {
		super.onSaveInstanceState(outState);
		outState.putParcelableArrayList(Constant.ARGUMENT1, mDatas);
		outState.putInt(Constant.ARGUMENT2, mPager.getCurrentItem());
	}
	
	
	@Override
	public void onPause() {
		super.onPause();
		mResume = false;
		Log.d(TAG, "pause");
		mSlideShowHandler.removeCallbacks(mSlideShowRunnable);
	}
	
	@Override
	public void onResume() {
		super.onResume();
		mResume = true;
		Log.d(TAG, "resume");
		delaySlideShow(AUTO_SLIDE_SHOW_FIRST_DELAY_MILLIS);
	}
	

	public void onKeyEvent(int keyCode, KeyEvent event) {
		Log.d(TAG, "keycode:" + keyCode);
		Log.d(TAG, "stop slide show");
		mSlideShowHandler.removeCallbacks(mSlideShowRunnable);
		
		int currentPosition = mPager.getCurrentItem();

		switch(keyCode) {
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
	
	public void pause(int position) {
		if(position != -1) {
			mPager.setCurrentItem(position, false);
		}
	}
	
	private void startSlideShow() {
		Log.d(TAG, "startSlideShow");

		if(mResume == false) {
			Log.d(TAG, "ignore slide show because on pause");
		} else {
			PhotoItemClickEvent event = new PhotoItemClickEvent();
			event.datas = mDatas;
			event.position = mPager.getCurrentItem();
			
			EventBus.getDefault().post(event);
		}
	}
	
	private void delaySlideShow(int delayMillis) {
		Log.d(TAG, "delaySlideShow:" + delayMillis);
		mSlideShowHandler.removeCallbacks(mSlideShowRunnable);
		mSlideShowHandler.postDelayed(mSlideShowRunnable, delayMillis);
	}

	@Override
	public void onPageScrollStateChanged(int state) {
		delaySlideShow(AUTO_SLIDE_SHOW_AFTER_CONTROL_DELAY_MILLIS);
	}

	@Override
	public void onPageScrolled(int arg0, float arg1, int arg2) {
	}

	@Override
	public void onPageSelected(int arg0) {
	}

}
