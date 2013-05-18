package com.waveface.sync.event;

public class LabelImportedEvent {
	public static final int STATUS_DONE = 0;
	public static final int STATUS_SYNCING = 1;
	public int status;
	public int currentFile;
	public int totalFile;
	public long singleTime;
	
	public LabelImportedEvent(int status) {
		this.status = status;
	}
}
