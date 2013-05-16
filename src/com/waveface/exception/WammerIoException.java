package com.waveface.exception;

import java.io.IOException;

public class WammerIoException extends WammerException {

	public WammerIoException(IOException paramIOException) {
		super(paramIOException);
	}
}
