using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.WebsocketProtocol
{
	public class ThumbnailReceivedEventArgs : EventArgs
	{
		public string thumbnail_path { get; private set; }
		public string device_id { get; private set; }

		public ThumbnailReceivedEventArgs(string thumbnail_path, string device_id)
		{
			this.thumbnail_path = thumbnail_path;
			this.device_id = device_id;
		}
	}
}
