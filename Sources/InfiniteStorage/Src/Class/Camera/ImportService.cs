using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteStorage.Model;
using System.IO;
using InfiniteStorage.WebsocketProtocol;
using InfiniteStorage.Win32;

namespace InfiniteStorage.Camera
{
	class ImportService : readCamera.ImportService
	{

		public readCamera.IStorage GetStorage(string deviceId, string deviceName)
		{
			var folder = "";

			using (var db = new MyDbContext())
			{
				var query = from dev in db.Object.Devices
							where dev.device_id == deviceId
							select dev;

				if (!query.Any())
				{
					folder = DeviceUtility.GetUniqueDeviceFolder(deviceName);
					db.Object.Devices.Add(
						new Device {
							device_id = deviceId,
							device_name = deviceName,
							folder_name = folder
						});
					db.Object.SaveChanges();
				}
				else
				{
					folder = query.First().folder_name;
				}
			}

			return new ImportStorage { device_id = deviceId, device_folder = folder };
		}
	}

	class ImportStorage : readCamera.IStorage
	{
		public string device_id { get; set; }
		public string device_folder { get; set; }


		ProgressTooltip progressDialog;


		public ImportStorage()
		{
			var dir = new DirectoryInfo(MyFileFolder.Temp);
			if (!dir.Exists)
			{
				dir.Create();
				dir.Attributes |= FileAttributes.Hidden;
			}
		}

		public bool IsFileExist(string path)
		{
			using (var db = new MyDbContext())
			{
				var query = from f in db.Object.Files
							where f.file_path == path && f.device_id == device_id
							select 1;

				return query.Any();
			}
		}

		public void AddToStorage(string temp, readCamera.FileType type, DateTime time, string file_path)
		{
			var file_size = new FileInfo(temp).Length;
			var file_name = Path.GetFileName(file_path);

			var storage = new DefaultFolderFileStorage();
			storage.setDeviceName(device_folder);


			var full_path = storage.MoveToStorage(temp, new FileContext { file_name = file_name });
			var partial_path = PathUtil.MakeRelative(full_path, MyFileFolder.Photo);


			if (time.Kind == DateTimeKind.Unspecified)
				time = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond, DateTimeKind.Local).ToUniversalTime();
			else if (time.Kind == DateTimeKind.Local)
				time = time.ToUniversalTime();

			var fileAsset = new FileAsset
			{
				device_id = device_id,
				event_time = time,
				file_id = Guid.NewGuid(),
				file_name = file_name,
				file_path = file_path,
				file_size = file_size,
				type = (type == readCamera.FileType.Image) ? (int)FileAssetType.image : (int)FileAssetType.video,
				saved_path = partial_path,
				parent_folder = Path.GetDirectoryName(partial_path),
				seq = SeqNum.GetNextSeq()
			};

			var util = new TransmitUtility();
			util.SaveFileRecord(fileAsset);


			ProgressTooltip.Instance.ShowFile(fileAsset.file_id, fileAsset.file_name, (FileAssetType)fileAsset.type, device_folder, device_id);
		}

		public string TempFolder
		{
			get {
				return MyFileFolder.Temp;
			}
		}


		public void Connecting()
		{
			SynchronizationContextHelper.SendMainSyncContext(() => {
				ProgressTooltip.Instance.ShowWaitingDevice(device_folder);
			});
		}

		public void Connected()
		{
		}

		public void Completed()
		{
			ProgressTooltip.Instance.ShowCompleted(device_folder);
		}
	}
}
