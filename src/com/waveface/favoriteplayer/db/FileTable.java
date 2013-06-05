package com.waveface.favoriteplayer.db;

import android.net.Uri;
import android.provider.BaseColumns;

import com.waveface.favoriteplayer.provider.PlayerProvider;

public class FileTable implements BaseColumns {

	public static final String FILES_NAME = "files";
	public static final Uri FILE_URI = Uri.parse("content://"
			+ PlayerProvider.AUTHORITY + "/" + FILES_NAME);
	public static final Uri CONTENT_URI =  FILE_URI;
	public static final String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.favoritplayer.files";
	public static final String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.favoritplayer.files";
	public static final String DEFAULT_SORT_ORDER = FileTable.FILES_NAME+" DESC";

	public static final String TABLE_NAME = "Files";
	public static final String COLUMN_FILE_ID = "fileId";
	public static final String COLUMN_FILE_NAME = "fileName";
	public static final String COLUMN_FOLDER = "folder";
	public static final String COLUMN_THUMB_READY = "thumb_ready";
	public static final String COLUMN_TYPE = "type";//0:phot,1:video
	public static final String COLUMN_DEV_ID = "dev_id";
	public static final String COLUMN_DEV_NAME = "dev_name";
	public static final String COLUMN_DEV_TYPE = "dev_type";
	public static final String COLUMN_WIDTH="width";
	public static final String COLUMN_HEIGHT="height";
	public static final String COLUMN_EVENT_TIME="event_time";
	public static final String COLUMN_STATUS="status";
	public static final String COLUMN_ORIENTATION="orientation";
	
//	public static int updateLabelFiles(Context context, String labelId,String[] files) {
//		int result = 0;
//		ContentResolver cr = context.getContentResolver();
//		try {
//		    for(String file: files){
//			ContentValues cv = new ContentValues();
//			cv.put(FileTable.COLUMN_LABEL_ID, labelId);
//			cv.put(FileTable.COLUMN_FILE_ID, file);
//			result = cr.bulkInsert(FileTable.CONTENT_URI,
//						new ContentValues[] { cv });
//			}	
//			
//		} catch (Exception e) {
//			e.printStackTrace();
//		}
//		return result;
//		}
//	
//	public static Cursor getLableFiles(Context context, String labelId) {
//		Cursor cursor = context.getContentResolver().query(
//				FileTable.CONTENT_URI, 
//				null,
//				FileTable.COLUMN_LABEL_ID+" = ?", 
//				new String[] {labelId}, null);
//		return cursor;
//	}
}
