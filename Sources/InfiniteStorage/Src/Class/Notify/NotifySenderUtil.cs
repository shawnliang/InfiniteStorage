using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using InfiniteStorage.Model;

namespace InfiniteStorage.Notify
{
	class NotifySenderUtil : INotifySenderUtil
	{
		public List<FileChangeData> QueryChangedFiles(long from_seq)
		{
			using (var db = new MyDbContext())
			{
				var result = from f in db.Object.Files
							 join d in db.Object.Devices on f.device_id equals d.device_id
							 where f.seq >= from_seq && f.type == (int)FileAssetType.image
							 orderby f.seq ascending
							 select new FileChangeData
							 {
								 id = f.file_id,
								 file_name = f.file_name,
								 thumb_ready = f.thumb_ready,
								 width = 0, //TODO
								 height = 0,//TODO
								 size = f.file_size,
								 type = f.type,
								 dev_id = f.device_id,
								 dev_name = d.device_name,
								 dev_type = 0,//TODO
								 deleted = f.deleted,
								 seq = f.seq,

								 saved_path = f.saved_path,
								 device_folder = d.folder_name
							 };

				return result.ToList();
			}
		}


		public List<Label> QueryAllLabels()
		{
			using (var db = new MyDbContext())
			{
				return (from b in db.Object.Labels
						select b).ToList();
			}
		}

		public List<Guid> QueryLabeledFiles(Guid label_id)
		{
			using (var db = new MyDbContext())
			{
				var result = from lb in db.Object.LabelFiles
							 join f in db.Object.Files on lb.file_id equals f.file_id
							 where lb.label_id == label_id
							 orderby f.event_time ascending
							 select lb.file_id;

				return result.ToList();
			}
		}
	}
}
