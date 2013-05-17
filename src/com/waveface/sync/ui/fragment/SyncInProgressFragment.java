package com.waveface.sync.ui.fragment;

import com.waveface.sync.R;
import com.waveface.sync.event.LabelImportedEvent;

import de.greenrobot.event.EventBus;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

public class SyncInProgressFragment extends Fragment{
	private TextView mSyncContent;
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		EventBus.getDefault().register(this);
	}
	
	@Override
	public void onDestroy() {
		super.onDestroy();
		EventBus.getDefault().unregister(this);
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		View root = inflater.inflate(R.layout.fragment_sync_in_progress, container, false);
		mSyncContent = (TextView) root.findViewById(R.id.sync_content);
		mSyncContent.setVisibility(View.INVISIBLE);
		
		return root;
	}
	
	public void onEvent(LabelImportedEvent event) {
	}

}
