package com.waveface.sync.entity;

import com.google.gson.annotations.SerializedName;

public class FilesIndexEntity {

	@SerializedName("action")
	public String action;
	
	public FileEntity[] files;
	
	@SerializedName("dup_files")
	public String[] dupFiles;
}
