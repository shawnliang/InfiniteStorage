using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Waveface.Client
{
	class TestData
	{
		public ImageSource Image { get; set; }
		public String Text { get; set; }

		public Uri ImageUri { get; set; }
		public String AlbumName { get; set; }
		public String AlbumID { get; set; }
		public bool IsAddToNewAlbum { get; set; }
	}

	class TestList : List<TestData>
	{
	}
}
