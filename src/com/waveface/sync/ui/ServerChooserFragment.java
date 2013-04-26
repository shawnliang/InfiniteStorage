package com.waveface.sync.ui;

import java.util.ArrayList;

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
import android.widget.CompoundButton;
import android.widget.CompoundButton.OnCheckedChangeListener;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.RuntimeState;
import com.waveface.sync.db.BonjourServersTable;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.logic.ServersLogic;


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
	private Button mBtnBackup;
	
	private AlertDialog mAlertDialog;


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
				R.layout.first_use_bonjour_servers, null);
		ListView listview = (ListView) mRootView.findViewById(R.id.listview);
		mAdapter = new ServerChooseAdapter(getActivity(), ServersLogic.getBonjourServers(getActivity()));
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
		if(mAdapter.getCount()>0){
		    mProgressBar.setVisibility(View.INVISIBLE);
		    mTvSearch.setVisibility(View.INVISIBLE);
		}
		
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
						refreshUI();
						dismissProgress();
						if(mAlertDialog!=null && mAlertDialog.isShowing()){
							mAlertDialog.dismiss();
						}
//						Toast.makeText(getActivity(), R.string.pairing_starting_backup, Toast.LENGTH_LONG).show();
						getActivity().setResult(getActivity().RESULT_OK,new Intent());
						mListener.goNext(TAG);
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
			case R.id.btnPre:
				mListener.goBack(TAG);
				break;
		}
	}
	public void refreshUI(){
		mAdapter.setData(ServersLogic.getBonjourServers(getActivity()));
		if(mAdapter.getCount()>0){
		    mProgressBar.setVisibility(View.INVISIBLE);
		    mTvSearch.setVisibility(View.INVISIBLE);
		}
	}
	
	@Override
	public void onCheckedChanged(CompoundButton arg0, boolean arg1) {
	}
}
