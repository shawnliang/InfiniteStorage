package com.waveface.sync.event;

public class LabelImportedEvent {
	public static final int STATUS_DONE = 0;
	public static final int STATUS_SYNCING = 1;
	public int labelCount;
	public int status;
	
	public LabelImportedEvent(int count) {
		labelCount = count;
	}
}
