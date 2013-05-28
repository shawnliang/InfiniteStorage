using System.IO;

namespace InfiniteStorage
{
	public class EventDirOrganizer : IEventDirOrganizer
	{
		public string GetEventFolder(PendingEvent evt)
		{
			var folder_name = evt.title;

			if (string.IsNullOrEmpty(folder_name))
			{
				var evtTime = evt.time_start.ToLocalTime();
				folder_name = string.Format("{0} ~ {1}", evtTime.ToString("yyyy-MM-dd HH-mm-ss"), evt.time_end.ToLocalTime().ToString("yyyy-MM-dd HH-mm-ss"));
			}
			else
				folder_name = sanitizeFolderName(folder_name);

			return folder_name;
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

			return evtTime.ToString("yyyy-MM");
		}
	}
}