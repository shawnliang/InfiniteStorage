package com.waveface.favoriteplayer.ui.fragment;

import java.util.ArrayList;

import idv.jason.lib.imagemanager.ImageManager;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.db.LabelFileView;
import com.waveface.favoriteplayer.entity.PlaybackData;
import com.waveface.favoriteplayer.event.PlaybackItemClickEvent;
import com.waveface.favoriteplayer.ui.adapter.VideoPagerAdapter;

import de.greenrobot.event.EventBus;

import android.database.Cursor;
import android.graphics.Bitmap;
import android.media.ThumbnailUtils;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore.Video.Thumbnails;
import android.support.v4.app.Fragment;
import android.support.v4.view.ViewPager;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

public class VideoPagerFragment extends Fragment{
	private ViewPager mPager;
	private ArrayList<PlaybackData> mDatas = new ArrayList<PlaybackData>();
	
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
		
		Cursor lfc = LabelDB.getLabelFileViewByLabelId(getActivity(), labelId);
		ImageManager imageManager = SyncApplication.getWavefacePlayerApplication(getActivity()).getImageManager();
		for(int i=0; i<lfc.getCount(); ++i) {
			lfc.moveToPosition(i);
			PlaybackData vdata = new PlaybackData();
			String fileName =  Environment.getExternalStorageDirectory().getAbsolutePath()
					+ Constant.VIDEO_FOLDER+ "/"  +lfc
					.getString(lfc
							.getColumnIndex(LabelFileView.COLUMN_FILE_NAME));
			Bitmap bmThumbnail = ThumbnailUtils.createVideoThumbnail(fileName, 
			        Thumbnails.MINI_KIND);

			imageManager.setBitmapToFile(bmThumbnail, fileName, null, false);
			vdata.url = fileName;
			mDatas.add(vdata);
		}
		lfc.close();

		
		View root = inflater.inflate(R.layout.fragment_videopager, container, false);
		
		mPager = (ViewPager) root.findViewById(R.id.pager);
		mPager.setAdapter(new VideoPagerAdapter(getActivity(), mDatas));
		
		return root;
	}
	
	public boolean onKeyEvent(int keyCode, KeyEvent event) {
		switch(keyCode) {
		case KeyEvent.KEYCODE_ENTER:
		case KeyEvent.KEYCODE_MEDIA_PLAY:
			PlaybackItemClickEvent vevent = new PlaybackItemClickEvent();
			vevent.position = mPager.getCurrentItem();

			EventBus.getDefault().post(vevent);
			return true;
		}
		return false;
	}
}
