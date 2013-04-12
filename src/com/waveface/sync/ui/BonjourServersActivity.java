package com.waveface.sync.ui;

import java.util.ArrayList;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.entity.ServerEntity;

public class BonjourServersActivity extends Activity {
	private String TAG = BonjourServersActivity.class.getSimpleName();
	private ServerArrayAdapter mAdapter ; 
	@Override
	protected void onCreate(Bundle bundle) {
		super.onCreate(bundle);
		setContentView(R.layout.bonjour_servers);
		IntentFilter filter = new IntentFilter();
		filter.addAction(Constant.ACTION_BONJOUR_MULTICAT_EVENT);
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
		        Finish(mAdapter.getItem(position));
		      }
		});
	}
	private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			String action = intent.getAction();
			Log.d(TAG, "action:" + intent.getAction());
			if (Constant.ACTION_BONJOUR_MULTICAT_EVENT.equals(action)) {		
				ServerEntity entity = (ServerEntity) intent.getExtras().get(Constant.PARAM_SERVER_DATA);
				mAdapter.addData(entity);
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
    
    public void Finish(ServerEntity entity){
    	Intent returnIntent = new Intent();
    	returnIntent.putExtra(Constant.PARAM_RESULT,entity);
    	setResult(RESULT_OK,returnIntent);
    	finish();
    }
    
}
