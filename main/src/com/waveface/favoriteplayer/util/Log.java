package com.waveface.favoriteplayer.util;


public class Log {
	static final boolean LOG = false;

	public static void i(String tag, String string) {
		if (LOG)
			android.util.Log.i(tag, string);
	}

	public static void i(String tag, String string, Exception e) {
		if (LOG)
			android.util.Log.i(tag, string, e);
	}

	public static void e(String tag, String string) {
		if (LOG) {
			if (string != null)
				android.util.Log.e(tag, string);
		}
	}

	public static void e(String tag, String string, Exception e) {
		if (LOG) {
			if (string != null && e != null)
				android.util.Log.e(tag, string, e);
		}
	}

	public static void d(String tag, String string) {
		if (LOG)
			android.util.Log.d(tag, string);
	}

	public static void d(String tag, String string, Exception e) {
		if (LOG)
			android.util.Log.d(tag, string, e);
	}

	public static void v(String tag, String string) {
		if (LOG)
			android.util.Log.v(tag, string);
	}

	public static void v(String tag, String string, Exception e) {
		if (LOG)
			android.util.Log.v(tag, string, e);
	}

	public static void w(String tag, String string) {
		if (LOG)
			android.util.Log.w(tag, string);
	}

	public static void w(String tag, String string, Exception e) {
		if (LOG)
			android.util.Log.w(tag, string, e);
	}
}
