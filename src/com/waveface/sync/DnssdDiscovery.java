package com.waveface.sync;

import java.io.IOException;
import java.util.Enumeration;

import javax.jmdns.JmDNS;
import javax.jmdns.ServiceEvent;
import javax.jmdns.ServiceInfo;
import javax.jmdns.ServiceListener;

import org.jwebsocket.kit.WebSocketException;

import android.annotation.SuppressLint;
import android.annotation.TargetApi;
import android.app.Activity;
import android.content.Context;
import android.content.SharedPreferences;
import android.os.Build;
import android.os.Bundle;
import android.text.TextUtils;
import android.widget.TextView;

import com.waveface.sync.websocket.RuntimeWebClient;


public class DnssdDiscovery extends Activity {
	private String TAG = DnssdDiscovery.class.getSimpleName();
    android.net.wifi.WifiManager.MulticastLock lock;
    android.os.Handler handler = new android.os.Handler();

    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.discover);

        handler.postDelayed(new Runnable() {
            public void run() {
                setUp();
            }
            }, 1000);

    }    /** Called when the activity is first created. */


    private String type = "_infinite-storage._tcp.local.";
    private JmDNS jmdns = null;
    private ServiceListener listener = null;
    private ServiceInfo serviceInfo;
    @TargetApi(Build.VERSION_CODES.DONUT)
	@SuppressLint("NewApi")
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
					String display ="Name:" + si.getName();
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
                    if(TextUtils.isEmpty(wsLocation) || RuntimePlayer.OnWebSocketOpened == false ){
                    	wsLocation = "ws://"+host+"/"+port;
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
                    // Required to force serviceResolved to be called again (after the first search)
                    jmdns.requestServiceInfo(event.getType(), event.getName(), 1);
                }
            });
//            serviceInfo = ServiceInfo.create("_infinite-storage._tcp.local.", "Android from Lm ", 12345, "version=1.0:name=cool");
//            jmdns.registerService(serviceInfo);
        } catch (IOException e) {
            e.printStackTrace();
            return;
        }
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