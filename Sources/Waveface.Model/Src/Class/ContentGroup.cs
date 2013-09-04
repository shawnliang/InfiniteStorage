#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

#endregion

namespace Waveface.Model
{
	public class ContentGroup : ContentEntity, IContentGroup
	{
		#region Var

		private ObservableCollection<IContentEntity> _observableContents;
		private ReadOnlyObservableCollection<IContentEntity> _readOnlyObservableContents;
		private Action<ObservableCollection<IContentEntity>> _setContentAction;

		#endregion

		#region Private Property

		private bool m_NeedRefresh { get; set; }

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

		public ReadOnlyObservableCollection<IContentEntity> Contents
		{
			get
			{
				if (_readOnlyObservableContents == null)
				{
					_readOnlyObservableContents = new ReadOnlyObservableCollection<IContentEntity>(m_ObservableContents);
				}

				if (m_NeedRefresh)
				{
					var newContents = new ObservableCollection<IContentEntity>();
					_setContentAction(newContents);

					m_ObservableContents.RefreshTo(newContents);

					foreach (var content in m_ObservableContents)
						content.Refresh();

					m_NeedRefresh = false;
				}
				return _readOnlyObservableContents;
			}
			private set { _readOnlyObservableContents = value; }
		}

		public virtual int ContentCount
		{
			get { return Contents.Count; }
		}

		#endregion

		#region Event

		public event EventHandler<ContentPropertyChangeEventArgs> ContentPropertyChanged;

		#endregion

		#region Constructor

		public ContentGroup()
		{
			m_NeedRefresh = true;
		}

		public ContentGroup(string id, string name, Uri uri)
			: this(id, name, uri, (contents) => { })
		{
			m_NeedRefresh = true;
		}

		public ContentGroup(string id, string name, Uri uri, IEnumerable<IContentEntity> value)
			: this(id, name, uri)
		{
			m_NeedRefresh = true;

			SetContents(value);
		}

		public ContentGroup(string id, string name, Uri uri, Action<ObservableCollection<IContentEntity>> func)
			: base(id, name, uri)
		{
			m_NeedRefresh = true;

			SetContents(func);
		}

		public ContentGroup(string id, string name, Uri uri, Action<IContentGroup, ObservableCollection<IContentEntity>> func)
			: base(id, name, uri)
		{
			m_NeedRefresh = true;

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
			_setContentAction = (contents) =>
									{
										contents.Clear();
										func(contents);

										foreach (var content in contents)
											(content as ContentEntity).Service = Service;
									};
		}

		protected void SetContents(IEnumerable<IContentEntity> values)
		{
			SetContents((contents) => { contents.AddRange(values); });
		}

		#endregion

		#region Public Method

		public override void Refresh()
		{
			//Contents = null;
			//m_ObservableContents.Clear();

			m_NeedRefresh = true;
			OnPropertyChanged("ContentCount");
		}

		#endregion

		#region Event Process

		private void _observableContents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

		private void group_ContentPropertyChanged(object sender, ContentPropertyChangeEventArgs e)
		{
			OnContentPropertyChanged(e);
		}

		private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnContentPropertyChanged(new ContentPropertyChangeEventArgs(sender as IContentEntity, e.PropertyName));
		}

		#endregion
	}
}