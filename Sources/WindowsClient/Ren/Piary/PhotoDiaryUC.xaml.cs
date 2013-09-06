#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using InfiniteStorage.Model;
using Microsoft.Win32;
using Waveface.ClientFramework;
using Waveface.Model;
using WpfAnimatedGif;
using Application = System.Windows.Forms.Application;

#endregion

namespace Waveface.Client
{
	public partial class PhotoDiaryUC : UserControl
	{
		private IService m_currentDevice;
		private MainWindow m_mainWindow;

		private DispatcherTimer m_startTimer;
		private DispatcherTimer m_refreshTimer;
		private DispatcherTimer m_sizeChangedDelayTimer;

		private int m_videosCount;
		private int m_photosCount;

		private string m_basePath;
		private string m_thumbsPath;
		private SolidColorBrush m_solidColorBrush;

		private int m_oldEventFilesCount;

		private Dictionary<string, EventEntry> m_eventID_Events;
		private Dictionary<string, List<FileEntry>> m_eventID_FileEntrys;

		private ObservableCollection<P_ItemUC> m_eventUCs;

		private double m_myWidth;
		private double m_myHeight;
		private bool m_inited;

		public PhotoDiaryUC()
		{
			InitializeComponent();

			m_basePath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "ResourceFolder", "");
			m_thumbsPath = Path.Combine(m_basePath, ".thumbs");
			m_solidColorBrush = new SolidColorBrush(Color.FromArgb(255, 120, 0, 34));

			InitTimer();

			setWH(280);
		}

		private void InitTimer()
		{
			m_startTimer = new DispatcherTimer();
			m_startTimer.Tick += StartTimerOnTick;
			m_startTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);

			m_refreshTimer = new DispatcherTimer();
			m_refreshTimer.Tick += RefreshTimerOnTick;
			m_refreshTimer.Interval = new TimeSpan(0, 0, 0, 0, 3000);

			m_sizeChangedDelayTimer = new DispatcherTimer();
			m_sizeChangedDelayTimer.Tick += SizeChangedDelayTimer_Tick;
			m_sizeChangedDelayTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
		}

		private void SizeChangedDelayTimer_Tick(object sender, EventArgs e)
		{
			m_sizeChangedDelayTimer.Stop();

			if (m_inited)
			{
				ShowEvents();
			}
		}

		public void Stop()
		{
			if (m_startTimer != null)
			{
				m_startTimer.Stop();
			}
		}

		public void Load(IService device, MainWindow mainWindow)
		{
			m_inited = false;

			gridWaitingPanel.Visibility = Visibility.Visible;
			BitmapImage _biWaiting = new BitmapImage();
			_biWaiting.BeginInit();
			_biWaiting.UriSource = new Uri("pack://application:,,,/Resource/loading_GIF_120.gif");
			_biWaiting.EndInit();
			ImageBehavior.SetAnimatedSource(imgWaiting, _biWaiting);

			m_mainWindow = mainWindow;
			m_currentDevice = device;

			refreshTitleInfo();

			m_startTimer.Start();
		}

		private void StartTimerOnTick(object sender, EventArgs e)
		{
			m_startTimer.Stop();

			List<EventFile> _eventsFiles = GetEventFilesFromDB();

			if (_eventsFiles.Count == 0)
			{
				tbTitle.Visibility = Visibility.Collapsed;

				m_startTimer.Start();
				return;
			}

			prepareData(_eventsFiles);

			refreshTitleInfo();

			tbTitle.Visibility = Visibility.Visible;

			tbTitle.Text = "焦點動態";

			ShowEvents_Init();

			gridWaitingPanel.Visibility = Visibility.Collapsed;

			m_refreshTimer.Start();

			m_inited = true;
		}

		private void RefreshTimerOnTick(object sender, EventArgs e)
		{
			m_refreshTimer.Stop();

			try
			{
				List<EventFile> _eventFiles = GetEventFilesFromDB();

				if (_eventFiles.Count != m_oldEventFilesCount)
				{
					prepareData(_eventFiles);

					ShowEvents();
				}
			}
			catch
			{
			}

			GC.Collect();

			m_refreshTimer.Start();
		}

		private void refreshTitleInfo()
		{
		}

		#region DB

		private List<FileAsset> GetFilesFromDB()
		{
			using (var _db = new MyDbContext())
			{
				IQueryable<FileAsset> _q = from _f in _db.Object.Files select _f;

				return _q.ToList();
			}
		}

		private List<Event> GetEventsFromDB()
		{
			using (var _db = new MyDbContext())
			{
				IQueryable<Event> _q = from _f in _db.Object.Events select _f;

				return _q.ToList();
			}
		}

		private List<EventFile> GetEventFilesFromDB()
		{
			using (var _db = new MyDbContext())
			{
				IQueryable<EventFile> _q = from _f in _db.Object.EventFiles select _f;

				return _q.ToList();
			}
		}

		#endregion

		public void prepareData(List<EventFile> eventFiles)
		{
			m_oldEventFilesCount = eventFiles.Count;

			List<FileAsset> _filesFromDB = GetFilesFromDB();

			m_eventID_FileEntrys = new Dictionary<string, List<FileEntry>>();

			foreach (EventFile _eventFile in eventFiles)
			{
				try
				{
					FileAsset _fa = _filesFromDB.First(f => f.file_id == _eventFile.file_id);

					if (_fa != null)
					{
						FileEntry _fe = new FileEntry
											{
												id = _fa.file_id.ToString(),
												tiny_path = Path.Combine(m_thumbsPath, _fa.file_id + ".tiny.thumb"),
												s92_path = Path.Combine(m_thumbsPath, _fa.file_id + ".s92.thumb"),
												taken_time = _fa.event_time,
												type = _fa.type,
												has_origin = _fa.has_origin
											};

						if (string.IsNullOrEmpty(_fa.saved_path))
						{
							_fe.saved_path = "";
						}
						else
						{
							_fe.saved_path = Path.Combine(m_basePath, _fa.saved_path);
						}

						string _event_id = _eventFile.event_id.ToString();

						if (!m_eventID_FileEntrys.ContainsKey(_event_id))
						{
							m_eventID_FileEntrys.Add(_event_id, new List<FileEntry>());
						}

						m_eventID_FileEntrys[_event_id].Add(_fe);
					}
				}
				catch
				{
				}
			}
		}

		public Dictionary<string, EventEntry> GetEvents()
		{
			Dictionary<string, EventEntry> _id_event_s = new Dictionary<string, EventEntry>();

			List<Event> _events = GetEventsFromDB();

			foreach (Event _event in _events)
			{
				string _eventID = _event.event_id.ToString();

				EventEntry _eventEntry = new EventEntry
									 {
										 event_id = _eventID,
										 Event = _event,
										 Files = m_eventID_FileEntrys[_eventID]
									 };

				_id_event_s.Add(_eventID, _eventEntry);
			}

			return _id_event_s;
		}

		#region Show

		private void ShowEvents_Init()
		{
			m_photosCount = 0;
			m_videosCount = 0;

			m_eventUCs = new ObservableCollection<P_ItemUC>();
			listBoxEvent.ItemsSource = m_eventUCs;

			m_eventID_Events = GetEvents();

			foreach (KeyValuePair<string, EventEntry> _pair in m_eventID_Events)
			{
				P_ItemUC _ctl = new P_ItemUC
									{
										Item = _pair.Value,
										PhotoDiaryUC = this,
									};

				_ctl.SetUI(m_myWidth, m_myHeight);

				m_photosCount += _ctl.PhotosCount;
				m_videosCount += _ctl.VideosCount;

				m_eventUCs.Add(_ctl);

				Application.DoEvents();
			}
		}

		private void ShowEvents()
		{
			m_photosCount = 0;
			m_videosCount = 0;

			Dictionary<string, EventEntry> _id_event_s = GetEvents();

			foreach (KeyValuePair<string, EventEntry> _pair in _id_event_s)
			{
				P_ItemUC _ctl = null;

				if (m_eventID_Events.Keys.Contains(_pair.Key))
				{
					foreach (P_ItemUC _eventUc in m_eventUCs)
					{
						if (_eventUc.Item.Event.event_id.ToString() == _pair.Key)
						{
							_ctl = _eventUc;
							break;
						}
					}

					if (_ctl == null)
					{
						continue;
					}

					_ctl.Item = _pair.Value;
				}
				else
				{
					_ctl = new P_ItemUC
							   {
								   Item = _pair.Value,
								   PhotoDiaryUC = this,
							   };

					_ctl.Changed = true;

					m_eventUCs.Add(_ctl);
				}

				_ctl.SetUI(m_myWidth, m_myHeight);

				m_photosCount += _ctl.PhotosCount;
				m_videosCount += _ctl.VideosCount;

				Application.DoEvents();
			}

			m_eventID_Events = _id_event_s;
		}

		public static string GetCountsString(int photosCount, int videosCount)
		{
			string _c = string.Empty;

			string _photo = " " + (string)System.Windows.Application.Current.FindResource("photo");
			string _photos = " " + (string)System.Windows.Application.Current.FindResource("photos");
			string _video = " " + (string)System.Windows.Application.Current.FindResource("video");
			string _videos = " " + (string)System.Windows.Application.Current.FindResource("videos");

			if (photosCount > 0)
			{
				_c = photosCount + ((photosCount == 1) ? _photo : _photos);
			}

			if (videosCount > 0)
			{
				if (photosCount > 0)
				{
					_c = _c + ", ";
				}

				_c = _c + videosCount + ((videosCount == 1) ? _video : _videos);
			}

			return _c;
		}

		#endregion

		private void listBoxEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (listBoxEvent.SelectedItem != null)
			{
			}
		}

		public void ToPhotoDiary2ndLevel(List<FileEntry> fileEntrys, string title)
		{
			ObservableCollection<IContentEntity> _contents = new ObservableCollection<IContentEntity>();

			foreach (FileEntry _fe in fileEntrys)
			{
				if (File.Exists(_fe.saved_path))
				{
					Content _ce = new BunnyContent(new Uri(_fe.saved_path), _fe.id, ContentType.Photo);
					_contents.Add(_ce);
				}
			}

			ContentGroup gropup = new ContentGroup(Guid.NewGuid().ToString(), title, new Uri(@"c:\"), _contents);

			m_mainWindow.ToPhotoDiary2ndLevel(gropup);
		}

		private void zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			setWH(e.NewValue);

			if (m_inited)
			{
				m_sizeChangedDelayTimer.Stop();

				m_sizeChangedDelayTimer.Start();
			}
		}

		private void setWH(double value)
		{
			m_myWidth = value;
			m_myHeight = m_myWidth + 64;
		}


		// ----------------

		private void CreateNormalAlbum(IEnumerable<IContentEntity> allEntities, IEnumerable<IContentEntity> selectedEntities, string title)
		{
			m_mainWindow.CreateNormalAlbum(allEntities, selectedEntities, title);
		}

		private void btnPushToDevice_Click(object sender, RoutedEventArgs e)
		{
			m_mainWindow.PushToDevice();
		}

		private void btnShare_Click(object sender, RoutedEventArgs e)
		{
			sharePopup.IsOpen = true;
		}

		private void ShareCallout_SaveToClicked(object sender, EventArgs e)
		{
			m_mainWindow.SaveTo(new List<IContentEntity>());
		}

		private void btnAddToAlbum_Click(object sender, RoutedEventArgs e)
		{
			addToAlbumPopup.IsOpen = true;

			var _albums = new List<IContentEntity>
				              {
					              new CreateNewAlbumContentEntity((string) FindResource("CreateFavorite"))
				              };

			_albums.AddRange(ClientFramework.Client.Default.Favorites.OrderBy(x => x.Name));

			addToAlbumPopup.DataContext = _albums;
		}

		private void ShareCallout_CreateOnlineAlbumClicked(object sender, EventArgs e)
		{
			//Todo:

			m_mainWindow.CreateCloudAlbum(new List<ContentEntity>(), new List<ContentEntity>(), "Hello World OnLine");
		}

		private void btnCreateAlbum_Click(object sender, RoutedEventArgs e)
		{
			//Todo:

			CreateNormalAlbum(new List<ContentEntity>(), new List<ContentEntity>(), "Hello World");
		}


		private void AddToAlbum_AlbumClicked(object sender, AlbumClickedEventArgs e)
		{
			if (e.DataContext is CreateNewAlbumContentEntity)
			{
				//Todo:


				CreateNormalAlbum(new List<ContentEntity>(), new List<ContentEntity>(), "Hello World");
			}
			else
			{
				//ToDo:

				IContentGroup _album = e.DataContext as IContentGroup;

				List<IContentEntity> selectedEntities = new List<IContentEntity>();

				m_mainWindow.AddToFavorite(_album.ID, selectedEntities);
			}

			addToAlbumPopup.IsOpen = false;
		}
	}
}