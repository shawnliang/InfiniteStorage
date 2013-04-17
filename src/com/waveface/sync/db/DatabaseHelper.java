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
	private static final int DATABASE_VERSION = 1;
	private static final String[] TABLE_NAME_LIST = {
			ImportTable.TABLE_NAME,
			ImportFilesTable.TABLE_NAME,
			BackupedServersTable.TABLE_NAME,
			BackupDetailsTable.TABLE_NAME};

	private interface Views {
		String VIEW_SERVER_FILES = "serverFilesView";
	}

	private static final String INDEX_IMPORT_FILE_1 = "IDX_"+ImportFilesTable.TABLE_NAME+"_1";
	private static final String INDEX_BACKUP_DETAILS_1 = "IDX_"+BackupDetailsTable.TABLE_NAME+"_1";

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

		// Create Backuped Servers table
		sqlBuilder = new StringBuilder();
		sqlBuilder.append("Create Table {0} (")
		  .append(BackupedServersTable.COLUMN_SERVER_ID + " TEXT PRIMARY KEY,")
		  .append(BackupedServersTable.COLUMN_SERVER_NAME+" TEXT NOT NULL DEFAULT ''' ,")
		  .append(BackupedServersTable.COLUMN_STATUS+" TEXT NOT NULL,")
		  .append(BackupedServersTable.COLUMN_START_DATETIME + " TEXT NOT NULL DEFAULT ''',")
		  .append(BackupedServersTable.COLUMN_END_DATETIME + " TEXT NOT NULL DEFAULT ''',")		  
		  .append(BackupedServersTable.COLUMN_FOLDER + " TEXT NOT NULL DEFAULT ''',")
	      .append(BackupedServersTable.COLUMN_FREE_SPACE + " TEXT NOT NULL DEFAULT '0',")
	      .append(BackupedServersTable.COLUMN_PHOTO_COUNT + " TEXT NOT NULL DEFAULT '0',")
	      .append(BackupedServersTable.COLUMN_VIDEO_COUNT + " TEXT NOT NULL DEFAULT '0',")
	      .append(BackupedServersTable.COLUMN_AUDIO_COUNT + " TEXT NOT NULL DEFAULT '0',")		
	      .append(BackupedServersTable.COLUMN_LAST_BACKUP_DATETIME+ " TEXT NOT NULL);");
		createTable(db, sqlBuilder.toString(), BackupedServersTable.TABLE_NAME);

		// Create BonjourServers table
		sqlBuilder = new StringBuilder();
		sqlBuilder.append("Create Table {0} (")
		  .append(BonjourServersTable.COLUMN_SERVER_ID + " TEXT PRIMARY KEY,")
		  .append(BonjourServersTable.COLUMN_SERVER_NAME+" TEXT NOT NULL DEFAULT ''' ,")
		  .append(BonjourServersTable.COLUMN_SERVER_OS+" TEXT NOT NULL,")
		  .append(BonjourServersTable.COLUMN_WS_LOCATION + " TEXT NOT NULL );");		
		createTable(db, sqlBuilder.toString(), BonjourServersTable.TABLE_NAME);

		// Create Servers table
		sqlBuilder = new StringBuilder();
		sqlBuilder.append("Create Table {0} (")
		  .append(BackupDetailsTable.COLUMN_SERVER_ID + " TEXT ,")
		  .append(BackupDetailsTable.COLUMN_FILENAME+" TEXT NOT NULL,")
		  .append(BackupDetailsTable.COLUMN_STATUS+" TEXT NOT NULL,")		
		  .append("PRIMARY KEY ( "+BackupDetailsTable.COLUMN_SERVER_ID+","+BackupDetailsTable.COLUMN_FILENAME+"));");

		createTable(db, sqlBuilder.toString(), BackupDetailsTable.TABLE_NAME);

		//Create Indexes
		String sql = "CREATE INDEX "+INDEX_IMPORT_FILE_1+" on "+ImportFilesTable.TABLE_NAME+"("+ImportFilesTable.COLUMN_FILETYPE+");";
		db.execSQL(sql);
		Log.d(TAG, "CREATE INDEXES(1) FOR IMPORT FILE TABLE:"+sql);
		
		sql = "CREATE INDEX "+INDEX_BACKUP_DETAILS_1+" on "+BackupDetailsTable.TABLE_NAME+"("+BackupDetailsTable.COLUMN_STATUS+");";
		db.execSQL(sql);
		Log.d(TAG, "CREATE INDEXES(1) FOR BACKUP DETAILS TABLE:"+sql);
	
		createView(db);
	}
	private void createView(SQLiteDatabase db) {
		StringBuilder sqlBuilder = new StringBuilder();
		String sql = null;
		sqlBuilder.append(" CREATE VIEW IF NOT EXISTS " + Views.VIEW_SERVER_FILES + " AS")
	      .append(" SELECT ")
	      .append(" B."+BackupDetailsTable.COLUMN_SERVER_ID+" AS "+ServerFilesView.COLUMN_SERVER_ID+",")	      
	      .append(" A."+ImportFilesTable.COLUMN_FILENAME+" AS "+ServerFilesView.COLUMN_FILENAME+",")
	      .append(" A."+ImportFilesTable.COLUMN_MIMETYPE+" AS "+ServerFilesView.COLUMN_MIMETYPE+",")
	      .append(" A."+ImportFilesTable.COLUMN_SIZE+" AS "+ServerFilesView.COLUMN_SIZE+",")
	      .append(" A."+ImportFilesTable.COLUMN_FOLDER+" AS "+ServerFilesView.COLUMN_FOLDER+",")
	      .append(" A."+ImportFilesTable.COLUMN_DATE+" AS "+ServerFilesView.COLUMN_DATE)	      
	      .append(" FROM "+ImportFilesTable.TABLE_NAME+" A,"+BackupDetailsTable.TABLE_NAME+" B")
	      .append(" WHERE A."+ImportFilesTable.COLUMN_FILENAME+"=B."+BackupDetailsTable.COLUMN_FILENAME)
	      .append(" AND B."+BackupDetailsTable.COLUMN_STATUS+"!='"+Constant.IMPORT_FILE_INCLUDED+"'");

		sql = sqlBuilder.toString();
		db.execSQL(sql);
		Log.d(TAG, "CREATE VIEW FOR SERVER_FILES IMAGE  RELATIVE TABLE:"+sql);

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