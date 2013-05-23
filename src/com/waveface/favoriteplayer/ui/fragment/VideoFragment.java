package com.waveface.favoriteplayer.ui.fragment;

import android.database.Cursor;
import android.graphics.Bitmap;
import android.graphics.PixelFormat;
import android.media.ThumbnailUtils;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore.Video.Thumbnails;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.MediaController;
import android.widget.VideoView;


import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.db.LabelFileView;

import de.greenrobot.event.EventBus;

public class VideoFragment extends Fragment implements OnClickListener{
	public static final String TAG = VideoFragment.class.getSimpleName(); 
	
	private String mFullFilename ;
	private VideoView mVV;
	private ImageView mIVVideoThumb ;
	private ImageView mIVVideoPlay ;
	
	
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
	};
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		getActivity().getWindow().setFormat(PixelFormat.TRANSLUCENT);
	        
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
				if (type.equals(Constant.FILE_TYPE_VIDEO)) {
					mFullFilename = Environment.getExternalStorageDirectory().getAbsolutePath()
							+ Constant.VIDEO_FOLDER+ "/" + fileName;
					break;
				}
				filecursor.moveToNext();				
			}
		}
		filecursor.close();
		
		View root = inflater.inflate(R.layout.fragment_video_play, container, false);
        
		// MINI_KIND: 512 x 384 thumbnail 
		mIVVideoThumb = (ImageView) root.findViewById(R.id.ivVideoThumb);
		mIVVideoThumb.setOnClickListener(this);
		mIVVideoPlay = (ImageView) root.findViewById(R.id.ivVideoPlay);
		
		Bitmap bmThumbnail = ThumbnailUtils.createVideoThumbnail(mFullFilename, 
        Thumbnails.MINI_KIND);
		mIVVideoThumb.setImageBitmap(bmThumbnail);
		mVV = (VideoView) root.findViewById(R.id.myvideoview);
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
	
	
	@Override
	public void onClick(View v) {
		switch(v.getId()){
			case R.id.ivVideoThumb:
				PlayVideo();
				break;
		}
	}
}
