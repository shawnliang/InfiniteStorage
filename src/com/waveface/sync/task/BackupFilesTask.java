package com.waveface.sync.task;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.util.Arrays;

import org.jwebsocket.kit.WebSocketException;

import android.content.ContentResolver;
import android.content.Context;
import android.content.SharedPreferences;
import android.database.Cursor;
import android.os.AsyncTask;

import com.waveface.sync.Constant;
import com.waveface.sync.RuntimeConfig;
import com.waveface.sync.db.BackupedServersTable;
import com.waveface.sync.db.ImportFilesTable;
import com.waveface.sync.entity.FIleTransferEntity;
import com.waveface.sync.logic.ServersLogic;
import com.waveface.sync.util.StringUtil;
import com.waveface.sync.websocket.RuntimeWebClient;

public class BackupFilesTask extends AsyncTask<Void, Void, Void> {
	private static final String TAG = BackupFilesTask.class.getSimpleName();

	private Context mContext;

	public BackupFilesTask(Context context) {
		mContext = context;
	}
	@Override
	protected Void doInBackground(Void... params) {
		FIleTransferEntity entity = new FIleTransferEntity();
		byte[] buffer = new byte[256 * Constant.K_BYTES];
		byte[] finalBuffer = null;		
		InputStream ios = null;
		boolean isSuccesed = false;
		String filename = null;
    	SharedPreferences prefs = mContext.getSharedPreferences(Constant.PREFS_NAME, Context.MODE_PRIVATE);    	
    	String serverId = prefs.getString(Constant.PREF_SERVER_ID, "");
    	String lastBackupTimestamp = null;
    	//select from serverFiles 
		Cursor cursor = null;
		ContentResolver cr = mContext.getContentResolver();
		cursor = cr.query(BackupedServersTable.CONTENT_URI, 
				new String[]{
				BackupedServersTable.COLUMN_END_DATETIME,
				}, 
				BackupedServersTable.COLUMN_SERVER_ID+"=?", 
				new String[]{serverId},null);	
		if(cursor!=null && cursor.getCount()>0){
			cursor.moveToFirst();
			lastBackupTimestamp = cursor.getString(0); 
		}	
		
		cursor = cr.query(ImportFilesTable.CONTENT_URI, 
				new String[]{
				ImportFilesTable.COLUMN_FILENAME,
				ImportFilesTable.COLUMN_MIMETYPE,
				ImportFilesTable.COLUMN_SIZE,
				ImportFilesTable.COLUMN_FOLDER,
				ImportFilesTable.COLUMN_DATE}, 
				ImportFilesTable.COLUMN_DATE+">=?", 
				new String[]{lastBackupTimestamp}, 
				ImportFilesTable.COLUMN_DATE);	
		if(cursor!=null && cursor.getCount()>0){
			cursor.moveToFirst();
			int count = cursor.getCount();
			for(int i = 0 ; i<count ;i++){
				// send file index for start
				entity.action = Constant.WS_ACTION_FILE_START;
				filename = cursor.getString(0);
				entity.fileName = StringUtil.getFilename(filename);
				entity.mimetype = cursor.getString(1);
				entity.fileSize = cursor.getString(2);
				entity.folder = cursor.getString(3);				
				entity.datetime = cursor.getString(4);
			
				try {
					if(RuntimeConfig.OnWebSocketOpened){
//						RuntimeWebClient.setDefaultFormat();
						RuntimeWebClient.send(RuntimeConfig.GSON.toJson(entity));
						//TODO:wait for dedup information
					}
					else{
						isSuccesed = true;
						break;
					}
					// send file binary
					ios = new FileInputStream(new File(filename));
					int read = 0;
					while ((read = ios.read(buffer)) != -1) {
						if(RuntimeConfig.OnWebSocketOpened){
							if (read != buffer.length) {
								finalBuffer = new byte[read];
								finalBuffer = Arrays.copyOf(buffer, read);
								RuntimeWebClient.sendFile(finalBuffer);
							} else {
								RuntimeWebClient.sendFile(buffer);
							}
						}
						else{
							isSuccesed = false;
							break;
						}
					}
					// send file index for end
					if(RuntimeConfig.OnWebSocketOpened){
//						RuntimeWebClient.setDefaultFormat();
						entity.action = Constant.WS_ACTION_FILE_END;
						RuntimeWebClient.send(RuntimeConfig.GSON.toJson(entity));
					}else{
						isSuccesed = false;
						break;						
					}
					isSuccesed = true;
				} catch (FileNotFoundException e) {
					e.printStackTrace();
				} catch (IOException e) {
					e.printStackTrace();
				} catch (WebSocketException e) {
					e.printStackTrace();
				} finally {
					try {
						if (ios != null)
							ios.close();
					} catch (IOException e) {
					}
					if(isSuccesed){
						ServersLogic.updateServerLastBackupTimestamp(mContext, entity.datetime, serverId);
//						FileBackup.updateBackupStatus(mContext, entity.fileName, Constant.IMPORT_FILE_BACKUPED);
					}
					isSuccesed = false;
				}
				cursor.moveToNext();
			}
		}
		return null;
	}
	@Override
	protected void onPostExecute(Void entity) {
		RuntimeConfig.isBackuping = false;
		super.onPostExecute(entity);
	}
	@Override
	protected void onPreExecute() {
		super.onPreExecute();
	}

}
