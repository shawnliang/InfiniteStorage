package com.waveface.favoriteplayer.entity;

import android.os.Parcel;
import android.os.Parcelable;

public class VideoData implements Parcelable{
	public String url;
	
	public static final Parcelable.Creator<VideoData> CREATOR = new Creator<VideoData>() {

		@Override
		public VideoData[] newArray(int size) {
			return new VideoData[size];
		}

		@Override
		public VideoData createFromParcel(Parcel source) {
			return new VideoData(source);
		}
	};
	
	public VideoData() {
		url = null;
	}
	
	public VideoData(Parcel in) {
		url = in.readString();
	}
	
	@Override
	public int describeContents() {
		return 0;
	}
	@Override
	public void writeToParcel(Parcel out, int flags) {
		out.writeString(url);
	}
}