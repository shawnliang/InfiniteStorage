package com.waveface.sync.entity;

import java.io.Serializable;

public class ServerEntity implements Serializable{
	private static final long serialVersionUID = -9171255717746612838L;
	public String serverId;
	public String serverName;
	public String serverOS;	
	public String status;	
	public String startDatetime;	
	public String endDatetime;		
	public String Folder;	
	public String freespace;	
	public String photoCount;	
	public String videoCount;
	public String audioCount;
	public String wsLocation;
}
