package com.waveface.favoriteplayer.ui;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentTabHost;
import android.support.v4.app.FragmentTransaction;
import android.util.Log;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.ViewGroup;
import android.widget.TabHost;
import android.widget.TextView;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.event.DispatchKeyEvent;
import com.waveface.favoriteplayer.event.OverviewItemClickEvent;
import com.waveface.favoriteplayer.ui.fragment.OverviewFragment;
import com.waveface.favoriteplayer.ui.fragment.PlaybackFragment;
import com.waveface.favoriteplayer.ui.fragment.VideoFragment;
import com.waveface.favoriteplayer.ui.fragment.VideoPagerFragment;

import de.greenrobot.event.EventBus;

public class MainTabActivity extends FragmentActivity{
	private static final String TAG = MainTabActivity.class.getSimpleName();
	private FragmentTabHost mTabHost;
	private static final String FAVORITETAG = OverviewFragment.class.getSimpleName() + "_favorite";
	
	private String mCurrentFragment;
	
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
	
	public void onEvent(OverviewItemClickEvent event) {
		FragmentTransaction transaction;
		Bundle data;
		switch(event.type) {
		case OverviewFragment.OVERVIEW_VIEW_TYPE_FAVORITE:
		case OverviewFragment.OVERVIEW_VIEW_TYPE_RECENT_PHOTO:
			transaction = getSupportFragmentManager().beginTransaction();
			mCurrentFragment = PlaybackFragment.class.getSimpleName();
			PlaybackFragment playback = new PlaybackFragment();
			data = new Bundle();
			data.putString(Constant.ARGUMENT1, event.labelId);
			playback.setArguments(data);
			
			transaction.add(R.id.realtab_content, playback, mCurrentFragment);
			transaction.addToBackStack(mCurrentFragment);
			transaction.commit();
			break;
		case OverviewFragment.OVERVIEW_VIEW_TYPE_RECENT_VIDEO:
			transaction = getSupportFragmentManager().beginTransaction();
			mCurrentFragment = VideoPagerFragment.class.getSimpleName();
			VideoPagerFragment video = new VideoPagerFragment();
			data = new Bundle();
			data.putString(Constant.ARGUMENT1, event.labelId);
			video.setArguments(data);
			
			transaction.add(R.id.realtab_content, video, mCurrentFragment);
			transaction.addToBackStack(mCurrentFragment);
			transaction.commit();
			break;
		}
	}
	
	@Override
	public void onBackPressed() {
		if(mCurrentFragment == null) {
			super.onBackPressed();
		} else {
			getSupportFragmentManager().popBackStack();
			mCurrentFragment = null;
		}
	}
	
	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {
		Log.d(TAG, "onKeyDown:"  + keyCode);
		if(mCurrentFragment != null && keyCode != KeyEvent.KEYCODE_BACK) {
			EventBus.getDefault().post(new DispatchKeyEvent(keyCode));
			return true;
		} else {
			return super.onKeyDown(keyCode, event);
		}
	}
	
	@Override
	protected void onCreate(Bundle arg0) {
		super.onCreate(arg0);
		
        setContentView(R.layout.activity_maintab);
        mTabHost = (FragmentTabHost)findViewById(android.R.id.tabhost);
        mTabHost.setup(this, getSupportFragmentManager(), R.id.realtab_content);
        
        LayoutInflater inflater = LayoutInflater.from(this);
        ViewGroup tabInfo = (ViewGroup) inflater.inflate(R.layout.item_tabinfo, null, false);
        TextView tv = (TextView) tabInfo.findViewById(R.id.tabinfo);
        tv.setText(getResources().getString(R.string.favorite));
        TabHost.TabSpec spec = mTabHost.newTabSpec(FAVORITETAG);
        spec.setIndicator(tabInfo);
        
        Bundle data = new Bundle();
        data.putInt(Constant.ARGUMENT1, OverviewFragment.OVERVIEW_VIEW_TYPE_FAVORITE);
        
        mTabHost.addTab(spec,
                OverviewFragment.class, data);
        
        tabInfo = (ViewGroup) inflater.inflate(R.layout.item_tabinfo, null, false);
        tv = (TextView) tabInfo.findViewById(R.id.tabinfo);
        tv.setText(getResources().getString(R.string.recent_photos));
        spec = mTabHost.newTabSpec(PlaybackFragment.class.getSimpleName());
        spec.setIndicator(tabInfo);        

        data = new Bundle();
        data.putInt(Constant.ARGUMENT1, OverviewFragment.OVERVIEW_VIEW_TYPE_RECENT_PHOTO);
        mTabHost.addTab(spec,
        		OverviewFragment.class, data);
        
        tabInfo = (ViewGroup) inflater.inflate(R.layout.item_tabinfo, null, false);
        tv = (TextView) tabInfo.findViewById(R.id.tabinfo);
        tv.setText(getResources().getString(R.string.recent_videos));
        spec = mTabHost.newTabSpec("recent videos");
        spec.setIndicator(tabInfo);
        
        data = new Bundle();
        data.putInt(Constant.ARGUMENT1, OverviewFragment.OVERVIEW_VIEW_TYPE_RECENT_VIDEO);
        mTabHost.addTab(spec,
        		OverviewFragment.class, data);
	}
	
}
