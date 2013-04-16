package com.waveface.sync.entity;

import java.io.Serializable;

import com.google.gson.annotations.SerializedName;

public class ServerEntity implements Serializable {
	
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	public String serverId;
	
	public String serverName;
	
	public String serverOS;	
	
	public String status;	
	
	@SerializedName("action")
	public String action;

	@SerializedName("reason")
	public String reason;
	
	@SerializedName("backup_startdate")
	public String startDatetime;

	@SerializedName("backup_enddate")
	public String endDatetime;		
	
	@SerializedName("backup_folder")
	public String Folder;	
	
	@SerializedName("backup_folder_free_space")
	public long freespace;	
	
	@SerializedName("photo_count")
	public int photoCount;	
	
	@SerializedName("video_count")
	public int videoCount;
	
	@SerializedName("audio_count")
	public int audioCount;
	
	public String wsLocation;
		
}
