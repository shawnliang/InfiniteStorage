using System;
using System.IO;
using System.Text;

namespace Wammer.MultiPart
{
	public class Serializer
	{
		private readonly string boundary;
		private readonly byte[] boundaryData;
		private readonly Stream output;

		public Serializer(Stream output)
		{
			if (output == null)
				throw new ArgumentNullException("output");

			this.output = output;
			boundary = Guid.NewGuid().ToString("N");
			boundaryData = Encoding.UTF8.GetBytes(boundary);
		}

		public Serializer(Stream output, string boundary)
		{
			if (output == null)
				throw new ArgumentNullException("output");

			this.output = output;
			this.boundary = boundary;
			boundaryData = Encoding.UTF8.GetBytes(boundary);
		}

		public string Boundary
		{
			get { return boundary; }
		}

		public void Put(Part part)
		{
			part.CopyTo(output, boundaryData);
		}

		public void Put(Part[] parts)
		{
			foreach (Part t in parts)
				Put(t);
		}

		public void PutNoMoreData()
		{
			output.Write(Part.CRLF, 0, Part.CRLF.Length);
			output.Write(Part.DASH_DASH, 0, Part.DASH_DASH.Length);
			output.Write(boundaryData, 0, boundaryData.Length);
			output.Write(Part.DASH_DASH, 0, Part.DASH_DASH.Length);
			output.Write(Part.CRLF, 0, Part.CRLF.Length);
		}
	}
}