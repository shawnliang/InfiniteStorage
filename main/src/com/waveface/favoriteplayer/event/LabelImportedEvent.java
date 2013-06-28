package com.waveface.favoriteplayer.event;

public class LabelImportedEvent {
	public static final int STATUS_DONE = 0;
	public static final int STATUS_SETTING = 1;
	public static final int STATUS_SYNCING = 2;
	public int status = -1;
	public int offset = -1;
	public int currentIndex = -1;
	public int totalFile = -1;
	public long singleTime = 0;
	
	public LabelImportedEvent(int status) {
		this.status = status;
	}
}
