#region

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Wammer.MultiPart;
using log4net;

#endregion

namespace Wammer.Station
{
	#region UploadedFile

	public class UploadedFile
	{
		public UploadedFile(string name, ArraySegment<byte> data, string contentType)
		{
			Name = name;
			Data = data;
			ContentType = contentType;
		}

		public string Name { get; private set; }
		public ArraySegment<byte> Data { get; private set; }
		public string ContentType { get; private set; }
	}

	#endregion

	#region HttpHandler

	public abstract class HttpHandler : IHttpHandler
	{
		#region Const

		private const string BOUNDARY = "boundary=";
		private const string URL_ENCODED_FORM = "application/x-www-form-urlencoded";
		private const string MULTIPART_FORM = "multipart/form-data";
		// private const string API_PATH_GROUP_NAME = @"APIPath";
		// private const string API_PATH_MATCH_PATTERN = @"/V\d+/(?<" + API_PATH_GROUP_NAME + ">.+)";

		#endregion

		private static readonly ILog s_logger = LogManager.GetLogger("HttpHandler");
		private long m_beginTime;

		public HttpListenerRequest Request { get; internal set; }
		public HttpListenerResponse Response { get; internal set; }
		public NameValueCollection Parameters { get; internal set; }
		public List<UploadedFile> Files { get; private set; }
		public byte[] RawPostData { get; internal set; }

		#region IHttpHandler Members

		public event EventHandler<HttpHandlerEventArgs> ProcessSucceeded;

		public void SetBeginTimestamp(long beginTime)
		{
			m_beginTime = beginTime;
		}

		public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			Files = new List<UploadedFile>();
			Request = request;
			Response = response;

			if (string.Compare(Request.HttpMethod, "POST", true) == 0)
			{
				var content = readPostContent(request);

				Action action = () =>
									{
										RawPostData = content.ToArray();
										ParseAndHandleRequest();
									};

				HttpHandlingTask.HandleRequestWithinExceptionHandler(action, Response);
				return;
			}

			ParseAndHandleRequest();
		}

		private MemoryStream readPostContent(HttpListenerRequest request)
		{
			var buff = new byte[65535];
			var nread = 0;
			var content = new MemoryStream(postBufferSize());

			while ((nread = request.InputStream.Read(buff, 0, buff.Length)) > 0)
			{
				content.Write(buff, 0, nread);
			}
			return content;
		}

		private int postBufferSize()
		{
			var initialSize = (int)Request.ContentLength64;
			if (initialSize <= 0)
				initialSize = 65535;
			return initialSize;
		}

		public abstract void HandleRequest();

		public virtual object Clone()
		{
			return MemberwiseClone();
		}

		#endregion

		#region Protected Method

		protected void CheckParameter(params string[] arguementNames)
		{
			if (arguementNames == null)
				throw new ArgumentNullException("arguementNames");

			var nullArgumentNames = from arguementName in arguementNames
									where Parameters[arguementName] == null
									select arguementName;

			if (!nullArgumentNames.Any())
				return;

			throw new FormatException(string.Format("Parameter {0} is null.", string.Join("、", nullArgumentNames.ToArray())));
		}

		#endregion

		private void LogRequest()
		{
			if (s_logger.IsDebugEnabled)
			{
				Debug.Assert(Request.RemoteEndPoint != null, "Request.RemoteEndPoint != null");


				if (Request.RemoteEndPoint.Address.ToString() == "127.0.0.1" &&
					Request.Url.AbsolutePath.Contains("/ping"))
					return;

				s_logger.Info("====== Request " + Request.Url.AbsolutePath +
							" from " + Request.RemoteEndPoint.Address + " ======");
				foreach (string key in Parameters.AllKeys)
				{
					if (key == "password")
					{
						s_logger.InfoFormat("{0} : *", key);
					}
					else
					{
						s_logger.InfoFormat("{0} : {1}", key, Parameters[key]);
					}
				}
				foreach (UploadedFile file in Files)
					s_logger.InfoFormat("file: {0}, mime: {1}, size: {2}", file.Name, file.ContentType, file.Data.Count.ToString());
			}
		}

		protected void OnProcessSucceeded(HttpHandlerEventArgs evt)
		{
			EventHandler<HttpHandlerEventArgs> handler = ProcessSucceeded;

			if (handler != null)
			{
				handler(this, evt);
			}
		}

		private void ParseAndHandleRequest()
		{
			Parameters = InitParameters(Request);

			if (HasMultiPartFormData(Request))
			{
				ParseMultiPartData(Request);
			}

			LogRequest();

			HandleRequest();

			long end = Stopwatch.GetTimestamp();

			long duration = end - m_beginTime;
			if (duration < 0)
				duration += long.MaxValue;

			OnProcessSucceeded(new HttpHandlerEventArgs(duration));
		}

		private void ParseMultiPartData(HttpListenerRequest request)
		{
			try
			{
				var boundary = GetMultipartBoundary(request.ContentType);
				var parser = new Parser(boundary);

				var parts = parser.Parse(RawPostData);
				foreach (var part in parts)
				{
					if (part.ContentDisposition == null)
						continue;

					ExtractParamsFromMultiPartFormData(part);
				}
			}
			catch (FormatException)
			{
				string filename = Guid.NewGuid().ToString();
				using (var w = new BinaryWriter(File.OpenWrite(@"log\" + filename)))
				{
					w.Write(RawPostData);
				}
				s_logger.Warn("Parsing multipart data error. Post data written to log\\" + filename);
				throw;
			}
		}

		private void ExtractParamsFromMultiPartFormData(Part part)
		{
			Disposition disp = part.ContentDisposition;

			if (disp == null)
				throw new ArgumentException("incorrect use of this function: " +
											"input part.ContentDisposition is null");

			if (disp.Value.Equals("form-data", StringComparison.CurrentCultureIgnoreCase))
			{
				string filename = disp.Parameters["filename"];

				if (filename != null)
				{
					var file = new UploadedFile(filename, part.Bytes,
												part.Headers["Content-Type"]);
					Files.Add(file);
				}
				else
				{
					string name = disp.Parameters["name"];
					Parameters.Add(name, part.Text);
				}
			}
		}

		private static bool HasMultiPartFormData(HttpListenerRequest request)
		{
			return request.ContentType != null &&
				   request.ContentType.StartsWith(MULTIPART_FORM, StringComparison.CurrentCultureIgnoreCase);
		}

		private static string GetMultipartBoundary(string contentType)
		{
			if (contentType == null)
				throw new ArgumentNullException();

			try
			{
				var parts = contentType.Split(';');
				foreach (var part in parts)
				{
					var idx = part.IndexOf(BOUNDARY);
					if (idx < 0)
						continue;

					return part.Substring(idx + BOUNDARY.Length);
				}

				throw new FormatException("Multipart boundary is nout found in content-type header");
			}
			catch (Exception e)
			{
				throw new FormatException("Error finding multipart boundary. Content-Type: " +
										  contentType, e);
			}
		}

		private NameValueCollection InitParameters(HttpListenerRequest req)
		{
			if (RawPostData != null &&
				req.ContentType != null &&
				req.ContentType.StartsWith(URL_ENCODED_FORM, StringComparison.CurrentCultureIgnoreCase))
			{
				var postData = Encoding.UTF8.GetString(RawPostData);
				return HttpUtility.ParseQueryString(postData);
			}
			else if (req.HttpMethod.Equals("GET", StringComparison.CurrentCultureIgnoreCase))
			{
				return HttpUtility.ParseQueryString(Request.Url.Query); //req.QueryString;
			}

			return new NameValueCollection();
		}

		protected void respondSuccess(object data = null)
		{
			var replyContent = "";
			if (data == null)
				replyContent = JsonConvert.SerializeObject(new { api_ret_code = 0, api_ret_message = "success", status = 200 });
			else
				replyContent = JsonConvert.SerializeObject(data);

			Response.StatusCode = (int)HttpStatusCode.OK;
			Response.ContentType = "application/json";
			using (var w = new StreamWriter(Response.OutputStream))
			{
				w.Write(replyContent);
			}
		}
	}

	#endregion

	#region HttpHandlerEventArgs

	public class HttpHandlerEventArgs : EventArgs
	{
		public HttpHandlerEventArgs(long durationInTicks)
		{
			DurationInTicks = durationInTicks;
		}

		public long DurationInTicks { get; private set; }
	}

	#endregion
}