using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace InfiniteStorage.Manipulation
{
	internal class FileToManipulate : AbstractFileToManipulate
	{
		public override string saved_full_path
		{
			get { return Path.Combine(MyFileFolder.Photo, saved_path); }
		}

		public override bool IsPendingFile
		{
			get { return false; }
		}
	}

	internal class PendingFileToManipulate : AbstractFileToManipulate
	{
		public override string saved_full_path
		{
			get { return Path.Combine(MyFileFolder.Pending, saved_path); }
		}

		public override bool IsPendingFile
		{
			get { return true; }
		}
	}

	internal abstract class AbstractFileToManipulate
	{
		public Guid file_id { get; set; }
		public string saved_path { get; set; }
		public string file_name { get; set; }
		public string temp_file_path { get; set; }

		public abstract string saved_full_path { get; }
		public abstract bool IsPendingFile { get; }

		protected AbstractFileToManipulate()
		{

		}

		public string small_thumb_path
		{
			get { return Path.Combine(MyFileFolder.Thumbs, file_id + ".small.thumb"); }
		}

		public string medium_thumb_path
		{
			get { return Path.Combine(MyFileFolder.Thumbs, file_id + ".small.thumb"); }
		}

		public string large_thumb_path
		{
			get { return Path.Combine(MyFileFolder.Thumbs, file_id + ".small.thumb"); }
		}

		public string tiny_thumb_path
		{
			get { return Path.Combine(MyFileFolder.Thumbs, file_id + ".small.thumb"); }
		}

		public void DeleteThumbnails()
		{
			var small = new FileInfo(small_thumb_path);
			delete(small);

			var medium = new FileInfo(medium_thumb_path);
			delete(medium);

			var large = new FileInfo(large_thumb_path);
			delete(large);

			var tiny = new FileInfo(tiny_thumb_path);
			delete(tiny);
		}

		public void DeleteRecycleBinFile()
		{
			if (!string.IsNullOrEmpty(temp_file_path))
				delete(new FileInfo(temp_file_path));
		}

		private void delete(FileInfo file)
		{
			try
			{
				if (file.Exists)
					file.Delete();
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Unable to delete file: " + file.FullName, err);
			}
		}

		public void Move(string dest)
		{
			File.Move(saved_full_path, dest);
		}
	}
}
