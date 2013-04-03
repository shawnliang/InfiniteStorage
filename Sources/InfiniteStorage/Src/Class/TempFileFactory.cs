using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace InfiniteStorage
{
	class TempFileFactory : ITempFileFactory
	{
		private string tempFolder;

		public TempFileFactory(string tempFolder)
		{
			this.tempFolder = tempFolder;

			if (!Directory.Exists(tempFolder))
				Directory.CreateDirectory(tempFolder);
		}

		public ITempFile CreateTempFile()
		{
			return new TempFile(tempFolder);
		}
	}
}
