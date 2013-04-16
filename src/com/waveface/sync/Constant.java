package com.waveface.sync;

public class Constant {

	public final static String APP_FOLDER = "/InfiniteStorage";


	public final static String ACTION_WS_SERVER_NOTIFY = "com.waveface.wammer.action.WS_SERVER_NOTIFY";	
	public final static String ACTION_BONJOUR_MULTICAT_EVENT = "com.waveface.wammer.action.BONJOUR_MULTICAT_EVENT";	
	public final static String ACTION_BACKUP_FILE = "com.waveface.wammer.action.BACKUP_FILE";
	public final static String ACTION_SCAN_FILE = "com.waveface.wammer.action.SCAN_FILE";

	//
	public static final String EXTRA_BONJOUR_SERVICE_EVENT = "com.waveface.wammer.extra.BONJOUR_SERVICE_EVENT";
	public final static String EXTRA_SERVER_DATA = "com.waveface.wammer.extra.SERVER_DATA";
	
	public static final int EVENT_BONJOUR_RESOLVED = 1;
	public static final int EVENT_BONJOUR_REMOVED = 2;
	public static final int EVENT_BONJOUR_ADDED = 3;
	
	//SERVER STATUS
	public static final String SERVER_LINKING = "1";
	public static final String SERVER_OFFLINE = "2";	
	
	//SERVICE SETTING
    public static String INFINTE_STORAGE = "_infinite-storage._tcp.local.";
	
	//File Type
	public static final int TYPE_IMAGE = 1;
	public static final int TYPE_AUDIO = 2;
	public static final int TYPE_VIDEO = 3;	
	public static final int TYPE_DOC = 4;		
	
	public static final int K_BYTES = 1024;

	
	public final static String ACTION_NETWORK_STATE_CHANGED = "com.waveface.wammer.action.NETWORK_STATE_CHANGED";
	public final static String ACTION_STATION_LINKED = "com.waveface.wammer.action.STATION_LINKED";
	public final static String ACTION_SERVICE_SHOW_TOAST = "com.waveface.wammer.action.SERVICE_SHOW_TOAST";
	public final static String ACTION_STATION_NOTIFY = "com.waveface.wammer.action.STATION_NOTIFY";
	public final static String ACTION_HAS_NOTIFY_QUEUE = "com.waveface.wammer.action.HAS_NOTIFY_QUEUE";
	public final static String ACTION_REMOVE_UPLOAD_IMAGE_NOTIFY = "com.waveface.wammer.action.REMOVE_UPLOAD_IMAGE_NOTIFY";

	public final static String ACTION_PHOTO_IMPORT = "com.waveface.wammer.action.PHOTO_IMPORT";
	public final static String ACTION_PHOTO_AUTO_IMPORTING = "com.waveface.wammer.action.PHOTO_AUTO_IMPORTING";
	public final static String ACTION_POST_IMPORT_CREATE = "com.waveface.wammer.action.POST_IMPORT_CREATE";
	public final static String ACTION_FIRSTUSE_LINK_TO_STATION = "com.waveface.wammer.action.FIRSTUSE_LINK_TO_STATION";
	
	public final static int STATE_SUCCESS = 0;
	public final static int STATE_FAILED = 1;
	public final static int STATE_REFRESHING = 2;

	public static final int TIMELINE_IMAGE_TOTAL_HEIGHT = 200;
	public static final int TIMELINE_IMAGE_2CELL_TOTAL_HEIGHT = 148;
	public static final int TIMELINE_IMAGE_TOTAL_WIDTH = 285;

	public static final String STATE_OK = "OK";

	public static final int RESULT_BACK = 20;

	//WS ACTION
	public final static String WS_ACTION_CONNECT = "connect";
	public final static String WS_ACTION_FILES_INDEX = "files-index";
	public final static String WS_ACTION_FILE_START = "file-start";
	public final static String WS_ACTION_FILE_END = "file-end";
	public final static String WS_ACTION_BACKUP_INFO = "backup-info";

	//EXTRA DATA
	public final static String EXTRA_SERVER_NOTIFY_CONTENT = "server_notify_content";	
	
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
	
	public final static String PARAM_CREATOR_ID = "creator_id";
	public final static String PARAM_DEVICE_NAME = "device_name";
	public final static String PARAM_DEVICE_ID = "device_id";
	public final static String PARAM_FILENAMES = "filenames";
	public final static String PARAM_FAVORITE = "favorite";
	public final static String PARAM_IMPORT = "import";
	public final static String PARAM_FILE_PATH = "file_path";
	public final static String PARAM_IMPORT_TIME = "import_time";
	public final static String PARAM_FILE_CREATE_TIME = "file_create_time";
	public final static String PARAM_EXIF = "exif";
	public final static String PARAM_TIMEZONE = "timezone";
	public final static String PARAM_COVER_ATTACH = "cover_attach";
	public final static String PARAM_FROM_COMPOSER = "from_composer";
	
	public final static String PARAM_GROUP_ID = "group_id";
	public final static String PARAM_POST_ID = "post_id";
	public final static String PARAM_COLLECTION_ID = "collection_id";
	public final static String PARAM_COLLECTION_ID_LIST = "collection_id_list";
	public final static String PARAM_UPDATE_TIME = "update_time";
	public final static String PARAM_LAST_UPDATE_TIME = "last_update_time";
	public final static String PARAM_EVENT_TIME = "event_time";
	public final static String PARAM_CONTENT = "content";
	public final static String PARAM_TEXT = "text";
	public final static String PARAM_TYPE = "type";
	public final static String PARAM_TITLE = "title";
	public final static String PARAM_DESCRIPTION = "description";
	public final static String PARAM_IMAGE = "image";
	public final static String PARAM_IMAGE_META = "image_meta";
	public final static String PARAM_FILE = "file";
	public final static String PARAM_LOGIN = "login";
	public final static String PARAM_ACCOUNT_TYPE = "account_type";
	public final static String PARAM_SIGNUP = "signup";
	public final static String PARAM_API_RETURN_CODE = "api_return_code";
	public final static String PARAM_API_RETURN_MESSAGE = "api_return_message";
	public final static String PARAM_IS_BROADCAST = "isBroadcast";
	public final static String PARAM_FILTER = "filter";
	public final static String PARAM_FILTER_ENTITY = "filter_entity";
	public final static String PARAM_OPEN_GRPPH = "og";
	public final static String PARAM_SOURCE = "source";
	public final static String PARAM_USER_ID = "user_id";
	public final static String PARAM_USER_UUID = "user_uuid";
	public final static String PARAM_SESSION_TOKEN = "session_token";
	public final static String PARAM_USER_PASSWORD = "password";
	public final static String PARAM_USER_NICKNAME = "nickname";
	public final static String PARAM_USER_AVATOR_URL = "avatar_url";
	public final static String PARAM_TIMESTAMP = "timestamp";
	public final static String PARAM_COVER_IMAGE = "cover_attach";
	public final static String PARAM_ATTACHMENT_ID_ARRAY = "attachment_id_array";
	public final static String PARAM_SINCE = "since";
	public final static String PARAM_SINCE_SEQ_NUM = "since_seq_num";
	public final static String PARAM_LIMIT = "limit";
	public final static String PARAM_UP_LIMIT = "up_limit";
	public final static String PARAM_DOWN_LIMIT = "down_limit";
	public final static String PARAM_COMPONENT_OPTIONS = "component_options";
	public final static String PARAM_UUID = "UUID";
	public final static String PARAM_ACTION = "acton";
	public final static String PARAM_WEB_LOGIN = "web_login";
	public final static String PARAM_PASS_FLOW = "pass_flow";
	public final static String PARAM_URL = "url";
	public final static String PARAM_XURL = "xurl";
	public final static String PARAM_LOCALE = "locale";
	public final static String PARAM_DEVICE = "device";
	public final static String PARAM_TOKEN = "token";
	public final static String PARAM_LASTEST = "lastest";
	public final static String PARAM_APIKEY = "apikey";
	public final static String PARAM_OBJECT_TYPE = "object_type";
	public final static String PARAM_OBJECT_ID = "object_id";
	public final static String PARAM_OBJECT_IDS = "object_ids";
	public final static String PARAM_ATTACHMENT_IDS = "attachment_ids";
	public final static String PARAM_TARGET = "target";
	public final static String PARAM_PAGE = "page";
	public final static String PARAM_FORCE_HTTP = "force_http";
	public final static String PARAM_METADATA = "metadata";
	public final static String PARAM_DB_ID = "db_id";
	public final static String PARAM_COUNT = "count";
	public final static String PARAM_DATUM = "datum";
	public final static String PARAM_WEB_CALLBACK = "web_callback";
	public final static String PARAM_TOP_TIMESTAMP = "top_timestamp";
	public final static String PARAM_BOTTOM_TIMESTAMP = "bottom_timestamp";
	public final static String PARAM_WEB_SIGNUP_XURL = "stream://android?api_ret_code=%(api_ret_code)d&api_ret_message=%(api_ret_message)s&device_id=%(device_id)s&session_token=%(session_token)s&user_id=%(user_id)s&account_type=%(account_type)s&email=%(email)s&password=%(password)s";
	public final static String PARAM_WEB_FACEBOOK_LOGIN_XURL = "stream://android?api_ret_code=%(api_ret_code)d&api_ret_message=%(api_ret_message)s&device_id=%(device_id)s&session_token=%(session_token)s&user_id=%(user_id)s";
	public final static String PARAM_WEB_CONNECT_XURL = "waveface://snsConnect?api_ret_code=%(api_ret_code)d&api_ret_message=%(api_ret_message)s";
	public final static String PARAM_SNS = "sns";
	public final static String PARAM_AUTH_TOKEN = "auth_token";
	public final static String PARAM_SNS_CONNECT = "sns_connect";
	public final static String PARAM_SUBSCRIBED = "subscribed";
	public final static String PARAM_LANG = "lang";
	public final static String PARAM_PURGE_ALL = "purge_all";
	public final static String PARAM_CONNECTION_TIMEOUT = "connectionTimeout";
	public final static String PARAM_SOCKET_TIMEOUT = "socketTimeout";
	public final static String PARAM_FACEBOOK = "facebook";
	public final static String PARAM_FACEBOOK_ACTION = "facebook_import";
	public final static String PARAM_FACEBOOK_DISCONNECTIONED = "disconnected";
	public final static String PARAM_GOOGLE = "google";
	public final static String PARAM_TWITTER = "twitter";
	public final static String PARAM_FOURSQUARE = "foursquare";

	public final static String PARAM_EMAIL = "email";
	public final static String PARAM_PASSWORD = "password";
	public final static String PARAM_NICKNAME = "nickname";
	public final static String PARAM_TOAST_MESSAGE = "toastMessage";
	public final static String PARAM_TIMER_COMMAND = "timerCommand";
	public final static String PARAM_COMMAND_CANCEL = "cmdCancel";
	public final static String PARAM_COMMAND_RESTART = "cmdRestart";
	public final static String PARAM_START = "start";

	// FOR QUEUE JOB STATUS
	public final static String QUEUE_STATUS_INIT = "";
	public final static String QUEUE_STATUS_PROCESS = "P";
	public final static String QUEUE_STATUS_DONE = "1";

	public final static String PREFS_NAME = "InfinitePref";
	public final static String PREF_USERNAME = "username";
	public final static String PREF_USER_ID = "UserId";
	public final static String PREF_USER_EMAIL = "UserEmail";
	public final static String PREF_USER_STATE = "state";
	public final static String PREF_SESSION_TOKEN = "sessionToken";
	public final static String PREF_STATION_SESSION_TOKEN = "stationSessionToken";
	public final static String PREF_SESSION_EXPIRES = "sessionExpires";
	public final static String PREF_GROUP_ID = "groupId";
	public final static String PREF_SERVER_HOST = "host";
	public final static String PREF_API_VERSION = "api_version";
	public final static String PREF_LATEST_ID = "latest_id";
	public final static String PREF_LATEST_TIMESTAMP = "latest_timestamp";
	public final static String PREF_UNREAD = "unread";
	public final static String PREF_NOTIFICATION_ID = "notification_id";
	public final static String PREF_STATION_WEB_SOCKET_URL = "station_websocket_url";


	//DISPLAY DEVICE NAME
	public final static String PREF_DISPLAY_DEVICE_NAME = "display_device_name";


	//FOR SERVER
	public final static String PREF_CLOUD_SERVER = "cloud_server";

	//CONNECT CLOUD SERVICES
	public final static int GOOGLE_CONNECT = 1 ;
	public final static int TWITTER_CONNECT = 2 ;
	public final static int FOURSQUARE_CONNECT = 3 ;


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

	//STORAGE FOR KEEP ORIGINAL FILE
	public final static String PREF_STORAGE_FOR_KEEP = "storage_for_keep";
	public final static int STORAGE_KEEP_DEFAULT_MB = 500;


	//CALENDAR
	public final static String CALENDAR_CELL_TYPE = "type";
	public final static String CALENDAR_CELL_DESC = "desc";
	public final static String CALENDAR_CELL_IMAGE_PATH = "imagePath";



	// PREFERENCE
	//public static String PREF_USER_STATE = "user_state";
	public static String PREF_SERVER = "Link_Server";
	public static String PREF_SERVER_ID = "Current_Server_ID";
	
	public final static String DEVICE_NAME = "Android";
	public final static String DEVICE_TABLET_NAME = "AndroidTablet";
	public final static String SHARED_URL_LINK = "SHARED_URL_LINK";

	//REQUEST CODE
	public static final int REQUEST_CODE_OPEN_SERVER_CHOOSER = 0;


	public static final int RESULT_CODE_FINISH = 1;
	public static final int RESULT_CODE_NATIVE_SIGNUP_FAIL = 2;


	public static final String EXTRA_POST_PHOTOS = "com.waveface.wammer.extra.POST_PHOTOS";

	public static final String READITLATE_CSS = "<style type='text/css'>"
			+ "body { "
			+ "color: #e8e8e2; margin:0; padding:0; font-size: medium;" + " }"
			+ "</style>";

	public static final String STRING_PATTERN_MOBILE_YOUTUBE = "HTTP://M.YOUTUBE.COM/";
	public static final String STRING_PATTERN_YOUTUBE = "HTTP://WWW.YOUTUBE.COM/";

	public static final int ERROR_UNKNOWN = 0;
	public static final int ERROR_NETWORK_NOTAVAILABLE = 1;
	public static final int ERROR_HAS_NO_ORIGIN_BODY = 2;
	public static final int ERROR_HAS_NO_SESSION_TOKEN = 3;

	public static final int NETWORK_UNAVAILABLE = 0;
	public static final int NETWORK_3G = 1;
	public static final int NETWORK_WIFI = 2;


	public static final String SUPPORT_ATTACH_TYPES = "pdf";

	public static final String TYPE_ATTACHMENT_PHOTO = "image";
	public static final String TYPE_ATTACHMENT_DOCUMENT = "doc";
	public static final String TYPE_ATTACHMENT_WEBTHUMB = "webthumb";

	public final static String NAMESPACE_TIMELINE = "TIMELINE";
	public final static String NAMESPACE_MEDIUM = "MEDIUM";
	public final static String NAMESPACE_PHOTO = "PHOTO";
	public final static String NAMESPACE_ARTICLE = "ARTICLE";
	public final static String NAMESPACE_QUICK = "QUICK";
	public final static String NAMESPACE_ACTION = "ACTION";
	public final static String NAMESPACE_POST = "POST";

	public final static String LOCAL_FILE_PREFIX = "file://";

	public final static String IMAGE_ORIGIN = "origin";
	public final static String IMAGE_LARGE = "large";
	public final static String IMAGE_MEDIUM = "medium";
	public final static String IMAGE_SMALL = "small";
	public final static String TARGET_IMAGE = "image";
	public final static String TARGET_PREVIEW = "preview";

	public final static int    IMAGE_UPLOAD_SCALE_LENGTH = 1024;
	public final static float  IMAGE_UPLOAD_COMPRASS_GAP = 150.00f;
	public final static int    IMAGE_UPLOAD_COMPRASS_RATE = 85;

	public final static long DETECT_STATION_MINUTES = 1;
	public final static long CHECK_CHANGE_LOGS_ON_3G_SEC = 60;
	public final static long CHECK_CHANGE_LOGS_ON_WIFI_SEC = 30;
	public final static long CHECK_CHANGE_LOGS_WITH_STATION_ON_WIFI_SEC = 10;
	public final static long CHECK_SYNC_NOTIFY_DELAY_SEC = 3;
	public final static long CHECK_SYNC_NOTIFY_PERIOD_SEC = 20;
	public final static long CHECK_BACKGROUND_WORKER_DELAY_SEC = 10;
	public final static long CHECK_BACKGROUND_WORKER_PERIOD_SEC = 20;


	public final static int VIEW_LIMIT = 15;
	public final static int TIMELINE_HANDLE_USERTRACK_CALLBACK_LIMIT = 10;
	public final static int FETCH_POSTS_LIMIT = 50;
	public final static int FETCH_POSTS_BACKEND_LIMIT = 25;
	public final static long FETCH_POSTS_BACKEND_WAIT_MICRO_SEC = 500;
	public final static int RESERVE_ATT_ID_COUNT = 500;
	public final static int DOWNLOAD_IMAGE_THREAD_COUNT_WITH_STATION = 3;
	public final static int DOWNLOAD_IMAGE_THREAD_COUNT_WITH_WIFI = 2;
	public final static int DOWNLOAD_IMAGE_THREAD_COUNT_WITH_GPRS = 1;
	public final static int DOWNLOAD_META_LIMIT = 100 ;
	public final static int KEEP_ALIVE_MICRO_SEC = 20000;
	public final static String DOWNLOAD_META_VIEW_COMPACT = "compact" ;
	public final static String DOWNLOAD_META_VIEW_ALL = "all" ;

	//UPLOAD IMAGE STATUS
	public static final String UPLOAD_IMAGE_INDEXED = "0";
	public static final String UPLOAD_IMAGE_ON_PROGRESS = "-";
	public static final String UPLOAD_IMAGE_DONE = "1";

	//IMPORT POST ID
	public final static String AUTO_IMPORT_POST_ID = "AUTO";

	//IMPORT NETWORK OPTIONS
	public final static int UPLOAD_OVER_3G_OR_WIFI = 0;
	public final static int UPLOAD_OVER_WIFI_ONLY = 1;

	//IMPORT TIME OPTIONS
	public final static int AUTO_IMPORT_5_SECONDS = 0;
	public final static int AUTO_IMPORT_15_SECONDS = 1;
	public final static int AUTO_IMPORT_30_SECONDS = 2;
	public final static int AUTO_IMPORT_1_MNUTE = 3;
	public final static int AUTO_IMPORT_5_MNUTES = 4;
	public final static int AUTO_IMPORT_10_MNUTES = 5;
	public final static int AUTO_IMPORT_1_HOUR = 6;
	public final static int AUTO_IMPORT_EVERYDAY = 7;

	//IMPORTED FILE STATUS
	public static final String IMPORT_FILE_INCLUDED = "0";
	public static final String IMPORT_FILE_BACKUPED = "3";
	public static final String IMPORT_FILE_EXCLUDE = "99";
	public static final String IMPORT_FILE_DELETED = "100";

	//FIRST USE ACTION
	public final static String FIRST_USE_SIGN_UP = "signUp";
	public final static String FIRST_USE_LOGIN = "login";



	//PAY PLAN
	public final static String PLAN_TYPE_FREE = "free";
	public final static String PLAN_TYPE_PAID = "paid";

	public final static int PAYMENT_FREE = 0;
	public final static int PAYMENT_PREMIUM = 1;
	public final static int PAYMENT_UNLIMITED = 2;


	public static final String DATA_ITEM_LIST = "com.waveface.wammer.data.ITEM_LIST";
	public static final String DATA_PAGE_LIST = "com.waveface.wammer.data.PAGE_LIST";
	public static final String DATA_PAGE_NUM = "com.waveface.wammer.data.PAGE_NUM";
	public static final String DATA_PATH_INFO = "com.waveface.wammer.data.PATH_INFO";
	public static final String DATA_FOLDER_INFO = "com.waveface.wammer.data.FOLDER_INFO";
	public static final String DATA_SOURCE = "com.waveface.wammer.data.SOURCE";
	public static final String DATA_TYPE = "com.waveface.wammer.data.TYPE";
	public static final String DATA_END_ROW_TIME ="com.waveface.wammer.data.mEventDayEndRowTime";
	public static final String DATA_TASK_NAME ="com.waveface.wammer.data.TASK_NAME";
	public static final String DATA_TASK_RESULT ="com.waveface.wammer.data.TASK_RESULT";
	public static final String DATA_COLLECTION_NAME ="com.waveface.wammer.data.COLLECTION_NAME";
	public static final String DATA_COLLECTION_ID ="com.waveface.wammer.data.COLLECTION_ID";

	public static final String DATA_CHECKIN_NAME ="com.waveface.wammer.data.latitude.DATA_CHECKIN_NAME";
    public static final String DATA_ZOOM_LEVEL ="com.waveface.wammer.data.latitude.DATA_GPS_ZOOM_LEVEL";
	public static final String DATA_LATITUDE ="com.waveface.wammer.data.latitude.DATA_GPS_LATITUDE";
    public static final String DATA_LONGTITUDE ="com.waveface.wammer.data.latitude.DATA_GPS_LONGTITUDE";
    public static final String DATA_PAGENUM = "com.waveface.wammer.data.PAGENUM";
    public static final String DATA_PHOTOS = "com.waveface.wammer.data.PHOTOS";
    public static final String DATA_EVENTS = "com.waveface.wammer.data.EVENTS";
    public static final String DATA_CURRENT_DATE="com.waveface.wammer.CalendarActivity.DATA_CURRENT_DATE";
    public static final String DATA_EVENT_DAY_LIST="com.waveface.wammer.CalendarActivity.DATA_EVENT_DAY_LIST";
    public static final String DATA_EVENT_CHECKLIST="com.waveface.wammer.CalendarActivity.DATA_EVENT_CHECKLIST";
    public static final String DATA_EVETN_AVATORS = "com.waveface.wammer.data.DATA_EVETN_AVATORS";
    public static final String DATA_EXIF ="com.waveface.wammer.data.EXIF";
    public static final String DATA_OBJECT_ID ="com.waveface.wammer.data.OBJECT_ID";
    public static final String DATA_ADDMONTH = "com.waveface.wammer.data.ADDMONTH";
    public static final String DATA_EVENT_DESC = "com.waveface.wammer.data.EVENT_DESC";
    public static final String DATA_DATE = "com.waveface.wammer.data.DATE";


    public static final String DATA_CHOICE_MODE ="com.waveface.wammer.data.CHOICE_MODE";
    public static final String DATA_CHECKED_INDEX ="com.waveface.wammer.data.CHECKED_INDEX";
    public static final String DATA_CHECKED_COUNT ="com.waveface.wammer.data.CHECKED_COUNT";
    public static final String DATA_URL ="com.waveface.wammer.data.URL";
    public static final String DATA_HIDE_BACK ="com.waveface.wammer.data.HIDE_BACK";

	public static final String DATA_QUERY_URI ="com.waveface.wammer.data.QUERY_URI";
	public static final String DATA_QUERY_PROJECTIONS ="com.waveface.wammer.data.QUERY_PROJECTIONS";
	public static final String DATA_QUERY_SELECTIONS ="com.waveface.wammer.data.QUERY_SELECTIONS";
	public static final String DATA_QUERY_ARGUMENTS ="com.waveface.wammer.data.QUERY_ARGUMENTS";
	public static final String DATA_QUERY_SORT ="com.waveface.wammer.data.QUERY_SORTING";

	public final static String ACTION_STATUS_CHANGE = "com.waveface.wammer.action.STATUS_CHANGE";
	public static final String DATA_STATUS_TYPE ="com.waveface.wammer.data.STATUS_TYPE";
	public static final int STATUS_DOWNLOAD_META = 1;
	public static final int STATUS_DOWNLOAD_CHANGELOG = 2;
	public static final int STATUS_UPLOAD_IMAGE = 3;
	public static final int STATUS_UPLOAD_ORIGINL_IMAGE = 4;



	public static final int AUTO_IMPORT_ID = 100001;

	public final static String STREAM_SUPPORT_ID = "support@waveface.com";
	public final static int AVAIABLE_SPACE =30;

	public final static int SLIDING_WIDTH_DIP = 80;

	public final static String SIMPLE_TIME_FORMAT = "hh:mm aa";

	public final static String JSON_ERROR_TAG ="JSON_ERROR";

	//google analytics
	public final static String clickHardwareMenu ="click_hardware_menu";
	public final static String clickLiginExistFBBtn ="click_login_exist_fb_btn";

}
