package com.waveface.favoriteplayer.mdns;

import java.util.HashMap;

import android.content.Context;
import android.text.TextUtils;

import com.waveface.favoriteplayer.logic.ServersLogic;

public class MatchedServer {

	public static HashMap<String,ServerInfo> servers = new HashMap<String,ServerInfo>();
	
	public static boolean addServer(Context context,DataPacket dataPacket){
		boolean  updated = false;
		ServerInfo newServer = dataPacket.serverInfo;
		if(!TextUtils.isEmpty(newServer.serverId)){ 
			if(servers.containsKey(newServer.serverId)){
				ServerInfo existedServer = servers.get(newServer.serverId);
				if(TextUtils.isEmpty(existedServer.wsPort) && !TextUtils.isEmpty(newServer.wsPort)){
					servers.put(newServer.serverId, newServer);
					updated = true;
				}
				else if(!TextUtils.isEmpty(existedServer.wsPort) 
					&& existedServer.wsPort.equals(newServer.wsPort)){
					if(TextUtils.isEmpty(existedServer.notifyPort) && !TextUtils.isEmpty(newServer.notifyPort)){
						servers.put(newServer.serverId, newServer);
						updated = true;
					}
				}
			}
			else{
				servers.put(newServer.serverId, newServer);
				updated = true;
			}
		}
		if(updated){
			ServersLogic.addBonjourServer(context,dataPacket);
		}
		return updated ;
	}
}
