package com.waveface.favoriteplayer.db;

import android.content.Context;
import android.database.Cursor;
import android.net.Uri;
import android.provider.BaseColumns;

import com.waveface.favoriteplayer.provider.PlayerProvider;

public class LabelTable implements BaseColumns {

	public static final String LABELS_NAME = "labels";
	public static final Uri LABELS_URI = Uri.parse("content://"
			+ PlayerProvider.AUTHORITY + "/" + LABELS_NAME);
	public static final Uri CONTENT_URI =  LABELS_URI;
	public static final String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.favoritplayer.labels";
	public static final String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.favoritplayer.labels";
	public static final String DEFAULT_SORT_ORDER = LabelTable.COLUMN_SEQ+" DESC";

	public static final String TABLE_NAME = "Labels";
	public static final String COLUMN_LABEL_ID = "id";	
	public static final String COLUMN_LABEL_NAME = "labelName";
	public static final String COLUMN_SEQ = "seq";
	public static final String COLUMN_SERVER_SEQ = "serverSeq";
	public static final String COLUMN_UPDATE_TIME = "updateTime";
	public static final String COLUMN_COVER_URL = "coverUrl";
	public static final String COLUMN_AUTO_TYPE = "autoType";
	// auto_type ; 0:favorite ; 1:Recent photo today  ; 2: Recent photo yesterday ; 3: Recent photo this week  
	// 4:Recent video today  ; 5: Recent video yesterday ; 6: Recent video this week
	public static final String COLUMN_ON_AIR = "onAir";
	public static final String COLUMN_DISPLAY_STATUS = "displayStatus";
	
	private LabelTable() {

	}
	

	
	public static int getLabelCount(Context context) {
		Cursor cursor = context.getContentResolver().query(
				LabelTable.CONTENT_URI, 
				new String[] {LabelTable.COLUMN_LABEL_ID}, 
				null, 
				null, null);
		int count = 0;
		if(cursor != null && cursor.moveToFirst())
			count = cursor.getCount();
		if (cursor != null)
			cursor.close();
		return count;
	}
	
	
	public static Cursor getLabels(Context context) {
		Cursor cursor = context.getContentResolver().query(
				LabelTable.CONTENT_URI, 
				null,
				null, 
				null,null);
		return cursor;
	}
}
