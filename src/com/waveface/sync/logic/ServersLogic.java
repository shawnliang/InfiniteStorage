package com.waveface.sync.logic;

import java.util.ArrayList;

import org.jwebsocket.kit.WebSocketException;

import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.database.Cursor;
import android.text.TextUtils;

import com.waveface.sync.Constant;
import com.waveface.sync.RuntimeConfig;
import com.waveface.sync.db.BackupedServersTable;
import com.waveface.sync.db.BonjourServersTable;
import com.waveface.sync.db.ImportFilesTable;
import com.waveface.sync.entity.ConnectEntity;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.util.DeviceUtil;
import com.waveface.sync.util.Log;
import com.waveface.sync.websocket.RuntimeWebClient;

public class ServersLogic {
	private static String TAG = ServersLogic.class.getSimpleName();
	
	public static int updateBackupedServer(Context context ,ServerEntity entity){
		int result = 0;
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		cursor = cr.query(BackupedServersTable.CONTENT_URI, 
				new String[]{BackupedServersTable.COLUMN_SERVER_ID}, 
				BackupedServersTable.COLUMN_SERVER_ID+"=?", 
				new String[]{entity.serverId}, 
				null);
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
		cv.put(BackupedServersTable.COLUMN_FOLDER, entity.Folder);
		cv.put(BackupedServersTable.COLUMN_FREE_SPACE, entity.freespace);
		cv.put(BackupedServersTable.COLUMN_PHOTO_COUNT, entity.photoCount);
		cv.put(BackupedServersTable.COLUMN_VIDEO_COUNT, entity.videoCount);
		cv.put(BackupedServersTable.COLUMN_AUDIO_COUNT, entity.audioCount);		
		
		//update
		if(cursor!=null && cursor.getCount()>0){
			result = cr.update(BackupedServersTable.CONTENT_URI, cv, BackupedServersTable.COLUMN_SERVER_ID+"=?", new String[]{entity.serverId});
			cv = new ContentValues();
			cv.put(BackupedServersTable.COLUMN_STATUS, Constant.SERVER_OFFLINE);
			cr.update(BackupedServersTable.CONTENT_URI, cv, BackupedServersTable.COLUMN_SERVER_ID+"!=?", new String[]{entity.serverId});
		}
		//insert
		else{
			result = cr.bulkInsert(BackupedServersTable.CONTENT_URI,new ContentValues[]{cv});
		}
		cursor.close();
		return result;
	}
	
	
	public static ArrayList<ServerEntity> getBackupedServers(Context context){
		ArrayList<ServerEntity> datas = new ArrayList<ServerEntity>();
		ServerEntity entity = null;
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		cursor = cr.query(BackupedServersTable.CONTENT_URI, 
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
				entity.status = cursor.getString(2);
            	entity.startDatetime =cursor.getString(3);
            	entity.endDatetime = cursor.getString(4);
            	entity.Folder = cursor.getString(5);
            	entity.freespace = cursor.getLong(6);
            	entity.photoCount = cursor.getInt(7);
            	entity.videoCount = cursor.getInt(8);
            	entity.audioCount =cursor.getInt(9);
            	datas.add(entity);
				cursor.moveToNext();
			}
		}
		cursor.close();
		return datas;
	}
	public static int updateBonjourServer(Context context ,ServerEntity entity){
		int result = 0;
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
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
		cursor.close();
		return result;
	}
	
	
	public static ArrayList<ServerEntity> getBonjourServers(Context context){
		ArrayList<ServerEntity> datas = new ArrayList<ServerEntity>();
		ServerEntity entity = null;
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
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
		cursor.close();
		return datas;
	}
	public static int purgeBonjourServer(Context context){
		ContentResolver cr = context.getContentResolver();
		return cr.delete(BonjourServersTable.CONTENT_URI, 
				null,null);
	}
	
	
	public static void resetStatus(Context context ){
		ContentValues cv = new ContentValues();
		cv.put(BackupedServersTable.COLUMN_STATUS, Constant.SERVER_OFFLINE);
		context.getContentResolver().update(BackupedServersTable.CONTENT_URI, cv, null, null);
	}
	
    public static void startWSServerConnect(Context context,String wsLocation,String serverId){
	    //SETUP WS URL ANDLink to WS
	    if(RuntimeConfig.OnWebSocketOpened == false){
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
	    		RuntimeWebClient.send(RuntimeConfig.GSON.toJson(connect));
	    	} catch (WebSocketException e) {
	    		e.printStackTrace();
	    	}
	    }
    }
	public static long[] getTransferCountAndSize(Context context,String serverId){
		long count = 0;
		long totalSize = 0 ;
		String lastTimestamp = null;
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = cr.query(BackupedServersTable.CONTENT_URI, 
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
		cursor.close();
		return new long[]{count,totalSize};
	}

    public static void startBackuping(Context context,ServerEntity entity){
		RuntimeConfig.OnWebSocketOpened = true;	            	
    	entity.status = Constant.SERVER_LINKING;
    	int result = ServersLogic.updateBackupedServer(context, entity);
    	Log.d(TAG, "Update Server:"+result);
		Intent intent = new Intent(Constant.ACTION_BACKUP_FILE);
		context.sendBroadcast(intent);
    }
	public static int updateServerLastBackupTimestamp(Context context,String lastBackupTime,String serverId){
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(BackupedServersTable.COLUMN_END_DATETIME, lastBackupTime);

		String startBackupTimestamp = null;
		Cursor cursor = cr.query(BackupedServersTable.CONTENT_URI, 
				new String[]{BackupedServersTable.COLUMN_START_DATETIME}, 
				BackupedServersTable.COLUMN_SERVER_ID+" =? ", 
				new String[]{serverId},null);	
		if(cursor!=null && cursor.getCount()>0){
			cursor.moveToFirst();
			startBackupTimestamp = cursor.getString(0);
		}		
		cursor.close();
		if(TextUtils.isEmpty(startBackupTimestamp)){
			cv.put(BackupedServersTable.COLUMN_START_DATETIME, lastBackupTime);
		}		
		return cr.update(BackupedServersTable.CONTENT_URI, cv,BackupedServersTable.COLUMN_SERVER_ID+"=?" , new String[]{serverId});
	}
}
