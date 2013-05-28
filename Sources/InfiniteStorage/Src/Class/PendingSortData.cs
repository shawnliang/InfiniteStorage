using System;
using System.Collections.Generic;

namespace InfiniteStorage
{
	public class PendingSortData
	{
		public string device_id { get; set; }
		public List<Guid> discard { get; set; }
		public List<PendingEvent> events { get; set; }
	}

	public enum EventType
	{
		Monthly = 0,
		Manual = 1
	}

	public class PendingEvent
	{
		public DateTime time_start { get; set; }
		public DateTime time_end { get; set; }
		public List<Guid> files { get; set; }

		public string title { get; set; }
		public int type { get; set; }
	}
}
