using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.Notify
{
	class Notifier
	{
		private List<NotifySender> senders = new List<NotifySender>();
		private object cs = new object();
		private System.Timers.Timer timer = new System.Timers.Timer(500.0);

		public Notifier()
		{
			timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
		}

		public void Start()
		{
			timer.Start();
		}

		public void Stop()
		{
			timer.Stop();
		}

		public void OnSubscribing(object sender, NotifyChannelEventArgs arg)
		{
			var channel = new NotifySender(arg.Ctx, new NotifySenderUtil());

			lock (cs)
			{
				senders.Add(channel);
			}
		}

		public void OnChannelDisconnected(object sender, NotifyChannelEventArgs arg)
		{
			lock (cs)
			{
				var channel = senders.Where(x => x.ctx == arg).FirstOrDefault();

				if (channel != null)
					senders.Remove(channel);
			}
		}

		void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			timer.Stop();
			try
			{
				List<NotifySender> allChannels = null;

				lock (cs)
				{
					allChannels = senders.ToList();
				}

				foreach (var channel in allChannels)
				{
					try
					{
						channel.Notify();
					}
					catch (Exception err)
					{
						log4net.LogManager.GetLogger(GetType()).Warn("unable to notify:", err);
					}
				}
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("unable to notify:", err);
			}
			finally
			{
				timer.Start();
			}
		}
	}
}
