#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InfiniteStorage.Model;
using Microsoft.Win32;
using Waveface.Model;

#endregion

namespace Waveface.Client
{
	public class RT
	{
		public List<List<FileChange>> Events { get; set; }
		public RtData RtData { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public Dictionary<string, DateTime> DateTimeCache { get; set; }

		public bool Init(List<FileAsset> files, IService device)
		{
			RtData = new RtData();

			string _basePath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "ResourceFolder", "");
			string _ThumbsPath = Path.Combine(_basePath, ".thumbs");

			List<FileChange> _fCs = files.Select(x => new FileChange
														  {
															  id = x.file_id.ToString(),
															  file_name = x.file_name,
															  tiny_path =
																  (x.type == (int)FileAssetType.image)
																	  ? Path.Combine(_ThumbsPath, x.file_id + ".tiny.thumb")
																	  : Path.Combine(_ThumbsPath, x.file_id + ".medium.thumb"),
															  taken_time = x.event_time.ToString("yyyy-MM-dd HH:mm:ss"),
															  width = x.width,
															  height = x.height,
															  size = x.file_size,
															  type = x.type,
															  saved_path = Path.Combine(_basePath, x.saved_path)
														  }).ToList();

			RtData.file_changes = _fCs;

			if (RtData.file_changes.Count == 0)
			{
				return false;
			}

			SortRowDate();
			ParserDate();


			StartDate = DateTimeCache[RtData.file_changes[0].taken_time].Date;
			EndDate = DateTimeCache[RtData.file_changes[RtData.file_changes.Count - 1].taken_time].Date;

			return true;
		}

		private void SortRowDate()
		{
			RtData.file_changes = RtData.file_changes.OrderBy(o => o.taken_time).ToList();
		}

		private void ParserDate()
		{
			DateTimeCache = new Dictionary<string, DateTime>();

			foreach (FileChange _item in RtData.file_changes)
			{
				string _dt = _item.taken_time;

				if (!DateTimeCache.ContainsKey(_dt))
				{
					DateTimeCache.Add(_dt, DateTime.Parse(_dt));
				}
			}
		}

		public void GroupingByEvent()
		{
			BY_MONTH();
		}

		private void BY_MONTH()
		{
			Dictionary<string, List<FileChange>> _day_Files = new Dictionary<string, List<FileChange>>();

			Events = new List<List<FileChange>>();

			foreach (FileChange _item in RtData.file_changes)
			{
				DateTime _dt = DateTimeCache[_item.taken_time];

				string _by = string.Empty;

				_by = _dt.ToString("yyyy-MM");

				if (!_day_Files.ContainsKey(_by))
				{
					_day_Files.Add(_by, new List<FileChange>());
				}

				_day_Files[_by].Add(_item);
			}

			_day_Files.Keys.ToList().Sort();

			foreach (string _day in _day_Files.Keys)
			{
				Events.Add(_day_Files[_day]);
			}
		}
	}
}