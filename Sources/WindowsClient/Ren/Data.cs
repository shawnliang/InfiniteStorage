#region

using System;
using System.Collections.Generic;

#endregion

namespace Waveface.Client
{
	public class FileEntry
	{
		public string id;
		public string tiny_path;
		public string s92_path;
		public DateTime taken_time;
		public int type;
		public bool has_origin;
	}
}