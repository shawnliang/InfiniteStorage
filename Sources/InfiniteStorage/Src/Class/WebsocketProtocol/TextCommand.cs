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

		public bool isFileStartCmd()
		{
			return "file-start".Equals(action);
		}

		public bool isFileEndCmd()
		{
			return "file-end".Equals(action);
		}
	}
}
