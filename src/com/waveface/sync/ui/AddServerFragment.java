package com.waveface.sync.ui;

import java.util.ArrayList;

import org.jwebsocket.kit.WebSocketException;

import android.app.AlertDialog;
import android.app.ProgressDialog;
import android.content.BroadcastReceiver;
import android.content.ContentResolver;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.ActivityInfo;
import android.database.ContentObserver;
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
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.RuntimeState;
import com.waveface.sync.db.BonjourServersTable;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.websocket.RuntimeWebClient;


public class AddServerFragment extends FragmentBase implements OnClickListener{
	public final String TAG = AddServerFragment.class.getSimpleName();

	private ViewGroup mRootView;
	
	private ServerArrayAdapter mNewServerAdapter ;
	private ServerArrayAdapter mPairedAdapter ;	
	
    private Handler mHandler = new Handler();
	private BonjourServerContentObserver mContentObserver;
	private ProgressDialog mProgressDialog;
	private Button mBtnCancel;
	
	private AlertDialog mAlertDialog;
	private ListView mAddServerListview ;
	private ListView mPairedServerListview ;

	//DATA
	private ServerEntity mServer ;

	public int getHeight() {
		return mRootView.getMeasuredHeight();
	}

	@Override
	public void onBackPressed() {
		getActivity().finish();
		
	}

	private class BonjourServerContentObserver extends ContentObserver {

		public BonjourServerContentObserver() {
			super(new Handler());
		}

		@Override
		public void onChange(boolean selfChange) {
			if(getActivity() != null) {
				refreshUI();
			}
		}

	}
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {

		mRootView = (ViewGroup) inflater.inflate(
				R.layout.add_server, null);
		mAddServerListview = (ListView) mRootView.findViewById(R.id.newServerListview);
		mNewServerAdapter = new ServerArrayAdapter(getActivity(), ServersLogic.getBonjourServersExportPaired(getActivity()));
		mAddServerListview.setAdapter(mNewServerAdapter);
		mAddServerListview.setOnItemClickListener(new AdapterView.OnItemClickListener() {

		      @Override
		      public void onItemClick(AdapterView<?> parent, final View view,
		          int position, long id) {
		    	  mServer = mNewServerAdapter.getItem(position);
		    	  if(RuntimeState.OnWebSocketStation){
		    		  try {
						RuntimeWebClient.close();
					} catch (WebSocketException e) {
						e.printStackTrace();
					}
		    	  }
		    	  ServersLogic.updateAllBackedServerOffline(getActivity());
		    	  clickToLinkServer(mServer);
		      }
		});
		
		
		mPairedServerListview = (ListView) mRootView.findViewById(R.id.pairedServerListview);
		mPairedAdapter = new ServerArrayAdapter(getActivity(), ServersLogic.getBackupedServers(getActivity()));
		mPairedServerListview.setAdapter(mPairedAdapter);
//		mPairedServerListview.setOnItemClickListener(new AdapterView.OnItemClickListener() {
//
//		      @Override
//		      public void onItemClick(AdapterView<?> parent, final View view,
//		          int position, long id) {
//		        Log.d(TAG, "position:"+position);
//		        mServer = mPairedAdapter.getItem(position);
//		        clickToLinkServer(mServer);
//		      }
//		});


		mBtnCancel = (Button) mRootView.findViewById(R.id.btnCancel);
		mBtnCancel.setOnClickListener(this);
				
        mHandler.postDelayed(new Runnable() {
            public void run() {
             	 mNewServerAdapter.setData(ServersLogic.getBonjourServersExportPaired(getActivity()));
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
		mContentObserver = new BonjourServerContentObserver();
		ContentResolver cr = getActivity().getContentResolver();
		cr.registerContentObserver(BonjourServersTable.BONJOUR_SERVER_URI, false, mContentObserver);

	}
	private void dismissProgress(){
		if ((mProgressDialog != null) && mProgressDialog.isShowing()) {
			mProgressDialog.dismiss();
		}		
	}
	private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			String action = intent.getAction();
			Log.d(TAG, "action:" + intent.getAction());
			if (Constant.ACTION_BONJOUR_MULTICAT_EVENT.equals(action)) {		
				refreshUI();
			}
			else if(Constant.ACTION_WS_SERVER_NOTIFY.equals(action)){
				String response = intent.getStringExtra(Constant.EXTRA_WEB_SOCKET_EVENT_CONTENT);
				if(response!=null){
					if(response.equals(Constant.WS_ACTION_ACCEPT)){
						dismissProgress();
						refreshUI();
						mBtnCancel.setEnabled(true);
						if(mAlertDialog!=null && mAlertDialog.isShowing()){
							mAlertDialog.dismiss();
						}
						getActivity().setResult(getActivity().RESULT_OK,new Intent());
					}
					else if(response.equals(Constant.WS_ACTION_DENIED)){
						dismissProgress();
						openDialog(context,Constant.WS_ACTION_DENIED);
					}				
				}
			}
		}
	};

	private void openDialog(Context context,String action){
		if(mAlertDialog!=null && mAlertDialog.isShowing()){
			mAlertDialog.dismiss();
		}
		String title = context.getString(R.string.title_pairing);
		String message = null;
		if(action.equals(Constant.WS_ACTION_DENIED)){
			message = context.getString(R.string.pairing_denied);
		}
		else if(action.equals(Constant.WS_ACTION_WAIT_FOR_PAIR)){
			message = context.getString(R.string.pairing_wait);
		}
		
		AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(context); 
		// set title
		alertDialogBuilder.setTitle(title);
		// set dialog message
		alertDialogBuilder
			.setMessage(message)
			.setCancelable(false)
			.setPositiveButton(R.string.btn_ok,new DialogInterface.OnClickListener() {
				public void onClick(DialogInterface dialog,int id) {
					dialog.cancel();
				}
			  }); 
		// create alert dialog
		mAlertDialog = alertDialogBuilder.create();
		// show it
		mAlertDialog.show();
	}
	@Override
	public void onDestroy() {
		getActivity().unregisterReceiver(mReceiver);
		super.onDestroy();
		ContentResolver cr = getActivity().getContentResolver();
		cr.unregisterContentObserver(mContentObserver);
	}
    private void clickToLinkServer(ServerEntity entity){
		mProgressDialog = ProgressDialog.show(getActivity(), "",getString(R.string.pairing));
		mProgressDialog.setCancelable(true);
    	ServersLogic.startWSServerConnect(getActivity(), entity.wsLocation,entity.serverId);
    }
	@Override
	public void onClick(View v) {
		int viewId = v.getId();
		switch (viewId) {
			case R.id.btnCancel:
				getActivity().finish();
				break;
		}
	}
	public void refreshUI(){
		mNewServerAdapter.setData(ServersLogic.getBonjourServersExportPaired(getActivity()));
		mPairedAdapter.setData(ServersLogic.getBackupedServers(getActivity()));
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
//		    String status = ServersLogic.getStatusByServerId(context, entity.serverId);
		    if(!TextUtils.isEmpty(entity.serverId) 
		    		&& entity.serverId.equals(RuntimeState.mWebSocketServerId)){
		    	iv = (ImageView) rowView.findViewById(R.id.ivConnected);
		    	iv.setVisibility(View.VISIBLE);
		    	tv.setTextColor(context.getResources().getColor(R.color.linked));
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
}
