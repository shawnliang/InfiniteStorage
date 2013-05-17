package com.waveface.sync.ui.fragment;


import android.app.Activity;
import android.content.Context;
import android.content.pm.ActivityInfo;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.inputmethod.InputMethodManager;

import com.waveface.sync.util.Log;
import com.waveface.sync.util.ViewUtil;

public class FragmentBase extends Fragment {
	public static final String TAG = FragmentBase.class.getSimpleName();
	protected onFragmentChangedListener mListener;

	public interface onFragmentChangedListener {
		public void goBack(String id);
		public void goNext(String id);
	}

	@Override
	public void onActivityCreated(Bundle savedInstanceState) {
	    super.onActivityCreated(savedInstanceState);
	    final InputMethodManager imm = (InputMethodManager) getActivity().getSystemService(Context.INPUT_METHOD_SERVICE);
	    imm.hideSoftInputFromWindow(getView().getWindowToken(), 0);
	}

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
//		if (ViewUtil.qualifiedTabletLayout(getActivity()) == false) {
//			getActivity().setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);
//		}
	}

	@Override
	public void onAttach(Activity activity) {
		super.onAttach(activity);
		try {
			mListener = (onFragmentChangedListener) activity;
		} catch (ClassCastException e) {
			throw new ClassCastException(activity.toString()
					+ " must implement onFragmentChangedListener");
		}
	}

	public void onBackPressed() {
		Log.d(TAG, "onBackPressed");
	}

}
