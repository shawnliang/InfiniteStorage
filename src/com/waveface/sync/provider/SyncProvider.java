package com.waveface.sync.provider;

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
import com.waveface.sync.db.BackupedServersTable;
import com.waveface.sync.db.BonjourServersTable;
import com.waveface.sync.db.DatabaseHelper;

public class SyncProvider extends ContentProvider {


	private final static int BACKUPED_SERVERS = 5;
	private final static int BACKUPED_SERVERS_ID = 6;

	private final static int BONJOUR_SERVERS = 7;
	private final static int BONJOUR_SERVERS_ID = 8;
	
	
	public static final String AUTHORITY = "com.waveface.sync";

	private static UriMatcher sUriMatcher;

	static {
		sUriMatcher = new UriMatcher(UriMatcher.NO_MATCH);

		//FOR BACKUPED SERVER
		sUriMatcher.addURI(AUTHORITY, BackupedServersTable.BACKUPED_SERVERS_NAME, BACKUPED_SERVERS);
		sUriMatcher.addURI(AUTHORITY, BackupedServersTable.BACKUPED_SERVERS_NAME + "/*", BACKUPED_SERVERS_ID);

		//FOR BONJOUR SERVER
		sUriMatcher.addURI(AUTHORITY, BonjourServersTable.BONJOUR_SERVERS_NAME, BONJOUR_SERVERS);
		sUriMatcher.addURI(AUTHORITY, BonjourServersTable.BONJOUR_SERVERS_NAME + "/*", BONJOUR_SERVERS_ID);
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
			tableName = BackupedServersTable.TABLE_NAME;
			break;
		case BACKUPED_SERVERS_ID:
			tableName = BackupedServersTable.TABLE_NAME;
			where = (!TextUtils.isEmpty(where) ? " AND (" + where + ")" : "");
			break;			
		case BONJOUR_SERVERS:
			tableName = BonjourServersTable.TABLE_NAME;
			break;
		case BONJOUR_SERVERS_ID:
			tableName = BonjourServersTable.TABLE_NAME;
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
			return BackupedServersTable.CONTENT_TYPE;
		case BACKUPED_SERVERS_ID:
			return BackupedServersTable.CONTENT_ITEM_TYPE;
		case BONJOUR_SERVERS:
			return BonjourServersTable.CONTENT_TYPE;
		case BONJOUR_SERVERS_ID:
			return BonjourServersTable.CONTENT_ITEM_TYPE;
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
			rowId = db.insert(BackupedServersTable.TABLE_NAME,
					BackupedServersTable.BACKUPED_SERVERS_NAME, values);
			if (rowId > 0) {
				Uri queueUri = ContentUris.withAppendedId(
						BackupedServersTable.CONTENT_URI, rowId);
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
			if (TextUtils.isEmpty(sortOrder)) {
				orderBy = BackupedServersTable.DEFAULT_SORT_ORDER;
			} else {
				orderBy = sortOrder;
			}
			c = mDb.query(BackupedServersTable.TABLE_NAME, projection, where,
					whereArgs, null, null, orderBy);
			c.setNotificationUri(getContext().getContentResolver(),
					BackupedServersTable.CONTENT_URI);
			break;
		case BACKUPED_SERVERS_ID:
			String serverId = uri.getLastPathSegment();
			c = executeGeneralQuery(serverId, BackupedServersTable.TABLE_NAME,
					BackupedServersTable.COLUMN_SERVER_ID, where, whereArgs);
			c.setNotificationUri(getContext().getContentResolver(),
					BackupedServersTable.CONTENT_URI);
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
			affected = db.update(BackupedServersTable.TABLE_NAME, values, where,
					whereArgs);
			//SEND CUMTOMIZED NOTIFY INFO
			sendCustomizedNotifyChangedInfo(matchUri);
			break;
		case BACKUPED_SERVERS_ID:
			String serverId = uri.getPathSegments().get(1);
			affected = db.update(BackupedServersTable.TABLE_NAME, values,
					BackupedServersTable.COLUMN_SERVER_ID
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
						+ BackupedServersTable.TABLE_NAME + "("
						+ BackupedServersTable.COLUMN_SERVER_ID + ","
						+ BackupedServersTable.COLUMN_SERVER_NAME + ","
						+ BackupedServersTable.COLUMN_STATUS + ","
						+ BackupedServersTable.COLUMN_START_DATETIME + ","
						+ BackupedServersTable.COLUMN_END_DATETIME + ","
						+ BackupedServersTable.COLUMN_FOLDER + ","
						+ BackupedServersTable.COLUMN_FREE_SPACE + ","
						+ BackupedServersTable.COLUMN_PHOTO_COUNT + ","
						+ BackupedServersTable.COLUMN_VIDEO_COUNT + ","
						+ BackupedServersTable.COLUMN_AUDIO_COUNT + ","	
						+ BackupedServersTable.COLUMN_LAST_DISPLAY_BACKUP_DATETIME + ","
						+ BackupedServersTable.COLUMN_LAST_FILE_MEDIA_ID + ","
						+ BackupedServersTable.COLUMN_LAST_FILE_DATE + ","
						+ BackupedServersTable.COLUMN_LAST_FILE_UPDATED_DATETIME+ ")"
						+ " values (?,?,?,?,?,?,?,?,?,?,?,?,?,?)");

				for (ContentValues value : values) {
					insert.bindString(1, value.getAsString(BackupedServersTable.COLUMN_SERVER_ID));
					insert.bindString(2, value.getAsString(BackupedServersTable.COLUMN_SERVER_NAME));
					insert.bindString(3, value.getAsString(BackupedServersTable.COLUMN_STATUS));
					insert.bindString(4, value.getAsString(BackupedServersTable.COLUMN_START_DATETIME));
					insert.bindString(5, value.getAsString(BackupedServersTable.COLUMN_END_DATETIME));
					insert.bindString(6, value.getAsString(BackupedServersTable.COLUMN_FOLDER));
					insert.bindString(7, value.getAsString(BackupedServersTable.COLUMN_FREE_SPACE));
					insert.bindString(8, value.getAsString(BackupedServersTable.COLUMN_PHOTO_COUNT));
					insert.bindString(9, value.getAsString(BackupedServersTable.COLUMN_VIDEO_COUNT));
					insert.bindString(10, value.getAsString(BackupedServersTable.COLUMN_AUDIO_COUNT));			
					insert.bindString(11, value.getAsString(BackupedServersTable.COLUMN_LAST_DISPLAY_BACKUP_DATETIME));
					insert.bindString(12, value.getAsString(BackupedServersTable.COLUMN_LAST_FILE_MEDIA_ID));				
					insert.bindString(13, value.getAsString(BackupedServersTable.COLUMN_LAST_FILE_DATE));														
					insert.bindString(14, value.getAsString(BackupedServersTable.COLUMN_LAST_FILE_UPDATED_DATETIME));									
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
				insert = db.compileStatement("INSERT INTO "
						+ BonjourServersTable.TABLE_NAME + "("
						+ BonjourServersTable.COLUMN_SERVER_ID + ","
						+ BonjourServersTable.COLUMN_SERVER_NAME + ","
						+ BonjourServersTable.COLUMN_SERVER_OS + ","
						+ BonjourServersTable.COLUMN_WS_LOCATION + ")"						
						+ " values (?,?,?,?)");

				for (ContentValues value : values) {
					insert.bindString(1, value.getAsString(BonjourServersTable.COLUMN_SERVER_ID));
					insert.bindString(2, value.getAsString(BonjourServersTable.COLUMN_SERVER_NAME));
					insert.bindString(3, value.getAsString(BonjourServersTable.COLUMN_SERVER_OS));
					insert.bindString(4, value.getAsString(BonjourServersTable.COLUMN_WS_LOCATION));
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
			getContext().getContentResolver().notifyChange(BackupedServersTable.BACKUPED_SERVER_URI, null);
			break;
		}
	}
}
