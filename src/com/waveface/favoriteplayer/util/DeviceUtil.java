package com.waveface.favoriteplayer.util;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.RandomAccessFile;
import java.util.LinkedList;
import java.util.List;
import java.util.UUID;

import android.accounts.Account;
import android.accounts.AccountManager;
import android.content.Context;
import android.content.SharedPreferences;
import android.os.Environment;
import android.text.TextUtils;

import com.waveface.favoriteplayer.Constant;

public class DeviceUtil {
	private static String sID = null;
	private static final String INSTALLATION = "DEVICE_INFO";

	public synchronized static String id(Context context) {
		if (sID == null) {
			File appDirectory = new File(
					Environment.getExternalStorageDirectory() + "/"
							+ Constant.APP_FOLDER);
			if (!appDirectory.exists())
				appDirectory.mkdir();
			File installation = new File(appDirectory, INSTALLATION);
			try {
				if (!installation.exists())
					writeInstallationFile(installation);
				sID = readInstallationFile(installation);
			} catch (Exception e) {
				throw new RuntimeException(e);
			}
		}
		return sID;
	}

	private static String readInstallationFile(File installation)
			throws IOException {
		RandomAccessFile f = new RandomAccessFile(installation, "r");
		byte[] bytes = new byte[(int) f.length()];
		f.readFully(bytes);
		f.close();
		return new String(bytes);
	}

	private static void writeInstallationFile(File installation)
			throws IOException {
		FileOutputStream out = new FileOutputStream(installation);
		String id = UUID.randomUUID().toString();
		out.write(id.getBytes());
		out.close();
	}

	public static String getUsername(Context context) {
		AccountManager manager = AccountManager.get(context);
		Account[] accounts = manager.getAccountsByType("com.google");
		List<String> possibleEmails = new LinkedList<String>();

		for (Account account : accounts) {
			possibleEmails.add(account.name);
		}

		if (!possibleEmails.isEmpty() && possibleEmails.get(0) != null) {
			String email = possibleEmails.get(0);
			String[] parts = email.split("@");
			if (parts.length > 0 && parts[0] != null)
				return parts[0];
			else
				return null;
		} else
			return null;
	}
	public static String getEmailAccount(Context context) {
		String email = "";
		AccountManager manager = AccountManager.get(context);
		Account[] accounts = manager.getAccountsByType("com.google");
		List<String> possibleEmails = new LinkedList<String>();

		for (Account account : accounts) {
			possibleEmails.add(account.name);
		}

		if (!possibleEmails.isEmpty() && possibleEmails.get(0) != null) {
			email = possibleEmails.get(0);
		}
		return email;
	}


	public static String getDeviceName() {
		return android.os.Build.MODEL;
	}
	public static int getApiLevel() {
		return android.os.Build.VERSION.SDK_INT;
	}
	
	public static String getDeviceNameForDisplay(Context context){
    	SharedPreferences prefs = context.getSharedPreferences(Constant.PREFS_NAME, Context.MODE_PRIVATE);
    	String value =prefs.getString(Constant.PREF_DISPLAY_DEVICE_NAME, "");
    	if(TextUtils.isEmpty(value)){
			value = DeviceUtil.getEmailAccount(context);
			value = value.split("@")[0];
			value += "-"+DeviceUtil.getDeviceName();
			prefs.edit().putString(Constant.PREF_DISPLAY_DEVICE_NAME, value).commit();
    	}
		return value;

	}
	
}
