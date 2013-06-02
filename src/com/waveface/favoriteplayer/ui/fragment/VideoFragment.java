package com.waveface.favoriteplayer.ui.fragment;

import java.util.ArrayList;

import android.graphics.PixelFormat;
import android.media.MediaPlayer;
import android.media.MediaPlayer.OnCompletionListener;
import android.os.Bundle;
import android.os.Handler;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.MediaController;
import android.widget.Toast;
import android.widget.VideoView;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.entity.PlaybackData;
import com.waveface.favoriteplayer.event.PlaybackCancelEvent;

import de.greenrobot.event.EventBus;

public class VideoFragment extends Fragment implements OnCompletionListener{
	public static final String TAG = VideoFragment.class.getSimpleName(); 
	
	private VideoView mVV;
	
	private int mCurrentPosition;
	private ArrayList<PlaybackData> mVideos;
	
	private Handler mHandler = new Handler();
	private Runnable mPlayRunnable = new Runnable() {
		
		@Override
		public void run() {
			playVideo();
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
		
		mVideos = data.getParcelableArrayList(Constant.ARGUMENT1);
		mCurrentPosition = data.getInt(Constant.ARGUMENT2);
		
		getActivity().getWindow().setFormat(PixelFormat.TRANSLUCENT);
		
		View root = inflater.inflate(R.layout.fragment_video_play, container, false);
        
		mVV = (VideoView) root.findViewById(R.id.myvideoview);
		mVV.setOnCompletionListener(this);

		mHandler.postDelayed(mPlayRunnable, 200);
		return root;
	}
	
	private void playVideo(){
		mVV.setVideoPath(mVideos.get(mCurrentPosition).url);
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
	
	private boolean moveNext() {
		do {
			mCurrentPosition++;
			if(mCurrentPosition >= mVideos.size()) {
				return false;
			}
		} while(Constant.FILE_TYPE_VIDEO.equals(mVideos.get(mCurrentPosition).type) == false);
		return true;
	}

	@Override
	public void onCompletion(MediaPlayer arg0) {
		if(moveNext()) {
			playVideo();
		} else {
			Toast.makeText(getActivity(), R.string.everything_played, Toast.LENGTH_SHORT).show();
			PlaybackCancelEvent event = new PlaybackCancelEvent();
			event.position = 0;
			EventBus.getDefault().post(event);
		}
	}
}
