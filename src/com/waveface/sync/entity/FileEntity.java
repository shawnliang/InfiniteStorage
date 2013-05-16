package com.waveface.sync.entity;

import java.util.ArrayList;
import java.util.List;

import com.google.gson.annotations.SerializedName;

public class FileEntity {
	
	@SerializedName("files")
	public List<File> files = new ArrayList<File>();
	
public class File{	
	@SerializedName("id")
	public String id;

	@SerializedName("file_name")
	public String file_name;

	@SerializedName("folder")
	public String folder;

	@SerializedName("thumb_ready")
	public String thumb_ready;

	@SerializedName("type")
	public String type;

	@SerializedName("dev_id")
	public String dev_id;
	
	@SerializedName("dev_name")
	public String dev_name;
	
	@SerializedName("dev_type")
	public String dev_type;

	@SerializedName("width")
	public String width;
	
	@SerializedName("height")
	public String height;
	
	@SerializedName("size")
	public String size;
	
	@SerializedName("deleted")
	public String deleted;
	
	@SerializedName("seq")
	public String seq;
}

}
