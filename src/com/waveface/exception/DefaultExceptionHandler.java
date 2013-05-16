package com.waveface.exception;

public class DefaultExceptionHandler implements Thread.UncaughtExceptionHandler {

	private Thread.UncaughtExceptionHandler defaultExceptionHandler;

	public DefaultExceptionHandler(
			Thread.UncaughtExceptionHandler paramUncaughtExceptionHandler) {
		this.defaultExceptionHandler = paramUncaughtExceptionHandler;
	}

	public void uncaughtException(Thread paramThread, Throwable paramThrowable) {
		this.defaultExceptionHandler.uncaughtException(paramThread,
				paramThrowable);
		// ExceptionHandler.outputException(paramThrowable);

	}
}
