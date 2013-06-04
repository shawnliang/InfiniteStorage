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
import android.widget.ImageView.ScaleType;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.entity.PlaybackData;
import com.waveface.favoriteplayer.ui.widget.SquareImageView;
import com.waveface.favoriteplayer.util.FileUtil;
import com.waveface.favoriteplayer.util.ImageUtil;
import com.waveface.favoriteplayer.util.Log;

public class GalleryViewAdapter extends BaseAdapter{
	private static final String TAG = GalleryViewAdapter.class.getSimpleName();
	private LayoutInflater mInflater;
	private ArrayList<PlaybackData> mDatas;
	private ImageManager mImageManager;
	private int mImageSize;
	
	public GalleryViewAdapter(Context context, ArrayList<PlaybackData> datas) {
		mInflater = LayoutInflater.from(context);
		mDatas = datas;
		mImageManager = SyncApplication.getWavefacePlayerApplication(context).getImageManager();
		mImageSize = context.getResources().getDimensionPixelSize(R.dimen.galleryview_item_size);
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
//		attr.setLoadFromThread(true);
		attr.setDoneScaleType(ScaleType.CENTER_CROP);
		
		//Display Image
		if(Constant.FILE_TYPE_VIDEO.equals(mDatas.get(position).type)) {
			String fullFilename = mDatas.get(position).url;
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
			root.findViewById(R.id.image_play).setVisibility(View.VISIBLE);
		}
		mImageManager.getImage(mDatas.get(position).url, attr);
		return root;
	}

}
