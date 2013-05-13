package com.waveface.sync.entity;

import com.google.gson.annotations.SerializedName;

public class LabelEntity {

	@SerializedName("label_id")
	public String label_id;

	@SerializedName("files")
	public String[] files;
	
	@SerializedName("label_name")
	public String label_name;
}
