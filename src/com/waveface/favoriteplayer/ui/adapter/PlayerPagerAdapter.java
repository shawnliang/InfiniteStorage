package com.waveface.favoriteplayer.ui.adapter;

import java.util.ArrayList;

import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.entity.OverviewData;

import idv.jason.lib.imagemanager.ImageAttribute;
import idv.jason.lib.imagemanager.ImageManager;

import android.content.Context;
import android.support.v4.view.PagerAdapter;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.ImageView.ScaleType;

public class PlayerPagerAdapter extends PagerAdapter {
	private LayoutInflater mInflater;
	private ImageManager mImageManager;

	private ArrayList<OverviewData> mDatas;

	public PlayerPagerAdapter(Context context, ArrayList<OverviewData> datas) {
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
		attr.setMaxSizeEqualsScreenSize(mInflater.getContext());
		attr.setDoneScaleType(ScaleType.FIT_CENTER);
		mImageManager.getImage(mDatas.get(position).url, attr);
		container.addView(root);
		return root;
	}

}
