package com.waveface.favoriteplayer.ui.adapter;

import idv.jason.lib.imagemanager.ImageAttribute;
import idv.jason.lib.imagemanager.ImageManager;

import java.util.ArrayList;

import android.content.Context;
import android.graphics.Bitmap;
import android.media.ThumbnailUtils;
import android.provider.MediaStore.Video.Thumbnails;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.ImageView.ScaleType;
import android.widget.ProgressBar;
import android.widget.TextView;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.entity.OverviewData;
import com.waveface.favoriteplayer.util.FileUtil;
import com.waveface.favoriteplayer.util.Log;

public class OverviewAdapter extends BaseAdapter{
	private static final String TAG = OverviewAdapter.class.getSimpleName();
	private ArrayList<OverviewData> mDatas;
	private ImageManager mImageManager;
	private LayoutInflater mInflater;
	
	private int mChildWidth, mChildHeight;
	private String mFilePath;
	
	private int mNormalPadding, mFirstPadding;
	
	public class ViewHolder {
		public ImageView image;
		public ImageView reflection;
		public TextView labelText;
		public ImageView placeholder;
		public TextView countText;
		public ProgressBar progress;
		public String labelId;
	}
	
	public OverviewAdapter(Context context, ArrayList<OverviewData> datas) {
		if(context == null)
			return;
		mDatas = datas;
		mImageManager = SyncApplication.getWavefacePlayerApplication(context).getImageManager();
		mInflater = LayoutInflater.from(context);
		

		mChildWidth = context.getResources().getDimensionPixelSize(R.dimen.overview_image_width) + 200;
		mChildHeight = context.getResources().getDimensionPixelSize(R.dimen.overview_image_height) + 200;
		
		mFilePath = FileUtil.getDownloadFolder(context) + Constant.VIDEO_FOLDER + "/";

		mNormalPadding = context.getResources().getDimensionPixelSize(R.dimen.overview_item_padding);
		mFirstPadding = context.getResources().getDimensionPixelSize(R.dimen.overview_item_first_left_padding);
	}
	
	public ArrayList<OverviewData> getDatas() {
		return mDatas;
	}

	@Override
	public int getCount() {
		return mDatas==null?0:mDatas.size();
	}

	@Override
	public Object getItem(int position) {
		return mDatas.get(position);
	}

	@Override
	public long getItemId(int position) {
		return position;
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent) {
		if(convertView == null) {
			convertView = getView(parent, false);
		}
		
		setupView(position, convertView, mDatas.get(position));
		
		return convertView;
	}
	
	public View generateView(int position, ViewGroup parent) {
		View view = getView(parent, true);
		
		setupView(position, view, mDatas.get(position));
		
		return view;
	}

	private View getView(ViewGroup parent, boolean isFake) {
		View convertView = mInflater.inflate(R.layout.item_overview, parent, false);
		ViewHolder holder = new ViewHolder();
		holder.image = (ImageView) convertView.findViewById(R.id.image);
		holder.reflection = (ImageView) convertView.findViewById(R.id.reflection_image);
		holder.labelText = (TextView) convertView.findViewById(R.id.text_label);
		holder.placeholder = (ImageView) convertView.findViewById(R.id.image_play);
		holder.countText = (TextView) convertView.findViewById(R.id.text_count);
		holder.progress = (ProgressBar)convertView.findViewById(R.id.progress);
		convertView.setTag(holder);
		return convertView;
	}
	
	private void setupView(int position, View view, OverviewData data) {
		ViewHolder holder = (ViewHolder) view.getTag();
		holder.labelId = data.labelId;
		

		String coverUrl = null;
		
		ImageAttribute attr = new ImageAttribute(holder.image);
		attr = new ImageAttribute(holder.image);
		attr.setResizeSize(mChildWidth, mChildHeight);
		attr.setApplyWithAnimation(true);
		attr.setDoneScaleType(ScaleType.CENTER_CROP);
		
		if(Constant.FILE_TYPE_VIDEO.equals(data.type)) {
			
			String fullFilename = mFilePath + data.filename;
			Bitmap bmThumbnail = mImageManager.getImage(fullFilename, attr);
			if(bmThumbnail==null){
				if(FileUtil.isFileExisted(fullFilename)){
					bmThumbnail = ThumbnailUtils.createVideoThumbnail(fullFilename, 
					        Thumbnails.MINI_KIND);
					String dbId = mImageManager.setBitmapToFile(bmThumbnail, 
							fullFilename, null, false);
					Log.d(TAG, "ThumbNail DB ID:"+dbId);
				}
				else{
					
				}
			}
			holder.placeholder.setVisibility(View.VISIBLE);
			coverUrl = fullFilename;
		}
		else{
			holder.placeholder.setVisibility(View.INVISIBLE);
			coverUrl = data.url;
		}
		mImageManager.getImage(coverUrl, attr);			

		attr = new ImageAttribute(holder.reflection);
		attr.setResizeSize(mChildWidth, mChildHeight);
		attr.setReflection(true);
		attr.setHighQuality(true);
		attr.setApplyWithAnimation(true);
		attr.setDoneScaleType(ScaleType.CENTER_CROP);
		mImageManager.getImage(coverUrl, attr);

		holder.labelText.setText(data.title);
		holder.countText.setText(Integer.toString(data.count));

		if(position == 0) {
			view.setPadding(mFirstPadding, mNormalPadding, mNormalPadding, mNormalPadding);
		} else {
			view.setPadding(mNormalPadding, mNormalPadding, mNormalPadding, mNormalPadding);
		}
	}
}
