using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waveface.Model;

namespace Waveface.ClientFramework
{
	public class BunnyDeviceTimelineContentGroup : ContentGroup
	{
		private string deviceId;
		private int _contentCount = -1;

		public BunnyDeviceTimelineContentGroup(string deviceId)
			: base("Unsorted", "Timeline", new Uri(@"c:\"), (x) => { })
		{
			this.deviceId = deviceId;
		}

		public override int ContentCount
		{
			get
			{
				if (_contentCount < 0)
					_contentCount = countWholeTimeline(this.deviceId);

				return _contentCount;
			}
		}

		public static int countWholeTimeline(string deviceID)
		{
			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = //"select count(*) from [Files] where device_id = @dev";
						"select sum(num) from ( " +
							"select count(*) as num from files where device_id = @dev union " +
							"select count(*) as num from PendingFiles where device_id = @dev)";
					cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter("@dev", deviceID));
					return (int)(long)cmd.ExecuteScalar();
				}
			}
		}

		public override void Refresh()
		{
			var curCount = countWholeTimeline(this.deviceId);

			if (curCount != _contentCount)
			{
				_contentCount = curCount;
				OnPropertyChanged("ContentCount");
			}
		}
	}
}
