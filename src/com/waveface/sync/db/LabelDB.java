package com.waveface.sync.db;


import com.waveface.sync.entity.FileEntity;
import com.waveface.sync.entity.LabelEntity;
import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.util.Log;




public class LabelDB {
	
	public static String TAG ="LabelDB";
	
	
	
	public static void updateLabelInfo(Context context,LabelEntity.Label label,FileEntity fileEntity){
		
		
		deleteLabel(context,label.label_id);
		updateLabel(context, label.label_id,label.label_name);
		updateLabelFiles(context,label);
		updateFiles(context,fileEntity);
	}
	
	
	public static int updateLabel(Context context, String labelId,String labelName) {
		int result = 0;
		
		ContentResolver cr = context.getContentResolver();
		try {
		
			ContentValues cv = new ContentValues();
			cv.put(LabelTable.COLUMN_LABEL_ID, labelId);
			cv.put(LabelTable.COLUMN_LABEL_NAME, labelName);

			// insert label
			result = cr.bulkInsert(LabelTable.CONTENT_URI,
						new ContentValues[] { cv });
		
		} catch (Exception e) {
			e.printStackTrace();
		}
		return result;
	}
	
	
	public static int updateLabelFiles(Context context,LabelEntity.Label label) {
		int result = 0;
		int order =0;
		ContentResolver cr = context.getContentResolver();
		try {
			if(label.files.length>0){
		    for(String  file: label.files){
			ContentValues cv = new ContentValues();
			cv.put(LabelFileTable.COLUMN_LABEL_ID, label.label_id);
			cv.put(LabelFileTable.COLUMN_FILE_ID, file);
			order+=order+1;
			cv.put(LabelFileTable.COLUMN_ORDER, order);
			result = cr.bulkInsert(LabelFileTable.CONTENT_URI,
						new ContentValues[] { cv });
			}
		}
			
		} catch (Exception e) {
			e.printStackTrace();
		}
		return result;
		}
	
	public static int updateFiles(Context context,FileEntity fileEntity ) {
		int result = 0;
		ContentResolver cr = context.getContentResolver();
		try {
			
		    for(FileEntity.File file: fileEntity.files){
			ContentValues cv = new ContentValues();
			cv.put(FileTable.COLUMN_FILE_ID, file.id);
			cv.put(FileTable.COLUMN_FILE_NAME, file.file_name);
			cv.put(FileTable.COLUMN_FOLDER, file.folder);
			cv.put(FileTable.COLUMN_THUMB_READY, file.thumb_ready);
			cv.put(FileTable.COLUMN_TYPE, file.type);
			cv.put(FileTable.COLUMN_DEV_ID, file.dev_id);
			cv.put(FileTable.COLUMN_DEV_NAME, file.dev_name);
			cv.put(FileTable.COLUMN_DEV_TYPE, file.type);
			result = cr.bulkInsert(FileTable.CONTENT_URI,
						new ContentValues[] { cv });
			}	
			
		} catch (Exception e) {
			e.printStackTrace();
		}
		return result;
		}
	
	
	public static void deleteLabel(Context context, String labelId) {
		ContentResolver cr = context.getContentResolver();
		removeAllFileInLabel(context, labelId);
		Cursor cursor = cr.query(
				LabelTable.CONTENT_URI, 
				new String[] {LabelTable.COLUMN_LABEL_ID},
				LabelTable.COLUMN_LABEL_ID+" = ?", 
				new String[] {labelId}, null);
		cursor.moveToFirst();
		if (cursor.getCount()==1) {			
			String label = cursor.getString(0);
			cr.delete(LabelTable.CONTENT_URI, LabelTable.COLUMN_LABEL_ID+"=?", new String[]{label});
		}
		cursor.close();
	}
	
	
	private static void removeAllFileInLabel(Context context, String labelId) {
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = cr.query(
				LabelFileTable.CONTENT_URI, 
				new String[] {LabelFileTable.COLUMN_FILE_ID},
				LabelFileTable.COLUMN_LABEL_ID+" = ?", 
				new String[] {labelId}, null);
		while (cursor!=null && cursor.moveToNext()) {
			cr.delete(FileTable.CONTENT_URI, 
					FileTable.COLUMN_FILE_ID+"=?", 
					new String[]{cursor.getString(0)});
			Log.v(TAG, "delete file "+cursor.getString(0));
		}
		if (cursor!=null)
			cursor.close();
		cr.delete(LabelFileTable.CONTENT_URI, 
				LabelFileTable.COLUMN_LABEL_ID+"=?", 
				new String[]{labelId});

	}
	
	
	
	
	public static Cursor getAllLabes(Context context) {
		
		Cursor cursor = context.getContentResolver()
				.query(LabelTable.CONTENT_URI,
						new String[]{LabelTable.COLUMN_LABEL_ID,
						LabelTable.COLUMN_LABEL_NAME },
						null,
						null,
						null);
		
		return cursor;
	}
	
	
	
	public static Cursor getLabelByLabelId(Context context,String labelId) {
		
		Cursor cursor = context.getContentResolver()
				.query(LabelTable.CONTENT_URI,
						new String[]{LabelTable.COLUMN_LABEL_ID,
						LabelTable.COLUMN_LABEL_NAME },
						LabelTable.COLUMN_LABEL_ID+" = ?",
						new String[]{labelId},
						null);
		
		return cursor;
	}
	
	
	
	public static Cursor getFilesByLabelId(Context context,String labelId, int limit) {
		
		Cursor cursor = context.getContentResolver()
				.query(LabelFileView.CONTENT_URI,
						new String[]{LabelFileView.COLUMN_LABEL_ID,
						LabelFileView.COLUMN_FILE_ID,
						LabelFileView.COLUMN_ORDER,
						LabelFileView.COLUMN_FILE_NAME,
						LabelFileView.COLUMN_FOLDER ,
						LabelFileView.COLUMN_THUMB_READY,
						LabelFileView.COLUMN_TYPE,
						LabelFileView.COLUMN_DEV_ID ,
						LabelFileView.COLUMN_DEV_NAME,
						LabelFileView.COLUMN_DEV_TYPE },
						LabelFileView.COLUMN_LABEL_ID+" = ?",
						new String[]{labelId},
						LabelFileView.COLUMN_ORDER+"  DESC LIMIT "+limit);
		
		return cursor;
	}

}
