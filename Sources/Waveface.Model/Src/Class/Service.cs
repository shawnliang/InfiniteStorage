#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

#endregion

namespace Waveface.Model
{
	public class Service : IService, INotifyPropertyChanged
	{
		#region Var

		private string _id;
		private string _name;
		private IServiceSupplier _supplier;
		private ObservableCollection<IContentEntity> _observableContents;

		#endregion

		#region Private Property

		private bool m_NeedRefresh { get; set; }

		private Lazy<IEnumerable<IContentEntity>> m_Contents { get; set; }

		//private Action<ObservableCollection<IContentEntity>> m_populateFunc;

		#endregion

		#region Protected Property

		/// <summary>
		/// Gets the m_ observable contents.
		/// </summary>
		/// <value>The m_ observable contents.</value>
		protected ObservableCollection<IContentEntity> m_ObservableContents
		{
			get
			{
				if (_observableContents == null)
				{
					_observableContents = new ObservableCollection<IContentEntity>();
					_observableContents.CollectionChanged += _observableContents_CollectionChanged;
				}

				return _observableContents;
			}
		}

		#endregion

		#region Public Property

		public virtual string ID
		{
			get { return _id ?? string.Empty; }
			protected set { _id = value; }
		}

		public virtual string Name
		{
			get
			{
				if (_name == null)
				{
					_name = GetType().Name;
				}

				return _name;
			}
			private set { _name = value; }
		}


		/// <summary>
		/// Gets the supplier.
		/// </summary>
		/// <value>The supplier.</value>
		public IServiceSupplier Supplier
		{
			get { return _supplier; }
			set { _supplier = value; }
		}

		/// <summary>
		/// Gets or sets the contents.
		/// </summary>
		/// <value>The contents.</value>
		public IEnumerable<IContentEntity> Contents
		{
			get { return m_Contents.Value; }
		}

		public string Description { get; private set; }

		public Uri Uri { get; protected set; }

		#endregion

		#region Event

		public event EventHandler<ContentPropertyChangeEventArgs> ContentPropertyChanged;

		#endregion

		#region Constructor

		public Service()
		{
			m_NeedRefresh = true;
		}

		public Service(string id, IServiceSupplier supplier, string name)
		{
			m_NeedRefresh = true;

			ID = id;
			Supplier = supplier;
			Name = name;
		}

		public Service(string id, IServiceSupplier supplier, string name, IEnumerable<IContentEntity> value)
		{
			m_NeedRefresh = true;

			ID = id;
			Supplier = supplier;
			Name = name;

			SetContents(value);
		}

		public Service(string id, IServiceSupplier supplier, string name, Action<ObservableCollection<IContentEntity>> func)
		{
			m_NeedRefresh = true;

			ID = id;
			Supplier = supplier;
			Name = name;

			SetContents(func);
		}

		#endregion

		#region Private Method

		protected void SetContents(Action<ObservableCollection<IContentEntity>> func)
		{
			//m_populateFunc = func;

			m_Contents = new Lazy<IEnumerable<IContentEntity>>(() =>
																   {
																	   //m_ObservableContents.Clear();

																	   if (m_NeedRefresh)
																	   {
																		   var newContents = new ObservableCollection<IContentEntity>();
																		   func(newContents);

																		   m_ObservableContents.RefreshTo(newContents);

																		   m_NeedRefresh = false;
																	   }

																	   foreach (var content in m_ObservableContents)
																		   (content as ContentEntity).Service = this;

																	   return m_ObservableContents;
																   });
		}

		public void SetContents(IEnumerable<IContentEntity> values)
		{
			SetContents((contents) => { contents.AddRange(values); });
		}

		#endregion

		protected void OnContentPropertyChanged(ContentPropertyChangeEventArgs e)
		{
			if (ContentPropertyChanged == null)
				return;

			ContentPropertyChanged(this, e);
		}

		#region Public Method

		public override string ToString()
		{
			return Name;
		}

		public void AddContent(IContentEntity content)
		{
			int index = 0;

			while (index < m_ObservableContents.Count && m_ObservableContents[index].Name.CompareTo(content.Name) > 0)
				index++;

			m_ObservableContents.Insert(index, content);
		}

		#endregion

		private void _observableContents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (IContentGroup item in e.NewItems)
				{
					item.ContentPropertyChanged += item_ContentPropertyChanged;
				}
			}
		}

		private void item_ContentPropertyChanged(object sender, ContentPropertyChangeEventArgs e)
		{
			OnContentPropertyChanged(e);
		}

		public virtual void Refresh()
		{
			m_Contents.ClearValue();

			m_NeedRefresh = true;
			//m_ObservableContents.Clear();
			//m_populateFunc(m_ObservableContents);

			OnPropertyChanged("Contents");
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name)
		{
			var handler = PropertyChanged;

			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}