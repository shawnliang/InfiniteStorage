using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waveface.Model;
using System.IO;

namespace Waveface.ClientFramework
{
	public class BunnyDeviceTimelineContentGroup : ContentGroup
	{
		private string deviceId;
		private int _contentCount = -1;

		public BunnyDeviceTimelineContentGroup(string deviceId, string deviceFolder)
			: base("Unsorted", "Timeline", new Uri(Path.Combine(deviceFolder, "vTimeline")), (x) => { })
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
							"select count(*) as num from files where device_id = @dev and deleted = 0 union " +
							"select count(*) as num from PendingFiles where device_id = @dev and deleted = 0)";
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

		public override bool Equals(object obj)
		{
			//檢查參數是否為null
			if (obj == null)
				return false;

			//檢查是否與自身是相同物件
			if (object.ReferenceEquals(this, obj))
				return true;

			//檢查是否型態相等
			var value = obj as BunnyDeviceTimelineContentGroup;
			if (value == null)
				return false;

			//比較內容是否相等
			return this.ID == value.ID;
		}

		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}
	}
}
