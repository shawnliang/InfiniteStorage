package com.waveface.favoriteplayer.ui.fragment;

import idv.jason.lib.imagemanager.ImageManager;

import java.util.ArrayList;

import android.content.Intent;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.media.ThumbnailUtils;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore.Video.Thumbnails;
import android.support.v4.app.Fragment;
import android.text.TextUtils;
import android.util.Log;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.AdapterView.OnItemLongClickListener;
import android.widget.TextView;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.db.LabelFileView;
import com.waveface.favoriteplayer.entity.OverviewData;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.ui.PlaybackActivity;
import com.waveface.favoriteplayer.ui.VideoActivity;
import com.waveface.favoriteplayer.ui.adapter.OverviewAdapter;
import com.waveface.favoriteplayer.ui.widget.TwoWayView;

public class OverviewFragment extends Fragment implements OnItemClickListener, OnItemLongClickListener{
	public static final String TAG = OverviewFragment.class.getSimpleName();
	private TwoWayView mList;
	private TextView mNoContent;
	private int mType = OVERVIEW_VIEW_TYPE_FAVORITE;
	
	public static final int OVERVIEW_VIEW_TYPE_FAVORITE = 1;
	public static final int OVERVIEW_VIEW_TYPE_RECENT_PHOTO = 2;
	public static final int OVERVIEW_VIEW_TYPE_RECENT_VIDEO = 3;

	
	private ImageManager mImageManager;
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		mImageManager = SyncApplication.getWavefacePlayerApplication(getActivity()).getImageManager();
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		View root = inflater.inflate(R.layout.fragment_overview, container, false);
		
		mList = (TwoWayView) root.findViewById(R.id.list);
		mList.setLongClickable(true);
		mList.setOnItemLongClickListener(this);
		mList.setOnItemClickListener(this);
		
		mList.setOnKeyListener(new View.OnKeyListener() {
			
			@Override
			public boolean onKey(View v, int keyCode, KeyEvent event) {
				// TODO: IMPLEMENT key event
				return false;
			}
		});
		
		Bundle data = null;
		if(savedInstanceState == null) {
			data = getArguments();
		} else {
			data = savedInstanceState;
		}
		
		if(data != null) {
			mType = data.getInt(Constant.ARGUMENT1);
		}
		
		new PrepareViewTask().execute(null, null, null);

		
		mNoContent = (TextView) root.findViewById(R.id.text_no_content);
		switch(mType) {
		case OVERVIEW_VIEW_TYPE_FAVORITE:
			mNoContent.setText(R.string.no_favorite);
			break;
		case OVERVIEW_VIEW_TYPE_RECENT_PHOTO:
			mNoContent.setText(R.string.no_photo);
			break;
		case OVERVIEW_VIEW_TYPE_RECENT_VIDEO:
			mNoContent.setText(R.string.no_video);
			break;
		}
		return root;
	}
	
	private class PrepareViewTask extends AsyncTask<Void, Void, ArrayList<OverviewData>> {
		private String mServerUrl = null; 

		@Override
		protected ArrayList<OverviewData> doInBackground(Void... params) {
			if(getActivity() == null)
				return null;
			ArrayList<OverviewData> datas = new ArrayList<OverviewData>();
			ArrayList<ServerEntity> servers = ServersLogic.getPairedServer(getActivity());
			ServerEntity pairedServer = servers.get(0);
			if(TextUtils.isEmpty(pairedServer.restPort)){
				pairedServer.restPort ="14005";
			}
			mServerUrl ="http://"+pairedServer.ip+":"+pairedServer.restPort;
			Log.d(TAG, "mServerUrl:" +mServerUrl);
			switch(mType) {
			case OVERVIEW_VIEW_TYPE_FAVORITE:
				fillData(datas, Constant.TYPE_FAVORITE);
				break;
				
			case OVERVIEW_VIEW_TYPE_RECENT_PHOTO:
				fillData(datas, Constant.TYPE_RECENT_PHOTO_TODAY);
				fillData(datas, Constant.TYPE_RECENT_PHOTO_YESTERDAY);
				fillData(datas, Constant.TYPE_RECENT_PHOTO_THISWEEK);
				break;
				
			case OVERVIEW_VIEW_TYPE_RECENT_VIDEO:
				fillData(datas, Constant.TYPE_RECENT_VIDEO_TODAY);
				fillData(datas, Constant.TYPE_RECENT_VIDEO_YESTERDAY);
				fillData(datas, Constant.TYPE_RECENT_VIDEO_THISWEEK);
				break;
			}
			return datas;
		}
		
		@Override
		protected void onPostExecute(ArrayList<OverviewData> datas) {
			if(datas != null) {
				OverviewAdapter adapter = new OverviewAdapter(getActivity(), datas);
				mList.setAdapter(adapter);
				
				if(datas.size() == 0) {
					mNoContent.setVisibility(View.VISIBLE);
				}
			}
		}
		
		private void fillData(ArrayList<OverviewData> datas, int type) {
			Cursor c = LabelDB.getCategoryLabelByLabelId(getActivity(), type);
			for(int i=0; i<c.getCount(); ++i) {
				c.moveToPosition(i);
				Cursor fc = LabelDB.getLabelFilesByLabelId(getActivity(), c.getString(0));
				if(fc.getCount() > 0) {
					OverviewData data = new OverviewData();
					data.labelId = c.getString(0);
					data.url = mServerUrl + c.getString(1);

					switch(mType) {
					case OVERVIEW_VIEW_TYPE_FAVORITE:
						data.title = c.getString(2);
						break;
					case OVERVIEW_VIEW_TYPE_RECENT_PHOTO:
						switch(type) {
						case Constant.TYPE_RECENT_PHOTO_TODAY:
							data.title = getResources().getText(R.string.today).toString();
							break;
						case Constant.TYPE_RECENT_PHOTO_YESTERDAY:
							data.title = getResources().getText(R.string.yesterday).toString();
							break;
						case Constant.TYPE_RECENT_PHOTO_THISWEEK:
							data.title = getResources().getText(R.string.thisweek).toString();
							break;
						}
						break;
					case OVERVIEW_VIEW_TYPE_RECENT_VIDEO:
						Cursor lfc = LabelDB.getLabelFileViewByLabelId(getActivity(), data.labelId);
						if(lfc.moveToFirst()) {
							String fileName =  Environment.getExternalStorageDirectory().getAbsolutePath()
									+ Constant.VIDEO_FOLDER+ "/"  +lfc
									.getString(lfc
											.getColumnIndex(LabelFileView.COLUMN_FILE_NAME));
							Bitmap bmThumbnail = ThumbnailUtils.createVideoThumbnail(fileName, 
							        Thumbnails.MINI_KIND);

							mImageManager.setBitmapToFile(bmThumbnail, fileName, null, false);
							data.url = fileName;
						}
						lfc.close();
						
						switch(type) {
						case Constant.TYPE_RECENT_VIDEO_TODAY:
							data.title = getResources().getText(R.string.today).toString();
							break;
						case Constant.TYPE_RECENT_VIDEO_YESTERDAY:
							data.title = getResources().getText(R.string.yesterday).toString();
							break;
						case Constant.TYPE_RECENT_VIDEO_THISWEEK:
							data.title = getResources().getText(R.string.thisweek).toString();
							break;
						}
						break;
					}
					
					datas.add(data);
				}
				fc.close();
			}
			c.close();
		}
	}

	@Override
	public void onItemClick(AdapterView<?> listview, View view, int position, long id) {
		Intent intent = null;
		Bundle data = new Bundle();
		switch(mType) {
		case OverviewFragment.OVERVIEW_VIEW_TYPE_FAVORITE:
		case OverviewFragment.OVERVIEW_VIEW_TYPE_RECENT_PHOTO:
			intent = new Intent(getActivity(), PlaybackActivity.class);
			data.putString(Constant.ARGUMENT1, ((OverviewAdapter)listview.getAdapter()).getDatas().get(position).labelId);
			intent.putExtras(data);
			startActivity(intent);
			break;
		case OverviewFragment.OVERVIEW_VIEW_TYPE_RECENT_VIDEO:
			intent = new Intent(getActivity(), VideoActivity.class);
			data.putString(Constant.ARGUMENT1, ((OverviewAdapter)listview.getAdapter()).getDatas().get(position).labelId);
			intent.putExtras(data);
			startActivity(intent);
			break;
		}
	}

	@Override
	public boolean onItemLongClick(AdapterView<?> arg0, View arg1, int arg2,
			long arg3) {
		return true;
	}
}
