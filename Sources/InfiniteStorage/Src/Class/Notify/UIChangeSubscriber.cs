using System;
using System.Collections.Generic;
using System.Linq;

namespace InfiniteStorage.Notify
{
	class UIChangeSubscriber
	{
		private static UIChangeSubscriber instance = new UIChangeSubscriber();

		private object cs = new object();
		private List<SubscriptionContext> subscribers = new List<SubscriptionContext>();

		public static UIChangeSubscriber Instance
		{
			get { return instance; }
		}

		public void Add(SubscriptionContext ctx)
		{
			lock (cs)
			{
				if (!subscribers.Contains(ctx))
					subscribers.Add(ctx);
			}
		}

		public void Remove(SubscriptionContext ctx)
		{
			lock (cs)
			{
				subscribers.Remove(ctx);
			}
		}

		public void SendMsg(string data)
		{
			List<SubscriptionContext> peers;
			lock (cs)
			{
				peers = subscribers.ToList();
			}

			foreach (var peer in peers)
			{
				try
				{
					peer.Send(data);
				}
				catch (Exception err)
				{
					log4net.LogManager.GetLogger(GetType()).Warn("Unable to notify ui change", err);
				}
			}
		}
	}
}
