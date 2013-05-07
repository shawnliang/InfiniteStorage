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
import android.provider.Settings;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.ListView;


import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.RuntimeState;
import com.waveface.sync.db.BonjourServersTable;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.util.NetworkUtil;
import com.waveface.sync.websocket.RuntimeWebClient;


public class AddServerFragment extends FragmentBase implements OnClickListener{
	public final String TAG = AddServerFragment.class.getSimpleName();

	private ViewGroup mRootView;
	
	private ServerChooseAdapter mNewServerAdapter ;
	private ServerChooseAdapter mPairedAdapter ;	
	
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
		mNewServerAdapter = new ServerChooseAdapter(getActivity(), ServersLogic.getBonjourServersExportPaired(getActivity()));
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
		    	  ServersLogic.updateAllBackedServerStatus(getActivity(),Constant.SERVER_DENIED_BY_CLIENT);
		    	  clickToLinkServer(mServer);
		      }
		});
		
		
		mPairedServerListview = (ListView) mRootView.findViewById(R.id.pairedServerListview);
		mPairedAdapter = new ServerChooseAdapter(getActivity(), ServersLogic.getBackupedServers(getActivity()));
		mPairedServerListview.setAdapter(mPairedAdapter);

		mBtnCancel = (Button) mRootView.findViewById(R.id.btnCancel);
		mBtnCancel.setOnClickListener(this);

		if(!NetworkUtil.isWifiNetworkAvailable(getActivity())){
			ServersLogic.purgeAllBonjourServer(getActivity());
			openDialog(getActivity(),Constant.NETWORK_IS_NOT_WIFI);
		}
				
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
    private void clickToLinkServer(ServerEntity entity){
		openDialog(getActivity(),Constant.WS_ACTION_WAIT_FOR_PAIR);
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
}
