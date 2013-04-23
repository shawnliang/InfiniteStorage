package com.waveface.sync.ui;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.Button;

import com.waveface.sync.R;

public class LinkFragment extends LinkFragmentBase implements OnClickListener{
	public final String TAG = LinkFragment.class.getSimpleName();
	private ViewGroup mRootView;
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

		Button mNextBtn = (Button) mRootView.findViewById(R.id.btnNext);
		mNextBtn.setOnClickListener(this);

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
	private void startAcctivityWithAnimation(Intent intent) {
		startActivity(intent);
		getActivity().overridePendingTransition(android.R.anim.fade_in,
				android.R.anim.fade_out);
	}
	public void gotoTimeline() {
		Intent intent = new Intent(getActivity(), MainActivity.class);
		startAcctivityWithAnimation(intent);
		getActivity().finish();
	}

}