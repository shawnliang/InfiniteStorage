using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using Waveface.Model;
using System.Threading;
using System.Linq;

namespace Waveface.ClientFramework
{
	class BunnyService : Service
	{
		private BunnyDeviceTimelineContentGroup timeline;
		private Timer timer;
		private bool timerStarted;
		private bool _isRecving;


		public bool IsRecving
		{
			get
			{
				return _isRecving;
			}

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
			Uri = new System.Uri(Path.Combine(BunnyDB.ResourceFolder, devFolderName));
			SetContents(PopulateContent);
		}

		private void PopulateContent(ObservableCollection<IContentEntity> content)
		{
			timeline = new BunnyDeviceTimelineContentGroup(this.ID, Path.Combine(BunnyDB.ResourceFolder, Name));
			content.Add(timeline);

			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();

				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select [name] from [Folders] " +
									 "where parent_folder = @parent " +
									 "order by name desc";


					cmd.Parameters.Add(new SQLiteParameter("@parent", this.Name));

					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							var dir = reader.GetString(0);
							content.Add(new BunnyContentGroup(Name, dir, this.ID));
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
					
					var unsorteds = this.Contents.Where(x => x.Name == "Unsorted").ToList();
					foreach (BunnyContentGroup unsorted in unsorteds)
					{
						unsorted.Refresh();
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
			if (object.ReferenceEquals(this, obj))
				return true;

			//檢查是否型態相等
			var value = obj as IService;
			if (value == null)
				return false;

			//比較內容是否相等
			return this.Name == value.Name;
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}
		#endregion
	}
}
