package com.waveface.favoriteplayer.ui.adapter;

import idv.jason.lib.imagemanager.ImageAttribute;
import idv.jason.lib.imagemanager.ImageManager;

import java.util.ArrayList;

import android.content.Context;
import android.graphics.Bitmap;
import android.media.ThumbnailUtils;
import android.os.Environment;
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
	
	private boolean mVideo;
	
	public class ViewHolder {
		public ImageView image;
		public ImageView reflection;
		public TextView labelText;
		public ImageView placeholder;
		public TextView countText;
		public ProgressBar progress;
		public String labelId;
	}
	
	public OverviewAdapter(Context context, ArrayList<OverviewData> datas, boolean isVideo) {
		if(context == null)
			return;
		mDatas = datas;
		mImageManager = SyncApplication.getWavefacePlayerApplication(context).getImageManager();
		mInflater = LayoutInflater.from(context);
		
		mVideo = isVideo;
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
		ViewHolder holder;
		Context context = mInflater.getContext();
		if(convertView == null) {
			convertView = mInflater.inflate(R.layout.item_overview, parent, false);
			holder = new ViewHolder();
			holder.image = (ImageView) convertView.findViewById(R.id.image);
			holder.reflection = (ImageView) convertView.findViewById(R.id.reflection_image);
			holder.labelText = (TextView) convertView.findViewById(R.id.text_label);
			holder.placeholder = (ImageView) convertView.findViewById(R.id.image_play);
			holder.countText = (TextView) convertView.findViewById(R.id.text_count);
			holder.progress = (ProgressBar)convertView.findViewById(R.id.progress);
			convertView.setTag(holder);
		} else {
			holder = (ViewHolder) convertView.getTag();
		}
		
		holder.labelId = mDatas.get(position).labelId;
		
		int width = context.getResources().getDimensionPixelSize(R.dimen.overview_image_width) + 200;
		int height = context.getResources().getDimensionPixelSize(R.dimen.overview_image_height) + 200;

		ImageAttribute attr = new ImageAttribute(holder.image);
		attr = new ImageAttribute(holder.image);
		attr.setResizeSize(width, height);
		attr.setApplyWithAnimation(true);
		attr.setDoneScaleType(ScaleType.CENTER_CROP);
		
		if(Constant.FILE_TYPE_VIDEO.equals(mDatas.get(position).type)) {
			
			String fullFilename = FileUtil.getDownloadFolder(context)
					+ Constant.VIDEO_FOLDER + "/" + mDatas.get(position).filename;
			Bitmap bmThumbnail = mImageManager.getImage(fullFilename, attr);
			if(bmThumbnail==null){
				if(FileUtil.isFileExisted(fullFilename)){
					bmThumbnail = ThumbnailUtils.createVideoThumbnail(fullFilename, 
					        Thumbnails.MINI_KIND);
					String dbId = mImageManager.setBitmapToFile(bmThumbnail, 
							fullFilename, null, false);
					Log.d(TAG, "ThumbNail DB ID:"+dbId);
				}
			}
			holder.placeholder.setVisibility(View.VISIBLE);
			mImageManager.getImage(fullFilename, attr);						
		}
		else{
			holder.placeholder.setVisibility(View.INVISIBLE);
			mImageManager.getImage(mDatas.get(position).url, attr);			
		}
		attr = new ImageAttribute(holder.reflection);
		attr.setResizeSize(width, height);
		attr.setReflection(true);
		attr.setHighQuality(true);
		attr.setApplyWithAnimation(true);
		attr.setDoneScaleType(ScaleType.CENTER_CROP);
		mImageManager.getImage(mDatas.get(position).url, attr);

		
		holder.labelText.setText(mDatas.get(position).title);

		int paddingNormal = context.getResources().getDimensionPixelSize(R.dimen.overview_item_padding);
		if(position == 0) {
			int paddingLeft = context.getResources().getDimensionPixelSize(R.dimen.overview_item_first_left_padding);
			convertView.setPadding(paddingLeft, paddingNormal, paddingNormal, paddingNormal);
		} else {
			convertView.setPadding(paddingNormal, paddingNormal, paddingNormal, paddingNormal);
		}
				
		holder.countText.setText(Integer.toString(mDatas.get(position).count));
		
		return convertView;
	}

}
