using InfiniteStorage.Model;
using System;
using System.IO;
using System.Linq;
using System.Net;
using Wammer.Station;

namespace InfiniteStorage.REST
{
	enum IMG_SIZE
	{
		tiny,
		small,
		medium,
		large,
		origin
	}


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

			IMG_SIZE size = IMG_SIZE.large;
			if (!Enum.TryParse<IMG_SIZE>(segments[2], out size))
				size = IMG_SIZE.large;


			FileResult file = null;

			using (var db = new MyDbContext())
			{
				file = (from f in db.Object.Files
						join d in db.Object.Devices on f.device_id equals d.device_id
						where f.file_id == file_id
						select new FileResult
						{
							file_id = f.file_id,
							saved_path = f.saved_path,
							thumb_ready = f.thumb_ready,
							deleted = f.deleted,
							type = f.type
						}).FirstOrDefault();

				if (file == null)

					file = (from f in db.Object.PendingFiles
							join d in db.Object.Devices on f.device_id equals d.device_id
							where f.file_id == file_id
							select new FileResult
							{
								file_id = f.file_id,
								saved_path = @".pending\" + f.saved_path,
								thumb_ready = f.thumb_ready,
								deleted = f.deleted,
								type = f.type
							}).FirstOrDefault();
			}

			if (file == null || file.deleted)
			{
				Response.StatusCode = (int)HttpStatusCode.NotFound;
				Response.Close();
				return;
			}

			Response.StatusCode = (int)HttpStatusCode.Found;

			string file_relative_path;
			if (size == IMG_SIZE.origin)
			{
				file_relative_path = file.saved_path;
			}
			else
			{
				if (file.type == (int)FileAssetType.image)
				{
					file_relative_path = Path.Combine(".thumbs", file.file_id.ToString() + "." + size.ToString() + ".thumb");

					var full_path = Path.Combine(MyFileFolder.Photo, file_relative_path);
					if (!File.Exists(full_path))
						file_relative_path = file.saved_path;
				}
				else
				{
					file_relative_path = Path.Combine(".thumbs", file.file_id.ToString() + ".medium.thumb");

					var full_path = Path.Combine(MyFileFolder.Photo, file_relative_path);
					if (!File.Exists(full_path))
					{
						Response.StatusCode = (int)HttpStatusCode.NotFound;
						Response.Close();
						return;
					}
				}

			}

			var url = new UriBuilder("http", Request.Url.Host, 12888, file_relative_path).ToString();
			Response.AddHeader("Location", url);
			Response.Close();
		}
	}

	internal class FileResult
	{
		public Guid file_id { get; set; }
		public string saved_path { get; set; }
		public bool deleted { get; set; }
		public bool thumb_ready { get; set; }
		public int type { get; set; }
	}
}
