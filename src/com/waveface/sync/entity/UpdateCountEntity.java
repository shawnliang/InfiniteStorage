package com.waveface.sync.entity;

import com.google.gson.annotations.SerializedName;

public class UpdateCountEntity {

	@SerializedName("action")
	public String action;

	@SerializedName("transfer_count")
	public long transferCount;

	@SerializedName("backuped_count")
	public long backupedCount;
}
