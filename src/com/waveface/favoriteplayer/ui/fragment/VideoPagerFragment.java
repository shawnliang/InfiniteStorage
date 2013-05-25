package com.waveface.favoriteplayer.ui.fragment;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.ui.adapter.VideoPagerAdapter;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.view.ViewPager;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

public class VideoPagerFragment extends Fragment{
	private ViewPager mPager;
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		
		Bundle data = null;
		if(savedInstanceState == null) {
			data = getArguments();
		} else {
			data = savedInstanceState;
		}
		
		String labelId = data.getString(Constant.ARGUMENT1);
		
		View root = inflater.inflate(R.layout.fragment_videopager, container, false);
		
		mPager = (ViewPager) root.findViewById(R.id.pager);
		mPager.setAdapter(new VideoPagerAdapter(getActivity(), labelId));
		
		return root;
	}
}
