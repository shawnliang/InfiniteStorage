package com.waveface.favoriteplayer.ui.fragment;

import java.io.File;
import java.io.IOException;

import android.database.Cursor;
import android.graphics.PixelFormat;
import android.media.MediaPlayer;
import android.os.Bundle;
import android.os.Environment;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.MediaController;
import android.widget.VideoView;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.db.LabelFileView;

public class VideoFragment extends Fragment{
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
	};
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		getActivity().getWindow().setFormat(PixelFormat.TRANSLUCENT);
		String fullFilename = "";
	        
		String labelId = LabelDB.getVideoLabelId(getActivity());   
		Cursor filecursor = LabelDB.getLabelFileViewByLabelId(getActivity(), labelId);
		if(filecursor!=null && filecursor.getCount() > 0) {
			filecursor.moveToFirst();
			int count = filecursor.getCount();
			for (int j = 0; j < count; j++) {

				String type = filecursor
						.getString(filecursor
								.getColumnIndex(LabelFileView.COLUMN_TYPE));
				String fileName = filecursor
						.getString(filecursor
								.getColumnIndex(LabelFileView.COLUMN_FILE_NAME));
				
				if (type.equals("1")) {
					fullFilename = Environment.getExternalStorageDirectory().getAbsolutePath()
							+ Constant.VIDEO_FOLDER+ "/" + fileName;
					break;
				}
				filecursor.moveToNext();				
			}
		}
		filecursor.close();
		
//		View root = inflater.inflate(R.layout.fragment_video_play, container, false);
		VideoView vv = new VideoView(getActivity());
//		setContentView(vv);
	    if (new File(fullFilename).exists()) {
//			VideoView vv = (VideoView) root.findViewById(R.id.myvideoview);
	        vv.setVideoPath(fullFilename);
	        MediaController MC = new MediaController(getActivity());
	        vv.setMediaController(MC);
	        vv.requestFocus();
	        vv.start();	
	    }        
		return vv;
	}
	
	@Override
	public void onPause() {
		super.onPause();
	}
	
	@Override
	public void onResume() {
		super.onResume();
	}

}
