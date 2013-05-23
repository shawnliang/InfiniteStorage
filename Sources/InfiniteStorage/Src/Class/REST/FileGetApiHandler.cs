using InfiniteStorage.Model;
using InfiniteStorage.Notify;
using System;
using System.Collections.Generic;
using System.Linq;
using Wammer.Station;

namespace InfiniteStorage.REST
{
	class FileGetApiHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			CheckParameter("files");

			var fileList = Parameters["files"];
			var file_ids = fileList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(
				x => new Guid(x)).ToList();


			List<FileChangeData> result = new List<FileChangeData>();

			using (var db = new MyDbContext())
			{

				// TODO:
				// Don't know why using linq-to-Entity where clause : where file_ids.Contains(f.file_id)
				// does not work. So here a nested loop is used. Need to figure it out.
				foreach (var fid in file_ids)
				{
					var query = (from f in db.Object.Files
								 join d in db.Object.Devices on f.device_id equals d.device_id
								 where fid == f.file_id
								 select new FileChangeData
								 {
									 id = f.file_id,
									 file_name = f.file_name,
									 thumb_ready = f.thumb_ready,
									 width = f.width,
									 height = f.height,
									 size = f.file_size,
									 type = f.type,
									 dev_id = f.device_id,
									 dev_name = d.device_name,
									 dev_type = 0,//TODO
									 deleted = f.deleted,
									 seq = f.seq,

									 saved_path = f.saved_path,
									 device_folder = d.folder_name
								 }).Union(
								 from f in db.Object.PendingFiles
								 join d in db.Object.Devices on f.device_id equals d.device_id
								 where fid == f.file_id
								 select new FileChangeData
								 {
									 id = f.file_id,
									 file_name = f.file_name,
									 thumb_ready = f.thumb_ready,
									 width = f.width,
									 height = f.height,
									 size = f.file_size,
									 type = f.type,
									 dev_id = f.device_id,
									 dev_name = d.device_name,
									 dev_type = 0,//TODO
									 deleted = f.deleted,
									 seq = f.seq,

									 saved_path = f.saved_path,
									 device_folder = d.folder_name
								 });

					var file = query.FirstOrDefault();

					if (file != null)
						result.Add(file);
				}

			}

			respondSuccess(new { files = result });
		}
	}
}
