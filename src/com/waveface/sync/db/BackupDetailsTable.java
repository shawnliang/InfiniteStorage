package com.waveface.sync.db;

import android.net.Uri;
import android.provider.BaseColumns;

import com.waveface.sync.provider.SyncProvider;

public class BackupDetailsTable implements BaseColumns {

	public static final String BACKUP_DETAILS_NAME = "backup_details";
	public static final Uri SERVER_URI = Uri.parse("content://"
			+ SyncProvider.AUTHORITY + "/" + BACKUP_DETAILS_NAME);
	public static final Uri CONTENT_URI = SERVER_URI;
	public static final String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.wammer.sync.backupDetails";
	public static final String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.wammer.sync.backupDetails";
	public static final String DEFAULT_SORT_ORDER = BackupDetailsTable.COLUMN_SERVER_ID+" DESC";

	public static final String TABLE_NAME = "BackupDetails";
	public static final String COLUMN_SERVER_ID = "id";	
	public static final String COLUMN_FILENAME = "filename";
	public static final String COLUMN_STATUS = "status";		
	
	private BackupDetailsTable() {

	}
}