
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
namespace Waveface.Model
{
	/// <summary>
	/// 
	/// </summary>
	public class ContentEntity : IContentEntity
	{
		#region Var
		private string _id;
		private string _name;
		private IContentEntity _parent;
		private string _contentPath;
		private Image _image;
		private string _description;
		private Dictionary<string, string> _memo;
		#endregion

		#region Public Property
		/// <summary>
		/// Gets the ID.
		/// </summary>
		/// <value>
		/// The ID.
		/// </value>
		public string ID
		{
			get
			{
				return _id ?? string.Empty;
			}
			protected set
			{
				_id = value;
			}
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name
		{
			get
			{
				return _name ?? string.Empty;
			}
			protected set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Gets the location.
		/// </summary>
		/// <value>
		/// The location.
		/// </value>
		public Uri Uri
		{
			get;
			protected set;
		}


		/// <summary>
		/// Gets or sets the parent.
		/// </summary>
		/// <value>The parent.</value>
		public IContentEntity Parent
		{
			get { return _parent; }
			internal set { _parent = value; }
		}

		public string ContentPath
		{
			get
			{
				if (_contentPath == null)
					_contentPath = GetContentPath();
				return _contentPath;
			}
		}

		public Image Image
		{
			get
			{
				if (!File.Exists(Uri.LocalPath))
					return null;
				return _image ?? (_image = Image.FromFile(Uri.LocalPath));
			}
		}

		public System.Drawing.Image Thumbnail
		{
			get;
			internal set;
		}

		public long Size
		{
			get;
			internal set;
		}

		public System.DateTime CreateTime
		{
			get;
			internal set;
		}

		public System.DateTime ModifyTime
		{
			get;
			internal set;
		}

		public string Description
		{
			get
			{
				return _description ?? string.Empty;
			}
			set
			{
				_description = value;
			}
		}

		public System.Collections.Generic.Dictionary<string, string> Memo
		{
			get
			{
				if (_memo == null)
					_memo = new Dictionary<string, string>();
				return _memo;
			}
			set
			{
				_memo = value;
			}
		}
		#endregion


		#region Constructor
		public ContentEntity()
		{

		}

		public ContentEntity(string id, string name, Uri uri)
		{
			this.ID = id;
			this.Name = name;
			this.Uri = uri;
		}

		public ContentEntity(string name, Uri uri)
			: this(uri.GetHashCode().ToString(), name, uri)
		{
		}
		#endregion


		#region Private Method
		/// <summary>
		/// Gets the content path.
		/// </summary>
		/// <returns></returns>
		private string GetContentPath()
		{
			return (Parent == null) ? 
				string.Format(@"{0}", this.Name) :
				string.Format(@"{0}\{1}", Parent.ContentPath, this.Name);
		}
		#endregion
	}
}
