package com.waveface.sync.db;

import android.net.Uri;
import android.provider.BaseColumns;

import com.waveface.sync.provider.SyncProvider;

public class ImportFilesTable implements BaseColumns {

	public static final String IMPORT_FILE_NAME = "importFiles";
	public static final Uri IMPORT_FILE_URI = Uri.parse("content://"
			+ SyncProvider.AUTHORITY + "/" + IMPORT_FILE_NAME);
	public static final Uri CONTENT_URI = IMPORT_FILE_URI;
	public static final String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.wammer.sync.importfiles";
	public static final String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.wammer.sync.importfiles";
	public static final String DEFAULT_SORT_ORDER = ImportFilesTable.COLUMN_DATE+" DESC";

	public static final String TABLE_NAME = "ImportFiles";
	public static final String COLUMN_FILENAME = "filename";
	public static final String COLUMN_MIMETYPE = "mimetype";	
	public static final String COLUMN_SIZE = "file_size";
	public static final String COLUMN_DATE = "date";
	public static final String COLUMN_STATUS = "status";
	public static final String COLUMN_FILETYPE = "file_type";	
	public static final String COLUMN_FOLDER = "folder";
	public static final String COLUMN_IMAGE_ID = "imageId";
	
	private ImportFilesTable() {

	}
}