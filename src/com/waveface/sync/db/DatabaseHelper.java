package com.waveface.sync.db;

import java.text.MessageFormat;

import android.content.Context;
import android.content.pm.ApplicationInfo;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;

import com.waveface.sync.Constant;
import com.waveface.sync.util.Log;

public class DatabaseHelper extends SQLiteOpenHelper {
	public static final String DATABASE_NAME = "infinites.db";

	private static final String TAG = DatabaseHelper.class.getSimpleName();
	private static final int DATABASE_VERSION = 8;
	private static final String[] TABLE_NAME_LIST = {
			BonjourServersTable.TABLE_NAME,
			BackupedServersTable.TABLE_NAME};


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


		// Create Backuped Servers table
		sqlBuilder = new StringBuilder();
		sqlBuilder.append("Create Table {0} (")
		  .append(BackupedServersTable.COLUMN_SERVER_ID + " TEXT PRIMARY KEY,")
		  .append(BackupedServersTable.COLUMN_SERVER_NAME+" TEXT NOT NULL DEFAULT ''' ,")
		  .append(BackupedServersTable.COLUMN_STATUS+" TEXT NOT NULL,")
		  .append(BackupedServersTable.COLUMN_IP+" TEXT NOT NULL,")
		  .append(BackupedServersTable.COLUMN_WS_PORT+" TEXT NOT NULL,")
		  .append(BackupedServersTable.COLUMN_NOTIFY_PORT+" TEXT NOT NULL,")
		  .append(BackupedServersTable.COLUMN_REST_PORT+ " TEXT );");
		createTable(db, sqlBuilder.toString(), BackupedServersTable.TABLE_NAME);

		// Create BonjourServers table
		sqlBuilder = new StringBuilder();
		sqlBuilder.append("Create Table {0} (")
		  .append(BonjourServersTable.COLUMN_SERVER_ID + " TEXT PRIMARY KEY,")
		  .append(BonjourServersTable.COLUMN_SERVER_NAME+" TEXT NOT NULL DEFAULT ''' ,")
		  .append(BonjourServersTable.COLUMN_IP+" TEXT NOT NULL,")		  
		  .append(BonjourServersTable.COLUMN_WS_PORT+" TEXT NOT NULL,")
		  .append(BonjourServersTable.COLUMN_NOTIFY_PORT + " TEXT NOT NULL,")		
		  .append(BonjourServersTable.COLUMN_REST_PORT + " TEXT NOT NULL );");
		createTable(db, sqlBuilder.toString(), BonjourServersTable.TABLE_NAME);
		
		// Create Labels table
		sqlBuilder = new StringBuilder();
		sqlBuilder.append("Create Table {0} (")
		  .append(LabelTable.COLUMN_LABEL_ID + " TEXT PRIMARY KEY,")
		  .append(LabelTable.COLUMN_LABEL_NAME + " TEXT NOT NULL );");		
		createTable(db, sqlBuilder.toString(), LabelTable.TABLE_NAME);
		
		// Create Label Files table
		sqlBuilder = new StringBuilder();
		sqlBuilder.append("Create Table {0} (")
		  .append(LabelFileTable.COLUMN_LABEL_ID + " TEXT PRIMARY KEY,")
		  .append(LabelFileTable.COLUMN_FILE_ID + " TEXT NOT NULL );");		
		createTable(db, sqlBuilder.toString(), LabelFileTable.TABLE_NAME);

	}

	@Override
	public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
		Log.i(TAG, "onUpgrade()");
//		if (newVersion > oldVersion) {
		for (String tableName : TABLE_NAME_LIST) {
			db.execSQL("DROP TABLE IF EXISTS " + tableName);
			Log.d(TAG, "drop table "+tableName);
		}
		onCreate(db);
//		}
	}
}