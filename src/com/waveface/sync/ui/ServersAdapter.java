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
	    	tv.setText( entity.serverName+" ( "+context.getString(R.string.backuping)+" )");
	    	iv.setImageResource(R.drawable.ic_transfer);
	    	pb.setVisibility(View.VISIBLE);
	    }
	    else{
	    	tv.setText( entity.serverName+" ( "+context.getString(R.string.offline)+" )");
	    	iv.setImageResource(R.drawable.ic_offline);
	    	pb.setVisibility(View.INVISIBLE);
	    }
	    tv = (TextView) rowView.findViewById(R.id.textBackupDays);
	    tv.setText(context.getString(R.string.backup_period,"2010-01-01","2013-04-12"));	    
	    tv = (TextView) rowView.findViewById(R.id.textFreespace);
	    tv.setText(context.getString(R.string.free_space,"500TB"));
	    tv = (TextView) rowView.findViewById(R.id.textFolder);
	    tv.setText(context.getString(R.string.backup_folder,"C:\\infiniteStorage"));	    
	    tv = (TextView) rowView.findViewById(R.id.textBackupInfo);
	    tv.setText(context.getString(R.string.backup_info,100,200,300));	    
	    
	    
	    
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