using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using InfiniteStorage.WebsocketProtocol;

namespace InfiniteStorage
{
	public class FlatFileStorage: IFileStorage
	{
		private string deviceName;
		private IDirOrganizer dirOrganizer;

		public IFileMove FileMover { get; set; }
		public IFileStorageLocationProvider StorageLocationProvider { get; set; }

		public FlatFileStorage(IDirOrganizer dirOrganizer)
		{
			this.dirOrganizer = dirOrganizer;
			this.FileMover = new FileMover();
			this.StorageLocationProvider = new FileStorageLocationProvider();
		}

		public void setDeviceName(string deviceName)
		{
			if (string.IsNullOrEmpty(deviceName))
				throw new ArgumentException("deviceName is null or empty");

			this.deviceName = deviceName;

			var invalidChars = Path.GetInvalidFileNameChars();
			foreach (var inv in invalidChars)
			{
				this.deviceName = this.deviceName.Replace(inv, '-');
			}

			var photoDir = Path.Combine(StorageLocationProvider.PhotoLocation, this.deviceName);
			if (!Directory.Exists(photoDir))
				Directory.CreateDirectory(photoDir);

			var videoDir = Path.Combine(StorageLocationProvider.VideoLocation, this.deviceName);
			if (!Directory.Exists(videoDir))
				Directory.CreateDirectory(videoDir);

			var audioDir = Path.Combine(StorageLocationProvider.AudioLocation, this.deviceName);
			if (!Directory.Exists(audioDir))
				Directory.CreateDirectory(audioDir);
		}

		public SavedPath MoveToStorage(string tempfile, FileContext file)
		{
			if (string.IsNullOrEmpty(deviceName))
				throw new InvalidOperationException("should setDeviceName() first");

			string baseDir = StorageLocationProvider.PhotoLocation;

			switch (file.type)
			{
				case Model.FileAssetType.image:
					baseDir = StorageLocationProvider.PhotoLocation;
					break;
				case Model.FileAssetType.video:
					baseDir = StorageLocationProvider.VideoLocation;
					break;
				case Model.FileAssetType.audio:
					baseDir = StorageLocationProvider.AudioLocation;
					break;
			}

			var devDir = Path.Combine(baseDir, deviceName);
			var relativeDir = dirOrganizer.GetDir(file);
			var fullTargetDir = Path.Combine(devDir, relativeDir);

			if (!Directory.Exists(fullTargetDir))
				Directory.CreateDirectory(fullTargetDir);

			var saved_file = Path.Combine(fullTargetDir, file.file_name);
			var saved_path = FileMover.Move(tempfile, saved_file);

			return new SavedPath { device_folder = devDir, relative_file_path = Path.Combine(relativeDir, Path.GetFileName(saved_path)) };
		}
	}
}
