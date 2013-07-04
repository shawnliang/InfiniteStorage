package com.waveface.favoriteplayer.ui.adapter;

import idv.jason.lib.imagemanager.ImageAttribute;
import idv.jason.lib.imagemanager.ImageManager;

import java.util.ArrayList;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView.ScaleType;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.entity.PlaybackData;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.ui.widget.SquareImageView;
import com.waveface.favoriteplayer.util.Log;

public class GalleryViewAdapter extends BaseAdapter{
	private static final String TAG = GalleryViewAdapter.class.getSimpleName();
	private LayoutInflater mInflater;
	private ArrayList<PlaybackData> mDatas;
	private ImageManager mImageManager;
	private int mImageSize;
	private Context mContext;
	public GalleryViewAdapter(Context context, ArrayList<PlaybackData> datas) {
		mContext =context;
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
		attr.setApplyWithAnimation(true);
		attr.setLoadFromThread(true);
		attr.setDoneScaleType(ScaleType.CENTER_CROP);
		
		//Display Image
		if(Constant.FILE_TYPE_VIDEO.equals(mDatas.get(position).type)) {
			String fileId =mDatas.get(position).fileId;
			ArrayList<ServerEntity> servers = ServersLogic.getPairedServer(mContext);
			ServerEntity pairedServer = servers.get(0);
			String restfulAPIURL = "http://" + pairedServer.ip + ":"
					+ pairedServer.restPort;
			String url = restfulAPIURL + Constant.URL_IMAGE
					+ "/" + fileId + Constant.URL_IMAGE_MEDIUM;	
			mImageManager.getImage(url, attr);
			root.findViewById(R.id.image_play).setVisibility(View.VISIBLE);
		} else {
			Log.d(TAG, "get image:" + mDatas.get(position).url);
			attr.setMaxSize(mImageSize, mImageSize);
			mImageManager.getImage(mDatas.get(position).url, attr);
		}
		return root;
	}

}
