package com.waveface.favoriteplayer.entity;

import android.os.Parcel;
import android.os.Parcelable;


public class OverviewData implements Parcelable{
	public String url;
	public boolean landscape;
	public String title;
	public String labelId;
	public String type;	
	public String filename;		
	public int count;
	
	public static final Parcelable.Creator<OverviewData> CREATOR = new Creator<OverviewData>() {

		@Override
		public OverviewData[] newArray(int size) {
			return new OverviewData[size];
		}

		@Override
		public OverviewData createFromParcel(Parcel source) {
			return new OverviewData(source);
		}
	};
	
	public OverviewData() {
		url = null;
		landscape = false;
		title = null;
		labelId = null;
		type = null;
		filename = null;
		count = 0;
	}
	
	public OverviewData(Parcel in) {
		url = in.readString();
		landscape = in.readInt() == 1 ? true:false;
		title = in.readString();
		labelId = in.readString();
		type = in.readString();
		filename = in.readString();
		count = in.readInt();
	}
	
	@Override
	public int describeContents() {
		return 0;
	}
	@Override
	public void writeToParcel(Parcel out, int flags) {
		out.writeString(url);
		out.writeInt(landscape?1:0);
		out.writeString(title);
		out.writeString(labelId);
		out.writeString(type);		
		out.writeString(filename);				
		out.writeInt(count);
	}
}

