package com.waveface.favoriteplayer.ui.adapter;

import idv.jason.lib.imagemanager.ImageAttribute;
import idv.jason.lib.imagemanager.ImageManager;

import java.util.ArrayList;

import android.content.Context;
import android.support.v4.view.PagerAdapter;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.ImageView.ScaleType;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.entity.PlaybackData;
import com.waveface.favoriteplayer.event.PlaybackItemClickEvent;

import de.greenrobot.event.EventBus;

public class PlayerPagerAdapter extends PagerAdapter {
	private LayoutInflater mInflater;
	private ImageManager mImageManager;

	private ArrayList<PlaybackData> mDatas;

	public PlayerPagerAdapter(Context context, ArrayList<PlaybackData> datas) {
		mDatas = datas;
		mInflater = LayoutInflater.from(context);
		mImageManager = SyncApplication.getWavefacePlayerApplication(context)
				.getImageManager();
	}

	@Override
	public int getCount() {
		if (mDatas == null)
			return 0;
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
		ImageAttribute attr = new ImageAttribute(iv);
		attr.setDoneScaleType(ScaleType.FIT_CENTER);
		attr.setApplyWithAnimation(true);
		
		root.setTag(Integer.valueOf(position));
		
		if(Constant.FILE_TYPE_VIDEO.equals(mDatas.get(position).type)) {
			mImageManager.getLocalVideoThumbnail(mDatas.get(position).url, attr);
			View placeholder = root.findViewById(R.id.image_play);
			placeholder.setVisibility(View.VISIBLE);
			placeholder.setTag(Integer.valueOf(position));
			placeholder.setOnClickListener(new View.OnClickListener() {
				@Override
				public void onClick(View v) {
					int position = (Integer) v.getTag();
					startVideoPlayback(position);
				}
			});
		} else {
			attr.setMaxSizeEqualsScreenSize(mInflater.getContext());
			mImageManager.getImage(mDatas.get(position).url, attr);			
		}
		container.addView(root);
		return root;
	}
	
	private void startVideoPlayback(int position) {
		PlaybackItemClickEvent event = new PlaybackItemClickEvent();
		event.position = position;
		EventBus.getDefault().post(event);
	}
}
