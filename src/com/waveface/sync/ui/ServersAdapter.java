package com.waveface.sync.ui;

import java.util.ArrayList;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.entity.ServerEntity;

public class ServersAdapter  extends BaseAdapter {
	  private final Context context;
	  private ArrayList<ServerEntity> mServers;

	  public ServersAdapter(Context context, ArrayList<ServerEntity> servers) {
	    super();
	    this.context = context;
	    this.mServers = servers;
	  }

	  @Override
	  public View getView(int position, View convertView, ViewGroup parent) {
	    LayoutInflater inflater = (LayoutInflater) context
	        .getSystemService(Context.LAYOUT_INFLATER_SERVICE);
	    View rowView = inflater.inflate(R.layout.server_row, parent, false);
	    ServerEntity entity = mServers.get(position);
	    ImageView iv = (ImageView) rowView.findViewById(R.id.imagePC);
	    ProgressBar pb = (ProgressBar) rowView.findViewById(R.id.pbBackup);
	    TextView tv = (TextView) rowView.findViewById(R.id.textBackupPC);
	    
	    
	    if(entity.status.equals(Constant.SERVER_LINKING)){
	    	tv.setText( entity.serverName+" ( BACKUPING... )");
	    	iv.setImageResource(R.drawable.ic_transfer);
	    	pb.setVisibility(View.VISIBLE);
	    }
	    else{
	    	tv.setText( entity.serverName+" ( OFFLINE )");
	    	iv.setImageResource(R.drawable.ic_offline);
	    	pb.setVisibility(View.INVISIBLE);
	    }
	    //tv = (TextView) rowView.findViewById(R.id.textBackupDays);
	    return rowView;
	  }

	  
	@Override
	public int getCount() {
		return mServers.size();
	}

	@Override
	public ServerEntity getItem(int position) {
		return mServers.get(position);
	}

	@Override
	public long getItemId(int position) {
		// TODO Auto-generated method stub
		return 0;
	}
	public void addData(ServerEntity entity) {
		mServers.add(entity);
		this.notifyDataSetChanged();
	}
	public void setData(ArrayList<ServerEntity> servers) {
		mServers = servers;
		this.notifyDataSetChanged();
	}	
}