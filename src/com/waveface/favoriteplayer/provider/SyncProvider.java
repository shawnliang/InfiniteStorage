package com.waveface.favoriteplayer.provider;

import android.content.ContentProvider;

import android.content.ContentUris;
import android.content.ContentValues;
import android.content.Context;
import android.content.UriMatcher;
import android.database.AbstractWindowedCursor;
import android.database.Cursor;
import android.database.SQLException;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteStatement;
import android.net.Uri;
import android.text.TextUtils;

import com.waveface.favoriteplayer.db.LabelFileView;
import com.waveface.favoriteplayer.db.PairedServersTable;
import com.waveface.favoriteplayer.db.BonjourServersTable;
import com.waveface.favoriteplayer.db.DatabaseHelper;
import com.waveface.favoriteplayer.db.FileTable;
import com.waveface.favoriteplayer.db.LabelFileTable;
import com.waveface.favoriteplayer.db.LabelTable;


public class SyncProvider extends ContentProvider {


	private final static int BACKUPED_SERVERS = 5;
	private final static int BACKUPED_SERVERS_ID = 6;

	private final static int BONJOUR_SERVERS = 7;
	private final static int BONJOUR_SERVERS_ID = 8;
	
	private final static int LABELS = 9;
	private final static int LABELS_ID = 10;
	
	private final static int LABELFILES = 11;
	private final static int LABELFILES_ID = 12;

	private final static int FILES = 13;
	private final static int FILES_ID = 14;

	
	private final static int LABELFILEVIEWS = 101;
	private final static int LABELFILEVIEWS_ID = 102;

	public static final String AUTHORITY = "com.waveface.favoriteplayer";

	private static UriMatcher sUriMatcher;

	static {
		sUriMatcher = new UriMatcher(UriMatcher.NO_MATCH);

		//FOR BACKUPED SERVER
		sUriMatcher.addURI(AUTHORITY, PairedServersTable.BACKUPED_SERVERS_NAME, BACKUPED_SERVERS);
		sUriMatcher.addURI(AUTHORITY, PairedServersTable.BACKUPED_SERVERS_NAME + "/*", BACKUPED_SERVERS_ID);

		//FOR BONJOUR SERVER
		sUriMatcher.addURI(AUTHORITY, BonjourServersTable.BONJOUR_SERVERS_NAME, BONJOUR_SERVERS);
		sUriMatcher.addURI(AUTHORITY, BonjourServersTable.BONJOUR_SERVERS_NAME + "/*", BONJOUR_SERVERS_ID);
		
		//FOR LABEL
		sUriMatcher.addURI(AUTHORITY, LabelTable.LABELS_NAME, LABELS);
		sUriMatcher.addURI(AUTHORITY, LabelTable.LABELS_NAME + "/*", LABELS_ID);

		//FOR FILES
		sUriMatcher.addURI(AUTHORITY, FileTable.FILES_NAME, FILES);
		sUriMatcher.addURI(AUTHORITY, FileTable.FILES_NAME + "/*", FILES_ID);		
		
		//FOR LABEL FILES
		sUriMatcher.addURI(AUTHORITY, LabelFileTable.LABELFILE_NAME, LABELFILES);
		sUriMatcher.addURI(AUTHORITY, LabelFileTable.LABELFILE_NAME + "/*", LABELFILES_ID);
				
		//FOR LABEL FILES VIEW
		sUriMatcher.addURI(AUTHORITY, LabelFileView.LABEL_FILE_VIEW_NAME, LABELFILEVIEWS);
		sUriMatcher.addURI(AUTHORITY, LabelFileView.LABEL_FILE_VIEW_NAME + "/*", LABELFILEVIEWS_ID);

	
	
	}

	private DatabaseHelper mOpenHelper;
	private SQLiteDatabase mDb;

	public class DummyCursor extends AbstractWindowedCursor{

		public DummyCursor(){}
		@Override
		public int getCount(){
			return 0;
		}
		@Override
		public String[] getColumnNames() {
			// TODO Auto-generated method stub
			return null;
		}

	}

	public SyncProvider() {
	}

	public SyncProvider(Context context) {
		init();
	}

	private void init() {
		mOpenHelper = new DatabaseHelper(getContext());
		mDb = mOpenHelper.getWritableDatabase();
	}

	private Cursor executeGeneralQuery(String id, String tableName,
			String Column, String where, String[] whereArgs) {
		Cursor c;
		String w = (!TextUtils.isEmpty(where) ? " AND (" + where + ")" : "");
		String selection = String.format("%s = \"%s\" %s", Column, id, w);
		c = mDb.query(tableName, null, selection, whereArgs, null, null, null);
		return c;
	}

	@Override
	public int delete(Uri uri, String where, String[] whereArgs) {
		int match = sUriMatcher.match(uri);
		int affected;
		String tableName;

		switch (match) {
		case BACKUPED_SERVERS:
			tableName = PairedServersTable.TABLE_NAME;
			break;
		case BACKUPED_SERVERS_ID:
			tableName = PairedServersTable.TABLE_NAME;
			where = (!TextUtils.isEmpty(where) ? " AND (" + where + ")" : "");
			break;			
		case BONJOUR_SERVERS:
			tableName = BonjourServersTable.TABLE_NAME;
			break;
		case BONJOUR_SERVERS_ID:
			tableName = BonjourServersTable.TABLE_NAME;
			where = (!TextUtils.isEmpty(where) ? " AND (" + where + ")" : "");
			break;	
		case LABELS:
			tableName = LabelTable.TABLE_NAME;
			break;
		case LABELS_ID:
			tableName = LabelTable.TABLE_NAME;
			where = (!TextUtils.isEmpty(where) ? " AND (" + where + ")" : "");
			break;				
		case LABELFILES:
			tableName = LabelFileTable.TABLE_NAME;
			break;
		case LABELFILES_ID:
			tableName = LabelFileTable.TABLE_NAME;
			where = (!TextUtils.isEmpty(where) ? " AND (" + where + ")" : "");
			break;
		case FILES:
			tableName = FileTable.TABLE_NAME;
			break;
		case FILES_ID:
			tableName = FileTable.TABLE_NAME;
			where = (!TextUtils.isEmpty(where) ? " AND (" + where + ")" : "");
			break;			
		default:
			throw new IllegalArgumentException("unknown post element: " + uri);
		}
		affected = mDb.delete(tableName, where, whereArgs);
		getContext().getContentResolver().notifyChange(uri, null);
		return affected;
	}

	@Override
	public String getType(Uri uri) {
		switch (sUriMatcher.match(uri)) {
		case BACKUPED_SERVERS:
			return PairedServersTable.CONTENT_TYPE;
		case BACKUPED_SERVERS_ID:
			return PairedServersTable.CONTENT_ITEM_TYPE;
		case BONJOUR_SERVERS:
			return BonjourServersTable.CONTENT_TYPE;
		case BONJOUR_SERVERS_ID:
			return BonjourServersTable.CONTENT_ITEM_TYPE;
		case LABELS:
			return LabelTable.CONTENT_TYPE;
		case LABELS_ID:
			return LabelTable.CONTENT_ITEM_TYPE;	
		case LABELFILES:
			return LabelFileTable.CONTENT_TYPE;
		case LABELFILES_ID:
			return LabelFileTable.CONTENT_ITEM_TYPE;
		case FILES:
			return FileTable.CONTENT_TYPE;
		case FILES_ID:
			return FileTable.CONTENT_ITEM_TYPE;				
		case LABELFILEVIEWS:
			return LabelFileView.CONTENT_TYPE;
		case LABELFILEVIEWS_ID:
			return LabelFileView.CONTENT_ITEM_TYPE;				
		default:
			throw new IllegalArgumentException("Unknown post type: " + uri);
		}
	}

	@Override
	public Uri insert(Uri uri, ContentValues initialValues) {
		ContentValues values;
		SQLiteDatabase db = mOpenHelper.getWritableDatabase();
		long rowId;

		if (initialValues != null) {
			values = new ContentValues(initialValues);
		} else {
			values = new ContentValues();
		}

		switch (sUriMatcher.match(uri)) {
		case BACKUPED_SERVERS:
			rowId = db.insert(PairedServersTable.TABLE_NAME,
					PairedServersTable.BACKUPED_SERVERS_NAME, values);
			if (rowId > 0) {
				Uri queueUri = ContentUris.withAppendedId(
						PairedServersTable.CONTENT_URI, rowId);
				getContext().getContentResolver().notifyChange(queueUri, null);
				return queueUri;
			}
			break;
		case BONJOUR_SERVERS:
			rowId = db.insert(BonjourServersTable.TABLE_NAME,
					BonjourServersTable.BONJOUR_SERVERS_NAME, values);
			if (rowId > 0) {
				Uri queueUri = ContentUris.withAppendedId(
						BonjourServersTable.CONTENT_URI, rowId);
				getContext().getContentResolver().notifyChange(queueUri, null);
				return queueUri;
			}
			break;
		case LABELS:
			rowId = db.insert(LabelTable.TABLE_NAME,
					LabelTable.LABELS_NAME, values);
			if (rowId > 0) {
				Uri queueUri = ContentUris.withAppendedId(
						LabelTable.CONTENT_URI, rowId);
				getContext().getContentResolver().notifyChange(queueUri, null);
				return queueUri;
			}
		case LABELFILES:
			rowId = db.insert(LabelFileTable.TABLE_NAME,
					LabelFileTable.LABELFILE_NAME, values);
			if (rowId > 0) {
				Uri queueUri = ContentUris.withAppendedId(
						LabelFileTable.CONTENT_URI, rowId);
				getContext().getContentResolver().notifyChange(queueUri, null);
				return queueUri;
			}	
			break;
		case FILES:
			rowId = db.insert(FileTable.TABLE_NAME,
					FileTable.FILES_NAME, values);
			if (rowId > 0) {
				Uri queueUri = ContentUris.withAppendedId(
						FileTable.CONTENT_URI, rowId);
				getContext().getContentResolver().notifyChange(queueUri, null);
				return queueUri;
			}	
			break;				
		default:
			throw new IllegalArgumentException("Unknown URI " + uri);
		}
		throw new SQLException("Failed to insert row into " + uri);
	}

	@Override
	public boolean onCreate() {
		init();
		return true;
	}

	@Override
	public Cursor query(Uri uri, String[] projection, String where,
			String[] whereArgs, String sortOrder) {
		String orderBy;

		Cursor c = null;
		int match = sUriMatcher.match(uri);

		switch (match) {
		case BACKUPED_SERVERS:

			c = mDb.query(PairedServersTable.TABLE_NAME, projection, where,
					whereArgs, null, null, null);
			c.setNotificationUri(getContext().getContentResolver(),
					PairedServersTable.CONTENT_URI);
			break;
		case BACKUPED_SERVERS_ID:
			String serverId = uri.getLastPathSegment();
			c = executeGeneralQuery(serverId, PairedServersTable.TABLE_NAME,
					PairedServersTable.COLUMN_SERVER_ID, where, whereArgs);
			c.setNotificationUri(getContext().getContentResolver(),
					PairedServersTable.CONTENT_URI);
			break;
		case BONJOUR_SERVERS:
			if (TextUtils.isEmpty(sortOrder)) {
				orderBy = BonjourServersTable.DEFAULT_SORT_ORDER;
			} else {
				orderBy = sortOrder;
			}
			c = mDb.query(BonjourServersTable.TABLE_NAME, projection, where,
					whereArgs, null, null, orderBy);
			c.setNotificationUri(getContext().getContentResolver(),
					BonjourServersTable.CONTENT_URI);
			break;
		case BONJOUR_SERVERS_ID:
			String bonjourServerId = uri.getLastPathSegment();
			c = executeGeneralQuery(bonjourServerId, BonjourServersTable.TABLE_NAME,
					BonjourServersTable.COLUMN_SERVER_ID, where, whereArgs);
			c.setNotificationUri(getContext().getContentResolver(),
					BonjourServersTable.CONTENT_URI);
			break;
		case LABELS:
			if (TextUtils.isEmpty(sortOrder)) {
				orderBy = LabelTable.DEFAULT_SORT_ORDER;
			} else {
				orderBy = sortOrder;
			}
			c = mDb.query(LabelTable.TABLE_NAME, projection, where,
					whereArgs, null, null, orderBy);
			c.setNotificationUri(getContext().getContentResolver(),
					LabelTable.CONTENT_URI);
			break;
		case LABELS_ID:
			String labelId = uri.getLastPathSegment();
			c = executeGeneralQuery(labelId, LabelTable.TABLE_NAME,
					LabelTable.COLUMN_LABEL_ID, where, whereArgs);
			c.setNotificationUri(getContext().getContentResolver(),
					LabelTable.CONTENT_URI);
			break;
		case LABELFILES:
			if (TextUtils.isEmpty(sortOrder)) {
				orderBy = LabelFileTable.DEFAULT_SORT_ORDER;
			} else {
				orderBy = sortOrder;
			}
			c = mDb.query(LabelFileTable.TABLE_NAME, projection, where,
					whereArgs, null, null, orderBy);
			c.setNotificationUri(getContext().getContentResolver(),
					LabelFileTable.CONTENT_URI);
			break;
		case LABELFILES_ID:
			String labelFileId = uri.getLastPathSegment();
			c = executeGeneralQuery(labelFileId, LabelFileTable.TABLE_NAME,
					LabelFileTable.COLUMN_LABEL_ID, where, whereArgs);
			c.setNotificationUri(getContext().getContentResolver(),
					LabelFileTable.CONTENT_URI);
			break;
		case FILES:
			if (TextUtils.isEmpty(sortOrder)) {
				orderBy = FileTable.DEFAULT_SORT_ORDER;
			} else {
				orderBy = sortOrder;
			}
			c = mDb.query(FileTable.TABLE_NAME, projection, where,
					whereArgs, null, null, orderBy);
			c.setNotificationUri(getContext().getContentResolver(),
					FileTable.CONTENT_URI);
			break;
		case FILES_ID:
			String fileId = uri.getLastPathSegment();
			c = executeGeneralQuery(fileId, LabelFileTable.TABLE_NAME,
					FileTable.COLUMN_FILE_ID, where, whereArgs);
			c.setNotificationUri(getContext().getContentResolver(),
					FileTable.CONTENT_URI);
			break;
		case LABELFILEVIEWS:
			if (TextUtils.isEmpty(sortOrder)) {
				orderBy = LabelFileView.DEFAULT_SORT_ORDER;
			} else {
				orderBy = sortOrder;
			}
			c = mDb.query(LabelFileView.VIEW_NAME, projection, where,
					whereArgs, null, null, orderBy);
			c.setNotificationUri(getContext().getContentResolver(),
					LabelFileView.CONTENT_URI);
			break;
		case LABELFILEVIEWS_ID:
			String labelFileViewId = uri.getLastPathSegment();
			c = executeGeneralQuery(labelFileViewId, LabelFileView.VIEW_NAME,
					LabelFileView.COLUMN_LABEL_ID, where, whereArgs);
			c.setNotificationUri(getContext().getContentResolver(),
					LabelFileView.CONTENT_URI);
			break;
			
		default:
			c = new DummyCursor();
			break;
		}
		return c;
	}

	@Override
	public synchronized int update(Uri uri, ContentValues values, String where,
			String[] whereArgs) {
		SQLiteDatabase db = mOpenHelper.getWritableDatabase();
		int affected;
		int matchUri = sUriMatcher.match(uri);
		switch (matchUri) {
		case BACKUPED_SERVERS:
			affected = db.update(PairedServersTable.TABLE_NAME, values, where,
					whereArgs);
			//SEND CUMTOMIZED NOTIFY INFO
			sendCustomizedNotifyChangedInfo(matchUri);
			break;
		case BACKUPED_SERVERS_ID:
			String serverId = uri.getPathSegments().get(1);
			affected = db.update(PairedServersTable.TABLE_NAME, values,
					PairedServersTable.COLUMN_SERVER_ID
							+ "='"
							+ serverId
							+ "'"
							+ (!TextUtils.isEmpty(where) ? " AND (" + where
									+ ")" : ""), whereArgs);
			//SEND CUMTOMIZED NOTIFY INFO
			sendCustomizedNotifyChangedInfo(matchUri);
			break;
		case BONJOUR_SERVERS:
			affected = db.update(BonjourServersTable.TABLE_NAME, values, where,
					whereArgs);
			//SEND CUMTOMIZED NOTIFY INFO
			sendCustomizedNotifyChangedInfo(matchUri);
			break;
		case BONJOUR_SERVERS_ID:
			String bonjourServerId = uri.getPathSegments().get(1);
			affected = db.update(BonjourServersTable.TABLE_NAME, values,
					BonjourServersTable.COLUMN_SERVER_ID
							+ "='"
							+ bonjourServerId
							+ "'"
							+ (!TextUtils.isEmpty(where) ? " AND (" + where
									+ ")" : ""), whereArgs);
			//SEND CUMTOMIZED NOTIFY INFO
			sendCustomizedNotifyChangedInfo(matchUri);
			break;

		default:
			throw new IllegalArgumentException("Unknown URI " + uri);
		}

		getContext().getContentResolver().notifyChange(uri, null);
		return affected;
	}

	@Override
	public synchronized int bulkInsert(Uri uri, ContentValues[] values) {
		final SQLiteDatabase db = mOpenHelper.getWritableDatabase();
		SQLiteStatement insert = null;
		final int match = sUriMatcher.match(uri);
		int numInserted = 0;
		switch (match) {
		case BACKUPED_SERVERS:
			db.beginTransaction();
			try {
				// standard SQL insert statement, that can be reused
				insert = db.compileStatement("INSERT INTO "
						+ PairedServersTable.TABLE_NAME + "("
						+ PairedServersTable.COLUMN_SERVER_ID + ","
						+ PairedServersTable.COLUMN_SERVER_NAME + ","
						+ PairedServersTable.COLUMN_STATUS + ","
						+ PairedServersTable.COLUMN_IP + ","
						+ PairedServersTable.COLUMN_WS_PORT + ","
						+ PairedServersTable.COLUMN_NOTIFY_PORT + ","
						+ PairedServersTable.COLUMN_REST_PORT  + ")"
						+ " values (?,?,?,?,?,?,?)");

				for (ContentValues value : values) {
					insert.bindString(1, value.getAsString(PairedServersTable.COLUMN_SERVER_ID));
					insert.bindString(2, value.getAsString(PairedServersTable.COLUMN_SERVER_NAME));
					insert.bindString(3, value.getAsString(PairedServersTable.COLUMN_STATUS));
					insert.bindString(4, value.getAsString(PairedServersTable.COLUMN_IP));
					insert.bindString(5, value.getAsString(PairedServersTable.COLUMN_WS_PORT));
					insert.bindString(6, value.getAsString(PairedServersTable.COLUMN_NOTIFY_PORT));
					insert.bindString(7, value.getAsString(PairedServersTable.COLUMN_REST_PORT));
				
					insert.execute();
				}
				db.setTransactionSuccessful();
				//SEND CUMTOMIZED NOTIFY INFO
				sendCustomizedNotifyChangedInfo(BACKUPED_SERVERS);
				numInserted = values.length;
			} finally {
				if (insert != null) {
					insert.close();
				}
				db.endTransaction();
			}
			return numInserted;
		case BONJOUR_SERVERS:
			db.beginTransaction();
			try {
				// standard SQL insert statement, that can be reused
				insert = db.compileStatement("INSERT OR REPLACE INTO "
						+ BonjourServersTable.TABLE_NAME + "("
						+ BonjourServersTable.COLUMN_SERVER_ID + ","
						+ BonjourServersTable.COLUMN_SERVER_NAME + ","
						+ BonjourServersTable.COLUMN_IP + ","						
						+ BonjourServersTable.COLUMN_WS_PORT + ","
						+ BonjourServersTable.COLUMN_NOTIFY_PORT + ","						
						+ BonjourServersTable.COLUMN_REST_PORT + ")"						
						+ " values (?,?,?,?,?,?)");

				for (ContentValues value : values) {
					insert.bindString(1, value.getAsString(BonjourServersTable.COLUMN_SERVER_ID));
					insert.bindString(2, value.getAsString(BonjourServersTable.COLUMN_SERVER_NAME));
					insert.bindString(3, value.getAsString(BonjourServersTable.COLUMN_IP));					
					insert.bindString(4, value.getAsString(BonjourServersTable.COLUMN_WS_PORT));
					insert.bindString(5, value.getAsString(BonjourServersTable.COLUMN_NOTIFY_PORT));
					insert.bindString(6, value.getAsString(BonjourServersTable.COLUMN_REST_PORT));
					insert.execute();
				}
				db.setTransactionSuccessful();
				//SEND CUMTOMIZED NOTIFY INFO
				sendCustomizedNotifyChangedInfo(BONJOUR_SERVERS);
				
				numInserted = values.length;
			} finally {
				if (insert != null) {
					insert.close();
				}
				db.endTransaction();
			}
			return numInserted;
		case LABELS:
			db.beginTransaction();
			try {
				// standard SQL insert statement, that can be reused
				insert = db.compileStatement("INSERT OR REPLACE INTO "
						+ LabelTable.TABLE_NAME + "("
						+ LabelTable.COLUMN_LABEL_ID + ","
						+ LabelTable.COLUMN_LABEL_NAME + ","
						+ LabelTable.COLUMN_SEQ + ")"
						+ " values (?,?,?)");

				for (ContentValues value : values) {
					// bind the 1-indexed ?'s to the values specified
					insert.bindString(1, value.getAsString(LabelTable.COLUMN_LABEL_ID));
					insert.bindString(2, value.getAsString(LabelTable.COLUMN_LABEL_NAME));
					insert.bindString(3, value.getAsString(LabelTable.COLUMN_SEQ));
					insert.execute();
				}
				db.setTransactionSuccessful();
				numInserted = values.length;
			} finally {
				if (insert != null) {
					insert.close();
				}
				db.endTransaction();
			}
			return numInserted;
		case LABELFILES:
			db.beginTransaction();
			try {
				// standard SQL insert statement, that can be reused
				insert = db.compileStatement("INSERT OR REPLACE INTO "
						+ LabelFileTable.TABLE_NAME + "("
						+ LabelFileTable.COLUMN_LABEL_ID + ","
						+LabelFileTable.COLUMN_FILE_ID + ","
						+LabelFileTable.COLUMN_ORDER+ ")"
						+ " values (?,?,?)");

				for (ContentValues value : values) {
					// bind the 1-indexed ?'s to the values specified
					insert.bindString(1, value.getAsString(LabelFileTable.COLUMN_LABEL_ID));
					insert.bindString(2, value.getAsString(LabelFileTable.COLUMN_FILE_ID));
					insert.bindString(3, value.getAsString(LabelFileTable.COLUMN_ORDER));
					insert.execute();
				}
				db.setTransactionSuccessful();
				numInserted = values.length;
			} finally {
				if (insert != null) {
					insert.close();
				}
				db.endTransaction();
			}
			return numInserted;	
		case FILES:
			db.beginTransaction();
			try {
				// standard SQL insert statement, that can be reused
				insert = db.compileStatement("INSERT OR REPLACE INTO "
						+ FileTable.TABLE_NAME + "("
						+ FileTable.COLUMN_FILE_ID + ","
						+ FileTable.COLUMN_FILE_NAME + ","
						+ FileTable.COLUMN_FOLDER + ","
						+ FileTable.COLUMN_THUMB_READY + ","
						+ FileTable.COLUMN_TYPE + ","
						+ FileTable.COLUMN_DEV_ID + ","
						+ FileTable.COLUMN_DEV_NAME + ","
						+FileTable.COLUMN_DEV_TYPE+ ")"
						+ " values (?,?,?,?,?,?,?,?)");

				for (ContentValues value : values) {
					// bind the 1-indexed ?'s to the values specified
					insert.bindString(1, value.getAsString(FileTable.COLUMN_FILE_ID));
					insert.bindString(2, value.getAsString(FileTable.COLUMN_FILE_NAME));
					insert.bindString(3, value.getAsString(FileTable.COLUMN_FOLDER));
					insert.bindString(4, value.getAsString(FileTable.COLUMN_THUMB_READY));
					insert.bindString(5, value.getAsString(FileTable.COLUMN_TYPE));
					insert.bindString(6, value.getAsString(FileTable.COLUMN_DEV_ID));
					insert.bindString(7, value.getAsString(FileTable.COLUMN_DEV_NAME));
					insert.bindString(8, value.getAsString(FileTable.COLUMN_DEV_TYPE));
					insert.execute();
				}
				db.setTransactionSuccessful();
				numInserted = values.length;
			} finally {
				if (insert != null) {
					insert.close();
				}
				db.endTransaction();
			}
			return numInserted;				
		default:
			throw new UnsupportedOperationException("unsupported uri: " + uri);
		}
	}
	
	public void sendCustomizedNotifyChangedInfo(int match){
		
		switch(match){

		case BONJOUR_SERVERS:
		case BONJOUR_SERVERS_ID:
//			Uri bonjourUri = Uri.withAppendedPath(BonjourServersTable.BONJOUR_SERVER_URI, "");
			getContext().getContentResolver().notifyChange(BonjourServersTable.BONJOUR_SERVER_URI, null);
			break;
		case BACKUPED_SERVERS:
		case BACKUPED_SERVERS_ID:
			getContext().getContentResolver().notifyChange(PairedServersTable.BACKUPED_SERVER_URI, null);
			break;
		}
	}
}
