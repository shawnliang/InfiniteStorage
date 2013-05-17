package com.waveface.exception;

public class PrintingExceptionHandler implements ExceptionHandler {

	public void handleException(Throwable paramThrowable) {
		System.err.print(paramThrowable.getMessage());
	}
}
