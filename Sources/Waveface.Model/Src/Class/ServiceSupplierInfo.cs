using System;
using System.Drawing;

namespace Waveface.Model
{
	/// <summary>
	/// 
	/// </summary>
	public class ServiceSupplierInfo
	{
		#region Var
		private String _name;
		private String _officialUrl;
		private String _comment;
		#endregion

		#region Property
		/// <summary>
		/// Gets or sets the logo.
		/// </summary>
		/// <value>The logo.</value>
		public Image Logo
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public String Name
		{
			get
			{
				if (_name == null)
					return String.Empty;
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Gets or sets the official URL.
		/// </summary>
		/// <value>The official URL.</value>
		public String OfficialUrl
		{
			get
			{
				if (_officialUrl == null)
					return String.Empty;
				return _officialUrl;
			}
			set
			{
				_officialUrl = value;
			}
		}

		/// <summary>
		/// Gets or sets the comment.
		/// </summary>
		/// <value>The comment.</value>
		public String Comment
		{
			get
			{
				if (_comment == null)
					return String.Empty;
				return _comment;
			}
			set
			{
				_comment = value;
			}
		}
		#endregion
	}
}
