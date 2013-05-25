package com.waveface.favoriteplayer.ui.adapter;

import java.util.ArrayList;

import idv.jason.lib.imagemanager.ImageAttribute;
import idv.jason.lib.imagemanager.ImageManager;

import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.entity.VideoData;
import com.waveface.favoriteplayer.event.VideoItemClickEvent;

import de.greenrobot.event.EventBus;

import android.content.Context;
import android.support.v4.view.PagerAdapter;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.ImageView.ScaleType;

public class VideoPagerAdapter extends PagerAdapter implements OnClickListener{
	private ImageManager mImageManager;
	private ArrayList<VideoData> mDatas = new ArrayList<VideoData>();
	private LayoutInflater mInflater;
	
	
	public VideoPagerAdapter(Context context, ArrayList<VideoData> datas) {
		mImageManager = SyncApplication.getWavefacePlayerApplication(context).getImageManager();		
		mInflater = LayoutInflater.from(context);
		mDatas = datas;
	}

	@Override
	public int getCount() {
		return mDatas.size();
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
		attr.setMaxSizeEqualsScreenSize(mInflater.getContext());
		attr.setDoneScaleType(ScaleType.FIT_CENTER);
		mImageManager.getImage(mDatas.get(position).url, attr);
		container.addView(root);
		iv.setTag(position);
		return root;
	}

	@Override
	public void onClick(View v) {
		int position = (Integer) v.getTag();

		VideoItemClickEvent event = new VideoItemClickEvent();
		event.urls =  mDatas;
		event.position = position;

		EventBus.getDefault().post(event);
	}
}
