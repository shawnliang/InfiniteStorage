using InfiniteStorage.Model;
using System;
using System.IO;
using System.Linq;

namespace InfiniteStorage.WebsocketProtocol
{
	class TransmitUtility : ITransmitStateUtility, IFileUtility
	{
		public void SaveFileRecord(Model.FileAsset file)
		{
			using (var db = new MyDbContext())
			{
				db.Object.Files.Add(file);
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
								 select new { 
									has_origin = f.has_origin
								 };

				if (file.is_thumbnail)
					return saved_file.Any();
				else
					return saved_file.Any() && saved_file.First().has_origin;
			}
		}

		public long GetNextSeq()
		{
			return SeqNum.GetNextSeq();
		}


		public Guid? QueryFileId(string device_id, string file_path)
		{
			using (var db = new MyDbContext())
			{
				var file = from f in db.Object.Files
							  where f.file_path.Equals(file_path, StringComparison.InvariantCultureIgnoreCase) && f.device_id == device_id
							  select new { file_id = f.file_id };

				return file.Any() ? file.First().file_id : (Guid?)null;
			}
		}
	}
}
