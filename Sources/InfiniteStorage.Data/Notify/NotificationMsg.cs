using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.Data.Notify
{
	public class NotificationMsg
	{
		public List<string> active_devices { get; set; }

		public string NewDevice { get; set; }
	}
}
