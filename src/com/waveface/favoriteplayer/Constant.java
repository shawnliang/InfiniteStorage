package com.waveface.favoriteplayer;

public class Constant {

	//DEVICE TYPE
	public static boolean PHONE = true;

	public final static String APP_FOLDER = "/FavoritePlayer";
	public final static String VIDEO_FOLDER = APP_FOLDER+"/Video";
	
	
	public final static String ACTION_NETWORK_STATE_CHANGE = "com.waveface.favoriteplayer.action.NETWORK_STATE_CHANGE";	
	public final static String ACTION_WS_SERVER_NOTIFY = "com.waveface.favoriteplayer.action.WS_SERVER_NOTIFY";	

	public final static String ACTION_BONJOUR_SERVER_MANUAL_PAIRING = "com.waveface.favoriteplayer.action.BONJOUR_SERVER_MANUAL_PAIRING";	
	public final static String ACTION_BONJOUR_SERVER_AUTO_PAIRING = "com.waveface.favoriteplayer.action.BONJOUR_SERVER_AUTO_PAIRING";	

	public final static String ACTION_BONJOUR_MULTICAT_EVENT = "com.waveface.favoriteplayer.action.BONJOUR_MULTICAT_EVENT";	
	public final static String ACTION_UPLOADING_FILE = "com.waveface.favoriteplayer.action.UPLOADING_FILE";		
	public final static String ACTION_BACKUP_FILE = "com.waveface.favoriteplayer.action.BACKUP_FILE";	
	public final static String ACTION_BACKUP_START = "com.waveface.favoriteplayer.action.BACKUP_START";		
	public final static String ACTION_BACKUP_DONE = "com.waveface.favoriteplayer.action.BACKUP_DONE";	
	public final static String ACTION_SCAN_FILE = "com.waveface.favoriteplayer.action.SCAN_FILE";
	public final static String ACTION_FILE_DELETED = "com.waveface.favoriteplayer.action.FILE_DELETED";	
	public final static String ACTION_FAVORITE_PLAYER_ALARM = "com.waveface.favoriteplayer.action.FAVORITE_PLAYER_ALARM";
	public final static String ACTION_WEB_SOCKET_SERVER_CONNECTED = "com.waveface.favoriteplayer.action.WEB_SOCKET_SERVER_CONNECTED";
	public final static String ACTION_WEB_SOCKET_SERVER_DISCONNECTED = "com.waveface.favoriteplayer.action.WEB_SOCKET_SERVER_DISCONNECTED";	
	public final static String ACTION_RELEASE_PAIRED_SERVER_BY_CLIENT = "com.waveface.favoriteplayer.action.RELEASE_PAIRED_SERVER_BY_CLIENT";	
    public final static String ACTION_NOT_ENOUGH_SPACE="com.waveface.favoriteplayer.action.NOT_ENOUGH_SPACE";	

	//BUNDLE DATA
	public static final String BUNDLE_FILE_TYPE = "FILE_TYPE";	
	
	//EXTRA DATA
	public static final String EXTRA_BONJOUR_AUTO_PAIR_STATUS = "com.waveface.favoriteplayer.extra.BONJOUR_AUTO_PAIR_STATUS";	
	public static final String EXTRA_BONJOUR_SERVICE_EVENT = "com.waveface.favoriteplayer.extra.BONJOUR_SERVICE_EVENT";
	public final static String EXTRA_WEB_SOCKET_EVENT_CONTENT = "com.waveface.favoriteplayer.extra.WEB_SOCKET_EVENT_CONTENT";	
	public final static String EXTRA_NETWROK_STATE = "com.waveface.favoriteplayer.extra.NETWROK_STATE";	
	public final static String EXTRA_NOTIFICATION_ID = "com.waveface.favoriteplayer.extra.NOTIFICATION_ID";
	public final static String EXTRA_BACKING_UP_FILE_STATE = "com.waveface.favoriteplayer.extra.BACKING_UP_FILE_STATE";
	
	
	public static final int EVENT_BONJOUR_RESOLVED = 1;
	public static final int EVENT_BONJOUR_REMOVED = 2;
	public static final int EVENT_BONJOUR_ADDED = 3;
	
	public static final int BONJOUR_PAIRING = 1;
	public static final int BONJOUR_PAIRED = 2;
	
	public static final int STATION_CONNECTION_TIMEOUT = 20000;
	public static final int STATION_SOCKET_TIMEOUT = 20000;
	
	//ALARM INTERVAL
	public static final long ALARM_INTERVAL =  1 *30 * 1000;


	//CATEGORY
	public static final String CATEGORY_UI = "ui";
	public static final String CATEGORY_SERVICE = "service";
	
	
	//UI ACTION
	public static final String ANALYTICS_ACTION_BTN_PRESS = "btn_press";
	
	//SERVICE ACTION
	public static final String ANALYTICS_ACTION_FINAL_BACKUP_COUNT = "final_backedup_count";
	
	//USRER ACTION VALUE
	public static final String ANALYTICS_LABEL_SETTING = "click_setting";
	public static final String ANALYTICS_LABEL_IMAGE = "click_image";
	public static final String ANALYTICS_LABEL_VIDEO = "click_video";
	public static final String ANALYTICS_LABEL_AUDIO = "click_audio";	
	public static final String ANALYTICS_LABEL_ADD_PC = "click_add_pc";	
	public static final String ANALYTICS_LABEL_COUNT = "count";
	

	//SERVER STATUS
	public static final String SERVER_LINKING = "1";
	public static final String SERVER_OFFLINE = "2";	
	public static final String SERVER_DENIED_BY_SERVER = "3";	
	public static final String SERVER_DENIED_BY_CLIENT = "4";	
	

	//FILE BACKUP STATE
	public static final int JOB_START = 0;	
	public static final int FILE_START = 1;
	
	//SERVICE SETTING
    public static String INFINTE_STORAGE = "_infinite-storage._tcp.local.";
	public static String BONJOUR_NAME = "infiniteDisplay";
	
	//File Type
	public static final int TYPE_IMAGE = 1;
	public static final int TYPE_AUDIO = 2;
	public static final int TYPE_VIDEO = 3;	
	public static final int TYPE_DOC = 4;	
	
	//LABEL AUTO_Type
	public static final int TYPE_FAVORITE = 0;
	public static final int TYPE_RECENT_PHOTO_TODAY = 1;
	public static final int TYPE_RECENT_PHOTO_YESTERDAY = 2;
	public static final int TYPE_RECENT_PHOTO_THISWEEK = 3;
	public static final int TYPE_RECENT_VIDEO_TODAY = 4;
	public static final int TYPE_RECENT_VIDEO_YESTERDAY = 5;
	public static final int TYPE_RECENT_VIDEO_THISWEEK = 6;
	
	

	//File Type FOR DATABASE
	public static final String FILE_TYPE_IMAGE = "0";
	public static final String FILE_TYPE_VIDEO = "1";

	
	public static final String TRANSFER_TYPE_IMAGE = "image";
	public static final String TRANSFER_TYPE_AUDIO = "audio";
	public static final String TRANSFER_TYPE_VIDEO = "video";	
	
	public static final int K_BYTES = 1024;

	public static final String STATE_OK = "OK";

	public static final int RESULT_BACK = 20;

	//
	public final static String NETWORK_ACTION_WIFI_BROKEN = "network_broken";	
	public final static String NETWORK_ACTION_WIFI_CONNECTED = "network_wifi_connected";	

	//BONJOUR SERVER ACTION
	public final static String BS_ACTION_SERVER_REMOVED = "bonjour_server_removed";
	
	//WS ACTION
	public final static String WS_ACTION_SOCKET_OPENED = "socket_opened";	
	public final static String WS_ACTION_SOCKET_CLOSED = "socket_closed";
	public final static String WS_ACTION_START_BACKUP = "start_backup";	
	public final static String WS_ACTION_END_BACKUP = "end_backup";
	
	
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
	public final static String WS_ACTION_SERVER_REMOVED = "server_removed";
	//NETWORK
	public final static String NETWORK_IS_NOT_WIFI = "network_is_not_wifi";
	
	
	public final static String PARAM_FOLDERNAME = "foldername";	
	public final static String PARAM_FILENAME = "filename";
	public final static String PARAM_FILESIZE = "filesize";	
	
	public final static String PARAM_SERVER_DATA = "server_data";
	public final static String PARAM_SERVER_ID = "server_id";
	public final static String PARAM_SERVER_NAME = "server_name";	
	public final static String PARAM_SERVER_IP = "server_ip";	
	public final static String PARAM_WS_PORT = "ws_port";	
	public final static String PARAM_NOTIFY_PORT = "notify_port";	
	public final static String PARAM_REST_PORT = "rest_port";	
	
	public final static String PARAM_SERVER_WS_LOCATION = "server_ws_location";		
	public final static String PARAM_SERVER_OS = "os";		
	public final static String PARAM_RESULT = "result";
	
	//FOR LABEL RESTFUL API
	public final static String URL_GET_ALL_LABELS = "/label/list_all";
	public final static String URL_GET_LABEL = "/label/get";
	public final static String URL_GET_FILE = "/file/get";
	public final static String URL_IMAGE = "/image";
	public final static String URL_IMAGE_LARGE = "/large";
	public final static String URL_IMAGE_MEDIUM = "/medium";
	public final static String URL_IMAGE_SMALL = "/small";
	public final static String URL_IMAGE_ORIGIN="origin";
	
	
	// FOR LABEL DATE PARAMETER
	public final static String PARAM_FILES = "files";
	public final static String PARAM_SEQ = "seq";
	public final static String PARAM_LABEL_ID = "label_id";
	// FOR QUEUE JOB STATUS
	public final static String PREFS_NAME = "InfinitePref";
	public final static String PREF_NOTIFICATION_ID = "notification_id";
	public final static String PREF_STATION_WEB_SOCKET_URL = "station_websocket_url";


	//DISPLAY DEVICE NAME
	public final static String PREF_DISPLAY_DEVICE_NAME = "display_device_name";


    //FOR BONJOUR SERVER ALARM SETTING
	public final static String PREF_BONJOUR_SERVER_ALRM_ENNABLED = "bonjour_alarm_enabled";
	
	//FOR MEDIA Import
	public final static String PREF_AUTO_IMPORT_ENABLED = "auto_import_enabled";
	public final static String PREF_AUTO_IMPORT_BORN_TIME ="auto_import_born_time";
	public final static String PREF_AUTO_IMPORT_HOUR = "auto_import_hour";
	public final static String PREF_AUTO_IMPORT_MINUTE = "auto_import_minute";
	public final static String PREF_AUTO_IMPORT_NETWORK = "auto_import_network";
	public final static String PREF_AUTO_IMPORT_TIME_SETTINGS = "auto_import_time_settings";
	public final static String PREF_FILE_IMPORT_FIRST_TIME_DONE = "file_import_first_time_done";

	//SYNC STATUS
	public final static String PREF_SYNC_STATUS_THUMB_UPLOAD_ID = "sync_status_thumb_upload_id";
	
	//REQUEST CODE
	public static final int REQUEST_CODE_OPEN_SERVER_CHOOSER = 0;
	public static final int REQUEST_CODE_ADD_SERVER = 1;
	public static final int REQUEST_CODE_CLEAN_STORAGE = 2;
	public static final int REQUEST_CODE_CHANGE_SERVER = 3;
	
	

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

	//NOTIFICATION CODE
	public static final int NOTIFICATION_BACK_UP_START = 0;
	public static final int NOTIFICATION_BACK_UP_ON_PROGRESS = 1;
	public static final int NOTIFICATION_BACKED_UP = 2;
	
	
	//IMPORTED FILE STATUS
	public static final String IMPORT_FILE_INCLUDED = "0";
	public static final String IMPORT_FILE_BACKUPED = "3";
	public static final String IMPORT_FILE_EXCLUDE = "99";
	public static final String IMPORT_FILE_DELETED = "100";

	public final static String SIMPLE_TIME_FORMAT = "hh:mm aa";

	public final static String JSON_ERROR_TAG ="JSON_ERROR";
	

	// passing to bundle
	public static final String ARGUMENT1 = "argument1";
	public static final String ARGUMENT2 = "argument2";
	public static final String ARGUMENT3 = "argument3";
	
	//Label SharedPreferences
	public final static String PREF_SERVER_LABEL_SEQ = "server_label_seq";
	public final static String PREF_SERVER_CHANGE_LABEL_ID = "server_change_label_id";
	public final static String PREF_DOWNLOAD_LABEL_INIT_STATUS = "download_label_init_status";
	
	public final static int AVAIABLE_SPACE =30;
}
