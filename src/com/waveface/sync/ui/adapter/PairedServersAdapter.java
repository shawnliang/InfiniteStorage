package com.waveface.sync.ui.adapter;

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
import com.waveface.sync.RuntimeState;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.logic.BackupLogic;
import com.waveface.sync.util.StringUtil;

public class PairedServersAdapter  extends BaseAdapter {
	  private final Context context;
	  private ArrayList<ServerEntity> mServers;
      private static boolean[] isFocused ;  
      private static int whichClick = -1;  
	  
	  public PairedServersAdapter(Context context, ArrayList<ServerEntity> servers) {
	    super();
	    this.context = context;
	    this.mServers = servers;
	    setFocus();
	  }

	  public void setFocus(){
	    isFocused = new boolean[this.mServers.size()];  
        for(int i=0;i< isFocused.length;i++){            
                isFocused[i] = false;  
        }  
	  }
	  
	  public void changeBackGround(int position){  
             isFocused[whichClick==-1?0:whichClick] = false;  
             whichClick = position;  
             isFocused[position] = true;  
             notifyDataSetChanged();  
      }  
	  
	  static class ViewHolder {
		    ImageView iv ;
		    ProgressBar pb ;
		    TextView tvBackupPC;
		    TextView tvBackupDays;
		    TextView tvFreespace;
		    TextView tvFolder;
		    TextView tvBackupInfo;		    
		    TextView textPhotoCount;		    
		    TextView textVideoCount;		    
		    TextView textAudioCount;		    		 
		    TextView tvLastBackupTime;
		    
	  }
	  
	  @Override
	  public View getView(int position, View v, ViewGroup parent) {
		  ViewHolder viewHolder = null;
		  if( v==null){
		    LayoutInflater inflater = (LayoutInflater) context
		        .getSystemService(Context.LAYOUT_INFLATER_SERVICE);
		    v = inflater.inflate(R.layout.item_paired_servers, parent, false);
		    viewHolder = new ViewHolder();
		    viewHolder.iv = (ImageView) v.findViewById(R.id.imagePC);
		    viewHolder.pb = (ProgressBar) v.findViewById(R.id.pbBackup);
		    viewHolder.tvBackupPC = (TextView) v.findViewById(R.id.textBackupPC);		    
		    viewHolder.tvBackupDays = (TextView) v.findViewById(R.id.textBackupDays);
		    viewHolder.tvFreespace = (TextView) v.findViewById(R.id.textFreespace);
		    viewHolder.tvFolder = (TextView) v.findViewById(R.id.textFolder);
		    viewHolder.tvBackupInfo = (TextView) v.findViewById(R.id.textBackupInfo);
		    viewHolder.textPhotoCount = (TextView) v.findViewById(R.id.textPhotoCount);
		    viewHolder.textVideoCount = (TextView) v.findViewById(R.id.textVideoCount);
		    viewHolder.textAudioCount = (TextView) v.findViewById(R.id.textAudioCount);		    		 		 
		    viewHolder.tvLastBackupTime = (TextView) v.findViewById(R.id.textLastBackupTime);
		    
		    v.setTag(viewHolder);
		  }
		  else{
			  viewHolder = (ViewHolder) v.getTag();
		  }
		  v.setBackgroundColor(isFocused[position]?
				  R.color.bonjour_serve_click:R.color.bonjour_serve_click);
		  
		  //DATA FOR DISPLAY
		  ServerEntity entity = mServers.get(position);
		  if (entity != null) {
//			    if( !TextUtils.isEmpty(RuntimeState.mWebSocketServerId) 
//			    		&& RuntimeState.mWebSocketServerId.equals(entity.serverId)){
			    if(entity.status.equals(Constant.SERVER_LINKING)){	    
			    	viewHolder.iv.setImageResource(R.drawable.ic_transfer);
			    	String wording = null;
			    	if(BackupLogic.needToBackup(context, entity.serverId) && RuntimeState.isScaning == false){
			    		wording = entity.serverName+" ( "+context.getString(R.string.backuping)+" )";
			    		viewHolder.pb.setVisibility(View.VISIBLE);
			    	}
			    	else{
			    		wording = entity.serverName+" ( "+context.getString(R.string.backuped_completed)+" )";
			    		viewHolder.pb.setVisibility(View.INVISIBLE);
			    	}
			    	viewHolder.tvBackupPC.setText( wording);	
			    }
			    else{
			    	viewHolder.tvBackupPC.setText( entity.serverName+" ( "+context.getString(R.string.offline)+" )");
			    	viewHolder.iv.setImageResource(R.drawable.ic_offline);
			    	viewHolder.pb.setVisibility(View.INVISIBLE);
			    }
			    if(!TextUtils.isEmpty(entity.startDatetime)){
			    	entity.startDatetime = entity.startDatetime.substring(0, 10);
			    }
			    if(!TextUtils.isEmpty(entity.endDatetime)){
			    	entity.endDatetime = entity.endDatetime.substring(0, 10);
			    }
			    viewHolder.tvBackupDays.setText(context.getString(R.string.backup_period,entity.startDatetime,entity.endDatetime));	    
			    viewHolder.tvFreespace.setText(context.getString(R.string.free_space,StringUtil.byteCountToDisplaySize(entity.freespace)));
			    viewHolder.tvFolder.setText(context.getString(R.string.backup_folder,entity.folder));	    
//			    viewHolder.tvBackupInfo.setText(context.getString(R.string.backup_info,entity.photoCount,entity.videoCount,entity.audioCount));	 
			    viewHolder.textPhotoCount.setText(context.getString(R.string.photos,entity.photoCount));
			    viewHolder.textVideoCount.setText(context.getString(R.string.videos,entity.videoCount));
			    viewHolder.textAudioCount.setText(context.getString(R.string.audios,entity.audioCount));		    		 		 
			    String lastBackupTime = StringUtil.displayLocalTime(entity.lastLocalBackupTime,StringUtil.DATE_FORMAT);
			    viewHolder.tvLastBackupTime.setText(context.getString(R.string.backup_last_local_time,lastBackupTime));
		  }
	    return v;
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
		setFocus();
		this.notifyDataSetChanged();
	}
	public void setData(ArrayList<ServerEntity> servers) {
		mServers = servers;
		setFocus();
		this.notifyDataSetChanged();
	}	
}