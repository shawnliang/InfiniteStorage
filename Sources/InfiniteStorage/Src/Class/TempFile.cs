using System;
using System.IO;

namespace InfiniteStorage
{
	public class TempFile : ITempFile
	{
		private FileStream stream;

		public TempFile(string tempFolder)
		{
			this.Path = System.IO.Path.Combine(tempFolder, Guid.NewGuid().ToString());
			this.stream = File.OpenWrite(this.Path);
		}

		public string Path
		{
			get;
			private set;
		}

		public long BytesWritten
		{
			get;
			private set;
		}

		public void Write(byte[] data)
		{
			stream.Write(data, 0, data.Length);
			BytesWritten += data.Length;
		}

		public void EndWrite()
		{
			stream.Close();
		}

		public void Delete()
		{
			stream.Close();
			File.Delete(Path);
		}
	}
}
