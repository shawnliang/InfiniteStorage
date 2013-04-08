package com.waveface.sync;

import java.util.Date;

import android.content.Context;

import com.google.gson.Gson;

public class RuntimePlayer {
	private static final String TAG = RuntimePlayer.class.getSimpleName();
	private static RuntimePlayer instance;

	public static Context mContext;
	public static boolean IsLogined = false;
	public static boolean IsLogining = false; // should combine with is logined
	public static boolean OnStation = false;
	public static boolean OnWebSocketStation = false;
	public static boolean OnWebSocketOpened = false;
	public static boolean OnFirstUseLinkStation = false;
	public static boolean FilterMode = false;
	public static boolean FindStationMode = false;
	public static boolean UploadImageQueueBackground = false;
	public static boolean UploadOriginal = false;
	public static boolean UploadDraftPost = false;
	public static boolean UploadChangedPost = false;
	public static boolean DownloadMoreBeckend = false;
	public static boolean DownloadImageBeckend = false;
	public static boolean DownloadImageMetaBeckend = false;
	public static boolean DownloadImageMetaOnDemand = false;
	public static boolean ChangeLogsBeckend = false;
	public static boolean CheckImageCache = false;
	public static boolean isImportProcessing = false;
	public static boolean isDeletingPhoto = false;
	public static boolean isRemovingPhotoFromEvent = false;
	public static boolean NewerImportProcessing = false;
	public static boolean OlderImportProcessing = false;
	public static boolean KeepOnDeviceDownloading = false;
	public static boolean LocationImporting = false;
	public static boolean NotificationForUploadThumbShow = false;
	public static boolean SummaryRefreshing = false;


	public static boolean HasGap = false;
	public static Gson GSON = new Gson();
	public static boolean OutOfMemoryOccurs = false;
	public static boolean switchMode = false;
	public static boolean previewPostPosted = false;
	public static boolean reservingIds = false;
	public static String EDITTING_POST_ID ;
	public static String UPDATING_POST_ID ;
    public static int LAST_TOUCH_SHOEBOX;
    public static Date DATA_CURRENT_DATE = null;

	public static int FilterType = -1;

	public static RuntimePlayer getInstance() {
		if (instance == null) {
			instance = new RuntimePlayer();
		}
		return instance;
	}


}
