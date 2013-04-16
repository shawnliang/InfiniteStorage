package com.waveface.sync.task;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.Arrays;

import org.jwebsocket.kit.WebSocketException;

import android.content.Context;
import android.database.Cursor;
import android.os.AsyncTask;
import android.provider.MediaStore;
import android.text.TextUtils;

import com.waveface.sync.Constant;
import com.waveface.sync.RuntimeConfig;
import com.waveface.sync.entity.FIleTransferEntity;
import com.waveface.sync.util.Log;
import com.waveface.sync.util.StringUtil;
import com.waveface.sync.websocket.RuntimeWebClient;

public class BackupTask extends AsyncTask<Void, Void, Void> {
	private String TAG = BackupTask.class.getSimpleName();
	private final int BASE_BYTES = 1024;
	private static ArrayList<String> mFolders = new ArrayList<String>();
	private static ArrayList<String> mFilenames = new ArrayList<String>();
	private static ArrayList<String> mFilesizes = new ArrayList<String>();
	private Context mContext;

	public BackupTask(Context context) {
		mContext = context;
	}

	@Override
	protected Void doInBackground(Void... params) {
		String foldername = null;
		String filename = null;
		String filesize = null;
		sendFile(mContext, Constant.TYPE_AUDIO);
		sendFile(mContext, Constant.TYPE_VIDEO);
		sendFile(mContext, Constant.TYPE_IMAGE);

		try {
			for (int i = 0; i < mFilenames.size(); i++) {
				foldername = mFolders.get(i);
				filename = mFilenames.get(i);
				filesize = mFilesizes.get(i);

				// send file index for start
				FIleTransferEntity entity = new FIleTransferEntity();
				entity.action = Constant.WS_ACTION_FILE_START;
				entity.folder = foldername;
				entity.fileName = StringUtil.getFilename(filename);
				entity.fileSize = filesize;
				String jsonOuput = RuntimeConfig.GSON.toJson(entity);
				RuntimeWebClient.setDefaultFormat();
				RuntimeWebClient.send(jsonOuput);
				try {
					Thread.sleep(1000);
				} catch (InterruptedException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				// send file binary

				InputStream ios = null;
				RuntimeWebClient.setBinaryFormat();
				try {
					byte[] buffer = new byte[64 * BASE_BYTES];
					byte[] finalBuffer = null;
					ios = new FileInputStream(new File(filename));
					int read = 0;
					while ((read = ios.read(buffer)) != -1) {
						if (read != buffer.length) {
							finalBuffer = new byte[read];
							finalBuffer = Arrays.copyOf(buffer, read);
							RuntimeWebClient.sendFile(finalBuffer);
						} else {
							RuntimeWebClient.sendFile(buffer);
						}
					}
				} catch (FileNotFoundException e) {
					e.printStackTrace();
				} catch (IOException e) {
					e.printStackTrace();
				} finally {
					try {
						if (ios != null)
							ios.close();
					} catch (IOException e) {
					}
				}
				// send file index for end
				RuntimeWebClient.setDefaultFormat();
				entity.action = Constant.WS_ACTION_FILE_END;
				jsonOuput = RuntimeConfig.GSON.toJson(entity);
				RuntimeWebClient.send(jsonOuput);
			}

		} catch (WebSocketException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return null;
	}

	public void sendFile(Context context, int type) {
		String currentDate = "";
		String cursorDate = "";
		long refCursorDate = 0;
		long dateTaken = -1;
		long dateModified = 0;
		long dateAdded = 0;
		String fileSize = null;
		String folderName = null;
		String mediaData = null;
		String[] projection = null;
		String selection = null;
		String selectionArgs[] = { currentDate };
		Cursor cursor = null;
		if (type == Constant.TYPE_IMAGE) {// IMAGES
			projection = new String[] { MediaStore.Images.Media.DATA,
					MediaStore.Images.Media.DATE_TAKEN,
					MediaStore.Images.Media.DISPLAY_NAME,
					MediaStore.Images.Media.DATE_ADDED,
					MediaStore.Images.Media.DATE_MODIFIED,
					MediaStore.Images.Media.SIZE, MediaStore.Images.Media._ID };
			selection = getImageSelection(context);

			Log.d(TAG, "selection => " + selection);
			cursor = context.getContentResolver().query(
					MediaStore.Images.Media.EXTERNAL_CONTENT_URI, projection,
					selection, selectionArgs,
					MediaStore.Images.Media.DATE_TAKEN + " DESC LIMIT 1");
		} else if (type == Constant.TYPE_VIDEO) {// VIDEO
			projection = new String[] { MediaStore.Video.Media.DATA,
					MediaStore.Video.Media.DATE_TAKEN,
					MediaStore.Video.Media.DISPLAY_NAME,
					MediaStore.Video.Media.DATE_ADDED,
					MediaStore.Video.Media.DATE_MODIFIED,
					MediaStore.Video.Media.SIZE, MediaStore.Video.Media._ID };

			selection = getVideoSelection(context);
			Log.d(TAG, "selection => " + selection);
			cursor = context.getContentResolver().query(
					MediaStore.Video.Media.EXTERNAL_CONTENT_URI, projection,
					selection, selectionArgs,
					MediaStore.Video.Media.DATE_TAKEN + " DESC LIMIT 1");
		} else if (type == Constant.TYPE_AUDIO) {// AUDIO
			projection = new String[] { MediaStore.Audio.Media.DATA,
					MediaStore.Audio.Media.DISPLAY_NAME,
					MediaStore.Audio.Media.ALBUM,
					MediaStore.Audio.Media.DATE_ADDED,
					MediaStore.Audio.Media.DATE_MODIFIED,
					MediaStore.Audio.Media.SIZE, MediaStore.Audio.Media._ID };

			selection = getAudioSelection(context);

			Log.d(TAG, "selection => " + selection);
			cursor = context.getContentResolver().query(
					MediaStore.Audio.Media.EXTERNAL_CONTENT_URI, projection,
					selection, selectionArgs,
					MediaStore.Audio.Media.DATE_ADDED + " DESC LIMIT 1");
		}
		if (cursor != null && cursor.getCount() > 0) {
			cursor.moveToFirst();
			int count = cursor.getCount();
			for (int i = 0; i < count; i++) {
				if (type == Constant.TYPE_IMAGE) {
					mediaData = cursor.getString(cursor
							.getColumnIndex(MediaStore.Images.Media.DATA));
					dateTaken = cursor
							.getLong(cursor
									.getColumnIndex(MediaStore.Images.Media.DATE_TAKEN));
					dateModified = cursor
							.getLong(cursor
									.getColumnIndex(MediaStore.Images.Media.DATE_MODIFIED));
					dateAdded = cursor
							.getLong(cursor
									.getColumnIndex(MediaStore.Images.Media.DATE_ADDED));
				} else if (type == Constant.TYPE_AUDIO) {
					mediaData = cursor.getString(cursor
							.getColumnIndex(MediaStore.Audio.Media.DATA));
					dateModified = cursor
							.getLong(cursor
									.getColumnIndex(MediaStore.Audio.Media.DATE_MODIFIED));
					dateAdded = cursor.getLong(cursor
							.getColumnIndex(MediaStore.Audio.Media.DATE_ADDED));
				} else if (type == Constant.TYPE_VIDEO) {
					mediaData = cursor.getString(cursor
							.getColumnIndex(MediaStore.Video.Media.DATA));
					dateTaken = cursor.getLong(cursor
							.getColumnIndex(MediaStore.Video.Media.DATE_TAKEN));
					dateModified = cursor
							.getLong(cursor
									.getColumnIndex(MediaStore.Video.Media.DATE_MODIFIED));
					dateAdded = cursor.getLong(cursor
							.getColumnIndex(MediaStore.Video.Media.DATE_ADDED));
				}
				fileSize = cursor.getString(5);
				cursor.getString(6);
				folderName = getFoldername(mediaData);

				if (dateTaken != -1) {
					refCursorDate = dateTaken / 1000;
				} else if (dateModified != -1) {
					refCursorDate = dateModified;
				} else if (dateAdded != -1) {
					refCursorDate = dateAdded;
				}
				cursorDate = StringUtil.getConverDate(refCursorDate);
				Log.d(TAG, "cursorDate ==>" + cursorDate);
				Log.d(TAG, "Filename ==>" + mediaData);
				mFolders.add(folderName);
				mFilenames.add(mediaData);
				mFilesizes.add(fileSize);

				cursor.moveToNext();
			}
			cursor.close();
		}
	}

	public static String getFoldername(String imageFullpath) {
		if (!TextUtils.isEmpty(imageFullpath)) {
			int lastIndex = imageFullpath.lastIndexOf(File.separator);
			int lastSecondIndex = imageFullpath.substring(0, lastIndex)
					.lastIndexOf(File.separator);
			return imageFullpath.substring(lastSecondIndex + 1, lastIndex);
		} else {
			return "";
		}
	}

	public static String getImageSelection(Context context) {
		String selection = "(" + MediaStore.Images.Media.DATE_ADDED + " <= ? ";
		// IMPORT ALL
		selection += " AND " + MediaStore.Images.Media.DISPLAY_NAME
				+ " NOT LIKE '%.jps%' )";
		return selection;
	}

	public static String getVideoSelection(Context context) {
		String selection = MediaStore.Video.Media.DATE_ADDED + " <= ? ";
		// IMPORT ALL
		// selection += " AND "+ MediaStore.Video.Media.DISPLAY_NAME
		// +" NOT LIKE '%.jps%' )";
		return selection;
	}

	public static String getAudioSelection(Context context) {
		String selection = MediaStore.Audio.Media.DATE_ADDED + " <= ? ";
		// IMPORT ALL
		// selection += " AND "+ MediaStore.Video.Media.DISPLAY_NAME
		// +" NOT LIKE '%.jps%' )";
		return selection;
	}

}
