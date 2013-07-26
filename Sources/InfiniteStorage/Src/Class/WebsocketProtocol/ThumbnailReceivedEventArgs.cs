using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.WebsocketProtocol
{
	public class ThumbnailReceivedEventArgs : EventArgs
	{
		public string thumbnail_path { get; private set; }
		public ProtocolContext ctx { get; private set; }
		public int transfer_count { get; private set; }

		public ThumbnailReceivedEventArgs(string thumbnail_path, ProtocolContext ctx, int transfer_count)
		{
			this.thumbnail_path = thumbnail_path;
			this.ctx = ctx;
			this.transfer_count = transfer_count;
		}
	}
}
