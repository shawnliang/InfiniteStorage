package com.waveface.favoriteplayer.entity;

import java.io.InputStream;

public class AttachmentViewResponse {
	private String json;
	private InputStream content;
	private long totalLength;
	private String url;

	public InputStream getContent() {
		return content;
	}

	public void setContent(InputStream is) {
		this.content = is;
	}

	private String mimetype;

	public String getMimetype() {
		return mimetype;
	}

	public void setMimetype(String mimetype) {
		this.mimetype = mimetype;
	}

	public String getJson() {
		return json;
	}

	public void setJson(String json) {
		this.json = json;
	}

	public long getTotalLength() {
		return totalLength;
	}

	public void setTotalLength(long totalLength) {
		this.totalLength = totalLength;
	}

	public String getUrl() {
		return url;
	}

	public void setUrl(String url) {
		this.url = url;
	}
}
