package com.waveface.sync.entity;

import com.google.gson.annotations.SerializedName;

public class LabelChangeEntity {
	
	
	@SerializedName("label_change")
	public Label label_change;

	
	
	
	public class Label{
	
	@SerializedName("label_id")
	public String label_id;

	@SerializedName("seq")
	public String seq;

	@SerializedName("label_name")
	public String label_name;
	
	@SerializedName("deleted")
	public String deleted;
	
	@SerializedName("cover_url")
	public String cover_url;
	
	@SerializedName("auto_type")
	public String auto_type;

	
	}

}
