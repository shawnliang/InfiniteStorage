package com.waveface.sync.logic;

import java.util.ArrayList;
import java.util.TreeSet;

import org.jwebsocket.kit.WebSocketException;

import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Context;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.database.Cursor;
import android.text.TextUtils;

import com.waveface.sync.Constant;
import com.waveface.sync.RuntimeState;
import com.waveface.sync.db.BackupedServersTable;
import com.waveface.sync.db.BonjourServersTable;
import com.waveface.sync.db.ImportFilesTable;
import com.waveface.sync.entity.ConnectEntity;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.entity.UpdateCountEntity;
import com.waveface.sync.util.DeviceUtil;
import com.waveface.sync.util.StringUtil;
import com.waveface.sync.websocket.RuntimeWebClient;

public class ServersLogic {
	private static String TAG = ServersLogic.class.getSimpleName();
	
	public static int updateBackupedServerStatus(Context context ,String serverId,String status){
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(BackupedServersTable.COLUMN_STATUS, status);
		//update
		return cr.update(BackupedServersTable.CONTENT_URI, 
				 cv, 
			 	 BackupedServersTable.COLUMN_SERVER_ID+"=?", new String[]{serverId});
	}
	   
	public static void updateBackupedServerByServerNotify(Context context ,ServerEntity entity,boolean accept){
		//Update Server Info
		ServerEntity pairedServer = getBonjourServerByServerId(context, RuntimeState.mWebSocketServerId);
		if(TextUtils.isEmpty(entity.serverName)){
			entity.serverName = pairedServer.serverName;
		}
		entity.serverOS = pairedServer.serverOS;
		entity.wsLocation = pairedServer.wsLocation;
		entity.status = Constant.SERVER_LINKING;
		if(accept){
		    SharedPreferences mPrefs = context.getSharedPreferences(Constant.PREFS_NAME, Context.MODE_PRIVATE);
			Editor mEditor = mPrefs.edit();
		    mEditor.putString(Constant.PREF_STATION_WEB_SOCKET_URL, entity.wsLocation);
	    	mEditor.commit();
		}
	    updateBackupedServer(context, entity);
	}
	
	public static int updateBackupedServer(Context context ,ServerEntity entity){
		int result = 0;
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(BackupedServersTable.COLUMN_SERVER_ID, entity.serverId);		
		cv.put(BackupedServersTable.COLUMN_SERVER_NAME, entity.serverName);
		cv.put(BackupedServersTable.COLUMN_STATUS, entity.status);
		if(TextUtils.isEmpty(entity.startDatetime)){
			entity.startDatetime = "";
		}
		cv.put(BackupedServersTable.COLUMN_START_DATETIME, entity.startDatetime);
		if(TextUtils.isEmpty(entity.endDatetime)){
			entity.endDatetime = "";
		}
		cv.put(BackupedServersTable.COLUMN_END_DATETIME, entity.endDatetime);
		cv.put(BackupedServersTable.COLUMN_FOLDER, entity.folder);
		cv.put(BackupedServersTable.COLUMN_FREE_SPACE, entity.freespace);
		cv.put(BackupedServersTable.COLUMN_PHOTO_COUNT, entity.photoCount);
		cv.put(BackupedServersTable.COLUMN_VIDEO_COUNT, entity.videoCount);
		cv.put(BackupedServersTable.COLUMN_AUDIO_COUNT, entity.audioCount);		
		cv.put(BackupedServersTable.COLUMN_LAST_DISPLAY_BACKUP_DATETIME, StringUtil.getLocalDate());
		cv.put(BackupedServersTable.COLUMN_LAST_FILE_MEDIA_ID, "");	
		
		try {
			cursor = cr.query(BackupedServersTable.CONTENT_URI, 
					new String[]{
						BackupedServersTable.COLUMN_LAST_FILE_MEDIA_ID}, 
					BackupedServersTable.COLUMN_SERVER_ID+"=?", 
					new String[]{entity.serverId}, 
					null);
			//update
			if(cursor!=null && cursor.getCount()>0){
				cursor.moveToFirst();
				cv.put(BackupedServersTable.COLUMN_LAST_FILE_MEDIA_ID, cursor.getString(0));			
				result = cr.update(BackupedServersTable.CONTENT_URI, 
						 cv, 
					 	 BackupedServersTable.COLUMN_SERVER_ID+"=?", new String[]{entity.serverId});
				cv = new ContentValues();
				cv.put(BackupedServersTable.COLUMN_STATUS, Constant.SERVER_OFFLINE);
				result = cr.update(BackupedServersTable.CONTENT_URI, 
						cv, 
						BackupedServersTable.COLUMN_SERVER_ID+"!=? AND "+BackupedServersTable.COLUMN_STATUS+"!=?", 
						new String[]{entity.serverId,Constant.SERVER_DENIED});
			}
			//insert
			else{
				result = cr.bulkInsert(BackupedServersTable.CONTENT_URI,new ContentValues[]{cv});
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		finally{
			if(cursor!=null){
				cursor.close();
				cursor = null;
			}
		}
		return result;
	}
	
	public static boolean hasBackupedServers(Context context){
		boolean hasServer = false;
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = null ;
		try {
			cursor = cr.query(BackupedServersTable.CONTENT_URI, 
					null, 
					BackupedServersTable.COLUMN_STATUS+"!=?", 
					new String[]{Constant.SERVER_DENIED}, 
					BackupedServersTable.COLUMN_SERVER_ID+" LIMIT 1");
			if(cursor!=null && cursor.getCount()>0){
				hasServer = true; 
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		finally{
			if(cursor!=null){
				cursor.close();
				cursor = null;
			}
		}
		return hasServer;
	}	
	public static ArrayList<ServerEntity> getBackupedServers(Context context){
		ArrayList<ServerEntity> datas = new ArrayList<ServerEntity>();
		ServerEntity entity = null;
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		try {
			cursor = cr.query(BackupedServersTable.CONTENT_URI, 
					null, 
					BackupedServersTable.COLUMN_STATUS+"!=?", 
					new String[]{Constant.SERVER_DENIED}, 
					null);
			
			if(cursor!=null && cursor.getCount()>0){
				int count = cursor.getCount();
				cursor.moveToFirst();
				for(int i = 0 ; i < count ;i++){
					entity = new ServerEntity();
					entity.serverId = cursor.getString(0);
					entity.serverName = cursor.getString(1);
					entity.status = cursor.getString(2);
			    	entity.startDatetime =cursor.getString(3);
			    	entity.endDatetime = cursor.getString(4);
			    	entity.folder = cursor.getString(5);
			    	entity.freespace = cursor.getLong(6);
			    	entity.photoCount = cursor.getInt(7);
			    	entity.videoCount = cursor.getInt(8);
			    	entity.audioCount =cursor.getInt(9);
			    	entity.lastLocalBackupTime = cursor.getString(10);

			    	datas.add(entity);
					cursor.moveToNext();
				}
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		finally{
			if(cursor!=null){
				cursor.close();
				cursor = null;
			}
		}
		return datas;
	}
	public static String getLastBackupTime(Context context,String serverId){
		String time = "" ;
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		try {
			cursor = cr.query(BackupedServersTable.CONTENT_URI, 
					new String[]{BackupedServersTable.COLUMN_LAST_DISPLAY_BACKUP_DATETIME}, 
					BackupedServersTable.COLUMN_SERVER_ID+"=?", 
					new String[]{serverId}, 
					null);
			
			if(cursor!=null && cursor.getCount()>0){
				int count = cursor.getCount();
				cursor.moveToFirst();
				time = cursor.getString(0);
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		finally{
			if(cursor!=null){
				cursor.close();
				cursor = null;
			}
		}
		return time;
	}

	public static int updateBonjourServer(Context context ,ServerEntity entity){
		int result = 0;
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		try {
			cursor = cr.query(BonjourServersTable.CONTENT_URI, 
					new String[]{BonjourServersTable.COLUMN_SERVER_ID}, 
					BonjourServersTable.COLUMN_SERVER_ID+"=?", 
					new String[]{entity.serverId}, 
					null);
			ContentValues cv = new ContentValues();
			cv.put(BonjourServersTable.COLUMN_SERVER_ID, entity.serverId);
			cv.put(BonjourServersTable.COLUMN_SERVER_NAME, entity.serverName);
			cv.put(BonjourServersTable.COLUMN_SERVER_OS, entity.serverOS);
			cv.put(BonjourServersTable.COLUMN_WS_LOCATION, entity.wsLocation);
			
			//update
			if(cursor!=null && cursor.getCount()>0){
				result = cr.update(BonjourServersTable.CONTENT_URI, cv, BonjourServersTable.COLUMN_SERVER_ID+"=?", new String[]{entity.serverId});
			}
			//insert
			else{
				result = cr.bulkInsert(BonjourServersTable.CONTENT_URI,new ContentValues[]{cv});
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		finally{
			if(cursor!=null){
				cursor.close();
				cursor = null;
			}
		}
		return result;
	}
	public static boolean hasBonjourServers(Context context){
		boolean hasServer = false;
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = null ;
		try {
			cursor = cr.query(BonjourServersTable.CONTENT_URI, 
					new String[]{BonjourServersTable.COLUMN_SERVER_ID}, 
					null, 
					null, 
					BonjourServersTable.COLUMN_SERVER_ID+" LIMIT 1");
			
			if(cursor!=null && cursor.getCount()>0){
				hasServer = true;
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		finally{
			if(cursor!=null){
				cursor.close();
				cursor = null;
			}
		}
		return hasServer;
	}

	
	public static ArrayList<ServerEntity> getBonjourServers(Context context){
		ArrayList<ServerEntity> datas = new ArrayList<ServerEntity>();
		ServerEntity entity = null;
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		try {
			cursor = cr.query(BonjourServersTable.CONTENT_URI, 
					null, 
					null, 
					null, 
					null);
			
			if(cursor!=null && cursor.getCount()>0){
				int count = cursor.getCount();
				cursor.moveToFirst();
				for(int i = 0 ; i < count ;i++){
					entity = new ServerEntity();
					entity.serverId = cursor.getString(0);
					entity.serverName = cursor.getString(1);
					entity.serverOS = cursor.getString(2);
			    	entity.wsLocation =cursor.getString(3);
			     	datas.add(entity);
					cursor.moveToNext();
				}
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		finally{
			if(cursor!=null){
				cursor.close();
				cursor = null;
			}
		}
		return datas;
	}
	public static ArrayList<ServerEntity> getBonjourServersExportPaired(Context context){
		ArrayList<ServerEntity> datas = new ArrayList<ServerEntity>();
		ServerEntity entity = null;
		TreeSet<String> pairedServerId = new TreeSet<String>();
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = null;
		try {
			cursor = cr.query(BackupedServersTable.CONTENT_URI, 
					new String[]{BackupedServersTable.COLUMN_SERVER_ID}, 
					null, 
					null, 
					null);
			
			if(cursor!=null && cursor.getCount()>0){
				int count = cursor.getCount();
				cursor.moveToFirst();
				for(int i = 0 ; i < count ;i++){
					pairedServerId.add(cursor.getString(0));
					cursor.moveToNext();
				}
			}
			cursor = cr.query(BonjourServersTable.CONTENT_URI, 
					null, 
					null, 
					null, 
					null);
			
			if(cursor!=null && cursor.getCount()>0){
				int count = cursor.getCount();
				cursor.moveToFirst();
				for(int i = 0 ; i < count ;i++){
					if(!pairedServerId.contains(cursor.getString(0))){
						entity = new ServerEntity();
						entity.serverId = cursor.getString(0);
						entity.serverName = cursor.getString(1);
						entity.serverOS = cursor.getString(2);
			        	entity.wsLocation =cursor.getString(3);
			         	datas.add(entity);
					}
					cursor.moveToNext();
				}
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		finally{
			if(cursor!=null){
				cursor.close();
				cursor = null;
			}
		}
		return datas;
	}

	public static ServerEntity getBonjourServerByServerId(Context context,String serverId){
		ServerEntity entity = null;
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		try {
			cursor = cr.query(BonjourServersTable.CONTENT_URI, 
					null, 
					BonjourServersTable.COLUMN_SERVER_ID+"=?", 
					new String[]{serverId}, 
					null);
			
			if(cursor!=null && cursor.getCount()>0){
				int count = cursor.getCount();
				cursor.moveToFirst();
				for(int i = 0 ; i < count ;i++){
					entity = new ServerEntity();
					entity.serverId = cursor.getString(0);
					entity.serverName = cursor.getString(1);
					entity.serverOS = cursor.getString(2);
			    	entity.wsLocation =cursor.getString(3);
					cursor.moveToNext();
				}
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		finally{
			if(cursor!=null){
				cursor.close();
				cursor = null;
			}
		}
		return entity;
	}
	
	public static int updateAllBackedServerOffline(Context context){
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(BackupedServersTable.COLUMN_STATUS, Constant.SERVER_OFFLINE);
		return cr.update(BackupedServersTable.CONTENT_URI, 
				cv,BackupedServersTable.COLUMN_STATUS+"!=?",new String[]{Constant.SERVER_DENIED});
	}
	
	public static int purgeAllBonjourServer(Context context){
		ContentResolver cr = context.getContentResolver();
		return cr.delete(BonjourServersTable.CONTENT_URI, 
				null,null);
	}

	public static int purgeBonjourServerByServerId(Context context,String serverId){
		ContentResolver cr = context.getContentResolver();
		return cr.delete(BonjourServersTable.CONTENT_URI, 
				BonjourServersTable.COLUMN_SERVER_ID+"=?",new String[]{serverId});
	}	
	
    public static void startWSServerConnect(Context context,String wsLocation,String serverId){
	    //SETUP WS URL ANDLink to WS
	    if(RuntimeState.OnWebSocketOpened == false){
	    	RuntimeWebClient.init(context);
	    	RuntimeWebClient.setURL(wsLocation);
	    	try {
	    		RuntimeWebClient.open();
	    		//send connect cmd
	    		ConnectEntity connect = new ConnectEntity();
	    		connect.action = Constant.WS_ACTION_CONNECT;
	    		connect.deviceId = DeviceUtil.id(context);
	    		connect.deviceName = DeviceUtil.getDeviceNameForDisplay(context);
	    		long[] countAndSize = getTransferCountAndSize(context, serverId);
	    		connect.transferCount = countAndSize[0];
	    		connect.transferSize = countAndSize[1];
	    		RuntimeWebClient.send(RuntimeState.GSON.toJson(connect));
	    	} catch (WebSocketException e) {
	    		e.printStackTrace();
	    	}
	    }
    }
    public static void updateCount(Context context,String serverId){
    	try {
    		//send connect cmd
    		UpdateCountEntity entity = new UpdateCountEntity();
    		entity.action = Constant.WS_ACTION_UPDATE_COUNT;
    		entity.transferCount = getTransferCount(context, serverId);
    		RuntimeWebClient.send(RuntimeState.GSON.toJson(entity));
    	} catch (WebSocketException e) {
    		e.printStackTrace();
    	}
    }    
	public static long getTransferCount(Context context,String serverId){
		long count = 0;
		String lastTimestamp = null;
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = null;
		try {
			cursor = cr.query(BackupedServersTable.CONTENT_URI, 
					new String[]{BackupedServersTable.COLUMN_END_DATETIME}, 
					BackupedServersTable.COLUMN_SERVER_ID+" =? ", 
					new String[]{serverId},null);	
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
				where = ImportFilesTable.COLUMN_DATE+">=? AND "+
						ImportFilesTable.COLUMN_STATUS+"!=? AND "+ImportFilesTable.COLUMN_STATUS+"!=? ";
				whereArgs = new String[]{lastTimestamp,Constant.IMPORT_FILE_DELETED,Constant.IMPORT_FILE_EXCLUDE};
			}
			
			cursor = cr.query(ImportFilesTable.CONTENT_URI, 
					new String[]{
					ImportFilesTable.COLUMN_SIZE}, 
					where, 
					whereArgs,null);	
			if(cursor!=null && cursor.getCount()>0){			
				count = cursor.getCount();
			}
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		finally{
			if(cursor!=null){
				cursor.close();
				cursor = null;
			}
		}
		return count;
	}

    public static long[] getTransferCountAndSize(Context context,String serverId){
		long count = 0;
		long totalSize = 0 ;
		String lastTimestamp = null;
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = null ;
		try {
			cursor = cr.query(BackupedServersTable.CONTENT_URI, 
					new String[]{BackupedServersTable.COLUMN_END_DATETIME}, 
					BackupedServersTable.COLUMN_SERVER_ID+" =? ", 
					new String[]{serverId},null);	
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
				where = ImportFilesTable.COLUMN_DATE+">=? AND "+
						ImportFilesTable.COLUMN_STATUS+"!=? AND "+ImportFilesTable.COLUMN_STATUS+"!=? ";
				whereArgs = new String[]{lastTimestamp,Constant.IMPORT_FILE_DELETED,Constant.IMPORT_FILE_EXCLUDE};
			}
			
			cursor = cr.query(ImportFilesTable.CONTENT_URI, 
					new String[]{
					ImportFilesTable.COLUMN_SIZE}, 
					where, 
					whereArgs,null);	
			if(cursor!=null && cursor.getCount()>0){			
				count = cursor.getCount();
				cursor.moveToFirst();
				for(int i =0; i < count ;i++){
					totalSize += cursor.getLong(0);
					cursor.moveToNext();
				}
			}
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		finally{
			if(cursor!=null){
				cursor.close();
				cursor = null;
			}
		}
		return new long[]{count,totalSize};
	}
    
	public static int deniedPairedServer(Context context,String serverId){
		int result = 0;
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(BackupedServersTable.COLUMN_STATUS, Constant.SERVER_DENIED);
		Cursor cursor = null ;
		try {
			cursor = cr.query(BackupedServersTable.CONTENT_URI, 
					new String[]{BackupedServersTable.COLUMN_SERVER_ID}, 
					BackupedServersTable.COLUMN_SERVER_ID+" =? ", 
					new String[]{serverId},null);	
			if(cursor!=null && cursor.getCount()>0){
				result = cr.update(BackupedServersTable.CONTENT_URI, cv, BackupedServersTable.COLUMN_SERVER_ID+"=?", new String[]{serverId});
			}
		} catch (Exception e) {
			e.printStackTrace();
		}	
		finally{
			if(cursor!=null){
				cursor.close();
				cursor = null;
			}
		}
		return result;
	}    
	public static int updateServerLastBackupTime(Context context,String serverId,String fileDateTime){
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(BackupedServersTable.COLUMN_LAST_DISPLAY_BACKUP_DATETIME, StringUtil.getLocalDate());
		cv.put(BackupedServersTable.COLUMN_LAST_FILE_MEDIA_ID, fileDateTime);		
		return cr.update(BackupedServersTable.CONTENT_URI, cv,BackupedServersTable.COLUMN_SERVER_ID+"=?" , new String[]{serverId});
	}
	public static String getStatusByServerId(Context context,String serverId){
		if(serverId.equals(RuntimeState.mWebSocketServerId)){
			return Constant.SERVER_LINKING;
		}
		else{
			return Constant.SERVER_OFFLINE;
		}
	}
	public static ServerEntity getServerById(Context context,String serverId){
		ServerEntity entity = null;
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = null ;
		try {
			cursor = cr.query(BackupedServersTable.CONTENT_URI, 
					null, 
					BackupedServersTable.COLUMN_STATUS+"!=?", 
					new String[]{Constant.SERVER_DENIED}, 
					null);
			if(cursor!=null && cursor.getCount()>0){		
				cursor.moveToFirst();
					entity = new ServerEntity();
					entity.serverId = cursor.getString(0);
					entity.serverName = cursor.getString(1);
					entity.status = cursor.getString(2);
			    	entity.startDatetime =cursor.getString(3);
			    	entity.endDatetime = cursor.getString(4);
			    	entity.folder = cursor.getString(5);
			    	entity.freespace = cursor.getLong(6);
			    	entity.photoCount = cursor.getInt(7);
			    	entity.videoCount = cursor.getInt(8);
			    	entity.audioCount =cursor.getInt(9);
			    	entity.lastLocalBackupTime = cursor.getString(10);
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		finally{
			if(cursor!=null){
				cursor.close();
				cursor = null;
			}
		}
		return entity;
	}
	public static int getServerBackupedCountById(Context context,String serverId){
		int count = 0;
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = null;
		try {
			cursor = cr.query(BackupedServersTable.CONTENT_URI, 
					new String[]{
						BackupedServersTable.COLUMN_PHOTO_COUNT,
						BackupedServersTable.COLUMN_VIDEO_COUNT,
						BackupedServersTable.COLUMN_AUDIO_COUNT
					}, 
					BackupedServersTable.COLUMN_STATUS+"!=?", 
					new String[]{Constant.SERVER_DENIED}, 
					null);
			if(cursor!=null && cursor.getCount()>0){
				cursor.moveToFirst();
				count += cursor.getInt(0);
				count += cursor.getInt(1);
				count += cursor.getInt(2);
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		finally{
			if(cursor!=null){
				cursor.close();
				cursor = null;
			}
		}
		return count;
	}
	public static void getLastBackupState(Context context){
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		String lastBackupMediaId = null;
		try {			
			cursor = cr.query(BackupedServersTable.CONTENT_URI, 
					new String[]{
					BackupedServersTable.COLUMN_LAST_FILE_MEDIA_ID
					}, 
					BackupedServersTable.COLUMN_STATUS+"!=?", 
					new String[]{Constant.SERVER_DENIED}, 
					null);
			
			if(cursor!=null && cursor.getCount()>0){
				cursor.moveToFirst();
				lastBackupMediaId = cursor.getString(0);
			}
			if(!TextUtils.isEmpty(lastBackupMediaId)){
				cursor = cr.query(ImportFilesTable.CONTENT_URI, 
						new String[]{
						ImportFilesTable.COLUMN_FILENAME,
						ImportFilesTable.COLUMN_FILETYPE,
						ImportFilesTable.COLUMN_IMAGE_ID
						}, 
						ImportFilesTable.COLUMN_IMAGE_ID+"=?", 
						new String[]{lastBackupMediaId}, 
						null);
				
				if(cursor!=null && cursor.getCount()>0){
					cursor.moveToFirst();
					RuntimeState.mFilename = cursor.getString(0);
					RuntimeState.mFileType = cursor.getInt(1);
					RuntimeState.mMediaID = cursor.getLong(2);				
				}
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		finally{
			if(cursor!=null){
				cursor.close();
				cursor = null;
			}
		}	
	}
}