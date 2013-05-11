using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.Notify
{
	public class NotifyChannelEventArgs : EventArgs
	{
		public SubscriptionContext Ctx { get; set; }
	}
}
