package com.waveface.favoriteplayer.ui;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.event.VideoItemClickEvent;
import com.waveface.favoriteplayer.ui.fragment.VideoFragment;
import com.waveface.favoriteplayer.ui.fragment.VideoPagerFragment;

import de.greenrobot.event.EventBus;

import android.os.Bundle;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentTransaction;
import android.view.KeyEvent;

public class VideoActivity extends FragmentActivity{
	private String mCurrentFragment;
	
	@Override
	protected void onCreate(Bundle arg0) {
		super.onCreate(arg0);
		
		setContentView(R.layout.activity_video);
		
		Bundle data = null;
		
		if(arg0 == null) {
			data = getIntent().getExtras();
		} else {
			data = arg0;
		}
		

		FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
		mCurrentFragment = VideoPagerFragment.class.getSimpleName();
		VideoPagerFragment video = new VideoPagerFragment();
		video.setArguments(data);
		transaction.add(R.id.content, video, mCurrentFragment);
		transaction.addToBackStack(mCurrentFragment);
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
	
	public void onEvent(VideoItemClickEvent event) {
		VideoFragment fragment = new VideoFragment();
		Bundle data = new Bundle();
		data.putParcelableArrayList(Constant.ARGUMENT1, event.urls);
		data.putInt(Constant.ARGUMENT2, event.position);
		fragment.setArguments(data);
		FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
		transaction.add(R.id.content, fragment, VideoFragment.class.getSimpleName());
		mCurrentFragment = VideoFragment.class.getSimpleName();
		transaction.addToBackStack(mCurrentFragment);
		transaction.commit();
	}
	
	@Override
	public void onBackPressed() {
		if(VideoFragment.class.getSimpleName().equals(mCurrentFragment)) {
			getSupportFragmentManager().popBackStack();
			mCurrentFragment = VideoPagerFragment.class.getSimpleName();
		} else {
			finish();
		}
	}
	
	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {
		if(VideoPagerFragment.class.getSimpleName().equals(mCurrentFragment) && keyCode != KeyEvent.KEYCODE_BACK) {
			VideoPagerFragment fragment = (VideoPagerFragment) getSupportFragmentManager().findFragmentByTag(mCurrentFragment);
			return fragment.onKeyEvent(keyCode, event);
		} else {
			return super.onKeyDown(keyCode, event);
		}
	}

}
