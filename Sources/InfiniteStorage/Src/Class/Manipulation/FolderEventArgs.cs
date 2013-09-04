using System;

namespace InfiniteStorage.Manipulation
{
	class FolderEventArgs : EventArgs
	{
		public string name { get; set; }
		public string parent_folder { get; set; }
		public string path { get; set; }
	}
}
