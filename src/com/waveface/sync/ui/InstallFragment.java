package com.waveface.sync.ui;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.Button;

import com.waveface.sync.R;
import com.waveface.sync.logic.ServersLogic;

public class InstallFragment extends FragmentBase implements OnClickListener{
	public final String TAG = InstallFragment.class.getSimpleName();
	private ViewGroup mRootView;
    private Handler mHandler = new Handler();
	private InstallFragmentListener mListener;


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
				R.layout.first_use_install, null);

		Button mSendLinkBtn = (Button) mRootView.findViewById(R.id.btnSendLink);
		mSendLinkBtn.setOnClickListener(this);

		final Button mNextBtn = (Button) mRootView.findViewById(R.id.btnNext);
		mNextBtn.setOnClickListener(this);
		mNextBtn.setEnabled(false);
        mHandler.postDelayed(new Runnable() {
            public void run() {
            	mNextBtn.setEnabled(true);
            	}
            }, 4000);
		return mRootView;
	}
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
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
			case R.id.btnSendLink:
				mListener.onSendEmail();
				break;
			case R.id.btnNext:
				mListener.onInstallNext();
				break;
		}
	}
}