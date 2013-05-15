package com.waveface.sync.db;

import android.net.Uri;
import android.provider.BaseColumns;

import com.waveface.sync.provider.SyncProvider;

public class BackupedServersTable implements BaseColumns {

	public static final String BACKUPED_SERVERS_NAME = "backupedServers";
	public static final Uri BACKUPED_SERVER_URI = Uri.parse("content://"
			+ SyncProvider.AUTHORITY + "/" + BACKUPED_SERVERS_NAME);
	public static final Uri CONTENT_URI = BACKUPED_SERVER_URI;
	public static final String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.wammer.sync.backupedServers";
	public static final String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.wammer.sync.backupedServers";

	public static final String TABLE_NAME = "BackupedServers";
	public static final String COLUMN_SERVER_ID = "id";	
	public static final String COLUMN_SERVER_NAME = "serverName";
	public static final String COLUMN_IP = "serverIP";
	public static final String COLUMN_WS_PORT = "wsPort";
	public static final String COLUMN_NOTIFY_PORT = "notifyPort";
	public static final String COLUMN_REST_PORT = "restPort";
	public static final String COLUMN_STATUS = "status";
	
	private BackupedServersTable() {

	}
}