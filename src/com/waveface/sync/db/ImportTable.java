package com.waveface.sync.db;

import android.net.Uri;
import android.provider.BaseColumns;

import com.waveface.sync.provider.SyncProvider;

public class ImportTable  implements BaseColumns {

	public static final String IMPORT_NAME = "imports";
	public static final Uri IMPORT_URI = Uri.parse("content://"
			+ SyncProvider.AUTHORITY + "/" + IMPORT_NAME);
	public static final Uri CONTENT_URI = IMPORT_URI;
	public static final String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.wammer.dev.import";
	public static final String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.wammer.dev.import";
	public static final String DEFAULT_SORT_ORDER = ImportTable.COLUMN_FOLDER_NAME;

	public static final String TABLE_NAME = "Import";
	public static final String COLUMN_DB_ID = "_id";
	public static final String COLUMN_FOLDER_NAME = "folder_name";
	public static final String COLUMN_TYPE = "type";
	public static final String COLUMN_ENABLE = "enable";
	public static final String COLUMN_LAST_IMPORT_TIME = "last_import_time";
	public static final String COLUMN_ADDED_TIME = "added_time";

	public static final int COLUMN_POS_DB_ID = 0;
	public static final int COLUMN_POS_FOLDER_NAME = 1;
	public static final int COLUMN_POS_TYPE = 2;
	public static final int COLUMN_POS_ENABLE = 3;
	public static final int COLUMN_POS_LAST_IMPORT_TIME = 4;
	public static final int COLUMN_POS_ADDED_TIME = 5;

	private ImportTable() {
	}
}