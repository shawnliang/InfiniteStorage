using System;

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
