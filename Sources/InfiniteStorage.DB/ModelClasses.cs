using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;


namespace InfiniteStorage.Model
{
	public class Device
	{
		[Key]
		public string device_id { get; set; }

		public string device_name { get; set; }

		public string folder_name { get; set; }

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

		public DateTime event_time { get; set; }

		// .net 4.0 does not support enum fields for entity model, so use integer instead
		public int type { get; set; }

		public string saved_path { get; set; }

		public string parent_folder { get; set; }

		public long seq { get; set; }

		public bool thumb_ready { get; set; }

		public bool deleted { get; set; }

		public int width { get; set; }

		public int height { get; set; }

		public int? orientation { get; set; }

		public bool? on_cloud { get; set; }
	}


	public enum AutoLabelType
	{
		NotAuto = 0,

		PhotoToday = 1,
		PhotoYesterday = 2,
		PhotoThisWeek = 3,

		VideoToday = 4,
		VideoYesterday = 5,
		VideoThisWeek = 6
	}

	public class Label
	{
		public Label()
		{
			on_air = true;
			auto_type = (int)AutoLabelType.NotAuto;
			share_enabled = false;
		}

		[Key]
		public Guid label_id { get; set; }

		public string name { get; set; }

		public long seq { get; set; }

		public bool deleted { get; set; }

		public int auto_type { get; set; }

		public bool on_air { get; set; }

		public string share_code { get; set; }

		public bool? share_enabled { get; set; }

		public long? share_proc_seq { get; set; }

		public string share_post_id { get; set; }

		public virtual ICollection<ShareRecipient> Recipients { get; set; }
	}

	[Table("LabelFiles")]
	public class LabeledFile
	{
		[Key]
		[Column(Order = 0)]
		public Guid label_id { get; set; }

		[Key]
		[Column(Order = 1)]
		public Guid file_id { get; set; }
	}

	[Table("LabelShareTo")]
	public class ShareRecipient
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long id { get; set; }

		public Guid label_id { get; set; }

		public string email { get; set; }

		public string name { get; set; }

		public bool? on_cloud { get; set; }
	}

	[Table("PendingFiles")]
	public class PendingFile
	{
		[Key]
		public Guid file_id { get; set; }

		public string file_name { get; set; }

		public string file_path { get; set; }

		public long file_size { get; set; }

		public string device_id { get; set; }

		public DateTime event_time { get; set; }

		// .net 4.0 does not support enum fields for entity model, so use integer instead
		public int type { get; set; }

		public string saved_path { get; set; }

		public long seq { get; set; }

		public bool thumb_ready { get; set; }

		public bool deleted { get; set; }

		public int width { get; set; }

		public int height { get; set; }

		public int? orientation { get; set; }
	}

	public class Folder
	{
		[Key]
		/// <summary>
		/// full path from device folder, like "GTI-9300\2013-05\2013-05-03"
		/// </summary>
		public string path { get; set; }

		/// <summary>
		/// parent folder path, like "GTI-9300\2013-05"
		/// </summary>
		public string parent_folder { get; set; }

		/// <summary>
		///  name of this folder, like "2013-05-13"
		/// </summary>
		public string name { get; set; }
	}

	public enum FileAssetType
	{
		image = 0,
		video = 1,
		audio = 2,
	}

	public class InfiniteStorageContext : DbContext
	{
		public DbSet<Device> Devices { get; set; }

		public DbSet<Folder> Folders { get; set; }

		public DbSet<FileAsset> Files { get; set; }

		public DbSet<Label> Labels { get; set; }

		public DbSet<LabeledFile> LabelFiles { get; set; }

		public DbSet<PendingFile> PendingFiles { get; set; }

		public DbSet<ShareRecipient> ShareRecipients { get; set; }

		public InfiniteStorageContext(DbConnection conn, bool contextOwnsConnection)
			: base(conn, contextOwnsConnection)
		{
		}

	}
}
