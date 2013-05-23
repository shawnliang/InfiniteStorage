package com.waveface.favoriteplayer.db;

import android.content.Context;
import android.database.Cursor;
import android.net.Uri;
import android.provider.BaseColumns;

import com.waveface.favoriteplayer.provider.PlayerProvider;

public class LabelFileTable implements BaseColumns {

	public static final String LABELFILE_NAME = "labelFile";
	public static final Uri LABELFILE_URI = Uri.parse("content://"
			+ PlayerProvider.AUTHORITY + "/" + LABELFILE_NAME);
	public static final Uri CONTENT_URI =  LABELFILE_URI;
	public static final String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.favoritplayer.labelfile";
	public static final String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.favoritplayer.labelfile";
	public static final String DEFAULT_SORT_ORDER = LabelFileTable.COLUMN_ORDER;

	public static final String TABLE_NAME = "Labelfile";
	public static final String COLUMN_LABEL_ID = "labelId";	
	public static final String COLUMN_FILE_ID = "fileId";
	public static final String COLUMN_ORDER = "orders";
	private LabelFileTable() {

	}
	

	
	public static Cursor getLableFiles(Context context, String labelId) {
		Cursor cursor = context.getContentResolver().query(
				LabelFileTable.CONTENT_URI, 
				null,
				LabelFileTable.COLUMN_LABEL_ID+" = ?", 
				new String[] {labelId}, null);
		return cursor;
	}
}
