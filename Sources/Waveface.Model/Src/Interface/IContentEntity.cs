using System;
using System.Collections.Generic;
using System.Drawing;

namespace Waveface.Model
{
	public interface IContentEntity
	{
		#region Property
		/// <summary>
		/// Gets the ID.
		/// </summary>
		/// <value>
		/// The ID.
		/// </value>
		String ID { get; }

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		string Name { get; }

		/// <summary>
		/// Gets the URI.
		/// </summary>
		/// <value>
		/// The URI.
		/// </value>
		Uri Uri { get; }


		/// <summary>
		/// Gets the parent.
		/// </summary>
		/// <value>
		/// The parent.
		/// </value>
		IContentEntity Parent { get; }

		/// <summary>
		/// Gets the content path.
		/// </summary>
		/// <value>
		/// The content path.
		/// </value>
		string ContentPath { get; }

		/// <summary>
		/// Gets the image.
		/// </summary>
		/// <value>
		/// The image.
		/// </value>
		Image Image { get; }


		/// <summary>
		/// Gets the thumbnails.
		/// </summary>
		/// <value>
		/// The thumbnails.
		/// </value>
		Image Thumbnail { get; }


		/// <summary>
		/// Gets or sets the size.
		/// </summary>
		/// <value>
		/// The size.
		/// </value>
		long Size { get; }

		/// <summary>
		/// Gets the create time.
		/// </summary>
		/// <value>
		/// The create time.
		/// </value>
		DateTime CreateTime { get; }

		/// <summary>
		/// Gets the modify time.
		/// </summary>
		/// <value>
		/// The modify time.
		/// </value>
		DateTime ModifyTime { get; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		String Description { get; set; }

		/// <summary>
		/// Gets or sets the memo.
		/// </summary>
		/// <value>
		/// The memo.
		/// </value>
		Dictionary<String, String> Memo { get; set; }
		#endregion
	}
}
