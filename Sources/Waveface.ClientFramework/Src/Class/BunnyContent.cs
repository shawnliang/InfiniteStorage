using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using Waveface.Model;

namespace Waveface.ClientFramework
{
	public class BunnyContent : Content
	{

		#region Property
		public override string ID
		{
			get
			{
				return GetContentID();
			}
		}

		public override System.Drawing.Image Thumbnail
		{
			get
			{
				return GetContentThumbnail();
			}
		}

		public override bool Liked
		{
			get
			{
				return GetLiked();
			}
		}
		#endregion


		#region Constructor
		public BunnyContent()
		{

		}

		public BunnyContent(Uri uri)
			: base(uri.GetHashCode().ToString(), uri)
		{
		} 
		#endregion


		#region Private Method
		/// <summary>
		/// Gets the content ID.
		/// </summary>
		/// <returns></returns>
		private string GetContentID()
		{
			var savedPath = this.ContentPath;

			var appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny");

			var dbFilePath = Path.Combine(appDir, "database.s3db");

			var conn = new SQLiteConnection(string.Format("Data source={0}", dbFilePath));

			conn.Open();

			var cmd = new SQLiteCommand(string.Format("SELECT file_id FROM Files where saved_path = '{0}'", savedPath), conn);

			var contentID = cmd.ExecuteScalar().ToString();

			conn.Close();

			return contentID;
		}

		/// <summary>
		/// Gets the content thumbnail.
		/// </summary>
		/// <returns></returns>
		private System.Drawing.Image GetContentThumbnail()
		{
			var resourceFolderValue = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("BunnyHome").GetValue("ResourceFolder").ToString();
			var imageFile = string.Format(@"{0}\.thumbs\{1}.small.thumb", resourceFolderValue, this.ID);

			return System.Drawing.Image.FromFile(imageFile);
		}

		private bool GetLiked()
		{
			var savedPath = this.ContentPath;

			var appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny");

			var dbFilePath = Path.Combine(appDir, "database.s3db");

			var conn = new SQLiteConnection(string.Format("Data source={0}", dbFilePath));

			conn.Open();

			var cmd = new SQLiteCommand("SELECT label_id FROM LabelFiles where file_id = @fid", conn);
			cmd.Parameters.Add(new SQLiteParameter("@fid", new Guid(ID)));

			var liked =  cmd.ExecuteScalar() != null;

			conn.Close();

			return liked;
		}
		#endregion
	}
}
