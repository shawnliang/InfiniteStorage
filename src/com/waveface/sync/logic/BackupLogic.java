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

import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Context;
import android.content.SharedPreferences;
import android.database.Cursor;
import android.provider.MediaStore;
import android.text.TextUtils;

import com.waveface.sync.Constant;
import com.waveface.sync.RuntimeConfig;
import com.waveface.sync.db.BackupDetailsTable;
import com.waveface.sync.db.ImportFilesTable;
import com.waveface.sync.db.ServerFilesView;
import com.waveface.sync.db.BackupedServersTable;
import com.waveface.sync.entity.FIleTransferEntity;
import com.waveface.sync.util.FileUtil;
import com.waveface.sync.util.Log;
import com.waveface.sync.util.MediaFile;
import com.waveface.sync.util.NetworkUtil;
import com.waveface.sync.util.StringUtil;
import com.waveface.sync.websocket.RuntimeWebClient;

public class BackupLogic {
	private static String TAG = BackupLogic.class.getSimpleName();
	public static String DATE_FORMAT = "yyyyMMddHHmmss";
	public static String ISO_DATE_FORMAT = "yyyy-MM-dd";
	
	public static void scanFileForBackup(Context context,int type){
		SimpleDateFormat sdfLimit = new SimpleDateFormat(DATE_FORMAT,Locale.US);
		String currentDate = "";
		String cursorDate = "";
		long refCursorDate = 0 ;
		long dateTaken = -1;
		long dateModified = 0;
		long dateAdded = 0;
		String fileSize = null;
		String folderName = null;
		String mediaData = null;
		String mimetype = null;
		String imageId = null;		
		String[] projection = null;
		String selection =  null;
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();

		cursor = cr.query(ImportFilesTable.CONTENT_URI, 
				new String[]{ImportFilesTable.COLUMN_DATE}, 
				ImportFilesTable.COLUMN_FILETYPE+"=?", 
				new String[]{String.valueOf(type)}, 
				ImportFilesTable.DEFAULT_SORT_ORDER+" LIMIT 1");	
		if(cursor!=null && cursor.getCount()>0){
			cursor.moveToFirst();
			currentDate = cursor.getString(0);
		}
		if(!TextUtils.isEmpty(currentDate)){
			currentDate = StringUtil.getEndDate(sdfLimit.format(StringUtil
					.parseDate(currentDate)));
		}
		String selectionArgs[] = { currentDate };
		
		if(type == Constant.TYPE_IMAGE){//IMAGES
			projection = new String[]{
					MediaStore.Images.Media.DATA,
					MediaStore.Images.Media.DATE_TAKEN,
					MediaStore.Images.Media.DISPLAY_NAME,
					MediaStore.Images.Media.DATE_ADDED,
					MediaStore.Images.Media.DATE_MODIFIED,
					MediaStore.Images.Media.SIZE,
					MediaStore.Images.Media.MIME_TYPE,
					MediaStore.Images.Media._ID};
			selection =  getImageSelection(context,currentDate);
			Log.d(TAG, "selection => " + selection);
			cursor =cr.query(
					MediaStore.Images.Media.EXTERNAL_CONTENT_URI, projection,
					selection, selectionArgs,
					MediaStore.Images.Media.DATE_TAKEN+" DESC");
		}
		else if(type == Constant.TYPE_VIDEO){//VIDEO
			projection = new String[]{
					MediaStore.Video.Media.DATA,
					MediaStore.Video.Media.DATE_TAKEN,
					MediaStore.Video.Media.DISPLAY_NAME,
					MediaStore.Video.Media.DATE_ADDED,
					MediaStore.Video.Media.DATE_MODIFIED,
					MediaStore.Video.Media.SIZE,
					MediaStore.Video.Media.MIME_TYPE,
					MediaStore.Video.Media._ID};
	
			selection =  getVideoSelection(context,currentDate);
			Log.d(TAG, "selection => " + selection);
			cursor =cr.query(
					MediaStore.Video.Media.EXTERNAL_CONTENT_URI, projection,
					selection, selectionArgs,
					MediaStore.Video.Media.DATE_TAKEN+" DESC");
		}
		else if(type == Constant.TYPE_AUDIO){//AUDIO
			projection = new String[]{
					MediaStore.Audio.Media.DATA,
					MediaStore.Audio.Media.DISPLAY_NAME,
					MediaStore.Audio.Media.ALBUM,
					MediaStore.Audio.Media.DATE_ADDED,
					MediaStore.Audio.Media.DATE_MODIFIED,
					MediaStore.Audio.Media.SIZE,
					MediaStore.Audio.Media.MIME_TYPE,
					MediaStore.Audio.Media._ID};
	
			selection =  getAudioSelection(context,currentDate);
	
			Log.d(TAG, "selection => " + selection);
			cursor = cr.query(
					MediaStore.Audio.Media.EXTERNAL_CONTENT_URI, projection,
					selection, selectionArgs,
					MediaStore.Audio.Media.DATE_ADDED+" DESC");
		}
		if(cursor!=null && cursor.getCount()>0){
			ContentValues[] cvs = null;
			ContentValues cv = null;
			cursor.moveToFirst();
			ArrayList<ContentValues> datas = new ArrayList<ContentValues>();
			TreeSet<String> filenamesSet = new TreeSet<String>();
			cursor.moveToFirst();
			int count = cursor.getCount();			
			for(int i = 0 ; i < count; i++){
				if(type == Constant.TYPE_IMAGE){
					mediaData = cursor.getString(cursor.getColumnIndex(MediaStore.Images.Media.DATA));
					dateTaken = cursor.getLong(cursor.getColumnIndex(MediaStore.Images.Media.DATE_TAKEN));
					dateModified = cursor.getLong(cursor.getColumnIndex(MediaStore.Images.Media.DATE_MODIFIED));
					dateAdded = cursor.getLong(cursor.getColumnIndex(MediaStore.Images.Media.DATE_ADDED));					
				}
				else if(type == Constant.TYPE_AUDIO){
					mediaData = cursor.getString(cursor.getColumnIndex(MediaStore.Audio.Media.DATA));
					dateModified = cursor.getLong(cursor.getColumnIndex(MediaStore.Audio.Media.DATE_MODIFIED));
					dateAdded = cursor.getLong(cursor.getColumnIndex(MediaStore.Audio.Media.DATE_ADDED));
				}
				else if(type == Constant.TYPE_VIDEO){
					mediaData = cursor.getString(cursor.getColumnIndex(MediaStore.Video.Media.DATA));
					dateTaken = cursor.getLong(cursor.getColumnIndex(MediaStore.Video.Media.DATE_TAKEN));
					dateModified = cursor.getLong(cursor.getColumnIndex(MediaStore.Video.Media.DATE_MODIFIED));
					dateAdded = cursor.getLong(cursor.getColumnIndex(MediaStore.Video.Media.DATE_ADDED));
				}
				fileSize = cursor.getString(5);
				mimetype = cursor.getString(6);
				if(TextUtils.isEmpty(mimetype)){
					mimetype = MediaFile.getMimetype(mediaData);
				}
				imageId = cursor.getString(7);		
				folderName =  getFoldername(mediaData);
				
				if (dateTaken != -1) {
					refCursorDate = dateTaken / 1000;
				} else if (dateModified != -1) {
					refCursorDate = dateModified ;
				} else if (dateAdded != -1) {
					refCursorDate = dateAdded ;
				}
				cursorDate = StringUtil.getConverDate(refCursorDate,StringUtil.ISO_8601_DATE_FORMAT);
				Log.d(TAG, "cursorDate ==>" + cursorDate);
				FileUtil.getFileCreateTime(mediaData);
				if(cursorDate.startsWith("1970")){
					cursorDate = FileUtil.getFileCreateTime(mediaData);
					Log.d(TAG, "modified cursorDate ==>" + cursorDate);
				}
				Log.d(TAG, "Filename ==>" + mediaData);
				
				if(!duplicateFilename(context,mediaData) && !filenamesSet.contains(mediaData)){
					cv = new ContentValues();
					cv.put(ImportFilesTable.COLUMN_FILENAME, mediaData);
					cv.put(ImportFilesTable.COLUMN_SIZE, fileSize);
					cv.put(ImportFilesTable.COLUMN_DATE, cursorDate);
					cv.put(ImportFilesTable.COLUMN_STATUS, Constant.IMPORT_FILE_INCLUDED);
					cv.put(ImportFilesTable.COLUMN_FILETYPE, type);		
					cv.put(ImportFilesTable.COLUMN_MIMETYPE, mimetype);							
					cv.put(ImportFilesTable.COLUMN_FOLDER, folderName);
					cv.put(ImportFilesTable.COLUMN_IMAGE_ID, imageId);
					filenamesSet.add(mediaData);
					datas.add(cv);
				}
				cursor.moveToNext();
				if(datas.size() == 100){
					//SAVE TO DB
					cvs = new ContentValues[datas.size()];
					datas.toArray(cvs);
					int result = cr.bulkInsert(ImportFilesTable.CONTENT_URI, cvs);
					Log.d(TAG,result+ " Files Imported!");
					datas.clear();
				}
			}
			cursor.close();			
			if(datas.size()>0){
				//SAVE TO DB
				cvs = new ContentValues[datas.size()];
				datas.toArray(cvs);
				int result = cr.bulkInsert(ImportFilesTable.CONTENT_URI, cvs);
				Log.d(TAG,result+ " Files Imported!");
			}
			filenamesSet = null;
			datas= null;
		}
    }
	public static boolean duplicateFilename(Context context,String filename){
		ContentResolver cr = context.getContentResolver();
		int ramindCount = 0 ;
		Cursor cursor = cr.query(ImportFilesTable.CONTENT_URI,
					new String[]{ImportFilesTable.COLUMN_SIZE},
					ImportFilesTable.COLUMN_FILENAME+"=?",
					new String[]{filename},null);
		if(cursor!=null && cursor.getCount()>0){
			ramindCount = cursor.getCount();
		}
		cursor.close();
		if(ramindCount>0)
			return true;
		else
			return false;
	}

	public static String getFoldername(String imageFullpath){
		if(!TextUtils.isEmpty(imageFullpath)){
			int lastIndex = imageFullpath.lastIndexOf(File.separator);
			int lastSecondIndex = imageFullpath.substring(0,lastIndex).lastIndexOf(File.separator);
			return imageFullpath.substring(lastSecondIndex+1, lastIndex);
		}
		else{
			return "";
		}
	}
	public static String getImageSelection(Context context,String date){
		String selection = "(" + MediaStore.Images.Media.DATE_ADDED ;
		if(TextUtils.isEmpty(date)){
			selection += " <= ? ";
		}
		else{
			selection += " >= ? ";
		}
		//IMPORT ALL
		selection += " AND "+ MediaStore.Images.Media.DISPLAY_NAME
				+" NOT LIKE '%.jps%' )";
		return selection;
	}
	public static String getVideoSelection(Context context,String date){
		String selection = MediaStore.Video.Media.DATE_ADDED ;
		if(TextUtils.isEmpty(date)){
			selection += " <= ? ";
		}
		else{
			selection += " >= ? ";
		}

		//IMPORT ALL
//		selection += " AND "+ MediaStore.Video.Media.DISPLAY_NAME
//				+" NOT LIKE '%.jps%' )";
		return selection;
	}
	public static String getAudioSelection(Context context,String date){
		String selection = MediaStore.Audio.Media.DATE_ADDED ;
		if(TextUtils.isEmpty(date)){
			selection += " <= ? ";
		}
		else{
			selection += " >= ? ";
		}
		//IMPORT ALL
		return selection;
	}
	public static String[] getFilePeriods(Context context){
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		String newestDay = null;
		String oldestDay = null;
		
		cursor = cr.query(ImportFilesTable.CONTENT_URI, 
				new String[]{ImportFilesTable.COLUMN_DATE}, 
				null, 
				null, 
				ImportFilesTable.DEFAULT_SORT_ORDER+" LIMIT 1");	
		if(cursor!=null && cursor.getCount()>0){
			cursor.moveToFirst();
			newestDay = cursor.getString(0);
			newestDay = newestDay.substring(0,10);
		}
		cursor = cr.query(ImportFilesTable.CONTENT_URI, 
				new String[]{ImportFilesTable.COLUMN_DATE}, 
				null, 
				null, 
				ImportFilesTable.COLUMN_DATE+" LIMIT 1");	
		if(cursor!=null && cursor.getCount()>0){
			cursor.moveToFirst();
			oldestDay = cursor.getString(0);
			oldestDay = oldestDay.substring(0,10);
		}
		cursor.close();
		return new String[]{oldestDay,newestDay};
	}
	public static long[] getFileInfo(Context context,int type){
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		long count = 0;
		long totalSizes = 0;
		
		cursor = cr.query(ImportFilesTable.CONTENT_URI, 
				new String[]{ImportFilesTable.COLUMN_SIZE}, 
				ImportFilesTable.COLUMN_FILETYPE+"=?", 
				new String[]{String.valueOf(type)}, 
				null);	
		if(cursor!=null && cursor.getCount()>0){
			count = cursor.getCount();
			cursor.moveToFirst();
			for(int i = 0 ; i < count ; i++){
				totalSizes += cursor.getLong(0); 
			}
		}
		cursor.close();
		return new long[]{count,totalSizes};
	}
	public static void autoBackupStart(Context context){
		SharedPreferences pref = context.getSharedPreferences(Constant.PREFS_NAME,Context.MODE_PRIVATE);
		String autoImportBornTime = pref.getString(Constant.PREF_AUTO_IMPORT_BORN_TIME, "");
		if(TextUtils.isEmpty(autoImportBornTime)){
			String bornTime = StringUtil.changeToLocalString(StringUtil.formatDate(new Date()));
			pref.edit().putString(Constant.PREF_AUTO_IMPORT_BORN_TIME, bornTime).commit();
		}
		pref.edit().putBoolean(Constant.PREF_AUTO_IMPORT_ENABLED, true).commit();
	}

	public static int updateBackupStatus(Context context,String filename,String status){
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(ImportFilesTable.COLUMN_STATUS, status);
		return cr.update(ImportFilesTable.CONTENT_URI, cv,ImportFilesTable.COLUMN_FILENAME+"=?" , new String[]{filename});
	}

	public static int addFileIndexForServer(Context context,String serverId){
		String lastTimestamp = null;
		ContentValues[] cvs = null;
		ArrayList<ContentValues> datas = new ArrayList<ContentValues>();
		ContentValues cv = new ContentValues();
		cv.put(BackupDetailsTable.COLUMN_SERVER_ID, serverId);
		cv.put(BackupDetailsTable.COLUMN_STATUS, Constant.IMPORT_FILE_INCLUDED);		
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = cr.query(ServerFilesView.CONTENT_URI, 
				new String[]{ServerFilesView.COLUMN_DATE}, 
				ServerFilesView.COLUMN_SERVER_ID+" =? ", 
				new String[]{serverId}, 
				ServerFilesView.COLUMN_DATE+" DESC LIMIT 1");	
		if(cursor!=null && cursor.getCount()>0){
			cursor.moveToFirst();
			lastTimestamp = cursor.getString(0);
		}		
		String where = null;
		String[] whereArgs = null;
		if(TextUtils.isEmpty(lastTimestamp)){
			where = ImportFilesTable.COLUMN_STATUS+"!=? AND "+ImportFilesTable.COLUMN_STATUS+"!=? ";
			whereArgs = new String[]{Constant.IMPORT_FILE_DELETED,Constant.IMPORT_FILE_EXCLUDE};
		}
		else{
			where = ImportFilesTable.COLUMN_DATE+">=? "+
					ImportFilesTable.COLUMN_STATUS+"!=? AND "+ImportFilesTable.COLUMN_STATUS+"!=? ";
			whereArgs = new String[]{lastTimestamp,Constant.IMPORT_FILE_DELETED,Constant.IMPORT_FILE_EXCLUDE};
		}
		
		cursor = cr.query(ImportFilesTable.CONTENT_URI, 
				new String[]{
				ImportFilesTable.COLUMN_FILENAME}, 
				where, 
				whereArgs, 
				ImportFilesTable.COLUMN_DATE);	
		if(cursor!=null && cursor.getCount()>0){
			cursor.moveToFirst();
			int count = cursor.getCount();
			for(int i = 0 ; i<count ;i++){
				cv.put(BackupDetailsTable.COLUMN_FILENAME,cursor.getString(0));
				datas.add(cv);
				cursor.moveToNext();
			}
		}
		cursor.close();
		int result = 0;
		if(datas.size()>0){
			//SAVE TO DB
			cvs = new ContentValues[datas.size()];
			datas.toArray(cvs);
			result = cr.bulkInsert(BackupDetailsTable.CONTENT_URI, cvs);
			Log.d(TAG,result+ " Files Imported!");
		}
		datas= null;

		return result;
	}
	public static int updateBackupStatus(Context context,String serverId,String filename,String status){
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(BackupDetailsTable.COLUMN_STATUS, status);
		return cr.update(BackupDetailsTable.CONTENT_URI, cv,
				BackupDetailsTable.COLUMN_SERVER_ID+" =? AND"+
				BackupDetailsTable.COLUMN_FILENAME+" =?" , new String[]{serverId,filename});
	}
	public static int[] getBackupProgressInfo(Context context,String serverId){
    	int totalCount = 0;
    	int backupedCount = 0;
		String lastBackupTimestamp = null;
    	//select from serverFiles 
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		cursor = cr.query(BackupedServersTable.CONTENT_URI, 
				new String[]{
				BackupedServersTable.COLUMN_END_DATETIME,
				}, 
				BackupedServersTable.COLUMN_SERVER_ID+"=?", 
				new String[]{serverId},null);	
		if(cursor!=null && cursor.getCount()>0){
			cursor.moveToFirst();
			lastBackupTimestamp = cursor.getString(0); 
		}	
		String where = ImportFilesTable.COLUMN_STATUS+"!=? AND "+ImportFilesTable.COLUMN_STATUS+"!=? ";
		String[] whereArgs = new String[]{Constant.IMPORT_FILE_DELETED,Constant.IMPORT_FILE_EXCLUDE};

		//GET TOTAL COUUNT
		cursor = cr.query(ImportFilesTable.CONTENT_URI, 
				new String[]{
				ImportFilesTable.COLUMN_DATE}, 
				where,whereArgs,null);	
		if(cursor!=null && cursor.getCount()>0){
			cursor.moveToFirst();
			totalCount = cursor.getCount();
		}
        //GET BACKUPED COUNT
		if(!TextUtils.isEmpty(lastBackupTimestamp)){
			where = ImportFilesTable.COLUMN_DATE+"<=? AND "+
					ImportFilesTable.COLUMN_STATUS+"!=? AND "+ImportFilesTable.COLUMN_STATUS+"!=? ";
			whereArgs = new String[]{lastBackupTimestamp,Constant.IMPORT_FILE_DELETED,Constant.IMPORT_FILE_EXCLUDE};
			cursor = cr.query(ImportFilesTable.CONTENT_URI, 
					new String[]{
					ImportFilesTable.COLUMN_DATE}, 
					where, 
					whereArgs, 
					null);	
			if(cursor!=null && cursor.getCount()>0){
				cursor.moveToFirst();
				backupedCount = cursor.getCount();
			}
		}
		cursor.close();
		return new int[]{backupedCount,totalCount};
	}
	public static boolean needToBackup(Context context,String serverId){
		int[] backupAndTotalCount = BackupLogic.getBackupProgressInfo(context, serverId);
		if(backupAndTotalCount[0]!=backupAndTotalCount[1]){
			return true;
		}
		else{
			return false;
		}
	}
	public static boolean canBackup(Context context){
		if(NetworkUtil.isWifiNetworkAvailable(context) && RuntimeConfig.OnWebSocketOpened){
			return true;
		}
		else{
			return false;
		}
	}
	public static void backupFiles(Context context,String serverId) {
		FIleTransferEntity entity = new FIleTransferEntity();
		byte[] buffer = new byte[256 * Constant.K_BYTES];
		byte[] finalBuffer = null;		
		InputStream ios = null;
		boolean isSuccesed = false;
		String filename = null;
		int filetype = 0;
    	String lastBackupTimestamp = null;
    	//select from serverFiles 
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = cr.query(BackupedServersTable.CONTENT_URI, 
				new String[]{
				BackupedServersTable.COLUMN_END_DATETIME,
				}, 
				BackupedServersTable.COLUMN_SERVER_ID+"=?", 
				new String[]{serverId},null);	

		if(cursor!=null && cursor.getCount()>0){
			cursor.moveToFirst();
			lastBackupTimestamp = cursor.getString(0); 
		}	
		
		cursor = cr.query(ImportFilesTable.CONTENT_URI, 
				new String[]{
				ImportFilesTable.COLUMN_FILENAME,
				ImportFilesTable.COLUMN_MIMETYPE,
				ImportFilesTable.COLUMN_SIZE,
				ImportFilesTable.COLUMN_FOLDER,
				ImportFilesTable.COLUMN_DATE,
				ImportFilesTable.COLUMN_FILETYPE}, 
				ImportFilesTable.COLUMN_DATE+">=?", 
				new String[]{lastBackupTimestamp}, 
				ImportFilesTable.COLUMN_DATE);	
		if(cursor!=null && cursor.getCount()>0){
			cursor.moveToFirst();
			int count = cursor.getCount();
			for(int i = 0 ; i<count ;i++){
				// send file index for start
				entity.action = Constant.WS_ACTION_FILE_START;
				filename = cursor.getString(0);
				entity.fileName = StringUtil.getFilename(filename);
				entity.mimetype = cursor.getString(1);
				entity.fileSize = cursor.getString(2);
				entity.folder = cursor.getString(3);				
				entity.datetime = cursor.getString(4);
				filetype = cursor.getInt(5);
				try {
					if(canBackup(context)){
						RuntimeWebClient.send(RuntimeConfig.GSON.toJson(entity));
					}
					else{
						isSuccesed = true;
						break;
					}
					// send file binary
					ios = new FileInputStream(new File(filename));
					int read = 0;
					while ((read = ios.read(buffer)) != -1) {
						if(canBackup(context)){
							if (read != buffer.length) {
								finalBuffer = new byte[read];
								finalBuffer = Arrays.copyOf(buffer, read);
								RuntimeWebClient.sendFile(finalBuffer);
							} else {
								RuntimeWebClient.sendFile(buffer);
							}
						}
						else{
							isSuccesed = false;
							break;
						}
					}					
					// send file index for end
					if(canBackup(context)){
						entity.action = Constant.WS_ACTION_FILE_END;
						RuntimeWebClient.send(RuntimeConfig.GSON.toJson(entity));
						isSuccesed = true;
					}else{
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
					if(isSuccesed){
						ServersLogic.updateServerStatus(context, entity.datetime,filetype, serverId);
					}
					isSuccesed = false;
				}
				cursor.moveToNext();
				if(!canBackup(context)){
					break;//cut off for loop
				}
			}
		}
		cursor.close();
	}
}