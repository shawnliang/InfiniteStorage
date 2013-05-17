using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Waveface.Model;

namespace Waveface.ClientFramework
{
	public class BunnyContentGroup : ContentGroup
	{
		#region Constructor
		public BunnyContentGroup()
		{

		}

		public BunnyContentGroup(Uri uri)
			: base(uri.LocalPath.GetHashCode().ToString(), Path.GetFileName(uri.LocalPath), uri)
		{
			SetContents((contents) =>
			{
				var path = uri.LocalPath;
				var directories = Directory.GetDirectories(path);

				foreach (var directory in directories)
				{
					contents.Add(new BunnyContentGroup(new Uri(directory)));
				}

				var files = Directory.GetFiles(path, "*.jpg");

				foreach (var file in files)
				{
					contents.Add(new BunnyContent(new Uri(file)));
				}
			});
		}
		#endregion
	}
}
