package com.waveface.favoriteplayer.ui.adapter;

import idv.jason.lib.imagemanager.ImageAttribute;
import idv.jason.lib.imagemanager.ImageManager;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView;

import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.entity.OverviewData;

public class OverviewAdapter extends BaseAdapter{
	private OverviewData mDatas[];
	private ImageManager mImageManager;
	private LayoutInflater mInflater;
	
	private class ViewHolder {
		public ImageView image;
		public ImageView reflection;
	}
	
	public OverviewAdapter(Context context, OverviewData[] datas) {
		if(context == null)
			return;
		mDatas = datas;
		mImageManager = SyncApplication.getWavefacePlayerApplication(context).getImageManager();
		mInflater = LayoutInflater.from(context);
	}

	@Override
	public int getCount() {
		return mDatas==null?0:mDatas.length;
	}

	@Override
	public Object getItem(int position) {
		return mDatas[position];
	}

	@Override
	public long getItemId(int position) {
		return position;
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent) {
		ViewHolder holder;
		Context context = mInflater.getContext();
		if(convertView == null) {
			convertView = mInflater.inflate(R.layout.item_overview, parent, false);
			holder = new ViewHolder();
			holder.image = (ImageView) convertView.findViewById(R.id.image);
			holder.reflection = (ImageView) convertView.findViewById(R.id.reflection_image);
			convertView.setTag(holder);
		} else {
			holder = (ViewHolder) convertView.getTag();
		}
		
		int width = context.getResources().getDimensionPixelSize(R.dimen.overview_image_width);
		int height = context.getResources().getDimensionPixelSize(R.dimen.overview_image_height);
		
		ImageAttribute attr = new ImageAttribute(holder.image);
		attr.setResizeSize(width, height);
		mImageManager.getImage(mDatas[position].url, attr);
		
		attr = new ImageAttribute(holder.reflection);
		attr.setResizeSize(width, height);
		attr.setReflection(true);
		mImageManager.getImage(mDatas[position].url, attr);
		
		
		
		return convertView;
	}

}
