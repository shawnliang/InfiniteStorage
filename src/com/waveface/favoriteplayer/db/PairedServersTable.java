package com.waveface.favoriteplayer.db;

import android.net.Uri;
import android.provider.BaseColumns;

import com.waveface.favoriteplayer.provider.PlayerProvider;

public class PairedServersTable implements BaseColumns {

	public static final String BACKUPED_SERVERS_NAME = "backupedServers";
	public static final Uri BACKUPED_SERVER_URI = Uri.parse("content://"
			+ PlayerProvider.AUTHORITY + "/" + BACKUPED_SERVERS_NAME);
	public static final Uri CONTENT_URI = BACKUPED_SERVER_URI;
	public static final String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.favoritplayer.backupedServers";
	public static final String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.favoritplayer.backupedServers";

	public static final String TABLE_NAME = "BackupedServers";
	public static final String COLUMN_SERVER_ID = "id";	
	public static final String COLUMN_SERVER_NAME = "serverName";
	public static final String COLUMN_IP = "serverIP";
	public static final String COLUMN_WS_PORT = "wsPort";
	public static final String COLUMN_NOTIFY_PORT = "notifyPort";
	public static final String COLUMN_REST_PORT = "restPort";
	public static final String COLUMN_STATUS = "status";
	
	private PairedServersTable() {

	}
}
