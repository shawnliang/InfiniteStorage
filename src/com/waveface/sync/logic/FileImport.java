package com.waveface.sync.logic;

import java.io.File;
import java.util.ArrayList;
import java.util.TreeSet;

import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.provider.MediaStore;
import android.text.TextUtils;

import com.waveface.sync.Constant;
import com.waveface.sync.db.ImportFilesTable;
import com.waveface.sync.util.Log;
import com.waveface.sync.util.StringUtil;

public class FileImport {
	private static String TAG = FileImport.class.getSimpleName();

	public static void scanFileForImport(Context context,int type){
		String currentDate = "";
		String cursorDate = "";
		long refCursorDate = 0 ;
		long dateTaken = -1;
		long dateModified = 0;
		long dateAdded = 0;
		String fileSize = null;
		String folderName = null;
		String mediaData = null;
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
		
		String selectionArgs[] = { currentDate };
		
		if(type == Constant.TYPE_IMAGE){//IMAGES
			projection = new String[]{
					MediaStore.Images.Media.DATA,
					MediaStore.Images.Media.DATE_TAKEN,
					MediaStore.Images.Media.DISPLAY_NAME,
					MediaStore.Images.Media.DATE_ADDED,
					MediaStore.Images.Media.DATE_MODIFIED,
					MediaStore.Images.Media.SIZE,
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
				imageId = cursor.getString(6);		
				folderName =  getFoldername(mediaData);
				
				if (dateTaken != -1) {
					refCursorDate = dateTaken / 1000;
				} else if (dateModified != -1) {
					refCursorDate = dateModified ;
				} else if (dateAdded != -1) {
					refCursorDate = dateAdded ;
				}
				cursorDate = StringUtil.getConverDate(refCursorDate);
				Log.d(TAG, "cursorDate ==>" + cursorDate);
				Log.d(TAG, "Filename ==>" + mediaData);
				
				if(!duplicateFilename(context,mediaData) && !filenamesSet.contains(mediaData)){
					cv = new ContentValues();
					cv.put(ImportFilesTable.COLUMN_FILENAME, mediaData);
					cv.put(ImportFilesTable.COLUMN_SIZE, fileSize);
					cv.put(ImportFilesTable.COLUMN_DATE, cursorDate);
					cv.put(ImportFilesTable.COLUMN_IMPORTED, Constant.IMPORT_FILE_INCLUDED);
					cv.put(ImportFilesTable.COLUMN_FILETYPE, type);					
					cv.put(ImportFilesTable.COLUMN_FOLDER, folderName);
					cv.put(ImportFilesTable.COLUMN_IMAGE_ID, imageId);
					filenamesSet.add(mediaData);
					datas.add(cv);
				}
				cursor.moveToNext();
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
		}
		cursor = cr.query(ImportFilesTable.CONTENT_URI, 
				new String[]{ImportFilesTable.COLUMN_DATE}, 
				null, 
				null, 
				ImportFilesTable.COLUMN_DATE+" LIMIT 1");	
		if(cursor!=null && cursor.getCount()>0){
			cursor.moveToFirst();
			oldestDay = cursor.getString(0);
		}
		cursor.close();
		return new String[]{newestDay,oldestDay};
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
	
}
