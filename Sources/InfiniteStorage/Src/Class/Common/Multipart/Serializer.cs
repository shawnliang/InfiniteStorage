#region

using System;
using System.IO;
using System.Text;

#endregion

namespace Wammer.MultiPart
{
	public class Serializer
	{
		private readonly string m_boundary;
		private readonly byte[] m_boundaryData;
		private readonly Stream m_output;

		public Serializer(Stream output)
		{
			if (output == null)
				throw new ArgumentNullException("output");

			m_output = output;
			m_boundary = Guid.NewGuid().ToString("N");
			m_boundaryData = Encoding.UTF8.GetBytes(m_boundary);
		}

		public Serializer(Stream output, string boundary)
		{
			if (output == null)
				throw new ArgumentNullException("output");

			m_output = output;
			m_boundary = boundary;
			m_boundaryData = Encoding.UTF8.GetBytes(boundary);
		}

		public string Boundary
		{
			get { return m_boundary; }
		}

		public void Put(Part part)
		{
			part.CopyTo(m_output, m_boundaryData);
		}

		public void Put(Part[] parts)
		{
			foreach (Part t in parts)
				Put(t);
		}

		public void PutNoMoreData()
		{
			m_output.Write(Part.CRLF, 0, Part.CRLF.Length);
			m_output.Write(Part.DASH_DASH, 0, Part.DASH_DASH.Length);
			m_output.Write(m_boundaryData, 0, m_boundaryData.Length);
			m_output.Write(Part.DASH_DASH, 0, Part.DASH_DASH.Length);
			m_output.Write(Part.CRLF, 0, Part.CRLF.Length);
		}
	}
}