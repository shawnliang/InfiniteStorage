package com.waveface.favoriteplayer.mdns;

import android.content.ContentResolver;
import android.content.Context;
import android.database.Cursor;
import android.text.TextUtils;

import com.waveface.favoriteplayer.db.BonjourServersTable;
import com.waveface.favoriteplayer.logic.ServersLogic;

public class MatchedServer {

	
	public static boolean addServer(Context context,DataPacket dataPacket){
		boolean  updated = false;
		ServerInfo newServer = dataPacket.serverInfo;
		Cursor cursor = null;
		String wsPort = null;
		String notifyPort = null;
		if(!TextUtils.isEmpty(newServer.serverId)){ 
			//Select Bonjour Server FROM DB
			ContentResolver cr = context.getContentResolver();
			try {
				cursor = cr.query(BonjourServersTable.CONTENT_URI,
						new String[] { 
						BonjourServersTable.COLUMN_WS_PORT, 
						BonjourServersTable.COLUMN_NOTIFY_PORT},
						BonjourServersTable.COLUMN_SERVER_ID + "=?",
						new String[] { newServer.serverId }, null);
				// update
				if (cursor != null && cursor.getCount() > 0) {
					cursor.moveToFirst();
					wsPort = cursor.getString(0);
					notifyPort = cursor.getString(1);
					if(TextUtils.isEmpty(wsPort) && !TextUtils.isEmpty(newServer.wsPort)){
						updated = true;
					}
					else if(!TextUtils.isEmpty(wsPort) 
						&& wsPort.equals(newServer.wsPort)){
						if(TextUtils.isEmpty(notifyPort) && !TextUtils.isEmpty(newServer.notifyPort)){
							updated = true;
						}
					}					
					cursor.close();
				}
				else{
					updated = true;
				}
				cursor = null ;
			} catch (Exception e) {
				e.printStackTrace();
			}

		}
		if(updated){
			ServersLogic.addBonjourServer(context,dataPacket);
		}
		return updated ;
	}
}
