using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace InfiniteStorage
{
	class FlatFileStorage: IFileStorage
	{
		private string storagePath;

		public FlatFileStorage(string storagePath)
		{
			this.storagePath = storagePath;
		}

		public void MoveToStorage(string tempfile, string file_name)
		{
			File.Move(tempfile, Path.Combine(storagePath, file_name));
		}
	}
}
