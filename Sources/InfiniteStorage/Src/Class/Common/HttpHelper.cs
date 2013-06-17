using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Web;

namespace Waveface.Common
{
	class HttpHelper
	{
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
	}
}
