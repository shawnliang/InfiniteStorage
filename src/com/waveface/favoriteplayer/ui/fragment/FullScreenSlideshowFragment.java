package com.waveface.favoriteplayer.ui.fragment;

import java.util.ArrayList;

import idv.jason.lib.imagemanager.ImageAttribute;
import idv.jason.lib.imagemanager.ImageManager;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.entity.OverviewData;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.view.animation.AlphaAnimation;
import android.view.animation.Animation;
import android.widget.ImageView;
import android.widget.Toast;
import android.widget.ViewAnimator;
import android.widget.ImageView.ScaleType;

public class FullScreenSlideshowFragment extends Fragment {
	private ViewAnimator mViewAnimator;
	private Animation mFadeIn, mFadeOut;
	
	private ImageManager mImageManager;

	private ArrayList<OverviewData> mDatas;

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
		return root;
	}
	
	private void close() {
		Intent intent = new Intent();
		intent.putExtra(Constant.ARGUMENT1, mCurrentPosition);
		getActivity().setResult(Activity.RESULT_OK, intent);
		getActivity().finish();
	}
	
	
	public void onBackPressed() {
		close();
	}
	
	@Override
	public void onResume() {
		super.onResume();
		mSlideShowHandler.postDelayed(mSlideNextRunnable, AUTO_SLIDE_SHOW_DELAY_MILLIS);
	}
	
	@Override
	public void onPause() {
		super.onPause();
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
}
