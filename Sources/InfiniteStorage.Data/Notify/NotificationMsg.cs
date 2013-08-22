using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.Data.Notify
{
	public class NotificationMsg
	{
		public List<ReceivingStatus> recving_devices { get; set; }

		public string new_device { get; set; }

		public folder_info new_folder { get; set; }

		public folder_info update_folder { get; set; }
	}

	public class ReceivingStatus
	{
		public string DeviceId { get; set; }
		public bool IsPreparing { get; set; }
		public bool IsReceiving { get; set; }
		public int Total { get; set; }
		public int Received { get; set; }
	}

	public class folder_info
	{
		public string name { get; set; }

		public string parent_folder { get; set; }

		public string path { get; set; }
	}
}
