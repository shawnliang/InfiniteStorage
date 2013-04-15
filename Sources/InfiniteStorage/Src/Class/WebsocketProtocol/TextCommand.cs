using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.WebsocketProtocol
{
	public class TextCommand
	{
		public string action { get; set; }
		public string file_name { get; set; }
		public long file_size { get; set; }
		public string mimetype { get; set; }
		public string UTI { get; set; }
		public string folder { get; set; }
		public DateTime datetime { get; set; }
		public string device_name { get; set; }
		public string device_id { get; set; }
		public int transfer_count { get; set; }


		public bool isFileStartCmd()
		{
			return "file-start".Equals(action);
		}

		public bool isFileEndCmd()
		{
			return "file-end".Equals(action);
		}

		public bool isConnectCmd(TextCommand cmd)
		{
			return "connect".Equals(action);
		}
	}
}
