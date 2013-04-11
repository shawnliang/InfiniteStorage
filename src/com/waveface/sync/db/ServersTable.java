package com.waveface.sync.db;

import android.net.Uri;
import android.provider.BaseColumns;

import com.waveface.sync.provider.SyncProvider;

public class ServersTable implements BaseColumns {

	public static final String SERVERS_NAME = "servers";
	public static final Uri SERVER_URI = Uri.parse("content://"
			+ SyncProvider.AUTHORITY + "/" + SERVERS_NAME);
	public static final Uri CONTENT_URI = SERVER_URI;
	public static final String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.wammer.sync.server";
	public static final String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.wammer.sync.server";
	public static final String DEFAULT_SORT_ORDER = ServersTable.COLUMN_SERVER_NAME+" DESC";

	public static final String TABLE_NAME = "Servers";
	public static final String COLUMN_SERVER_ID = "id";	
	public static final String COLUMN_SERVER_NAME = "serverName";
	public static final String COLUMN_STATUS = "status";		
	public static final String COLUMN_START_DATETIME = "startDatetime";
	public static final String COLUMN_END_DATETIME = "endDatetime";	
	public static final String COLUMN_FOLDER = "folderName";
	public static final String COLUMN_FREE_SPACE = "freeSpace";
	public static final String COLUMN_PHOTO_COUNT = "photoCount";
	public static final String COLUMN_VIDEO_COUNT = "videoCount";	
	public static final String COLUMN_AUDIO_COUNT = "audioCount";

	public static final int COLUMN_POS_SERVER_ID = 0;
	public static final int COLUMN_POS_SERVER_NAME = 1;
	public static final int COLUMN_POS_STATUS = 2;
	public static final int COLUMN_POS_START_DATETIME = 3;
	public static final int COLUMN_POS_END_DATETIME = 4;	
	public static final int COLUMN_POS_FOLDER = 5;
	public static final int COLUMN_POS_FREE_SPACE = 6;
	public static final int COLUMN_POS_PHOTO_COUNT = 7;	
	public static final int COLUMN_POS_VIDEO_COUNT = 8;
	public static final int COLUMN_POS_AUDIO_COUNT = 9;
	
	
	private ServersTable() {

	}
}