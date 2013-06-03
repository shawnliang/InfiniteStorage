package com.waveface.favoriteplayer.ui.fragment;

import idv.jason.lib.imagemanager.ImageAttribute;
import idv.jason.lib.imagemanager.ImageManager;

import java.util.ArrayList;

import android.media.MediaPlayer;
import android.media.MediaPlayer.OnCompletionListener;
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
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.ImageView.ScaleType;
import android.widget.MediaController;
import android.widget.Toast;
import android.widget.VideoView;
import android.widget.ViewAnimator;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.entity.PlaybackData;
import com.waveface.favoriteplayer.event.PlaybackCancelEvent;

import de.greenrobot.event.EventBus;

public class FullScreenSlideshowFragment extends Fragment implements AnimationListener,OnCompletionListener {
	public static final String TAG = FullScreenSlideshowFragment.class.getSimpleName();
	private ViewAnimator mViewAnimator;
	private Animation mFadeIn, mFadeOut;
	
	private FrameLayout mSlideFrame;
	private ImageView mSlideFrameImage;	
	private VideoView mVV;
	
	
	private ImageManager mImageManager;

	private ArrayList<PlaybackData> mDatas;
	

	private ImageView mPlayIcon;
	private ImageView mPauseIcon;

	private Animation mShowIconAnimation;
	
	private boolean mPlaying = false;

	private int mCurrentPosition = 0;
	
	private static final int AUTO_SLIDE_SHOW_DELAY_MILLIS = 3000;
	private static final int AUTO_SLIDE_VIDEO_SHOW_DELAY_MILLIS = 1500;

	
	private Handler mSlideShowHandler = new Handler();
	private Runnable mSlideNextRunnable = new Runnable() {
		@Override
		public void run() {
			autoSlideNext();
		}
	};
	
	private Handler mHandler = new Handler();
	private Runnable mPlayVideoRunnable = new Runnable() {
		
		@Override
		public void run() {
			playVideo();
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

		mSlideFrame = (FrameLayout) root.findViewById(R.id.slideVideoFrame);
		mSlideFrameImage = (ImageView)root.findViewById(R.id.slideVideoImage);
		
		mVV = (VideoView) root.findViewById(R.id.fullVideoView);
		mVV.setOnCompletionListener(this);
		
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
		setAnimatorImage(getNextItem(), 1);
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
	
	private boolean moveNext() {
//		do {
//			mCurrentPosition++;
//			if(mCurrentPosition >= mDatas.size()) {
//				return false;
//			}
//		} while(mCurrentPosition < mDatas.size());
//		return true;
		mCurrentPosition++;
		if(mCurrentPosition >= mDatas.size()) {
			return false;
		}
		else{
			return true;
		}
	}
	
	private int getNextItem() {
		int position = mCurrentPosition;
		do {
			position++;
			if(position >= mDatas.size()) {
				return -1;
			}
		} while(Constant.FILE_TYPE_IMAGE.equals(mDatas.get(position).type) == false);
		return position;
	}
	
	private void autoSlideNext() {
		if(moveNext()) {			
			if(mDatas.get(mCurrentPosition).type.equals(Constant.FILE_TYPE_IMAGE)){
				mVV.setVisibility(View.INVISIBLE);
				mSlideFrame.setVisibility(View.INVISIBLE);
				mViewAnimator.setVisibility(View.VISIBLE);
				mViewAnimator.setInAnimation(mFadeIn);
				mViewAnimator.setOutAnimation(mFadeOut);
				mViewAnimator.showNext();
				setAnimatorImage(getNextItem(), getNextView());
				mSlideShowHandler.postDelayed(mSlideNextRunnable, AUTO_SLIDE_SHOW_DELAY_MILLIS);
			}
			else{
				mViewAnimator.setVisibility(View.INVISIBLE);
				mVV.setVisibility(View.VISIBLE);
				mSlideFrame.setVisibility(View.VISIBLE);
				ImageAttribute attr = new ImageAttribute(mSlideFrameImage);
				attr.setMaxSizeEqualsScreenSize(getActivity());
				attr.setDoneScaleType(ScaleType.FIT_CENTER);
				attr.setApplyWithAnimation(true);
				mImageManager.getImage(mDatas.get(mCurrentPosition).url, attr);
				mHandler.postDelayed(mPlayVideoRunnable, AUTO_SLIDE_VIDEO_SHOW_DELAY_MILLIS);
			}
			
		}  else {
			// hit end
			Toast.makeText(getActivity(), R.string.everything_played, Toast.LENGTH_SHORT).show();
			PlaybackCancelEvent event = new PlaybackCancelEvent();
			event.position = 0;
			EventBus.getDefault().post(event);
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

	@Override
	public void onCompletion(MediaPlayer arg0) {
		if(moveNext()) {
			mSlideShowHandler.postDelayed(mSlideNextRunnable, AUTO_SLIDE_SHOW_DELAY_MILLIS);
		} else {
			Toast.makeText(getActivity(), R.string.everything_played, Toast.LENGTH_SHORT).show();
			PlaybackCancelEvent event = new PlaybackCancelEvent();
			event.position = 0;
			EventBus.getDefault().post(event);
		}
	}
	
	private void playVideo(){
		mVV.setVideoPath(mDatas.get(mCurrentPosition).url);
        mVV.setMediaController(new MediaController(getActivity()));
        mVV.requestFocus();
        mVV.start();			
	}
}
