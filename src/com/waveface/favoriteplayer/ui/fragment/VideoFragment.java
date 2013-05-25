package com.waveface.favoriteplayer.ui.fragment;

import android.graphics.PixelFormat;
import android.os.Bundle;
import android.os.Handler;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.MediaController;
import android.widget.VideoView;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;

public class VideoFragment extends Fragment{
	public static final String TAG = VideoFragment.class.getSimpleName(); 
	
	private String mFullFilename ;
	private VideoView mVV;
	
	private Handler mHandler = new Handler();
	private Runnable mPlayRunnable = new Runnable() {
		
		@Override
		public void run() {
			PlayVideo();
		}
	};
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
	};
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		
		Bundle data = null;
		
		if(savedInstanceState == null) {
			data = getArguments();
		} else {
			data = savedInstanceState;
		}
		
		mFullFilename = data.getString(Constant.ARGUMENT1);
		
		getActivity().getWindow().setFormat(PixelFormat.TRANSLUCENT);
		
		View root = inflater.inflate(R.layout.fragment_video_play, container, false);
        
		mVV = (VideoView) root.findViewById(R.id.myvideoview);

		mHandler.postDelayed(mPlayRunnable, 200);
		return root;
	}
	
	private void PlayVideo(){
		mVV.setVisibility(View.VISIBLE);
		mVV.setVideoPath(mFullFilename);
        mVV.setMediaController(new MediaController(getActivity()));
        mVV.requestFocus();
        mVV.start();			
	}
	
	@Override
	public void onPause() {
		super.onPause();
	}
	
	@Override
	public void onResume() {
		super.onResume();
	}

	@Override
	public void onDestroy() {
		super.onDestroy();
	}
}
