package com.waveface.sync.logic;

import java.util.ArrayList;
import java.util.TreeSet;

import org.jwebsocket.kit.WebSocketException;

import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.database.Cursor;
import android.text.TextUtils;
import android.util.Log;

import com.waveface.sync.Constant;
import com.waveface.sync.RuntimeState;
import com.waveface.sync.db.BackupedServersTable;
import com.waveface.sync.db.BonjourServersTable;
import com.waveface.sync.entity.ConnectForGTVEntity;
import com.waveface.sync.entity.ServerEntity;
import com.waveface.sync.util.DeviceUtil;
import com.waveface.sync.websocket.RuntimeWebClient;

public class ServersLogic {
	private static String TAG = ServersLogic.class.getSimpleName();

	public static int updateBackupedServerStatus(Context context,
			String serverId, String status) {
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(BackupedServersTable.COLUMN_STATUS, status);
		// update
		return cr.update(BackupedServersTable.CONTENT_URI, cv,
				BackupedServersTable.COLUMN_SERVER_ID + "=?",
				new String[] { serverId });
	}

	public static void updateBackupedServerByServerNotify(Context context,
			ServerEntity entity, boolean accept) {
		// Update Server Info
		ServerEntity pairedServer = getBonjourServerByServerId(context,
				RuntimeState.mWebSocketServerId);
		if (TextUtils.isEmpty(entity.serverName)) {
			entity.serverName = pairedServer.serverName;
		}
		entity.serverOS = pairedServer.serverOS;
		entity.wsLocation = pairedServer.wsLocation;
		entity.status = Constant.SERVER_LINKING;
		if (accept) {
			SharedPreferences mPrefs = context.getSharedPreferences(
					Constant.PREFS_NAME, Context.MODE_PRIVATE);
			Editor mEditor = mPrefs.edit();
			mEditor.putString(Constant.PREF_STATION_WEB_SOCKET_URL,
					entity.wsLocation);
			mEditor.commit();
		}
		updateBackupedServer(context, entity);
	}

	public static int updateBackupedServer(Context context, ServerEntity entity) {
		int result = 0;
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(BackupedServersTable.COLUMN_SERVER_ID, entity.serverId);
		cv.put(BackupedServersTable.COLUMN_SERVER_NAME, entity.serverName);
		if(TextUtils.isEmpty(entity.status)){
			entity.status="";
		}		
		cv.put(BackupedServersTable.COLUMN_STATUS, entity.status);
		cv.put(BackupedServersTable.COLUMN_IP, entity.ip);
		if(TextUtils.isEmpty(entity.notifyPort)){
			entity.notifyPort="";
		}
		cv.put(BackupedServersTable.COLUMN_NOTIFY_PORT, entity.notifyPort);		
		if(TextUtils.isEmpty(entity.restPort)){
			entity.restPort="";
		}
		cv.put(BackupedServersTable.COLUMN_REST_PORT, entity.restPort);
		if(TextUtils.isEmpty(entity.wsPort)){
			entity.wsPort="";
		}
		cv.put(BackupedServersTable.COLUMN_WS_PORT, entity.wsPort);
		
				result = cr.bulkInsert(BackupedServersTable.CONTENT_URI,
						new ContentValues[] { cv });
		
		return result;
	}

	public static boolean hasBackupedServers(Context context) {
		boolean hasServer = false;
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = null;
		try {
			cursor = cr.query(BackupedServersTable.CONTENT_URI, null,
					BackupedServersTable.COLUMN_STATUS + " NOT IN(?,?)",
					new String[] { Constant.SERVER_DENIED_BY_SERVER,
							   Constant.SERVER_DENIED_BY_CLIENT}, 
					BackupedServersTable.COLUMN_SERVER_ID + " LIMIT 1");
			if (cursor != null && cursor.getCount() > 0) {
				hasServer = true;
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
		return hasServer;
	}
	public static boolean canPairedServers(Context context,String serverId) {
		boolean canPair = false;
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = null;
		try {
			cursor = cr.query(BackupedServersTable.CONTENT_URI, null,
					BackupedServersTable.COLUMN_SERVER_ID+"=? AND "+ 
			        BackupedServersTable.COLUMN_STATUS + " NOT IN(?,?)",
					new String[] { serverId,Constant.SERVER_DENIED_BY_SERVER,
							   Constant.SERVER_DENIED_BY_CLIENT}, 
					BackupedServersTable.COLUMN_SERVER_ID + " LIMIT 1");
			if (cursor != null && cursor.getCount() > 0) {
				canPair = true;
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
		return canPair;
	}

	
	public static ArrayList<ServerEntity> getBackupedServers(Context context) {
		ArrayList<ServerEntity> datas = new ArrayList<ServerEntity>();
		ServerEntity entity = null;
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		try {
			cursor = cr.query(BackupedServersTable.CONTENT_URI, null,
					BackupedServersTable.COLUMN_STATUS + " NOT IN(?,?)",
					new String[] { Constant.SERVER_DENIED_BY_SERVER,
								   Constant.SERVER_DENIED_BY_CLIENT}, null);

			if (cursor != null && cursor.getCount() > 0) {
				int count = cursor.getCount();
				cursor.moveToFirst();
				for (int i = 0; i < count; i++) {
					entity = new ServerEntity();
					entity.serverId = cursor.getString(cursor.getColumnIndex(BackupedServersTable.COLUMN_SERVER_ID));
					entity.serverName = cursor.getString(cursor.getColumnIndex(BackupedServersTable.COLUMN_SERVER_NAME));
					entity.status = cursor.getString(cursor.getColumnIndex(BackupedServersTable.COLUMN_STATUS));
					entity.ip = cursor.getString(cursor.getColumnIndex(BackupedServersTable.COLUMN_IP));
					entity.notifyPort = cursor.getString(cursor.getColumnIndex(BackupedServersTable.COLUMN_NOTIFY_PORT));
					entity.restPort =cursor.getString(cursor.getColumnIndex(BackupedServersTable.COLUMN_REST_PORT));
					entity.wsPort = cursor.getString(cursor.getColumnIndex(BackupedServersTable.COLUMN_WS_PORT));
					datas.add(entity);
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
		return datas;
	}



	public static int updateBonjourServer(Context context, ServerEntity entity) {
		int result = 0;
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		try {
			cursor = cr.query(BonjourServersTable.CONTENT_URI,
					new String[] { BonjourServersTable.COLUMN_SERVER_ID },
					BonjourServersTable.COLUMN_SERVER_ID + "=?",
					new String[] { entity.serverId }, null);
			ContentValues cv = new ContentValues();
			cv.put(BonjourServersTable.COLUMN_SERVER_ID, entity.serverId);
			cv.put(BonjourServersTable.COLUMN_SERVER_NAME, entity.serverName);
			cv.put(BonjourServersTable.COLUMN_IP, entity.ip);			
			cv.put(BonjourServersTable.COLUMN_WS_PORT, entity.wsPort);
			cv.put(BonjourServersTable.COLUMN_NOTIFY_PORT, entity.notifyPort);
			cv.put(BonjourServersTable.COLUMN_REST_PORT, entity.restPort);

			// update
			if (cursor != null && cursor.getCount() > 0) {
				result = cr.update(BonjourServersTable.CONTENT_URI, cv,
						BonjourServersTable.COLUMN_SERVER_ID + "=?",
						new String[] { entity.serverId });
				cursor.close();
			}
			// insert
			else {
				result = cr.bulkInsert(BonjourServersTable.CONTENT_URI,
						new ContentValues[] { cv });
			}
			cursor = null ;
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			if (cursor != null) {
				cursor.close();
				cursor = null;
			}
		}
		return result;
	}

	public static boolean hasBonjourServers(Context context) {
		boolean hasServer = false;
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = null;
		try {
			cursor = cr.query(BonjourServersTable.CONTENT_URI,
					new String[] { BonjourServersTable.COLUMN_SERVER_ID },
					null, null, BonjourServersTable.COLUMN_SERVER_ID
							+ " LIMIT 1");

			if (cursor != null && cursor.getCount() > 0) {
				hasServer = true;
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
		return hasServer;
	}

	public static ArrayList<ServerEntity> getBonjourServers(Context context) {
		ArrayList<ServerEntity> datas = new ArrayList<ServerEntity>();
		ServerEntity entity = null;
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		try {
			cursor = cr.query(BonjourServersTable.CONTENT_URI, null, null,
					null, null);

			if (cursor != null && cursor.getCount() > 0) {
				int count = cursor.getCount();
				cursor.moveToFirst();
				for (int i = 0; i < count; i++) {
					entity = new ServerEntity();
					entity.serverId = cursor.getString(0);
					entity.serverName = cursor.getString(1);
					entity.ip = cursor.getString(2);
					entity.wsPort = cursor.getString(3);
					entity.notifyPort = cursor.getString(4);
					entity.restPort = cursor.getString(5);					
					datas.add(entity);
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
		return datas;
	}

	public static ArrayList<ServerEntity> getBonjourServersExportPaired(
			Context context) {
		ArrayList<ServerEntity> datas = new ArrayList<ServerEntity>();
		ServerEntity entity = null;
		TreeSet<String> pairedServerId = new TreeSet<String>();
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = null;
		try {
			cursor = cr.query(BackupedServersTable.CONTENT_URI,
					new String[] { BackupedServersTable.COLUMN_SERVER_ID },
					BackupedServersTable.COLUMN_STATUS + " NOT IN(?,?)",
					new String[] { Constant.SERVER_DENIED_BY_SERVER,
							   Constant.SERVER_DENIED_BY_CLIENT}, null);

			if (cursor != null && cursor.getCount() > 0) {
				int count = cursor.getCount();
				cursor.moveToFirst();
				for (int i = 0; i < count; i++) {
					pairedServerId.add(cursor.getString(0));
					cursor.moveToNext();
				}
				cursor.close();
			}
			cursor = null;
			cursor = cr.query(BonjourServersTable.CONTENT_URI, null, null,
					null, null);

			if (cursor != null && cursor.getCount() > 0) {
				int count = cursor.getCount();
				cursor.moveToFirst();
				for (int i = 0; i < count; i++) {
					if (!pairedServerId.contains(cursor.getString(0))) {
						entity = new ServerEntity();
						entity.serverId = cursor.getString(0);
						entity.serverName = cursor.getString(1);
						entity.serverOS = cursor.getString(2);
						entity.wsLocation = cursor.getString(3);
						datas.add(entity);
					}
					cursor.moveToNext();
				}
				cursor.close();
			}
			cursor = null ;
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			if (cursor != null) {
				cursor.close();
				cursor = null;
			}
		}
		return datas;
	}

	public static ServerEntity getBonjourServerByServerId(Context context,
			String serverId) {
		ServerEntity entity = null;
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		try {
			cursor = cr.query(BonjourServersTable.CONTENT_URI, null,
					BonjourServersTable.COLUMN_SERVER_ID + "=?",
					new String[] { serverId }, null);

			if (cursor != null && cursor.getCount() > 0) {
				int count = cursor.getCount();
				cursor.moveToFirst();
				for (int i = 0; i < count; i++) {
					entity = new ServerEntity();
					entity.serverId = cursor.getString(0);
					entity.serverName = cursor.getString(1);
					entity.serverOS = cursor.getString(2);
					entity.wsLocation = cursor.getString(3);
					cursor.moveToNext();
				}
				cursor.close();
			}
			cursor= null;
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			if (cursor != null) {
				cursor.close();
				cursor = null;
			}
		}
		return entity;
	}
	public static String getCurrentBackupedServerName(Context context) {
		String serverName = "";
		Cursor cursor = null;
		ContentResolver cr = context.getContentResolver();
		try {
			cursor = cr.query(BackupedServersTable.CONTENT_URI, 
					new String[]{BackupedServersTable.COLUMN_SERVER_NAME},
					BackupedServersTable.COLUMN_STATUS + " NOT IN(?,?)",
					new String[] { Constant.SERVER_DENIED_BY_SERVER,
							   Constant.SERVER_DENIED_BY_CLIENT}, null);

			if (cursor != null && cursor.getCount() > 0) {
				cursor.moveToFirst();
				serverName = cursor.getString(0);
				cursor.close();
			}
			cursor= null;
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			if (cursor != null) {
				cursor.close();
				cursor = null;
			}
		}
		return serverName;
	}
	

	public static int updateAllBackedServerStatus(Context context,String status) {
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(BackupedServersTable.COLUMN_STATUS, status);
		return cr.update(BackupedServersTable.CONTENT_URI, cv,
				BackupedServersTable.COLUMN_STATUS + " NOT IN(?,?)",
				new String[] { Constant.SERVER_DENIED_BY_SERVER,
						   Constant.SERVER_DENIED_BY_CLIENT});
	}

	public static int purgeAllBonjourServer(Context context) {
		ContentResolver cr = context.getContentResolver();
		return cr.delete(BonjourServersTable.CONTENT_URI, null, null);
	}

	public static int purgeBonjourServerByServerId(Context context,
			String serverId) {
		ContentResolver cr = context.getContentResolver();
		return cr.delete(BonjourServersTable.CONTENT_URI,
				BonjourServersTable.COLUMN_SERVER_ID + "=?",
				new String[] { serverId });
	}

	public static void startWSServerConnect(Context context, String wsLocation,
			String serverId,String serverName,String ip ,String notifyPort,String restPort) {
		// SETUP WS URL ANDLink to WS
		if (RuntimeState.OnWebSocketOpened == false) {
			RuntimeWebClient.init(context);
			RuntimeWebClient.setURL(wsLocation);
			try {
				RuntimeWebClient.open();
				//TODO:CHANGE TO NEW PROTOCAL
				Intent intent = new Intent(Constant.ACTION_WEB_SOCKET_SERVER_CONNECTED);
				context.sendBroadcast(intent);
				//ConnectEntity connect = new ConnectEntity();
				ConnectForGTVEntity connectForGTV = new ConnectForGTVEntity();
				ConnectForGTVEntity.Connect  connect = new ConnectForGTVEntity.Connect();
				connect.deviceId=DeviceUtil.id(context);
				connect.deviceName = DeviceUtil
						.getDeviceNameForDisplay(context);
				connectForGTV.setConnect(connect);
				ConnectForGTVEntity.Subscribe subscribe = new ConnectForGTVEntity.Subscribe();
				subscribe.labels=true;
				connectForGTV.setSubscribe(subscribe);
				Log.d(TAG, "send message="+RuntimeState.GSON.toJson(connectForGTV));
				RuntimeWebClient.send(RuntimeState.GSON.toJson(connectForGTV));
				//TODO:ADD PAIRED SERVER
				
				ServerEntity entity = new ServerEntity();
				entity.serverId=serverId;
				entity.serverName = serverName;
				entity.ip=ip;
				entity.notifyPort=notifyPort;
				entity.restPort=restPort;
				updateBackupedServer(context, entity);
				
			} catch (WebSocketException e) {
				e.printStackTrace();
			}
		}
	}

	public static int deniedPairedServer(Context context, String serverId,String status) {
		int result = 0;
		ContentResolver cr = context.getContentResolver();
		ContentValues cv = new ContentValues();
		cv.put(BackupedServersTable.COLUMN_STATUS, status);
		Cursor cursor = null;
		try {
			cursor = cr.query(BackupedServersTable.CONTENT_URI,
					new String[] { BackupedServersTable.COLUMN_SERVER_ID },
					BackupedServersTable.COLUMN_SERVER_ID + " =? ",
					new String[] { serverId }, null);
			if (cursor != null && cursor.getCount() > 0) {
				result = cr.update(BackupedServersTable.CONTENT_URI, cv,
						BackupedServersTable.COLUMN_SERVER_ID + "=?",
						new String[] { serverId });
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
		return result;
	}



	public static String getStatusByServerId(Context context, String serverId) {
		if (serverId.equals(RuntimeState.mWebSocketServerId)) {
			return Constant.SERVER_LINKING;
		} else {
			return Constant.SERVER_OFFLINE;
		}
	}

	public static ServerEntity getServerById(Context context, String serverId) {
		ServerEntity entity = null;
		ContentResolver cr = context.getContentResolver();
		Cursor cursor = null;
		try {
			cursor = cr.query(BackupedServersTable.CONTENT_URI, null,
					BackupedServersTable.COLUMN_STATUS + " NOT IN(?,?)",
					new String[] { Constant.SERVER_DENIED_BY_SERVER,
							   Constant.SERVER_DENIED_BY_CLIENT}, null);
			if (cursor != null && cursor.getCount() > 0) {
				cursor.moveToFirst();
				entity = new ServerEntity();
				entity.serverId = cursor.getString(0);
				entity.serverName = cursor.getString(1);
				entity.status = cursor.getString(2);
				entity.startDatetime = cursor.getString(3);
				entity.endDatetime = cursor.getString(4);
				entity.folder = cursor.getString(5);
				entity.freespace = cursor.getLong(6);
				entity.photoCount = cursor.getInt(7);
				entity.videoCount = cursor.getInt(8);
				entity.audioCount = cursor.getInt(9);
				entity.lastLocalBackupTime = cursor.getString(10);
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
		return entity;
	}


	
//	public static int updateLabelInfo(Context context, LabelEntity entity) {
//		int result = 0;
//		
//		ContentResolver cr = context.getContentResolver();
//		try {
//		
//			ContentValues cv = new ContentValues();
//			cv.put(LabelTable.COLUMN_LABEL_ID, entity.label_id);
//			cv.put(LabelTable.COLUMN_LABEL_NAME, entity.label_name);
//
//			// insert label
//			result = cr.bulkInsert(LabelTable.CONTENT_URI,
//						new ContentValues[] { cv });
//			Log.d(TAG, "insert label result="+result);
//		   // inser label file
//			updateLabelFiles(context,entity.label_id,entity.files);
//		
//		} catch (Exception e) {
//			e.printStackTrace();
//		}
//		return result;
//	}
//	
//	public static int updateLabelFiles(Context context, String labelId,String[] files) {
//		int result = 0;
//		ContentResolver cr = context.getContentResolver();
//		try {
//		    for(String file: files){
//			ContentValues cv = new ContentValues();
//			cv.put(LabelFileTable.COLUMN_LABEL_ID, labelId);
//			cv.put(LabelFileTable.COLUMN_FILE_ID, file);
//			result = cr.bulkInsert(LabelFileTable.CONTENT_URI,
//						new ContentValues[] { cv });
//			Log.d(TAG, "insert label files result="+result);
//		    }	
//			
//		} catch (Exception e) {
//			e.printStackTrace();
//		}
//		return result;
//		}
	}