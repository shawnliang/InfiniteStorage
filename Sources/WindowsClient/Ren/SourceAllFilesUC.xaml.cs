﻿#region

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

#endregion

namespace Waveface.Client
{
	public partial class SourceAllFilesUC : UserControl
	{
		private IService m_currentDevice;
		private MainWindow m_mainWindow;

		private DispatcherTimer m_startTimer;
		private DispatcherTimer m_refreshTimer;
		private DispatcherTimer m_sizeChangedDelayTimer;

		private int m_videosCount;
		private int m_photosCount;
		private int m_hasOriginCount;

		private string m_basePath;
		private string m_thumbsPath;
		private SolidColorBrush m_solidColorBrush;
		private List<FileEntry> m_fileEntries { get; set; }

		private Dictionary<string, List<FileEntry>> m_YM_Files;
		private List<List<FileEntry>> m_months;
		private ObservableCollection<EventUC> m_eventUCs;
		private int m_countPerLine;

		private bool m_inited;

		public SourceAllFilesUC()
		{
			InitializeComponent();

			m_basePath = (string) Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "ResourceFolder", "");
			m_thumbsPath = Path.Combine(m_basePath, ".thumbs");
			m_solidColorBrush = new SolidColorBrush(Color.FromArgb(255, 120, 0, 34));

			InitTimer();

			SizeChanged += SourceAllFilesUC_SizeChanged;
		}

		private void InitTimer()
		{
			m_startTimer = new DispatcherTimer();
			m_startTimer.Tick += StartTimerOnTick;
			m_startTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);

			m_refreshTimer = new DispatcherTimer();
			m_refreshTimer.Tick += RefreshTimerOnTick;
			m_refreshTimer.Interval = new TimeSpan(0, 0, 0, 0, 2000);

			m_sizeChangedDelayTimer = new DispatcherTimer();
			m_sizeChangedDelayTimer.Tick += SizeChangedDelayTimer_Tick;
			m_sizeChangedDelayTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
		}

		private void SourceAllFilesUC_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			m_sizeChangedDelayTimer.Stop();
			m_sizeChangedDelayTimer.Start();
		}

		private void getCountPerLine(double width)
		{
			m_countPerLine = (int) ((width - 8)/(60 + 1.5));
		}

		private void SizeChangedDelayTimer_Tick(object sender, EventArgs e)
		{
			m_sizeChangedDelayTimer.Stop();

			if (m_inited)
			{
				ShowEvents(true);
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

			m_inited = true;
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

					ShowEvents(false);
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
			var service = (BunnyService) m_currentDevice;

			tbAutoImport.IsChecked = (m_currentDevice as BunnyService).SyncEnabled;

			if (service.SyncEnabled)
			{
				if (service.RecvStatus.IsPreparing)
				{
					imgCircleProgress.Visibility = Visibility.Visible;
					
					header_title.Text = FindResource("header_title_indexing") as string;
					header_subtitle.Text = FindResource("header_subtitle_indexing") as string;
				}
				else if (service.RecvStatus.IsReceiving)
				{
					imgCircleProgress.Visibility = Visibility.Visible;
					//tbTitleInfo.Text = string.Format("Importing... ({0} / {1})", service.RecvStatus.Received, service.RecvStatus.Total);

					var remaining_items =  service.RecvStatus.Total - service.RecvStatus.Received;
					var remaining_minute = (int)((remaining_items * 1.5) / 60);

					if (remaining_minute <= 0)
						remaining_minute = 1;

					header_title.Text = string.Format(FindResource("header_title_importing") as string, service.RecvStatus.Total);
					header_subtitle.Text = string.Format(FindResource("header_subtitle_importing") as string, service.RecvStatus.Received, remaining_minute);
				}
				else
				{
					imgCircleProgress.Visibility = Visibility.Collapsed;


					if (isImportComplete())
					{
						var lastImportInfo = getLastImportInfo();

						header_title.Text = string.Format(FindResource("header_title_complete") as string,
							lastImportInfo.count,
							lastImportInfo.x_days_ago > 0 ?
								string.Format(FindResource("x_days_ago") as string, lastImportInfo.x_days_ago) :
								FindResource("0_days_ago") as string);
						header_subtitle.Text = FindResource("header_subtitle_complete") as string;
					}
					else
					{
						header_title.Text = string.Format(FindResource("header_title_stopped") as string, getRemainCount());
						header_subtitle.Text = FindResource("header_subtitle_stopped") as string;
					}
				}
			}
			else
			{
				imgCircleProgress.Visibility = Visibility.Collapsed;

				header_title.Text = FindResource("header_title_paused") as string;
				header_subtitle.Text = FindResource("header_subtitle_paused") as string;
			}
		}

		private int getRemainCount()
		{
			using (var db = new MyDbContext())
			{
				var q = from f in db.Object.Files
						where !f.has_origin && f.device_id == m_currentDevice.ID
						select f;

				return q.Count();
			}
		}

		private bool isImportComplete()
		{
			using (var db = new MyDbContext())
			{
				var q = from f in db.Object.Files
						where !f.has_origin && f.device_id == m_currentDevice.ID
						select f;

				return !q.Any();
			}
		}

		private LastImportInfo getLastImportInfo()
		{
			using (var db = new MyDbContext())
			{
				var q = from f in db.Object.Files
						where f.import_time != null && f.device_id == m_currentDevice.ID
						select f.import_time;

				var lastImportTime = q.Max();

				if (lastImportTime != null)
				{
					var start = new DateTime(lastImportTime.Value.Year, lastImportTime.Value.Month, lastImportTime.Value.Day);
					var end = start.AddDays(1.0);

					var q2 = from f in db.Object.Files
							 where f.import_time != null && f.import_time >= start && f.import_time < end && f.device_id == m_currentDevice.ID
							 select 1;
					var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);


					return new LastImportInfo
					{
						x_days_ago = (int)(today - start).TotalDays,
						count = q2.Count()
					};
				}
				else
				{
					return new LastImportInfo();
				}
			}
		}

		private DateTime? getLastImportTime()
		{
			using (var db = new MyDbContext())
			{
				var q = from f in db.Object.Files
				        where f.import_time != null && f.device_id == m_currentDevice.ID
				        select f.import_time;

				return q.Max();
			}
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

		public Dictionary<string, List<FileEntry>> GroupingByMonth()
		{
			Dictionary<string, List<FileEntry>> _YM_Files = new Dictionary<string, List<FileEntry>>();

			m_months = new List<List<FileEntry>>();

			foreach (FileEntry _item in m_fileEntries)
			{
				DateTime _dt = _item.taken_time;

				string _by = _dt.ToString("yyyy-MM");

				if (!_YM_Files.ContainsKey(_by))
				{
					_YM_Files.Add(_by, new List<FileEntry>());
				}

				_YM_Files[_by].Add(_item);
			}

			_YM_Files.Keys.ToList().Sort();

			foreach (string _day in _YM_Files.Keys)
			{
				m_months.Add(_YM_Files[_day]);
			}

			m_months.Reverse();

			return _YM_Files;
		}

		#region Show

		private void ShowEvents_Init()
		{
			getCountPerLine(ActualWidth);

			m_photosCount = 0;
			m_videosCount = 0;

			m_eventUCs = new ObservableCollection<EventUC>();

			listBoxEvent.ItemsSource = m_eventUCs;

			m_YM_Files = GroupingByMonth();

			foreach (List<FileEntry> _entries in m_months)
			{
				EventUC _ctl = new EventUC
					               {
						               FileEntrys = _entries,
						               YM = _entries[0].taken_time.ToString("yyyy-MM"),
						               MyMainWindow = m_mainWindow,
						               CurrentDevice = m_currentDevice,
					               };

				_ctl.SetUI(m_countPerLine, true);

				m_photosCount += _ctl.PhotosCount;
				m_videosCount += _ctl.VideosCount;

				m_eventUCs.Add(_ctl);

				System.Windows.Forms.Application.DoEvents();
			}
		}

		private void ShowEvents(bool forceChange)
		{
			getCountPerLine(ActualWidth);

			m_photosCount = 0;
			m_videosCount = 0;

			Dictionary<string, List<FileEntry>> _YM_Files = GroupingByMonth();

			foreach (List<FileEntry> _entries in m_months)
			{
				EventUC _ctl = null;
				string _YM = _entries[0].taken_time.ToString("yyyy-MM");

				if (m_YM_Files.Keys.Contains(_YM))
				{
					foreach (EventUC _eventUc in m_eventUCs)
					{
						if (_eventUc.YM == _YM)
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
					_ctl = new EventUC
						       {
							       FileEntrys = _entries,
							       YM = _YM,
							       MyMainWindow = m_mainWindow,
							       CurrentDevice = m_currentDevice,
						       };

					_ctl.Changed = true;

					m_eventUCs.Add(_ctl);
				}

				_ctl.SetUI(m_countPerLine, forceChange);

				m_photosCount += _ctl.PhotosCount;
				m_videosCount += _ctl.VideosCount;

				System.Windows.Forms.Application.DoEvents();
			}

			m_YM_Files = _YM_Files;
		}

		public static string GetCountsString(int photosCount, int videosCount)
		{
			string _c = string.Empty;

			string _photo = " " + (string) Application.Current.FindResource("photo");
			string _photos = " " + (string) Application.Current.FindResource("photos");
			string _video = " " + (string) Application.Current.FindResource("video");
			string _videos = " " + (string) Application.Current.FindResource("videos");

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

		private void tbtnCloudSharing_Checked(object sender, RoutedEventArgs e)
		{
			(m_currentDevice as BunnyService).SyncEnabled = true;
			tbAutoImport.Background = m_solidColorBrush;
			tbAutoImport.Content = "關閉自動匯入";
			refreshTitleInfo();
		}

		private void tbtnCloudSharing_Unchecked(object sender, RoutedEventArgs e)
		{
			(m_currentDevice as BunnyService).SyncEnabled = false;
			tbAutoImport.Background = Brushes.DodgerBlue;
			tbAutoImport.Content = "開啟自動匯入";
			refreshTitleInfo();
		}
	}


	class LastImportInfo
	{
		public int count { get; set; }
		public int x_days_ago { get; set; }
	}
}