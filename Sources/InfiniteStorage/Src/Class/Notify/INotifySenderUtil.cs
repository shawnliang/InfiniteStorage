using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteStorage.Model;
using Newtonsoft.Json;
using System.IO;

namespace InfiniteStorage.Notify
{
	public interface INotifySenderUtil
	{
		IEnumerable<FileChangeData> QueryChangedFiles(long from_seq);
	}


	public class FileChangeData
	{
		public Guid id { get; set; }
		public string file_name { get; set; }
		public string folder
		{
			get
			{
				var folder = Path.GetDirectoryName(saved_path);
				var folderElems = folder.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

				return "/" + device_folder + "/" + string.Join("/", folderElems);
			}
		}

		[JsonIgnore]
		public string saved_path { get; set; }
		[JsonIgnore]
		public string device_folder { get; set; }

		public bool? thumb_ready { get; set; }
		public long width { get; set; }
		public long height { get; set; }
		public long size { get; set; }
		public int type { get; set; }

		public string dev_id { get; set; }
		public string dev_name { get; set; }
		public int dev_type { get; set; }

		public bool? deleted { get; set; }
		public long seq { get; set; }
	}
}
