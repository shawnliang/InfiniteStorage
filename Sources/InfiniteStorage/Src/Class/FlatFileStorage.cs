using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace InfiniteStorage
{
	public class FlatFileStorage: IFileStorage
	{
		private string storagePath;

		public FlatFileStorage(string storagePath)
		{
			this.storagePath = storagePath;
		}

		public void MoveToStorage(string tempfile, string file_name)
		{
			var saved_file = Path.Combine(storagePath, file_name);
			int num = 1;

			while (true)
			{
				try
				{
					File.Move(tempfile, saved_file);
					break;
				}
				catch (IOException e)
				{
					if (File.Exists(saved_file))
					{
						saved_file = Path.Combine(storagePath, Path.GetFileNameWithoutExtension(file_name) + "." + num + Path.GetExtension(file_name));
						num += 1;
					}
					else
						throw new IOException("Unable to move file to " + saved_file, e);
				}
			}
		}
	}
}
