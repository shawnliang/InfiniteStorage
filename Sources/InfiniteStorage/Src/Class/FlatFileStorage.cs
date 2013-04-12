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
		private string photoLocation;
		private string videoLocation;
		private string audioLocation;
		private string deviceName;

		public FlatFileStorage(string photoLocation, string videoLocation, string audioLocation)
		{
			this.photoLocation = photoLocation;
			this.videoLocation = videoLocation;
			this.audioLocation = audioLocation;
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

			var photoDir = Path.Combine(photoLocation, this.deviceName);
			if (!Directory.Exists(photoDir))
				Directory.CreateDirectory(photoDir);

			var videoDir = Path.Combine(videoLocation, this.deviceName);
			if (!Directory.Exists(videoDir))
				Directory.CreateDirectory(videoDir);

			var audioDir = Path.Combine(audioLocation, this.deviceName);
			if (!Directory.Exists(audioDir))
				Directory.CreateDirectory(audioDir);
		}

		public void MoveToStorage(string tempfile, FileContext file)
		{
			if (string.IsNullOrEmpty(deviceName))
				throw new InvalidOperationException("should setDeviceName() first");

			//TODO: seperate photo/video/audio
			var storagePath = Path.Combine(photoLocation, deviceName);

			var saved_file = Path.Combine(storagePath, file.file_name);
			int num = 1;

			while (true)
			{
				try
				{
					File.Move(tempfile, saved_file);
					break;
				}
				catch (IOException e)
				{
					if (File.Exists(saved_file))
					{
						saved_file = Path.Combine(storagePath, Path.GetFileNameWithoutExtension(file.file_name) + "." + num + Path.GetExtension(file.file_name));
						num += 1;
					}
					else
						throw new IOException("Unable to move file to " + saved_file, e);
				}
			}
		}
	}
}
