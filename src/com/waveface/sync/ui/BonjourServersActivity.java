package com.waveface.sync.ui;

import java.util.ArrayList;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.logic.BackupLogic;
import com.waveface.sync.logic.ServersLogic;

public class BonjourServersActivity extends Activity implements OnClickListener{
	private String TAG = BonjourServersActivity.class.getSimpleName();

	private ServerArrayAdapter mAdapter ; 
	private Button mBtnPre ;
	private Button mBtnNext ;
	
	//DATA
	private ServerEntity mServer ;

	@Override
	protected void onCreate(Bundle bundle) {
		super.onCreate(bundle);
		setContentView(R.layout.first_use_bonjour_servers);
		IntentFilter filter = new IntentFilter();
		filter.addAction(Constant.ACTION_BONJOUR_MULTICAT_EVENT);
		filter.addAction(Constant.ACTION_WS_SERVER_NOTIFY);		
		registerReceiver(mReceiver, filter);

		ListView listview = (ListView) findViewById(R.id.listview);
		ServerEntity entity = (ServerEntity) getIntent().getExtras().get(Constant.PARAM_SERVER_DATA);
		ArrayList<ServerEntity> datas = new ArrayList<ServerEntity>();
		datas.add(entity);
		mAdapter = new ServerArrayAdapter(this, datas);
		listview.setAdapter(mAdapter);
		listview.setOnItemClickListener(new AdapterView.OnItemClickListener() {

		      @Override
		      public void onItemClick(AdapterView<?> parent, final View view,
		          int position, long id) {
		        Log.d(TAG, "position:"+position);
		        mServer = mAdapter.getItem(position);
		        clickToLinkServer(mServer);
//		        Finish(mAdapter.getItem(position));
		      }
		});
		mBtnPre = (Button) findViewById(R.id.btnPre);
		mBtnPre.setOnClickListener(this);
		
		mBtnNext = (Button) findViewById(R.id.btnNext);		
		mBtnNext.setOnClickListener(this);
		
	}
	private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			String action = intent.getAction();
			Log.d(TAG, "action:" + intent.getAction());
			if (Constant.ACTION_BONJOUR_MULTICAT_EVENT.equals(action)) {		
				ServerEntity entity = (ServerEntity) intent.getExtras().get(Constant.EXTRA_SERVER_DATA);
				mAdapter.addData(entity);
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
			    	SharedPreferences prefs = getSharedPreferences(Constant.PREFS_NAME, Context.MODE_PRIVATE);
			    	Editor editor = prefs.edit();
			    	editor.putString(Constant.PREF_STATION_WEB_SOCKET_URL, mServer.wsLocation);
			    	editor.putString(Constant.PREF_SERVER_ID,mServer.serverId);
			    	editor.commit();
			    	//Add Server Id for file
			    	BackupLogic.addFileIndexForServer(context, entity.serverId);
					ServersLogic.startBackuping(context, mServer);
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

	class ServerArrayAdapter extends BaseAdapter {
		  private final Context context;
		  private final ArrayList<ServerEntity> values;

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
		
	}
    @Override
    protected void onPause() {
        super.onPause();
    }
    
    @Override
    protected void onResume() {
        super.onResume();
    }
    
    @Override
    protected void onDestroy() {
		unregisterReceiver(mReceiver);
    	super.onDestroy();
    }
    
    private void clickToLinkServer(ServerEntity entity){
    	ServersLogic.startWSServerConnect(this, entity.wsLocation,entity.serverId);
    }
    public void NextPage(){
//    	Intent returnIntent = new Intent();
//    	returnIntent.putExtra(Constant.PARAM_RESULT,entity);
//    	setResult(RESULT_OK,returnIntent);    	
    	setResult(RESULT_OK);
    	finish();
    }

	@Override
	public void onClick(View v) {
		switch(v.getId()){
			case R.id.btnPre:
				//Go Pre Page
				break;
			case R.id.btnNext:
				//Go Next Page
				NextPage();
				break;
		}		
	}
    
}
