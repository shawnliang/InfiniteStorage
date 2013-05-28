using System;
namespace Waveface.Model
{
	public class Content : ContentEntity, IContent
	{
		#region Protected Var
		protected bool _liked;
		#endregion


		#region Public Property
		public virtual bool Liked
		{
			get
			{
				return _liked;
			}
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
			this.Uri = location;
		}

		public Content(string id, Uri uri)
			: this(id, System.IO.Path.GetFileName(uri.LocalPath), uri)
		{
		}

		public Content(Uri uri)
			: this(uri.GetHashCode().ToString(), uri)
		{
		}
		#endregion
	}
}
