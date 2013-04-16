using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.Entity;


namespace InfiniteStorage.Model
{
	public class Device
	{
		[Key]
		public string device_id { get; set; }

		public string device_name { get; set; }

		public int photo_count { get; set; }

		public int video_count { get; set; }

		public int audio_count { get; set; }
	}


	public class InfiniteStorageContext: DbContext
	{
		public DbSet<Device> Devices { get; set; }

		public InfiniteStorageContext(DbConnection conn, bool contextOwnsConnection)
			:base(conn, contextOwnsConnection)
		{
		}
	}	
}
