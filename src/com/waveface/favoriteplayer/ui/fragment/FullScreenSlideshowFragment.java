package com.waveface.favoriteplayer.ui.fragment;

import idv.jason.lib.imagemanager.ImageAttribute;
import idv.jason.lib.imagemanager.ImageManager;

import java.util.ArrayList;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.view.animation.AlphaAnimation;
import android.view.animation.Animation;
import android.view.animation.Animation.AnimationListener;
import android.view.animation.AnimationUtils;
import android.widget.ImageView;
import android.widget.ImageView.ScaleType;
import android.widget.Toast;
import android.widget.ViewAnimator;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.entity.PlaybackData;
import com.waveface.favoriteplayer.event.PlaybackCancelEvent;

import de.greenrobot.event.EventBus;

public class FullScreenSlideshowFragment extends Fragment implements AnimationListener {
	public static final String TAG = FullScreenSlideshowFragment.class.getSimpleName();
	private ViewAnimator mViewAnimator;
	private Animation mFadeIn, mFadeOut;
	
	private ImageManager mImageManager;

	private ArrayList<PlaybackData> mDatas;
	

	private ImageView mPlayIcon;
	private ImageView mPauseIcon;

	private Animation mShowIconAnimation;
	private Animation mShowIconAnimation2;
	
	private boolean mPlaying = false;

	private int mCurrentPosition = 0;
	
	private static final int AUTO_SLIDE_SHOW_DELAY_MILLIS = 3000;
	
	private Handler mSlideShowHandler = new Handler();
	private Runnable mSlideNextRunnable = new Runnable() {
		@Override
		public void run() {
			autoSlideNext();
		}
	};
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		mImageManager = SyncApplication.getWavefacePlayerApplication(getActivity()).getImageManager();
		
		if(savedInstanceState == null) {
			mDatas = getArguments().getParcelableArrayList(Constant.ARGUMENT1);		
			mCurrentPosition = getArguments().getInt(Constant.ARGUMENT2);
		} else {
			mDatas = savedInstanceState.getParcelableArrayList(Constant.ARGUMENT1);		
			mCurrentPosition = savedInstanceState.getInt(Constant.ARGUMENT2);
		}
	};
	
	@Override
	public void onSaveInstanceState(Bundle outState) {
		super.onSaveInstanceState(outState);
		
		outState.putParcelableArrayList(Constant.ARGUMENT1, mDatas);
		outState.putInt(Constant.ARGUMENT2, mCurrentPosition);
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		View root = inflater.inflate(R.layout.fragment_full_screen_pager, container, false);

		mFadeIn = new AlphaAnimation(0, 1);
		mFadeIn.setDuration(1000);

		mFadeOut = new AlphaAnimation(1, 0);
		mFadeOut.setDuration(1000);
		
		mViewAnimator = (ViewAnimator) root.findViewById(R.id.background_view);
		mViewAnimator.setDisplayedChild(0);
		mViewAnimator.setOnTouchListener(new View.OnTouchListener() {
			
			@Override
			public boolean onTouch(View v, MotionEvent event) {
				close();
				return true;
			}
		});
		
		resetToFirst();
		

		
		mPlayIcon = (ImageView) root.findViewById(R.id.play);
		mPauseIcon = (ImageView) root.findViewById(R.id.pause);
		
		mShowIconAnimation = AnimationUtils.loadAnimation(getActivity(), R.anim.scale_fade);
		mShowIconAnimation.setAnimationListener(this);
		
		mPlayIcon.startAnimation(mShowIconAnimation);
		return root;
	}
	
	private void close() {
		Log.d(TAG, "close");
		mSlideShowHandler.removeCallbacks(mSlideNextRunnable);

		mPauseIcon.setVisibility(View.VISIBLE);
		mPauseIcon.startAnimation(mShowIconAnimation);
	}
	
	public void onBackPressed() {
		close();
	}
	
	@Override
	public void onResume() {
		super.onResume();
		Log.d(TAG, "onresume");
		mSlideShowHandler.postDelayed(mSlideNextRunnable, AUTO_SLIDE_SHOW_DELAY_MILLIS);
	}
	
	@Override
	public void onPause() {
		super.onPause();
		Log.d(TAG, "onpause");
		mSlideShowHandler.removeCallbacks(mSlideNextRunnable);
	}

	private void resetToFirst() {
		setAnimatorImage(mCurrentPosition, 0);
		setAnimatorImage(mCurrentPosition+1, 1);
		mViewAnimator.setDisplayedChild(0);
	}
	
	private void setAnimatorImage(int imagePosition, int viewPosition) {
		if(imagePosition >= 0 && imagePosition < mDatas.size()) {
			ImageView iv = (ImageView) mViewAnimator.getChildAt(viewPosition);
			
			ImageAttribute attr = new ImageAttribute(iv);
			attr.setDoneScaleType(ScaleType.CENTER_CROP);
			attr.setLoadFromThread(true);
			attr.setMaxSizeEqualsScreenSize(getActivity());
			mImageManager.getImage( mDatas.get(imagePosition).url , attr);
		}
	}
	

	private int getNextView() {
		int next = mViewAnimator.getDisplayedChild() +1;
		if(next == 3)
			next = 0;
		return next;
	}
	
	private void autoSlideNext() {
		if(mCurrentPosition+1 < mDatas.size()) {
			mCurrentPosition++;
			mViewAnimator.setInAnimation(mFadeIn);
			mViewAnimator.setOutAnimation(mFadeOut);
			mViewAnimator.showNext();
			setAnimatorImage(mCurrentPosition+1, getNextView());
			mSlideShowHandler.postDelayed(mSlideNextRunnable, AUTO_SLIDE_SHOW_DELAY_MILLIS);
		}  else {
			// hit end
			Toast.makeText(getActivity(), R.string.everything_played, Toast.LENGTH_SHORT).show();
		}
	}
	
	public void onKeyEvent(int keyCode, KeyEvent event) {
		close();
	}

	@Override
	public void onAnimationEnd(Animation arg0) {
		mPlaying = !mPlaying;
		mPlayIcon.setVisibility(View.INVISIBLE);	
		mPauseIcon.setVisibility(View.INVISIBLE);
		
		if(mPlaying == false) {
			Log.d(TAG, "PlaybackCancelEvent");
			PlaybackCancelEvent event = new PlaybackCancelEvent();
			event.position = mCurrentPosition;
			EventBus.getDefault().post(event);
		}
	}

	@Override
	public void onAnimationRepeat(Animation arg0) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void onAnimationStart(Animation animation) {
		Log.d(TAG, "onAnimationStart");
		if(mPlaying == false) {
			mPlayIcon.setVisibility(View.VISIBLE);
		} else {
			mPauseIcon.setVisibility(View.VISIBLE);
		}
	}
}
