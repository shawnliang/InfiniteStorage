package com.waveface.sync.ui.fragment;

import java.util.ArrayList;
import java.util.HashMap;

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
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Handler;
import android.provider.Settings;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.CompoundButton.OnCheckedChangeListener;
import android.widget.ListView;
import android.widget.ProgressBar;
import android.widget.TextView;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.db.BonjourServersTable;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.ui.adapter.ServerChooseAdapter;
import com.waveface.sync.util.NetworkUtil;


public class ServerChooserFragment extends FragmentBase 
	implements OnClickListener, OnCheckedChangeListener{
	public final String TAG = ServerChooserFragment.class.getSimpleName();

	private ViewGroup mRootView;
	private ServerChooseAdapter mAdapter ; 
    private Handler mHandler = new Handler();
	private BonjourServerContentObserver mContentObserver;
	private ProgressDialog mProgressDialog;
	private ProgressBar mProgressBar;
	private TextView mTvSearch;
	private AlertDialog mAlertDialog;
	private AlertDialog mSendEmailDialog;
	


	//DATA
	private ServerEntity mServer ;

	public int getHeight() {
		return mRootView.getMeasuredHeight();
	}

	@Override
	public void onBackPressed() {
		if ((mProgressDialog != null) && mProgressDialog.isShowing()) {
			mProgressDialog.dismiss();
		} else {
			mListener.goBack(TAG);
		}
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
				R.layout.fragment_server_chooser, null);
		ListView listview = (ListView) mRootView.findViewById(R.id.listview);
		ArrayList<ServerEntity> serverEntityList =ServersLogic.getBonjourServers(getActivity());
		mAdapter = new ServerChooseAdapter(getActivity(), serverEntityList);
    	mAdapter.setData(serverEntityList);
		listview.setAdapter(mAdapter);
		listview.setOnItemClickListener(new AdapterView.OnItemClickListener() {

		      @Override
		      public void onItemClick(AdapterView<?> parent, final View view,
		          int position, long id) {
		        mServer = mAdapter.getItem(position);
		        clickToLinkServer(mServer);
		      }
		});

		Button btn = (Button) mRootView.findViewById(R.id.btnPre);
		btn.setOnClickListener(this);
		
	    mProgressBar = (ProgressBar) mRootView.findViewById(R.id.pbSearch);
	    mTvSearch = (TextView) mRootView.findViewById(R.id.tvSearch);
		displayProgressBar();
		
		//OPEN WI-Fi SETTING
		if(!NetworkUtil.isWifiNetworkAvailable(getActivity())){
			openDialog(getActivity(),Constant.NETWORK_IS_NOT_WIFI);
		}
		else{
			detectInstall();
		}
		return mRootView;
	}

	private void detectInstall() {
		if(!ServersLogic.hasBonjourServers(getActivity())){
		    mHandler.postDelayed(new Runnable() {
		        public void run() {
		        	  openSendDialog(getActivity());
		        	}
		        }, 300);
		}
	}

	private void displayProgressBar() {
		if(mAdapter.getCount()>0){
		    mProgressBar.setVisibility(View.GONE);
		    mTvSearch.setVisibility(View.GONE);
		}
		else{
		    mProgressBar.setVisibility(View.VISIBLE);
		    mTvSearch.setVisibility(View.VISIBLE);			
		}
	}

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		IntentFilter filter = new IntentFilter();
		filter.addAction(Constant.ACTION_BONJOUR_MULTICAT_EVENT);
		filter.addAction(Constant.ACTION_WEB_SOCKET_SERVER_CONNECTED);		
        getActivity().registerReceiver(mReceiver, filter);

		if (Constant.PHONE) {
			getActivity().setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);
		}

		mContentObserver = new BonjourServerContentObserver();
		ContentResolver cr = getActivity().getContentResolver();
		cr.registerContentObserver(BonjourServersTable.BONJOUR_SERVER_URI, false, mContentObserver);

	}
	private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			String action = intent.getAction();
			Log.d(TAG, "action:" + intent.getAction());
			if (Constant.ACTION_BONJOUR_MULTICAT_EVENT.equals(action)) {		
				refreshUI();
			}
			else if(Constant.ACTION_WEB_SOCKET_SERVER_CONNECTED.equals(action)){
				getActivity().finish();
			}
		}
	};
	private void openSendDialog(Context context){
		if(mSendEmailDialog!=null && mSendEmailDialog.isShowing()){
			mSendEmailDialog.dismiss();
		}
		String title = context.getString(R.string.install_dialog_title);
		String message = context.getString(R.string.install_dialog_send_mail);
		
		AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(context); 
		// set title
		alertDialogBuilder.setTitle(title);
		// set dialog message
		alertDialogBuilder
			.setMessage(message)
			.setCancelable(false)
			.setPositiveButton(R.string.email_description,new DialogInterface.OnClickListener() {
				public void onClick(DialogInterface dialog,int id) {
//					FlowLogic.sendDownloadEmail(getActivity());
					dialog.cancel();
				}
			  })
			.setNegativeButton(R.string.btn_no,new DialogInterface.OnClickListener() {
				public void onClick(DialogInterface dialog,int id) {
					dialog.cancel();
				}
			  }); 
		
		// create alert dialog
		mSendEmailDialog = alertDialogBuilder.create();
		// show it
		mSendEmailDialog.show();
	}
	
	private void openDialog(Context context,final String action){
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
		else if(action.equals(Constant.NETWORK_IS_NOT_WIFI)){
			title = context.getString(R.string.network_state);
			message = context.getString(R.string.without_wifi);
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
					if(action.equals(Constant.NETWORK_IS_NOT_WIFI)){
						startActivity(new Intent(Settings.ACTION_WIFI_SETTINGS));
						dialog.cancel();
						detectInstall();
					}
					else{
						dialog.cancel();
					}
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
    @SuppressWarnings("unchecked")
	private void clickToLinkServer(ServerEntity entity){
//		mProgressDialog = ProgressDialog.show(getActivity(), "",getString(R.string.pairing));
//		mProgressDialog.setCancelable(true);
		openDialog(getActivity(),Constant.WS_ACTION_WAIT_FOR_PAIR);
		String wsLocation = "ws://"+entity.ip+":"+entity.notifyPort;
		HashMap<String,String> param = new HashMap<String,String>();
		param.put(Constant.PARAM_SERVER_WS_LOCATION, wsLocation);
		param.put(Constant.PARAM_SERVER_ID, entity.serverId);
		param.put(Constant.PARAM_SERVER_NAME, entity.serverName);		
		param.put(Constant.PARAM_SERVER_IP, entity.ip);
		param.put(Constant.PARAM_NOTIFY_PORT, entity.notifyPort);
		param.put(Constant.PARAM_REST_PORT,entity.restPort);
		new LinkBonjourServer().execute(param);
    }
	@Override
	public void onClick(View v) {
		int viewId = v.getId();
		switch (viewId) {
			case R.id.btnPre:
				getActivity().finish();
//				mListener.goBack(TAG);
				break;
		}
	}
	public void refreshUI(){		
		if(ServersLogic.hasBonjourServers(getActivity())){
			if(mSendEmailDialog !=null && mSendEmailDialog.isShowing()){
				mSendEmailDialog.dismiss();
			}
		}
		mAdapter.setData(ServersLogic.getBonjourServers(getActivity()));
		displayProgressBar();
	}
	
	@Override
	public void onCheckedChanged(CompoundButton arg0, boolean arg1) {
	}
	
	class LinkBonjourServer extends AsyncTask<HashMap<String,String>,Void,Void>{

		@Override
		protected Void doInBackground(HashMap<String,String>... params) {
			
			String wsLocation = params[0].get(Constant.PARAM_SERVER_WS_LOCATION);
			String serverId = params[0].get(Constant.PARAM_SERVER_ID);
			String serverName = params[0].get(Constant.PARAM_SERVER_NAME);			
			String ip=params[0].get(Constant.PARAM_SERVER_IP);
			String notifyPort =params[0].get(Constant.PARAM_NOTIFY_PORT);
			String restPort =params[0].get(Constant.PARAM_REST_PORT);
			ServersLogic.startWSServerConnect(getActivity(), wsLocation,serverId,serverName,ip,notifyPort,restPort);
			return null;
		}
		
	}
}
