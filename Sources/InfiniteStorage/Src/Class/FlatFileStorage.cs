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
		private IDirOrganizer dirOrganizer;

		public IFileMove FileMover { get; set; }

		public FlatFileStorage(string photoLocation, string videoLocation, string audioLocation, IDirOrganizer dirOrganizer)
		{
			this.photoLocation = photoLocation;
			this.videoLocation = videoLocation;
			this.audioLocation = audioLocation;
			this.dirOrganizer = dirOrganizer;
			this.FileMover = new FileMover();
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

			string baseDir = photoLocation;

			if (!string.IsNullOrEmpty(file.mimetype))
			{
				if (file.mimetype.StartsWith("image", StringComparison.InvariantCultureIgnoreCase))
				{
					baseDir = photoLocation;
				}
				else if (file.mimetype.StartsWith("video", StringComparison.InvariantCultureIgnoreCase))
				{
					baseDir = videoLocation;
				}
				else if (file.mimetype.StartsWith("audio", StringComparison.InvariantCultureIgnoreCase))
				{
					baseDir = audioLocation;
				}
				else
					log4net.LogManager.GetLogger("FileStorage").WarnFormat(
						"Unable to categorize mime type: {0}. Assume photo: {1}", file.mimetype, file.file_name);
			}

			var baseDir2 = Path.Combine(baseDir, deviceName);
			var baseDir3 = Path.Combine(baseDir2, dirOrganizer.GetDir(file));

			if (!Directory.Exists(baseDir3))
				Directory.CreateDirectory(baseDir3);

			var saved_file = Path.Combine(baseDir3, file.file_name);

			// TODO: handle saved_file_name
			var saved_file_name = FileMover.Move(tempfile, saved_file);
		}
	}
}
