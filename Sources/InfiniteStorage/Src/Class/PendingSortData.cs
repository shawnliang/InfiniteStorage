using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage
{
	class PendingSortData
	{
		public List<Guid> discard { get; set; }
		public List<PendingEvent> events { get; set; }
	}

	class PendingEvent
	{
		public DateTime time_start { get; set; }
		public DateTime time_end { get; set; }
		public List<Guid> files { get; set; }

		public string title { get; set; }
		public int type { get; set; }
	}
}
