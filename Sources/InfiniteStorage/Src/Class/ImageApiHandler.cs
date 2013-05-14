using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;
using InfiniteStorage.Model;
using System.Net;
using System.IO;

namespace InfiniteStorage
{
	class ImageApiHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			var segments = Request.Url.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

			if (!segments[0].Equals("image", StringComparison.InvariantCultureIgnoreCase))
				throw new Exception("url path not starting with /image");

			if (segments.Length != 3)
				throw new Exception("url path format error: " + Request.Url.AbsolutePath);

			var file_id = new Guid(segments[1]);
			var size = segments[2];


			FileResult file = null;

			using (var db = new MyDbContext())
			{
				file = (from f in db.Object.Files
						join d in db.Object.Devices on f.device_id equals d.device_id
						where f.file_id == file_id
						select new FileResult
						{
							saved_path = f.saved_path,
							thumb_ready = f.thumb_ready,
							deleted = f.deleted,
							folder = d.folder_name
						}).FirstOrDefault();
			}

			if (file == null || file.deleted)
			{
				Response.StatusCode = (int)HttpStatusCode.NotFound;
				Response.Close();
				return;
			}

			Response.StatusCode = (int)HttpStatusCode.Moved;

			var url = new UriBuilder("http", Request.Url.Host, 12888, Path.Combine(file.folder, file.saved_path)).ToString();
			Response.AddHeader("Location", url);
			Response.Close();
		}
	}

	internal class FileResult
	{
		public string saved_path { get; set; }
		public string folder { get; set; }
		public bool deleted { get; set; }
		public bool thumb_ready { get; set; }
	}
}
