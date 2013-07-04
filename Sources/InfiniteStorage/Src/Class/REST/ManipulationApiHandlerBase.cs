using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;
using System.Data.SQLite;
using InfiniteStorage.Model;
using System.IO;

namespace InfiniteStorage.REST
{
	abstract class ManipulationApiHandlerBase : HttpHandler
	{
		protected List<Guid> GetIds()
		{
			return parseToList(Parameters["ids"]).Select(x => new Guid(x)).ToList();
		}

		protected List<string> GetPaths()
		{
			return parseToList(Parameters["paths"]).Select(x => PathUtil.MakeRelative(x, MyFileFolder.Photo)).ToList();
		}

		protected string[] parseToList(string para)
		{
			if (string.IsNullOrEmpty(para))
				return new string[] { };

			return para.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		}

		protected List<AbstractFileToManipulate> GetFilesFromFolder(List<string> folders)
		{
			using (var db = new MyDbContext())
			{
				var files = from f in db.Object.Files
							where folders.Contains(f.parent_folder)
							select new FileToManipulate
							{
								file_id = f.file_id,
								saved_path = f.saved_path,
								file_name = f.file_name
							};

				return files.ToList().Cast<AbstractFileToManipulate>().ToList();
			}
		}


		protected List<AbstractFileToManipulate> GetFilesById(List<Guid> file_ids)
		{
			var ret = new List<AbstractFileToManipulate>();

			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select 0 as kind, saved_path, file_name from files where file_id = @file and deleted = 0 " +
									  "union " +
									  "select 1 as kind, saved_path, file_name from pendingfiles where file_id = @file and deleted = 0";
					cmd.Prepare();

					foreach (var file_id in file_ids)
					{
						cmd.Parameters.Clear();
						cmd.Parameters.Add(new SQLiteParameter("@file", file_id));
						using (var reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								var saved_path = reader["saved_path"].ToString();
								var kind = (long)reader["kind"];
								var file_name = reader["file_name"].ToString();

								if (kind == 0)
									ret.Add(new FileToManipulate { file_id = file_id, saved_path = saved_path, file_name = file_name });
								else
									ret.Add(new PendingFileToManipulate { file_id = file_id, saved_path = saved_path, file_name = file_name });
							}
						}
					}
				}
			}

			return ret;
		}
	}


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
