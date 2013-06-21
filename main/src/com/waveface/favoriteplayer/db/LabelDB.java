package com.waveface.favoriteplayer.db;

import java.util.ArrayList;
import java.util.Date;

import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.text.TextUtils;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.entity.FileEntity;
import com.waveface.favoriteplayer.entity.LabelEntity;
import com.waveface.favoriteplayer.util.StringUtil;
import com.waveface.sync.entity.LabelChangeEntity;

public class LabelDB {

	public static String TAG = "LabelDB";

	public static void updateLabelInfo(Context context,
			LabelEntity.Label label, FileEntity fileEntity,
			boolean isChangeLabel) {
		updateLabel(context, label);
		if (isChangeLabel) {
			if (label.deleted.equals("true")) {
//				deleteLabel(context, label.label_id);
				removeAllFileInLabel(context, label.label_id);
				removeFile(context, label);
			} else {
				// Changed Label
				// delete labelfile in database
				removeAllFileInLabel(context, label.label_id);
				if (label.on_air.equals("false")) {
					// TODO: delete file in db and file's real source(image or
					// video file)
					removeFile(context, label);
				} else {
					updateLabelFiles(context, label);
					updateFiles(context, fileEntity);
				}
			}
		} else {
			// delete labelfile in database
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
			cv.put(LabelTable.COLUMN_SERVER_SEQ, label.seq);
			cv.put(LabelTable.COLUMN_UPDATE_TIME,
					StringUtil.localtimeToIso8601(new Date()));
			if (TextUtils.isEmpty(label.cover_url)) {
				label.cover_url = "";
			}
			cv.put(LabelTable.COLUMN_COVER_URL, label.cover_url);
			if (TextUtils.isEmpty(label.auto_type)) {
				label.auto_type = "";
			}
			cv.put(LabelTable.COLUMN_AUTO_TYPE, label.auto_type);

			if (TextUtils.isEmpty(label.on_air)) {
				label.on_air = "";
			}
			cv.put(LabelTable.COLUMN_ON_AIR, label.on_air);
			if (!TextUtils.isEmpty(label.deleted)
					&& label.deleted.equals("true")) {
				cv.put(LabelTable.COLUMN_DISPLAY_STATUS, "false");
			} else {
				cv.put(LabelTable.COLUMN_DISPLAY_STATUS, "true");
			}
			result = cr.bulkInsert(LabelTable.CONTENT_URI,
					new ContentValues[] { cv });

		} catch (Exception e) {
			e.printStackTrace();
		}
		return result;
	}

	public static int addNewLabelForChangNotify(Context context,
			LabelEntity.Label label) {
		int result = 0;
		ContentResolver cr = context.getContentResolver();
		try {
			ContentValues cv = new ContentValues();
			cv.put(LabelTable.COLUMN_LABEL_ID, label.label_id);
			cv.put(LabelTable.COLUMN_LABEL_NAME, label.label_name);
			cv.put(LabelTable.COLUMN_SEQ, "0");
			cv.put(LabelTable.COLUMN_SERVER_SEQ, label.seq);
			cv.put(LabelTable.COLUMN_UPDATE_TIME,
					StringUtil.localtimeToIso8601(new Date()));
			if (TextUtils.isEmpty(label.cover_url)) {
				label.cover_url = "";
			}
			cv.put(LabelTable.COLUMN_COVER_URL, label.cover_url);
			if (TextUtils.isEmpty(label.auto_type)) {
				label.auto_type = "";
			}
			cv.put(LabelTable.COLUMN_AUTO_TYPE, label.auto_type);
			if (TextUtils.isEmpty(label.on_air)) {
				label.on_air = "true";
			}
			cv.put(LabelTable.COLUMN_ON_AIR, label.on_air);
			if (!TextUtils.isEmpty(label.deleted)
					&& label.deleted.equals("true")) {
				cv.put(LabelTable.COLUMN_DISPLAY_STATUS, "false");
			} else {
				cv.put(LabelTable.COLUMN_DISPLAY_STATUS, "true");
			}
			result = cr.bulkInsert(LabelTable.CONTENT_URI,
					new ContentValues[] { cv });

		} catch (Exception e) {
			e.printStackTrace();
		}
		return result;
	}
	public static int updateLabelByServerChangeNotify(Context context,
			LabelChangeEntity entity) {
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(LabelTable.COLUMN_LABEL_NAME, entity.label_change.label_name);
		if (!TextUtils.isEmpty(entity.label_change.deleted)
				&& entity.label_change.deleted.equals("true")) {
			cv.put(LabelTable.COLUMN_DISPLAY_STATUS, "false");
		} else {
			cv.put(LabelTable.COLUMN_DISPLAY_STATUS, "true");
		}
		cv.put(LabelTable.COLUMN_SERVER_SEQ, entity.label_change.seq);
		cv.put(LabelTable.COLUMN_UPDATE_TIME,
				StringUtil.localtimeToIso8601(new Date()));
		if (TextUtils.isEmpty(entity.label_change.cover_url)) {
			entity.label_change.cover_url = "";
		}		
		cv.put(LabelTable.COLUMN_COVER_URL, entity.label_change.cover_url);
		if (TextUtils.isEmpty(entity.label_change.auto_type)) {
			entity.label_change.auto_type = "";
		}
		cv.put(LabelTable.COLUMN_AUTO_TYPE, entity.label_change.auto_type);
		if (TextUtils.isEmpty(entity.label_change.on_air)) {
			entity.label_change.on_air = "true";
		}
		cv.put(LabelTable.COLUMN_ON_AIR, entity.label_change.on_air);
		return cr.update(LabelTable.CONTENT_URI, cv, LabelTable.COLUMN_LABEL_ID
				+ "=?", new String[] { entity.label_change.label_id });
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
				result = cr.bulkInsert(LabelFileTable.CONTENT_URI, cvs);
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
					cv.put(FileTable.COLUMN_STATUS,
							Constant.FILE_STATUS_NON_DELETE);
					cv.put(FileTable.COLUMN_ORIENTATION,
							file.orientation == null ? "" : file.orientation);
					cv.put(FileTable.COLUMN_ORIGINAL_PATH,
							file.original_path == null ? ""
									: file.original_path);
					datas.add(cv);
				}
				ContentValues[] cvs = new ContentValues[datas.size()];
				cvs = datas.toArray(cvs);
				result = cr.bulkInsert(FileTable.CONTENT_URI, cvs);
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		return result;
	}

	public static void deleteLabel(Context context, String labelId) {
		ContentResolver cr = context.getContentResolver();
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

	public static void removeAllFileInLabel(Context context, String labelId) {
		ContentResolver cr = context.getContentResolver();
		cr.delete(LabelFileTable.CONTENT_URI, LabelFileTable.COLUMN_LABEL_ID
				+ "=?", new String[] { labelId });
	}

	public static Cursor getPhotoLabelId(Context context) {
		return context.getContentResolver().query(LabelTable.CONTENT_URI,
				new String[] { LabelTable.COLUMN_LABEL_ID },
				LabelTable.COLUMN_LABEL_NAME + " != ?",
				new String[] { "videos" }, null);
	}

	public static Cursor getLabelByLabelId(Context context, String labelId) {

		Cursor cursor = context.getContentResolver().query(
				LabelTable.CONTENT_URI,
				new String[] { LabelTable.COLUMN_LABEL_ID,
						LabelTable.COLUMN_LABEL_NAME,
						LabelTable.COLUMN_COVER_URL,
						LabelTable.COLUMN_AUTO_TYPE },
				LabelTable.COLUMN_LABEL_ID + " = ? AND "
						+ LabelTable.COLUMN_ON_AIR + " = ? AND "
						+ LabelTable.COLUMN_DISPLAY_STATUS + "=?",
				new String[] { labelId, "true", "true" }, null);

		return cursor;
	}
	public static boolean hasThisLabel(Context context, String labelId) {
		boolean hasLabelData = false;
		Cursor cursor = context.getContentResolver().query(
				LabelTable.CONTENT_URI,
				new String[] { LabelTable.COLUMN_LABEL_ID},
				LabelTable.COLUMN_LABEL_ID + " = ? ",
				new String[] { labelId}, null);
		if(cursor!=null && cursor.getCount()>0){
			hasLabelData = true;
			cursor.close();
		}
		
		return hasLabelData;
	}
	
	

	public static Cursor getCategoryLabelByLabelId(Context context, int type) {

		Cursor cursor = context.getContentResolver().query(
				LabelTable.CONTENT_URI,
				new String[] { LabelTable.COLUMN_LABEL_ID,
						LabelTable.COLUMN_COVER_URL,
						LabelTable.COLUMN_LABEL_NAME },
				LabelTable.COLUMN_AUTO_TYPE + "= ? AND "
						+ LabelTable.COLUMN_ON_AIR + "= ? AND "
						+ LabelTable.COLUMN_DISPLAY_STATUS + " =? ",
				new String[] { Integer.toString(type), "true", "true" }, null);

		return cursor;
	}

	public static String getMAXServerSeq(Context context) {
		String serverSeq = "0";
		Cursor cursor = context.getContentResolver().query(
				LabelTable.CONTENT_URI,
				new String[] { LabelTable.COLUMN_SERVER_SEQ,
						LabelTable.COLUMN_SEQ, LabelTable.COLUMN_LABEL_NAME },
				null, null, LabelTable.COLUMN_SERVER_SEQ + " DESC LIMIT 1");
		if (cursor != null && cursor.getCount() != 0) {
			cursor.moveToFirst();
			serverSeq = cursor.getString(0);
		}
		cursor.close();
		return serverSeq;
	}

	public static boolean needToSyncLabel(Context context) {
		boolean needToSync = false;
		String whereCLause = LabelTable.COLUMN_SEQ + " <> "
				+ LabelTable.COLUMN_SERVER_SEQ + " AND "
				+ LabelTable.COLUMN_DISPLAY_STATUS + "=?";
		Cursor cursor = context.getContentResolver().query(
				LabelTable.CONTENT_URI,
				new String[] { LabelTable.COLUMN_LABEL_ID,
						LabelTable.COLUMN_LABEL_NAME, LabelTable.COLUMN_SEQ,
						LabelTable.COLUMN_SERVER_SEQ,
						LabelTable.COLUMN_COVER_URL,
						LabelTable.COLUMN_AUTO_TYPE, LabelTable.COLUMN_ON_AIR,
						LabelTable.COLUMN_DISPLAY_STATUS }, 
						whereCLause,
						new String[] { "true" }, 
						LabelTable.COLUMN_SEQ+" LIMIT 1");
		if(cursor!=null && cursor.getCount()>0){
			needToSync = true;
			cursor.close();
		}
		return needToSync;
	}
	
	public static Cursor getUnsyncedLabel(Context context) {

		String whereCLause = LabelTable.COLUMN_SEQ + " <> "
				+ LabelTable.COLUMN_SERVER_SEQ + " AND "
				+ LabelTable.COLUMN_DISPLAY_STATUS + "=?";
		Cursor cursor = context.getContentResolver().query(
				LabelTable.CONTENT_URI,
				new String[] { LabelTable.COLUMN_LABEL_ID,
						LabelTable.COLUMN_LABEL_NAME, LabelTable.COLUMN_SEQ,
						LabelTable.COLUMN_SERVER_SEQ,
						LabelTable.COLUMN_COVER_URL,
						LabelTable.COLUMN_AUTO_TYPE, LabelTable.COLUMN_ON_AIR,
						LabelTable.COLUMN_DISPLAY_STATUS }, whereCLause,
				new String[] { "true" }, LabelTable.COLUMN_SEQ);

		return cursor;
	}

	public static Cursor getLabelFileViewByLabelId(Context context,
			String labelId) {
		Cursor cursor = context.getContentResolver().query(
				LabelFileView.CONTENT_URI,
				new String[] { LabelFileView.COLUMN_LABEL_ID,
						LabelFileView.COLUMN_FILE_ID,
						LabelFileView.COLUMN_ORDER,
						LabelFileView.COLUMN_FILE_NAME,
						LabelFileView.COLUMN_FOLDER,
						LabelFileView.COLUMN_THUMB_READY,
						LabelFileView.COLUMN_TYPE, LabelFileView.COLUMN_DEV_ID,
						LabelFileView.COLUMN_DEV_NAME,
						LabelFileView.COLUMN_HEIGHT,
						LabelFileView.COLUMN_WIDTH,
						LabelFileView.COLUMN_DEV_TYPE,
						LabelFileView.COLUMN_STATUS,
						LabelFileView.COLUMN_ORIENTATION,
						LabelFileView.COLUMN_ORIGINAL_PATH },
				LabelFileView.COLUMN_LABEL_ID + "=? AND "
						+ LabelFileView.COLUMN_STATUS + "=?",
				new String[] { labelId, "0" }, null);

		return cursor;
	}

	public static Cursor getAllLabelFileViewByLabelId(Context context) {
		Cursor cursor = context.getContentResolver().query(
				LabelFileView.CONTENT_URI,
				new String[] { LabelFileView.COLUMN_LABEL_ID,
						LabelFileView.COLUMN_FILE_ID,
						LabelFileView.COLUMN_ORDER,
						LabelFileView.COLUMN_FILE_NAME,
						LabelFileView.COLUMN_FOLDER,
						LabelFileView.COLUMN_THUMB_READY,
						LabelFileView.COLUMN_TYPE, LabelFileView.COLUMN_DEV_ID,
						LabelFileView.COLUMN_DEV_NAME,
						LabelFileView.COLUMN_HEIGHT,
						LabelFileView.COLUMN_WIDTH,
						LabelFileView.COLUMN_DEV_TYPE }, null, null, null);

		return cursor;
	}

	public static Cursor getLabelFilesByLabelId(Context context, String labelId) {
		Cursor cursor = context.getContentResolver().query(
				LabelFileTable.CONTENT_URI, null,
				LabelFileTable.COLUMN_LABEL_ID + " = ?",
				new String[] { labelId }, LabelFileTable.DEFAULT_SORT_ORDER);
		return cursor;
	}

	public static String getVideoLabelId(Context context) {
		String labelId = null;
		Cursor cursor = context.getContentResolver().query(
				LabelTable.CONTENT_URI,
				new String[] { LabelTable.COLUMN_LABEL_ID },
				LabelTable.COLUMN_LABEL_NAME + " = ?",
				new String[] { "videos" }, null);
		if (cursor != null && cursor.getCount() > 0) {
			cursor.moveToFirst();
			labelId = cursor.getString(0);
		}
		cursor.close();
		return labelId;
	}

	public static Cursor getLabeFilelCountByLabelId(Context context,
			String labelId) {

		Cursor cursor = context.getContentResolver().query(
				LabelFileTable.CONTENT_URI,
				new String[] { LabelFileTable.COLUMN_LABEL_ID,
						LabelFileTable.COLUMN_FILE_ID },
				LabelFileTable.COLUMN_LABEL_ID + " = ?",
				new String[] { labelId }, null);
		return cursor;
	}

	public static void removeLabelFileByFileId(Context context, String fileId) {
		ContentResolver cr = context.getContentResolver();
		cr.delete(LabelFileTable.CONTENT_URI, LabelFileTable.COLUMN_FILE_ID
				+ "=?", new String[] { fileId });
	}

	public static void removeLabelFileByLabelId(Context context, String labelId) {
		ContentResolver cr = context.getContentResolver();
		cr.delete(LabelFileTable.CONTENT_URI, LabelFileTable.COLUMN_LABEL_ID
				+ "=?", new String[] { labelId });
	}

	public static int updateLabelSeq(Context context, String labelId, String seq) {
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(LabelTable.COLUMN_SEQ, Integer.parseInt(seq));

		return cr.update(LabelTable.CONTENT_URI, cv, LabelTable.COLUMN_LABEL_ID
				+ "=?", new String[] { labelId });
	}

	public static int updateLabelServerSeqAndCoverUrl(Context context,
			String labelId, String seq, String coverUrl) {
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(LabelTable.COLUMN_SERVER_SEQ, Integer.parseInt(seq));
		cv.put(LabelTable.COLUMN_COVER_URL, coverUrl);
		return cr.update(LabelTable.CONTENT_URI, cv, LabelTable.COLUMN_LABEL_ID
				+ "=?", new String[] { labelId });
	}

	public static int updateLabeDisplayStatus(Context context, String labelId,
			String status) {
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(LabelTable.COLUMN_DISPLAY_STATUS, status);

		return cr.update(LabelTable.CONTENT_URI, cv, LabelTable.COLUMN_LABEL_ID
				+ "=?", new String[] { labelId });
	}

	public static int updateFileStatus(Context context, String fileId,
			String status) {
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(FileTable.COLUMN_STATUS, status);

		return cr.update(FileTable.CONTENT_URI, cv, FileTable.COLUMN_FILE_ID
				+ "=?", new String[] { fileId });
	}

	private static void removeFile(Context context, LabelEntity.Label label) {
		ContentResolver cr = context.getContentResolver();

		for (String file : label.files) {
			Cursor cursor = cr.query(LabelFileView.CONTENT_URI,
					new String[] { LabelFileView.COLUMN_FILE_ID },
					LabelFileView.COLUMN_LABEL_ID + " != ? AND "
							+ LabelFileView.COLUMN_FILE_ID + " =?",
					new String[] { label.label_id, file }, null);

			if (cursor.getCount() == 0) {
				updateFileStatus(context, file, Constant.FILE_STATUS_DELETE);
			}
		}
	}

}
