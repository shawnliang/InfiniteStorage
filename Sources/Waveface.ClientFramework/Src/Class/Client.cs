#region

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.Linq;
using Waveface.Model;

#endregion

namespace Waveface.ClientFramework
{
	public class Client
	{
		#region Static Var

		private static Client _default;

		#endregion

		#region Var

		private static string _labelID = Guid.Empty.ToString();

		private ObservableCollection<IContentEntity> _favorites;
		private ReadOnlyObservableCollection<IContentEntity> _readonlyFavorites;

		private ObservableCollection<IContentEntity> _recent;
		private ReadOnlyObservableCollection<IContentEntity> _readonlyRecent;

		#endregion

		#region Public Static Property

		public static Client Default
		{
			get { return _default ?? (_default = new Client()); }
		}

		#endregion

		#region Private Property

		public static string StarredLabelId
		{
			get { return _labelID; }
		}

		private ObservableCollection<IContentEntity> m_Favorites
		{
			get
			{
				if (_favorites == null)
				{
					_favorites = new ObservableCollection<IContentEntity>(GetFavorites(false));
				}

				return _favorites;
			}
		}

		private ObservableCollection<IContentEntity> m_Recent
		{
			get
			{
				if (_recent == null)
				{
					_recent = new ObservableCollection<IContentEntity>(GetRecent());
				}

				return _recent;
			}
		}

		#endregion

		#region Public Property

		public IEnumerable<IService> Services
		{
			get { return BunnyServiceSupplier.Default.Services; }
		}

		public ReadOnlyObservableCollection<IContentEntity> Favorites
		{
			get { return _readonlyFavorites ?? (_readonlyFavorites = new ReadOnlyObservableCollection<IContentEntity>(m_Favorites)); }
		}

		public ReadOnlyObservableCollection<IContentEntity> Recent
		{
			get { return _readonlyRecent ?? (_readonlyRecent = new ReadOnlyObservableCollection<IContentEntity>(m_Recent)); }
		}

		#endregion

		public Client()
		{
			foreach (var service in BunnyServiceSupplier.Default.Services)
			{
				service.ContentPropertyChanged += service_ContentPropertyChanged;
			}

			BunnyServiceSupplier.Default.Services.CollectionChanged += Services_CollectionChanged;
		}

		private void Services_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			foreach (var service in e.NewItems.OfType<IService>())
			{
				service.ContentPropertyChanged += service_ContentPropertyChanged;
			}
		}

		#region Private Method

		public IEnumerable<IContentEntity> GetFavorites(bool all)
		{
			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();

				using (var cmd = conn.CreateCommand())
				{
					if (all)
					{
						cmd.CommandText = "SELECT * FROM Labels where auto_type == 0";
					}
					else
					{
						cmd.CommandText = "SELECT * FROM Labels where auto_type == 0 and deleted == 0";
					}


					using (var dr = cmd.ExecuteReader())
					{
						while (dr.Read())
						{
							var labelID = dr["label_id"].ToString();
							var labelName = dr["name"].ToString();
							var share_enabled = (bool)dr["share_enabled"];
							var share_code = dr["share_code"].ToString();

							if (labelID != "00000000-0000-0000-0000-000000000000")
							{
								yield return new BunnyLabelContentGroup(labelID, labelName, share_enabled, share_code);
							}
						}
					}
				}
			}
		}

		public static IEnumerable<IContentEntity> GetRecent()
		{
			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();

				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "SELECT * FROM Labels where auto_type <> 0 or label_id = @starId";
					cmd.Parameters.Add(new SQLiteParameter("@starId", Guid.Empty));

					using (var dr = cmd.ExecuteReader())
					{
						while (dr.Read())
						{
							var labelID = dr["label_id"].ToString();
							var labelName = dr["name"].ToString();
							var share_enabled = (bool)dr["share_enabled"];
							var share_code = dr["share_code"].ToString();

							yield return new BunnyLabelContentGroup(labelID, labelName, share_enabled, share_code);
						}
					}
				}
			}
		}

		#endregion

		public void RefreshRecent()
		{
			foreach (IContentEntity _contentEntity in m_Recent)
			{
				_contentEntity.Refresh();
			}
		}

		//TODO: tag & untag 接口一致...

		public void Tag(IEnumerable<IContent> contents, string starredLabelId)
		{
			StationAPI.Tag(string.Join(",", contents.Select(content => content.ID).ToArray()), starredLabelId);

			(m_Recent.First() as IContentGroup).Refresh();
		}

		public void Tag(IEnumerable<IContent> contents)
		{
			Tag(contents, StarredLabelId);
		}

		public void SaveToFavorite(IEnumerable<IContentEntity> contents, string favoriteName)
		{
			var labelID = Guid.NewGuid().ToString();
			StationAPI.AddLabel(labelID, favoriteName);

			StationAPI.Tag(string.Join(",", contents.Select(content => content.ID).ToArray()), labelID);

			m_Favorites.Clear();
			m_Favorites.AddRange(GetFavorites(false));
		}

		public void Delete(IEnumerable<string> ids = null, IEnumerable<string> paths = null)
		{
			StationAPI.Delete(ids, paths);
		}

		public void Move(IEnumerable<string> ids, string targetPath)
		{
			StationAPI.Move(ids, targetPath);
		}

		public void UnTag(string contentID)
		{
			UnTag(StarredLabelId, contentID);
		}

		public void UnTag(string labelID, string contentID)
		{
			StationAPI.UnTag(contentID, labelID);

			(m_Recent.First() as IContentGroup).Refresh();
		}

		public void AddToFavorite(IEnumerable<IContentEntity> contents, string favoriteID)
		{
			StationAPI.Tag(string.Join(",", contents.Select(content => content.ID).ToArray()), favoriteID);

			m_Favorites.Clear();
			m_Favorites.AddRange(GetFavorites(false));
		}

		public void RemoveFavorite(string favoriteID)
		{
			StationAPI.DeleteLabel(favoriteID);

			m_Favorites.Clear();
			m_Favorites.AddRange(GetFavorites(false));
		}

		public void ClearTaggedContents()
		{
			StationAPI.ClearLabel(StarredLabelId);
		}

		public void OnAir(string labelID, Boolean isOnAir)
		{
			StationAPI.OnAirLabel(labelID, isOnAir);
		}

		public void ShareLabel(string labelID, Boolean isShared)
		{
			StationAPI.ShareLabel(labelID, isShared);
		}

		private void service_ContentPropertyChanged(object sender, ContentPropertyChangeEventArgs e)
		{
			var content = e.Content as IContent;

			if (content.Liked)
			{
				Tag(new[] { content }, StarredLabelId);
			}
			else
			{
				UnTag(StarredLabelId, content.ID);
			}
		}

		public bool IsOnAir(IContentGroup group)
		{
			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();

				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select on_air from [Labels] where label_id = @label";
					cmd.Parameters.Add(new SQLiteParameter("@label", new Guid(group.ID)));
					var on_air = cmd.ExecuteScalar();

					return on_air != null && (bool)on_air;
				}
			}
		}

		public string GetLastImportDevice()
		{
			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select device_id from files where import_time in (select max(import_time) from files)";
					var device_id = cmd.ExecuteScalar();
					return (device_id != null) ? device_id.ToString() : null;
				}
			}
		}

		public bool HomeSharingEnabled
		{
			get { return Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "HomeSharing", "true").ToString().Equals("true", StringComparison.CurrentCultureIgnoreCase); }
			set { Registry.SetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "HomeSharing", value.ToString()); }
		}
	}
}