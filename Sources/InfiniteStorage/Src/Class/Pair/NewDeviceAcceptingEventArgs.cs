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

		public string device_id { get; set; }
	}
}
