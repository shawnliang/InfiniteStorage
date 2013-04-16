using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.WebsocketProtocol
{
	public class WebsocketEventArgs : EventArgs
	{
		public ProtocolContext ctx { get; set; }

		public WebsocketEventArgs(ProtocolContext ctx)
		{
			this.ctx = ctx;
		}
	}
}
