package com.waveface.exception;

public abstract class WammerException extends Exception {

	public WammerException() {

	}

	public WammerException(String paramString) {
		super(paramString);
	}

	public WammerException(String paramString, Throwable paramThrowable) {
		super(paramString, paramThrowable);
	}

	public WammerException(Throwable paramThrowable) {
		super(paramThrowable);
	}

}
