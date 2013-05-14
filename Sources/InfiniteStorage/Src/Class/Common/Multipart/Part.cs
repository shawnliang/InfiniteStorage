using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace Wammer.MultiPart
{
	public class Part
	{
		public static byte[] DASH_DASH = Encoding.UTF8.GetBytes("--");
		public static byte[] CRLF = Encoding.UTF8.GetBytes("\r\n");

		private readonly ArraySegment<byte> bytes;
		private readonly NameValueCollection headers;
		private ArraySegment<byte> data;
		private Disposition disposition;
		private string text;

		public Part(ArraySegment<byte> data, NameValueCollection headers)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			if (headers == null)
				throw new ArgumentNullException("headers");

			this.data = data;
			bytes = data;
			this.headers = headers;

			if (headers["content-disposition"] != null)
			{
				disposition = Disposition.Parse(headers["content-disposition"]);
			}
		}

		public Part(ArraySegment<byte> data)
		{
			if (data == null)
				throw new ArgumentNullException();

			this.data = data;
			headers = new NameValueCollection();
		}

		//public Part(string data)
		//{
		//    if (data == null)
		//        throw new ArgumentNullException();

		//    this.data = Encoding.UTF8.GetBytes(data);
		//    this.start = 0;
		//    this.len = this.data.Length;
		//    this.headers = new NameValueCollection();
		//}

		public Part(string data, NameValueCollection headers)
		{
			if (data == null || headers == null)
				throw new ArgumentNullException();

			byte[] dataUtf8 = Encoding.UTF8.GetBytes(data);

			this.data = new ArraySegment<byte>(dataUtf8);
			this.headers = headers;

			if (headers["content-disposition"] != null)
			{
				disposition = Disposition.Parse(headers["content-disposition"]);
			}
		}

		public string Text
		{
			get
			{
				var header = headers["content-transfer-encoding"];
				if (header != null &&
					header.Equals("binary"))
					return null;
				return text ?? (text = Encoding.UTF8.GetString(data.Array, data.Offset, data.Count));

				// text might have \r\n at its end
				//return text.TrimEnd(CRLFtail);
			}
		}

		public ArraySegment<byte> Bytes
		{
			get { return bytes; }
		}

		public NameValueCollection Headers
		{
			get { return headers; }
		}

		public Disposition ContentDisposition
		{
			get { return disposition; }
			set { disposition = value; }
		}

		public void CopyTo(Stream output, byte[] boundaryData)
		{
			output.Write(CRLF, 0, CRLF.Length);
			output.Write(DASH_DASH, 0, DASH_DASH.Length);
			output.Write(boundaryData, 0, boundaryData.Length);
			output.Write(CRLF, 0, CRLF.Length);

			if (disposition != null)
				disposition.CopyTo(output);

			foreach (string name in headers.AllKeys)
			{
				if (disposition != null && name.Equals(
					"content-disposition", StringComparison.CurrentCultureIgnoreCase))
					continue;


				string hdr = name + ":" + headers[name];
				byte[] hdrData = Encoding.UTF8.GetBytes(hdr);

				output.Write(hdrData, 0, hdr.Length);
				output.Write(CRLF, 0, CRLF.Length);
			}

			output.Write(CRLF, 0, CRLF.Length);

			output.Write(data.Array, data.Offset, data.Count);
		}
	}
}