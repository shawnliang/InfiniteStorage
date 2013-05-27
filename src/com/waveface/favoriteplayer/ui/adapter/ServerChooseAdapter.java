package com.waveface.favoriteplayer.ui.adapter;

import java.util.ArrayList;

import android.content.Context;
import android.text.TextUtils;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.TextView;

import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.RuntimeState;
import com.waveface.favoriteplayer.entity.ServerEntity;

public class ServerChooseAdapter extends BaseAdapter {
	private final Context context;
	private LayoutInflater mInflater;
	private ViewHolder holder;
	private ArrayList<ServerEntity> values;
	private boolean[] isFocused;
	private int whichClick = -1;

	public ServerChooseAdapter(Context context, ArrayList<ServerEntity> datas) {
		super();
		this.context = context;
		mInflater = (LayoutInflater) context
				.getSystemService(Context.LAYOUT_INFLATER_SERVICE);

		this.values = datas;
		setFocus();
	}

	public void setFocus() {
		isFocused = new boolean[values.size()];
		for (int i = 0; i < isFocused.length; i++) {
			isFocused[i] = false;
		}
	}

	public void changeBackGround(int position) {
		isFocused[whichClick == -1 ? 0 : whichClick] = false;
		whichClick = position;
		isFocused[position] = true;
		notifyDataSetChanged();
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent) {

		if (convertView == null) {
			convertView = mInflater.inflate(R.layout.item_choose_server_row, parent,
					false);
			holder = new ViewHolder();
			holder.imagePC = (ImageView) convertView.findViewById(R.id.imagePC);
			holder.serverName = (TextView) convertView
					.findViewById(R.id.textBackupPC);
			holder.imageConnect = (ImageView) convertView
					.findViewById(R.id.ivConnected);
			convertView.setTag(holder);
		} else {
			holder = (ViewHolder) convertView.getTag();
		}
//		convertView.setBackgroundColor(isFocused[position]? 
//				R.color.bonjour_serve_click:
//				R.color.bonjour_server_default);

		ServerEntity entity = values.get(position);
		if (!TextUtils.isEmpty(entity.serverOS)) {
			if (entity.serverOS.equals("OSX")) {
				holder.imagePC.setImageResource(R.drawable.ic_apple);
			} else {
				holder.imagePC.setImageResource(R.drawable.ic_windows);
			}
		}
		holder.serverName.setText(entity.serverName);
		if (!TextUtils.isEmpty(entity.serverId)
				&& entity.serverId.equals(RuntimeState.mWebSocketServerId)) {
			holder.imageConnect.setVisibility(View.VISIBLE);
			holder.serverName.setTextColor(context.getResources().getColor(
					R.color.linked));
		}
		return convertView;
	}

	static class ViewHolder {
		ImageView imagePC;
		TextView serverName;
		ImageView imageConnect;
	}

	@Override
	public int getCount() {
		return values.size();
	}

	@Override
	public ServerEntity getItem(int position) {
		return values.get(position);
	}

	@Override
	public long getItemId(int position) {
		return 0;
	}

	public void addData(ServerEntity entity) {
		values.add(entity);
		setFocus();
		this.notifyDataSetChanged();
	}

	public void setData(ArrayList<ServerEntity> entities) {
		values = entities;
		setFocus();
		this.notifyDataSetChanged();
	}
}
