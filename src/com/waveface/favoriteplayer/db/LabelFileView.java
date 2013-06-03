package com.waveface.favoriteplayer.db;

import android.net.Uri;

import com.waveface.favoriteplayer.provider.PlayerProvider;



public class LabelFileView {
	
	public static final String LABEL_FILE_VIEW_NAME = "labelFileView";
	public static final Uri LABEL_FILE_VIEW_URI = Uri.parse("content://"
			+ PlayerProvider.AUTHORITY + "/" + LABEL_FILE_VIEW_NAME);
	public static final Uri CONTENT_URI = LABEL_FILE_VIEW_URI;
	public static final String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.favoritplayer.labelFileView";
	public static final String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.favoritplayer.labelFileView";
    public static final String DEFAULT_SORT_ORDER = LabelFileView.COLUMN_EVENT_TIME ;

	public static final String VIEW_NAME = "LabelFileView";
	public static final String COLUMN_LABEL_ID = "label_id";
	public static final String COLUMN_FILE_ID = "file_id";
	public static final String COLUMN_ORDER = "orders";
	public static final String COLUMN_FILE_NAME = "fileName";
	public static final String COLUMN_FOLDER = "folder";
	public static final String COLUMN_THUMB_READY = "thumb_ready";
	public static final String COLUMN_TYPE = "type";
	public static final String COLUMN_DEV_ID = "dev_id";
	public static final String COLUMN_DEV_NAME = "dev_name";
	public static final String COLUMN_DEV_TYPE = "dev_type";
	public static final String COLUMN_WIDTH="width";
	public static final String COLUMN_HEIGHT="height";
	public static final String COLUMN_EVENT_TIME="event_time";
}
