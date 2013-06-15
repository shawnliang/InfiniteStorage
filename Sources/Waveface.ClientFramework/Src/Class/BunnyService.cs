using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using Waveface.Model;
using System.Threading;

namespace Waveface.ClientFramework
{
	class BunnyService : Service
	{
		private BunnyUnsortedContentGroup unsorted;
		private Timer timer;
		private bool timerStarted;

		public BunnyService(IServiceSupplier supplier, string devFolderName, string deviceId)
			: base(deviceId, supplier, devFolderName)
		{
			timer = new Timer(refresh, null, Timeout.Infinite, Timeout.Infinite);
			SetContents(PopulateContent);
		}

		private void PopulateContent(ObservableCollection<IContentEntity> content)
		{
			unsorted = new BunnyUnsortedContentGroup(this.ID);
			content.Add(unsorted);

			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();

				var cmd = conn.CreateCommand();
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

			if (!timerStarted)
			{
				timer.Change(500, Timeout.Infinite);
				timerStarted = true;
			}
		}

		private void refresh(object nil)
		{
			try
			{
				var unsortedGroup = unsorted;

				if (unsorted != null)
				{
					unsorted.Refresh();
				}

			}
			catch
			{
			}
			finally
			{
				timer.Change(500, Timeout.Infinite);
			}
		}
	}
}
