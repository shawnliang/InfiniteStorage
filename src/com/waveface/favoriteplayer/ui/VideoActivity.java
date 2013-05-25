package com.waveface.favoriteplayer.ui;

import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.ui.fragment.VideoFragment;

import android.os.Bundle;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentTransaction;

public class VideoActivity extends FragmentActivity{
	
	@Override
	protected void onCreate(Bundle arg0) {
		super.onCreate(arg0);
		
		setContentView(R.layout.activity_video);
		
		VideoFragment fragment = new VideoFragment();
		fragment.setArguments(getIntent().getExtras());
		FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
		transaction.add(R.id.content, fragment, VideoFragment.class.getSimpleName());
		transaction.commit();
	}

}
