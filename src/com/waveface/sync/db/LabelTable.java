package com.waveface.sync.db;

import android.content.ContentResolver;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.net.Uri;
import android.provider.BaseColumns;

import com.waveface.sync.entity.LabelEntity;
import com.waveface.sync.provider.SyncProvider;

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
	
	private LabelTable() {

	}
	
	public static int updateLabel(Context context, LabelEntity entity) {
		int result = 0;
		
		ContentResolver cr = context.getContentResolver();
		try {
		
			ContentValues cv = new ContentValues();
			cv.put(LabelTable.COLUMN_LABEL_ID, entity.label_id);
			cv.put(LabelTable.COLUMN_LABEL_NAME, entity.label_name);

			// insert label
			result = cr.bulkInsert(LabelTable.CONTENT_URI,
						new ContentValues[] { cv });
		
		} catch (Exception e) {
			e.printStackTrace();
		}
		return result;
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