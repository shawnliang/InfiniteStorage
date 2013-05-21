package com.waveface.favoriteplayer.entity;

import java.util.ArrayList;
import java.util.List;
import com.google.gson.annotations.SerializedName;


public class LabelEntity {

	@SerializedName("labels")
	public List<Label> labels = new ArrayList<Label>();



	public class Label{	
	@SerializedName("label_id")
	public String label_id;

	@SerializedName("seq")
	public String seq;

	@SerializedName("label_name")
	public String label_name;

	@SerializedName("cover_url")
	public String cover_url;
	
	@SerializedName("auto")
	public String auto;

	public String[] files;
	}

}
