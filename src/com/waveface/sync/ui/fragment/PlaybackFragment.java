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
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.animation.AccelerateInterpolator;
import android.view.animation.AlphaAnimation;
import android.view.animation.Animation;
import android.view.animation.Animation.AnimationListener;
import android.view.animation.DecelerateInterpolator;
import android.widget.ImageView;
import android.widget.ViewAnimator;
import android.widget.ImageView.ScaleType;

public class PlaybackFragment extends Fragment implements OnPageChangeListener {
	public static final String TAG = PlaybackFragment.class.getSimpleName(); 
	private ViewAnimator mViewAnimator;
	private Cursor mCursor;
	
	private ViewPager mPager;
	
	private ImageManager mImageManager;
	
	private static final int AUTO_SLIDE_SHOW_DELAY_MILLIS = 3000;
	private static final int AUTO_SLIDE_SHOW_AFTER_CONTROL_DELAY_MILLIS = 10000;
	
	private Animation mFadeIn, mFadeOut;
	private String mServerUrl;
	
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
		delaySlideShow(AUTO_SLIDE_SHOW_DELAY_MILLIS);
	}
	

	public void onEvent(DispatchKeyEvent e) {
		stopSlideShow();
		mSlideShowHandler.removeCallbacks(mSlideShowRunnable);
		delaySlideShow(AUTO_SLIDE_SHOW_AFTER_CONTROL_DELAY_MILLIS);
//		mPager.setVisibility(View.INVISIBLE);
		switch(e.keycode) {
		case android.view.KeyEvent.KEYCODE_DPAD_LEFT:
			Log.d(TAG, "onEvent left");
			if(mCurrentPosition > 0) {
				mCurrentPosition--;
				mPager.setCurrentItem(mCurrentPosition, true);
			}
			break;
		case android.view.KeyEvent.KEYCODE_DPAD_RIGHT:
			Log.d(TAG, "onEvent right");
			if(mCurrentPosition < mCursor.getCount()-1) {
				mCurrentPosition++;
				mPager.setCurrentItem(mCurrentPosition, true);
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
		mCurrentPosition++;
		mViewAnimator.showNext();
		mPager.setVisibility(View.INVISIBLE);
		mPager.setCurrentItem(mCurrentPosition);
		setAnimatorImage(mCurrentPosition+1, getNextView());
		mSlideShowHandler.postDelayed(mSlideNextRunnable, AUTO_SLIDE_SHOW_DELAY_MILLIS);
	}
	
	private void startSlideShow() {
		Log.d(TAG, "startSlideShow");
		mSlideShowHandler.postDelayed(mSlideNextRunnable, 0);
	}
	
	private void stopSlideShow() {
		Log.d(TAG, "stopSlideShow");
		mSlideShowHandler.removeCallbacks(mSlideNextRunnable);

	}
	
	private void delaySlideShow(int delayMillis) {
		Log.d(TAG, "delaySlideShow");
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
			attr.setMaxSize(1920, 1080);
			mImageManager.getImage(
					mServerUrl + Constant.URL_IMAGE + "/" + 
					mCursor.getString(mCursor.getColumnIndex(LabelFileTable.COLUMN_FILE_ID)) + 
					Constant.URL_IMAGE_LARGE, attr);
			mImageManager.getImage(mCursor.getString(0), attr);
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
		if(state != 0) {
			if(state == 1)
				mViewAnimator.setVisibility(View.INVISIBLE);
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
}
