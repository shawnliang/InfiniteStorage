package com.waveface.sync.db;

import android.content.ContentResolver;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.net.Uri;
import android.provider.BaseColumns;
import android.util.Log;

import com.waveface.sync.entity.LabelEntity;
import com.waveface.sync.provider.SyncProvider;

public class LabelFileTable implements BaseColumns {

	public static final String LABELFILE_NAME = "labelFile";
	public static final Uri LABELFILE_URI = Uri.parse("content://"
			+ SyncProvider.AUTHORITY + "/" + LABELFILE_NAME);
	public static final Uri CONTENT_URI =  LABELFILE_URI;
	public static final String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.wammer.sync.labelfile";
	public static final String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.wammer.sync.labelfile";
	public static final String DEFAULT_SORT_ORDER = LabelFileTable.LABELFILE_NAME+" DESC";

	public static final String TABLE_NAME = "Labelfile";
	public static final String COLUMN_LABEL_ID = "labelId";	
	public static final String COLUMN_FILE_ID = "fileId";
	
	private LabelFileTable() {

	}
	
	public static int updateLabelFiles(Context context, String labelId,String[] files) {
		int result = 0;
		ContentResolver cr = context.getContentResolver();
		try {
		    for(String file: files){
			ContentValues cv = new ContentValues();
			cv.put(LabelFileTable.COLUMN_LABEL_ID, labelId);
			cv.put(LabelFileTable.COLUMN_FILE_ID, file);
			result = cr.bulkInsert(LabelFileTable.CONTENT_URI,
						new ContentValues[] { cv });
			}	
			
		} catch (Exception e) {
			e.printStackTrace();
		}
		return result;
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