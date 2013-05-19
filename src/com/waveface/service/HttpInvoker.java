package com.waveface.service;

import java.io.BufferedReader;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.URL;
import java.net.URLConnection;
import java.security.KeyStore;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.zip.GZIPInputStream;

import org.apache.http.Header;
import org.apache.http.HeaderElement;
import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.HttpStatus;
import org.apache.http.HttpVersion;
import org.apache.http.NameValuePair;
import org.apache.http.client.HttpClient;
import org.apache.http.client.entity.UrlEncodedFormEntity;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.conn.ClientConnectionManager;
import org.apache.http.conn.params.ConnManagerPNames;
import org.apache.http.conn.params.ConnPerRouteBean;
import org.apache.http.conn.scheme.PlainSocketFactory;
import org.apache.http.conn.scheme.Scheme;
import org.apache.http.conn.scheme.SchemeRegistry;
import org.apache.http.conn.ssl.SSLSocketFactory;
import org.apache.http.entity.BufferedHttpEntity;
import org.apache.http.entity.mime.HttpMultipartMode;
import org.apache.http.entity.mime.MultipartEntity;
import org.apache.http.entity.mime.content.ByteArrayBody;
import org.apache.http.entity.mime.content.InputStreamBody;
import org.apache.http.entity.mime.content.StringBody;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.impl.conn.tsccm.ThreadSafeClientConnManager;
import org.apache.http.message.BasicNameValuePair;
import org.apache.http.params.BasicHttpParams;
import org.apache.http.params.CoreProtocolPNames;
import org.apache.http.params.HttpConnectionParams;
import org.apache.http.params.HttpParams;
import org.apache.http.params.HttpProtocolParams;
import org.apache.http.protocol.HTTP;
import org.apache.http.util.EntityUtils;
import org.apache.james.mime4j.util.CharsetUtil;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.NetworkInfo.State;
import android.net.wifi.SupplicantState;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;

import com.waveface.exception.WammerServerException;
import com.waveface.favoriteplayer.util.Log;


public class HttpInvoker {
	private static final String TAG = HttpInvoker.class.getSimpleName();
	public static String httpResponseString;
	private static DefaultHttpClient mClient;
	private static DefaultHttpClient mImageClient;
	private static final String HEADER_ACCEPT_ENCODING = "Accept-encoding";
	private static final String HEADER_WAVEFACE_STREAM = "Waveface-Stream";
	private static final String HEADER_CONNECTION = "Connection";
	private static final String ENCODING_GZIP = "gzip";
	private static final String KEEP_ALIVE = "Keep-Alive";
	private static final String CONECTION_API = "API";
	private static final String CONECTION_IMAGE = "IMAGE";

	private static final int mRetryCount = 1;



	public static InputStream retryStream(String url, int count)
			throws WammerServerException {
		InputStream content = null;
		HttpResponse response = null;
		try {
			if (count < mRetryCount) {
				Log.d(TAG, "URL(time:" + (count + 1) + "):" + url);
				HttpGet get = new HttpGet(url);
//				get.addHeader(HEADER_CONNECTION,KEEP_ALIVE);

				HttpClient httpclient = getHttpClient(0, 0, CONECTION_IMAGE,
						mImageClient);
				response = httpclient.execute(get);
				HttpEntity entity = response.getEntity();
				int statusCode = response.getStatusLine().getStatusCode();

				if (statusCode != HttpStatus.SC_BAD_REQUEST
						&& statusCode != HttpStatus.SC_NOT_FOUND) {
					content = new BufferedHttpEntity(entity).getContent();
				} else if (statusCode == HttpStatus.SC_UNAUTHORIZED) {
					throw new WammerServerException(response);
				}
			}
		} catch (Exception e) {
			count++;
			if (count == mRetryCount) {
				Log.e(TAG, e.getMessage());
				throw new WammerServerException(response, e);
			} else {
				content = retryStream(url, count);
			}
		}
		return content;
	}



	public static String getStringFromUrl(String url, int connectionTimeout,
			int socketTimeout) throws WammerServerException {
		String content = null;
		content = retry(url, 0, connectionTimeout, socketTimeout);
		return content;
	}

	public static final int CLOUD_CONNECTION_TIMEOUT = 20000;
	public static final int CLOUD_SOCKET_TIMEOUT = 20000;
	
	/**
	 *
	 * @param url
	 * @return
	 */
	public static String getStringFromUrl(String url)
			throws WammerServerException {
		String content = null;
		content = retry(url, 0, CLOUD_CONNECTION_TIMEOUT,
				CLOUD_SOCKET_TIMEOUT);
		return content;
	}

	public static String retry(String url, int count, int connectionTimeout,
			int socketTimout) throws WammerServerException {
		String jsonString = "";
		int countLimit = 3;
		if (count < countLimit) {
			//Log.d(TAG, "URL(time:" + (count + 1) + "):" + url);
			HttpGet get = new HttpGet(url);

			HttpResponse response = null;
			try {
				DefaultHttpClient httpclient = getHttpClient(connectionTimeout,
						socketTimout, CONECTION_IMAGE, mImageClient);
				response = httpclient.execute(get);
				boolean isGzip = false;
				HttpEntity entity = response.getEntity();
				isGzip = isGZIP(entity);
				StringBuffer builder = null;
				if (isGzip) {
					builder = new StringBuffer(5120);
					BufferedReader reader = new BufferedReader(
							new InputStreamReader(new GZIPInputStream(
									entity.getContent())));
					String currentLine = reader.readLine();
					while (currentLine != null) {
						builder.append(currentLine);
						currentLine = reader.readLine();
					}
					reader.close();
				} else {
					builder = new StringBuffer();
					builder.append(EntityUtils.toString(entity, "UTF-8"));
				}
				jsonString = builder.toString();
				entity.consumeContent();
				builder = null;
			} catch (Exception e) {
				Log.e(TAG, e.getLocalizedMessage());
				throw new WammerServerException(response, e);
			}
			catch (OutOfMemoryError err) {
				err.printStackTrace();
				Log.e(TAG, err.getLocalizedMessage());
			}
		}
		return jsonString;
	}

	public static boolean isGZIP(HttpEntity entity) {
		boolean isGzip = false;
		Header ceheader = entity.getContentEncoding();
		if (ceheader != null) {
			HeaderElement[] codecs = ceheader.getElements();
			for (int i = 0; i < codecs.length; i++) {
				if (codecs[i].getName().equalsIgnoreCase(ENCODING_GZIP)) {
					isGzip = true;
					break;
				}
			}
		}
		return isGzip;
	}

	public static DefaultHttpClient getHttpClient(int connectionTimeout,
			int socketTimeout, String type, DefaultHttpClient client) {
		DefaultHttpClient returnClient = null;
		if (client == null) {
			// Setting up parameters
			HttpParams params = new BasicHttpParams();
			//params.setIntParameter(CoreConnectionPNames.SO_TIMEOUT, Constant.KEEP_ALIVE_MICRO_SEC);
			params.setParameter(ConnManagerPNames.MAX_TOTAL_CONNECTIONS, 50);
			params.setParameter(ConnManagerPNames.MAX_CONNECTIONS_PER_ROUTE,
					new ConnPerRouteBean(50));
			HttpProtocolParams.setVersion(params, HttpVersion.HTTP_1_1);
			HttpProtocolParams.setContentCharset(params, "UTF-8");
			params.setBooleanParameter(CoreProtocolPNames.USE_EXPECT_CONTINUE,
					false);

			// Setting timeout
			HttpConnectionParams
					.setConnectionTimeout(params, connectionTimeout);
			HttpConnectionParams.setSoTimeout(params, socketTimeout);
			try {
				KeyStore trustStore = KeyStore.getInstance(KeyStore
						.getDefaultType());
				trustStore.load(null, null);

				SSLSocketFactory sf = new CustomSSLSocketFactory(trustStore);
				sf.setHostnameVerifier(SSLSocketFactory.ALLOW_ALL_HOSTNAME_VERIFIER);

				// Registering schemes for both HTTP and HTTPS
				SchemeRegistry registry = new SchemeRegistry();
				registry.register(new Scheme("http", PlainSocketFactory
						.getSocketFactory(), 80));
				registry.register(new Scheme("https", sf, 443));

				// Creating thread safe client connection manager
				ClientConnectionManager ccm = new ThreadSafeClientConnManager(
						params, registry);

				// Creating HTTP client

				returnClient = new DefaultHttpClient(ccm, params);
			} catch (Exception e) {
				SchemeRegistry scheme = new SchemeRegistry();
				scheme.register(new Scheme("http", PlainSocketFactory
						.getSocketFactory(), 80));
				scheme.register(new Scheme("https", new EasySSLSocketFactory(),
						443));
				ClientConnectionManager ccm = new ThreadSafeClientConnManager(
						params, scheme);
				returnClient = new DefaultHttpClient(ccm, params);
			}
		} else {
			returnClient = client;
			if (type.equalsIgnoreCase(CONECTION_API)) {
				mClient = returnClient;
			} else if (type.equalsIgnoreCase(CONECTION_IMAGE)) {
				mImageClient = returnClient;
			}
		}

		return returnClient;
	}

	public static String executePostSync(String url,
			HashMap<String, String> params) throws WammerServerException {
		return executePostSync(url, params, 0, 0);
	}

	public static String executePostSync(String url,
			HashMap<String, String> params, int connectionTimeout,
			int socketTimeout) throws WammerServerException {
		DefaultHttpClient httpclient = getHttpClient(connectionTimeout,
				socketTimeout, CONECTION_API, mImageClient);
		HttpPost post = new HttpPost(url);

		String jsonString = null;
		UrlEncodedFormEntity form = null;
		HttpResponse response = null;
		try {
			// Add your data
			List<NameValuePair> nameValuePairs = new ArrayList<NameValuePair>(
					params.size());
			Iterator<String> iter = params.keySet().iterator();
			String key = null;
			String value = null;
			while (iter.hasNext()) {
				key = iter.next();
				if (params.get(key) == null) {
					value = "";
				} else {
					value = params.get(key);
				}
			
				nameValuePairs.add(new BasicNameValuePair(key, value));
			}
			form = new UrlEncodedFormEntity(nameValuePairs, HTTP.UTF_8);
			form.setContentEncoding(HTTP.UTF_8);
			post.setEntity(form);
			//Log.d(TAG, post.getURI().toString());
			// Execute HTTP Post Request
			response = httpclient.execute(post);

			int statusCode = response.getStatusLine().getStatusCode();
			if (statusCode == HttpStatus.SC_OK
					|| statusCode == HttpStatus.SC_MOVED_TEMPORARILY
					|| statusCode == HttpStatus.SC_BAD_REQUEST) {
				boolean isGzip = false;
				HttpEntity entity = response.getEntity();
				isGzip = isGZIP(entity);
				StringBuilder builder = null;
				if (isGzip) {
					builder = new StringBuilder(5120);

					BufferedReader reader = new BufferedReader(
							new InputStreamReader(new GZIPInputStream(
									entity.getContent())));
					String currentLine = reader.readLine();
					while (currentLine != null) {
						builder.append(currentLine);
						currentLine = reader.readLine();
					}
					reader.close();
				} else {
					builder = new StringBuilder(2048);
					builder.append(EntityUtils.toString(entity, "UTF-8"));
				}
				jsonString = builder.toString();
				entity.consumeContent();
				builder = null;
			} else if (statusCode == HttpStatus.SC_UNAUTHORIZED) {
				throw new WammerServerException(response);
			}
		} catch (Exception e) {
			//Log.e(TAG, e.getLocalizedMessage());
			throw new WammerServerException(response, e);
		}
		catch (OutOfMemoryError err) {
			err.printStackTrace();
			//Log.e(TAG, err.getLocalizedMessage());
		}
		return jsonString;
	}

	public static int executePost(String url,
			int connectionTimeout, int socketTimeout) {
		int statusCode = -1;
		DefaultHttpClient httpclient = getHttpClient(connectionTimeout,
				socketTimeout, CONECTION_API, mClient);
		HttpPost post = new HttpPost(url);
//		post.addHeader(HEADER_CONNECTION,KEEP_ALIVE);

		HttpResponse response = null;
		try {
			response = httpclient.execute(post);
			statusCode = response.getStatusLine().getStatusCode();
		} catch (Exception e) {
			Log.e(TAG, e.getLocalizedMessage());
		}
		return statusCode;
	}

	public static String executePost(String url,
			HashMap<String, String> params, int connectionTimeout,
			int socketTimeout) throws WammerServerException {
		DefaultHttpClient httpclient = getHttpClient(connectionTimeout,
				socketTimeout, CONECTION_API, mClient);
		HttpPost post = new HttpPost(url);

		String jsonString = null;
		UrlEncodedFormEntity form = null;
		HttpResponse response = null;
		try {
			// Add your data
			List<NameValuePair> nameValuePairs = new ArrayList<NameValuePair>(
					params.size());
			Iterator<String> iter = params.keySet().iterator();
			String key = null;
			String value = null;
			while (iter.hasNext()) {
				key = iter.next();
				if (params.get(key) == null) {
					value = "";
				} else {
					value = params.get(key);
				}
				//Log.d(TAG, key + "=" + value);
				nameValuePairs.add(new BasicNameValuePair(key, value));
			}
			form = new UrlEncodedFormEntity(nameValuePairs, HTTP.UTF_8);
			form.setContentEncoding(HTTP.UTF_8);
			post.setEntity(form);
			//Log.d(TAG, post.getURI().toString());
			// Execute HTTP Post Request
			response = httpclient.execute(post);

			int statusCode = response.getStatusLine().getStatusCode();
			if (statusCode == HttpStatus.SC_OK
					|| statusCode == HttpStatus.SC_MOVED_TEMPORARILY
					|| statusCode == HttpStatus.SC_BAD_REQUEST) {
				boolean isGzip = false;
				HttpEntity entity = response.getEntity();
				isGzip = isGZIP(entity);
				StringBuffer builder = null;
				if (isGzip) {
					builder = new StringBuffer(7280);
					BufferedReader reader = new BufferedReader(
							new InputStreamReader(new GZIPInputStream(
									entity.getContent())));
					String currentLine = reader.readLine();
					while (currentLine != null) {
						builder.append(currentLine);
						currentLine = reader.readLine();
					}
					reader.close();
				} else {
					builder = new StringBuffer();
					builder.append(EntityUtils.toString(entity, "UTF-8"));
				}
				jsonString = builder.toString();
				entity.consumeContent();
				builder = null;
			} else if (statusCode == HttpStatus.SC_UNAUTHORIZED) {
				throw new WammerServerException(response);
			}
		} catch (Exception e) {
			e.printStackTrace();
			//Log.e(TAG, e.getLocalizedMessage());
			throw new WammerServerException(response);
		}
		catch (OutOfMemoryError err) {
			err.printStackTrace();
			//Log.e(TAG, err.getLocalizedMessage());
		}
		return jsonString;
	}


}
