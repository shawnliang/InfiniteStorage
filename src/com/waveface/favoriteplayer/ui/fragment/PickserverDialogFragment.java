package com.waveface.favoriteplayer.ui.fragment;

import java.util.ArrayList;
import java.util.HashMap;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.db.BonjourServersTable;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.event.ServerChooseEvent;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.ui.adapter.ServerChooseAdapter;

import de.greenrobot.event.EventBus;

import android.app.Dialog;
import android.content.BroadcastReceiver;
import android.content.ContentResolver;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.database.ContentObserver;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Handler;
import android.support.v4.app.DialogFragment;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.widget.AdapterView;
import android.widget.ListView;

public class PickserverDialogFragment extends DialogFragment {
	public static final String TAG = PickserverDialogFragment.class
			.getSimpleName();

	private ServerChooseAdapter mAdapter;
	private ViewGroup mProgressBar;
	private BonjourServerContentObserver mContentObserver;
	
	private ListView mList;

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		getDialog().getWindow().requestFeature(Window.FEATURE_NO_TITLE);
		
		View root = inflater.inflate(R.layout.dialog_frgment_pickserver,
				container, false);

		mProgressBar = (ViewGroup) root.findViewById(R.id.container_search);

		mList = (ListView) root.findViewById(R.id.listview);
		ArrayList<ServerEntity> serverEntityList = ServersLogic
				.getBonjourServers(getActivity());

		if(serverEntityList.size() == 0)
			mProgressBar.setVisibility(View.VISIBLE);
		else
			mProgressBar.setVisibility(View.INVISIBLE);
		mAdapter = new ServerChooseAdapter(getActivity(), serverEntityList);
		mList.setAdapter(mAdapter);
		mList.setOnItemClickListener(new AdapterView.OnItemClickListener() {

			@Override
			public void onItemClick(AdapterView<?> parent, final View view,
					int position, long id) {
				clickToLinkServer(mAdapter.getItem(position));
				EventBus.getDefault().post(new ServerChooseEvent());
				dismiss();
			}
		});


		return root;
	}
	
	@Override
	public void onResume() {
		super.onResume();
		mList.requestFocus();
	}
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		IntentFilter filter = new IntentFilter();
		filter.addAction(Constant.ACTION_BONJOUR_MULTICAT_EVENT);
		getActivity().registerReceiver(mReceiver, filter);

		mContentObserver = new BonjourServerContentObserver();
		ContentResolver cr = getActivity().getContentResolver();
		cr.registerContentObserver(BonjourServersTable.BONJOUR_SERVER_URI,
				false, mContentObserver);
	}

	@Override
	public void onDestroy() {
		getActivity().unregisterReceiver(mReceiver);
		ContentResolver cr = getActivity().getContentResolver();
		cr.unregisterContentObserver(mContentObserver);

		super.onDestroy();
	}

	public void refreshUI() {
		ArrayList<ServerEntity> servers = ServersLogic.getBonjourServers(getActivity());
		mAdapter.setData(servers);
		if(servers.size() > 0)
			mProgressBar.setVisibility(View.INVISIBLE);
		else
			mProgressBar.setVisibility(View.VISIBLE);
	}

	@SuppressWarnings("unchecked")
	private void clickToLinkServer(ServerEntity entity) {
		HashMap<String, String> param = new HashMap<String, String>();
		param.put(Constant.PARAM_SERVER_ID, entity.serverId);
		param.put(Constant.PARAM_SERVER_NAME, entity.serverName);
		param.put(Constant.PARAM_SERVER_IP, entity.ip);
		param.put(Constant.PARAM_NOTIFY_PORT, entity.notifyPort);
		param.put(Constant.PARAM_REST_PORT, entity.restPort);
		new LinkBonjourServer().execute(param);
	}

	class LinkBonjourServer extends
			AsyncTask<HashMap<String, String>, Void, Void> {
		@Override
		protected Void doInBackground(HashMap<String, String>... params) {
			String serverId = params[0].get(Constant.PARAM_SERVER_ID);
			String serverName = params[0].get(Constant.PARAM_SERVER_NAME);
			String ip = params[0].get(Constant.PARAM_SERVER_IP);
			String notifyPort = params[0].get(Constant.PARAM_NOTIFY_PORT);
			String restPort = params[0].get(Constant.PARAM_REST_PORT);
			String wsLocation = "ws://" + ip + ":" + notifyPort;
			ServersLogic.startWSServerConnect(getActivity(), wsLocation,
					serverId, serverName, ip, notifyPort, restPort, true);
			return null;
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
		}
	};

	private class BonjourServerContentObserver extends ContentObserver {
		public BonjourServerContentObserver() {
			super(new Handler());
		}

		@Override
		public void onChange(boolean selfChange) {
			if (getActivity() != null) {
				refreshUI();
			}
		}
	}
}
