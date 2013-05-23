package com.waveface.exception;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.StatusLine;



public class WammerServerException extends WammerException {

	public static final int BAD_REQUEST = 400;
	public static final int FORBIDDEN = 403;
	public static final int INSUFFICIENT_STORAGE = 507;
	public static final int INTERNAL_SERVER_ERROR = 500;
	public static final int LENGTH_REQUIRED = 411;
	public static final int METHOD_NOT_ALLOWED = 405;
	public static final int NOT_ACCEPTABLE = 406;
	public static final int NOT_FOUND = 404;
	public static final int NOT_IMPLEMENTED = 501;
	public static final int NOT_MODIFIED = 304;
	public static final int SERVICE_UNAVAILABLE = 503;
	public static final int UNAUTHORIZED = 401;
	private static final long serialVersionUID = 1L;
	public String httpBody;
	public int httpError;
	public String httpReason;
	public String ExceptionType = "";

	public WammerServerException(HttpResponse paramHttpResponse) {
		fillInStackTrace();
		if (paramHttpResponse != null) {
			StatusLine localStatusLine = paramHttpResponse.getStatusLine();
			this.httpError = localStatusLine.getStatusCode();
			this.httpReason = localStatusLine.getReasonPhrase();
			this.httpBody = readResponse(paramHttpResponse);
		}
		
	}

	public WammerServerException(HttpResponse paramHttpResponse,
			Exception exception) {
		fillInStackTrace();
		if (paramHttpResponse != null) {
			StatusLine localStatusLine = paramHttpResponse.getStatusLine();
			this.httpError = localStatusLine.getStatusCode();
			this.httpReason = localStatusLine.getReasonPhrase();
			this.httpBody = readResponse(paramHttpResponse);
		}

	}

	public String readResponse(HttpResponse paramHttpResponse) {
		StringBuffer sb = new StringBuffer();
		HttpEntity entity = paramHttpResponse.getEntity();
		if (entity != null) {
			try {
				BufferedReader BUF = new BufferedReader(new InputStreamReader(
						entity.getContent()));
				String str = null;
				while ((str = BUF.readLine()) != null) {
					entity.consumeContent();
					sb.append(str);
				}
			} catch (IllegalStateException e) {
				e.printStackTrace();
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
		return sb.toString();
	}

	public boolean isServerError() {
		if ((this.httpError >= 500) && (this.httpError < 600)) {
			return true;
		} else {
			return false;
		}
	}

	@Override
	public String toString() {
		StringBuilder sb = new StringBuilder("WammerServerException: ");
		sb.append(this.httpError).append(" ");
		sb.append(this.httpReason).append(" ");
		sb.append(this.httpBody);
		return sb.toString();
	}

}
