package com.waveface.sync.db;

import java.text.MessageFormat;

import android.content.Context;
import android.content.pm.ApplicationInfo;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;

import com.waveface.sync.util.Log;

public class DatabaseHelper extends SQLiteOpenHelper {
	public static final String DATABASE_NAME = "infinites.db";

	private static final String TAG = DatabaseHelper.class.getSimpleName();
	private static final int DATABASE_VERSION = 1;
	private static final String[] TABLE_NAME_LIST = {
			ImportTable.TABLE_NAME,
			ImportFilesTable.TABLE_NAME,
			ServersTable.TABLE_NAME};


	private static final String INDEX_IMPORT_FILE_1 = "IDX_"+ImportFilesTable.TABLE_NAME+"_1";

	private final boolean DEBUGGABLE;


	public DatabaseHelper(Context context) {
		super(context, DATABASE_NAME, null, DATABASE_VERSION);
		Log.d(TAG, "Finishing Constructing DatabaseHelper");
		DEBUGGABLE = (0 != (context.getApplicationInfo().flags & ApplicationInfo.FLAG_DEBUGGABLE));
	}

	private void createTable(SQLiteDatabase db, String sql, String tableName) {
		String createSql = MessageFormat.format(sql, tableName);
		db.execSQL(createSql);
		Log.d(TAG, "In onCreate : SQL " + createSql);
	}

	@Override
	public void onCreate(SQLiteDatabase db) {
		Log.i(TAG, "onCreate at WammerProvider!!!!");
		StringBuilder sqlBuilder = new StringBuilder();

		// Create Import table
		sqlBuilder = new StringBuilder();
		sqlBuilder.append("Create Table  {0} (")
				.append(ImportTable.COLUMN_DB_ID+" INTEGER PRIMARY KEY AUTOINCREMENT,")
				.append(ImportTable.COLUMN_FOLDER_NAME + " TEXT NOT NULL,")
				.append(ImportTable.COLUMN_TYPE + " TEXT NOT NULL,")
				.append(ImportTable.COLUMN_ENABLE + " INTEGER ,")
				.append(ImportTable.COLUMN_LAST_IMPORT_TIME + " TEXT NOT NULL DEFAULT ''',")
				.append(ImportTable.COLUMN_ADDED_TIME + " TEXT NOT NULL);");
		createTable(db, sqlBuilder.toString(), ImportTable.TABLE_NAME);


		// Create ImportFiles table
		sqlBuilder = new StringBuilder();
		sqlBuilder.append("Create Table {0} (")
		  .append(ImportFilesTable.COLUMN_FILENAME + " TEXT PRIMARY KEY,")
		  .append(ImportFilesTable.COLUMN_SIZE+" TEXT NOT NULL,")
		  .append(ImportFilesTable.COLUMN_MIMETYPE+" TEXT NOT NULL,")		  
		  .append(ImportFilesTable.COLUMN_DATE+" TEXT NOT NULL,")
		  .append(ImportFilesTable.COLUMN_STATUS + " TEXT NOT NULL DEFAULT '0',")
		  .append(ImportFilesTable.COLUMN_FILETYPE + " TEXT NOT NULL ,")		  
		  .append(ImportFilesTable.COLUMN_FOLDER + " TEXT NOT NULL DEFAULT ''',")
	      .append(ImportFilesTable.COLUMN_IMAGE_ID + " TEXT NOT NULL DEFAULT '-1');");
		createTable(db, sqlBuilder.toString(), ImportFilesTable.TABLE_NAME);

		// Create ImportFiles table
		sqlBuilder = new StringBuilder();
		sqlBuilder.append("Create Table {0} (")
		  .append(ServersTable.COLUMN_SERVER_ID + " TEXT PRIMARY KEY,")
		  .append(ServersTable.COLUMN_SERVER_NAME+" TEXT NOT NULL DEFAULT ''' ,")
		  .append(ServersTable.COLUMN_STATUS+" TEXT NOT NULL,")
		  .append(ServersTable.COLUMN_START_DATETIME + " TEXT NOT NULL DEFAULT ''',")
		  .append(ServersTable.COLUMN_END_DATETIME + " TEXT NOT NULL DEFAULT ''',")		  
		  .append(ServersTable.COLUMN_FOLDER + " TEXT NOT NULL DEFAULT ''',")
	      .append(ServersTable.COLUMN_FREE_SPACE + " TEXT NOT NULL DEFAULT '0',")
	      .append(ServersTable.COLUMN_PHOTO_COUNT + " TEXT NOT NULL DEFAULT '0',")
	      .append(ServersTable.COLUMN_VIDEO_COUNT + " TEXT NOT NULL DEFAULT '0',")
	      .append(ServersTable.COLUMN_AUDIO_COUNT + " TEXT NOT NULL DEFAULT '0');");
		
		createTable(db, sqlBuilder.toString(), ServersTable.TABLE_NAME);
		
		//Create Indexes
		String sql = "CREATE INDEX "+INDEX_IMPORT_FILE_1+" on "+ImportFilesTable.TABLE_NAME+"("+ImportFilesTable.COLUMN_FILETYPE+");";
		db.execSQL(sql);
		Log.d(TAG, "CREATE INDEXES(1) FOR IMPORT FILE TABLE:"+sql);
	
	}


	@Override
	public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
		Log.i(TAG, "onUpgrade()");
		if (newVersion > oldVersion) {
			for (String tableName : TABLE_NAME_LIST) {
				db.execSQL("DROP TABLE IF EXISTS " + tableName);
				Log.d(TAG, "drop table "+tableName);
			}
			onCreate(db);
		}
	}
}