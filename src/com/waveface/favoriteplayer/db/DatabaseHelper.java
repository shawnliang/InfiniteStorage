package com.waveface.favoriteplayer.db;

import java.text.MessageFormat;

import android.content.Context;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;

import com.waveface.favoriteplayer.util.Log;

public class DatabaseHelper extends SQLiteOpenHelper {
	public static final String DATABASE_NAME = "favoriteplayer.db";

	private static final String TAG = DatabaseHelper.class.getSimpleName();
	private static final int DATABASE_VERSION = 1;
	private static final String[] TABLE_NAME_LIST = {
			BonjourServersTable.TABLE_NAME,
			PairedServersTable.TABLE_NAME};

	private interface Views {
		String VIEW_LABEL_FILE = LabelFileView.VIEW_NAME;
	}
	public DatabaseHelper(Context context) {
		super(context, DATABASE_NAME, null, DATABASE_VERSION);
		Log.d(TAG, "Finishing Constructing DatabaseHelper");
	}

	private void createTable(SQLiteDatabase db, String sql, String tableName) {
		String createSql = MessageFormat.format(sql, tableName);
		db.execSQL(createSql);
		Log.d(TAG, "In onCreate : SQL " + createSql);
	}

	@Override
	public void onCreate(SQLiteDatabase db) {
		Log.i(TAG, "onCreate!!!!");
		StringBuilder sqlBuilder = new StringBuilder();


		// Create Backuped Servers table
		sqlBuilder = new StringBuilder();
		sqlBuilder.append("Create Table {0} (")
		  .append(PairedServersTable.COLUMN_SERVER_ID + " TEXT PRIMARY KEY,")
		  .append(PairedServersTable.COLUMN_SERVER_NAME+" TEXT NOT NULL DEFAULT ''' ,")
		  .append(PairedServersTable.COLUMN_STATUS+" TEXT NOT NULL,")
		  .append(PairedServersTable.COLUMN_IP+" TEXT NOT NULL,")
		  .append(PairedServersTable.COLUMN_WS_PORT+" TEXT NOT NULL,")
		  .append(PairedServersTable.COLUMN_NOTIFY_PORT+" TEXT NOT NULL,")
		  .append(PairedServersTable.COLUMN_REST_PORT+ " TEXT );");
		createTable(db, sqlBuilder.toString(), PairedServersTable.TABLE_NAME);

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
		  .append(LabelTable.COLUMN_LABEL_NAME + " TEXT NOT NULL ,")
		  .append(LabelTable.COLUMN_SEQ + "  INT  ,")
		    .append(LabelTable.COLUMN_UPDATE_TIME + " TEXT NOT NULL ,")
		     .append(LabelTable.COLUMN_COVER_URL + " TEXT NOT NULL ,")
		  .append( LabelTable.COLUMN_AUTO_TYPE + " TEXT NOT NULL );");		
		createTable(db, sqlBuilder.toString(), LabelTable.TABLE_NAME);
		
		// Create Label Files table
		sqlBuilder = new StringBuilder();
		sqlBuilder.append("Create Table {0} (")
		  .append(LabelFileTable.COLUMN_LABEL_ID + " TEXT NOT NULL ,")
		  .append(LabelFileTable.COLUMN_FILE_ID+ " TEXT NOT NULL ,")
		  .append(LabelFileTable.COLUMN_ORDER + " TEXT NOT NULL ,")		
		  .append(" PRIMARY KEY ( "+LabelFileTable.COLUMN_LABEL_ID+","+LabelFileTable.COLUMN_FILE_ID+"));");
		createTable(db, sqlBuilder.toString(), LabelFileTable.TABLE_NAME);
		
		// Create Files table
		sqlBuilder = new StringBuilder();
		sqlBuilder.append("Create Table {0} (")
		  .append(FileTable.COLUMN_FILE_ID + " TEXT PRIMARY KEY,")
		  .append(FileTable.COLUMN_FILE_NAME+ " TEXT NOT NULL ,")
		  .append(FileTable.COLUMN_FOLDER+ " TEXT NOT NULL ,")
		  .append(FileTable.COLUMN_THUMB_READY+ " TEXT NOT NULL ,")
		  .append(FileTable.COLUMN_TYPE+ " TEXT NOT NULL ,")
		  .append(FileTable.COLUMN_DEV_ID+ " TEXT NOT NULL ,")
		  .append(FileTable.COLUMN_DEV_NAME+ " TEXT NOT NULL ,")
		  .append(FileTable.COLUMN_WIDTH+ " TEXT NOT NULL ,")
		  .append(FileTable.COLUMN_HEIGHT+ " TEXT NOT NULL ,")
		  .append(FileTable.COLUMN_DEV_TYPE + " TEXT NOT NULL );");		
		createTable(db, sqlBuilder.toString(), FileTable.TABLE_NAME);
		
		//CREATE VIEW FOR LABEL AND FILE RELETIVE
		createView(db);

	}

	@Override
	public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
		Log.i(TAG, "onUpgrade()");
		for (String tableName : TABLE_NAME_LIST) {
			db.execSQL("DROP TABLE IF EXISTS " + tableName);
			Log.d(TAG, "drop table "+tableName);
		}
		onCreate(db);
	}
	
	private void createView(SQLiteDatabase db) {
		StringBuilder sqlBuilder = new StringBuilder();
		String sql = null;
		sqlBuilder.append(" CREATE VIEW IF NOT EXISTS " + Views.VIEW_LABEL_FILE + " AS")
	      .append(" SELECT ")
	      .append(" A."+LabelFileTable.COLUMN_LABEL_ID+" AS "+LabelFileView.COLUMN_LABEL_ID+",")
	      .append(" A."+LabelFileTable.COLUMN_FILE_ID+" AS "+LabelFileView.COLUMN_FILE_ID+",")
	      .append(" A."+LabelFileTable.COLUMN_ORDER+" AS "+LabelFileView.COLUMN_ORDER+",")
	      .append(" B."+FileTable.COLUMN_FILE_NAME+" AS "+LabelFileView.COLUMN_FILE_NAME+",")
	      .append(" B."+FileTable.COLUMN_FOLDER+" AS "+LabelFileView.COLUMN_FOLDER+",")
	      .append(" B."+FileTable.COLUMN_THUMB_READY+" AS "+LabelFileView.COLUMN_THUMB_READY+",")
	      .append(" B."+FileTable.COLUMN_TYPE+" AS "+LabelFileView.COLUMN_TYPE+",")
	      .append(" B."+FileTable.COLUMN_DEV_ID+" AS "+LabelFileView.COLUMN_DEV_ID+",")
	      .append(" B."+FileTable.COLUMN_DEV_NAME+" AS "+LabelFileView.COLUMN_DEV_NAME+",")
	      .append(" B."+FileTable.COLUMN_HEIGHT+" AS "+LabelFileView.COLUMN_HEIGHT+",")
	      .append(" B."+FileTable.COLUMN_WIDTH+" AS "+LabelFileView.COLUMN_WIDTH+",")
		  .append(" B."+FileTable.COLUMN_DEV_TYPE+" AS "+LabelFileView.COLUMN_DEV_TYPE)
	      .append(" FROM "+LabelFileTable.TABLE_NAME+" A,"+FileTable.TABLE_NAME+" B")
	      .append(" WHERE A."+LabelFileTable.COLUMN_FILE_ID+"= B."+FileTable.COLUMN_FILE_ID);

		sql = sqlBuilder.toString();
		db.execSQL(sql);
		Log.d(TAG, "CREATE VIEW FOR LABEL FILE  RELATIVE TABLE:"+sql);

	}
	
}
