package com.waveface.sync.logic;

import java.io.File;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.Locale;
import java.util.Set;
import java.util.TreeSet;

import android.app.AlarmManager;
import android.app.PendingIntent;
import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.database.Cursor;
import android.provider.MediaStore;
import android.text.TextUtils;

import com.waveface.sync.Constant;
import com.waveface.sync.RuntimePlayer;
import com.waveface.sync.db.ImportFilesTable;
import com.waveface.sync.db.ImportTable;
import com.waveface.sync.entity.ImportFile;
import com.waveface.sync.service.AlarmReceiver;
import com.waveface.sync.util.Log;
import com.waveface.sync.util.StringUtil;

public class PhotoImport {
	private static final String TAG = PhotoImport.class.getSimpleName();
	public static String DATE_FORMAT_POST = "yyyy-MM-dd'T'HH:mm:ss'Z'";
	public static String DATE_FOMAT_LIMIT = "yyyyMMddHHmmss";
	public static final int GROUP_LIMIT = 5;


	public static boolean hasPostCreated(Context context,String groupId,String timestamp){
		boolean posted = false;
//		ContentResolver cr = context.getContentResolver();
//		Cursor cursor = cr.query(PostTable.CONTENT_URI,
//				new String[]{PostTable.COLUMN_DB_ID},
//				PostTable.COLUMN_TIMESTAMP+"=? AND "+PostTable.COLUMN_GROUP_ID +"=?",
//				new String[]{timestamp,groupId},null);
//		if(cursor!=null && cursor.getCount()>0)
//			posted = true;
//		cursor.close();
		return posted;
	}
	public static void uploadImagesOfDay(Context context,String groupId,String[] objectIds,String[] filenames, String mPostId,String importTime,String importDay) {
		if (filenames != null && filenames.length > 0
				&& !TextUtils.isEmpty(filenames[0])) {
			//UploadMeta
			uploadMetadatas(context,objectIds,filenames);
			//scan  filenames and objectId again for avoid file deleted condition
			HashMap<String,String[]> datas = nextImportFilenamesByDay(context,importDay);
//			objectIds = datas.get(Constant.PARAM_OBJECT_IDS);
//			filenames = datas.get(Constant.PARAM_FILENAMES);
			//Update import status
			if(objectIds==null)
				return ;
			for(int i = 0 ; i < objectIds.length ; i++){
				updateImportStatus(context, objectIds[i], true,false,false);
			}
			// Insert ImageQueue Table
		}
	}
	public static boolean uploadMetadatas(Context context,String[] objectIds,String[] filenames){
		boolean uploadMeataSuccessed = false;
//		int timezone = StringUtil.getTimezoneMinute();
//		String sessionToken =
//				context.getSharedPreferences(Constant.PREFS_NAME,Context.MODE_PRIVATE)
//				.getString(Constant.PREF_SESSION_TOKEN, "");
//		if(HttpInvoker.isNetworkAvailable(context) && !TextUtils.isEmpty(sessionToken)){
//			PhotoUploadMetadataEntity[] entities = null;
//			int UPLOAD_COUNT = 20 ;
//			int quotient = objectIds.length / UPLOAD_COUNT;
//			int remainder = objectIds.length % UPLOAD_COUNT;
//			int totalCount = quotient;
//			if(remainder!=0){
//				totalCount++;
//			}
//			int arrayLength = 0;
//			String[] objectIdsInLoop = null;
//			String[] filenamesInLoop = null;
//			for(int i = 0; i < totalCount; i++){
//				sessionToken =
//						context.getSharedPreferences(Constant.PREFS_NAME,Context.MODE_PRIVATE)
//						.getString(Constant.PREF_SESSION_TOKEN, "");
//				if(TextUtils.isEmpty(sessionToken)){
//					uploadMeataSuccessed = false;
//					break;
//				}
//				if(i!= (totalCount-1)){
//					arrayLength = UPLOAD_COUNT;
//				}
//				else{
//					arrayLength = remainder;
//				}
//				objectIdsInLoop = new String[arrayLength];
//				filenamesInLoop = new String[arrayLength];
//				for(int j = 0 ; j< arrayLength;j++){
//					objectIdsInLoop[j] = objectIds[UPLOAD_COUNT*i+j];
//					filenamesInLoop[j] = filenames[UPLOAD_COUNT*i+j];
//				}
//				entities = new PhotoUploadMetadataEntity[objectIdsInLoop.length];
//				for(int j = 0 ; j < objectIdsInLoop.length;j++){
//					entities[j] = new PhotoUploadMetadataEntity();
//					entities[j].object_id = objectIdsInLoop[j];
//					entities[j].type = Constant.TYPE_ATTACHMENT_PHOTO;
//					entities[j].file_path = filenamesInLoop[j];
//					entities[j].file_name = StringUtil.getFilename(filenamesInLoop[j]);
//					entities[j].timezone = timezone;
//					entities[j].eventTime = GeoUtil.getEventTime(filenamesInLoop[j]);
//				}
//				try {
//					 sessionToken =
//							context.getSharedPreferences(Constant.PREFS_NAME,Context.MODE_PRIVATE)
//							.getString(Constant.PREF_SESSION_TOKEN, "");
//					if(!TextUtils.isEmpty(sessionToken)){
//						uploadMeataSuccessed = false;
//					}
//					else{
//						String jsonOutput =
//								WammerAPIHandler.uploadMetadtas(sessionToken,getMultiExifJsonStirng(context,entities));
//						Log.d(TAG, "Update Metadata Response:"+jsonOutput);
//						if(!TextUtils.isEmpty(jsonOutput)){
//							MultipleUploadMetaResponse response = RuntimePlayer.GSON.fromJson(jsonOutput, MultipleUploadMetaResponse.class);
//							if(response!=null){
//								if(response.status == HttpStatus.SC_OK){
//									uploadMeataSuccessed = true ;
//								}
//								else{
//									Log.d(TAG, "API RETURN:" +response.apiReturnCode+",Message:" +response.apiReturnMessage);
//								}
//							}
//						}
//					}
//
//				} catch (WammerServerException e) {
//					e.printStackTrace();
//				}
//			}
//		}
		return uploadMeataSuccessed;
	}
	/**
	 * Get Folders which are exclusive for Auto Import
	 * @param context
	 * @param type
	 * @param enable
	 * @return
	 */
	public static String queryImportFolders(Context context,String type,String enable){
		ContentResolver cr = context.getContentResolver();
		StringBuilder folders = new StringBuilder();
		Cursor cursor = cr.query(ImportTable.CONTENT_URI,
				new String[]{ImportTable.COLUMN_FOLDER_NAME}
				,ImportTable.COLUMN_TYPE+"=? AND "+
				 ImportTable.COLUMN_ENABLE+" =?"
				, new String[]{type,enable}, null);
		if(cursor.getCount()>0){
			cursor.moveToFirst();
			for(int i = 0 ; i < cursor.getCount(); i++){
				if(!TextUtils.isEmpty(cursor.getString(0))){
					folders.append("'"+cursor.getString(0)+"',");
				}
				cursor.moveToNext();
			}
		}
		cursor.close();
		String excludeFoldersString = folders.toString();
		if(excludeFoldersString.endsWith(",")){
			excludeFoldersString = excludeFoldersString.substring(0, excludeFoldersString.length()-1);
		}
		return excludeFoldersString;
	}
	public static void autoImportBorn(Context context){
		SharedPreferences pref = context.getSharedPreferences(Constant.PREFS_NAME,Context.MODE_PRIVATE);
		String autoImportBornTime = pref.getString(Constant.PREF_AUTO_IMPORT_BORN_TIME, "");
		if(TextUtils.isEmpty(autoImportBornTime)){
			String bornTime = StringUtil.changeToLocalString(StringUtil.formatDate(new Date()));
			pref.edit().putString(Constant.PREF_AUTO_IMPORT_BORN_TIME, bornTime).commit();
		}
		pref.edit().putBoolean(Constant.PREF_AUTO_IMPORT_ENABLED, true).commit();
	}
	public static boolean enableAutoImport(Context context){
		SharedPreferences pref = context.getSharedPreferences(Constant.PREFS_NAME,Context.MODE_PRIVATE);

		if(!TextUtils.isEmpty(pref.getString(Constant.PREF_SESSION_TOKEN, ""))
				&& pref.getBoolean(Constant.PREF_AUTO_IMPORT_ENABLED, false)){
			return true;
		}
		else{
			return false;
		}
	}


	public static void setDefaultImportALarm(Context context) {
		 setImportAlarmCondition(context);
		// get a Calendar object with current time
		 Calendar cal = Calendar.getInstance();
		 Date curr=new Date();
		 cal.setTime(curr);
		 cal.set(Calendar.MINUTE, 1);
		 long interval = 1 *60 * 1000;
		 Intent intent = new Intent(context, AlarmReceiver.class);
		 PendingIntent sender = PendingIntent.getBroadcast(context, Constant.AUTO_IMPORT_ID, intent, PendingIntent.FLAG_UPDATE_CURRENT);

		 // Get the AlarmManager service
		 AlarmManager am = (AlarmManager) context.getSystemService(Context.ALARM_SERVICE);
		 am.setRepeating(AlarmManager.RTC_WAKEUP, cal.getTimeInMillis(),interval, sender);
	}

	public static ImportFile[] nextImportData(Context context,String date){
		ContentResolver cr = context.getContentResolver();
		ImportFile[] files = null;
		ImportFile instance = null;
		Cursor cursor = cr.query(ImportFilesTable.CONTENT_URI,
				new String[]{
				ImportFilesTable.COLUMN_SIZE,
				ImportFilesTable.COLUMN_DATE,
				ImportFilesTable.COLUMN_FILENAME},
				ImportFilesTable.COLUMN_IMPORTED+"=?"+
				" AND "+ImportFilesTable.COLUMN_DATE+"=?"
				, new String[]{Constant.IMPORT_FILE_INCLUDED,date}, null);
		if(cursor.getCount()>0){
			files = new ImportFile[cursor.getCount()];
			cursor.moveToFirst();
			for(int i = 0 ; i < cursor.getCount(); i++){
				instance = new ImportFile();
				instance.objectId = cursor.getString(0);
				instance.date = cursor.getString(1);
				instance.filename = cursor.getString(2);
				cursor.moveToNext();
			}
		}
		cursor.close();
		return files;
	}
	public static HashMap<String,String[]> nextImportFilenamesByDay(Context context,String date){
		ContentResolver cr = context.getContentResolver();
		String[] objectIds = null;
		String[] files = null;
		Cursor cursor = cr.query(ImportFilesTable.CONTENT_URI,
				new String[]{
				ImportFilesTable.COLUMN_SIZE,
				ImportFilesTable.COLUMN_FILENAME},
				ImportFilesTable.COLUMN_IMPORTED+"=?"+
				" AND "+ImportFilesTable.COLUMN_DATE+"=?"
				, new String[]{Constant.IMPORT_FILE_INCLUDED,date}, null);
		if(cursor.getCount()>0){
			objectIds = new String[cursor.getCount()];
			files = new String[cursor.getCount()];
			cursor.moveToFirst();
			for(int i = 0 ; i < cursor.getCount(); i++){
				objectIds[i] = cursor.getString(0);
				files[i] = cursor.getString(1);
				cursor.moveToNext();
			}
		}
		cursor.close();
		HashMap<String,String[]> datas = new HashMap<String,String[]>();
		datas.put(Constant.PARAM_OBJECT_IDS, objectIds);
		datas.put(Constant.PARAM_FILENAMES, files);
		return datas;
	}

	public static String nextImportDay(Context context){
		ContentResolver cr = context.getContentResolver();
		String importDate = null;
		Cursor cursor = cr.query(ImportFilesTable.CONTENT_URI,
				new String[]{
				ImportFilesTable.COLUMN_DATE},
				ImportFilesTable.COLUMN_IMPORTED+"=? "
				, new String[]{Constant.IMPORT_FILE_INCLUDED}, ImportFilesTable.DEFAULT_SORT_ORDER+" LIMIT 1");
		if(cursor.getCount()>0){
			cursor.moveToFirst();
			importDate = cursor.getString(0);
		}
		cursor.close();
		return importDate;
	}
	public static void displayImportStatus(Context context,String importDay){
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = cr.query(ImportFilesTable.CONTENT_URI,
				new String[]{
				ImportFilesTable.COLUMN_DATE,
				ImportFilesTable.COLUMN_SIZE,
				ImportFilesTable.COLUMN_FILENAME,
				ImportFilesTable.COLUMN_IMPORTED},
				ImportFilesTable.COLUMN_DATE+"=?", new String[]{importDay}, null);
		if(cursor.getCount()>0){
			int count = cursor.getCount();
			cursor.moveToFirst();
			Log.d(TAG, "DATE:"+importDay);
			for(int i = 0 ; i < count ; i++){
				Log.d(TAG, "FILENAME:"+cursor.getString(2)+",IMPORT STATUS:"+cursor.getString(3));
				cursor.moveToNext();
			}
		}
		cursor.close();
	}
	public static void importFiles(Context context){
		if(enableAutoImport(context) == false){
			return;
		}
		if(RuntimePlayer.NewerImportProcessing == false){
			scanFileForImport(context,true);
		}
		String importDate = nextImportDay(context);
		if(!TextUtils.isEmpty(importDate)){
			displayImportStatus(context,importDate);
			boolean goNext = doImportJob(context, importDate);
			if(goNext){
				importFiles(context);
			}
		}
	}
	public static boolean hasMetaStatus(Context context){
		ContentResolver cr = context.getContentResolver();
		int ramindCount = 0 ;
		Cursor cursor = cr.query(ImportFilesTable.CONTENT_URI,
					new String[]{ImportFilesTable.COLUMN_DATE},
					ImportFilesTable.COLUMN_IMPORTED+"=?",
					new String[]{Constant.IMPORT_FILE_UPLOAD_META},null);
		if(cursor!=null && cursor.getCount()>0){
			ramindCount = cursor.getCount();
		}
		cursor.close();
		if(ramindCount>0)
			return true;
		else
			return false;
	}

	private static boolean doImportJob(Context context, String importDay) {
		Log.d(TAG, "Import Date:"+importDay);
		boolean goNext = false;
		String groupId = context.getSharedPreferences(Constant.PREFS_NAME,
				Context.MODE_PRIVATE).getString(Constant.PREF_GROUP_ID, "");
		if(TextUtils.isEmpty(groupId))
			return goNext;
		//add image id to old schema
		addImageInfo(context);
		// photo import for insert to post import time
		String currentTimestamp = StringUtil.changeToLocalString(StringUtil
				.formatDate(new Date()));
		String postTimestamp = StringUtil
				.changeToLocalString(importDay);
		if(!hasPostCreated(context, groupId, postTimestamp)){
			Log.d(TAG, "CREATE POST ");
			//QUERY POST'S TIMESTAMP FIRST TO AVOID DUPLICATE DATA
			String drafPostId = StringUtil.getUUID();
			Log.d(TAG, "postTimestamp ==>" + postTimestamp);
			HashMap<String,String[]> datas = nextImportFilenamesByDay(context,importDay);
			String[] objectIds = datas.get(Constant.PARAM_OBJECT_IDS);
			String[] filenames = datas.get(Constant.PARAM_FILENAMES);
			// uploadImage for one day
			if(objectIds==null)
				return true;
			uploadImagesOfDay(context, groupId,objectIds,filenames,drafPostId, currentTimestamp,importDay);
			try {
				Thread.sleep(100);
			} catch (InterruptedException e) {
				e.printStackTrace();
			}
			//Convert String array to String
			StringBuilder buf = new StringBuilder();
			for(int i = 0 ; i < filenames.length;i++){
				buf.append(filenames[i]);
				if(i!= (filenames.length-1)){
					buf.append(",");
				}
			}
			// Create post
//			FlowUtil.createPost(context, PostTable.TYPE_IMPORT,
//					"\n", buf.toString(), drafPostId,
//					postTimestamp, null, null, true, currentTimestamp);
			goNext = true;
		}
		else{
			Log.d(TAG, "UPDATE STATUS ");
			//STEP1:CHECKOUT unupload Image Meta
			uploadMissedImageMeta(context,importDay);
			//STEP2:update import statue form 0 -->1
			updateImportStatus(context,importDay,Constant.IMPORT_FILE_INCLUDED,Constant.IMPORT_FILE_UPLOAD_META);
		}
		return goNext;
	}

	public static boolean uploadMissedImageMeta(Context context,String importDay){
		boolean jobFinished = false;
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = cr.query(ImportFilesTable.CONTENT_URI,
				new String[]{ImportFilesTable.COLUMN_SIZE,ImportFilesTable.COLUMN_FILENAME},
				ImportFilesTable.COLUMN_DATE+"=? AND "+ImportFilesTable.COLUMN_IMPORTED+"=?",
				new String[]{importDay,Constant.IMPORT_FILE_INCLUDED}, null);

		String[] objectIds = null;
		String[] filenames = null;
		if(cursor!=null && cursor.getCount()>0){
			int count = cursor.getCount();
			objectIds = new String[count];
			filenames = new String[count];
			cursor.moveToFirst();
			for(int i = 0 ; i<count ;i++){
				objectIds[i] = cursor.getString(0);
				filenames[i] = cursor.getString(1);
			}
			jobFinished = uploadMetadatas(context,objectIds,filenames);
		}
		else{
			jobFinished = true;
		}
		if (cursor!=null)
			cursor.close();
		return jobFinished;
	}



	public static int updateImportStatus(Context context,String importDay,String nowStatus,String newStatus){
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(ImportFilesTable.COLUMN_IMPORTED, newStatus);
		return cr.update(ImportFilesTable.CONTENT_URI, cv,
				ImportFilesTable.COLUMN_DATE+"=? AND "+ImportFilesTable.COLUMN_IMPORTED+"=?" , new String[]{importDay,nowStatus});
	}
	public static int updateImportStatus(Context context,String nowStatus,String newStatus){
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(ImportFilesTable.COLUMN_IMPORTED, newStatus);
		return cr.update(ImportFilesTable.CONTENT_URI, cv,
				ImportFilesTable.COLUMN_IMPORTED+"=?" , new String[]{nowStatus});
	}

	public static int updateImportStatus(Context context,String objectId,boolean uploadMeta,boolean uploadThumb,boolean imported){
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		if(uploadMeta){
			cv.put(ImportFilesTable.COLUMN_IMPORTED, Constant.IMPORT_FILE_UPLOAD_META);
		}
		else if(uploadThumb){
			cv.put(ImportFilesTable.COLUMN_IMPORTED, Constant.IMPORT_FILE_UPLOAD_THUMB);
		}
		else if(imported){
			cv.put(ImportFilesTable.COLUMN_IMPORTED, Constant.IMPORT_FILE_BACKUP);
		}
		else{
			cv.put(ImportFilesTable.COLUMN_IMPORTED, Constant.IMPORT_FILE_INCLUDED);
		}
		return cr.update(ImportFilesTable.CONTENT_URI, cv,ImportFilesTable.COLUMN_SIZE+"=?" , new String[]{objectId});
	}
	public static int updateImportObjectId(Context context,String objectId,String newObjectId){
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(ImportFilesTable.COLUMN_SIZE, newObjectId);
		return cr.update(ImportFilesTable.CONTENT_URI, cv,ImportFilesTable.COLUMN_SIZE+"=?" , new String[]{objectId});
	}

	public static String getImageSelection(Context context,boolean newer){
		//OLDER VERSION
		String excludeFoldersString = queryImportFolders(context,"P","0");
		excludeFoldersString = MediaStore.Images.Media.BUCKET_DISPLAY_NAME + " NOT IN ("
								+excludeFoldersString+")";
		//NEW VERSION
//		String includeFoldersString = queryImportFolders(context,"P","1");
//		includeFoldersString = MediaStore.Images.Media.BUCKET_DISPLAY_NAME + " IN ("
//								+includeFoldersString+")";
		String selection = "";
		if(newer){
			selection = "(" + MediaStore.Images.Media.DATE_ADDED + " > ? ";
		}
		else{
			selection = "(" + MediaStore.Images.Media.DATE_ADDED + " <= ? ";
		}
		//IMPORT ALL
		selection += " AND " + excludeFoldersString
				+" AND "+ MediaStore.Images.Media.DISPLAY_NAME
				+" NOT LIKE '%.jps%' )";

		//IMPORT CAMERA ONLY
//		selection += " AND " + includeFoldersString
//					+" AND "+ MediaStore.Images.Media.DISPLAY_NAME
//					+" NOT LIKE '%.jps%' )";

		return selection;
	}

	public static void scanFilesForImportFolder(Context context,String folderName){
		boolean scanForFolder = false;
		ContentResolver cr = context.getContentResolver();
		Cursor cursor =
				cr.query(ImportFilesTable.CONTENT_URI,
						new String[]{ImportFilesTable.COLUMN_FOLDER},
						ImportFilesTable.COLUMN_FOLDER +"=?",
						new String[]{folderName},
						ImportFilesTable.DEFAULT_SORT_ORDER+" LIMIT 1");
		//IF THIS FOLDER WAS NOT IMPORTED
		if(cursor!=null && cursor.getCount()==0){
			scanForFolder = true;
		}
		cursor.close();
		if(scanForFolder){
			String postDate = "";
			long refCursorDate = 0 ;
			long dateTaken = 0;
			long dateModified = 0;
			long dateAdded = 0;
			String mediaData = null;
			String imageId = null;

			String[] projection = {
					MediaStore.Images.Media.DATA,
					MediaStore.Images.Media.DATE_TAKEN,
					MediaStore.Images.Media.DISPLAY_NAME,
					MediaStore.Images.Media.DATE_ADDED,
					MediaStore.Images.Media.DATE_MODIFIED,
					MediaStore.Images.Media._ID};

			String selection =  MediaStore.Images.Media.BUCKET_DISPLAY_NAME + " = ?"
								+" AND "+ MediaStore.Images.Media.DISPLAY_NAME
								+" NOT LIKE '%.jps%' ";

			String selectionArgs[] = { folderName };
			Log.d(TAG, "selection => " + selection);

			cursor =context.getContentResolver().query(
					MediaStore.Images.Media.EXTERNAL_CONTENT_URI, projection,
					selection, selectionArgs,
					MediaStore.Images.Media.DATE_TAKEN+" DESC");
			//With Images
			if(cursor!=null && cursor.getCount() > 0){
				int fileNum = cursor.getCount();
				ContentValues[] cvs = null;
				ContentValues cv = null;
				cursor.moveToFirst();
				ArrayList<ContentValues> datas = new ArrayList<ContentValues>();
				TreeSet<String> filenamesSet = new TreeSet<String>();
				for (int counter = 0; counter < fileNum ; counter++) {
					cv = new ContentValues();
					mediaData = cursor.getString(cursor.getColumnIndex(MediaStore.Images.Media.DATA));
					dateTaken = cursor.getLong(cursor.getColumnIndex(MediaStore.Images.Media.DATE_TAKEN));
					dateModified = cursor.getLong(cursor.getColumnIndex(MediaStore.Images.Media.DATE_MODIFIED));
					dateAdded = cursor.getLong(cursor.getColumnIndex(MediaStore.Images.Media.DATE_ADDED));
					imageId = cursor.getString(5);
					folderName =  getFoldername(mediaData);

					if (dateTaken != -1) {
						refCursorDate = dateTaken / 1000;
					} else if (dateModified != -1) {
						refCursorDate = dateModified;
					} else if (dateAdded != -1) {
						refCursorDate = dateAdded;
					}
					if(counter==0){
						postDate = StringUtil.getConverDate(refCursorDate,DATE_FORMAT_POST);
					}
					if( counter!=0 && counter%GROUP_LIMIT ==0){
						postDate = StringUtil.getConverDate(refCursorDate,DATE_FORMAT_POST);
						//SAVE TO DB
						cvs = new ContentValues[datas.size()];
						datas.toArray(cvs);
						int result = cr.bulkInsert(ImportFilesTable.CONTENT_URI, cvs);
						Log.d(TAG,result+ " Files Imported!");
						filenamesSet.clear();
						datas.clear();
					}
					if(!duplicateFilename(context,mediaData) && !filenamesSet.contains(mediaData)){
						cv.put(ImportFilesTable.COLUMN_FILENAME, mediaData);
						cv.put(ImportFilesTable.COLUMN_SIZE, StringUtil.getUUID());
						cv.put(ImportFilesTable.COLUMN_DATE, postDate);
						cv.put(ImportFilesTable.COLUMN_IMPORTED, Constant.IMPORT_FILE_INCLUDED);
						cv.put(ImportFilesTable.COLUMN_FOLDER, folderName);
						cv.put(ImportFilesTable.COLUMN_IMAGE_ID, imageId);
						filenamesSet.add(mediaData);
						datas.add(cv);
					}
					if(counter == fileNum-1){
						if(datas.size()>0){
							//SAVE TO DB
							cvs = new ContentValues[datas.size()];
							datas.toArray(cvs);
							int result = cr.bulkInsert(ImportFilesTable.CONTENT_URI, cvs);
							Log.d(TAG,result+ " Files Imported!");
						}
					}
					else{
						cursor.moveToNext();
					}
				}
				filenamesSet = null;
				datas= null;
			}
			if (cursor!=null){
				cursor.close();
			}
		}
	}

	public static synchronized void scanFileForImport(Context context,boolean newer){
		if(newer){
			RuntimePlayer.NewerImportProcessing = true;
		}
		else{
			RuntimePlayer.OlderImportProcessing = true;
		}
		SharedPreferences prefs = context.getSharedPreferences(Constant.PREFS_NAME,
				Context.MODE_PRIVATE);
		String currentDate = "";
		SimpleDateFormat sdfLimit = new SimpleDateFormat(DATE_FOMAT_LIMIT,Locale.US);
		String cursorDate = "";
		long refCursorDate = 0 ;
		long dateTaken = 0;
		long dateModified = 0;
		long dateAdded = 0;
		String fileSize = null;
		String mediaData = null;
		String imageId = null;
		String folderName = null;
		String autoImportBornTime = null ;

		String[] projection = {
				MediaStore.Images.Media.DATA,
				MediaStore.Images.Media.DATE_TAKEN,
				MediaStore.Images.Media.DISPLAY_NAME,
				MediaStore.Images.Media.DATE_ADDED,
				MediaStore.Images.Media.DATE_MODIFIED,
				MediaStore.Images.Media.SIZE,
				MediaStore.Images.Media._ID};

		autoImportBornTime = prefs.getString(Constant.PREF_AUTO_IMPORT_BORN_TIME, "");
		if(newer){
			String lastPostTime = "";
			//Change to Query Newest date
			Cursor cursor =context.getContentResolver().query(
					ImportFilesTable.CONTENT_URI,
					new String[]{ImportFilesTable.COLUMN_DATE},
					null, null, ImportFilesTable.DEFAULT_SORT_ORDER+" LIMIT 1");
			if(cursor!=null && cursor.getCount()>0){
				cursor.moveToFirst();
				lastPostTime = StringUtil.changeToLocalString(cursor.getString(0));

			}
			cursor.close();
			if (lastPostTime.length() == 0) {
				currentDate = StringUtil.getEndDate(sdfLimit.format(StringUtil
						.parseDate(autoImportBornTime)));
			} else {
				if(StringUtil.before(lastPostTime, autoImportBornTime)){
					lastPostTime = autoImportBornTime;
				}
				currentDate = StringUtil.getEndDate(sdfLimit.format(StringUtil
						.parseDate(lastPostTime)));
			}
		}
		else{
			currentDate = StringUtil.getEndDate(sdfLimit.format(StringUtil
					.parseDate(autoImportBornTime)));
		}

		Log.w(TAG, " currentDate = " + currentDate);
		if(!TextUtils.isEmpty(currentDate) && currentDate.length()>0){
		Log.w(TAG, " currentDate is " +StringUtil
				.getConverDate(Long.parseLong(currentDate)));
		}

		String selection =  getImageSelection(context,newer);

		String selectionArgs[] = { currentDate };
		Log.d(TAG, "selection => " + selection);

		Cursor cursor =context.getContentResolver().query(
				MediaStore.Images.Media.EXTERNAL_CONTENT_URI, projection,
				selection, selectionArgs,
				MediaStore.Images.Media.DATE_TAKEN+" DESC");
		//With No Images
		if(cursor==null || cursor.getCount() == 0){
			if(!newer){
				prefs.edit().putBoolean(Constant.PREF_AUTO_IMPORT_FIRST_TIME_DONE, true).commit();
			}
			prefs.edit().putBoolean(Constant.PREF_AUTO_IMPORTING, false).commit();
			Intent intent = new Intent(Constant.ACTION_PHOTO_IMPORT);
			context.sendBroadcast(intent);
		}
		else{
			int fileNum = cursor.getCount();
			ContentResolver cr = context.getContentResolver();
			ContentValues[] cvs = null;
			ContentValues cv = null;
			cursor.moveToFirst();
			ArrayList<ContentValues> datas = new ArrayList<ContentValues>();
			TreeSet<String> filenamesSet = new TreeSet<String>();
			for (int counter = 0; counter < fileNum ; counter++) {
				cv = new ContentValues();
				mediaData = cursor.getString(cursor.getColumnIndex(MediaStore.Images.Media.DATA));
				dateTaken = cursor.getLong(cursor.getColumnIndex(MediaStore.Images.Media.DATE_TAKEN));
				dateModified = cursor.getLong(cursor.getColumnIndex(MediaStore.Images.Media.DATE_MODIFIED));
				dateAdded = cursor.getLong(cursor.getColumnIndex(MediaStore.Images.Media.DATE_ADDED));
				fileSize = cursor.getString(5);
				imageId = cursor.getString(6);
				folderName =  getFoldername(mediaData);

				if (dateTaken != -1) {
					refCursorDate = dateTaken / 1000;
				} else if (dateModified != -1) {
					refCursorDate = dateModified / 1000;
				} else if (dateAdded != -1) {
					refCursorDate = dateAdded / 1000;
				}
				cursorDate = StringUtil.getConverDate(refCursorDate);
				Log.d(TAG, "cursorDate ==>" + cursorDate);
				Log.d(TAG, "Filename ==>" + mediaData);

				if(!duplicateFilename(context,mediaData) && !filenamesSet.contains(mediaData)){
					cv.put(ImportFilesTable.COLUMN_FILENAME, mediaData);
					cv.put(ImportFilesTable.COLUMN_SIZE, StringUtil.getUUID());
					cv.put(ImportFilesTable.COLUMN_DATE, cursorDate);
					cv.put(ImportFilesTable.COLUMN_IMPORTED, Constant.IMPORT_FILE_INCLUDED);
					cv.put(ImportFilesTable.COLUMN_FOLDER, folderName);
					cv.put(ImportFilesTable.COLUMN_IMAGE_ID, imageId);
					filenamesSet.add(mediaData);
					datas.add(cv);
				}
				cursor.moveToNext();
			}
			if(datas.size()>0){
				//SAVE TO DB
				cvs = new ContentValues[datas.size()];
				datas.toArray(cvs);
				int result = cr.bulkInsert(ImportFilesTable.CONTENT_URI, cvs);
				Log.d(TAG,result+ " Files Imported!");
			}
			if(!newer){
				prefs.edit().putBoolean(Constant.PREF_AUTO_IMPORT_FIRST_TIME_DONE, true).commit();
				prefs.edit().putString(Constant.PREF_AUTO_IMPORT_OLDEST_FILE,mediaData).commit();
			}
			filenamesSet = null;
			datas= null;
		}
		if (cursor!=null)
			cursor.close();
		if(newer){
			RuntimePlayer.NewerImportProcessing = false;
		}
		else{
			RuntimePlayer.OlderImportProcessing = false;
		}
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
	public static void setImportAlarmCondition(Context context) {
		final Calendar c = Calendar.getInstance();
		int mHour = c.get(Calendar.HOUR_OF_DAY);
		int mMinute = c.get(Calendar.MINUTE);
		Editor editor = context.getSharedPreferences(Constant.PREFS_NAME, Context.MODE_PRIVATE).edit();
		editor.putInt(Constant.PREF_AUTO_IMPORT_HOUR, mHour);
		editor.putInt(Constant.PREF_AUTO_IMPORT_MINUTE, mMinute);
		editor.commit();
	}

	public static int deleteRecord(Context context,String filepath){
		//DELETE DATABSE RECORD WHERE FILE HAD DELETED
		ContentResolver cr = context.getContentResolver();
		return cr.delete(ImportFilesTable.CONTENT_URI, ImportFilesTable.COLUMN_FILENAME+"=?", new String[]{filepath});
	}
	/**
	 * add (photo/doc/audio/video) import folder to database
	 * @param context
	 * @param folderNames all the folders aboout type
	 * @param type "1" means Photos,"2" means Audio,"3" means Video,"4" means Document
	 */
	public static void addDefaultImportFolder(Context context,String[] folderNames,int type){
		ArrayList<ContentValues> imports = new ArrayList<ContentValues>();
		ContentResolver cr = context.getContentResolver();
		String currentTimestamp = StringUtil.getLocalDate();
		Set<String> existedFolders = new TreeSet<String>();
		if(folderNames!=null && folderNames.length>0){
			//query by folder name and type
			Cursor cursor = cr.query(ImportTable.CONTENT_URI,
					new String[]{ImportTable.COLUMN_FOLDER_NAME}
					,ImportTable.COLUMN_TYPE+"=?"
					, new String[]{String.valueOf(type)}, null);
			if(cursor!=null && cursor.getCount()>0){
				cursor.moveToFirst();
				for(int i = 0 ; i < cursor.getCount(); i++){
					existedFolders.add(cursor.getString(0));
					cursor.moveToNext();
				}
			}
			if(cursor!=null)
				cursor.close();

			ContentValues cv = null;
			for(int i=0;i<folderNames.length;i++){
				if(!existedFolders.contains(folderNames[i]) && !TextUtils.isEmpty(folderNames[i])){
					cv = new ContentValues();
					cv.put(ImportTable.COLUMN_FOLDER_NAME,folderNames[i]);
					cv.put(ImportTable.COLUMN_TYPE,type);
					cv.put(ImportTable.COLUMN_ENABLE, "1");
					cv.put(ImportTable.COLUMN_LAST_IMPORT_TIME, "");
					cv.put(ImportTable.COLUMN_ADDED_TIME, currentTimestamp);
					imports.add(cv);
				}
			}
			ContentValues[] cvs = new ContentValues[imports.size()];
			cvs = imports.toArray(cvs);
			cr.bulkInsert(ImportTable.CONTENT_URI, cvs);
		}
	}

	public static void addImageInfo(Context context){
		ContentResolver cr = context.getContentResolver();
		ArrayList<String> filenames = new ArrayList<String>();
		Cursor cursor = null;
		String Selection = ImportFilesTable.COLUMN_IMAGE_ID+"=? AND "+ImportFilesTable.COLUMN_IMPORTED+"=?";
		cursor = cr.query(ImportFilesTable.CONTENT_URI,
				new String[]{ImportFilesTable.COLUMN_FILENAME},
				Selection,new String[]{"-1",Constant.IMPORT_FILE_INCLUDED}, null);
		int count = 0;
		if(cursor!=null && cursor.getCount()>0){
			count = cursor.getCount();
			cursor.moveToFirst();
			for(int i=0; i < count ;i++){
				filenames.add(cursor.getString(0));
				cursor.moveToNext();
			}
		}
		cursor.close();

		String folderName = null;
		String imageId = null;
		ContentValues cv = null;
		long result = 0;
		String[] projection = {
				MediaStore.Images.Media.BUCKET_DISPLAY_NAME,
				MediaStore.Images.Media._ID};
		for(int i = 0 ; i < filenames.size();i++){
			cursor = cr.query(
					MediaStore.Images.Media.EXTERNAL_CONTENT_URI, projection,
					MediaStore.Images.Media.DATA+"=?", new String[]{filenames.get(i)},
					null);
			if(cursor!=null && cursor.getCount()>0){
				cursor.moveToFirst();
				folderName = cursor.getString(0);
				imageId = cursor.getString(1);
				cv = new ContentValues();
				cv.put(ImportFilesTable.COLUMN_FOLDER, cursor.getString(0));
				cv.put(ImportFilesTable.COLUMN_IMAGE_ID, cursor.getString(1));
				result = cr.update(ImportFilesTable.CONTENT_URI, cv, ImportFilesTable.COLUMN_FILENAME+"=?", new String[]{filenames.get(i)});
				Log.d(TAG, "UPDATE IMPORT FILE("+result+"):Filename("+filenames.get(i)+"),imageId:("+imageId+"),folderName:("+folderName+")");
			}
		}
		cursor.close();
	}
}