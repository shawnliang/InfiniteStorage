package com.waveface.favoriteplayer.db;

import java.util.ArrayList;



import java.util.Date;

import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.text.TextUtils;


import com.waveface.favoriteplayer.entity.FileEntity;
import com.waveface.favoriteplayer.entity.LabelEntity;
import com.waveface.favoriteplayer.util.StringUtil;




public class LabelDB {

	public static String TAG = "LabelDB";

	public static void updateLabelInfo(Context context,
			LabelEntity.Label label, FileEntity fileEntity ,boolean isChangeLabel) {

		if(!isChangeLabel){
			updateLabel(context, label);
			//removeAllFileInLabel(context, label.label_id);
			updateLabelFiles(context, label);
			updateFiles(context, fileEntity);
		}else{
			updateLabelSeq(context,label.label_id,label.seq);
			removeAllFileInLabel(context, label.label_id);
			updateLabelFiles(context, label);
			updateFiles(context, fileEntity);			
		}
	}

	public static int updateLabel(Context context, LabelEntity.Label label) {
		int result = 0;

		ContentResolver cr = context.getContentResolver();
		try {

			ContentValues cv = new ContentValues();
			cv.put(LabelTable.COLUMN_LABEL_ID, label.label_id);
			cv.put(LabelTable.COLUMN_LABEL_NAME, label.label_name);
			cv.put(LabelTable.COLUMN_SEQ, label.seq);
			cv.put(LabelTable.COLUMN_UPDATE_TIME, StringUtil.localtimeToIso8601(new Date()));
			if(TextUtils.isEmpty(label.cover_url)){
				label.cover_url = "";
			}
			cv.put(LabelTable.COLUMN_COVER_URL, label.cover_url);
			if(TextUtils.isEmpty(label.auto_type)){
				label.auto_type = "";
			}			
			cv.put(LabelTable.COLUMN_AUTO_TYPE, label.auto_type);
			
			if(TextUtils.isEmpty(label.on_air)){
				label.on_air = "";
			}
			cv.put(LabelTable.COLUMN_ON_AIR, label.on_air);
			
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
					cv.put(FileTable.COLUMN_WIDTH, file.width);
					cv.put(FileTable.COLUMN_HEIGHT, file.height);
					cv.put(FileTable.COLUMN_EVENT_TIME, file.event_time);
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
		//removeAllFileInLabel(context, labelId);
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


	
	private static void removeAllFileInLabel(Context context, String labelId) {
		ContentResolver cr = context.getContentResolver();
		cr.delete(LabelFileTable.CONTENT_URI, 
				LabelFileTable.COLUMN_LABEL_ID+"=?", 
				new String[]{labelId});

	}
	
	
	public static Cursor getPhotoLabelId(Context context) {
		return context.getContentResolver().query(
				LabelTable.CONTENT_URI,
				new String[]{LabelTable.COLUMN_LABEL_ID},
				LabelTable.COLUMN_LABEL_NAME + " != ?",
				new String[] { "videos" },null);
	}

	

	public static Cursor getAllLabels(Context context) {
		Cursor cursor = context.getContentResolver().query(
				LabelTable.CONTENT_URI,
				new String[] { LabelTable.COLUMN_LABEL_ID,
						LabelTable.COLUMN_LABEL_NAME,LabelTable.COLUMN_SEQ }, null, null, null);
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
	
	
	public static Cursor getCategoryLabelByLabelId(Context context, int type) {

		Cursor cursor = context.getContentResolver().query(
				LabelTable.CONTENT_URI,
				new String[] { LabelTable.COLUMN_LABEL_ID,
						LabelTable.COLUMN_COVER_URL,
						LabelTable.COLUMN_LABEL_NAME },
				LabelTable.COLUMN_AUTO_TYPE +"= ? AND "+LabelTable.COLUMN_ON_AIR +" =?" , new String[] { Integer.toString(type),"true" },
				null);

		return cursor;
	}
	
	
	public static Cursor getMAXSEQLabel(Context context) {

		Cursor cursor = context.getContentResolver().query(
				LabelTable.CONTENT_URI,
				new String[] { LabelTable.COLUMN_LABEL_ID,
						LabelTable.COLUMN_LABEL_NAME,LabelTable.COLUMN_SEQ,LabelTable.COLUMN_COVER_URL,LabelTable.COLUMN_AUTO_TYPE },
				null, null,
				LabelTable.COLUMN_SEQ + " DESC LIMIT 1");

		return cursor;
	}

	
	


	public static Cursor getLabelFileViewByLabelId(Context context, String labelId) {
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
						LabelFileView.COLUMN_HEIGHT,
						LabelFileView.COLUMN_WIDTH,
						LabelFileView.COLUMN_DEV_TYPE },
						LabelFileView.COLUMN_LABEL_ID + "=?",
						new String[] { labelId},
						null);

		return cursor;
	}
	
	public static Cursor getAllLabelFileViewByLabelId(Context context) {
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
						LabelFileView.COLUMN_HEIGHT,
						LabelFileView.COLUMN_WIDTH,
						LabelFileView.COLUMN_DEV_TYPE },
						null,
						null,
						null);

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
	public static String getVideoLabelId(Context context) {
		String labelId = null;
		Cursor cursor = context.getContentResolver().query(
				LabelTable.CONTENT_URI,
				new String[]{LabelTable.COLUMN_LABEL_ID},
				LabelTable.COLUMN_LABEL_NAME + " = ?",
				new String[] { "videos" },null);
		if(cursor!=null && cursor.getCount()>0){
			cursor.moveToFirst();
			labelId = cursor.getString(0);
		}
		cursor.close();
		return labelId;
	}
	
	public static Cursor getLabeFilelCountByLabelId(Context context, String labelId) {

		Cursor cursor = context.getContentResolver().query(
				LabelFileTable.CONTENT_URI,
				new String[] { LabelFileTable.COLUMN_LABEL_ID,
						LabelFileTable.COLUMN_FILE_ID },
						LabelFileTable.COLUMN_LABEL_ID + " = ?", new String[] { labelId },
				null);
				return cursor;
	}
	
	private static void removeLabelFileByFileId(Context context, String fileId) {
		ContentResolver cr = context.getContentResolver();
		cr.delete(LabelFileTable.CONTENT_URI, 
				LabelFileTable.COLUMN_FILE_ID+"=?", 
				new String[]{fileId});
	}
	
	public static int updateLabelSeq(Context context, String labelId,String seq) {
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(LabelTable.COLUMN_SEQ, Integer.parseInt(seq));

		return cr.update(LabelTable.CONTENT_URI, cv, LabelTable.COLUMN_LABEL_ID + "=?",
				new String[] { labelId });
	}
}
