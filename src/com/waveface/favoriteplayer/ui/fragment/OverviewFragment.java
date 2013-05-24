package com.waveface.favoriteplayer.ui.fragment;

import java.util.ArrayList;

import android.database.Cursor;
import android.os.AsyncTask;
import android.os.Bundle;
import android.support.v4.app.Fragment;
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
	
	private class PrepareViewTask extends AsyncTask<Void, Void, OverviewData[]> {

		@Override
		protected OverviewData[] doInBackground(Void... params) {

			if(getActivity() == null)
				return null;
			OverviewData datas[] = null;
			ArrayList<ServerEntity> servers = ServersLogic.getBackupedServers(getActivity());
			ServerEntity pairedServer = servers.get(0);
			String serverUrl ="http://"+pairedServer.ip+":"+pairedServer.restPort;
			
			switch(mType) {
			case OVERVIEW_VIEW_TYPE_FAVORITE:
				String labelId = null;
				Cursor c = LabelDB.getAllLabels(getActivity());
				if(c.getCount() > 0) {
					c.moveToFirst();
					labelId = c.getString(0);
				}
				c.close();
				c = LabelDB.getLabelFilesByLabelId(getActivity(), labelId);
				
				datas = new OverviewData[c.getCount()];
				for(int i=0; i<c.getCount(); ++i) {
					c.moveToPosition(i);
					OverviewData data = new OverviewData();
					data.url = serverUrl + Constant.URL_IMAGE + "/" + 
							c.getString(c.getColumnIndex(LabelFileTable.COLUMN_FILE_ID)) + 
							Constant.URL_IMAGE_MEDIUM;
					datas[i] = data;
				}
				c.close();
				
				break;
			}
			return datas;
		}
		
		@Override
		protected void onPostExecute(OverviewData[] datas) {
			if(datas != null) {
				OverviewAdapter adapter = new OverviewAdapter(getActivity(), datas);
				mList.setAdapter(adapter);
			}
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
