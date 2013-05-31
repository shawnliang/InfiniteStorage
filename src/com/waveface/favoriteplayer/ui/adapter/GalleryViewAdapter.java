package com.waveface.favoriteplayer.ui.adapter;

import idv.jason.lib.imagemanager.ImageAttribute;
import idv.jason.lib.imagemanager.ImageManager;

import java.util.ArrayList;

import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.entity.PlaybackData;
import com.waveface.favoriteplayer.ui.fragment.OverviewFragment;
import com.waveface.favoriteplayer.ui.widget.SquareImageView;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView.ScaleType;

public class GalleryViewAdapter extends BaseAdapter{
	private LayoutInflater mInflater;
	private ArrayList<PlaybackData> mDatas;
	private ImageManager mImageManager;
	private int mImageSize;
	private boolean mVideo;
	
	public GalleryViewAdapter(Context context, ArrayList<PlaybackData> datas, boolean showVideoPlaceholder) {
		mInflater = LayoutInflater.from(context);
		mDatas = datas;
		mImageManager = SyncApplication.getWavefacePlayerApplication(context).getImageManager();
		mImageSize = context.getResources().getDimensionPixelSize(R.dimen.galleryview_item_size);
		mVideo = showVideoPlaceholder;
	}

	@Override
	public int getCount() {
		return mDatas.size();
	}

	@Override
	public Object getItem(int arg0) {
		return mDatas.get(arg0);
	}

	@Override
	public long getItemId(int arg0) {
		return 0;
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent) {
		View root = mInflater.inflate(R.layout.item_galleryview, parent, false);
		
		SquareImageView iv = (SquareImageView) root.findViewById(R.id.picture);
		
		ImageAttribute attr = new ImageAttribute(iv);
		attr.setMaxSize(mImageSize, mImageSize);
		attr.setApplyWithAnimation(true);
		attr.setLoadFromThread(true);
		attr.setDoneScaleType(ScaleType.CENTER_CROP);
		mImageManager.getImage(mDatas.get(position).url, attr);
		
		if(mVideo) {
			root.findViewById(R.id.image_play).setVisibility(View.VISIBLE);
		}
		
		return root;
	}

}
