using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.Share
{
	class PreviewData
	{
		public int fps { get; set; }
		public List<PreviewFrame> seq { get; set; }
	}

	class PreviewFrame
	{
		public string url { get; set; }
		public float duration { get; set; }
	}
}
