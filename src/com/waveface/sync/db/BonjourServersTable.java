package com.waveface.sync.db;

import android.net.Uri;
import android.provider.BaseColumns;

import com.waveface.sync.provider.SyncProvider;

public class BonjourServersTable implements BaseColumns {

	public static final String BONJOUR_SERVERS_NAME = "bonjourServers";
	public static final Uri BONJOUR_SERVER_URI = Uri.parse("content://"
			+ SyncProvider.AUTHORITY + "/" + BONJOUR_SERVERS_NAME);
	public static final Uri CONTENT_URI = BONJOUR_SERVER_URI;
//	public static final Uri BONJOUR_NOTIFY_URI = Uri.parse("content://"
//			+ SyncProvider.AUTHORITY + ".update." + BONJOUR_SERVERS_NAME);

	public static final String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.wammer.sync.bonjourServers";
	public static final String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.wammer.sync.bonjourServers";
	public static final String DEFAULT_SORT_ORDER = BonjourServersTable.COLUMN_SERVER_NAME+" DESC";

	public static final String TABLE_NAME = "BonjourServers";
	public static final String COLUMN_SERVER_ID = "id";	
	public static final String COLUMN_SERVER_NAME = "serverName";
	public static final String COLUMN_IP = "serverIP";
	public static final String COLUMN_WS_PORT = "wsPort";
	public static final String COLUMN_NOTIFY_PORT = "notifyPort";
	public static final String COLUMN_REST_PORT = "restPort";
	
	
	private BonjourServersTable() {

	}
}