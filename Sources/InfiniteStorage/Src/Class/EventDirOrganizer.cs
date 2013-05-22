using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace InfiniteStorage
{
	public class EventDirOrganizer : IEventDirOrganizer
	{
		public string GetEventFolder(PendingEvent evt)
		{
			var title = evt.title;

			if (!string.IsNullOrEmpty(title))
				title = sanitizeFolderName(title);

			var evtTime = evt.time_start.ToLocalTime();

			return string.Format(@"{0}\{1}\{2}",
				evtTime.ToString("yyyy"),
				evtTime.ToString("yyyy-MM"),
				string.IsNullOrEmpty(title) ? string.Format("{0} ~ {1}", evtTime.ToString("yyyy-MM-dd HH-mm-ss"), evt.time_end.ToLocalTime().ToString("yyyy-MM-dd HH-mm-ss")) : title				
				);
		}

		private static string sanitizeFolderName(string name)
		{
			foreach (var inv_char in Path.GetInvalidFileNameChars())
			{
				name = name.Replace(inv_char, '_');
			}
			return name;
		}
	}

	public class MonthlyDirOrganizer : IEventDirOrganizer
	{
		public string GetEventFolder(PendingEvent evt)
		{
			var evtTime = evt.time_start.ToLocalTime();

			return string.Format(@"{0}\{1}",
				evtTime.ToString("yyyy"),
				evtTime.ToString("yyyy-MM"));
		}
	}
}