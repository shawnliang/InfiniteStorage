package com.waveface.favoriteplayer.util;

import java.io.BufferedInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.nio.channels.FileChannel;
import java.util.Date;
import java.util.Stack;

import android.content.Context;
import android.content.SharedPreferences;
import android.text.TextUtils;

import com.waveface.favoriteplayer.Constant;

public class FileUtil {
	private static final String TAG = FileUtil.class.getSimpleName();

	public static void fileCopy( String in, String out )
            throws IOException
    {
		fileCopy(new File(in), new File(out));
    }

	public static void fileCopy( File in, File out )
            throws IOException
    {
        FileChannel inChannel = new FileInputStream( in ).getChannel();
        FileChannel outChannel = new FileOutputStream( out ).getChannel();
        try
        {
            // magic number for Windows, 64Mb - 32Kb)
            int maxCount = (64 * 1024 * 1024) - (32 * 1024);
            long size = inChannel.size();
            long position = 0;
            while ( position < size )
            {
               position += inChannel.transferTo( position, maxCount, outChannel );
            }
        }
        finally
        {
            if ( inChannel != null )
            {
               inChannel.close();
            }
            if ( outChannel != null )
            {
                outChannel.close();
            }
        }
    }
	public static String getFileCreateTime(String filename){
		File file = new File(filename);
		Date lastModified = null;
		if (file.exists()) {
		  lastModified = new Date(file.lastModified());
		}
		if( lastModified==null){
			return null;
		}
		else{
			return StringUtil.changeToLocalString(StringUtil.formatDate(lastModified));
		}
	}

	public static long getDirectorySize(File directory) {
	    long result = 0;
	    Stack<File> dirlist= new Stack<File>();
	    dirlist.clear();
	    dirlist.push(directory);
	    while(!dirlist.isEmpty())
	    {
	        File dirCurrent = dirlist.pop();

	        File[] fileList = dirCurrent.listFiles();
	        for (int i = 0; i < fileList.length; i++) {

	            if(fileList[i].isDirectory())
	                dirlist.push(fileList[i]);
	            else
	                result += fileList[i].length();
	        }
	    }
	    return result;
	}
	public static boolean isFileExisted(String filePath) {
		if(TextUtils.isEmpty(filePath))
			return false;
		return new File(filePath).exists();
	}
	public static boolean downloadFile(InputStream is, String filename) {
		boolean downloaded = false;
		byte[] data = new byte[1024 * 10];
		int count;
		try {
			File f = new File(filename);
			FileOutputStream fos = new FileOutputStream(f);
			long startTime = System.currentTimeMillis();
			BufferedInputStream bis = new BufferedInputStream(is);
			while ((count = bis.read(data)) != -1) {
				fos.write(data, 0, count);
			}
			fos.flush();
			fos.close();
			bis.close();
			Log.d(TAG, "download ready in"
					+ ((System.currentTimeMillis() - startTime) / 1000)
					+ " sec");
			if (f.exists()) {
				downloaded = true;
			}
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		} catch (Exception e) {
			e.printStackTrace();
		}
		return downloaded;
	}
	public static String getDownloadFolder(Context context){
		return context.getSharedPreferences(Constant.PREFS_NAME, Context.MODE_PRIVATE)
				.getString(Constant.PREF_DOWNLOAD_FOLDER, "");
	}
}
