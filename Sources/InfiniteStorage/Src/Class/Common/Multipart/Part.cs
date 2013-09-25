#region

using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;

#endregion

namespace Wammer.MultiPart
{
	public class Part
	{
		public static byte[] DASH_DASH = Encoding.UTF8.GetBytes("--");
		public static byte[] CRLF = Encoding.UTF8.GetBytes("\r\n");

		private readonly ArraySegment<byte> m_bytes;
		private readonly NameValueCollection m_headers;
		private ArraySegment<byte> m_data;
		private Disposition m_disposition;
		private string m_text;

		public Part(ArraySegment<byte> data, NameValueCollection headers)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			if (headers == null)
				throw new ArgumentNullException("headers");

			m_data = data;
			m_bytes = data;
			m_headers = headers;

			if (headers["content-disposition"] != null)
			{
				m_disposition = Disposition.Parse(headers["content-disposition"]);
			}
		}

		public Part(ArraySegment<byte> data)
		{
			if (data == null)
				throw new ArgumentNullException();

			m_data = data;
			m_headers = new NameValueCollection();
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

			m_data = new ArraySegment<byte>(dataUtf8);
			m_headers = headers;

			if (headers["content-disposition"] != null)
			{
				m_disposition = Disposition.Parse(headers["content-disposition"]);
			}
		}

		public string Text
		{
			get
			{
				var header = m_headers["content-transfer-encoding"];

				if (header != null && header.Equals("binary"))
					return null;

				return m_text ?? (m_text = Encoding.UTF8.GetString(m_data.Array, m_data.Offset, m_data.Count));

				// text might have \r\n at its end
				//return text.TrimEnd(CRLFtail);
			}
		}

		public ArraySegment<byte> Bytes
		{
			get { return m_bytes; }
		}

		public NameValueCollection Headers
		{
			get { return m_headers; }
		}

		public Disposition ContentDisposition
		{
			get { return m_disposition; }
			set { m_disposition = value; }
		}

		public void CopyTo(Stream output, byte[] boundaryData)
		{
			output.Write(CRLF, 0, CRLF.Length);
			output.Write(DASH_DASH, 0, DASH_DASH.Length);
			output.Write(boundaryData, 0, boundaryData.Length);
			output.Write(CRLF, 0, CRLF.Length);

			if (m_disposition != null)
				m_disposition.CopyTo(output);

			foreach (string name in m_headers.AllKeys)
			{
				if (m_disposition != null && name.Equals("content-disposition", StringComparison.CurrentCultureIgnoreCase))
					continue;

				string hdr = name + ":" + m_headers[name];
				byte[] hdrData = Encoding.UTF8.GetBytes(hdr);

				output.Write(hdrData, 0, hdr.Length);
				output.Write(CRLF, 0, CRLF.Length);
			}

			output.Write(CRLF, 0, CRLF.Length);

			output.Write(m_data.Array, m_data.Offset, m_data.Count);
		}
	}
}