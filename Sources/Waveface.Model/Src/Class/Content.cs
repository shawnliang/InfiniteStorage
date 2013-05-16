using System;
namespace Waveface.Model
{
	public class Content : ContentEntity, IContent
	{
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
