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

		public NewDeviceRespondingEventArgs(string device_id, bool syncOld, int latest_x_items)
		{
			this.device_id = device_id;
			this.syncOld = syncOld;
			this.latest_x_items = latest_x_items;
		}

		public string device_id { get; set; }

		public bool syncOld { get; set; }

		public int latest_x_items { get; set; }
	}
}
