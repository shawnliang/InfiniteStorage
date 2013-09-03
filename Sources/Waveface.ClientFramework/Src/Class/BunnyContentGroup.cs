using System;
using System.Data.SQLite;
using System.IO;
using Waveface.Model;

namespace Waveface.ClientFramework
{
	public class BunnyContentGroup : ContentGroup
	{
		public string DeviceId { get; private set; }

		private string parentFolder;

		#region Constructor
		public BunnyContentGroup()
		{

		}

		public BunnyContentGroup(string parentFolder, string name, string deviceId)
			: base(Path.Combine(BunnyDB.ResourceFolder, parentFolder, name).GetHashCode().ToString(), name, new Uri(Path.Combine(BunnyDB.ResourceFolder, parentFolder, name)))
		{
			this.parentFolder = parentFolder;
			this.DeviceId = deviceId;

			SetContents((contents) =>
			{
				using (var conn = BunnyDB.CreateConnection())
				{
					conn.Open();
					AddSubfolders(contents, conn);
					AddFiles(contents, conn);
				}
			});
		}

		#endregion

		#region private methods
		private void AddSubfolders(System.Collections.ObjectModel.ObservableCollection<IContentEntity> contents, SQLiteConnection conn)
		{
			using (var cmd = conn.CreateCommand())
			{
				cmd.CommandText =
				   "select [name] from [Folders] " +
				   "where [parent_folder] = @parent " +
				   "order by [name]";

				cmd.Parameters.Add(new SQLiteParameter("@parent", Path.Combine(this.parentFolder, this.Name)));
				cmd.Parameters.Add(new SQLiteParameter("@dev", DeviceId));

				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						var subfolder = reader.GetString(0);
						contents.Add(new BunnyContentGroup(Path.Combine(parentFolder, Name), subfolder, DeviceId));
					}
				}
			}
		}

		private void AddFiles(System.Collections.ObjectModel.ObservableCollection<IContentEntity> contents, SQLiteConnection conn)
		{
			using (var cmd = conn.CreateCommand())
			{
				cmd.CommandText =
				   "select file_id, file_name, type, event_time from Files " +
				   "where parent_folder = @parent and device_id = @dev and deleted = 0 and has_origin = 1 " +
				   "order by event_time";

				cmd.Parameters.Add(new SQLiteParameter("@parent", Path.Combine(this.parentFolder, this.Name)));
				cmd.Parameters.Add(new SQLiteParameter("@dev", DeviceId));

				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						var file_path = Path.Combine(Uri.LocalPath, reader["file_name"].ToString());

						var type = ((long)reader["type"] == 0L) ? ContentType.Photo : ContentType.Video;

						var event_time = (DateTime)reader["event_time"];

						contents.Add(new BunnyContent(new Uri(file_path), reader["file_id"].ToString(), type, event_time)
						{
							EnableTag = true
						});
					}
				}
			}
		}
		#endregion



		#region Public Method
		public override void Refresh()
		{
			base.Refresh();

			m_ObservableContents.Sort(
				(x, y) =>
				{
					if (x is BunnyContent && y is BunnyContent)
					{
						return (x as BunnyContent).EventTime.CompareTo((y as BunnyContent).EventTime);
					}
					else if (x is BunnyContent && y is BunnyContentGroup)
					{
						return 1; // x > y
					}
					else if (x is BunnyContentGroup && y is BunnyContent)
					{
						return -1;
					}
					else if (x is BunnyContentGroup && y is BunnyContentGroup)
					{
						return (x as BunnyContentGroup).Name.CompareTo((y as BunnyContentGroup).Name);
					}
					else
						throw new InvalidDataException("Not supported combination: " + x.GetType() + " and " + y.GetType());
				});
		}
		#endregion
	}
}
