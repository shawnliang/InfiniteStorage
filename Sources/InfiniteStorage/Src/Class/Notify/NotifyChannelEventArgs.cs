using System;

namespace InfiniteStorage.Notify
{
	public class NotifyChannelEventArgs : EventArgs
	{
		public SubscriptionContext Ctx { get; set; }
	}
}
