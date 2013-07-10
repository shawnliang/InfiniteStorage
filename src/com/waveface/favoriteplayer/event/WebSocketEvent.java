package com.waveface.favoriteplayer.event;

public class WebSocketEvent {
	public static final int STATUS_NONE = 0;
	public static final int STATUS_CONNECT = 1;
	public static final int STATUS_DISCONNECT = 2;
	public int status = STATUS_NONE;
	
	public WebSocketEvent(int status) {
		this.status = status;
	}
}
