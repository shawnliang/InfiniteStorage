package com.waveface.sync.websocket;

import java.util.List;

import javolution.util.FastList;

import org.jwebsocket.api.WebSocketClientEvent;
import org.jwebsocket.api.WebSocketClientTokenListener;
import org.jwebsocket.api.WebSocketPacket;
import org.jwebsocket.kit.WebSocketException;
import org.jwebsocket.token.Token;

import android.content.Context;
import android.content.SharedPreferences;
import android.util.Log;

import com.waveface.sync.Constant;
import com.waveface.sync.RuntimePlayer;


public class RuntimeWebClient {
	private static final String TAG = RuntimeWebClient.class.getSimpleName();
	private static String mURL ;
	private static WavefaceTokenClient mWebSocketClient;
	private static Context mContext;
	private static List<WebSocketClientTokenListener> mListeners = new FastList<WebSocketClientTokenListener>();
	private static String DEF_ENCODING = "UTF-8";

	public static void init(Context context) {
		mContext = context;
		mWebSocketClient = new WavefaceTokenClient(mContext);
		mWebSocketClient.addListener(new Listener());
	}

	public static void loadSettings() {
		SharedPreferences prefs = mContext.getSharedPreferences(
				Constant.PREFS_NAME, Context.MODE_PRIVATE);
		mURL = prefs.getString(Constant.PREF_STATION_WEB_SOCKET_URL, "");
	}

	public static void open() throws WebSocketException {
		mWebSocketClient.open(mURL);
	}

	public static void close() throws WebSocketException {
		mWebSocketClient.close();
	}

	public static void send(String aString) throws WebSocketException {
		mWebSocketClient.send(aString, DEF_ENCODING);
	}

	public static void sendToken(Token aToken) throws WebSocketException {
		mWebSocketClient.sendToken(aToken);
	}

	public static void sendText(String aTarget, String aData) throws WebSocketException {
		mWebSocketClient.sendText(aTarget, aData);

	}

	public static void broadcastText(String aData) throws WebSocketException {
		mWebSocketClient.broadcastText(aData);
	}

	public static void saveFile(byte[] aData, String aFilename, String aScope,
			Boolean aNotify) throws WebSocketException {
		mWebSocketClient.saveFile(aData, aFilename, aScope, aNotify);
	}

	public static void sendFile(String aHeader, byte[] aData, String aFilename, String aTarget)
			throws WebSocketException {
		mWebSocketClient.sendFile(aHeader, aData, aFilename, aTarget);
	}

	public static void addListener(WebSocketClientTokenListener aListener) {
		mListeners.add(aListener);
	}

	public static void removeListener(WebSocketClientTokenListener aListener) {
		mListeners.remove(aListener);
	}

	public static void notifyOpened(WebSocketClientEvent aEvent) {
		for (WebSocketClientTokenListener lListener : mListeners) {
			lListener.processOpened(aEvent);
		}
	}

	public static void notifyPacket(WebSocketClientEvent aEvent, WebSocketPacket aPacket) {
		for (WebSocketClientTokenListener lListener : mListeners) {
			lListener.processPacket(aEvent, aPacket);
		}
	}

	public static void notifyToken(WebSocketClientEvent aEvent, Token aToken) {
		for (WebSocketClientTokenListener lListener : mListeners) {
			lListener.processToken(aEvent, aToken);
		}
	}

	public static void notifyClosed(WebSocketClientEvent aEvent) {
		for (WebSocketClientTokenListener lListener : mListeners) {
			lListener.processClosed(aEvent);
		}
	}

	public static boolean isConnected() {
		return mWebSocketClient.isConnected();
	}

	/**
	 * @return the URL
	 */
	public static String getURL() {
		return mURL;
	}

	/**
	 * @param aURL the URL to set
	 */
	public static void setURL(String aURL) {
		mURL = aURL;
	}

	/**
	 * @return the mJWC
	 */
	public static WavefaceTokenClient getClient() {
		return mWebSocketClient;
	}

	static class Listener implements WebSocketClientTokenListener {

		@Override
		public void processOpened(WebSocketClientEvent aEvent) {
			Log.d(TAG, "opened");
		}

		@Override
		public void processPacket(WebSocketClientEvent aEvent, WebSocketPacket aPacket) {
			Log.d(TAG, "processPacket");
		}

		@Override
		public void processToken(WebSocketClientEvent aEvent, Token aToken) {
			Log.d(TAG, "processToken");
		}

		@Override
		public void processClosed(WebSocketClientEvent aEvent) {
			Log.d(TAG, "processClosed");
			RuntimePlayer.OnWebSocketOpened = false;
		}

		@Override
		public void processOpening(WebSocketClientEvent aEvent) {
		}

		@Override
		public void processReconnecting(WebSocketClientEvent aEvent) {
		}
	}
}
