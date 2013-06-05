using InfiniteStorage.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Wammer.Utility;

namespace InfiniteStorage.Notify
{
	public interface INotifySenderUtil
	{
		List<FileChangeData> QueryChangedFiles(long from_seq);
		List<Label> QueryAllLabels();
		List<FileChangeData> QueryLabeledFiles(Guid label_id);
	}

	public enum Orientation {
		portrait,
		landscape,
	}

	public class FileChangeData
	{
		public Guid id { get; set; }
		public string file_name { get; set; }
		public string folder
		{
			get
			{
				return Path.GetDirectoryName(saved_path);
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

		public DateTime event_time { get; set; }

		[JsonIgnore]
		public int? _orientation { get; set; }

		public string original_path { get; set; }

		public Orientation? orientation
		{
			get
			{
				if (!_orientation.HasValue)
					return null;

				var needRotate90or270 =
					(_orientation == (int)ExifOrientations.LeftTop ||
					 _orientation == (int)ExifOrientations.RightTop ||
					 _orientation == (int)ExifOrientations.RightBottom ||
					 _orientation == (int)ExifOrientations.LeftBottom);

				if (needRotate90or270)
				{
					return (height > width) ? Orientation.landscape : Orientation.portrait;
				}
				else
				{
					return (width > height) ? Orientation.landscape : Orientation.portrait;
				}
			}
		}
	}
}
