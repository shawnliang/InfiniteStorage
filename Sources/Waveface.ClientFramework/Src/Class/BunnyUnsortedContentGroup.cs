using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waveface.Model;

namespace Waveface.ClientFramework
{
	class BunnyUnsortedContentGroup : ContentGroup
	{
		private string deviceId;
		private int _contentCount = -1;

		public BunnyUnsortedContentGroup(string deviceId)
			: base("Unsorted", "Unsorted files", new Uri(@"c:\"), (x) => { })
		{
			this.deviceId = deviceId;
		}

		public override int ContentCount
		{
			get
			{
				if (_contentCount < 0)
					_contentCount = countUnsortedItems();

				return _contentCount;
			}
		}

		private int countUnsortedItems()
		{
			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();
				var cmd = conn.CreateCommand();
				cmd.CommandText = "select count(*) from [PendingFiles] where device_id = @dev";
				cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter("@dev", this.deviceId));
				return (int)(long)cmd.ExecuteScalar();
			}
		}

		public void Refresh()
		{
			var curCount = countUnsortedItems();

			if (curCount != _contentCount)
			{
				_contentCount = curCount;
				OnPropertyChanged("ContentCount");
			}
		}
	}
}
