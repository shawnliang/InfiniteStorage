package com.waveface.favoriteplayer.ui.fragment;

import idv.jason.lib.imagemanager.ImageAttribute;
import idv.jason.lib.imagemanager.ImageManager;

import java.lang.ref.WeakReference;
import java.util.ArrayList;

import android.content.Intent;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.media.ThumbnailUtils;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Environment;
import android.os.Handler;
import android.provider.MediaStore.Video.Thumbnails;
import android.support.v4.app.Fragment;
import android.text.TextUtils;
import android.util.Log;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ProgressBar;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.AdapterView.OnItemLongClickListener;
import android.widget.ImageView.ScaleType;
import android.widget.TextView;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.SyncApplication;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.db.LabelFileView;
import com.waveface.favoriteplayer.entity.OverviewData;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.event.LabelChangeEvent;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.ui.PlaybackActivity;
import com.waveface.favoriteplayer.ui.adapter.OverviewAdapter;
import com.waveface.favoriteplayer.ui.adapter.OverviewAdapter.ViewHolder;
import com.waveface.favoriteplayer.ui.widget.TwoWayView;
import com.waveface.favoriteplayer.util.FileUtil;

import de.greenrobot.event.EventBus;

public class OverviewFragment extends Fragment implements OnItemClickListener, OnItemLongClickListener{
	public static final String TAG = OverviewFragment.class.getSimpleName();
	private TwoWayView mList;
	private TextView mNoContent;
	private ProgressBar mProgress;
	private ImageManager mImageManager;
	private int mType = OVERVIEW_VIEW_TYPE_FAVORITE;
	
	public static final int OVERVIEW_VIEW_TYPE_FAVORITE = 1;
	public static final int OVERVIEW_VIEW_TYPE_RECENT_PHOTO = 2;
	public static final int OVERVIEW_VIEW_TYPE_RECENT_VIDEO = 3;
	private String mServerUrl = null; 
	
	private Handler mHandler = new Handler();
	
	class ReloadRunnable implements Runnable {
		private String labelId;

		public void setupLabelId(String labelId) {
			this.labelId = labelId;
		}
		
		@Override
		public void run() {
			for(int i=0; i<mList.getChildCount(); ++i) {
				View child = mList.getChildAt(i);
				if(labelId == null)
					continue;
				ViewHolder holder = (ViewHolder) child.getTag();
				if(labelId.equals(holder.labelId)) {
					holder.progress.setVisibility(View.VISIBLE);
					new ReloadLabelTask(child, holder.labelId).execute(null, null, null);
				}
			}
		}
		
	}
 	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		
		ArrayList<ServerEntity> servers = ServersLogic.getPairedServer(getActivity());
		ServerEntity pairedServer = servers.get(0);
		if(TextUtils.isEmpty(pairedServer.restPort)){
			pairedServer.restPort ="14005";
		}
		mServerUrl ="http://"+pairedServer.ip+":"+pairedServer.restPort;
		mImageManager = SyncApplication.getWavefacePlayerApplication(getActivity()).getImageManager();
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		View root = inflater.inflate(R.layout.fragment_overview, container, false);
		
		mProgress = (ProgressBar) root.findViewById(R.id.progress);
		
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
	
	@Override
	public void onPause() {
		super.onPause();
		EventBus.getDefault().unregister(this);
	}
	
	@Override
	public void onResume() {
		super.onResume();
		EventBus.getDefault().register(this);
	}
	
	public void onEvent(LabelChangeEvent event) {
		ReloadRunnable runnable = new ReloadRunnable();
		runnable.setupLabelId(event.labelId);
		mHandler.post(runnable);
	}
	
	private class ReloadLabelTask extends AsyncTask<Void, Void, OverviewData> {
		private WeakReference<View> mView;
		private String mLabelId;
		
		public ReloadLabelTask(View view, String labelId) {
			mView = new WeakReference<View>(view);
			mLabelId = labelId;
		}

		@Override
		protected OverviewData doInBackground(Void... params) {
			return loadLabelData(mLabelId,FileUtil.getDownloadFolder(getActivity()));
		}
		
		@Override
		protected void onPostExecute(OverviewData result) {
			View view = mView.get();
			if(view != null) {
				ViewHolder holder = (ViewHolder) view.getTag();
				holder.progress.setVisibility(View.GONE);
				holder.countText.setText(result==null?"":Integer.toString
						(result.count));
				
				if(result!=null){
					int width = getActivity().getResources().getDimensionPixelSize(R.dimen.overview_image_width) + 200;
					int height = getActivity().getResources().getDimensionPixelSize(R.dimen.overview_image_height) + 200;
	
					ImageAttribute attr = new ImageAttribute(holder.image);
					attr = new ImageAttribute(holder.image);
					attr.setResizeSize(width, height);
					attr.setApplyWithAnimation(true);
					attr.setDoneScaleType(ScaleType.CENTER_CROP);
					
					if(Constant.FILE_TYPE_VIDEO.equals(result.type)) {
						String fullFilename = Environment.getExternalStorageDirectory().getAbsolutePath()
								+ Constant.VIDEO_FOLDER + "/" + result.filename;

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
						mImageManager.getImage(result.url, attr);			
					}
					
					attr = new ImageAttribute(holder.reflection);
					attr.setResizeSize(width, height);
					attr.setReflection(true);
					attr.setHighQuality(true);
					attr.setApplyWithAnimation(true);
					attr.setDoneScaleType(ScaleType.CENTER_CROP);
					mImageManager.getImage(result.url, attr);	
				}
			}
		}
		
	}
	
	private class PrepareViewTask extends AsyncTask<Void, Void, ArrayList<OverviewData>> {

		@Override
		protected ArrayList<OverviewData> doInBackground(Void... params) {
			if(getActivity() == null)
				return null;
			

			ArrayList<OverviewData> datas = new ArrayList<OverviewData>();
			
			Log.d(TAG, "PrepareViewTask: type=" + mType);
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
				mProgress.setVisibility(View.GONE);
				OverviewAdapter adapter = null;
				if(mType == OVERVIEW_VIEW_TYPE_RECENT_VIDEO)
					adapter = new OverviewAdapter(getActivity(), datas, true);
				else
					adapter = new OverviewAdapter(getActivity(), datas, false);
				mList.setAdapter(adapter);
				
				if(datas.size() == 0) {
					mNoContent.setVisibility(View.VISIBLE);
				}
				

				ReloadRunnable runnable = new ReloadRunnable();
//				runnable.setupLabelId(event.labelId);
				mHandler.postDelayed(runnable, 3000);
			}
		}
		
		private void fillData(ArrayList<OverviewData> datas, int type) {
			Cursor c = LabelDB.getCategoryLabelByLabelId(getActivity(), type);
			Log.d(TAG, "There are " + c.getCount() + " labels");
			for(int i=0; i<c.getCount(); ++i) {
				c.moveToPosition(i);
				String labelId = c.getString(0);
				OverviewData data = loadLabelData(labelId,FileUtil.getDownloadFolder(getActivity())); 
				
				if(data != null) {
					if(mType == OVERVIEW_VIEW_TYPE_RECENT_PHOTO) {
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
					} else if(mType == OVERVIEW_VIEW_TYPE_RECENT_VIDEO) {
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
					}
						
					datas.add(data);
				}
			}
			c.close();
		}
	}

	private OverviewData loadLabelData(String labelId,String realDownloadFolder) {
		Cursor labelCursor = LabelDB.getLabelByLabelId(getActivity(), labelId);
		Cursor fileCursor = LabelDB.getLabelFileViewByLabelId(getActivity(), labelId);
		Log.d(TAG, "There are " + fileCursor.getCount() + " files");
		OverviewData data = null;
		if(labelCursor.moveToFirst() && fileCursor.getCount() > 0) {
			data = new OverviewData();
			data.labelId = labelId;
			data.url = mServerUrl + labelCursor.getString(2);
			data.count = fileCursor.getCount();
			fileCursor.moveToFirst();
			data.type = fileCursor.getString(fileCursor.getColumnIndex(LabelFileView.COLUMN_TYPE));
			data.filename = fileCursor.getString(fileCursor.getColumnIndex(LabelFileView.COLUMN_FILE_NAME));
			if(mType == OVERVIEW_VIEW_TYPE_FAVORITE) {
				data.title = labelCursor.getString(1);
			} else if(mType == OVERVIEW_VIEW_TYPE_RECENT_VIDEO) {
//				if(fileCursor.moveToFirst()) {
					String fileName =  realDownloadFolder + Constant.VIDEO_FOLDER+ "/"  +fileCursor
							.getString(fileCursor.getColumnIndex(LabelFileView.COLUMN_FILE_NAME));
					data.url = fileName;
//				}
			}
		}
		fileCursor.close();
		labelCursor.close();
		return data;
	}

	@Override
	public void onItemClick(AdapterView<?> listview, View view, int position, long id) {
		Intent intent = null;
		Bundle data = new Bundle();
		switch(mType) {
		case OverviewFragment.OVERVIEW_VIEW_TYPE_FAVORITE:
		case OverviewFragment.OVERVIEW_VIEW_TYPE_RECENT_PHOTO:
		case OverviewFragment.OVERVIEW_VIEW_TYPE_RECENT_VIDEO:
			intent = new Intent(getActivity(), PlaybackActivity.class);
			data.putString(Constant.ARGUMENT1, ((OverviewAdapter)listview.getAdapter()).getDatas().get(position).labelId);
			data.putString(Constant.ARGUMENT3, ((OverviewAdapter)listview.getAdapter()).getDatas().get(position).title);
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
