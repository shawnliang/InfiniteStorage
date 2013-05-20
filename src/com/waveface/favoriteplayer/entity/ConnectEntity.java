package com.waveface.favoriteplayer.entity;

import com.google.gson.annotations.SerializedName;

public class ConnectEntity {

	@SerializedName("action")
	public String action;

	@SerializedName("device_name")
	public String deviceName;

	@SerializedName("device_id")
	public String deviceId;

	@SerializedName("transfer_count")
	public long transferCount;
	
	@SerializedName("transfer_size")
	public long transferSize;

	
}
