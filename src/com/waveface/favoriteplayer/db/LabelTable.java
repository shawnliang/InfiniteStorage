package com.waveface.favoriteplayer.db;

import android.content.ContentResolver;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.net.Uri;
import android.provider.BaseColumns;


import com.waveface.favoriteplayer.entity.LabelEntity;
import com.waveface.favoriteplayer.provider.SyncProvider;

public class LabelTable implements BaseColumns {

	public static final String LABELS_NAME = "labels";
	public static final Uri LABELS_URI = Uri.parse("content://"
			+ SyncProvider.AUTHORITY + "/" + LABELS_NAME);
	public static final Uri CONTENT_URI =  LABELS_URI;
//	public static final Uri BONJOUR_NOTIFY_URI = Uri.parse("content://"
//			+ SyncProvider.AUTHORITY + ".update." + BONJOUR_SERVERS_NAME);

	public static final String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.wammer.sync.labels";
	public static final String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.wammer.sync.labels";
	public static final String DEFAULT_SORT_ORDER = LabelTable.COLUMN_LABEL_NAME+" DESC";

	public static final String TABLE_NAME = "Labels";
	public static final String COLUMN_LABEL_ID = "id";	
	public static final String COLUMN_LABEL_NAME = "labelName";
	public static final String COLUMN_SEQ = "seq";
	
	private LabelTable() {

	}
	
	public static int updateLabel(Context context, String labelId,String labelName,String seq) {
		int result = 0;
		
		ContentResolver cr = context.getContentResolver();
		try {
		
			ContentValues cv = new ContentValues();
			cv.put(LabelTable.COLUMN_LABEL_ID, labelId);
			cv.put(LabelTable.COLUMN_LABEL_NAME, labelName);
			cv.put(LabelTable.COLUMN_SEQ, seq);
			// insert label
			result = cr.bulkInsert(LabelTable.CONTENT_URI,
						new ContentValues[] { cv });
		
		} catch (Exception e) {
			e.printStackTrace();
		}
		return result;
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
