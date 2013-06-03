using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using Waveface.Model;

namespace Waveface.ClientFramework
{
	public class Client
	{
		#region Static Var
		private static Client _default;
		#endregion


		#region Var
		private string _labelID;
		private ObservableCollection<IContentEntity> _taggedContents;
		private ReadOnlyObservableCollection<IContentEntity> _readonlyTaggedContents;
		private ObservableCollection<IContentEntity> _favorites;
		private ReadOnlyObservableCollection<IContentEntity> _readonlyFavorites;
		#endregion


		#region Public Static Property
		public static Client Default
		{
			get
			{
				return _default ?? (_default = new Client());
			}
		}
		#endregion


		#region Private Property
		private string m_LabelID
		{
			get
			{
				return _labelID ?? (_labelID = GetDefaultLabelID());
			}
		}

		private ObservableCollection<IContentEntity> m_TaggedContents
		{
			get
			{
				if (_taggedContents == null)
				{
					_taggedContents = new ObservableCollection<IContentEntity>(GetTaggedContents());
				}
				return _taggedContents;
			}
		}

		private ObservableCollection<IContentEntity> m_Favorites
		{
			get
			{
				if (_favorites == null)
				{
					_favorites = new ObservableCollection<IContentEntity>(GetFavorites());
				}
				return _favorites;
			}
		}
		#endregion


		#region Public Property
		public IEnumerable<IService> Services
		{
			get
			{
				return BunnyServiceSupplier.Default.Services;
			}
		}

		public ReadOnlyObservableCollection<IContentEntity> Favorites
		{
			get
			{
				return _readonlyFavorites ?? (_readonlyFavorites = new ReadOnlyObservableCollection<IContentEntity>(m_Favorites));
			}
		}

		public ReadOnlyObservableCollection<IContentEntity> TaggedContents
		{
			get
			{
				return _readonlyTaggedContents ?? (_readonlyTaggedContents = new ReadOnlyObservableCollection<IContentEntity>(m_TaggedContents));
			}
		}
		#endregion



		public Client()
		{
			foreach (var service in Services)
			{
				service.ContentPropertyChanged += service_ContentPropertyChanged;
			}
		}



		#region Private Method
		private string GetDefaultLabelID()
		{
			var json = StationAPI.GetAllLables();
			var labelID = JObject.Parse(json)["labels"][0]["label_id"].ToString();
			return labelID;
		}

		private IEnumerable<IContentEntity> GetTaggedContents()
		{
			var appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny");

			var dbFilePath = Path.Combine(appDir, "database.s3db");

			var conn = new SQLiteConnection(string.Format("Data source={0}", dbFilePath));

			conn.Open();

			var cmd = new SQLiteCommand("SELECT * FROM Files t1, LabelFiles t2, Labels t3 where t3.name = 'TAG' and t3.label_id = t2.label_id and t1.file_id = t2.file_id", conn);

			var dr = cmd.ExecuteReader();

			while (dr.Read())
			{
				var deviceID = dr["device_id"].ToString();

				cmd = new SQLiteCommand(string.Format("SELECT folder_name FROM Devices where device_id = '{0}'", deviceID), conn);

				var deviceFolder = cmd.ExecuteScalar().ToString();

				var savedPath = dr["saved_path"].ToString();

				var file = Path.Combine(BunnyDB.ResourceFolder, deviceFolder, savedPath);

				yield return new BunnyContent(new Uri(file), dr["file_id"].ToString());
			}


			conn.Close();
		}


		private IEnumerable<IContentEntity> GetFavorites()
		{
			var appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny");

			var dbFilePath = Path.Combine(appDir, "database.s3db");

			var conn = new SQLiteConnection(string.Format("Data source={0}", dbFilePath));

			conn.Open();

			var cmd = new SQLiteCommand("SELECT * FROM Labels where name != 'TAG'", conn);

			var dr = cmd.ExecuteReader();

			while (dr.Read())
			{
				var labelID = dr["label_id"].ToString();
				var labelName = dr["name"].ToString();

				yield return new ContentGroup(labelID, labelName, new Uri(string.Format("c:\\{0}",labelName)), (group, contents) => 
				{
			
				});
			}


			conn.Close();
		}
		#endregion

		void service_ContentPropertyChanged(object sender, ContentPropertyChangeEventArgs e)
		{
			var content = e.Content as IContent;

			m_TaggedContents.Remove(content);
			if (content.Liked)
			{
				m_TaggedContents.Add(content);
				StationAPI.Tag(content.ID, m_LabelID);
			}
			else
				StationAPI.UnTag(content.ID, m_LabelID);
		}
	}
}
