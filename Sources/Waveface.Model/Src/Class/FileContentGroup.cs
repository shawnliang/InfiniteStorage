using System;
using System.IO;

namespace Waveface.Model
{
	public class FileContentGroup : ContentGroup
	{
		#region Constructor
		public FileContentGroup()
		{

		}

		public FileContentGroup(Uri uri)
			: base(uri.LocalPath.GetHashCode().ToString(), Path.GetFileName(uri.LocalPath), uri)
		{
			SetContents((contents) =>
			{
				var path = uri.LocalPath;
				var directories = Directory.GetDirectories(path);

				foreach (var directory in directories)
				{
					contents.Add(new FileContentGroup(new Uri(directory)));
				}

				var files = Directory.GetFiles(path, "*.jpg");

				foreach (var file in files)
				{
					contents.Add(new Content(new Uri(file)));
				}
			});
		}
		#endregion
	}
}
