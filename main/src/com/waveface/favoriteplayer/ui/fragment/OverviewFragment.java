package com.waveface.favoriteplayer.ui.fragment;

import java.util.ArrayList;

import android.content.Intent;
import android.database.Cursor;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Handler;
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
import android.widget.TextView;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
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
	private OverviewAdapter mAdapter;
	private TextView mNoContent;
	private ProgressBar mProgress;
	private int mType = OVERVIEW_VIEW_TYPE_FAVORITE;
	
	public static final int OVERVIEW_VIEW_TYPE_FAVORITE = 1;
	public static final int OVERVIEW_VIEW_TYPE_RECENT_PHOTO = 2;
	public static final int OVERVIEW_VIEW_TYPE_RECENT_VIDEO = 3;
	private String mServerUrl = null; 
	
	private ArrayList<String> mUpdateList = new ArrayList<String>();
	private ArrayList<String> mPendingList = new ArrayList<String>();
	
	private Handler mHandler = new Handler();
	
	class ReloadRunnable implements Runnable {
		private String labelId;

		public void setupLabelId(String labelId) {
			this.labelId = labelId;
		}
		
		@Override
		public void run() {
			if(labelId == null)
				return;
			View child = null;
			for(int i=0; i<mList.getChildCount(); ++i) {
				child = mList.getChildAt(i);
				ViewHolder holder = (ViewHolder) child.getTag();
				if(labelId.equals(holder.labelId)) {
					holder.progress.setVisibility(View.VISIBLE);
					holder.countText.setVisibility(View.INVISIBLE);
					break;
				} else {
					child = null;
				}
			}
			synchronized (mUpdateList) {
				if(mUpdateList.contains(labelId) == false) {
					Log.d(TAG, "start task for label:" + labelId);
					new ReloadLabelTask(labelId).execute(null, null, null);
					mUpdateList.add(labelId);
				} else {
					Log.d(TAG, "task pending for label:" + labelId);
					if(mPendingList .contains(labelId) == false) {
						Log.d(TAG, "pend");
						mPendingList.add(labelId);
					}
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
		if(mAdapter == null)
			return;
		boolean needProcess = false;
		
		switch(mType) {
		case OVERVIEW_VIEW_TYPE_FAVORITE:
			if(event.autoType == Constant.TYPE_FAVORITE) {
				needProcess = true;
			}
			break;
		case OVERVIEW_VIEW_TYPE_RECENT_PHOTO:
			switch(event.autoType) {
			case Constant.TYPE_RECENT_PHOTO_TODAY:
			case Constant.TYPE_RECENT_PHOTO_YESTERDAY:
			case Constant.TYPE_RECENT_PHOTO_THISWEEK:
				needProcess = true;
				break;
			}
			break;
		case OVERVIEW_VIEW_TYPE_RECENT_VIDEO:
			switch(event.autoType) {
			case Constant.TYPE_RECENT_VIDEO_TODAY:
			case Constant.TYPE_RECENT_VIDEO_YESTERDAY:
			case Constant.TYPE_RECENT_VIDEO_THISWEEK:
				needProcess = true;
				break;
			}
			break;
		}
		
		if(needProcess) {
			ReloadRunnable runnable = new ReloadRunnable();
			runnable.setupLabelId(event.labelId);
			mHandler.post(runnable);
		}
	}
	
	private class ReloadLabelTask extends AsyncTask<Void, Void, OverviewData> {
		private String mLabelId;
		
		public ReloadLabelTask(String labelId) {
			mLabelId = labelId;
		}

		@Override
		protected OverviewData doInBackground(Void... params) {
			OverviewData data = loadLabelData(mLabelId,FileUtil.getDownloadFolder(getActivity()));
			if(data != null) {
				Log.d(TAG, "updatea success mLabelId=" + mLabelId);
				if(mType != OVERVIEW_VIEW_TYPE_FAVORITE) {
					// already get title from loadLabelData
					data.title = getLableTitle(data.autoType);
				}
			} else {
				Log.e(TAG, "updatea fail mLabelId=" + mLabelId);
			}
			return data;
		}
		
		@Override
		protected void onPostExecute(OverviewData result) {
			synchronized (mUpdateList) {
				if (result != null) {
					mAdapter.updateLabel(result);
					mAdapter.notifyDataSetChanged();
					mList.invalidate();
				} else {
					// remove
					mAdapter.removeLable(mLabelId);
					mAdapter.notifyDataSetChanged();
				}
				
				if(mAdapter.getCount() == 0) {
					mNoContent.setVisibility(View.VISIBLE);
				} else {
					mNoContent.setVisibility(View.INVISIBLE);
				}
			
				Log.d(TAG, "reload task done");
				mUpdateList.remove(mLabelId);
				if(mPendingList.contains(mLabelId)) {
					Log.d(TAG, "start pending task for label:" + mLabelId);
					ReloadRunnable runnable = new ReloadRunnable();
					runnable.setupLabelId(mLabelId);
					mHandler.post(runnable);
					
					mPendingList.remove(mLabelId);
				}
			}
		}
		
	}
	
	private class PrepareViewTask extends AsyncTask<Void, Void, ArrayList<OverviewData>> {

		@Override
		protected ArrayList<OverviewData> doInBackground(Void... params) {
			if(getActivity() == null)
				return null;
			
			Log.d(TAG, "PrepareViewTask start");
			ArrayList<OverviewData> datas = new ArrayList<OverviewData>();
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
			mProgress.setVisibility(View.GONE);
			if(datas != null) {
				mAdapter = new OverviewAdapter(getActivity(), datas);
				mList.setAdapter(mAdapter);
				
				if(datas.size() == 0) {
					mNoContent.setVisibility(View.VISIBLE);
				}
			} else {
				Log.d(TAG, "no data");
			}
			Log.d(TAG, "PrepareViewTask done");
		}
		
		private void fillData(ArrayList<OverviewData> datas, int type) {
			Cursor c = LabelDB.getCategoryLabelByLabelId(getActivity(), type);
			for(int i=0; i<c.getCount(); ++i) {
				c.moveToPosition(i);
				String labelId = c.getString(0);
				OverviewData data = loadLabelData(labelId,FileUtil.getDownloadFolder(getActivity())); 
				
				if(data != null) {
					if(mType != OVERVIEW_VIEW_TYPE_FAVORITE) {
						// already get title from loadLabelData
						data.title = getLableTitle(type);
					}
					datas.add(data);
				}
			}
			c.close();
		}
	}
	
	
	private String getLableTitle(int type) {
		String title = null;
		
		switch(type) {
		case Constant.TYPE_RECENT_VIDEO_TODAY:
		case Constant.TYPE_RECENT_PHOTO_TODAY:
			title = getResources().getText(R.string.today).toString();
			break;
		case Constant.TYPE_RECENT_VIDEO_YESTERDAY:
		case Constant.TYPE_RECENT_PHOTO_YESTERDAY:
			title = getResources().getText(R.string.yesterday).toString();
			break;
		case Constant.TYPE_RECENT_VIDEO_THISWEEK:
		case Constant.TYPE_RECENT_PHOTO_THISWEEK:
			title = getResources().getText(R.string.thisweek).toString();
			break;
		}
		return title;
	}

	private OverviewData loadLabelData(String labelId,String realDownloadFolder) {
		Cursor labelCursor = LabelDB.getLabelByLabelId(getActivity(), labelId);		
		Cursor viewCursor = LabelDB.getLabelFileViewByLabelId(getActivity(), labelId);
		OverviewData data = null;
		if(labelCursor.moveToFirst() && viewCursor.moveToFirst()) {
			data = new OverviewData();
			data.labelId = labelId;
			data.url = mServerUrl + labelCursor.getString(2);
			data.count = viewCursor.getCount();
			data.autoType = labelCursor.getInt(3);
			data.fileType = viewCursor.getString(viewCursor.getColumnIndex(LabelFileView.COLUMN_TYPE));
			data.filename = viewCursor.getString(viewCursor.getColumnIndex(LabelFileView.COLUMN_FILE_NAME));	
		
			if(mType == OVERVIEW_VIEW_TYPE_FAVORITE) {
				data.title = labelCursor.getString(1);
			} else if(mType == OVERVIEW_VIEW_TYPE_RECENT_VIDEO) {
				String fileName =  realDownloadFolder + Constant.VIDEO_FOLDER+ "/"  +viewCursor
						.getString(viewCursor.getColumnIndex(LabelFileView.COLUMN_FILE_NAME));
				data.url = fileName;
			}
		}
		viewCursor.close();
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