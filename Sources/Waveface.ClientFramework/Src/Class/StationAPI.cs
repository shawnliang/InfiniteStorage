using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace Waveface.ClientFramework
{
	/// <summary>
	/// 
	/// </summary>
	[Obfuscation]
	public static class StationAPI
	{
		#region Const
		private const string PORT = "14005";
		private const string API_BASE_URL = @"http://127.0.0.1:" + PORT;
		private const string LABEL_API_BASE_URL = API_BASE_URL + @"/label";
		private const string MANIPULATION_API_BASE_URL = API_BASE_URL + @"/manipulation";
		#endregion


		#region Private Static Method
		/// <summary>
		/// Toes the query string.
		/// </summary>
		/// <param name="nvc">The NVC.</param>
		/// <returns></returns>
		private static string ToQueryString(NameValueCollection nvc)
		{
			return string.Join("&", Array.ConvertAll(nvc.AllKeys, key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));
		}

		public static string Post(string uri, NameValueCollection parameters)
		{
			var queryString = ToQueryString(parameters);
			var data = Encoding.Default.GetBytes(queryString);
			var request = WebRequest.Create(uri) as HttpWebRequest;

			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = data.Length;
			request.Timeout = 10000;

			using (var requestStream = request.GetRequestStream())
			{
				requestStream.Write(data, 0, data.Length);
			}

			var response = request.GetResponse();
			// Get the stream containing content returned by the server.
			var dataStream = response.GetResponseStream();
			// Open the stream using a StreamReader for easy access.
			using (var sr = new StreamReader(dataStream))
			{
				return sr.ReadToEnd();
			}
		}
		#endregion


		#region Public Static Method
		public static string GetAllLables()
		{
			var uri = LABEL_API_BASE_URL + "/list_all";

			return Post(uri, new NameValueCollection() { });
		}

		public static string AddLabel(string name)
		{
			return AddLabel(Guid.NewGuid().ToString(), name);
		}

		public static string AddLabel(string labelID, string name)
		{
			var uri = LABEL_API_BASE_URL + "/add";

			return Post(uri, new NameValueCollection() 
			{
				{"label_id", labelID},
				{"name", name}
			});
		}

		public static string DeleteLabel(string labelID)
		{
			var uri = LABEL_API_BASE_URL + "/delete";

			return Post(uri, new NameValueCollection() 
			{
				{"label_id", labelID}
			});
		}

		public static string RenameLabel(string labelID, string name)
		{
			var uri = LABEL_API_BASE_URL + "/rename";

			return Post(uri, new NameValueCollection() 
			{
				{"label_id", labelID},
				{"name", name}
			});
		}

		public static string Tag(string fileID, string labelID)
		{
			var uri = LABEL_API_BASE_URL + "/tag";

			return Post(uri, new NameValueCollection() 
			{
				{"file_id", fileID},
				{"label_id", labelID}
			});
		}

		public static string UnTag(string fileID, string labelID)
		{
			var uri = LABEL_API_BASE_URL + "/untag";

			return Post(uri, new NameValueCollection() 
			{
				{"file_id", fileID},
				{"label_id", labelID}
			});
		}

		public static string ClearLabel(string labelID)
		{
			var uri = LABEL_API_BASE_URL + "/clear";

			return Post(uri, new NameValueCollection() 
			{
				{"label_id", labelID}
			});
		}

		public static string OnAirLabel(string labelID, bool isOnAir)
		{
			var uri = LABEL_API_BASE_URL + "/on_air";

			return Post(uri, new NameValueCollection() 
			{
				{"label_id", labelID},
				{"on_air", isOnAir? "true": "false"}
			});
		}

		public static string ShareLabel(string labelID, bool isShared)
		{
			var uri = LABEL_API_BASE_URL + "/share";

			return Post(uri, new NameValueCollection() 
			{
				{"label_id", labelID},
				{"enabled", isShared? "true": "false"}
			});
		}

		public static string Delete(IEnumerable<string> ids = null, IEnumerable<string> paths = null)
		{
			var uri = MANIPULATION_API_BASE_URL + "/delete";

			var parameters = new NameValueCollection();

			if (ids != null && ids.Any())
				parameters.Add("ids", string.Join(",", ids));

			if (paths != null && paths.Any())
				parameters.Add("paths", string.Join(",", paths));

			if (parameters.Count == 0)
				return null;

			return Post(uri, parameters);
		}

		public static string Move(IEnumerable<string> ids, string targetPath)
		{
			if (ids == null || !ids.Any())
				throw new ArgumentNullException("ids");

			if (string.IsNullOrWhiteSpace(targetPath))
				throw new ArgumentNullException("targetPath");

			var uri = MANIPULATION_API_BASE_URL + "/move";

			var parameters = new NameValueCollection();

			parameters.Add("ids", string.Join(",", ids));
			parameters.Add("targetPath", targetPath);

			return Post(uri, parameters);
		}
		#endregion

		public static string QueryPairingPasscode()
		{
			var parameters = new NameValueCollection();
			var uri = API_BASE_URL + "/pairing/passcode";
			var reply = Post(uri, parameters);

			var response = JsonConvert.DeserializeObject<dynamic>(reply);

			return response.passcode.Value.ToString();
		}
	}
}
