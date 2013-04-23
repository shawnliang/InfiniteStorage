package com.waveface.sync.entity;

import com.google.gson.annotations.SerializedName;

public class FileBackupEntity {
	
	@SerializedName("action")
	public String action;

	@SerializedName("file_name")
	public String fileName;

	@SerializedName("file_size")
	public String fileSize;
	
	@SerializedName("folder")
	public String folder;
	
	@SerializedName("type")
	public String type;
	
	@SerializedName("datetime")
	public String datetime;
}
