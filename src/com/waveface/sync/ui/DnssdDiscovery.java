package com.waveface.sync.ui;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Enumeration;

import javax.jmdns.JmDNS;
import javax.jmdns.ServiceEvent;
import javax.jmdns.ServiceInfo;
import javax.jmdns.ServiceListener;

import org.jwebsocket.kit.WebSocketException;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.SharedPreferences;
import android.database.Cursor;
import android.os.AsyncTask;
import android.os.Bundle;
import android.provider.MediaStore;
import android.text.TextUtils;
import android.widget.TextView;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.RuntimePlayer;
import com.waveface.sync.entity.FIleTransferEntity;
import com.waveface.sync.util.Log;
import com.waveface.sync.util.StringUtil;
import com.waveface.sync.websocket.RuntimeWebClient;


public class DnssdDiscovery extends Activity {
	private String TAG = DnssdDiscovery.class.getSimpleName();
    android.net.wifi.WifiManager.MulticastLock lock;
    android.os.Handler handler = new android.os.Handler();
    public static String DATE_FOMAT = "yyyyMMddHHmmss";
    private final int BASE_BYTES = 1024;
	private static ArrayList<String> mFolders = new ArrayList<String>();			
	private static ArrayList<String> mFilenames = new ArrayList<String>();
	private static ArrayList<String> mFilesizes = new ArrayList<String>();

    
    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.discover);
		IntentFilter filter = new IntentFilter();
		filter.addAction(Constant.ACTION_BACKUP_FILE);
		registerReceiver(mReceiver, filter);

        handler.postDelayed(new Runnable() {
            public void run() {
                setUp();
            }
            }, 1000);

    }    /** Called when the activity is first created. */
	private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			String action = intent.getAction();
			Log.d(TAG, "action:" + intent.getAction());
			if (Constant.ACTION_BACKUP_FILE.equals(action)) {
//				new SendFileTask().execute(new Void[]{});
			}
		}
	};
	@Override
	public void onDestroy() {
		super.onDestroy();
		unregisterReceiver(mReceiver);
	}

    private String type = "_infinite-storage._tcp.local.";
    private JmDNS jmdns = null;
    private ServiceListener listener = null;
    private void setUp() {
        android.net.wifi.WifiManager wifi = (android.net.wifi.WifiManager) getSystemService(android.content.Context.WIFI_SERVICE);
        lock = wifi.createMulticastLock("mylockthereturn");
        lock.setReferenceCounted(true);
        lock.acquire();
        try {
            jmdns = JmDNS.create();
            jmdns.addServiceListener(type, listener = new ServiceListener() {

                @Override
                public void serviceResolved(ServiceEvent ev) {
                	@SuppressWarnings("deprecation")
                	ServiceInfo si = ev.getInfo();
                	String server = si.getName();
					String display ="Name:" + server;
					String host = si.getHostAddress();
					int port =  si.getPort();
					display +="\nHost Adddress:" + si.getHostAddress() + " \nport:" + si.getPort();
                    Enumeration<String> keys = si.getPropertyNames();
                    String key = null;
                    String value = null;
                    
                    while(keys.hasMoreElements()){
                    	key = keys.nextElement();
                    	value =  si.getPropertyString(key);
                    	display +="\n"+key+":"+value;
                    }
                    notifyUser(display);
                    //SETUP WS URL ANDLink to WS
                    boolean isConnected = false;
                    SharedPreferences prefs = getSharedPreferences(
            				Constant.PREFS_NAME, Context.MODE_PRIVATE);
                    String wsLocation = prefs.getString(Constant.PREF_STATION_WEB_SOCKET_URL, "");
                    if( server.equals("ben-MBP2") && (TextUtils.isEmpty(wsLocation) || RuntimePlayer.OnWebSocketOpened == false )){
                    	wsLocation = "ws://"+host+":"+port;
                    	prefs.edit().putString(Constant.PREF_STATION_WEB_SOCKET_URL, wsLocation).commit();
            			if(RuntimePlayer.OnWebSocketOpened == false){
            				RuntimeWebClient.init(DnssdDiscovery.this);
            				RuntimeWebClient.setURL(wsLocation);
            				try {
            					RuntimeWebClient.open();
            					//send connect cmd
            					isConnected = true;
            				} catch (WebSocketException e) {
            					isConnected = false;
            					e.printStackTrace();
            				}
            				finally{
            					if(isConnected){
            						RuntimePlayer.OnWebSocketOpened = true;
            						notifyUser("Connected To "+si.getName());
//            						sendFile(DnssdDiscovery.this,Constant.TYPE_AUDIO);
            						sendFile(DnssdDiscovery.this,Constant.TYPE_VIDEO);
//            						sendFile(DnssdDiscovery.this,Constant.TYPE_IMAGE);
            						Intent intent = new Intent(Constant.ACTION_BACKUP_FILE);
            						sendBroadcast(intent);
            					}
            				}
            			}
                    }
                }

                @Override
                public void serviceRemoved(ServiceEvent ev) {
                    notifyUser("Service removed: " + ev.getName());
                }

                @Override
                public void serviceAdded(ServiceEvent event) {
                    jmdns.requestServiceInfo(event.getType(), event.getName(), 1);
                }
            });
        } catch (IOException e) {
            e.printStackTrace();
            return;
        }
    }

	public void sendFile(Context context,int type){
		String currentDate = "";
		String cursorDate = "";
		long refCursorDate = 0 ;
		long dateTaken = -1;
		long dateModified = 0;
		long dateAdded = 0;
		String fileSize = null;
		String folderName = null;
		String mediaData = null;
		String[] projection = null;
		String selection =  null;
		String selectionArgs[] = { currentDate };
		Cursor cursor = null;
		if(type == Constant.TYPE_IMAGE){//IMAGES
			projection = new String[]{
					MediaStore.Images.Media.DATA,
					MediaStore.Images.Media.DATE_TAKEN,
					MediaStore.Images.Media.DISPLAY_NAME,
					MediaStore.Images.Media.DATE_ADDED,
					MediaStore.Images.Media.DATE_MODIFIED,
					MediaStore.Images.Media.SIZE,
					MediaStore.Images.Media._ID};
			selection =  getImageSelection(context);
	
			Log.d(TAG, "selection => " + selection);
			cursor =context.getContentResolver().query(
					MediaStore.Images.Media.EXTERNAL_CONTENT_URI, projection,
					selection, selectionArgs,
					MediaStore.Images.Media.DATE_TAKEN+" DESC LIMIT 1");
		}
		else if(type == Constant.TYPE_VIDEO){//VIDEO
		projection = new String[]{
				MediaStore.Video.Media.DATA,
				MediaStore.Video.Media.DATE_TAKEN,
				MediaStore.Video.Media.DISPLAY_NAME,
				MediaStore.Video.Media.DATE_ADDED,
				MediaStore.Video.Media.DATE_MODIFIED,
				MediaStore.Video.Media.SIZE,
				MediaStore.Video.Media._ID};

		selection =  getVideoSelection(context);
		Log.d(TAG, "selection => " + selection);
		cursor =context.getContentResolver().query(
				MediaStore.Video.Media.EXTERNAL_CONTENT_URI, projection,
				selection, selectionArgs,
				MediaStore.Video.Media.DATE_TAKEN+" DESC LIMIT 1");
		}
		else if(type == Constant.TYPE_AUDIO){//AUDIO
			projection = new String[]{
					MediaStore.Audio.Media.DATA,
					MediaStore.Audio.Media.DISPLAY_NAME,
					MediaStore.Audio.Media.ALBUM,
					MediaStore.Audio.Media.DATE_ADDED,
					MediaStore.Audio.Media.DATE_MODIFIED,
					MediaStore.Audio.Media.SIZE,
					MediaStore.Audio.Media._ID};
	
			selection =  getAudioSelection(context);
	
			Log.d(TAG, "selection => " + selection);
			cursor = context.getContentResolver().query(
					MediaStore.Audio.Media.EXTERNAL_CONTENT_URI, projection,
					selection, selectionArgs,
					MediaStore.Audio.Media.DATE_ADDED+" DESC LIMIT 1");
		}
		if(cursor!=null && cursor.getCount()>0){
			cursor.moveToFirst();
			int count = cursor.getCount();			
			for(int i = 0 ; i < count; i++){
				if(type == Constant.TYPE_IMAGE){
					mediaData = cursor.getString(cursor.getColumnIndex(MediaStore.Images.Media.DATA));
					dateTaken = cursor.getLong(cursor.getColumnIndex(MediaStore.Images.Media.DATE_TAKEN));
					dateModified = cursor.getLong(cursor.getColumnIndex(MediaStore.Images.Media.DATE_MODIFIED));
					dateAdded = cursor.getLong(cursor.getColumnIndex(MediaStore.Images.Media.DATE_ADDED));					
				}
				else if(type == Constant.TYPE_AUDIO){
					mediaData = cursor.getString(cursor.getColumnIndex(MediaStore.Audio.Media.DATA));
					dateModified = cursor.getLong(cursor.getColumnIndex(MediaStore.Audio.Media.DATE_MODIFIED));
					dateAdded = cursor.getLong(cursor.getColumnIndex(MediaStore.Audio.Media.DATE_ADDED));
				}
				else if(type == Constant.TYPE_VIDEO){
					mediaData = cursor.getString(cursor.getColumnIndex(MediaStore.Video.Media.DATA));
					dateTaken = cursor.getLong(cursor.getColumnIndex(MediaStore.Video.Media.DATE_TAKEN));
					dateModified = cursor.getLong(cursor.getColumnIndex(MediaStore.Video.Media.DATE_MODIFIED));
					dateAdded = cursor.getLong(cursor.getColumnIndex(MediaStore.Video.Media.DATE_ADDED));
				}
				fileSize = cursor.getString(5);
				cursor.getString(6);		
				folderName =  getFoldername(mediaData);
				
				if (dateTaken != -1) {
					refCursorDate = dateTaken / 1000;
				} else if (dateModified != -1) {
					refCursorDate = dateModified ;
				} else if (dateAdded != -1) {
					refCursorDate = dateAdded ;
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
	public static String getFoldername(String imageFullpath){
		if(!TextUtils.isEmpty(imageFullpath)){
			int lastIndex = imageFullpath.lastIndexOf(File.separator);
			int lastSecondIndex = imageFullpath.substring(0,lastIndex).lastIndexOf(File.separator);
			return imageFullpath.substring(lastSecondIndex+1, lastIndex);
		}
		else{
			return "";
		}
	}
    class SendFileTask extends AsyncTask<Void,Void,Void>{

		@Override
		protected Void doInBackground(Void... params) {			
			String foldername = null;
			String filename = null;
			String filesize = null;

			try {
				for(int i = 0 ; i < mFilenames.size();i++){
					foldername = mFolders.get(i);
					filename = mFilenames.get(i);
					filesize = mFilesizes.get(i);
				
					//send file index for start
					FIleTransferEntity entity = new FIleTransferEntity();
					entity.action = Constant.PARAM_FILEACTION_START;
					entity.folder = foldername;
					entity.fileName = StringUtil.getFilename(filename);
					entity.fileSize = filesize;
					String jsonOuput = RuntimePlayer.GSON.toJson(entity);
					RuntimeWebClient.setDefaultFormat();
					RuntimeWebClient.send(jsonOuput);
					try {
						Thread.sleep(1000);
					} catch (InterruptedException e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					}
					//send file binary
	//				byte[] data = null;
	//				Bitmap bm = BitmapFactory.decodeFile(filename);
	//				ByteArrayOutputStream stream = new ByteArrayOutputStream();
	//				if (bm != null) {
	//					data = stream.toByteArray();
	//				}
					
	//		        ByteArrayOutputStream ous = null;
			        InputStream ios = null;
			        RuntimeWebClient.setBinaryFormat();
			        try {
//			            byte[] buffer = new byte[128*BASE_BYTES];
			            byte[] buffer = new byte[64*BASE_BYTES];			            
			            byte[] finalBuffer = null;
	//		            
			            ios = new FileInputStream(new File(filename));
			            int read = 0;
			            while ( (read = ios.read(buffer)) != -1 ) {
			            	if(read != buffer.length){
			            		finalBuffer = new byte[read];
			            		finalBuffer = Arrays.copyOf(buffer, read);
			            		RuntimeWebClient.sendFile(finalBuffer);
			            	}
			            	else{
			            		RuntimeWebClient.sendFile(buffer);
			            	}
	//						RuntimeWebClient.sendFile("123", buffer, entity.fileName, "123");		            	
			            }
			        } catch (FileNotFoundException e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					} catch (IOException e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					} finally { 
	//		            try {
	//		                 if ( ous != null ) 
	//		                     ous.close();
	//		            } catch ( IOException e) {
	//		            }
	
			            try {
			                 if ( ios != null ) 
			                      ios.close();
			            } catch ( IOException e) {
			            }
			        }				
					//send file index for end
			        RuntimeWebClient.setDefaultFormat();
			        entity.action = Constant.PARAM_FILEACTION_END;
					jsonOuput = RuntimePlayer.GSON.toJson(entity);
					RuntimeWebClient.send(jsonOuput);
				}

			} catch (WebSocketException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			return null;
		}	
    }
    
    public byte[] read(File file) throws IOException {

        ByteArrayOutputStream ous = null;
        InputStream ios = null;
        try {
            byte[] buffer = new byte[4096];
            ous = new ByteArrayOutputStream();
            ios = new FileInputStream(file);
            int read = 0;
            while ( (read = ios.read(buffer)) != -1 ) {
                ous.write(buffer, 0, read);
            }
        } finally { 
            try {
                 if ( ous != null ) 
                     ous.close();
            } catch ( IOException e) {
            }

            try {
                 if ( ios != null ) 
                      ios.close();
            } catch ( IOException e) {
            }
        }
        return ous.toByteArray();
    }
	public static String getImageSelection(Context context){
		String selection = "(" + MediaStore.Images.Media.DATE_ADDED + " <= ? ";
		//IMPORT ALL
		selection += " AND "+ MediaStore.Images.Media.DISPLAY_NAME
				+" NOT LIKE '%.jps%' )";
		return selection;
	}
	public static String getVideoSelection(Context context){
		String selection = MediaStore.Video.Media.DATE_ADDED + " <= ? ";
		//IMPORT ALL
//		selection += " AND "+ MediaStore.Video.Media.DISPLAY_NAME
//				+" NOT LIKE '%.jps%' )";
		return selection;
	}
	public static String getAudioSelection(Context context){
		String selection = MediaStore.Audio.Media.DATE_ADDED + " <= ? ";
		//IMPORT ALL
//		selection += " AND "+ MediaStore.Video.Media.DISPLAY_NAME
//				+" NOT LIKE '%.jps%' )";
		return selection;
	}
	public static String getFilesSelection(Context context){
		String selection = MediaStore.Files.FileColumns.DATE_ADDED + " <= ? ";
		//IMPORT ALL
//		selection += " AND "+ MediaStore.Video.Media.DISPLAY_NAME
//				+" NOT LIKE '%.jps%' )";
		return selection;
	}

    private void notifyUser(final String msg) {
        handler.postDelayed(new Runnable() {
            public void run() {

        TextView t = (TextView)findViewById(R.id.text);
        t.setText(msg+"\n=== "+t.getText());
            }
            }, 1);

    }

    @Override
        protected void onStart() {
        super.onStart();
        //new Thread(){public void run() {setUp();}}.start();
    }

    @SuppressLint("NewApi")
	@Override
        protected void onStop() {
    	if (jmdns != null) {
            if (listener != null) {
                jmdns.removeServiceListener(type, listener);
                listener = null;
            }
            jmdns.unregisterAllServices();
            try {
                jmdns.close();
            } catch (IOException e) {
                // TODO Auto-generated catch block
                e.printStackTrace();
            }
            jmdns = null;
    	}
    	//repo.stop();
        //s.stop();
        lock.release();
    	super.onStop();
    }
}