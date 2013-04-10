package com.waveface.sync.entity;

import com.google.gson.annotations.SerializedName;

public class FIleTransferEntity {
	
	@SerializedName("action")
	public String action;

	@SerializedName("file_name")
	public String fileName;

	@SerializedName("file_size")
	public String fileSize;
	
	@SerializedName("folder")
	public String folder;
}
