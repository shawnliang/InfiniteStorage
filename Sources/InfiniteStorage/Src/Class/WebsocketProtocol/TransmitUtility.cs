using InfiniteStorage.Model;
using System;
using System.IO;
using System.Linq;

namespace InfiniteStorage.WebsocketProtocol
{
	class TransmitUtility : ITransmitStateUtility
	{
		public void SaveFileRecord(Model.FileAsset file)
		{
			var pendingFile = new PendingFile
			{
				file_id = file.file_id,
				file_name = file.file_name,
				file_path = file.file_path,
				file_size = file.file_size,
				deleted = file.deleted,
				device_id = file.device_id,
				event_time = file.event_time,
				saved_path = file.saved_path,
				seq = file.seq,
				thumb_ready = file.thumb_ready,
				type = file.type
			};

			using (var db = new MyDbContext())
			{
				db.Object.PendingFiles.Add(pendingFile);
				db.Object.SaveChanges();
			}
		}

		public bool HasDuplicateFile(FileContext file, string device_id)
		{
			var full_path = Path.Combine(file.folder, file.file_name);

			using (var db = new MyDbContext())
			{
				var saved_file = from f in db.Object.Files
							 where f.file_path.Equals(full_path, StringComparison.InvariantCultureIgnoreCase) && f.device_id == device_id
							 select f;


				var pending_files = from f in db.Object.PendingFiles
									where f.file_path.Equals(full_path, StringComparison.InvariantCultureIgnoreCase) && f.device_id == device_id
									select f;
				
				return saved_file.Any() || pending_files.Any();
			}
		}

		public long GetNextSeq()
		{
			return SeqNum.GetNextSeq();
		}
	}
}
