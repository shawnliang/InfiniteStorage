using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;


namespace Wammer.Station
{
	public interface IHttpHandler : ICloneable
	{
		void HandleRequest(HttpListenerRequest request, HttpListenerResponse response);
		void HandleRequest();
		void SetBeginTimestamp(long beginTime);
		event EventHandler<HttpHandlerEventArgs> ProcessSucceeded;
	}

	public class HttpServer : IDisposable
	{
		#region Var
		private IHttpHandler _defaultHandler;
		private Dictionary<string, IHttpHandler> _handlers;
		private HttpListener _listener;
		private Object _lockSwitchObj;
		private ILog _logger;
		private bool _started;
		#endregion

		#region Private Property
		private bool m_Disposed { get; set; }

		private ILog m_Logger
		{
			get { return _logger ?? (_logger = LogManager.GetLogger("HttpServer")); }
		}

		private object m_LockSwitchObj
		{
			get { return _lockSwitchObj ?? (_lockSwitchObj = new object()); }
		}

		private int m_Port { get; set; }

		private HttpListener m_Listener
		{
			get { return _listener ?? (_listener = new HttpListener()); }
		}

		public Dictionary<string, IHttpHandler> m_Handlers
		{
			get { return _handlers ?? (_handlers = new Dictionary<string, IHttpHandler>()); }
		}

		#endregion

		#region Constructor

		public HttpServer(int port)
		{
			m_Port = port;
		}

		~HttpServer()
		{
			Dispose(false);
		}
		#endregion



		#region Private Method

		/// <summary>
		/// Responds 404 not found.
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		private void respond404NotFound(HttpListenerContext ctx)
		{
			try
			{
				ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
				ctx.Response.Close();
			}
			catch (Exception e)
			{
				m_Logger.Warn("Unable to respond 404 Not Found", e);
			}
		}



		/// <summary>
		/// Finds the best match.
		/// </summary>
		/// <param name="requestAbsPath">The request abs path.</param>
		/// <returns></returns>
		private IHttpHandler FindBestMatch(string requestAbsPath)
		{
			string path = requestAbsPath;
			if (!path.EndsWith("/"))
				path += "/";

			int maxMatchLen = 0;
			IHttpHandler handler = null;

			foreach (var pair in m_Handlers)
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

			return handler ?? _defaultHandler;
		}

		#endregion

		#region Protected Method
		/// <summary>
		/// Raises the <see cref="E:TaskQueue"/> event.
		/// </summary>
		/// <param name="e">The <see cref="Wammer.TaskQueueEventArgs"/> instance containing the event data.</param>
		//protected void OnTaskQueue(TaskQueueEventArgs e)
		//{
		//    //this.RaiseEvent(TaskEnqueue, e);
		//}

		protected virtual void Dispose(bool disposing)
		{
			if (m_Disposed)
				return;

			Close();

			m_Disposed = true;
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
			string urlPrefix = "http://+:" + m_Port.ToString();

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

			if (m_Handlers.ContainsKey(absPath))
				return;

			m_Handlers.Add(absPath, handler);
			m_Listener.Prefixes.Add(urlPrefix);
		}

		/// <summary>
		/// Adds the default handler.
		/// </summary>
		/// <param name="handler">The handler.</param>
		public void AddDefaultHandler(IHttpHandler handler)
		{
			if (handler == null)
				throw new ArgumentNullException();

			_defaultHandler = handler;
		}

		/// <summary>
		/// Starts this instance.
		/// </summary>
		public void Start()
		{
			lock (m_LockSwitchObj)
			{
				if (_started)
					return;

				m_Listener.Start();
				_started = true;
				m_Listener.BeginGetContext(ConnectionAccepted, null);
			}
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		public void Stop()
		{
			lock (m_LockSwitchObj)
			{
				if (_started)
				{
					m_Listener.Stop();
					_started = false;
				}
			}
		}

		/// <summary>
		/// Closes this instance.
		/// </summary>
		public void Close()
		{
			Stop();
			m_Listener.Close();
		}

		/// <summary>
		/// Connections the accepted.
		/// </summary>
		/// <param name="result">The result.</param>
		private void ConnectionAccepted(IAsyncResult result)
		{
			try
			{
				var context = m_Listener.EndGetContext(result);

				if (context == null)
					return;

				var beginTime = Stopwatch.GetTimestamp();
				m_Listener.BeginGetContext(ConnectionAccepted, null);


				var handler = FindBestMatch(context.Request.Url.AbsolutePath);

				if (handler == null)
				{
					respond404NotFound(context);
					return;
				}

				var task = new HttpHandlingTask(handler, context, beginTime);

				ThreadPool.QueueUserWorkItem((x) => { task.Execute(); });
			}
			catch (ObjectDisposedException)
			{
				m_Logger.Warn("Http server disposed. Shutdown server");
			}
			catch (Exception e)
			{
				m_Logger.Warn("Shutdown server", e);
			}
		}

		#endregion
	}

	interface ITask
	{
		void Execute();
	}

	internal class HttpHandlingTask : ITask
	{
		private static readonly ILog logger = LogManager.GetLogger("HttpHandler");
		private readonly long beginTime;
		private readonly HttpListenerContext context;
		private readonly IHttpHandler handler;

		public HttpHandlingTask(IHttpHandler handler, HttpListenerContext context, long beginTime)
		{
			this.handler = (IHttpHandler)handler.Clone();
			this.context = context;
			this.beginTime = beginTime;
		}

		#region ITask Members

		public void Execute()
		{

			Action action = () =>
			{
				handler.SetBeginTimestamp(beginTime);
				handler.HandleRequest(context.Request, context.Response);
			};

			HandleRequestWithinExceptionHandler(action, context.Response);
		}

		public static void HandleRequestWithinExceptionHandler(Action action, HttpListenerResponse response)
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
				logger.Warn(e.ToString());

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
}