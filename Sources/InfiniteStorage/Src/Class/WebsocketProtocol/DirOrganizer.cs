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

	public class DirOrganizerByYYYY : IDirOrganizer
	{
		public string GetDir(FileContext file)
		{
			return file.datetime.ToString("yyyy");
		}
	}

	public class DirOrganizerByYYYYMM : IDirOrganizer
	{
		public string GetDir(FileContext file)
		{
			return file.datetime.ToString(@"yyyy\\yyyy-MM");
		}
	}

	public class DirOrganizerByYYYYMMDD : IDirOrganizer
	{
		public string GetDir(FileContext file)
		{
			return file.datetime.ToString(@"yyyy\\yyyy-MM\\yyyy-MM-dd");
		}
	}
}
