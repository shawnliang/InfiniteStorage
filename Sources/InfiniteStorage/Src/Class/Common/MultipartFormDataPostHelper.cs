using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Waveface.Common
{
	#region MultipartFormDataPostHelper

	public static class MultipartFormDataPostHelper
	{
		public static Encoding encoding = Encoding.UTF8;

		// Post the data as a multipart form
		// postParameters with a value of type byte[] will be passed in the form as a file, and value of type string will be
		// passed as a name/value pair.
		public static HttpWebResponse MultipartFormDataPost(string postUrl, string userAgent, Dictionary<string, object> postParameters, string fileName, string mimeType)
		{
			string formDataBoundary = "--ABCDEFG";
			string contentType = "multipart/form-data; boundary=" + formDataBoundary;

			byte[] formData = GetMultipartFormData(postParameters, formDataBoundary, fileName, mimeType);

			return PostForm(postUrl, userAgent, contentType, formData);
		}


		public static HttpWebResponse PostWammerImage(string postUrl, string userAgent, Dictionary<string, object> postParameters, string fileName, string mimeType)
		{
			string formDataBoundary = "--ABCDEFG";
			string contentType = "multipart/form-data; boundary=" + formDataBoundary;

			byte[] formData = GetMultipartFormData(postParameters, formDataBoundary, fileName, mimeType);

			return PostForm(postUrl, userAgent, contentType, formData);
		}

		// Post a form
		private static HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData)
		{
			HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

			if (request == null)
			{
				throw new NullReferenceException("request is not a http request");
			}

			// Add these, as we're doing a POST
			// request.KeepAlive = false;
			// request.Timeout = 10000;

			request.Method = "POST";
			request.ContentType = contentType;
			request.UserAgent = userAgent;
			request.CookieContainer = new CookieContainer();

			// We need to count how many bytes we're sending. 
			request.ContentLength = formData.Length;

			using (Stream requestStream = request.GetRequestStream())
			{
				// Push it out there
				requestStream.Write(formData, 0, formData.Length);
				requestStream.Close();
			}

			HttpWebResponse _resp = request.GetResponse() as HttpWebResponse;
			return request.GetResponse() as HttpWebResponse;
		}

		// Turn the key and value pairs into a multipart form.
		// See http://www.ietf.org/rfc/rfc2388.txt for issues about file uploads
		private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary, string fileName, string mimeType)
		{
			Stream formDataStream = new MemoryStream();

			foreach (var param in postParameters)
			{
				if (param.Value is byte[])
				{
					byte[] fileData = param.Value as byte[];

					// Add just the first part of this param, since we will write the file data directly to the Stream
					string _header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n", boundary, param.Key, fileName.Equals("") ? param.Key : fileName, mimeType.Equals("") ? "application/octet-stream" : mimeType);

					//formDataStream.Write(encoding.GetBytes(_header), 0, _header.Length);

					byte[] _bytes = encoding.GetBytes(_header);
					formDataStream.Write(_bytes, 0, _bytes.Length);

					// Write the file data directly to the Stream, rather than serializing it to a string.
					formDataStream.Write(fileData, 0, fileData.Length);
				}
				else
				{
					string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", boundary, param.Key, param.Value);
					formDataStream.Write(encoding.GetBytes(postData), 0, postData.Length);
				}
			}

			// Add the end of the request
			string footer = "\r\n--" + boundary + "--\r\n";
			formDataStream.Write(encoding.GetBytes(footer), 0, footer.Length);

			// Dump the Stream into a byte[]
			formDataStream.Position = 0;
			byte[] formData = new byte[formDataStream.Length];
			formDataStream.Read(formData, 0, formData.Length);
			formDataStream.Close();

			return formData;
		}
	}

	#endregion
}