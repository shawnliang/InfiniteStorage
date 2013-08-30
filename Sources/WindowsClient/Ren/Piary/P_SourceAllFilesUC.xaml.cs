#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using InfiniteStorage.Model;
using Microsoft.Win32;
using Waveface.Model;
using WpfAnimatedGif;

#endregion

namespace Waveface.Client
{
	public partial class P_SourceAllFilesUC : UserControl
	{
		private IService m_currentDevice;
		private MainWindow m_mainWindow;

		private DispatcherTimer m_startTimer;
		private DispatcherTimer m_refreshTimer;

		private int m_videosCount;
		private int m_photosCount;
		private int m_hasOriginCount;

		private string m_basePath;
		private string m_thumbsPath;
		private SolidColorBrush m_solidColorBrush;
		private List<FileEntry> m_fileEntries { get; set; }

		private Dictionary<string, List<FileEntry>> m_YMD_Files;
		private List<List<FileEntry>> m_days;
		private ObservableCollection<P_ItemUC> m_eventUCs;

		public P_SourceAllFilesUC()
		{
			InitializeComponent();

			m_basePath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "ResourceFolder", "");
			m_thumbsPath = Path.Combine(m_basePath, ".thumbs");
			m_solidColorBrush = new SolidColorBrush(Color.FromArgb(255, 120, 0, 34));

			InitTimer();
		}

		private void InitTimer()
		{
			m_startTimer = new DispatcherTimer();
			m_startTimer.Tick += StartTimerOnTick;
			m_startTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);

			m_refreshTimer = new DispatcherTimer();
			m_refreshTimer.Tick += RefreshTimerOnTick;
			m_refreshTimer.Interval = new TimeSpan(0, 0, 0, 0, 3000);
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

			List<FileAsset> _files = GetFilesFromDB();

			if (_files.Count == 0)
			{
				tbTitle.Visibility = Visibility.Collapsed;

				m_startTimer.Start();
				return;
			}

			prepareData(_files);

			refreshTitleInfo();

			tbTitle.Visibility = Visibility.Visible;

			tbTitle.Text = m_currentDevice.Name;

			ShowEvents_Init();

			gridWaitingPanel.Visibility = Visibility.Collapsed;

			m_refreshTimer.Start();
		}

		private void RefreshTimerOnTick(object sender, EventArgs e)
		{
			m_refreshTimer.Stop();

			try
			{
				List<FileAsset> _files = GetFilesFromDB();
				int _hasOriginCount = GetHasOriginCount(_files);

				if ((_hasOriginCount == m_hasOriginCount) && (_files.Count == m_fileEntries.Count))
				{
					// 同步完成?!
				}
				else
				{
					prepareData(_files);

					ShowEvents();
				}

				refreshTitleInfo();
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

		private List<FileAsset> GetFilesFromDB()
		{
			using (var _db = new MyDbContext())
			{
				IQueryable<FileAsset> _q = from _f in _db.Object.Files
										   where _f.device_id == m_currentDevice.ID && !_f.deleted
										   select _f;

				return _q.ToList();
			}
		}

		public void prepareData(List<FileAsset> files)
		{
			m_fileEntries = new List<FileEntry>();

			List<FileEntry> _fCs = files.Select(x => new FileEntry
														 {
															 id = x.file_id.ToString(),
															 tiny_path = Path.Combine(m_thumbsPath, x.file_id + ".tiny.thumb"),
															 s92_path = Path.Combine(m_thumbsPath, x.file_id + ".s92.thumb"),
															 taken_time = x.event_time,
															 type = x.type,
															 has_origin = x.has_origin
														 }).ToList();

			m_fileEntries = _fCs.OrderBy(o => o.taken_time).ToList();

			m_hasOriginCount = GetHasOriginCount(files);
		}

		private int GetHasOriginCount(List<FileAsset> files)
		{
			int _hasOriginCount = 0;

			foreach (var _entry in files)
			{
				if (_entry.has_origin)
				{
					_hasOriginCount++;
				}
			}

			return _hasOriginCount;
		}

		public Dictionary<string, List<FileEntry>> GroupingByDay()
		{
			Dictionary<string, List<FileEntry>> _YMD_Files = new Dictionary<string, List<FileEntry>>();

			m_days = new List<List<FileEntry>>();

			foreach (FileEntry _item in m_fileEntries)
			{
				DateTime _dt = _item.taken_time;

				string _by = _dt.ToString("yyyy-MM-dd");

				if (!_YMD_Files.ContainsKey(_by))
				{
					_YMD_Files.Add(_by, new List<FileEntry>());
				}

				_YMD_Files[_by].Add(_item);
			}

			_YMD_Files.Keys.ToList().Sort();

			foreach (string _day in _YMD_Files.Keys)
			{
				m_days.Add(_YMD_Files[_day]);
			}

			m_days.Reverse();

			return _YMD_Files;
		}

		#region Show

		private void ShowEvents_Init()
		{
			m_photosCount = 0;
			m_videosCount = 0;

			m_eventUCs = new ObservableCollection<P_ItemUC>();

			listBoxEvent.ItemsSource = m_eventUCs;

			m_YMD_Files = GroupingByDay();

			foreach (List<FileEntry> _entries in m_days)
			{
				P_ItemUC _ctl = new P_ItemUC
								   {
									   FileEntrys = _entries,
									   YMD = _entries[0].taken_time.ToString("yyyy-MM-dd"),
									   MyMainWindow = m_mainWindow,
									   CurrentDevice = m_currentDevice
								   };

				_ctl.SetUI();

				m_photosCount += _ctl.PhotosCount;
				m_videosCount += _ctl.VideosCount;

				m_eventUCs.Add(_ctl);

				DoEvents();
			}
		}

		private void ShowEvents()
		{
			m_photosCount = 0;
			m_videosCount = 0;

			Dictionary<string, List<FileEntry>> _YMD_Files = GroupingByDay();

			foreach (List<FileEntry> _entries in m_days)
			{
				P_ItemUC _ctl = null;
				string _YMD = _entries[0].taken_time.ToString("yyyy-MM-dd");

				if (m_YMD_Files.Keys.Contains(_YMD))
				{
					foreach (P_ItemUC _eventUc in m_eventUCs)
					{
						if (_eventUc.YMD == _YMD)
						{
							_ctl = _eventUc;
							break;
						}
					}

					if (_ctl == null)
					{
						continue;
					}

					_ctl.FileEntrys = _entries;
				}
				else
				{
					_ctl = new P_ItemUC
							   {
								   FileEntrys = _entries,
								   YMD = _YMD,
								   MyMainWindow = m_mainWindow,
								   CurrentDevice = m_currentDevice
							   };

					_ctl.Changed = true;

					m_eventUCs.Add(_ctl);
				}

				_ctl.SetUI();

				m_photosCount += _ctl.PhotosCount;
				m_videosCount += _ctl.VideosCount;

				DoEvents();
			}

			m_YMD_Files = _YMD_Files;
		}

		public static string GetCountsString(int photosCount, int videosCount)
		{
			string _c = string.Empty;

			string _photo = " " + (string)Application.Current.FindResource("photo");
			string _photos = " " + (string)Application.Current.FindResource("photos");
			string _video = " " + (string)Application.Current.FindResource("video");
			string _videos = " " + (string)Application.Current.FindResource("videos");

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

		#region DoEvents

		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void DoEvents()
		{
			var _frame = new DispatcherFrame();

			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), _frame);

			Dispatcher.PushFrame(_frame);
		}

		public object ExitFrame(object f)
		{
			((DispatcherFrame)f).Continue = false;

			return null;
		}

		#endregion
	}
}