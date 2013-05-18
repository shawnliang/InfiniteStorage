package com.waveface.sync.db;

import java.util.ArrayList;
import java.util.Arrays;

import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.util.Log;

import com.waveface.sync.entity.FileEntity;
import com.waveface.sync.entity.LabelEntity;

public class LabelDB {

	public static String TAG = "LabelDB";

	public static void updateLabelInfo(Context context,
			LabelEntity.Label label, FileEntity fileEntity) {

		// deleteLabel(context,label.label_id);
		updateLabel(context, label.label_id, label.label_name, label.seq);
//		removeAllFileInLabel(context, label.label_id);
		updateLabelFiles(context, label);
		updateFiles(context, fileEntity);
	}

	public static int updateLabel(Context context, String labelId,
			String labelName, String seq) {
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

	public static int updateLabelFiles(Context context, LabelEntity.Label label) {
		int result = 0;
		int order = 0;
		ContentResolver cr = context.getContentResolver();
		ArrayList<ContentValues> datas = new ArrayList<ContentValues>();		
		try {
			if (label.files.length > 0) {
				for (String file : label.files) {
					ContentValues cv = new ContentValues();
					cv.put(LabelFileTable.COLUMN_LABEL_ID, label.label_id);
					cv.put(LabelFileTable.COLUMN_FILE_ID, file);
					order += 1;
					cv.put(LabelFileTable.COLUMN_ORDER, order);
					datas.add(cv);
				}
				ContentValues[] cvs = new ContentValues[datas.size()];
				cvs = datas.toArray(cvs);
				result = cr.bulkInsert(LabelFileTable.CONTENT_URI,cvs);
			}

		} catch (Exception e) {
			e.printStackTrace();
		}
		return result;
	}

	public static int updateFiles(Context context, FileEntity fileEntity) {
		int result = 0;
		ContentResolver cr = context.getContentResolver();
		ArrayList<ContentValues> datas = new ArrayList<ContentValues>();
		try {
			if (fileEntity.files.size() > 0) {
				for (FileEntity.File file : fileEntity.files) {
					ContentValues cv = new ContentValues();
					cv.put(FileTable.COLUMN_FILE_ID, file.id);
					cv.put(FileTable.COLUMN_FILE_NAME, file.file_name);
					cv.put(FileTable.COLUMN_FOLDER, file.folder);
					cv.put(FileTable.COLUMN_THUMB_READY, file.thumb_ready);
					cv.put(FileTable.COLUMN_TYPE, file.type);
					cv.put(FileTable.COLUMN_DEV_ID, file.dev_id);
					cv.put(FileTable.COLUMN_DEV_NAME, file.dev_name);
					cv.put(FileTable.COLUMN_DEV_TYPE, file.dev_type);
					datas.add(cv);
				}
				ContentValues[] cvs = new ContentValues[datas.size()];
				cvs = datas.toArray(cvs);
				result = cr.bulkInsert(FileTable.CONTENT_URI,cvs);
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		return result;
	}

	public static void deleteLabel(Context context, String labelId) {
		ContentResolver cr = context.getContentResolver();
		removeAllFileInLabel(context, labelId);
		Cursor cursor = cr.query(LabelTable.CONTENT_URI,
				new String[] { LabelTable.COLUMN_LABEL_ID },
				LabelTable.COLUMN_LABEL_ID + " = ?", new String[] { labelId },
				null);
		cursor.moveToFirst();
		if (cursor.getCount() == 1) {
			String label = cursor.getString(0);
			cr.delete(LabelTable.CONTENT_URI,
					LabelTable.COLUMN_LABEL_ID + "=?", new String[] { label });
		}
		cursor.close();
	}

	private static int removeAllFileInLabel(Context context, String labelId) {
		return context.getContentResolver().delete(LabelFileTable.CONTENT_URI, LabelFileTable.COLUMN_LABEL_ID
				+ "=?", new String[] { labelId });
	}

	public static Cursor getAllLabes(Context context) {
		Cursor cursor = context.getContentResolver().query(
				LabelTable.CONTENT_URI,
				new String[] { LabelTable.COLUMN_LABEL_ID,
						LabelTable.COLUMN_LABEL_NAME }, null, null, null);

		return cursor;
	}
	


	public static Cursor getLabelByLabelId(Context context, String labelId) {

		Cursor cursor = context.getContentResolver().query(
				LabelTable.CONTENT_URI,
				new String[] { LabelTable.COLUMN_LABEL_ID,
						LabelTable.COLUMN_LABEL_NAME },
				LabelTable.COLUMN_LABEL_ID + " = ?", new String[] { labelId },
				null);

		return cursor;
	}

	public static Cursor getFilesByLabelId(Context context, String labelId,
			int limit) {
		Cursor cursor = context.getContentResolver().query(
				LabelFileView.CONTENT_URI,
				new String[] { 
						LabelFileView.COLUMN_LABEL_ID,
						LabelFileView.COLUMN_FILE_ID,
						LabelFileView.COLUMN_ORDER,
						LabelFileView.COLUMN_FILE_NAME,
						LabelFileView.COLUMN_FOLDER,
						LabelFileView.COLUMN_THUMB_READY,
						LabelFileView.COLUMN_TYPE, 
						LabelFileView.COLUMN_DEV_ID,
						LabelFileView.COLUMN_DEV_NAME,
						LabelFileView.COLUMN_DEV_TYPE },
				LabelFileView.COLUMN_LABEL_ID + " = ?",
				new String[] { labelId },
				LabelFileView.DEFAULT_SORT_ORDER + "  LIMIT " + limit);

		return cursor;
	}
	public static Cursor getLabelFilesByLabelId(Context context, String labelId) {
		Cursor cursor = context.getContentResolver().query(
				LabelFileTable.CONTENT_URI,
				null,
				LabelFileTable.COLUMN_LABEL_ID + " = ?",
				new String[] { labelId },
				LabelFileTable.DEFAULT_SORT_ORDER);
		return cursor;
	}

}
