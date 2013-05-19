package com.waveface.favoriteplayer.ui.fragment;


import android.app.Activity;
import android.support.v4.app.Fragment;

import com.waveface.favoriteplayer.util.Log;

public abstract class SyncFragmentBase extends Fragment {
	public static final String TAG = SyncFragmentBase.class.getSimpleName();
	protected onSyncFragmentChangedListener mListener;

	public interface onSyncFragmentChangedListener {
		public void done(String id);
		public void fail(String id);
	}

	@Override
	public void onAttach(Activity activity) {
		super.onAttach(activity);
		try {
			mListener = (onSyncFragmentChangedListener) activity;
		} catch (ClassCastException e) {
			throw new ClassCastException(activity.toString()
					+ " must implement onFragmentChangedListener");
		}
	}

	public void onBackPressed() {
		Log.d(TAG, "onBackPressed");
	}

	protected void syncDone() {
		mListener.done(getFragmentId());
	}
	
	protected void syncFail() {
		mListener.fail(getFragmentId());
	}
	
	abstract String getFragmentId();
}
