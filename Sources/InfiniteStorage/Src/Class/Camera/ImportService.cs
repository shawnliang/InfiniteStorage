using InfiniteStorage.Model;
using InfiniteStorage.Notify;
using InfiniteStorage.WebsocketProtocol;
using InfiniteStorage.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InfiniteStorage.Camera
{
	static class ImportingCameraCollection
	{
		static List<string> devices = new List<string>();
		static object cs = new object();

		public static void Add(string device_id)
		{
			lock (cs)
			{
				if (!devices.Contains(device_id))
					devices.Add(device_id);
			}
		}

		public static void Remove(string device_id)
		{
			lock (cs)
			{
				if (devices.Contains(device_id))
					devices.Remove(device_id);
			}
		}

		public static ICollection<string> GetAllCameras()
		{
			lock (cs)
			{
				return devices.ToList();
			}
		}
	}


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
						new Device
						{
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

			return new ImportStorage(deviceId, folder);
		}
	}

	class ImportStorage : readCamera.IStorage
	{
		public string device_id { get; set; }
		public string device_folder { get; set; }

		private int recved_count { get; set; }
		private ProgressTooltip progress;
		private ByMonthFileStorage storage = new ByMonthFileStorage();


		public ImportStorage(string device_id, string device_folder)
		{
			var dir = new DirectoryInfo(MyFileFolder.Temp);
			if (!dir.Exists)
			{
				dir.Create();
				dir.Attributes = FileAttributes.Hidden;
			}

			this.device_folder = device_folder;
			this.device_id = device_id;

			storage.setDeviceName(device_folder);
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

			var full_path = storage.MoveToStorage(temp, new FileContext { file_name = file_name, datetime = time });
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
				seq = SeqNum.GetNextSeq(),
				has_origin = true
			};

			var util = new TransmitUtility();
			util.SaveFileRecord(fileAsset);

			recved_count++;
			SynchronizationContextHelper.SendMainSyncContext(() =>
			{
				progress.UpdateProgress(recved_count, recved_count, 100);
				if (type == readCamera.FileType.Image)
					progress.UpdateImage(full_path);
				else
					progress.UpdateImageToVideoIcon();
			});

			var folder_path = fileAsset.parent_folder;
			UIChangeNotificationController.NotifyFolderUpdate(new Folder { name = Path.GetFileName(folder_path), parent_folder = Path.GetDirectoryName(folder_path), path = folder_path });
		}

		public string TempFolder
		{
			get
			{
				return MyFileFolder.Temp;
			}
		}


		public void Connecting()
		{
			SynchronizationContextHelper.SendMainSyncContext(() =>
			{
				progress = new ProgressTooltip(device_folder, device_id);
			});

			ImportingCameraCollection.Add(device_id);
		}

		public void Connected()
		{
		}

		public void Completed()
		{
			SynchronizationContextHelper.SendMainSyncContext(() =>
			{
				progress.UpdateComplete(recved_count, recved_count);
			});

			ImportingCameraCollection.Remove(device_id);
		}
	}
}
