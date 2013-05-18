package com.waveface.sync.ui.fragment;

import java.util.ArrayList;

import idv.jason.lib.imagemanager.ImageAttribute;
import idv.jason.lib.imagemanager.ImageManager;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.SyncApplication;
import com.waveface.sync.db.LabelDB;
import com.waveface.sync.db.LabelFileTable;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.event.DispatchKeyEvent;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.ui.adapter.PlayerPagerAdapter;

import de.greenrobot.event.EventBus;

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
import android.view.View.OnClickListener;
import android.view.View.OnKeyListener;
import android.view.ViewGroup;
import android.view.animation.AccelerateInterpolator;
import android.view.animation.AlphaAnimation;
import android.view.animation.Animation;
import android.view.animation.Animation.AnimationListener;
import android.view.animation.DecelerateInterpolator;
import android.widget.ImageView;
import android.widget.ViewAnimator;
import android.widget.ImageView.ScaleType;

public class PlaybackFragment extends Fragment implements OnPageChangeListener, OnClickListener {
	public static final String TAG = PlaybackFragment.class.getSimpleName(); 
	private ViewAnimator mViewAnimator;
	private ViewAnimator mPlayAnimator;
	private Cursor mCursor;
	
	private ViewPager mPager;
	
	private ImageManager mImageManager;
	
	private static final int AUTO_SLIDE_SHOW_FIRST_DELAY_MILLIS = 5000;
	private static final int AUTO_SLIDE_SHOW_DELAY_MILLIS = 3000;
	private static final int AUTO_SLIDE_SHOW_AFTER_CONTROL_DELAY_MILLIS = 15000;
	
	private Animation mFadeIn, mFadeOut;
	private String mServerUrl;
	private ImageView mPlaySlideShow;
	
	public boolean mPagerReady = true;
	
	private Handler mSlideShowHandler = new Handler();
	private Runnable mSlideShowRunnable = new Runnable() {
		@Override
		public void run() {
			startSlideShow();
		}
	};
	
	private Runnable mSlideNextRunnable = new Runnable() {
		@Override
		public void run() {
			autoSlideNext();
		}
	};
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		ArrayList<ServerEntity> servers = ServersLogic.getBackupedServers(getActivity());
		ServerEntity pairedServer = servers.get(0);
		mServerUrl ="http://"+pairedServer.ip+":"+pairedServer.restPort;
	};
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		mImageManager = SyncApplication.getWavefacePlayerApplication(getActivity()).getImageManager();
		String labelId = null;
		Cursor c = LabelDB.getAllLabels(getActivity());
		if(c.getCount() > 0) {
			c.moveToFirst();
			labelId = c.getString(0);
		}
		c.close();
		mCursor = LabelDB.getLabelFilesByLabelId(getActivity(), labelId);
		
		View root = inflater.inflate(R.layout.fragment_playback, container, false);
		
		mPager = (ViewPager) root.findViewById(R.id.pager);
		mPager.setAdapter(new PlayerPagerAdapter(getActivity(), labelId));
		mPager.setOnPageChangeListener(this);
		mPager.setOnKeyListener(new OnKeyListener() {
			@Override
			public boolean onKey(View v, int keyCode, KeyEvent event) {
				if(event.getAction() == KeyEvent.ACTION_UP)
					EventBus.getDefault().post(new DispatchKeyEvent(keyCode));
				return true;
			}
		});
		
		mFadeIn = new AlphaAnimation(0, 1);
		mFadeIn.setInterpolator(new DecelerateInterpolator()); //add this
		mFadeIn.setDuration(1000);

		mFadeOut = new AlphaAnimation(1, 0);
		mFadeOut.setInterpolator(new AccelerateInterpolator()); //and this
//		mFadeOut.setStartOffset(1000);
		mFadeOut.setDuration(1000);
		
		mViewAnimator = (ViewAnimator) root.findViewById(R.id.background_view);
		mViewAnimator.setDisplayedChild(0);
		mViewAnimator.setInAnimation(mFadeIn);
		mViewAnimator.setOutAnimation(mFadeOut);
		setAnimatorImage(0, 0);
		setAnimatorImage(1, 1);
		
		mPlaySlideShow = (ImageView) root.findViewById(R.id.image_play);
		mPlaySlideShow.setOnClickListener(this);
		
		mPlayAnimator = (ViewAnimator) root.findViewById(R.id.play_animator);
		
		mFadeIn.setAnimationListener(new AnimationListener() {
			
			@Override
			public void onAnimationStart(Animation animation) {
				Log.d(TAG, "=======onAnimationStart");
			}
			
			@Override
			public void onAnimationRepeat(Animation animation) {
				// TODO Auto-generated method stub
				
			}
			
			@Override
			public void onAnimationEnd(Animation animation) {
				mPager.setVisibility(View.VISIBLE);
			}
		});
		
		EventBus.getDefault().register(this);
		return root;
	}
	
	@Override
	public void onPause() {
		super.onPause();
		
		mSlideShowHandler.removeCallbacks(mSlideShowRunnable);
		mSlideShowHandler.removeCallbacks(mSlideNextRunnable);
	}
	
	@Override
	public void onResume() {
		super.onResume();
		delaySlideShow(AUTO_SLIDE_SHOW_FIRST_DELAY_MILLIS);
	}
	

	public void onEvent(DispatchKeyEvent e) {
		Log.d(TAG, "e.keycode:" + e.keycode);
		stopSlideShow();
		mSlideShowHandler.removeCallbacks(mSlideShowRunnable);
		delaySlideShow(AUTO_SLIDE_SHOW_AFTER_CONTROL_DELAY_MILLIS);

		boolean switchPlayMode = false;
		if(mPlayAnimator.getDisplayedChild() == 1) {
			// we were playing, just stop because key
			// show the play icon
			switchPlayMode = true;
			mPlayAnimator.showNext();
		}
		
		switch(e.keycode) {
		case KeyEvent.KEYCODE_DPAD_LEFT:
		case KeyEvent.KEYCODE_MEDIA_REWIND:
			Log.d(TAG, "onEvent left");
			if(mCurrentPosition > 0) {
				mPager.setCurrentItem(mCurrentPosition, true);
			}
			break;
		case KeyEvent.KEYCODE_DPAD_RIGHT:
		case KeyEvent.KEYCODE_MEDIA_FAST_FORWARD:
			Log.d(TAG, "onEvent right");
			if(mCurrentPosition < mCursor.getCount()-1) {
				mPager.setCurrentItem(mCurrentPosition, true);
			}
			break;
		case KeyEvent.KEYCODE_ENTER:
		case KeyEvent.KEYCODE_MEDIA_PLAY:
			if(mPlayAnimator.getDisplayedChild() == 0 && switchPlayMode == false) {
				mPlayAnimator.showNext();
				delaySlideShow(AUTO_SLIDE_SHOW_DELAY_MILLIS);
			}
			break;
		}
	}
	
	@Override
	public void onDestroy() {
		super.onDestroy();
		mCursor.close();
		EventBus.getDefault().unregister(this);
	}
	
	private void autoSlideNext() {
		if(mCurrentPosition+1 < mCursor.getCount()) {
			mCurrentPosition++;
			mViewAnimator.showNext();
			mPager.setVisibility(View.INVISIBLE);
			mPager.setCurrentItem(mCurrentPosition);
			setAnimatorImage(mCurrentPosition+1, getNextView());
			mSlideShowHandler.postDelayed(mSlideNextRunnable, AUTO_SLIDE_SHOW_DELAY_MILLIS);
		}
	}
	
	private void startSlideShow() {
		Log.d(TAG, "startSlideShow");
		if(mPlayAnimator.getDisplayedChild() == 0)
			mPlayAnimator.showNext();
		mSlideShowHandler.postDelayed(mSlideNextRunnable, 0);
	}
	
	private void stopSlideShow() {
		Log.d(TAG, "stopSlideShow");
		mSlideShowHandler.removeCallbacks(mSlideNextRunnable);

	}
	
	private void delaySlideShow(int delayMillis) {
		Log.d(TAG, "delaySlideShow:" + delayMillis);
		mSlideShowHandler.removeCallbacks(mSlideShowRunnable);
		mSlideShowHandler.postDelayed(mSlideShowRunnable, delayMillis);
	}

	private int mCurrentPosition = 0;
	
	private void setAnimatorImage(int imagePosition, int viewPosition) {
		if(mCursor.moveToPosition(imagePosition)) {
			ImageView iv = (ImageView) mViewAnimator.getChildAt(viewPosition);
			
			ImageAttribute attr = new ImageAttribute(iv);
			attr.setDoneScaleType(ScaleType.CENTER_CROP);
			attr.setLoadFromThread(true);
			attr.setMaxSizeEqualsScreenSize(getActivity());
			
			String url = mServerUrl + Constant.URL_IMAGE + "/" + 
					mCursor.getString(mCursor.getColumnIndex(LabelFileTable.COLUMN_FILE_ID)) + 
					Constant.URL_IMAGE_LARGE;
			mImageManager.getImage( url , attr);
		}
	}
	
	private int getPreviousView() {
		int next = mViewAnimator.getDisplayedChild()-1;
		if(next < 0)
			next = 2;
		return next;
	}
	

	private int getNextView() {
		int next = mViewAnimator.getDisplayedChild() +1;
		if(next == 3)
			next = 0;
		return next;
	}

	@Override
	public void onPageScrollStateChanged(int state) {
		Log.d(TAG, "onPageScrollStateChanged:" + state);
		if(state != 0) {
			if(state == 1) {
				mViewAnimator.setVisibility(View.INVISIBLE);
				if(mPlayAnimator.getDisplayedChild() == 1)
					mPlayAnimator.showNext();
			}
			mPagerReady = false;
			mSlideShowHandler.removeCallbacks(mSlideShowRunnable);
			stopSlideShow();
		} else {
			mViewAnimator.setVisibility(View.VISIBLE);
			mPagerReady = true;
			delaySlideShow(AUTO_SLIDE_SHOW_AFTER_CONTROL_DELAY_MILLIS);
		}
	}

	@Override
	public void onPageScrolled(int arg0, float arg1, int arg2) {
	}

	@Override
	public void onPageSelected(int page) {
		if(page > mCurrentPosition) {
			mViewAnimator.showNext();
			mCurrentPosition++;
			if(mCurrentPosition+1 < mCursor.getCount()-1)
			setAnimatorImage(mCurrentPosition+1, getNextView());
		} else if(page < mCurrentPosition){
			mViewAnimator.showPrevious();
			mCurrentPosition--;
			if(mCurrentPosition-1 >= 0)
				setAnimatorImage(mCurrentPosition-1, getPreviousView());
			
		}
	}

	@Override
	public void onClick(View view) {
		switch(view.getId()) {
		case R.id.image_play:
			mPlayAnimator.showNext();
			delaySlideShow(AUTO_SLIDE_SHOW_DELAY_MILLIS);
			break;
		}
	}
}
