#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using log4net;

#endregion

namespace Wammer.Station
{
	#region IHttpHandler

	public interface IHttpHandler : ICloneable
	{
		void HandleRequest(HttpListenerRequest request, HttpListenerResponse response);
		void HandleRequest();
		void SetBeginTimestamp(long beginTime);
		event EventHandler<HttpHandlerEventArgs> ProcessSucceeded;
	}

	#endregion

	#region HttpServer

	public class HttpServer : IDisposable
	{
		#region Var

		private IHttpHandler m_defaultHandler;
		private Dictionary<string, IHttpHandler> m_handlers;
		private HttpListener m_listener;
		private Object m_lockSwitchObj;
		private ILog m_logger;
		private bool m_started;
		private int m_port;
		private bool m_disposed;

		#endregion

		#region Private Property

		private ILog Logger
		{
			get { return m_logger ?? (m_logger = LogManager.GetLogger("HttpServer")); }
		}

		private object LockSwitchObj
		{
			get { return m_lockSwitchObj ?? (m_lockSwitchObj = new object()); }
		}

		private HttpListener Listener
		{
			get { return m_listener ?? (m_listener = new HttpListener()); }
		}

		public Dictionary<string, IHttpHandler> Handlers
		{
			get { return m_handlers ?? (m_handlers = new Dictionary<string, IHttpHandler>()); }
		}

		#endregion

		public HttpServer(int port)
		{
			m_port = port;
		}

		~HttpServer()
		{
			Dispose(false);
		}

		#region Private Method

		private void respond404NotFound(HttpListenerContext ctx)
		{
			try
			{
				ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
				ctx.Response.Close();
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to respond 404 Not Found", e);
			}
		}

		private IHttpHandler FindBestMatch(string requestAbsPath)
		{
			string path = requestAbsPath;

			if (!path.EndsWith("/"))
				path += "/";

			int maxMatchLen = 0;
			IHttpHandler handler = null;

			foreach (var pair in Handlers)
			{
				if (path.StartsWith(pair.Key))
				{
					var matchLen = pair.Key.Length;

					if (matchLen > maxMatchLen)
					{
						maxMatchLen = matchLen;
						handler = pair.Value;
					}
				}
			}

			return handler ?? m_defaultHandler;
		}

		#endregion

		#region Protected Method

		protected virtual void Dispose(bool disposing)
		{
			if (m_disposed)
				return;

			Close();

			m_disposed = true;
		}

		#endregion

		#region Public Method

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Adds the handler.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="handler">The handler.</param>
		public void AddHandler(string path, IHttpHandler handler)
		{
			if (string.IsNullOrEmpty(path))
				throw new ArgumentNullException("path");

			if (handler == null)
				throw new ArgumentNullException("handler");

			string absPath = null;
			string urlPrefix = "http://+:" + m_port.ToString();

			if (path.StartsWith("/"))
			{
				urlPrefix += path;
				absPath = path;
			}
			else
			{
				urlPrefix += "/" + path;
				absPath = "/";
			}

			if (!path.EndsWith("/"))
			{
				urlPrefix += "/";
				absPath += "/";
			}

			if (Handlers.ContainsKey(absPath))
				return;

			Handlers.Add(absPath, handler);
			Listener.Prefixes.Add(urlPrefix);
		}

		/// <summary>
		/// Adds the default handler.
		/// </summary>
		/// <param name="handler">The handler.</param>
		public void AddDefaultHandler(IHttpHandler handler)
		{
			if (handler == null)
				throw new ArgumentNullException();

			m_defaultHandler = handler;
		}

		/// <summary>
		/// Starts this instance.
		/// </summary>
		public void Start()
		{
			lock (LockSwitchObj)
			{
				if (m_started)
					return;

				Listener.Start();
				m_started = true;
				Listener.BeginGetContext(ConnectionAccepted, null);
			}
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		public void Stop()
		{
			lock (LockSwitchObj)
			{
				if (m_started)
				{
					Listener.Stop();
					m_started = false;
				}
			}
		}

		/// <summary>
		/// Closes this instance.
		/// </summary>
		public void Close()
		{
			Stop();
			Listener.Close();
		}

		/// <summary>
		/// Connections the accepted.
		/// </summary>
		/// <param name="result">The result.</param>
		private void ConnectionAccepted(IAsyncResult result)
		{
			try
			{
				HttpListenerContext context = Listener.EndGetContext(result);

				if (context == null)
					return;

				long beginTime = Stopwatch.GetTimestamp();

				Listener.BeginGetContext(ConnectionAccepted, null);

				IHttpHandler handler = FindBestMatch(context.Request.Url.AbsolutePath);

				if (handler == null)
				{
					respond404NotFound(context);
					return;
				}

				HttpHandlingTask task = new HttpHandlingTask(handler, context, beginTime);

				ThreadPool.QueueUserWorkItem(x => task.Execute());
			}
			catch (ObjectDisposedException)
			{
				Logger.Warn("Http server disposed. Shutdown server");
			}
			catch (Exception e)
			{
				Logger.Warn("Shutdown server", e);
			}
		}

		#endregion
	}

	#endregion

	#region ITask

	internal interface ITask
	{
		void Execute();
	}

	#endregion

	#region HttpHandlingTask

	internal class HttpHandlingTask : ITask
	{
		private static readonly ILog s_logger = LogManager.GetLogger("HttpHandler");

		private readonly long m_beginTime;
		private readonly HttpListenerContext m_context;
		private readonly IHttpHandler m_handler;

		public HttpHandlingTask(IHttpHandler handler, HttpListenerContext context, long beginTime)
		{
			m_handler = (IHttpHandler)handler.Clone();
			m_context = context;
			m_beginTime = beginTime;
		}

		#region ITask

		public void Execute()
		{
			Action action = () =>
								{
									m_handler.SetBeginTimestamp(m_beginTime);
									m_handler.HandleRequest(m_context.Request, m_context.Response);
								};

			HandleRequestWithinExceptionHandler(action, m_context.Response);
		}

		public static void HandleRequestWithinExceptionHandler(Action action, HttpListenerResponse response)
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
				s_logger.Warn(e.ToString());

				response.StatusCode = (int)HttpStatusCode.BadRequest;
				response.ContentType = "application/json";

				using (var output = new StreamWriter(response.OutputStream))
				{
					output.WriteLine(JsonConvert.SerializeObject(
						new
							{
								api_ret_code = 400,
								api_ret_message = e.Message,
								status = (int)HttpStatusCode.BadRequest
							}));
				}
			}
		}

		#endregion
	}

	#endregion
}