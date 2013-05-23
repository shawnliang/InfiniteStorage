package com.waveface.favoriteplayer.ui;

import android.os.Bundle;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentTransaction;

import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.ui.fragment.FullScreenSlideshowFragment;

public class FullScreenSlideShowActivity extends FragmentActivity {
	@Override
	protected void onCreate(Bundle savedInstance) {
		super.onCreate(savedInstance);

		overridePendingTransition(R.anim.fade_in, R.anim.fade_out);
		
		setContentView(R.layout.activity_full_screen_slideshow);
		
		if(savedInstance == null) {
			FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
			FullScreenSlideshowFragment fragment = new FullScreenSlideshowFragment();
			
			fragment.setArguments(getIntent().getExtras());
			transaction.add(R.id.content, fragment, FullScreenSlideshowFragment.class.getSimpleName()).commit();
		}
	}
}
