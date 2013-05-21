using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InfiniteStorage.Model;
using InfiniteStorage.WebsocketProtocol;
using InfiniteStorage.Notify;
using System.IO;
using System.Data.SQLite;
using System.Diagnostics;
using System.Reflection;

namespace InfiniteStorage
{
	public partial class FakeSplitEventForm : Form
	{
		public long seq { get; set; }

		public FakeSplitEventForm()
		{
			InitializeComponent();
		}

		private void later_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void submit_Click(object sender, EventArgs e)
		{
			List<Guid> processed = new List<Guid>();

			using (var db = new MyDbContext())
			{
				var files = from f in db.Object.PendingFiles
							join d in db.Object.Devices on f.device_id equals d.device_id
							where f.seq <= seq && !f.deleted
							select new
							{
								file = f,
								device = d
							};

				foreach (var f in files)
				{
					var storage = new FlatFileStorage(new DirOrganizerProxy());
					storage.setDeviceName(f.device.folder_name);

					var ctx = new FileContext {
						datetime = f.file.event_time,
						file_name = f.file.file_name,
						file_size = f.file.file_size,
						folder = Path.GetDirectoryName(f.file.file_path),
						type = (FileAssetType)f.file.type
					};

					var file_path = Path.Combine(MyFileFolder.Photo, ".pending", f.file.saved_path);

					var saved = storage.MoveToStorage(file_path, ctx);

					db.Object.Files.Add(new FileAsset
					{
						deleted = f.file.deleted,
						device_id = f.file.device_id,
						event_time = f.file.event_time,
						file_id = f.file.file_id,
						file_name = f.file.file_name,
						file_path = f.file.file_path,
						file_size = f.file.file_size,
						height = f.file.height,
						parent_folder = Path.GetDirectoryName(saved.relative_file_path),
						saved_path = saved.relative_file_path,
						seq = SeqNum.GetNextSeq(),
						thumb_ready = f.file.thumb_ready,
						type = f.file.type,
						width = f.file.width
					});


					processed.Add(f.file.file_id);

					File.Delete(file_path);
				}

				db.Object.SaveChanges();
			}



			using (var db = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				db.Open();

				var cmd = db.CreateCommand();
				cmd.CommandText = "delete from [PendingFiles] where file_id = @fid";
				cmd.CommandType = CommandType.Text;

				var par = new SQLiteParameter("@fid");
				cmd.Parameters.Add(par);

				foreach (var fid in processed)
				{
					par.Value = fid;
					cmd.ExecuteNonQuery();
				}
			}


			var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			Process.Start(Path.Combine(dir, "Waveface.Client.exe"));

			Close();
		}
	}
}
