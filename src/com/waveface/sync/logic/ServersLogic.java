package com.waveface.sync.logic;

import java.util.ArrayList;

import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;

import com.waveface.sync.Constant;
import com.waveface.sync.db.ServersTable;
import com.waveface.sync.entity.ServerEntity;

public class ServersLogic {

	
	public static int updateServer(Context context ,ServerEntity entity){
		int result = 0;
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		cursor = cr.query(ServersTable.CONTENT_URI, 
				new String[]{ServersTable.COLUMN_SERVER_ID}, 
				ServersTable.COLUMN_SERVER_ID+"=?", 
				new String[]{entity.serverId}, 
				null);
		ContentValues cv = new ContentValues();
		cv.put(ServersTable.COLUMN_SERVER_ID, entity.serverId);
		cv.put(ServersTable.COLUMN_SERVER_NAME, entity.serverName);
		cv.put(ServersTable.COLUMN_STATUS, entity.status);
		cv.put(ServersTable.COLUMN_START_DATETIME, entity.startDatetime);
		cv.put(ServersTable.COLUMN_END_DATETIME, entity.endDatetime);
		cv.put(ServersTable.COLUMN_FOLDER, entity.Folder);
		cv.put(ServersTable.COLUMN_FREE_SPACE, entity.freespace);
		cv.put(ServersTable.COLUMN_PHOTO_COUNT, entity.photoCount);
		cv.put(ServersTable.COLUMN_VIDEO_COUNT, entity.videoCount);
		cv.put(ServersTable.COLUMN_AUDIO_COUNT, entity.audioCount);		
		
		//update
		if(cursor!=null && cursor.getCount()>0){
			result = cr.update(ServersTable.CONTENT_URI, cv, ServersTable.COLUMN_SERVER_ID+"=?", new String[]{entity.serverId});
			cv = new ContentValues();
			cv.put(ServersTable.COLUMN_STATUS, Constant.SERVER_OFFLINE);
			cr.update(ServersTable.CONTENT_URI, cv, ServersTable.COLUMN_SERVER_ID+"!=?", new String[]{entity.serverId});
		}
		//insert
		else{
			result = cr.bulkInsert(ServersTable.CONTENT_URI,new ContentValues[]{cv});
		}
		cursor.close();
		return result;
	}
	
	
	public static ArrayList<ServerEntity> getServers(Context context){
		ArrayList<ServerEntity> datas = new ArrayList<ServerEntity>();
		ServerEntity entity = null;
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		cursor = cr.query(ServersTable.CONTENT_URI, 
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
            	entity.freespace = cursor.getString(6);
            	entity.photoCount = cursor.getString(7);
            	entity.videoCount = cursor.getString(8);
            	entity.audioCount =cursor.getString(9);
            	datas.add(entity);
				cursor.moveToNext();
			}
		}
		cursor.close();
		return datas;
	}
	
	public static void resetStatus(Context context ){
		ContentValues cv = new ContentValues();
		cv.put(ServersTable.COLUMN_STATUS, Constant.SERVER_OFFLINE);
		context.getContentResolver().update(ServersTable.CONTENT_URI, cv, null, null);
	}

}
