using InfiniteStorage.Properties;
using System;

namespace InfiniteStorage.WebsocketProtocol
{
	public interface IDirOrganizer
	{
		string GetDir(FileContext file);
	}

	class DirOrganizerProxy : IDirOrganizer
	{
		private IDirOrganizer organizer;

		public string GetDir(FileContext file)
		{
			if (organizer == null)
			{
				organizer = getDirOrganizerFromSetting();
			}

			return organizer.GetDir(file);
		}

		private static IDirOrganizer getDirOrganizerFromSetting()
		{
			switch ((OrganizeMethod)Settings.Default.OrganizeMethod)
			{
				case OrganizeMethod.Year:
					return new DirOrganizerByYYYY();

				case OrganizeMethod.YearMonth:
					return new DirOrganizerByYYYYMM();

				case OrganizeMethod.YearMonthDay:
					return new DirOrganizerByYYYYMMDD();

				default:
					throw new NotImplementedException();
			}
		}
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
