package com.waveface.sync.util;

import java.util.LinkedList;
import java.util.List;
import android.accounts.Account;
import android.accounts.AccountManager;
import android.content.Context;

public class DeviceUtil {

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
}
