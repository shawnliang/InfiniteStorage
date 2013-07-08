using System;
using System.Collections.Generic;
using System.IO;

namespace InfiniteStorage
{
	public interface IPendingToResourceUtil
	{
		string CreateFolder(string folderUnderDevFolder);
		List<PendingFileData> GetPendingFiles(List<Guid> file_ids);
		string Move(string from, string to);
		void MoveDbRecord(List<FileData> data);
		string GetResourceFolder();
		string GetPendingFolder();

		string DevFolder { get; }

		void CreateFolderRecord(string dev_folder, string sub_folder);
	}

	public interface IEventDirOrganizer
	{
		string GetEventFolder(PendingEvent evt);
	}

	public class PendingToResource
	{
		private IPendingToResourceUtil util;

		public PendingToResource(IPendingToResourceUtil util)
		{
			this.util = util;
		}

		public void Do(PendingEvent evt)
		{

			IEventDirOrganizer organizer = null;
			if (evt.type == (int)EventType.Monthly)
				organizer = new MonthlyDirOrganizer();
			else
				organizer = new EventDirOrganizer();

			var folder = util.CreateFolder(organizer.GetEventFolder(evt));

			var pending_files = util.GetPendingFiles(evt.files);
			if (pending_files == null)
				return;

			var resDir = util.GetResourceFolder();
			var pendingDir = util.GetPendingFolder();
			var fileData = new List<FileData>();

			foreach (var pending_file in pending_files)
			{
				var full_path = util.Move(
					Path.Combine(pendingDir, pending_file.saved_path),
					Path.Combine(resDir, pending_file.dev_folder, folder, pending_file.file_name));

				var relative_path = PathUtil.MakeRelative(full_path, resDir);

				fileData.Add(
					new FileData
					{
						file_id = pending_file.file_id,
						parent_folder = Path.GetDirectoryName(relative_path),
						saved_path = relative_path,
					});
			}

			util.CreateFolderRecord(util.DevFolder, folder);
			util.MoveDbRecord(fileData);
		}
	}

	public class PendingFileData
	{
		public Guid file_id { get; set; }
		public string file_name { get; set; }
		public string saved_path { get; set; }
		public string dev_folder { get; set; }
	}

	public class FileData
	{
		public Guid file_id { get; set; }
		public string parent_folder { get; set; }
		public string saved_path { get; set; }
	}
}
