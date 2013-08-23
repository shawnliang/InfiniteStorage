using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Waveface.Model;

namespace Waveface.Client
{
	class CreateNewAlbumContentEntity : ContentGroup
	{
		public CreateNewAlbumContentEntity(string name)
			: base("CreateNewAlbumContentEntity", name, new Uri(@"C:\"))
		{
			SetContents(new List<IContentEntity> { new NewAlbumContentEntity() });
		}
	}

	class NewAlbumContentEntity : Content
	{
		public NewAlbumContentEntity()
			: base("", "", new Uri("pack://application:,,,/Resource/bar2_source_0.png"))
		{
		}

		public BitmapSource ThumbnailSource
		{
			get
			{
				return BitmapFrame.Create(this.Uri);
			}
		}
	}

}
