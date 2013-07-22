#region

using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading;
using Waveface.Model;

#endregion

namespace Waveface.ClientFramework
{
	internal class BunnyService : Service
	{
		private BunnyDeviceTimelineContentGroup timeline;
		private Timer timer;
		private bool timerStarted;
		private bool _isRecving;

		public bool IsRecving
		{
			get { return _isRecving; }

			set
			{
				if (_isRecving != value)
				{
					_isRecving = value;
					OnPropertyChanged("IsRecving");
				}
			}
		}

		public BunnyService(IServiceSupplier supplier, string devFolderName, string deviceId)
			: base(deviceId, supplier, devFolderName)
		{
			timer = new Timer(refresh, null, Timeout.Infinite, Timeout.Infinite);
			Uri = new Uri(Path.Combine(BunnyDB.ResourceFolder, devFolderName));
			SetContents(PopulateContent);
		}

		private void PopulateContent(ObservableCollection<IContentEntity> content)
		{
			timeline = new BunnyDeviceTimelineContentGroup(ID, Path.Combine(BunnyDB.ResourceFolder, Name));
			content.Add(timeline);

			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();

				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select [name] from [Folders] " +
					                  "where parent_folder = @parent " +
					                  "order by name desc";

					cmd.Parameters.Add(new SQLiteParameter("@parent", Name));

					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							var dir = reader.GetString(0);
							content.Add(new BunnyContentGroup(Name, dir, ID) { Service = this });
						}
					}
				}
			}

			if (!timerStarted)
			{
				timer.Change(1000, Timeout.Infinite);
				timerStarted = true;
			}
		}

		public override void Refresh()
		{
			// Truely refresh underlying contents because there is no good way to determine if a 
			// content group really "eqauls" to another content group.
			//
			// (Comparing name and id is insufficient because the contained items can be different.
			// Unless comparing the contained contents, but which indicates recursively enumerating contained contents,
			// no way to truly comparing content groups.)
			m_ObservableContents.Clear();
			PopulateContent(m_ObservableContents);
		}

		private void refresh(object nil)
		{
			try
			{
				if (IsRecving)
				{
					if (timeline != null)
					{
						timeline.Refresh();
					}

					var unsorteds = Contents.Where(x => x.Name == "Unsorted").ToList();

					if (unsorteds.Any())
					{
						foreach (BunnyContentGroup unsorted in unsorteds)
						{
							unsorted.Refresh();
						}
					}
					else
					{
						ObservableCollection<IContentEntity> newContents = new ObservableCollection<IContentEntity>();
						PopulateContent(newContents);

						if (newContents.Where(x => x.Name == "Unsorted").Any())
						{
							base.Refresh();
						}

					}
				}
			}
			catch
			{
			}
			finally
			{
				timer.Change(1000, Timeout.Infinite);
			}
		}

		#region Public Method

		public override bool Equals(object obj)
		{
			//檢查參數是否為null
			if (obj == null)
				return false;

			//檢查是否與自身是相同物件
			if (ReferenceEquals(this, obj))
				return true;

			//檢查是否型態相等
			var value = obj as IService;
			if (value == null)
				return false;

			//比較內容是否相等
			return Name == value.Name;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		#endregion
	}
}