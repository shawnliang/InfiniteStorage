#region

using System;
using System.Collections.Generic;

#endregion

namespace Waveface.Client
{
	public class FileEntry
	{
		public string id { get; set; }
		public string file_name { get; set; }
		public string tiny_path { get; set; }
		public DateTime taken_time { get; set; }
		public int width { get; set; }
		public int height { get; set; }
		public long size { get; set; }
		public int type { get; set; }
		public string saved_path { get; set; }
		public bool has_origin { get; set; }
	}

	public class Event
	{
		public string time_start { get; set; }
		public string time_end { get; set; }
		public List<string> files { get; set; }
		public string title { get; set; }
		public int type { get; set; }

		public Event()
		{
			files = new List<string>();
		}
	}
}