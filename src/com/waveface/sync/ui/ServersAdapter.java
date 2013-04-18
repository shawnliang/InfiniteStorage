package com.waveface.sync.ui;

import java.util.ArrayList;

import android.content.Context;
import android.text.TextUtils;
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
import com.waveface.sync.logic.FileBackup;
import com.waveface.sync.util.StringUtil;

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
	    	iv.setImageResource(R.drawable.ic_transfer);
	    	String wording = null;
	    	if(FileBackup.needToBackup(context, entity.serverId)){
	    		wording = entity.serverName+" ( "+context.getString(R.string.backuping)+" )";
	    		pb.setVisibility(View.VISIBLE);
	    	}
	    	else{
	    		wording = entity.serverName+" ( "+context.getString(R.string.backuped_completed)+" )";
	    		pb.setVisibility(View.INVISIBLE);
	    	}
	    	tv.setText( wording);
	    	
	    }
	    else{
	    	tv.setText( entity.serverName+" ( "+context.getString(R.string.offline)+" )");
	    	iv.setImageResource(R.drawable.ic_offline);
	    	pb.setVisibility(View.INVISIBLE);
	    }
	    tv = (TextView) rowView.findViewById(R.id.textBackupDays);
	    if(!TextUtils.isEmpty(entity.startDatetime)){
	    	entity.startDatetime = entity.startDatetime.substring(0, 10);
	    }
	    if(!TextUtils.isEmpty(entity.endDatetime)){
	    	entity.endDatetime = entity.endDatetime.substring(0, 10);
	    }

	    tv.setText(context.getString(R.string.backup_period,entity.startDatetime,entity.endDatetime));	    
	    tv = (TextView) rowView.findViewById(R.id.textFreespace);
	    tv.setText(context.getString(R.string.free_space,StringUtil.byteCountToDisplaySize(entity.freespace)));
	    tv = (TextView) rowView.findViewById(R.id.textFolder);
	    tv.setText(context.getString(R.string.backup_folder,entity.folder));	    
	    tv = (TextView) rowView.findViewById(R.id.textBackupInfo);
	    tv.setText(context.getString(R.string.backup_info,entity.photoCount,entity.videoCount,entity.audioCount));	    
	    
	    
	    
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