#region

using System;
using System.Threading;
using log4net;

#endregion

namespace Waveface.Common
{
	public class NoReentrantTimer
	{
		private int m_period;
		private Timer m_timer;
		private TimerCallback m_cb;
		private bool m_stop;
		private object m_cs = new object();
		private int m_dueTime;
		private bool m_executed;

		public NoReentrantTimer(TimerCallback cb, object state, int dueTime, int period)
		{
			m_cb = cb;
			m_period = period;
			m_dueTime = dueTime;
			m_timer = new Timer(timeouted, state, Timeout.Infinite, Timeout.Infinite);
		}

		private void timeouted(object state)
		{
			try
			{
				m_cb(state);
			}
			catch (Exception err)
			{
				LogManager.GetLogger(GetType()).Warn("Periodical tasks failed", err);
			}

			lock (m_cs)
			{
				m_executed = true;

				if (!m_stop)
					m_timer.Change(m_period, Timeout.Infinite);
			}
		}

		public void Stop()
		{
			lock (m_cs)
			{
				m_stop = true;
				m_timer.Change(Timeout.Infinite, Timeout.Infinite);
			}
		}

		public void Start()
		{
			lock (m_cs)
			{
				m_stop = false;

				if (m_executed)
					m_timer.Change(m_period, Timeout.Infinite);
				else
					m_timer.Change(m_dueTime, Timeout.Infinite);
			}
		}
	}
}