using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.WebsocketProtocol
{
	public interface IDirOrganizer
	{
		string GetDir(FileContext file);
	}

	class DirOrganizerByYYYY : IDirOrganizer
	{
		public string GetDir(FileContext file)
		{
			return file.datetime.Year.ToString("d4");
		}
	}

	class DirOrganizerByYYYYMM : IDirOrganizer
	{
		public string GetDir(FileContext file)
		{
			return file.datetime.Year.ToString("d4") + @"\" + file.datetime.Month.ToString("d2");
		}
	}

	class DirOrganizerByYYYYMMDD : IDirOrganizer
	{
		public string GetDir(FileContext file)
		{
			return file.datetime.Year.ToString("d4") + @"\" + file.datetime.Month.ToString("d2") + @"\" + file.datetime.Day.ToString("d2");
		}
	}
}
