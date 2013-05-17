package com.waveface.sync.entity;



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
	
	
	
	public String[] files;
	}
	
}
