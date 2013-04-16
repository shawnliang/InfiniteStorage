package com.waveface.sync.ui;

import java.util.ArrayList;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.content.pm.ActivityInfo;
import android.os.Bundle;
import android.os.Handler;
import android.text.TextUtils;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.CompoundButton.OnCheckedChangeListener;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.RuntimeConfig;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.logic.FileBackup;
import com.waveface.sync.logic.ServersLogic;


public class WSServerFragment extends LinkFragmentBase implements OnClickListener, OnCheckedChangeListener{
	public final String TAG = WSServerFragment.class.getSimpleName();

	private ViewGroup mRootView;
	private ServerArrayAdapter mAdapter ; 
    private Handler mHandler = new Handler();

	//DATA
	private ServerEntity mServer ;
    private RuntimeConfig mRuntime = RuntimeConfig.getInstance();

	public int getHeight() {
		return mRootView.getMeasuredHeight();
	}

	@Override
	public void onBackPressed() {
		mListener.goBack(TAG);
	}

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {

		mRootView = (ViewGroup) inflater.inflate(
				R.layout.bonjour_servers, null);
		ListView listview = (ListView) mRootView.findViewById(R.id.listview);
		mAdapter = new ServerArrayAdapter(getActivity(), ServersLogic.getBonjourServers(getActivity()));
		listview.setAdapter(mAdapter);
		listview.setOnItemClickListener(new AdapterView.OnItemClickListener() {

		      @Override
		      public void onItemClick(AdapterView<?> parent, final View view,
		          int position, long id) {
		        Log.d(TAG, "position:"+position);
		        mServer = mAdapter.getItem(position);
		        clickToLinkServer(mServer);
		      }
		});

		Button btn = (Button) mRootView.findViewById(R.id.btnPre);
		btn.setOnClickListener(this);

		btn = (Button) mRootView.findViewById(R.id.btnNext);
		btn.setOnClickListener(this);
        mHandler.postDelayed(new Runnable() {
            public void run() {
             	 mAdapter.setData(ServersLogic.getBonjourServers(getActivity()));
            	}
            }, 300);
		return mRootView;
	}

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		IntentFilter filter = new IntentFilter();
		filter.addAction(Constant.ACTION_BONJOUR_MULTICAT_EVENT);
		filter.addAction(Constant.ACTION_WS_SERVER_NOTIFY);		
        getActivity().registerReceiver(mReceiver, filter);

		if (Constant.PHONE) {
			getActivity().setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);
		}
	}
	private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			String action = intent.getAction();
			Log.d(TAG, "action:" + intent.getAction());
			if (Constant.ACTION_BONJOUR_MULTICAT_EVENT.equals(action)) {		
//				ServerEntity entity = (ServerEntity) intent.getExtras().get(Constant.EXTRA_SERVER_DATA);
//				mAdapter.addData(entity);
				mAdapter.setData(ServersLogic.getBonjourServers(getActivity()));
			}
			else if(Constant.ACTION_WS_SERVER_NOTIFY.equals(action)){
				String response = intent.getStringExtra(Constant.EXTRA_SERVER_NOTIFY_CONTENT);
				if(response.equals(Constant.WS_ACTION_ACCEPT)){
					ServerEntity entity = (ServerEntity) intent.getExtras().get(Constant.EXTRA_SERVER_DATA);
					entity.serverId = mServer.serverId;
					entity.serverName = mServer.serverName;
					entity.serverOS = mServer.serverOS;
					entity.wsLocation = mServer.wsLocation;
					mServer = entity;
			    	SharedPreferences prefs = getActivity().getSharedPreferences(Constant.PREFS_NAME, Context.MODE_PRIVATE);
			    	Editor editor = prefs.edit();
			    	editor.putString(Constant.PREF_STATION_WEB_SOCKET_URL, mServer.wsLocation);
			    	editor.putString(Constant.PREF_SERVER_ID,mServer.serverId);
			    	editor.commit();
					ServersLogic.startBackuping(context, mServer);
					Toast.makeText(context, "CONNECT TO "+entity.serverName, Toast.LENGTH_LONG).show();
				}
				else if(response.equals(Constant.WS_ACTION_DENIED)){
					Toast.makeText(context, "DENIED BY SERVER!", Toast.LENGTH_LONG).show();
				}				
				else if(response.equals(Constant.WS_ACTION_WAIT_FOR_PAIR)){
					Toast.makeText(context, "WAITING FOR PAIR......", Toast.LENGTH_LONG).show();
				}				
			}
		}
	};

	@Override
	public void onDestroy() {
		getActivity().unregisterReceiver(mReceiver);
		super.onDestroy();
	}
    private void clickToLinkServer(ServerEntity entity){
    	ServersLogic.startWSServerConnect(getActivity(), entity.wsLocation,entity.serverId);
    }
	@Override
	public void onClick(View v) {
		int viewId = v.getId();
		switch (viewId) {
			case R.id.btnPre:
				mListener.goBack(TAG);
				break;
			case R.id.btnNext:
				mListener.goNext(TAG);
				break;
		}
	}
	class ServerArrayAdapter extends BaseAdapter {
		  private final Context context;
		  private ArrayList<ServerEntity> values;

		  public ServerArrayAdapter(Context context, ArrayList<ServerEntity> datas) {
		    super();
		    this.context = context;
		    this.values = datas;
		  }

		  @Override
		  public View getView(int position, View convertView, ViewGroup parent) {
			  ServerEntity entity = values.get(position);

			  LayoutInflater inflater = (LayoutInflater) context
		        .getSystemService(Context.LAYOUT_INFLATER_SERVICE);
		    View rowView = inflater.inflate(R.layout.choose_server_row, parent, false);
		    TextView tv = (TextView) rowView.findViewById(R.id.textBackupPC);
		    tv.setText( entity.serverName);
		    ImageView iv = (ImageView) rowView.findViewById(R.id.imagePC);
		    if(!TextUtils.isEmpty(entity.serverOS)){
			    if(entity.serverOS.equals("OSX")){
			    	iv.setImageResource(R.drawable.ic_apple);
			    }
			    else{
			    	iv.setImageResource(R.drawable.ic_windows);
			    }
		    }
		    return rowView;
		  }

		  
		@Override
		public int getCount() {
			return values.size();
		}

		@Override
		public ServerEntity getItem(int position) {
			return values.get(position);
		}

		@Override
		public long getItemId(int position) {
			// TODO Auto-generated method stub
			return 0;
		}
		public void addData(ServerEntity entity) {
			values.add(entity);
			this.notifyDataSetChanged();
		}
		public void setData(ArrayList<ServerEntity> entities) {
			values = entities;
			this.notifyDataSetChanged();
		}
		
		
	}

	public interface AutoImportListener {
		public void importNow();
		public void notImportNow();
	}
	@Override
	public void onCheckedChanged(CompoundButton arg0, boolean arg1) {
	}
}
