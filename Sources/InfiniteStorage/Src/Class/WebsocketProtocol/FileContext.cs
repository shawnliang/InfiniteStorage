using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.WebsocketProtocol
{
	public class FileContext
	{
		public string file_name { get; set; }
		public long file_size { get; set; }
		public string mimetype { get; set; }
		public string UTI { get; set; }
		public string folder { get; set; }
		public DateTime datetime { get; set; }
	}
}
