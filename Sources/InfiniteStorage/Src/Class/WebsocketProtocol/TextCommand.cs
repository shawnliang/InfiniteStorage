﻿using System;

namespace InfiniteStorage.WebsocketProtocol
{
	public class TextCommand
	{
		public string action { get; set; }
		public string file_name { get; set; }
		public long file_size { get; set; }
		public string folder { get; set; }
		public DateTime datetime { get; set; }
		public string device_name { get; set; }
		public string device_id { get; set; }
		public long transfer_count { get; set; }
		public long transfer_size { get; set; }
		public string type { get; set; }

		public long backuped_count { get; set; }
		public long total_count { get; set; }

		public bool isFileStartCmd()
		{
			return "file-start".Equals(action);
		}

		public bool isFileEndCmd()
		{
			return "file-end".Equals(action);
		}

		public bool isConnectCmd()
		{
			return "connect".Equals(action);
		}

		public bool isUpdatecountCmd()
		{
			return "update-count".Equals(action);
		}
	}
}
