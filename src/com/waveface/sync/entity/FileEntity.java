package com.waveface.sync.entity;

import com.google.gson.annotations.SerializedName;

public class FileEntity {
	@SerializedName("file_path")
	public String filePath;

	@SerializedName("file_size")
	public String fileSize;

}
