package com.waveface.sync.ui.fragment;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.ContentResolver;
import android.content.Context;
import android.content.DialogInterface;
import android.database.ContentObserver;
import android.os.Bundle;
import android.os.Handler;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.Button;

import com.waveface.sync.R;
import com.waveface.sync.db.BonjourServersTable;
import com.waveface.sync.logic.ServersLogic;

public class InstallFragment extends FragmentBase implements OnClickListener{
	public final String TAG = InstallFragment.class.getSimpleName();
	private ViewGroup mRootView;
    private Handler mHandler = new Handler();
	private BonjourServerContentObserver mContentObserver;
	private InstallFragmentListener mListener;
	private AlertDialog mAlertDialog;


	@Override
    public void onAttach(Activity activity) {
        super.onAttach(activity);
        try {
        	mListener = (InstallFragmentListener) activity;
        } catch (ClassCastException e) {
            throw new ClassCastException(activity.toString() + " must implement InstallFragmentListener");
        }
    }

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		mRootView = (ViewGroup) inflater.inflate(
				R.layout.fragment_install, null);
		final Button mNextBtn = (Button) mRootView.findViewById(R.id.btnNext);
		mNextBtn.setOnClickListener(this);
		if(!ServersLogic.hasBonjourServers(getActivity())){
	        mHandler.postDelayed(new Runnable() {
	            public void run() {
	            	  openSendDialog(getActivity());
	            	}
	            }, 10000);
		}
		else{
			mListener.onInstallNext();
		}
		return mRootView;
	}
	private class BonjourServerContentObserver extends ContentObserver {
		public BonjourServerContentObserver() {
			super(new Handler());
		}

		@Override
		public void onChange(boolean selfChange) {
			if(getActivity() != null) {
				if(ServersLogic.hasBonjourServers(getActivity())){
					if(mAlertDialog!=null && mAlertDialog.isShowing()){
						mAlertDialog.dismiss();
					}
					mListener.onInstallNext();
				}
			}
		}
	}
     
	
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		mContentObserver = new BonjourServerContentObserver();
		ContentResolver cr = getActivity().getContentResolver();
		cr.registerContentObserver(BonjourServersTable.BONJOUR_SERVER_URI, false, mContentObserver);
	}
	public interface InstallFragmentListener {
		public void onInstallNext();
		public void onSendEmail();
	}

	@Override
	public void onDestroy() {
		super.onDestroy();
	}

	@Override
	public void onClick(View v) {
		int viewId = v.getId();
		switch (viewId) {
			case R.id.btnNext:
				//mListener.onInstallNext();
				getActivity().finish();
				break;
		}
	}
	private void openSendDialog(Context context){
		if(mAlertDialog!=null && mAlertDialog.isShowing()){
			mAlertDialog.dismiss();
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
					mListener.onSendEmail();
					dialog.cancel();
					openConfirmDialog(getActivity());
				}
			  })
			.setNegativeButton(R.string.btn_no,new DialogInterface.OnClickListener() {
				public void onClick(DialogInterface dialog,int id) {
					dialog.cancel();
				}
			  }); 
		
		// create alert dialog
		mAlertDialog = alertDialogBuilder.create();
		// show it
		mAlertDialog.show();
	}
	private void openConfirmDialog(Context context){
		if(mAlertDialog!=null && mAlertDialog.isShowing()){
			mAlertDialog.dismiss();
		}
		String message = context.getString(R.string.install_dialog_go_install);		
		AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(context); 
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

}