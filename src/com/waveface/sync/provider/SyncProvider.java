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

import com.waveface.sync.db.DatabaseHelper;
import com.waveface.sync.db.ImportFilesTable;
import com.waveface.sync.db.ImportTable;

public class SyncProvider extends ContentProvider {

	private final static int IMPORTS = 1;
	private final static int IMPORTS_ID = 2;

	private final static int IMPORTFILES = 3;
	private final static int IMPORTFILES_ID = 4;



	public static final String AUTHORITY = "com.waveface.sync";

	private static UriMatcher sUriMatcher;

	static {
		sUriMatcher = new UriMatcher(UriMatcher.NO_MATCH);
		//FOR IMPORT FOLDER
		sUriMatcher.addURI(AUTHORITY, ImportTable.IMPORT_NAME, IMPORTS);
		sUriMatcher.addURI(AUTHORITY, ImportTable.IMPORT_NAME + "/*", IMPORTS_ID);

		//FOR IMPORT FILES
		sUriMatcher.addURI(AUTHORITY, ImportFilesTable.IMPORT_FILE_NAME, IMPORTFILES);
		sUriMatcher.addURI(AUTHORITY, ImportFilesTable.IMPORT_FILE_NAME + "/*", IMPORTFILES_ID);

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
		case IMPORTFILES:
			tableName = ImportFilesTable.TABLE_NAME;
			break;
		case IMPORTFILES_ID:
			tableName = ImportFilesTable.TABLE_NAME;
			where = (!TextUtils.isEmpty(where) ? " AND (" + where + ")" : "");
			break;
		case IMPORTS:
			tableName = ImportTable.TABLE_NAME;
			break;
		case IMPORTS_ID:
			tableName = ImportTable.TABLE_NAME;
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
		case IMPORTS:
			return ImportTable.CONTENT_TYPE;
		case IMPORTS_ID:
			return ImportTable.CONTENT_ITEM_TYPE;
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
		case IMPORTS:
			rowId = db.insert(ImportTable.TABLE_NAME,
					ImportTable.IMPORT_NAME, values);
			if (rowId > 0) {
				Uri queueUri = ContentUris.withAppendedId(
						ImportTable.CONTENT_URI, rowId);
				getContext().getContentResolver().notifyChange(queueUri, null);
				return queueUri;
			}
			break;
		case IMPORTFILES:
			rowId = db.insert(ImportFilesTable.TABLE_NAME,
					ImportFilesTable.IMPORT_FILE_NAME, values);
			if (rowId > 0) {
				Uri queueUri = ContentUris.withAppendedId(
						ImportFilesTable.CONTENT_URI, rowId);
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
		case IMPORTS:
			if (TextUtils.isEmpty(sortOrder)) {
				orderBy = ImportTable.DEFAULT_SORT_ORDER;
			} else {
				orderBy = sortOrder;
			}
			c = mDb.query(ImportTable.TABLE_NAME, projection, where,
					whereArgs, null, null, orderBy);
			c.setNotificationUri(getContext().getContentResolver(),
					ImportTable.CONTENT_URI);
			break;
		case IMPORTS_ID:
			String importId = uri.getLastPathSegment();
			c = executeGeneralQuery(importId, ImportTable.TABLE_NAME,
					ImportTable.COLUMN_DB_ID, where, whereArgs);
			c.setNotificationUri(getContext().getContentResolver(),
					ImportTable.CONTENT_URI);
			break;
		case IMPORTFILES:
			if (TextUtils.isEmpty(sortOrder)) {
				orderBy = ImportFilesTable.DEFAULT_SORT_ORDER;
			} else {
				orderBy = sortOrder;
			}
			c = mDb.query(ImportFilesTable.TABLE_NAME, projection, where,
					whereArgs, null, null, orderBy);
			c.setNotificationUri(getContext().getContentResolver(),
					ImportFilesTable.CONTENT_URI);
			break;
		case IMPORTFILES_ID:
			String objectId = uri.getLastPathSegment();
			c = executeGeneralQuery(objectId, ImportFilesTable.TABLE_NAME,
					ImportFilesTable.COLUMN_DATE, where, whereArgs);
			c.setNotificationUri(getContext().getContentResolver(),
					ImportFilesTable.CONTENT_URI);
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
		switch (sUriMatcher.match(uri)) {
		case IMPORTS:
			affected = db.update(ImportTable.TABLE_NAME, values, where,
					whereArgs);
			break;
		case IMPORTS_ID:
			String importId = uri.getPathSegments().get(1);
			affected = db.update(ImportTable.TABLE_NAME, values,
							ImportTable.COLUMN_DB_ID
							+ "='"
							+ importId
							+ "'"
							+ (!TextUtils.isEmpty(where) ? " AND (" + where
									+ ")" : ""), whereArgs);
			break;
		case IMPORTFILES:
			affected = db.update(ImportFilesTable.TABLE_NAME, values, where,
					whereArgs);
			break;
		case IMPORTFILES_ID:
			String filename = uri.getPathSegments().get(1);
			affected = db.update(ImportFilesTable.TABLE_NAME, values,
					ImportFilesTable.COLUMN_FILENAME
							+ "='"
							+ filename
							+ "'"
							+ (!TextUtils.isEmpty(where) ? " AND (" + where
									+ ")" : ""), whereArgs);
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
		case IMPORTS:
			db.beginTransaction();
			try {
				// standard SQL insert statement, that can be reused
				insert = db.compileStatement("insert into "
						+ ImportTable.TABLE_NAME + "("
						+ ImportTable.COLUMN_FOLDER_NAME + ","
						+ ImportTable.COLUMN_TYPE + ","
						+ ImportTable.COLUMN_ENABLE + ","
						+ ImportTable.COLUMN_LAST_IMPORT_TIME + ","
						+ ImportTable.COLUMN_ADDED_TIME + ")"
						+ " values (?,?,?,?,?)");

				for (ContentValues value : values) {
					// bind the 1-indexed ?'s to the values specified
					insert.bindString(1,
							value.getAsString(ImportTable.COLUMN_FOLDER_NAME));
					insert.bindString(2,
							value.getAsString(ImportTable.COLUMN_TYPE));
					insert.bindString(3,
							value.getAsString(ImportTable.COLUMN_ENABLE));
					insert.bindString(4,
							value.getAsString(ImportTable.COLUMN_LAST_IMPORT_TIME));
					insert.bindString(5,
							value.getAsString(ImportTable.COLUMN_ADDED_TIME));
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
		case IMPORTFILES:
			db.beginTransaction();
			try {
				// standard SQL insert statement, that can be reused
				insert = db.compileStatement("INSERT INTO "
						+ ImportFilesTable.TABLE_NAME + "("
						+ ImportFilesTable.COLUMN_FILENAME + ","
						+ ImportFilesTable.COLUMN_SIZE + ","
						+ ImportFilesTable.COLUMN_DATE + ","
						+ ImportFilesTable.COLUMN_IMPORTED + ","
						+ ImportFilesTable.COLUMN_FILETYPE + ","
						+ ImportFilesTable.COLUMN_FOLDER + ","
						+ ImportFilesTable.COLUMN_IMAGE_ID + ")"
						+ " values (?,?,?,?,?,?,?)");

				for (ContentValues value : values) {
					insert.bindString(1, value.getAsString(ImportFilesTable.COLUMN_FILENAME));
					insert.bindString(2, value.getAsString(ImportFilesTable.COLUMN_SIZE));
					insert.bindString(3, value.getAsString(ImportFilesTable.COLUMN_DATE));
					insert.bindString(4, value.getAsString(ImportFilesTable.COLUMN_IMPORTED));
					insert.bindString(5, value.getAsString(ImportFilesTable.COLUMN_FILETYPE));
					insert.bindString(6, value.getAsString(ImportFilesTable.COLUMN_FOLDER));
					insert.bindString(7, value.getAsString(ImportFilesTable.COLUMN_IMAGE_ID));
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
}
