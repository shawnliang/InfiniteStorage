package com.waveface.favoriteplayer.websocket;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;
import java.util.concurrent.ScheduledThreadPoolExecutor;
import java.util.concurrent.TimeUnit;

import javolution.util.FastMap;

import org.jwebsocket.api.WebSocketClientEvent;
import org.jwebsocket.api.WebSocketClientListener;
import org.jwebsocket.api.WebSocketClientTokenListener;
import org.jwebsocket.api.WebSocketPacket;
import org.jwebsocket.api.WebSocketTokenClient;
import org.jwebsocket.client.java.ReliabilityOptions;
import org.jwebsocket.config.JWebSocketCommonConstants;
import org.jwebsocket.kit.WebSocketEncoding;
import org.jwebsocket.kit.WebSocketException;
import org.jwebsocket.kit.WebSocketSubProtocol;
import org.jwebsocket.token.PendingResponseQueueItem;
import org.jwebsocket.token.Token;
import org.jwebsocket.token.TokenFactory;
import org.jwebsocket.token.WebSocketResponseTokenListener;
import org.jwebsocket.util.Tools;

import android.annotation.SuppressLint;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.database.Cursor;
import android.text.TextUtils;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.RuntimeState;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.entity.HomeSharingEntity;
import com.waveface.favoriteplayer.entity.LabelEntity;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.event.LabelChangeEvent;

import com.waveface.favoriteplayer.logic.DownloadLogic;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.util.Log;
import com.waveface.favoriteplayer.util.NetworkUtil;
import com.waveface.service.HttpInvoker;
import com.waveface.sync.entity.LabelChangeEntity;

import de.greenrobot.event.EventBus;

public class WavefaceTokenClient extends WavefaceBaseWebSocketClient implements
		WebSocketTokenClient {
	private static final String TAG = WavefaceTokenClient.class.getSimpleName();
	/**
	 * base name space for jWebSocket
	 */
	private final static String NS_BASE = "com.waveface";

	/**
	 * token id
	 */
	private int CUR_TOKEN_ID = 0;
	/**
	 * sub protocol value
	 */
	public static WebSocketSubProtocol mSubProt = null;
	// private String mSubProt;
	// private WebSocketEncoding mEncoding;
	private String mUsername = null;
	private String fClientId = null;
	private final Map<Integer, PendingResponseQueueItem> mPendingResponseQueue = new FastMap<Integer, PendingResponseQueueItem>()
			.shared();
	private final ScheduledThreadPoolExecutor mResponseQueueExecutor = new ScheduledThreadPoolExecutor(
			1);
	private static Context mContext;

	/**
	 * Default constructor
	 */
	public WavefaceTokenClient(Context context) {
		this(JWebSocketCommonConstants.WS_SUBPROT_DEFAULT,
				JWebSocketCommonConstants.WS_ENCODING_DEFAULT);
		mContext = context;
	}

	/**
	 * 
	 * @param aReliabilityOptions
	 */
	public WavefaceTokenClient(ReliabilityOptions aReliabilityOptions) {
		this(JWebSocketCommonConstants.WS_SUBPROT_DEFAULT,
				JWebSocketCommonConstants.WS_ENCODING_DEFAULT);
		setReliabilityOptions(aReliabilityOptions);
	}

	/**
	 * 
	 * @param aSubProt
	 * @param aEncoding
	 */
	public WavefaceTokenClient(String aSubProt, WebSocketEncoding aEncoding) {
		mSubProt = new WebSocketSubProtocol(aSubProt, aEncoding);
		addSubProtocol(mSubProt);
		addListener(new TokenClientListener());
	}

	/**
	 * 
	 * @param aSubProt
	 */
	public WavefaceTokenClient(WebSocketSubProtocol aSubProt) {
		mSubProt = aSubProt;
		addSubProtocol(mSubProt);
		addListener(new TokenClientListener());
	}

	public void setSubProt(String aSubProt) {

		if (aSubProt.equals(JWebSocketCommonConstants.WS_SUBPROT_BINARY)) {
			changeNegotiatedSubProtocol(true);
		} else {
			changeNegotiatedSubProtocol(false);
		}

		mSubProt = new WebSocketSubProtocol(aSubProt,
				JWebSocketCommonConstants.WS_ENCODING_DEFAULT);
	}

	/**
	 * WebSocketClient listener implementation that receives the data packet and
	 * creates <tt>token</tt> objects
	 * 
	 * @author aschulze
	 */
	class TokenClientListener implements WebSocketClientListener {

		/**
		 * {@inheritDoc} Initialize all the variables when the process starts
		 */
		@Override
		public void processOpening(WebSocketClientEvent aEvent) {
		}

		/**
		 * {@inheritDoc} Initialize all the variables when the process starts
		 */
		@Override
		public void processOpened(WebSocketClientEvent aEvent) {
			RuntimeState.setServerStatus(Constant.WS_ACTION_SOCKET_OPENED);
		}

		@Override
		public void processPacket(WebSocketClientEvent aEvent,
				WebSocketPacket aPacket) {

			String jsonOutput = aPacket.getUTF8();
			Log.d(TAG, "WebSocket jsonOutput=" + jsonOutput);

			if (NetworkUtil.isWifiNetworkAvailable(mContext)) {
				SharedPreferences mPrefs = mContext.getSharedPreferences(
						Constant.PREFS_NAME, Context.MODE_PRIVATE);
				Editor mEditor = mPrefs.edit();
				LabelChangeEntity entity = 
						RuntimeState.GSON.fromJson(jsonOutput,
						LabelChangeEntity.class);

				boolean needToSync = true;
				
				int downloadLableInitStatus = mPrefs.getInt(
						Constant.PREF_DOWNLOAD_LABEL_INIT_STATUS, 0);

				if (entity.home_sharing != null) {

					mEditor.putString(Constant.PREF_HOME_SHARING_STATUS,
							entity.home_sharing);
					mEditor.commit();
				}
				String homeSharingStatus = mPrefs.getString(
						Constant.PREF_HOME_SHARING_STATUS, "");

				if (downloadLableInitStatus == 1
						&& homeSharingStatus.equals("true")) {
					try {
						if (entity != null) {
							Cursor cusor = LabelDB.getLabelByLabelId(
									mContext, entity.label_change.label_id);
							// label exist
							if (cusor !=null && cusor.getCount() > 0) {
								LabelDB.updateLabelServerSeq(mContext,
										entity.label_change.label_id,
										entity.label_change.seq);
								if (entity.label_change.deleted.equals("true")) {
//									LabelDB.deleteLabel(mContext,
//											entity.label_change.label_id);
//									LabelDB.removeAllFileInLabel(mContext,
//											entity.label_change.label_id);
									needToSync = false;
									LabelDB.updateLabeDisplayStatus(mContext,
											entity.label_change.label_id,"false");
								}
							}//new Label for insert 
							else {
									ArrayList<ServerEntity> servers = ServersLogic
											.getPairedServer(mContext);
									ServerEntity pairedServer = servers.get(0);
									String restfulAPIURL = "http://"
											+ pairedServer.ip + ":"
											+ pairedServer.restPort;
									String getLabelURL = restfulAPIURL
											+ Constant.URL_GET_LABEL;
									HashMap<String, String> param = new HashMap<String, String>();
									param.clear();
									param.put(Constant.PARAM_LABEL_ID,
											entity.label_change.label_id);
									jsonOutput = HttpInvoker.executePost(
											getLabelURL, param,
											Constant.STATION_CONNECTION_TIMEOUT,
											Constant.STATION_CONNECTION_TIMEOUT);
	
									LabelEntity.Label labelEntity = RuntimeState.GSON
											.fromJson(jsonOutput,
													LabelEntity.Label.class);
	
									labelEntity.cover_url = entity.label_change.cover_url;
									labelEntity.auto_type = entity.label_change.auto_type;
									labelEntity.on_air = entity.label_change.on_air;
									labelEntity.deleted = entity.label_change.deleted;
	
									LabelDB.updateLabelChang(mContext, labelEntity,
											true);
								if (entity.label_change.deleted.equals("true")) {
									needToSync = false;
								}
							}
							cusor.close();
							if(needToSync){
								mContext.sendBroadcast(new Intent(
										Constant.ACTION_LABEL_CHANGE_NOTIFICATION));
							}
						}
					} catch (Exception e) {
						e.printStackTrace();
					}
				}
			}
			// ORIGINAL Web Socket Code
			Token lToken = packetToToken(aPacket);
			// Notifying listeners
			for (WebSocketClientListener lListener : getListeners()) {
				if (lListener instanceof WebSocketClientTokenListener) {
					((WebSocketClientTokenListener) lListener).processToken(
							aEvent, lToken);
				}
			}
		}

		/**
		 * {@inheritDoc}
		 */
		@Override
		public void processClosed(WebSocketClientEvent aEvent) {
			RuntimeState.setServerStatus(Constant.WS_ACTION_SOCKET_CLOSED);
			ServersLogic.updateAllBackedServerStatus(mContext,
					Constant.SERVER_OFFLINE);
			Intent intent = new Intent(
					Constant.ACTION_WEB_SOCKET_SERVER_DISCONNECTED);
			mContext.sendBroadcast(intent);
		}

		/**
		 * {@inheritDoc}
		 */
		@Override
		public void processReconnecting(WebSocketClientEvent aEvent) {
		}
	}

	@Override
	public void addTokenClientListener(
			WebSocketClientTokenListener aTokenListener) {
		super.addListener(aTokenListener);
	}

	@Override
	public void removeTokenClientListener(
			WebSocketClientTokenListener aTokenListener) {
		super.removeListener(aTokenListener);
	}

	/**
	 * {@
	 */
	@Override
	public void close() {
		super.close();
	}

	/**
	 * @return the fUsername
	 */
	@Override
	public String getUsername() {
		return mUsername;
	}

	public void setUsername(String aUsername) {
		this.mUsername = aUsername;
	}

	@Override
	public boolean isAuthenticated() {
		return (mUsername != null);
	}

	/**
	 * @return the fClientId
	 */
	public String getClientId() {
		return fClientId;
	}

	/**
	 * 
	 * @param aPacket
	 * @return
	 */
	public Token packetToToken(WebSocketPacket aPacket) {
		Token lToken = null;
		if (JWebSocketCommonConstants.WS_FORMAT_JSON.equals(mSubProt
				.getFormat())) {
			lToken = JSONProcessor.packetToToken(aPacket);
		} else if (JWebSocketCommonConstants.WS_FORMAT_BINARY.equals(mSubProt
				.getFormat())) {
			lToken = FileProcessor.packetToToken(aPacket);
		}
		return lToken;
	}

	/**
	 * 
	 * @param aToken
	 * @return
	 */
	public WebSocketPacket tokenToPacket(Token aToken) {
		WebSocketPacket lPacket = null;

		if (JWebSocketCommonConstants.WS_FORMAT_JSON.equals(mSubProt
				.getFormat())) {
			lPacket = JSONProcessor.tokenToPacket(aToken);
		} else if (JWebSocketCommonConstants.WS_FORMAT_BINARY.equals(mSubProt
				.getFormat())) {
			lPacket = FileProcessor.tokenToPacket(aToken);
		}
		return lPacket;
	}

	/**
	 * 
	 * @param aToken
	 * @throws WebSocketException
	 */
	public void sendToken(Token aToken) throws WebSocketException {
		CUR_TOKEN_ID++;
		aToken.setInteger("utid", CUR_TOKEN_ID);
		super.send(tokenToPacket(aToken));
	}

	private class ResponseTimeoutTimer implements Runnable {

		private Integer mUTID = 0;

		public ResponseTimeoutTimer(Integer aUTID) {
			mUTID = aUTID;
		}

		@Override
		public void run() {
			synchronized (mPendingResponseQueue) {
				PendingResponseQueueItem lPRQI = (mUTID != null ? mPendingResponseQueue
						.get(mUTID) : null);
				if (lPRQI != null) {
					// if so start analyzing
					WebSocketResponseTokenListener lWSRTL = lPRQI.getListener();
					if (lWSRTL != null) {
						// fire on response
						lWSRTL.OnTimeout(lPRQI.getToken());
					}
					// and drop the pending queue item
					mPendingResponseQueue.remove(mUTID);
				}
			}
		}
	}

	/**
	 * 
	 * @param aToken
	 * @param aResponseListener
	 * @throws WebSocketException
	 */
	public void sendToken(Token aToken,
			WebSocketResponseTokenListener aResponseListener)
			throws WebSocketException {
		PendingResponseQueueItem lPRQI = new PendingResponseQueueItem(aToken,
				aResponseListener);
		int lUTID = CUR_TOKEN_ID + 1;
		mPendingResponseQueue.put(lUTID, lPRQI);
		ResponseTimeoutTimer lRTT = new ResponseTimeoutTimer(lUTID);
		mResponseQueueExecutor.schedule(lRTT, aResponseListener.getTimeout(),
				TimeUnit.MILLISECONDS);
		sendToken(aToken);
	}

	private final static String NS_SYSTEM_PLUGIN = NS_BASE + ".plugins.system";

	@Override
	public void login(String aUsername, String aPassword)
			throws WebSocketException {

	}

	@Override
	public void logout() throws WebSocketException {
	}

	@Override
	public void ping(boolean aEcho) throws WebSocketException {
	}

	@Override
	public void sendText(String aTarget, String aData)
			throws WebSocketException {
		Token lToken = TokenFactory.createToken(NS_SYSTEM_PLUGIN, "send");
		lToken.setString("targetId", aTarget);
		lToken.setString("sourceId", getClientId());
		lToken.setString("sender", getUsername());
		lToken.setString("data", aData);
		sendToken(lToken);
	}

	@Override
	public void broadcastText(String aData) throws WebSocketException {
		Token lToken = TokenFactory.createToken(NS_SYSTEM_PLUGIN, "broadcast");
		lToken.setString("sourceId", getClientId());
		lToken.setString("sender", getUsername());
		lToken.setString("data", aData);
		lToken.setBoolean("senderIncluded", false);
		lToken.setBoolean("responseRequested", true);
		sendToken(lToken);
	}

	private final static String NS_FILESYSTEM_PLUGIN = NS_BASE
			+ ".plugins.filesystem";

	// @Override
	/**
	 * 
	 * @param aData
	 * @param aFilename
	 * @param aScope
	 * @param aNotify
	 * @throws WebSocketException
	 */
	public void saveFile(byte[] aData, String aFilename, String aScope,
			Boolean aNotify) throws WebSocketException {
		Token lToken = TokenFactory.createToken(NS_FILESYSTEM_PLUGIN, "save");
		lToken.setString("sourceId", getClientId());
		lToken.setString("sender", getUsername());
		lToken.setString("filename", aFilename);
		// TODO: set mimetype correctly according to file extension based on
		// configuration in jWebSocket.xml
		lToken.setString("mimetype", "image/jpeg");
		lToken.setString("scope", aScope);
		lToken.setBoolean("notify", aNotify);

		// lToken.setString("data", Base64.encodeBase64String(aData));
		lToken.setString("data", Tools.base64Encode(aData));
		sendToken(lToken);
	}

	/**
	 * 
	 * @param aHeader
	 * @param aData
	 * @param aFilename
	 * @param aTarget
	 * @throws WebSocketException
	 */
	public void sendFile(String aHeader, byte[] aData, String aFilename,
			String aTarget) throws WebSocketException {

		// Token lToken = TokenFactory.createToken(NS_FILESYSTEM_PLUGIN,
		// "send");
		// lToken.setString("sourceId", "");
		// lToken.setString("sender", "");
		// lToken.setString("filename", aFilename);
		// // TODO: set mimetype correctly according to file extension based on
		// configuration in jWebSocket.xml
		// lToken.setString("mimetype", "image/jpeg");
		// lToken.setString("unid", aTarget);
		// int flags = Base64.NO_WRAP | Base64.URL_SAFE;
		// lToken.setString("data", Base64.encodeToString(aData, flags));
		// lToken.setString("data", Base64.encodeToString(aData, flags));
		// sendToken(lToken);
		// WebSocketPacket lPacket = new RawPacket(aData);
		super.send(aData);
	}

	/*
	 * functions of the Admin Plug-in
	 */
	private final static String NS_ADMIN_PLUGIN = NS_BASE + ".plugins.admin";

	@Override
	public void disconnect() throws WebSocketException {
	}

	/**
	 * 
	 * @throws WebSocketException
	 */
	public void shutdown() throws WebSocketException {
		Token lToken = TokenFactory.createToken(NS_ADMIN_PLUGIN, "shutdown");
		sendToken(lToken);
	}

	@Override
	public void getConnections() throws WebSocketException {
		Token lToken = TokenFactory.createToken(NS_ADMIN_PLUGIN,
				"getConnections");
		sendToken(lToken);
	}
}
