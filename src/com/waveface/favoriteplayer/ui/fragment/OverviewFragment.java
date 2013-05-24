package com.waveface.favoriteplayer.ui.fragment;

import java.util.ArrayList;

import android.database.Cursor;
import android.os.AsyncTask;
import android.os.Bundle;
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

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.db.LabelFileTable;
import com.waveface.favoriteplayer.entity.OverviewData;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.ui.adapter.OverviewAdapter;
import com.waveface.favoriteplayer.ui.widget.TwoWayView;

public class OverviewFragment extends Fragment implements OnItemClickListener, OnItemLongClickListener{
	public static final String TAG = OverviewFragment.class.getSimpleName();
	private TwoWayView mList;
	private int mType = OVERVIEW_VIEW_TYPE_FAVORITE;
	
	public static final int OVERVIEW_VIEW_TYPE_FAVORITE = 1;
	public static final int OVERVIEW_VIEW_TYPE_RECENT_PHOTO = 2;
	public static final int OVERVIEW_VIEW_TYPE_RECENT_VIDEO = 3;

	
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
			}
		}
		
		private void fillData(ArrayList<OverviewData> datas, int type) {
			Cursor c = LabelDB.getCategoryLabelByLabelId(getActivity(), type);
			if(c.moveToFirst()) {
				OverviewData data = new OverviewData();
				data.url = mServerUrl + c.getString(1);
				data.title = c.getString(2);
				datas.add(data);
			}
			c.close();
		}
	}

	@Override
	public void onItemClick(AdapterView<?> arg0, View arg1, int arg2, long arg3) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public boolean onItemLongClick(AdapterView<?> arg0, View arg1, int arg2,
			long arg3) {
		return true;
	}
}
