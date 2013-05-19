package com.waveface.favoriteplayer.ui.fragment;

import java.util.ArrayList;

import idv.jason.lib.imagemanager.ImageAttribute;
import idv.jason.lib.imagemanager.ImageManager;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.db.LabelFileTable;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.event.DispatchKeyEvent;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.ui.adapter.PlayerPagerAdapter;

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
import android.view.animation.AlphaAnimation;
import android.view.animation.Animation;
import android.view.animation.Animation.AnimationListener;
import android.widget.ImageView;
import android.widget.Toast;
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
		mFadeIn.setDuration(1000);

		mFadeOut = new AlphaAnimation(1, 0);
		mFadeOut.setDuration(1000);
		
		mViewAnimator = (ViewAnimator) root.findViewById(R.id.background_view);
		mViewAnimator.setDisplayedChild(0);
		mViewAnimator.setInAnimation(mFadeIn);
		mViewAnimator.setOutAnimation(mFadeOut);
		resetToFirst();
		
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
				mPager.setCurrentItem(mCurrentPosition-1, true);
			}
			break;
		case KeyEvent.KEYCODE_DPAD_RIGHT:
		case KeyEvent.KEYCODE_MEDIA_FAST_FORWARD:
			Log.d(TAG, "onEvent right");
			if(mCurrentPosition < mCursor.getCount()-1) {
				mPager.setCurrentItem(mCurrentPosition+1, true);
			}
			break;
		case KeyEvent.KEYCODE_ENTER:
		case KeyEvent.KEYCODE_MEDIA_PLAY:
			if(mPlayAnimator.getDisplayedChild() == 0 && switchPlayMode == false) {
				if(mCurrentPosition+1 >= mCursor.getCount()) {
					resetToFirst();
				}
				mPlayAnimator.showNext();
				delaySlideShow(AUTO_SLIDE_SHOW_DELAY_MILLIS);
			}
			break;
		case KeyEvent.KEYCODE_BACK:
			if(switchPlayMode == false)
				getActivity().finish();
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
		}  else {
			// hit end
			Toast.makeText(getActivity(), R.string.everything_played, Toast.LENGTH_SHORT).show();
			if(mPlayAnimator.getDisplayedChild() == 1)
				mPlayAnimator.showNext();
		}
	}
	
	private void startSlideShow() {
		Log.d(TAG, "startSlideShow");
		if(mPlayAnimator.getDisplayedChild() == 0)
			mPlayAnimator.showNext();
		mSlideShowHandler.postDelayed(mSlideNextRunnable, AUTO_SLIDE_SHOW_DELAY_MILLIS);
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
			attr.setDoneScaleType(ScaleType.FIT_CENTER);
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

	// user scroll 0->1->2->0
	// user touch  0->1->0
	// auto play   0->2->0
	int mPagerLastState;
	int mPagerState = PAGER_STATE_NONE;
	private static final int PAGER_STATE_NONE 			= 0;
	private static final int PAGER_STATE_USER_SCROLL 	= 1;
	private static final int PAGER_STATE_USER_TOUCH 	= 2;
	private static final int PAGER_STATE_AUTO_PLAY	 	= 3;
	@Override
	public void onPageScrollStateChanged(int state) {
		Log.d(TAG, "onPageScrollStateChanged:" + state);
		
		if(state == 0 && mPagerState != PAGER_STATE_NONE) {
			switch(mPagerState) {
			case PAGER_STATE_USER_SCROLL:
			case PAGER_STATE_USER_TOUCH:
				delaySlideShow(AUTO_SLIDE_SHOW_AFTER_CONTROL_DELAY_MILLIS);
				mViewAnimator.setVisibility(View.INVISIBLE);
				if(mPlayAnimator.getDisplayedChild() == 1)
					mPlayAnimator.showNext();
				break;
			case PAGER_STATE_AUTO_PLAY:
				break;
			}
			
			mViewAnimator.setVisibility(View.VISIBLE);
			

			mPagerState = PAGER_STATE_NONE;
		} else if(state == 1) {
			// since this state will only happen when user intercept, pause play
			mSlideShowHandler.removeCallbacks(mSlideShowRunnable);
			stopSlideShow();
			
			mPagerState = PAGER_STATE_USER_TOUCH;
		} else if(state == 2) {
			if(mPagerLastState != 0) {
				mPagerState = PAGER_STATE_USER_SCROLL;
			} else {
				mPagerState = PAGER_STATE_AUTO_PLAY;
			}
		}
		
		mPagerLastState = state;
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
	
	private void resetToFirst() {
		mPager.setCurrentItem(0);
		setAnimatorImage(0, 0);
		setAnimatorImage(1, 1);
		mCurrentPosition = 0;
	}

	@Override
	public void onClick(View view) {
		switch(view.getId()) {
		case R.id.image_play:
			if(mCurrentPosition+1 >= mCursor.getCount()) {
				resetToFirst();
			}
			mPlayAnimator.showNext();
			delaySlideShow(0);
			break;
		}
	}
}
