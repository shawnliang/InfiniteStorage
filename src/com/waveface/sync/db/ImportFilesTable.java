package com.waveface.sync.db;

import android.net.Uri;
import android.provider.BaseColumns;

import com.waveface.sync.provider.SyncProvider;

public class ImportFilesTable implements BaseColumns {

	public static final String IMPORT_FILE_NAME = "importFiles";
	public static final Uri IMPORT_FILE_URI = Uri.parse("content://"
			+ SyncProvider.AUTHORITY + "/" + IMPORT_FILE_NAME);
	public static final Uri CONTENT_URI = IMPORT_FILE_URI;
	public static final String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.wammer.dev.importfiles";
	public static final String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.wammer.dev.importfiles";
	public static final String DEFAULT_SORT_ORDER = ImportFilesTable.COLUMN_DATE+" DESC";

	public static final String TABLE_NAME = "ImportFiles";
	public static final String COLUMN_FILENAME = "filename";
	public static final String COLUMN_SIZE = "file_size";
	public static final String COLUMN_DATE = "date";
	public static final String COLUMN_IMPORTED = "imported";
	public static final String COLUMN_FOLDER = "folder";
	public static final String COLUMN_IMAGE_ID = "imageId";

	public static final int COLUMN_POS_FILENAME = 0;
	public static final int COLUMN_POS_OBJECT_ID = 1;
	public static final int COLUMN_POS_DATE = 2;
	public static final int COLUMN_POS_IMPORTED = 3;
	public static final int COLUMN_POS_FOLDER = 4;
	public static final int COLUMN_POS_IMAGE_ID = 5;

	private ImportFilesTable() {

	}
}