using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Waveface.Model
{
	/// <summary>
	/// 
	/// </summary>
	public class ContentGroup : ContentEntity, IContentGroup
	{
		#region Var
		private ObservableCollection<IContentEntity> _observableContents;
		#endregion


		#region Private Property
		/// <summary>
		/// Gets the m_ observable contents.
		/// </summary>
		/// <value>The m_ observable contents.</value>
		private ObservableCollection<IContentEntity> m_ObservableContents
		{
			get
			{
				if (_observableContents == null)
				{
					_observableContents = new ObservableCollection<IContentEntity>();
					_observableContents.CollectionChanged += new NotifyCollectionChangedEventHandler(_observableContents_CollectionChanged);
				}
				return _observableContents;
			}
		}

		private Lazy<IEnumerable<IContentEntity>> m_Contents { get; set; }
		#endregion


		#region Public Property
		public IEnumerable<IContentEntity> Contents
		{
			get
			{
				return m_Contents.Value;
			}
		}

		public virtual int ContentCount
		{
			get
			{
				return m_Contents.Value.Count();
			}
		}
		#endregion


		#region Event
		public event EventHandler<ContentPropertyChangeEventArgs> ContentPropertyChanged;
		#endregion


		#region Constructor
		public ContentGroup()
		{

		}

		public ContentGroup(string id, string name, Uri uri)
			: this(id, name, uri, (contents) => { })
		{
		}

		public ContentGroup(string id, string name, Uri uri, IEnumerable<IContentEntity> value)
			: this(id, name, uri)
		{
			SetContents(value);
		}

		public ContentGroup(string id, string name, Uri uri, Action<ObservableCollection<IContentEntity>> func)
			: base(id, name, uri)
		{
			SetContents(func);
		}

		public ContentGroup(string id, string name, Uri uri, Action<IContentGroup, ObservableCollection<IContentEntity>> func)
			: base(id, name, uri)
		{
			SetContents((contents) => func(this, contents));
		}
		#endregion


		#region Protected Method
		protected void OnContentPropertyChanged(ContentPropertyChangeEventArgs e)
		{
			if (ContentPropertyChanged == null)
				return;
			ContentPropertyChanged(this, e);
		}

		/// <summary>
		/// Sets the contents.
		/// </summary>
		/// <param name="func">The func.</param>
		protected void SetContents(Action<ObservableCollection<IContentEntity>> func)
		{
			m_Contents = new Lazy<IEnumerable<IContentEntity>>(() =>
			{
				m_ObservableContents.Clear();
				func(m_ObservableContents);
				return m_ObservableContents;
			});
		}

		protected void SetContents(IEnumerable<IContentEntity> values)
		{
			SetContents((contents) =>
			{
				contents.AddRange(values);
			});
		}
		#endregion


		#region Public Method
		public virtual void Refresh()
		{
			m_Contents.ClearValue();
			m_ObservableContents.Clear();
			OnPropertyChanged("ContentCount");
		}
		#endregion

		#region Event Process
		void _observableContents_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (ContentEntity item in e.NewItems)
				{
					item.Parent = this;
					item.PropertyChanged += item_PropertyChanged;

					var group = item as IContentGroup;
					if (group == null)
						continue;
					group.ContentPropertyChanged += group_ContentPropertyChanged;
				}
			}
		}

		void group_ContentPropertyChanged(object sender, ContentPropertyChangeEventArgs e)
		{
			OnContentPropertyChanged(e);
		}

		void item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			OnContentPropertyChanged(new ContentPropertyChangeEventArgs(sender as IContentEntity, e.PropertyName));
		}
		#endregion

	}
}
