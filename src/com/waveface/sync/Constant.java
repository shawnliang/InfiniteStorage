package com.waveface.sync;

public class Constant {

	//DEVICE TYPE
	public static boolean PHONE = true;

	public final static String APP_FOLDER = "/InfiniteStorage";
	
	public final static String ACTION_NETWORK_STATE_CHANGE = "com.waveface.sync.action.NETWORK_STATE_CHANGE";	
	public final static String ACTION_WS_SERVER_NOTIFY = "com.waveface.sync.action.WS_SERVER_NOTIFY";	

	public final static String ACTION_BONJOUR_SERVER_MANUAL_PAIRING = "com.waveface.sync.action.BONJOUR_SERVER_MANUAL_PAIRING";	
	public final static String ACTION_BONJOUR_SERVER_AUTO_PAIRING = "com.waveface.sync.action.BONJOUR_SERVER_AUTO_PAIRING";	
	
	public final static String ACTION_BONJOUR_MULTICAT_EVENT = "com.waveface.sync.action.BONJOUR_MULTICAT_EVENT";	
	public final static String ACTION_BACKUP_FILE = "com.waveface.sync.action.BACKUP_FILE";
	public final static String ACTION_BACKUP_DONE = "com.waveface.sync.action.BACKUP_DONE";	
	public final static String ACTION_SCAN_FILE = "com.waveface.sync.action.SCAN_FILE";
	

	//BUNDLE DATA
	public static final String BUNDLE_FILE_TYPE = "FILE_TYPE";	
	
	//EXTRA DATA
	public static final String EXTRA_BONJOUR_AUTO_PAIR_STATUS = "com.waveface.sync.extra.BONJOUR_AUTO_PAIR_STATUS";	
	public static final String EXTRA_BONJOUR_SERVICE_EVENT = "com.waveface.sync.extra.BONJOUR_SERVICE_EVENT";
	public final static String EXTRA_WEB_SOCKET_EVENT_CONTENT = "com.waveface.sync.extra.WEB_SOCKET_EVENT_CONTENT";	
	public final static String EXTRA_NETWROK_STATE = "com.waveface.sync.extra.NETWROK_STATE";	
	public final static String EXTRA_NOTIFICATION_ID = "com.waveface.sync.extra.NOTIFICATION_ID";
	
	public static final int EVENT_BONJOUR_RESOLVED = 1;
	public static final int EVENT_BONJOUR_REMOVED = 2;
	public static final int EVENT_BONJOUR_ADDED = 3;
	
	public static final int BONJOUR_PAIRING = 1;
	public static final int BONJOUR_PAIRED = 2;
	

	//SERVER STATUS
	public static final String SERVER_LINKING = "1";
	public static final String SERVER_OFFLINE = "2";	
	public static final String SERVER_DENIED = "3";	
	
	//SERVICE SETTING
    public static String INFINTE_STORAGE = "_infinite-storage._tcp.local.";
	
	//File Type
	public static final int TYPE_IMAGE = 1;
	public static final int TYPE_AUDIO = 2;
	public static final int TYPE_VIDEO = 3;	
	public static final int TYPE_DOC = 4;	
	
	public static final String TRANSFER_TYPE_IMAGE = "image";
	public static final String TRANSFER_TYPE_AUDIO = "audio";
	public static final String TRANSFER_TYPE_VIDEO = "video";	
	
	public static final int K_BYTES = 1024;

	public static final String STATE_OK = "OK";

	public static final int RESULT_BACK = 20;

	//
	public final static String NETWORK_ACTION_BROKEN = "network_broken";	
	public final static String NETWORK_ACTION_WIFI_CONNECTED = "network_wifi_connected";	

	//BONJOUR SERVER ACTION
	public final static String BS_ACTION_SERVER_REMOVED = "bonjour_server_removed";
	
	//WS ACTION
	public final static String WS_ACTION_SOCKET_OPENED = "socket_opened";	
	public final static String WS_ACTION_SOCKET_CLOSED = "socket_closed";
	
	//WS INFINITE STORAGE PROTOCOL
	public final static String WS_ACTION_CONNECT = "connect";
	public final static String WS_ACTION_UPDATE_COUNT = "update-count";	
	public final static String WS_ACTION_FILES_INDEX = "files-index";
	public final static String WS_ACTION_FILE_START = "file-start";
	public final static String WS_ACTION_FILE_END = "file-end";
	public final static String WS_ACTION_BACKUP_INFO = "backup-info";
	//WS NOTIFY HEADER
	public final static String WS_ACTION_WAIT_FOR_PAIR = "wait-for-pair";
	public final static String WS_ACTION_ACCEPT = "accept";
	public final static String WS_ACTION_DENIED = "denied";

	

	
	
	public final static String PARAM_FOLDERNAME = "foldername";	
	public final static String PARAM_FILENAME = "filename";
	public final static String PARAM_FILESIZE = "filesize";	
	
	public final static String PARAM_SERVER_DATA = "server_data";
	public final static String PARAM_SERVER_ID = "server_id";	
	public final static String PARAM_SERVER_OS = "os";		
	public final static String PARAM_RESULT = "result";
	
	// FOR QUEUE JOB STATUS
	public final static String PREFS_NAME = "InfinitePref";
	public final static String PREF_NOTIFICATION_ID = "notification_id";
	public final static String PREF_STATION_WEB_SOCKET_URL = "station_websocket_url";


	//DISPLAY DEVICE NAME
	public final static String PREF_DISPLAY_DEVICE_NAME = "display_device_name";


    //FOR BONJOUR SERVER ALARM SETTING
	public final static String PREF_BONJOUR_SERVER_ALRM_ENNABLED = "bonjour_alarm_enabled";

	//FOR PHOTO Import
	public final static String PREF_AUTO_IMPORTING = "auto_importing";
	public final static String PREF_AUTO_IMPORT_ENABLED = "auto_import_enabled";
	public final static String PREF_AUTO_IMPORT_BORN_TIME ="auto_import_born_time";
	public final static String PREF_AUTO_IMPORT_HOUR = "auto_import_hour";
	public final static String PREF_AUTO_IMPORT_MINUTE = "auto_import_minute";
	public final static String PREF_AUTO_IMPORT_NETWORK = "auto_import_network";
	public final static String PREF_AUTO_IMPORT_TIME_SETTINGS = "auto_import_time_settings";
	public final static String PREF_AUTO_IMPORT_FIRST_TIME_DONE = "auto_import_first_time_done";
	public final static String PREF_AUTO_IMPORT_OLDEST_FILE = "auto_import_oldest_file";


	//SYNC STATUS
	public final static String PREF_SYNC_STATUS_THUMB_UPLOAD_ID = "sync_status_thumb_upload_id";
	
	//REQUEST CODE
	public static final int REQUEST_CODE_OPEN_SERVER_CHOOSER = 0;
	public static final int REQUEST_CODE_ADD_SERVER = 1;
	public static final int REQUEST_CODE_CLEAN_STORAGE = 2;
	

	//RESULT CODE
	public static final int RESULT_CODE_FINISH = 1;
	public static final int RESULT_CODE_NATIVE_SIGNUP_FAIL = 2;

	public static final int ERROR_UNKNOWN = 0;
	public static final int ERROR_NETWORK_NOTAVAILABLE = 1;
	public static final int ERROR_HAS_NO_ORIGIN_BODY = 2;
	public static final int ERROR_HAS_NO_SESSION_TOKEN = 3;

	public static final int NETWORK_UNAVAILABLE = 0;
	public static final int NETWORK_3G = 1;
	public static final int NETWORK_WIFI = 2;

	//IMPORTED FILE STATUS
	public static final String IMPORT_FILE_INCLUDED = "0";
	public static final String IMPORT_FILE_BACKUPED = "3";
	public static final String IMPORT_FILE_EXCLUDE = "99";
	public static final String IMPORT_FILE_DELETED = "100";

	public final static String SIMPLE_TIME_FORMAT = "hh:mm aa";

	public final static String JSON_ERROR_TAG ="JSON_ERROR";
}
