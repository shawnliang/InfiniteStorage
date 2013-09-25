#region

using System;
using System.Threading;
using System.Windows.Forms;

#endregion

namespace InfiniteStorage.Win32
{
	public static class SynchronizationContextHelper
	{
		private static SynchronizationContext s_mainSyncContext { get; set; }

		static SynchronizationContextHelper()
		{
			s_mainSyncContext = SynchronizationContext.Current;

			if (s_mainSyncContext == null)
			{
				using (var form = new Form())
				{
					s_mainSyncContext = SynchronizationContext.Current;
				}
			}
		}

		public static void SetMainSyncContext()
		{
			s_mainSyncContext = SynchronizationContext.Current;
		}

		public static void SendMainSyncContext(Action target)
		{
			s_mainSyncContext.Send(obj => target(), null);
		}

		public static void SendMainSyncContext<T>(Action<T> target, Object o)
		{
			s_mainSyncContext.Send(obj => target((T) obj), o);
		}

		public static void PostMainSyncContext(Action target)
		{
			s_mainSyncContext.Post(obj => target(), null);
		}

		public static void PostMainSyncContext<T>(Action<T> target, Object o)
		{
			s_mainSyncContext.Post(obj => target((T) obj), o);
		}
	}
}