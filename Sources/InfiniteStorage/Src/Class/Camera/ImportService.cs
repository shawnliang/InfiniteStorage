using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteStorage.Model;
using System.IO;
using InfiniteStorage.WebsocketProtocol;

namespace InfiniteStorage.Camera
{
	class ImportService : SimpleDetector.ImportService
	{
		public bool device_exist(string device_id)
		{
			using (var db = new MyDbContext())
			{
				var query = from dev in db.Object.Devices
							where dev.device_id == device_id
							select dev;

				return query.Any();
			}
		}

		public void create_device(string device_id, string device_name)
		{
			var dev = new Device
			{
				device_id = device_id,
				folder_name = DeviceUtility.GetUniqueDeviceFolder(device_name),
				device_name = device_name
			};

			using (var db = new MyDbContext())
			{
				db.Object.Devices.Add(dev);
				db.Object.SaveChanges();
			}
		}

		public bool is_file_exist(string device_id, string file_path)
		{
			using (var db = new MyDbContext())
			{
				var query = from f in db.Object.Files
							where f.file_path == file_path && f.device_id == device_id
							select 1;

				return query.Any();
			}
		}

		public void copy_file(System.IO.Stream input, string file_path, SimpleDetector.FileType type, DateTime time, string device_id)
		{
			var temp = Path.Combine(MyFileFolder.Temp, Guid.NewGuid().ToString() + Path.GetExtension(file_path));

			using (var fs = File.OpenWrite(temp))
			{
				input.CopyTo(fs);
			}

			var file_size = new FileInfo(temp).Length;
			var file_name = Path.GetFileName(file_path);

			var storage = new PendingFileStorage();
			var saved = storage.MoveToStorage(temp, new FileContext { file_name = file_name });

			var fileAsset = new FileAsset
			{
				device_id = device_id,
				event_time = time,
				file_id = Guid.NewGuid(),
				file_name = file_name,
				file_path = file_path,
				file_size = file_size,
				type = (type == SimpleDetector.FileType.Image)? (int)FileAssetType.image : (int)FileAssetType.video,
				saved_path = saved.relative_file_path,
				parent_folder = Path.GetDirectoryName(saved.relative_file_path),
				seq = SeqNum.GetNextSeq()
			};

			var util = new TransmitUtility();
			util.SaveFileRecord(fileAsset);
		}

	}


}
