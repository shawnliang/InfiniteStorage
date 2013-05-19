package com.waveface.favoriteplayer.entity;

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

	@SerializedName("total_count")
	public long totalCount;

	@SerializedName("backuped_count")
	public long backupedCount;
}
