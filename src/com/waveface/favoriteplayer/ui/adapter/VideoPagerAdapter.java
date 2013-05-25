package com.waveface.favoriteplayer.ui.adapter;

import idv.jason.lib.imagemanager.ImageAttribute;
import idv.jason.lib.imagemanager.ImageManager;

import java.util.ArrayList;

import android.content.Context;
import android.content.Intent;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.media.ThumbnailUtils;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore.Video.Thumbnails;
import android.support.v4.view.PagerAdapter;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.ImageView.ScaleType;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.db.LabelFileView;
import com.waveface.favoriteplayer.ui.VideoActivity;

public class VideoPagerAdapter extends PagerAdapter implements OnClickListener{
	private ImageManager mImageManager;
	private ArrayList<VideoData> mThumbnails = new ArrayList<VideoData>();
	private LayoutInflater mInflater;
	
	class VideoData {
		public String thumbnail;
		public String video;
	}
	
	public VideoPagerAdapter(Context context, String labelId) {
		mImageManager = SyncApplication.getWavefacePlayerApplication(context).getImageManager();
		Cursor lfc = LabelDB.getLabelFileViewByLabelId(context, labelId);
		for(int i=0; i<lfc.getCount(); ++i) {
			lfc.moveToPosition(i);
			VideoData data = new VideoData();
			String fileName =  Environment.getExternalStorageDirectory().getAbsolutePath()
					+ Constant.VIDEO_FOLDER+ "/"  +lfc
					.getString(lfc
							.getColumnIndex(LabelFileView.COLUMN_FILE_NAME));
			Bitmap bmThumbnail = ThumbnailUtils.createVideoThumbnail(fileName, 
			        Thumbnails.MINI_KIND);

//			mImageManager.setBitmapToFile(bmThumbnail, fileName, null, false);
			data.thumbnail = fileName;
			mThumbnails.add(data);
		}
		lfc.close();
		
		mInflater = LayoutInflater.from(context);
	}

	@Override
	public int getCount() {
		return mThumbnails.size();
	}

	@Override
	public boolean isViewFromObject(View view, Object object) {
		return view == ((View) object);
	}

	@Override
	public void destroyItem(ViewGroup container, int position, Object object) {
		container.removeView((View) object);
	}


	@Override
	public Object instantiateItem(ViewGroup container, int position) {
		View root = mInflater.inflate(R.layout.item_player_pager, container,
				false);
		ImageView iv = (ImageView) root.findViewById(R.id.image);
		iv.setOnClickListener(this);
		ImageAttribute attr = new ImageAttribute(iv);
//		attr.setMaxSizeEqualsScreenSize(mInflater.getContext());
		attr.setDoneScaleType(ScaleType.FIT_CENTER);
		mImageManager.getImage(mThumbnails.get(position).thumbnail, attr);
		container.addView(root);
		iv.setTag(position);
		return root;
	}

	@Override
	public void onClick(View v) {
		int position = (Integer) v.getTag();

		Context context = mInflater.getContext();
		Intent intent = new Intent(mInflater.getContext(), VideoActivity.class);
		Bundle data = new Bundle();
		data.putString(Constant.ARGUMENT1, mThumbnails.get(position).thumbnail);
		intent.putExtras(data);
		context.startActivity(intent);
		
		
	}
}
