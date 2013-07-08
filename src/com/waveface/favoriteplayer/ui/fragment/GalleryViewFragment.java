package com.waveface.favoriteplayer.ui.fragment;

import java.util.ArrayList;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.entity.PlaybackData;
import com.waveface.favoriteplayer.event.PlaybackItemClickEvent;
import com.waveface.favoriteplayer.ui.adapter.GalleryViewAdapter;

import de.greenrobot.event.EventBus;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.view.animation.AnimationUtils;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.GridView;
import android.widget.TextView;

public class GalleryViewFragment extends Fragment implements OnItemClickListener, OnClickListener{
	private GridView mGridView;
	private View mRootView = null;
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		
		Bundle data = null;
		
		if(savedInstanceState == null) {
			data = getArguments();
		} else {
			data = savedInstanceState;
		}
		
		mRootView = inflater.inflate(R.layout.fragment_galleryview, container, false);
		
		mGridView = (GridView) mRootView.findViewById(R.id.grid_view);
		ArrayList<PlaybackData> datas = data.getParcelableArrayList(Constant.ARGUMENT1);
		GalleryViewAdapter adapter = new GalleryViewAdapter(getActivity(), datas);
		mGridView.setAdapter(adapter);
		
		mGridView.requestFocus();
		mGridView.setOnItemClickListener(this);
		
		return mRootView;
	}
	
	public void fadeOut() {
		mRootView.startAnimation(AnimationUtils.loadAnimation(getActivity(), R.anim.fade_out));
	}
	
	public void onKeyDown(int keyCode, KeyEvent event) {
		mGridView.onKeyDown(keyCode, event);
	}

	@Override
	public void onItemClick(AdapterView<?> gridView, View view, int position, long id) {
		PlaybackItemClickEvent event = new PlaybackItemClickEvent();
		event.position = position;
		EventBus.getDefault().post(event);
	}

	@Override
	public void onClick(View arg0) {
		getActivity().finish();
		getActivity().overridePendingTransition(R.anim.fade_in, R.anim.fade_out);
	}
	
	

}
