using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

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

			return Post(uri, new NameValueCollection(){});
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
		#endregion
	}
}
