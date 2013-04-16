package com.waveface.sync.db;

import android.net.Uri;

import com.waveface.sync.provider.SyncProvider;

public class ServerFilesView {
	public static final String SERVER_FILES_VIEW_NAME = "serverFilesView";
	public static final Uri SERVER_FILES_VIEW_URI = Uri.parse("content://"
			+ SyncProvider.AUTHORITY + "/" + SERVER_FILES_VIEW_NAME);
	public static final Uri CONTENT_URI = SERVER_FILES_VIEW_URI;
	public static final String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.wammer.sync.serverFilesView";
	public static final String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.wammer.sync.serverFilesView";
    public static final String DEFAULT_SORT_ORDER = ServerFilesView.COLUMN_DATE;

	public static final String VIEW_NAME = "ServerFilesView";
	public static final String COLUMN_SERVER_ID = "server_id";
	public static final String COLUMN_FILENAME = "filename";
	public static final String COLUMN_MIMETYPE = "mimetype";	
	public static final String COLUMN_SIZE = "file_size";
	public static final String COLUMN_DATE = "date";
	public static final String COLUMN_FOLDER = "folder";

	private ServerFilesView() {
	}
}
