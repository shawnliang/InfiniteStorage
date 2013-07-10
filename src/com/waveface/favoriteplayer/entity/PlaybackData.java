package com.waveface.favoriteplayer.entity;

import android.os.Parcel;
import android.os.Parcelable;

public class PlaybackData implements Parcelable{
	public String url;
	public String type;
	public String orientation;
	public String fileId;
	
	public static final Parcelable.Creator<PlaybackData> CREATOR = new Creator<PlaybackData>() {

		@Override
		public PlaybackData[] newArray(int size) {
			return new PlaybackData[size];
		}

		@Override
		public PlaybackData createFromParcel(Parcel source) {
			return new PlaybackData(source);
		}
	};
	
	public PlaybackData() {
		url = null;
		type = "";
		orientation = "";
		fileId="";
	}
	
	public PlaybackData(Parcel in) {
		url = in.readString();
		type = in.readString();
		orientation = in.readString();
		fileId=in.readString();
	}

	@Override
	public int describeContents() {
		return 0;
	}

	@Override
	public void writeToParcel(Parcel out, int flags) {
		out.writeString(url);
		out.writeString(type);
		out.writeString(orientation);	
		out.writeString(fileId);
	}
}
