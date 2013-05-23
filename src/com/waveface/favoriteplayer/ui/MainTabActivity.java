package com.waveface.favoriteplayer.ui;

import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.ui.fragment.OverviewFragment;
import com.waveface.favoriteplayer.ui.fragment.PlaybackFragment;

import android.os.Bundle;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentTabHost;
import android.view.LayoutInflater;
import android.view.ViewGroup;
import android.widget.TabHost;
import android.widget.TextView;

public class MainTabActivity extends FragmentActivity{
	private FragmentTabHost mTabHost;
	
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
        TabHost.TabSpec spec = mTabHost.newTabSpec(OverviewFragment.class.getSimpleName());
        spec.setIndicator(tabInfo);
        mTabHost.addTab(spec,
                OverviewFragment.class, arg0);
        
        tabInfo = (ViewGroup) inflater.inflate(R.layout.item_tabinfo, null, false);
        tv = (TextView) tabInfo.findViewById(R.id.tabinfo);
        tv.setText(getResources().getString(R.string.recent_photos));
        spec = mTabHost.newTabSpec(PlaybackFragment.class.getSimpleName());
        spec.setIndicator(tabInfo);        
        mTabHost.addTab(spec,
                PlaybackFragment.class, arg0);
        
        tabInfo = (ViewGroup) inflater.inflate(R.layout.item_tabinfo, null, false);
        tv = (TextView) tabInfo.findViewById(R.id.tabinfo);
        tv.setText(getResources().getString(R.string.recent_videos));
        spec = mTabHost.newTabSpec("recent videos");
        spec.setIndicator(tabInfo);        
        mTabHost.addTab(spec,
        		OverviewFragment.class, arg0);
	}
	
}
