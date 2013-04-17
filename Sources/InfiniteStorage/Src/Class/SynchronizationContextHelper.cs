using System;
using System.Threading;
using System.Windows.Forms;

namespace InfiniteStorage
{
	public static class SynchronizationContextHelper
	{
		#region Private Static Property
		/// <summary>
		/// Gets the m_ main sync context.
		/// </summary>
		/// <value>
		/// The m_ main sync context.
		/// </value>
		private static SynchronizationContext m_MainSyncContext { get; set; }
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes the <see cref="SynchronizationContextHelper" /> class.
		/// </summary>
		static SynchronizationContextHelper()
		{
			m_MainSyncContext = SynchronizationContext.Current;
			if (m_MainSyncContext == null)
			{
				using (var form = new Form())
				{
					m_MainSyncContext = SynchronizationContext.Current;
				}
			}
		}
		#endregion


		#region Public Static Method
		/// <summary>
		/// Sets the main sync context.
		/// </summary>
		public static void SetMainSyncContext()
		{
			m_MainSyncContext = SynchronizationContext.Current;
		}


		/// <summary>
		/// Sends the sync context.
		/// </summary>
		/// <param name="target">The target.</param>
		public static void SendMainSyncContext(Action target)
		{
			m_MainSyncContext.Send((obj) => target(), null);
		}

		/// <summary>
		/// Sends the sync context.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target">The target.</param>
		/// <param name="o">The o.</param>
		public static void SendMainSyncContext<T>(Action<T> target, Object o)
		{
			m_MainSyncContext.Send((obj) => target((T)obj), o);
		}

		/// <summary>
		/// Posts the main sync context.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target">The target.</param>
		public static void PostMainSyncContext(Action target)
		{
			m_MainSyncContext.Post((obj) => target(), null);
		}

		/// <summary>
		/// Posts the sync context.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target">The target.</param>
		/// <param name="o">The o.</param>
		public static void PostMainSyncContext<T>(Action<T> target, Object o)
		{
			m_MainSyncContext.Post((obj) => target((T)obj), o);
		}
		#endregion
	}
}
