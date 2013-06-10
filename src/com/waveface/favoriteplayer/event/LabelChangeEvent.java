package com.waveface.favoriteplayer.event;

public class LabelChangeEvent {
	public String labelId;
	public int autoType;
	
	public LabelChangeEvent(String labelId, String autoType) {
		this.labelId = labelId;
		this.autoType = Integer.parseInt(autoType);
	}
}
