using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.Pair
{
	class NewDeviceRespondingEventArgs : EventArgs
	{
		public NewDeviceRespondingEventArgs(string device_id)
		{
			this.device_id = device_id;
		}

		public NewDeviceRespondingEventArgs(string device_id, bool syncOld, int last_x_days)
		{
			this.device_id = device_id;
			this.syncOld = syncOld;
			this.last_x_days = last_x_days;
		}

		public string device_id { get; set; }

		public bool syncOld { get; set; }

		public int last_x_days { get; set; }
	}
}
