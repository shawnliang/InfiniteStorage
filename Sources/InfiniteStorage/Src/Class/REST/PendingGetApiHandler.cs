using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;
using InfiniteStorage.Model;
using System.IO;

namespace InfiniteStorage.REST
{
	class PendingGetApiHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			CheckParameter("device_id");

			var dev_id = Parameters["device_id"];
			var seq = Parameters["seq"] == null ? 0L : Int64.Parse(Parameters["seq"]);
			var limit = Parameters["limit"] == null ? 500 : Int32.Parse(Parameters["limit"]);


			List<PendingFile> pending_files;
			long total;
			Device dev;

			using (var db = new MyDbContext())
			{
				dev = (from d in db.Object.Devices
						   where d.device_id.Equals(dev_id, StringComparison.InvariantCultureIgnoreCase)
						   select d).FirstOrDefault();


				if (dev == null)
				{
					respondSuccess(new { remaining_count = 0, file_changes = new List<object>() });
					return;
				}


				var query = from f in db.Object.PendingFiles
							where f.seq >= seq && (f.type == (int)FileAssetType.image || f.type == (int)FileAssetType.video) && f.device_id.Equals(dev_id, StringComparison.InvariantCultureIgnoreCase)
							orderby f.seq ascending
							select f;

				pending_files = query.Take(limit).ToList();
				total = query.Count();
			}

			var nRemain = total - pending_files.Count;
			if (nRemain < 0)
				nRemain = 0;

			respondSuccess(new
			{
				remaining_count = nRemain,
				file_changes = pending_files.Select(x => new
				{
					id = x.file_id,
					file_name = x.file_name,
					tiny_path = Path.Combine(MyFileFolder.Photo, ".thumbs", x.file_id + ".tiny.thumb"),
					taken_time = x.event_time.ToString("yyyy-MM-dd HH:mm:ss"),
					width = x.width,
					height = x.height,
					size = x.file_size,
					type = x.type,

					dev_id = dev_id,
					dev_name = dev.device_name,
					dev_type = 0,

					seq = x.seq
				})
			});
		}
	}
}
