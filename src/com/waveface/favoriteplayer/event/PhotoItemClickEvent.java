package com.waveface.favoriteplayer.event;

import java.util.ArrayList;

import com.waveface.favoriteplayer.entity.PlaybackData;

public class PhotoItemClickEvent {
	public ArrayList<PlaybackData> datas;
	public int position = 0;
	public String labelId;
}
