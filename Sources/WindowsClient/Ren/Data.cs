#region

using System.Collections.Generic;

#endregion

namespace Waveface.Client
{
	public class FileChange
	{
		public string id { get; set; }
		public string file_name { get; set; }
		public string tiny_path { get; set; }
		public string taken_time { get; set; }
		public int width { get; set; }
		public int height { get; set; }
		public long size { get; set; }
		public int type { get; set; }
		public string saved_path { get; set; }
	}

	public class RtData
	{
		public List<FileChange> file_changes { get; set; }

		public RtData()
		{
			file_changes = new List<FileChange>();
		}
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

	public class PendingSort
	{
		public string device_id { get; set; }
		public List<string> discards { get; set; }
		public List<Event> events { get; set; }

		public PendingSort()
		{
			discards = new List<string>();
			events = new List<Event>();
		}
	}
}