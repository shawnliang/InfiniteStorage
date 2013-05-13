﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.Notify
{
	public class SubscriptionContext : ISubscriptionContext
	{
		private WebSocketSharp.Server.WebSocketService wsSvc;

		public string device_name { get; private set; }
		public string device_id { get; private set; }
		public long files_from_seq { get; set; }

		public SubscriptionContext(string dev_id, string dev_name, WebSocketSharp.Server.WebSocketService svc = null)
		{
			this.device_name = dev_name;
			this.device_id = dev_id;
			this.wsSvc = svc;
		}

		public void Send(string data)
		{
			wsSvc.Send(data);
		}
	}
}
