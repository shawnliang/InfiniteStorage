using System;
using System.Threading;


namespace Waveface.Common
{
	public class NoReentrantTimer
	{
		private int period;
		private Timer timer;
		private TimerCallback cb;
		private bool stop;
		private object cs = new object();
		private int dueTime;
		private bool executed;

		public NoReentrantTimer(TimerCallback cb, object state, int dueTime, int period)
		{
			this.cb = cb;
			this.period = period;
			this.dueTime = dueTime;
			this.timer = new Timer(timeouted, state, Timeout.Infinite, Timeout.Infinite);
		}


		private void timeouted(object state)
		{
			try
			{
				cb(state);
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Periodical tasks failed", err);
			}

			lock (cs)
			{
				executed = true;

				if (!stop)
					timer.Change(period, Timeout.Infinite);
			}
		}

		public void Stop()
		{
			lock (cs)
			{
				stop = true;
				timer.Change(Timeout.Infinite, Timeout.Infinite);
			}
		}

		public void Start()
		{
			lock (cs)
			{
				stop = false;

				if (executed)
					timer.Change(period, Timeout.Infinite);
				else
					timer.Change(dueTime, Timeout.Infinite);
			}
		}
	}
}
