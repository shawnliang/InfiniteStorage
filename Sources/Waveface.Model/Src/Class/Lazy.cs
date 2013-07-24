#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

#endregion

namespace Waveface.Model
{
	internal interface ILazy
	{
		#region Method

		void Init();
		void BeginInit();
		//IAsyncResult BeginInit();
		//IAsyncResult BeginInit(AsyncCallback callBack);
		void EndInit();
		//void EndInit(IAsyncResult asyncResult);

		void CancelInit();

		#endregion
	}

	public class Lazy<T> : ILazy
	{
		#region Delegate

		private delegate void InitMethodDelegate();

		#endregion

		#region Static Var

		private static bool _enableBackgroundInit;
		private static Thread _backgroundInitThread;
		private static object _unInitLazysLockKey;
		private static List<ILazy> _unInitLazys;

		#endregion

		#region Var

		private SynchronizationContext _syncContext = SynchronizationContext.Current;
		private Func<T> _func;
		private bool _isValueCreated;
		//private InitMethodDelegate _InitDelegate;
		private Thread _initThread;

		#endregion

		#region Private Static Property

		/// <summary>
		/// Gets the m_ background init thread.
		/// </summary>
		/// <value>The m_ background init thread.</value>
		private static Thread m_BackgroundInitThread
		{
			get
			{
				if (_backgroundInitThread == null)
				{
					_backgroundInitThread = new Thread(() =>
						                                   {
							                                   while (true)
							                                   {
								                                   ILazy lazyObj = m_UnInitLazys.FirstOrDefault();
								                                   if (lazyObj != null)
								                                   {
									                                   try
									                                   {
										                                   lazyObj.Init();
									                                   }
									                                   catch (Exception)
									                                   {
										                                   m_UnInitLazys.Remove(lazyObj);
										                                   m_UnInitLazys.Add(lazyObj);
									                                   }
								                                   }
								                                   //else
								                                   //{
								                                   Thread.Sleep(100);
								                                   Application.DoEvents();
								                                   //}
							                                   }
						                                   }) {IsBackground = true, Priority = ThreadPriority.Lowest};
				}
				return _backgroundInitThread;
			}
			set
			{
				if (_backgroundInitThread == value)
					return;

				if (_backgroundInitThread != null && value == null)
				{
					try
					{
						m_BackgroundInitThread.Abort();
					}
					catch
					{
					}
				}
				_backgroundInitThread = value;
			}
		}

		/// <summary>
		/// Gets the m_ un init lazys lock key.
		/// </summary>
		/// <value>The m_ un init lazys lock key.</value>
		private static object m_UnInitLazysLockKey
		{
			get
			{
				if (_unInitLazysLockKey == null)
					_unInitLazysLockKey = new object();
				return _unInitLazysLockKey;
			}
		}

		/// <summary>
		/// Gets the m_ un init lazys.
		/// </summary>
		/// <value>The m_ un init lazys.</value>
		private static List<ILazy> m_UnInitLazys
		{
			get
			{
				lock (m_UnInitLazysLockKey)
				{
					if (_unInitLazys == null)
						_unInitLazys = new List<ILazy>();
					return _unInitLazys;
				}
			}
		}

		#endregion

		#region Public Static Property

		public static bool EnableBackgroundInit
		{
			get { return _enableBackgroundInit; }
			set
			{
				if (_enableBackgroundInit == value)
					return;
				_enableBackgroundInit = value;
				if (value)
					m_BackgroundInitThread.Start();
				else
				{
					m_BackgroundInitThread = null;
				}
			}
		}

		#endregion

		#region Private Property

		///// <summary>
		///// Gets or sets the m_ async result.
		///// </summary>
		///// <value>The m_ async result.</value>
		//private IAsyncResult m_AsyncResult { get; set; }

		/// <summary>
		/// Gets the m_ sync context.
		/// </summary>
		/// <value>The m_ sync context.</value>
		private SynchronizationContext m_SyncContext
		{
			get
			{
				if (_syncContext == null)
					_syncContext = new SynchronizationContext();
				return _syncContext;
			}
		}

		/// <summary>
		/// Gets or sets the m_ func.
		/// </summary>
		/// <value>The m_ func.</value>
		private Func<T> m_Func
		{
			get { return _func; }
			set
			{
				if (_func != value)
				{
					_func = value;
					IsValueCreated = false;
				}
			}
		}

		/// <summary>
		/// Gets or sets the m_ result.
		/// </summary>
		/// <value>The m_ result.</value>
		private T m_Result { get; set; }


		///// <summary>
		///// Gets the m_ init delegate.
		///// </summary>
		///// <value>The m_ init delegate.</value>
		//private InitMethodDelegate m_InitDelegate
		//{
		//    get
		//    {
		//        if (_InitDelegate == null)
		//            _InitDelegate = new InitMethodDelegate(() =>
		//            {
		//                try
		//                {
		//                    Init();
		//                }
		//                catch 
		//                {
		//                    ClearValue();
		//                }
		//            });
		//        return _InitDelegate;
		//    }
		//}
		private Thread m_InitThread
		{
			get
			{
				if (_initThread == null)
				{
					_initThread = new Thread(() =>
						                         {
							                         try
							                         {
								                         Init();
							                         }
							                         catch
							                         {
								                         m_Result = default(T);
								                         IsValueCreated = false;
								                         IsIniting = false;
								                         _initThread = null;
							                         }
						                         });
					_initThread.Priority = ThreadPriority.Highest;
				}
				return _initThread;
			}
			set
			{
				if (_initThread == value)
					return;

				if (_initThread != null && _initThread.IsAlive)
				{
					try
					{
						_initThread.Abort();
					}
					catch (Exception e)
					{
					}
					finally
					{
						m_Result = default(T);
						IsValueCreated = false;
						IsIniting = false;
						_initThread = null;
					}
				}

				_initThread = value;
			}
		}

		#endregion

		#region Public Property

		/// <summary>
		/// Gets or sets the initial value.
		/// </summary>
		/// <value>The initial value.</value>
		public T InitialValue { get; set; }

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public T Value
		{
			get
			{
				if (!IsValueCreated)
				{
					if (InitialValue != null && !(InitialValue.Equals(default(T))))
					{
						if (!IsIniting)
							BeginInit();
						return InitialValue;
					}
					Init();
				}
				return m_Result;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is value created.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is value created; otherwise, <c>false</c>.
		/// </value>
		public Boolean IsValueCreated
		{
			get { return _isValueCreated; }
			private set
			{
				if (_isValueCreated == value)
					return;

				_isValueCreated = value;

				if (_isValueCreated)
					OnValueInited(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is initing.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is initing; otherwise, <c>false</c>.
		/// </value>
		public Boolean IsIniting { get; private set; }

		#endregion

		#region Event

		public event EventHandler ValueInited;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Lazy&lt;T&gt;"/> class.
		/// </summary>
		public Lazy()
			: this(default(T))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Lazy&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="func">The func.</param>
		public Lazy(Func<T> func)
		{
			ValueInited += Lazy_ValueInited;
			SetValue(func);
			m_UnInitLazys.Add(this);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Lazy&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="value">The value.</param>
		public Lazy(T value)
			: this(() => value)
		{
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="Lazy&lt;T&gt;"/> is reclaimed by garbage collection.
		/// </summary>
		~Lazy()
		{
			m_UnInitLazys.Remove(this);
		}

		#endregion

		#region Private Method

		/// <summary>
		/// Resets this instance.
		/// </summary>
		private void Reset()
		{
			m_Func = null;
			InitialValue = default(T);
			ClearValue();
		}

		/// <summary>
		/// Raises the event.
		/// </summary>
		/// <param name="handler">The handler.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void RaiseEvent(EventHandler handler, EventArgs e)
		{
			if (handler == null)
				return;
			PostSyncContext<EventArgs>((eventArgs) => handler(this, eventArgs), e);
		}

		/// <summary>
		/// Raises the event.
		/// </summary>
		/// <typeparam name="TEventArgs">The type of the event args.</typeparam>
		/// <param name="handler">The handler.</param>
		/// <param name="e">The <see cref="TEventArgs"/> instance containing the event data.</param>
		private void RaiseEvent<TEventArgs>(EventHandler<TEventArgs> handler, TEventArgs e) where TEventArgs : EventArgs
		{
			if (handler == null)
				return;
			PostSyncContext<TEventArgs>((eventArgs) => handler(this, eventArgs), e);
		}


		/// <summary>
		/// Posts the sync context.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target">The target.</param>
		/// <param name="o">The o.</param>
		private void PostSyncContext<T>(Action<T> target, Object o)
		{
			m_SyncContext.Post((obj) => target((T) obj), o);
		}

		/// <summary>
		/// Needs the init.
		/// </summary>
		/// <returns></returns>
		private Boolean NeedInit()
		{
			return !IsIniting && !IsValueCreated && m_Func != null;
		}

		#endregion

		#region Protected Method

		/// <summary>
		/// Raises the <see cref="E:ValueInited"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnValueInited(EventArgs e)
		{
			RaiseEvent(ValueInited, e);
		}

		#endregion

		#region Public Method

		/// <summary>
		/// Clears the value.
		/// </summary>
		public void ClearValue()
		{
			m_Result = default(T);
			IsValueCreated = false;
			IsIniting = false;
			m_InitThread = null;
		}

		/// <summary>
		/// Inits the value.
		/// </summary>
		public void Init()
		{
			if (IsValueCreated)
				return;

			if (IsIniting)
			{
				EndInit();
				return;
			}

			if (!NeedInit())
				return;

			IsIniting = true;
			m_Result = m_Func();
			IsValueCreated = true;
		}

		///// <summary>
		///// Inits the value.
		///// </summary>
		//public void Init()
		//{
		//    if (IsIniting && m_AsyncResult != null)
		//    {
		//        EndInit();
		//        return;
		//    }

		//    if (!NeedInit())
		//        return;

		//    IsIniting = true;
		//    m_Result = m_Func();
		//    IsValueCreated = true;
		//}

		public void BeginInit()
		{
			lock (this)
			{
				if (IsIniting || IsValueCreated)
					return;

				//if (m_InitThread.IsAlive)
				//    return;

				m_InitThread.Start();
			}
		}

		///// <summary>
		///// Begins the init.
		///// </summary>
		///// <returns></returns>
		//public IAsyncResult BeginInit()
		//{
		//    return BeginInit(null);
		//}

		///// <summary>
		///// Begins the init.
		///// </summary>
		///// <param name="callBack">The call back.</param>
		///// <returns></returns>
		//public IAsyncResult BeginInit(AsyncCallback callBack)
		//{
		//    if (IsIniting && m_AsyncResult != null)
		//        return m_AsyncResult;            

		//    return m_AsyncResult = m_InitDelegate.BeginInvoke(callBack, null);			
		//}

		/// <summary>
		/// Ends the init.
		/// </summary>
		public void EndInit()
		{
			lock (this)
			{
				if (!m_InitThread.IsAlive)
					return;

				m_InitThread.Join();
			}
		}

		///// <summary>
		///// Ends the init.
		///// </summary>
		//public void EndInit()
		//{
		//    EndInit(m_AsyncResult);
		//}

		///// <summary>
		///// Ends the init.
		///// </summary>
		///// <param name="asyncResult">The async result.</param>
		//public void EndInit(IAsyncResult asyncResult)
		//{            
		//    m_InitDelegate.EndInvoke(asyncResult);
		//}

		public void CancelInit()
		{
			lock (this)
			{
				if (!m_InitThread.IsAlive)
					return;

				m_InitThread = null;
			}
		}

		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <param name="func">The func.</param>
		public void SetValue(Func<T> func)
		{
			m_Func = func;
		}

		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void SetValue(T value)
		{
			SetValue(() => value);
		}

		#endregion

		#region Event Process

		/// <summary>
		/// Handles the ValueInited event of the Lazy control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Lazy_ValueInited(object sender, EventArgs e)
		{
			//m_AsyncResult = null;
			IsIniting = false;
			m_UnInitLazys.Remove(this);
		}

		#endregion
	}
}