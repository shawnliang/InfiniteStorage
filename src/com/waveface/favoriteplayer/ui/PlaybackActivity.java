package com.waveface.favoriteplayer.ui;

import android.os.Bundle;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentTransaction;
import android.util.Log;
import android.view.KeyEvent;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.event.PhotoItemClickEvent;
import com.waveface.favoriteplayer.event.PlaybackCancelEvent;
import com.waveface.favoriteplayer.ui.fragment.FullScreenSlideshowFragment;
import com.waveface.favoriteplayer.ui.fragment.PlaybackFragment;

import de.greenrobot.event.EventBus;

public class PlaybackActivity extends FragmentActivity {
	public static final String TAG = PlaybackActivity.class.getSimpleName();
	private String mCurrentFragment;
	
	@Override
	protected void onCreate(Bundle savedInstance) {
		super.onCreate(savedInstance);

		overridePendingTransition(R.anim.fade_in, R.anim.fade_out);
		
		setContentView(R.layout.activity_full_screen_slideshow);
		
		Bundle data = null;
		if(savedInstance == null) {
			data = getIntent().getExtras();
		} else {
			data = savedInstance;
		}
		

		FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
		mCurrentFragment = PlaybackFragment.class.getSimpleName();
		PlaybackFragment playback = new PlaybackFragment();
		playback.setArguments(data);
		transaction.setCustomAnimations(android.R.anim.fade_in, android.R.anim.fade_out);
		
		transaction.add(R.id.content, playback, mCurrentFragment);
		transaction.commit();
	}
	
	@Override
	protected void onResume() {
		super.onResume();
		EventBus.getDefault().register(this);
	}
	
	@Override
	protected void onPause() {
		super.onPause();
		EventBus.getDefault().unregister(this);
	}
	
	public void onEvent(PhotoItemClickEvent event) {
		Log.d(TAG, "PhotoItemClickEvent");
		Bundle data = new Bundle();
		data.putParcelableArrayList(Constant.ARGUMENT1, event.datas);
		data.putInt(Constant.ARGUMENT2, event.position);

		FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
		FullScreenSlideshowFragment fragment = new FullScreenSlideshowFragment();
		
		fragment.setArguments(data);
		mCurrentFragment = FullScreenSlideshowFragment.class.getSimpleName();
		transaction.addToBackStack(null);
//		transaction.setCustomAnimations(android.R.anim.fade_in, android.R.anim.fade_out, android.R.anim.fade_in, android.R.anim.fade_out);
		transaction.replace(R.id.content, fragment, mCurrentFragment).commit();
	}
	
	public void onEvent(PlaybackCancelEvent event) {
		getSupportFragmentManager().popBackStack();
		mCurrentFragment = PlaybackFragment.class.getSimpleName();

		PlaybackFragment fragment = (PlaybackFragment) getSupportFragmentManager().findFragmentByTag(mCurrentFragment);
		fragment.pause(event.position);
	}
	
	@Override
	public void onBackPressed() {
		Log.d(TAG, "onBackPressed:" + mCurrentFragment);
		if(PlaybackFragment.class.getSimpleName().equals(mCurrentFragment)) {
			finish();
		} 
	}
	
	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {
		Log.d(TAG, "onKeyDown:" + keyCode);
		if(PlaybackFragment.class.getSimpleName().equals(mCurrentFragment) && keyCode != KeyEvent.KEYCODE_BACK) {
			PlaybackFragment fragment = (PlaybackFragment) getSupportFragmentManager().findFragmentByTag(mCurrentFragment);
			fragment.onKeyEvent(keyCode, event);
			return true;
		} else if(FullScreenSlideshowFragment.class.getSimpleName().equals(mCurrentFragment)){
			FullScreenSlideshowFragment fragment = (FullScreenSlideshowFragment) getSupportFragmentManager().findFragmentByTag(mCurrentFragment);
			fragment.onKeyEvent(keyCode, event);
			return true;
		} else {
			Log.d(TAG, "mCurrentFragment=" + mCurrentFragment);
		}
		return super.onKeyDown(keyCode, event);
	}
}
