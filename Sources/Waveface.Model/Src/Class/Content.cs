#region

using System;
using System.IO;

#endregion

namespace Waveface.Model
{
	public class Content : ContentEntity, IContent
	{
		#region Protected Var

		protected ContentType? _type;
		protected bool _liked;

		#endregion

		#region Public Property

		public ContentType Type
		{
			get
			{
				if (!_type.HasValue)
				{
					_type = (Uri.LocalPath.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase) || Uri.LocalPath.EndsWith(".gif", StringComparison.CurrentCultureIgnoreCase))
								? ContentType.Photo
								: ContentType.Video;
				}

				return _type.Value;
			}
		}

		public virtual bool Liked
		{
			get { return _liked; }
			set
			{
				if (_liked == value)
					return;

				_liked = value;
				OnPropertyChanged("Liked");
			}
		}

		#endregion

		#region Constructor

		public Content()
		{
		}

		public Content(string id, string name, Uri location)
			: base(id, name, location)
		{
			Uri = location;
		}

		public Content(string id, Uri uri)
			: this(id, Path.GetFileName(uri.LocalPath), uri)
		{
		}

		public Content(Uri uri)
			: this(uri.GetHashCode().ToString(), uri)
		{
		}

		#endregion
	}
}