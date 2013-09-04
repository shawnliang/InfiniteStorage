using System;

namespace InfiniteStorage.DB
{
	class FileAndDevFolder
	{
		public Guid file_id { get; set; }
		public string dev_folder { get; set; }

		public FileAndDevFolder()
		{

		}

		public FileAndDevFolder(Guid file_id, string dev_folder)
		{
			this.file_id = file_id;
			this.dev_folder = dev_folder;
		}
	}
}
