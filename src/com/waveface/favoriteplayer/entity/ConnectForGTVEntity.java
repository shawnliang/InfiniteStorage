package com.waveface.favoriteplayer.entity;


import com.google.gson.annotations.SerializedName;

public class ConnectForGTVEntity {
	
	@SerializedName("connect")
    private Connect connect;
	
	@SerializedName("subscribe")
    private Subscribe subscribe;	
	
	
	public Connect getConnect() {
        return connect;
    }

    public void setConnect(Connect connect) {
        this.connect= connect;
    }
	public Subscribe getSubscribe() {
        return subscribe;
    }

    public void setSubscribe(Subscribe subscribe) {
        this.subscribe= subscribe;
    }	
	
	public static class Connect{
		
		@SerializedName("device_id")
		public String deviceId;

		@SerializedName("device_name")
		public String deviceName;
		
	}
	
	public static class Subscribe{
		@SerializedName("labels")
		public boolean labels;
		
		@SerializedName("labels_from_seq")
		public String labels_from_seq;

	}

}

