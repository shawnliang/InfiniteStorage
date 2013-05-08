package com.waveface.sync.logic;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Date;
import java.util.Locale;
import java.util.TreeSet;

import org.jwebsocket.kit.WebSocketException;

import android.app.AlarmManager;
import android.app.PendingIntent;
import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.database.Cursor;
import android.net.Uri;
import android.os.SystemClock;
import android.provider.MediaStore;
import android.text.TextUtils;

import com.waveface.sync.Constant;
import com.waveface.sync.RuntimeState;
import com.waveface.sync.db.BackupDetailsTable;
import com.waveface.sync.db.BackupedServersTable;
import com.waveface.sync.db.ImportFilesTable;
import com.waveface.sync.entity.FileBackupEntity;
import com.waveface.sync.util.FileUtil;
import com.waveface.sync.util.Log;
import com.waveface.sync.util.MediaFile;
import com.waveface.sync.util.StringUtil;
import com.waveface.sync.websocket.RuntimeWebClient;

public class BackupLogic {
	private static String TAG = BackupLogic.class.getSimpleName();
	public static String DATE_FORMAT = "yyyyMMddHHmmss";
	public static String ISO_DATE_FORMAT = "yyyy-MM-dd";

	public static void scanFileForBackup(Context context, int type) {
		Intent intent = new Intent(Constant.ACTION_SCAN_FILE);
		SimpleDateFormat sdfLimit = new SimpleDateFormat(DATE_FORMAT, Locale.US);
		String currentDate = "";
		String cursorDate = "";
		long refCursorDate = 0;
		long dateTaken = -1;
		long dateModified = 0;
		long dateAdded = 0;
		String fileSize = null;
		String folderName = null;
		String mediaData = null;
		String mimetype = null;
		long mediaId = -1;
		String[] projection = null;
		String selection = null;
		Cursor cursor = null;

		ContentResolver cr = context.getContentResolver();
		try {
			cursor = cr.query(ImportFilesTable.CONTENT_URI,
					new String[] { ImportFilesTable.COLUMN_DATE },
					ImportFilesTable.COLUMN_FILETYPE + "=?",
					new String[] { String.valueOf(type) },
					ImportFilesTable.DEFAULT_SORT_ORDER + " LIMIT 1");
			if (cursor != null && cursor.getCount() > 0) {
				cursor.moveToFirst();
				currentDate = cursor.getString(0);
				cursor.close();
			}
			cursor = null;
			if (!TextUtils.isEmpty(currentDate)) {
				currentDate = StringUtil.getEndDate(sdfLimit.format(StringUtil
						.parseDate(currentDate)));
			}
			String selectionArgs[] = { currentDate };
			Uri selectUri = null;
			String sqlOrder = null;
			if (type == Constant.TYPE_IMAGE) {// IMAGES
				Log.d(TAG, "SCAN IMAGE");
				selectUri = MediaStore.Images.Media.EXTERNAL_CONTENT_URI;
				projection = new String[] { MediaStore.Images.Media.DATA,
						MediaStore.Images.Media.DATE_TAKEN,
						MediaStore.Images.Media.DISPLAY_NAME,
						MediaStore.Images.Media.DATE_ADDED,
						MediaStore.Images.Media.DATE_MODIFIED,
						MediaStore.Images.Media.SIZE,
						MediaStore.Images.Media.MIME_TYPE,
						MediaStore.Images.Media._ID };
				selection = getImageSelection(context, currentDate);
				sqlOrder = MediaStore.Images.Media.DATE_ADDED ;
			} else if (type == Constant.TYPE_VIDEO) {// VIDEO
				Log.d(TAG, "SCAN VIDEO");
				selectUri = MediaStore.Video.Media.EXTERNAL_CONTENT_URI;
				projection = new String[] { MediaStore.Video.Media.DATA,
						MediaStore.Video.Media.DATE_TAKEN,
						MediaStore.Video.Media.DISPLAY_NAME,
						MediaStore.Video.Media.DATE_ADDED,
						MediaStore.Video.Media.DATE_MODIFIED,
						MediaStore.Video.Media.SIZE,
						MediaStore.Video.Media.MIME_TYPE,
						MediaStore.Video.Media._ID };
				selection = getVideoSelection(context, currentDate);
				sqlOrder = MediaStore.Video.Media.DATE_ADDED ;
			} else if (type == Constant.TYPE_AUDIO) {// AUDIO
				Log.d(TAG, "SCAN AUDIO");
				selectUri = MediaStore.Audio.Media.EXTERNAL_CONTENT_URI;
				projection = new String[] { MediaStore.Audio.Media.DATA,
						MediaStore.Audio.Media.DISPLAY_NAME,
						MediaStore.Audio.Media.ALBUM,
						MediaStore.Audio.Media.DATE_ADDED,
						MediaStore.Audio.Media.DATE_MODIFIED,
						MediaStore.Audio.Media.SIZE,
						MediaStore.Audio.Media.MIME_TYPE,
						MediaStore.Audio.Media._ID };
				selection = getAudioSelection(context, currentDate);
				sqlOrder = MediaStore.Audio.Media.DATE_ADDED ;
			}
			cursor = cr.query(selectUri, projection, selection, selectionArgs,
					sqlOrder);
			// if(!TextUtils.isEmpty(currentDate)){
			// Log.d(TAG, "selection => " +
			// selection+",currentDate:"+StringUtil.getConverDate(Long.parseLong(currentDate),StringUtil.ISO_8601_DATE_FORMAT));
			// }
			if (cursor != null && cursor.getCount() > 0) {
				ContentValues[] cvs = null;
				ContentValues cv = null;
				cursor.moveToFirst();
				ArrayList<ContentValues> datas = new ArrayList<ContentValues>();
				TreeSet<String> filenamesSet = new TreeSet<String>();
				cursor.moveToFirst();
				int count = cursor.getCount();
				for (int i = 0; i < count; i++) {
					mediaId = cursor.getLong(7);
					if (type == Constant.TYPE_IMAGE) {
						mediaData = cursor.getString(cursor
								.getColumnIndex(MediaStore.Images.Media.DATA));
						dateTaken = cursor
								.getLong(cursor
										.getColumnIndex(MediaStore.Images.Media.DATE_TAKEN));
						dateModified = cursor
								.getLong(cursor
										.getColumnIndex(MediaStore.Images.Media.DATE_MODIFIED));
						dateAdded = cursor
								.getLong(cursor
										.getColumnIndex(MediaStore.Images.Media.DATE_ADDED));
						if (mediaId > RuntimeState.maxImageId) {
							RuntimeState.maxImageId = mediaId;
						}
					} else if (type == Constant.TYPE_AUDIO) {
						mediaData = cursor.getString(cursor
								.getColumnIndex(MediaStore.Audio.Media.DATA));
						dateModified = cursor
								.getLong(cursor
										.getColumnIndex(MediaStore.Audio.Media.DATE_MODIFIED));
						dateAdded = cursor
								.getLong(cursor
										.getColumnIndex(MediaStore.Audio.Media.DATE_ADDED));
						if (mediaId > RuntimeState.maxAudioId) {
							RuntimeState.maxAudioId = mediaId;
						}
					} else if (type == Constant.TYPE_VIDEO) {
						mediaData = cursor.getString(cursor
								.getColumnIndex(MediaStore.Video.Media.DATA));
						dateTaken = cursor
								.getLong(cursor
										.getColumnIndex(MediaStore.Video.Media.DATE_TAKEN));
						dateModified = cursor
								.getLong(cursor
										.getColumnIndex(MediaStore.Video.Media.DATE_MODIFIED));
						dateAdded = cursor
								.getLong(cursor
										.getColumnIndex(MediaStore.Video.Media.DATE_ADDED));
						if (mediaId > RuntimeState.maxVideoId) {
							RuntimeState.maxVideoId = mediaId;
						}
					}
					Log.d(TAG, "cursorDate ==>" + cursorDate);
					fileSize = cursor.getString(5);
					mimetype = cursor.getString(6);
					if (TextUtils.isEmpty(mimetype)) {
						mimetype = MediaFile.getMimetype(mediaData);
					}
					folderName = getFoldername(mediaData);

					if (dateAdded != -1) {
						refCursorDate = dateAdded;
					}else if (dateModified != -1) {
						refCursorDate = dateModified;
					}else if (dateTaken != -1) {
						refCursorDate = dateTaken / 1000;
					}
					
					if (refCursorDate > 0 || cursorDate == null) {
						cursorDate = StringUtil.getLocalDate(refCursorDate);
					} 
					else {
						cursorDate = FileUtil.getFileCreateTime(mediaData);
					}
					if (!TextUtils.isEmpty(cursorDate)) {
						int year = Integer.parseInt(cursorDate.substring(0, 4));
						if (year <= 1970) {
							cursorDate = FileUtil.getFileCreateTime(mediaData);
						}
					}
					Log.d(TAG, "mediaId ==>" + mediaId);
					Log.d(TAG, "Filename ==>" + mediaData);
					Log.d(TAG, "cursorDate ==>" + cursorDate);

					if (!duplicateFilename(context, mediaData)
							&& !filenamesSet.contains(mediaData)) {
						cv = new ContentValues();
						cv.put(ImportFilesTable.COLUMN_FILENAME, mediaData);
						cv.put(ImportFilesTable.COLUMN_SIZE, fileSize);
						cv.put(ImportFilesTable.COLUMN_DATE, cursorDate);
						cv.put(ImportFilesTable.COLUMN_STATUS,
								Constant.IMPORT_FILE_INCLUDED);
						cv.put(ImportFilesTable.COLUMN_FILETYPE, type);
						cv.put(ImportFilesTable.COLUMN_MIMETYPE, mimetype);
						cv.put(ImportFilesTable.COLUMN_FOLDER, folderName);
						cv.put(ImportFilesTable.COLUMN_IMAGE_ID, mediaId);
						cv.put(ImportFilesTable.COLUMN_UPDATE_TIME,StringUtil.getLocalDate());
						Log.d(TAG, "File Updated Date ==>" + cv.getAsString(ImportFilesTable.COLUMN_UPDATE_TIME));						
						filenamesSet.add(mediaData);
						datas.add(cv);
					}
					cursor.moveToNext();
					if (datas.size() == 100) {
						// SAVE TO DB
						cvs = new ContentValues[datas.size()];
						datas.toArray(cvs);
						int result = cr.bulkInsert(
								ImportFilesTable.CONTENT_URI, cvs);
						Log.d(TAG, result + " Files Imported!");
						datas.clear();
						context.sendBroadcast(intent);
					}
				}
				if (datas.size() > 0) {
					// SAVE TO DB
					cvs = new ContentValues[datas.size()];
					datas.toArray(cvs);
					int result = cr.bulkInsert(ImportFilesTable.CONTENT_URI,
							cvs);
					Log.d(TAG, result + " Files Imported!");
					context.sendBroadcast(intent);
				}
				filenamesSet = null;
				datas = null;
			}
		} catch (Exception ex) {
			ex.printStackTrace();
		} finally {
			if (cursor != null) {
				cursor.close();
				cursor = null;
			}
		}

	}

	public static boolean duplicateFilename(Context context, String filename) {
		ContentResolver cr = context.getContentResolver();
		int ramindCount = 0;
		Cursor cursor = null;
		try {
			cursor = cr.query(ImportFilesTable.CONTENT_URI,
					new String[] { ImportFilesTable.COLUMN_SIZE },
					ImportFilesTable.COLUMN_FILENAME + "=?",
					new String[] { filename }, null);
			if (cursor != null && cursor.getCount() > 0) {
				ramindCount = cursor.getCount();
			}
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			if (cursor != null) {
				cursor.close();
				cursor = null;
			}
		}
		if (ramindCount > 0)
			return true;
		else
			return false;
	}

	public static String getFoldername(String imageFullpath) {
		if (!TextUtils.isEmpty(imageFullpath)) {
			int lastIndex = imageFullpath.lastIndexOf(File.separator);
			int lastSecondIndex = imageFullpath.substring(0, lastIndex)
					.lastIndexOf(File.separator);
			return imageFullpath.substring(lastSecondIndex + 1, lastIndex);
		} else {
			return "";
		}
	}

	public static String getImageSelection(Context context, String date) {
		String selection = "(" + MediaStore.Images.Media.DATE_ADDED;
		if (TextUtils.isEmpty(date)) {
			selection += " <= ? ";
		} else {
			selection += " > ? ";
		}
		selection += " AND " + MediaStore.Images.Media.DISPLAY_NAME
				+ " NOT LIKE '%.jps%' )";
		return selection;
	}

	public static String getVideoSelection(Context context, String date) {
		String selection = MediaStore.Video.Media.DATE_ADDED;
		if (TextUtils.isEmpty(date)) {
			selection += " <= ? ";
		} else {
			selection += " > ? ";
		}
		return selection;
	}

	public static String getAudioSelection(Context context, String date) {
		String selection = MediaStore.Audio.Media.DATE_ADDED;
		if (TextUtils.isEmpty(date)) {
			selection += " <= ? ";
		} else {
			selection += " > ? ";
		}
		return selection;
	}

	public static String[] getFilePeriods(Context context) {
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		String newestDay = null;
		String oldestDay = null;

		try {
			cursor = cr.query(ImportFilesTable.CONTENT_URI,
					new String[] { ImportFilesTable.COLUMN_DATE }, null, null,
					ImportFilesTable.DEFAULT_SORT_ORDER + " LIMIT 1");
			if (cursor != null && cursor.getCount() > 0) {
				cursor.moveToFirst();
				newestDay = cursor.getString(0);
				newestDay = newestDay.substring(0, 10);
			}
			cursor = cr.query(ImportFilesTable.CONTENT_URI,
					new String[] { ImportFilesTable.COLUMN_DATE }, null, null,
					ImportFilesTable.COLUMN_DATE + " LIMIT 1");
			if (cursor != null && cursor.getCount() > 0) {
				cursor.moveToFirst();
				oldestDay = cursor.getString(0);
				oldestDay = oldestDay.substring(0, 10);
			}
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			if (cursor != null) {
				cursor.close();
				cursor = null;
			}
		}
		return new String[] { oldestDay, newestDay };
	}

	public static long[] getFileInfo(Context context, int type) {
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		long count = 0;
		long totalSizes = 0;

		try {
			cursor = cr.query(ImportFilesTable.CONTENT_URI,
					new String[] { ImportFilesTable.COLUMN_SIZE },
					ImportFilesTable.COLUMN_FILETYPE + "=? AND "+
				    ImportFilesTable.COLUMN_STATUS + "!=? AND "+
				    ImportFilesTable.COLUMN_STATUS + "!=? ",
					new String[] { String.valueOf(type),
					               Constant.IMPORT_FILE_DELETED,
					               Constant.IMPORT_FILE_EXCLUDE}, null);
			if (cursor != null && cursor.getCount() > 0) {
				count = cursor.getCount();
				cursor.moveToFirst();
				for (int i = 0; i < count; i++) {
					totalSizes += cursor.getLong(0);
					cursor.moveToNext();
				}
			}
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			if (cursor != null) {
				cursor.close();
				cursor = null;
			}
		}
		return new long[] { count, totalSizes };
	}

	public static void autoBackupStart(Context context) {
		SharedPreferences pref = context.getSharedPreferences(
				Constant.PREFS_NAME, Context.MODE_PRIVATE);
		String autoImportBornTime = pref.getString(
				Constant.PREF_AUTO_IMPORT_BORN_TIME, "");
		if (TextUtils.isEmpty(autoImportBornTime)) {
			String bornTime = StringUtil.changeToLocalString(StringUtil
					.formatDate(new Date()));
			pref.edit()
					.putString(Constant.PREF_AUTO_IMPORT_BORN_TIME, bornTime)
					.commit();
		}
		pref.edit().putBoolean(Constant.PREF_AUTO_IMPORT_ENABLED, true)
				.commit();
	}

	public static int updateBackupStatus(Context context, String filename,
			String status) {
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(ImportFilesTable.COLUMN_STATUS, status);
		return cr.update(ImportFilesTable.CONTENT_URI, cv,
				ImportFilesTable.COLUMN_FILENAME + "=?",
				new String[] { filename });
	}

	public static int updateBackupStatus(Context context, String serverId,
			String filename, String status) {
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(BackupDetailsTable.COLUMN_STATUS, status);
		return cr.update(BackupDetailsTable.CONTENT_URI, cv,
				BackupDetailsTable.COLUMN_SERVER_ID + " =? AND"
						+ BackupDetailsTable.COLUMN_FILENAME + " =?",
				new String[] { serverId, filename });
	}

	public static int getBackedUpCountForPairedPC(Context context) {
		int backupedCount = 0;
		String lastBackupFileDate = null;		
		// select from serverFiles
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		try {
			cursor = cr
					.query(BackupedServersTable.CONTENT_URI,
							new String[] { 
							BackupedServersTable.COLUMN_LAST_FILE_DATE},
							BackupedServersTable.COLUMN_SERVER_ID + " NOT IN (?,?)",
							new String[] { 
							Constant.IMPORT_FILE_DELETED,
							Constant.IMPORT_FILE_EXCLUDE}, null);
			if (cursor != null && cursor.getCount() > 0) {
				cursor.moveToFirst();
				lastBackupFileDate = cursor.getString(0);
				cursor.close();
			}
			cursor = null;
			String where = ImportFilesTable.COLUMN_STATUS + "!=? AND "
					+ ImportFilesTable.COLUMN_STATUS + "!=? ";
			String[] whereArgs = new String[] { Constant.IMPORT_FILE_DELETED,
					Constant.IMPORT_FILE_EXCLUDE };

			// GET BACKUPED COUNT
			if (!TextUtils.isEmpty(lastBackupFileDate)) {
				
				where = ImportFilesTable.COLUMN_DATE + "<=? AND "
						+ ImportFilesTable.COLUMN_STATUS + "!=? AND "
						+ ImportFilesTable.COLUMN_STATUS + "!=? ";
				whereArgs = new String[] { 
						lastBackupFileDate,
						Constant.IMPORT_FILE_DELETED,
						Constant.IMPORT_FILE_EXCLUDE };
				cursor = cr.query(ImportFilesTable.CONTENT_URI, new String[] {
						ImportFilesTable.COLUMN_FILENAME,
						ImportFilesTable.COLUMN_DATE }, where, whereArgs, null);
				if (cursor != null && cursor.getCount() > 0) {
					backupedCount = cursor.getCount();
					cursor.close();
				}
				cursor = null;
			}
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			if (cursor != null) {
				cursor.close();
				cursor = null;
			}
		}
		return backupedCount;
	}
	
	public static int[] getBackupProgressInfo(Context context, String serverId) {
		int totalCount = 0;
		int backupedCount = 0;
		String lastBackupFileDate = null;		
		// select from serverFiles
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		try {
			cursor = cr
					.query(BackupedServersTable.CONTENT_URI,
							new String[] { 
							BackupedServersTable.COLUMN_LAST_FILE_DATE},
							BackupedServersTable.COLUMN_SERVER_ID + "=?",
							new String[] { serverId }, null);
			if (cursor != null && cursor.getCount() > 0) {
				cursor.moveToFirst();
				lastBackupFileDate = cursor.getString(0);
				cursor.close();
			}
			cursor = null;
			String where = ImportFilesTable.COLUMN_STATUS + "!=? AND "
					+ ImportFilesTable.COLUMN_STATUS + "!=? ";
			String[] whereArgs = new String[] { Constant.IMPORT_FILE_DELETED,
					Constant.IMPORT_FILE_EXCLUDE };

			// GET TOTAL COUUNT
			cursor = cr.query(ImportFilesTable.CONTENT_URI,
					new String[] { ImportFilesTable.COLUMN_DATE }, where,
					whereArgs, null);
			if (cursor != null && cursor.getCount() > 0) {
				totalCount = cursor.getCount();
				cursor.close();
			}
			cursor = null;
			// GET BACKUPED COUNT
			if (!TextUtils.isEmpty(lastBackupFileDate)) {
				
				where = ImportFilesTable.COLUMN_DATE + "<=? AND "
						+ ImportFilesTable.COLUMN_STATUS + "!=? AND "
						+ ImportFilesTable.COLUMN_STATUS + "!=? ";
				whereArgs = new String[] { 
						lastBackupFileDate,
						Constant.IMPORT_FILE_DELETED,
						Constant.IMPORT_FILE_EXCLUDE };
				cursor = cr.query(ImportFilesTable.CONTENT_URI, new String[] {
						ImportFilesTable.COLUMN_FILENAME,
						ImportFilesTable.COLUMN_DATE }, where, whereArgs, null);
				if (cursor != null && cursor.getCount() > 0) {
					backupedCount = cursor.getCount();
					cursor.close();
				}
				cursor = null;
			}
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			if (cursor != null) {
				cursor.close();
				cursor = null;
			}
		}
		return new int[] { backupedCount, totalCount };
	}

	public static boolean needToBackup(Context context, String serverId) {
		boolean needBackup = false;
		String lastMediaDatetime = null;				
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		try {
			cursor = cr
					.query(BackupedServersTable.CONTENT_URI,
							new String[] { 
							    BackupedServersTable.COLUMN_LAST_FILE_DATE
							},
							BackupedServersTable.COLUMN_SERVER_ID + "=?",
							new String[] { serverId }, null);
			if (cursor != null && cursor.getCount() > 0) {
				cursor.moveToFirst();
				lastMediaDatetime = cursor.getString(0);
				cursor.close();
			}
			cursor = null;
			
			String where = ImportFilesTable.COLUMN_DATE+">? AND " 
					+ ImportFilesTable.COLUMN_STATUS + "!=? AND "
					+ ImportFilesTable.COLUMN_STATUS + "!=? ";
			String[] whereArgs = 
					new String[] 
							{ lastMediaDatetime,
							  Constant.IMPORT_FILE_DELETED,
							  Constant.IMPORT_FILE_EXCLUDE };
			cursor = cr.query(ImportFilesTable.CONTENT_URI,
					new String[] { ImportFilesTable.COLUMN_DATE	}, 
						where,whereArgs, ImportFilesTable.COLUMN_IMAGE_ID
							+ " DESC LIMIT 1");
			if (cursor != null && cursor.getCount() > 0) {
				needBackup = true;
				cursor.close();
			}
			cursor = null;
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			if (cursor != null) {
				cursor.close();
				cursor = null;
			}
		}
		return needBackup;
	}

	public static void detectFile(Context context) {
		// select from serverFiles
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = null;
		try {
			cursor = cr.query(ImportFilesTable.CONTENT_URI, new String[] {
					ImportFilesTable.COLUMN_FILENAME,
					ImportFilesTable.COLUMN_DATE },
					ImportFilesTable.COLUMN_FILENAME + " like '%.ogg%'", null,
					null);
			if (cursor != null && cursor.getCount() > 0) {
				cursor.moveToFirst();
				int count = cursor.getCount();
				for (int i = 0; i < count; i++) {
					Log.d(TAG, "Filename:" + cursor.getString(0));
					Log.d(TAG, "Datetime:" + cursor.getString(1));
					cursor.moveToNext();
				}
				cursor.close();
			}
			cursor = null;
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			if (cursor != null) {
				cursor.close();
				cursor = null;
			}
		}
	}

	public static void backupFiles(Context context, String serverId) {
		FileBackupEntity entity = new FileBackupEntity();
		byte[] buffer = new byte[256 * Constant.K_BYTES];
		byte[] finalBuffer = null;
		InputStream ios = null;
		boolean isSuccesed = false;
		String filename = null;
		String fileDateTime = null;		
		String fileUpdatedTime = null;
		int filetype = 0;
		long lastBackupMediaId = 0;
		String lastBackupFileDateTime = null;		
		String status = null;
		// select from serverFiles
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = null;
		Intent intent = new Intent(Constant.ACTION_UPLOADING_FILE);
		try {
			cursor = cr
					.query(BackupedServersTable.CONTENT_URI,
							new String[] { 
							BackupedServersTable.COLUMN_LAST_FILE_DATE},
							BackupedServersTable.COLUMN_SERVER_ID + "=?",
							new String[] { serverId }, null);

			if (cursor != null && cursor.getCount() > 0) {
				cursor.moveToFirst();
				lastBackupFileDateTime = cursor.getString(0);
				cursor.close();
			}
			cursor = null;

			cursor = cr.query(ImportFilesTable.CONTENT_URI, new String[] {
					ImportFilesTable.COLUMN_FILENAME,
					ImportFilesTable.COLUMN_FILETYPE,
					ImportFilesTable.COLUMN_SIZE, 
					ImportFilesTable.COLUMN_DATE,
					ImportFilesTable.COLUMN_IMAGE_ID,
					ImportFilesTable.COLUMN_STATUS,
					ImportFilesTable.COLUMN_UPDATE_TIME},
					ImportFilesTable.COLUMN_DATE + " > ?",
					new String[] {
					    lastBackupFileDateTime},
					ImportFilesTable.COLUMN_DATE);
			if (cursor != null && cursor.getCount() > 0) {
				cursor.moveToFirst();
				int count = cursor.getCount();
				for (int i = 0; i < count; i++) {
					// send file index for start
					filename = cursor.getString(0);
					
					entity.action = Constant.WS_ACTION_FILE_START;
					entity.fileName = StringUtil.getFilename(filename);
					filetype = cursor.getInt(1);
					switch (filetype) {
					case Constant.TYPE_IMAGE:
						entity.type = Constant.TRANSFER_TYPE_IMAGE;
						break;
					case Constant.TYPE_AUDIO:
						entity.type = Constant.TRANSFER_TYPE_AUDIO;
						break;
					case Constant.TYPE_VIDEO:
						entity.type = Constant.TRANSFER_TYPE_VIDEO;
						break;
					}
					entity.fileSize = cursor.getString(2);
					entity.folder = StringUtil.getFilepath(filename,
							entity.fileName);
					entity.datetime = cursor.getString(3);

					int progress[] = BackupLogic.getBackupProgressInfo(context, serverId);

					entity.backupedCount = progress[0];
					entity.totalCount = progress[1];
					fileDateTime = entity.datetime; 						
					if(Integer.parseInt(entity.fileSize)==0){
						break;
					}
					lastBackupMediaId = cursor.getLong(4);
					status = cursor.getString(5);
					fileUpdatedTime = cursor.getString(6);
					
						// FOR UI DISPLAY
					if (FileUtil.isFileExisted(filename) 
							&& !status.equals(Constant.IMPORT_FILE_DELETED) 
							&& !status.equals(Constant.IMPORT_FILE_EXCLUDE)) {						
						try {
							// FOR UI DISPLAY
							RuntimeState.mFilename = entity.fileName;
							RuntimeState.mFileType = filetype;
							RuntimeState.mFilename = filename;
							RuntimeState.mMediaID = lastBackupMediaId;
							intent.putExtra(Constant.EXTRA_BACKING_UP_FILE_STATE,
									Constant.FILE_START);
							context.sendBroadcast(intent);
							Log.d(TAG, i+":BACKUPING: type:" + entity.type
									+ ",Filename:" + entity.fileName);
							Log.d(TAG, "filedate:"+fileDateTime+"fileUpdatedTime:"+fileUpdatedTime);
							Log.d(TAG, "lastBackupMediaId:"+lastBackupMediaId);							

							if (RuntimeState.isWebSocketAvaliable(context)) {
								RuntimeWebClient.send(RuntimeState.GSON
										.toJson(entity));

							} else {
								isSuccesed = true;
								break;
							}
							// send file binary
							ios = new FileInputStream(new File(filename));
							int read = 0;
							while ((read = ios.read(buffer)) != -1) {
								if (RuntimeState.isWebSocketAvaliable(context)) {
									if (read != buffer.length) {
										finalBuffer = new byte[read];
										finalBuffer = Arrays.copyOf(buffer,
												read);
										RuntimeWebClient.sendFile(finalBuffer);
									} else {
										RuntimeWebClient.sendFile(buffer);
									}
								} else {
									isSuccesed = false;
									break;
								}
							}
							// send file index for end
							if (RuntimeState.isWebSocketAvaliable(context)) {
								entity.action = Constant.WS_ACTION_FILE_END;
								RuntimeWebClient.send(RuntimeState.GSON
										.toJson(entity));
								isSuccesed = true;
							} else {
								isSuccesed = false;
								break;
							}
						} catch (FileNotFoundException e) {
							e.printStackTrace();
						} catch (IOException e) {
							e.printStackTrace();
						} catch (WebSocketException e) {
							e.printStackTrace();
						} finally {
							try {
								if (ios != null)
									ios.close();
							} catch (IOException e) {
							}
							if (isSuccesed) {
								ServersLogic.updateServerLastBackupStatus(
										context, serverId,
										String.valueOf(lastBackupMediaId),
										fileDateTime,
										fileUpdatedTime);
								RuntimeState.FileBackedUp(context);
							}
							isSuccesed = false;
						}
						cursor.moveToNext();
						if (!RuntimeState.isWebSocketAvaliable(context)) {
							break;// cut off for loop
						}
					} else {
						// UPDATE IMFOR FILE
						BackupLogic.updateBackupStatus(context, filename,
								Constant.IMPORT_FILE_DELETED);
						ServersLogic.updateServerLastBackupStatus(
								context, serverId,
								String.valueOf(lastBackupMediaId),
								fileDateTime,
								fileUpdatedTime);
						Intent intt = new Intent(Constant.ACTION_FILE_DELETED);
						context.sendBroadcast(intt);
						// UPDATE COUNT FOR DETECT FILE IS DELETED
						ServersLogic.updateCount(context, serverId);
					}
				}
				cursor.close();
			}
			cursor = null;
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			if (cursor != null) {
				cursor.close();
				cursor = null;
			}
		}
	}

	 
	
	public static void scanAllFiles(Context context) {
		RuntimeState.isScaning = true;
		scanFileForBackup(context, Constant.TYPE_AUDIO);
		scanFileForBackup(context, Constant.TYPE_VIDEO);
		scanFileForBackup(context, Constant.TYPE_IMAGE);
		RuntimeState.isScaning = false;
	}

	public static String getSpaceByType(Context context, String type,
			String startDate, String endDate) {

		int DataType = 0;
		if (type.equals(Constant.TRANSFER_TYPE_IMAGE)) {
			DataType = Constant.TYPE_IMAGE;
		} else if (type.equals(Constant.TRANSFER_TYPE_VIDEO)) {
			DataType = Constant.TYPE_VIDEO;
		} else if (type.equals(Constant.TRANSFER_TYPE_AUDIO)) {
			DataType = Constant.TYPE_AUDIO;
		}
		long totalSize = 0;
		ContentResolver cr = context.getContentResolver();
		long count = 0;
		Cursor cursor = null;
		try {
			cursor = cr
					.query(ImportFilesTable.CONTENT_URI,
							new String[] { ImportFilesTable.COLUMN_SIZE },
							ImportFilesTable.COLUMN_FILETYPE + "=? AND "
									+ ImportFilesTable.COLUMN_DATE + "<? AND "
									+ ImportFilesTable.COLUMN_DATE + ">=? ",
							new String[] { String.valueOf(DataType), startDate,
									endDate }, null);
			if (cursor != null && cursor.getCount() > 0) {
				count = cursor.getCount();
				cursor.moveToFirst();
				for (int i = 0; i < count; i++) {
					totalSize += cursor.getLong(0);
					cursor.moveToNext();
				}
				cursor.close();
			}
			cursor = null;
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			if (cursor != null) {
				cursor.close();
				cursor = null;
			}
		}
		return StringUtil.byteCountToDisplaySize(totalSize);
	}

	public static void setAlarmWakeUpService(Context context) {
		AlarmManager alarmManager = (AlarmManager) context
				.getSystemService(Context.ALARM_SERVICE);
		// ORIGINAL
		// Calendar cal = Calendar.getInstance();
		// cal.setTimeInMillis(System.currentTimeMillis());
		// cal.add(Calendar.SECOND, 10);
		// int type = AlarmManager.RTC_WAKEUP;
		// long triggerTime = cal.getTimeInMillis();
		// Intent intent = new Intent(context, InfiniteReceiver.class);
		// NEW
		int type = AlarmManager.ELAPSED_REALTIME_WAKEUP;
		long triggerTime = SystemClock.elapsedRealtime();
		Intent intent = new Intent(Constant.ACTION_INFINITE_STORAGE_ALARM);
		PendingIntent sender = PendingIntent.getBroadcast(context, 0, intent,
				PendingIntent.FLAG_UPDATE_CURRENT);
		// alarmManager.setRepeating(type, triggerTime,interval, sender);
		alarmManager.setInexactRepeating(type, triggerTime,
				Constant.ALARM_INTERVAL, sender);
	}

	public static long getMaxIdFromMediaDB(Context context, int type) {
		long maxMediaId = 0;
		Uri mediaUri = null;
		switch (type) {
		case Constant.TYPE_IMAGE:
			mediaUri = MediaStore.Images.Media.EXTERNAL_CONTENT_URI;
			break;
		case Constant.TYPE_VIDEO:
			mediaUri = MediaStore.Video.Media.EXTERNAL_CONTENT_URI;
			break;
		case Constant.TYPE_AUDIO:
			mediaUri = MediaStore.Audio.Media.EXTERNAL_CONTENT_URI;
			break;
		}
		Cursor cursor = context.getContentResolver().query(mediaUri, 
				new String[] { MediaStore.Audio.Media._ID },
				null, null, MediaStore.Images.Media._ID + " DESC");
		maxMediaId = cursor.moveToFirst() ? cursor.getLong(cursor
				.getColumnIndex(MediaStore.Audio.Media._ID)) : -1;
		if(cursor!=null){
			cursor.close();
		}
		cursor = null;
		return maxMediaId;
	}
	public static long getModifiedTime(Context context, int type,long mediaId) {
		long dateModified = -1;
		long dateAdded = -1;		
		Uri mediaUri = null;
		switch (type) {
		case Constant.TYPE_VIDEO:
			mediaUri = MediaStore.Video.Media.EXTERNAL_CONTENT_URI;
			break;
		case Constant.TYPE_AUDIO:
			mediaUri = MediaStore.Audio.Media.EXTERNAL_CONTENT_URI;
			break;
		}
		Cursor cursor = context.getContentResolver().query(mediaUri, 
				new String[] 
						{ MediaStore.Video.Media.DATE_MODIFIED,
						MediaStore.Video.Media.DATE_ADDED,
						MediaStore.Video.Media.SIZE,
						MediaStore.Video.Media.DATA
						},
						MediaStore.Images.Media._ID+"=?", 
						new String[]{String.valueOf(mediaId)}, 
						null);
		if(cursor !=null && cursor.getCount()>0){
			cursor.moveToFirst(); 
			dateModified = cursor.getLong(0);
			dateAdded = cursor.getLong(1);		
			Log.d("InfiniteService", "dateAdded:"+dateAdded);
			Log.d("InfiniteService", "size:"+cursor.getLong(2));
			Log.d("InfiniteService", "real size:"+ new File(cursor.getString(3)).length());
			cursor.close();
		}
		cursor = null;
		return dateModified;
	}
	public static long getFileSizeFromDB(Context context, int type,long mediaId) {
		long fileSize = -1;
		Uri mediaUri = null;
		switch (type) {
		case Constant.TYPE_VIDEO:
			mediaUri = MediaStore.Video.Media.EXTERNAL_CONTENT_URI;
			break;
		case Constant.TYPE_AUDIO:
			mediaUri = MediaStore.Audio.Media.EXTERNAL_CONTENT_URI;
			break;
		}
		Cursor cursor = context.getContentResolver().query(mediaUri, 
				new String[] 
						{MediaStore.Video.Media.SIZE},
						MediaStore.Images.Media._ID+"=?", 
						new String[]{String.valueOf(mediaId)}, 
						null);
		if(cursor !=null && cursor.getCount()>0){
			cursor.moveToFirst(); 
			fileSize = cursor.getLong(0);
			cursor.close();
		}
		cursor = null;
		return fileSize;
	}
	

	public static void setLastMediaSate(Context context) {
		int[] types = { Constant.TYPE_IMAGE, Constant.TYPE_VIDEO,
				Constant.TYPE_AUDIO };
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		try {
			
			for (int i = 0; i < types.length; i++) {
				cursor = cr.query(ImportFilesTable.CONTENT_URI,
						new String[] { ImportFilesTable.COLUMN_IMAGE_ID },
						ImportFilesTable.COLUMN_FILETYPE + "=?",
						new String[] { String.valueOf(types[i]) }, ImportFilesTable.COLUMN_IMAGE_ID+" DESC LIMIT 1");
				if (cursor != null && cursor.getCount() > 0) {
					cursor.moveToFirst();
					switch (types[i]) {
					case Constant.TYPE_IMAGE:
						RuntimeState.maxImageId = cursor.getLong(0);
						break;
					case Constant.TYPE_VIDEO:
						RuntimeState.maxVideoId = cursor.getLong(0);
						break;
					case Constant.TYPE_AUDIO:
						RuntimeState.maxAudioId = cursor.getLong(0);
						break;
					}
				}
			}
			if (cursor != null) {
				cursor.close();
				cursor = null;
			}
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			if (cursor != null) {
				cursor.close();
				cursor = null;
			}
		}
	}
}