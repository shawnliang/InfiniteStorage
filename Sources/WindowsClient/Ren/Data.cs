#region

using System;
using System.Collections.Generic;
using InfiniteStorage.Model;

#endregion

namespace Waveface.Client
{
	public class FileEntry
	{
		public string id;
		public string tiny_path;
		public string s92_path;
		public string saved_path;
		public DateTime taken_time;
		public int type;
		public bool has_origin;
	}

	public class EventEntry
	{
		public string event_id;
		public Event Event;
		public List<FileEntry> Files = new List<FileEntry>();
	}
}