using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace InfiniteStorage.Model
{
	public class Device
	{
		[Key]
		public string device_id { get; set; }

		public string device_name { get; set; }

		public virtual IEnumerable<FileAsset> files { get; set; }
	}

	[Table("Files")]
	public class FileAsset
	{
		[Key]
		public Guid file_id { get; set; }

		public string file_name { get; set; }

		public string file_path { get; set; }

		public long file_size { get; set; }
		
		public string device_id { get; set; }

		public string mime_type { get; set; }

		public string uti { get; set; }

		public DateTime event_time { get; set; }

		public FileAssetType type { get; set; }

		public virtual Device device { get; set; }
	}

	public enum FileAssetType
	{
		image = 0,
		video = 1,
		audio = 2,
	}

	public class InfiniteStorageContext: DbContext
	{
		public DbSet<Device> Devices { get; set; }
		
		public DbSet<FileAsset> Files { get; set; }

		public InfiniteStorageContext(DbConnection conn, bool contextOwnsConnection)
			:base(conn, contextOwnsConnection)
		{
		}

	}
}
