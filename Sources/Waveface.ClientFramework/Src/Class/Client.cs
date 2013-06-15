using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Waveface.Model;
using Microsoft.Win32;

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

			var cmd = new SQLiteCommand("SELECT * FROM Files t1, LabelFiles t2, Labels t3 where t3.name = 'STARRED' and t3.label_id = t2.label_id and t1.file_id = t2.file_id", conn);

			var dr = cmd.ExecuteReader();

			while (dr.Read())
			{
				var deviceID = dr["device_id"].ToString();

				var savedPath = dr["saved_path"].ToString();

				var file = Path.Combine(BunnyDB.ResourceFolder, savedPath);

				var type = ((long)dr["type"] == 0) ? ContentType.Photo : ContentType.Video;
				yield return new BunnyContent(new Uri(file), dr["file_id"].ToString(), type);
			}


			conn.Close();
		}


		private IEnumerable<IContentEntity> GetFavorites()
		{
			var appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny");

			var dbFilePath = Path.Combine(appDir, "database.s3db");

			var conn = new SQLiteConnection(string.Format("Data source={0}", dbFilePath));

			conn.Open();

			var cmd = new SQLiteCommand("SELECT * FROM Labels where auto_type == 0 and deleted == 0", conn);

			var dr = cmd.ExecuteReader();

			while (dr.Read())
			{
				var labelID = dr["label_id"].ToString();
				var labelName = dr["name"].ToString();

				yield return new ContentGroup(labelID, labelName, new Uri(string.Format("c:\\{0}",labelName)), (group, contents) => 
				{
					var conn2 = new SQLiteConnection(string.Format("Data source={0}", dbFilePath));

					conn2.Open();

					var cmd2 = new SQLiteCommand("SELECT * FROM Files t1, LabelFiles t2, Labels t3 where t3.label_id = @labelID and t3.label_id = t2.label_id and t1.file_id = t2.file_id", conn2);

					cmd2.Parameters.Add(new SQLiteParameter("@labelID", new Guid(labelID)));

					var dr2 = cmd2.ExecuteReader();

					while (dr2.Read())
					{
						var deviceID = dr2["device_id"].ToString();

						var savedPath = dr2["saved_path"].ToString();

						var file = Path.Combine(BunnyDB.ResourceFolder, savedPath);

						var type = ((long)dr2["type"] == 0) ? ContentType.Photo : ContentType.Video;
						contents.Add(new BunnyContent(new Uri(file), dr2["file_id"].ToString(), type));
					}

					conn2.Close();
				});
			}


			conn.Close();
		}
		#endregion

		public void Tag(IEnumerable<IContent> contents)
		{
			StationAPI.Tag(string.Join(",", contents.Select(taggedContent => taggedContent.ID).ToArray()), m_LabelID);
		}


		public void SaveToFavorite(string favoriteName)
		{
			var labelID = Guid.NewGuid().ToString();
			StationAPI.AddLabel(labelID, favoriteName);

			StationAPI.Tag(string.Join(",", m_TaggedContents.Select(taggedContent => taggedContent.ID).ToArray()), labelID);

			//StationAPI.ClearLabel(m_LabelID);

			//m_TaggedContents.Clear();
			m_Favorites.Clear();
			m_Favorites.AddRange(GetFavorites());
		}

		public void SaveToFavorite()
		{
			SaveToFavorite("Untitled Favorite");
		}


		public void UnTag(string labelID , string contentID)
		{
			StationAPI.UnTag(contentID, labelID);
		}

		public void AddToFavorite(string favoriteID)
		{
			StationAPI.Tag(string.Join(",", m_TaggedContents.Select(taggedContent => taggedContent.ID).ToArray()), favoriteID);

			//StationAPI.ClearLabel(m_LabelID);

			//m_TaggedContents.Clear();
			m_Favorites.Clear();
			m_Favorites.AddRange(GetFavorites());
		}

		public void RemoveFavorite(string favoriteID)
		{
			StationAPI.DeleteLabel(favoriteID);

			m_Favorites.Clear();
			m_Favorites.AddRange(GetFavorites());
		}

		public void ClearTaggedContents()
		{
			StationAPI.ClearLabel(m_LabelID);

			m_TaggedContents.Clear();
		}


		public void OnAir(string labelID, Boolean isOnAir)
		{
			StationAPI.OnAirLabel(labelID, isOnAir);
		}

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

		public bool IsOnAir(IContentGroup group)
		{
			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();
				var cmd = conn.CreateCommand();
				cmd.CommandText = "select on_air from [Labels] where label_id = @label";
				cmd.Parameters.Add(new SQLiteParameter("@label", new Guid(group.ID)));
				var on_air = cmd.ExecuteScalar();

				return on_air != null && (bool)on_air;
			}
		}

		public bool HomeSharingEnabled
		{
			get { return Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "HomeSharing", "true").Equals("true"); }
		}
	}
}
