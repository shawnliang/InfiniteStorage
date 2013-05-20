package com.waveface.favoriteplayer.ui.adapter;

import java.util.ArrayList;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.db.LabelFileTable;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.logic.ServersLogic;

import idv.jason.lib.imagemanager.ImageAttribute;
import idv.jason.lib.imagemanager.ImageManager;

import android.content.Context;
import android.database.Cursor;
import android.support.v4.view.PagerAdapter;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.ImageView.ScaleType;

public class PlayerPagerAdapter extends PagerAdapter{
	private Cursor mCursor;
	private LayoutInflater mInflater;
	private ImageManager mImageManager;
	private String mServerUrl;
	
	public PlayerPagerAdapter(Context context, String labelId) {
		mCursor = LabelDB.getLabelFilesByLabelId(context, labelId);
		mInflater = LayoutInflater.from(context);
		mImageManager = SyncApplication.getWavefacePlayerApplication(context).getImageManager();
		
		ArrayList<ServerEntity> servers = ServersLogic.getBackupedServers(context);
		ServerEntity pairedServer = servers.get(0);
		mServerUrl ="http://"+pairedServer.ip+":"+pairedServer.restPort;
	}
	
	public void releaseResources() {
		mCursor.close();
		mCursor = null;
	}

	@Override
	public int getCount() {
		if(mCursor != null && mCursor.getCount() != 0) {
			return mCursor.getCount();
		}
		return 0;
	}

	@Override
	public boolean isViewFromObject(View view, Object object) {
		return view == ((View) object);
	}
	
	@Override
	public void destroyItem(ViewGroup container, int position, Object object) {
		container.removeView((View)object);
	}
	
	@Override
	public Object instantiateItem(ViewGroup container, int position) {
		View root = mInflater.inflate(R.layout.item_player_pager, container, false);
		if(mCursor.moveToPosition(position)) {
			ImageView iv = (ImageView) root.findViewById(R.id.image);
			ImageAttribute attr = new ImageAttribute(iv);
			attr.setMaxSizeEqualsScreenSize(mInflater.getContext());
			attr.setDoneScaleType(ScaleType.FIT_CENTER);
			mImageManager.getImage(
					mServerUrl + Constant.URL_IMAGE + "/" + 
					mCursor.getString(mCursor.getColumnIndex(LabelFileTable.COLUMN_FILE_ID)) + 
					Constant.URL_IMAGE_LARGE, attr);
			container.addView(root);
		}
		return root;
	}

}
