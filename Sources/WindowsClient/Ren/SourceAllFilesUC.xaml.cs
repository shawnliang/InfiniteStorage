#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using InfiniteStorage.Model;
using Waveface.ClientFramework;
using Waveface.Model;

#endregion

namespace Waveface.Client
{
	public partial class SourceAllFilesUC : UserControl
	{
		public static SourceAllFilesUC Current { get; set; }

		public static int BY_MONTH = 30*24*60;

		private IService m_currentDevice;
		private IContentGroup m_unsortedGroup;
		private List<string> m_defaultEventNameCache;
		private ObservableCollection<EventUC> m_eventUCs;
		private DispatcherTimer m_dispatcherTimer;
		private MainWindow m_mainWindow;

		public int VideosCount { get; set; }
		public int PhotosCount { get; set; }
		public RT Rt { get; set; }

		public SourceAllFilesUC()
		{
			Current = this;

			InitializeComponent();

			Cursor = Cursors.Wait;

			InitTimer();
		}

		private void InitTimer()
		{
			m_dispatcherTimer = new DispatcherTimer();
			m_dispatcherTimer.Tick += dispatcherTimer_Tick;
			m_dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
		}

		public bool Init(IService device, IContentGroup unsortedGroup, MainWindow mainWindow)
		{
			m_mainWindow = mainWindow;

			m_dispatcherTimer.Stop();
			btnRefresh.Visibility = Visibility.Collapsed;

			m_currentDevice = device;
			m_unsortedGroup = unsortedGroup;

			Rt = new RT();

			List<FileAsset> m_files;

			using (var _db = new MyDbContext())
			{
				var _q = from _f in _db.Object.Files
				         where _f.device_id == device.ID && !_f.deleted
				         select _f;

				m_files = _q.ToList();
			}

			if (Rt.Init(m_files, device))
			{
				gridEmptyPanel.Visibility = Visibility.Collapsed;

				tbTitle0.Visibility = Visibility.Visible;
				tbTitle.Visibility = Visibility.Visible;
				tbTotalCount.Visibility = Visibility.Visible;
			}
			else
			{
				gridEmptyPanel.Visibility = Visibility.Visible;
				btnRefresh.Visibility = Visibility.Collapsed;

				tbTitle0.Visibility = Visibility.Collapsed;
				tbTitle.Visibility = Visibility.Collapsed;
				tbTotalCount.Visibility = Visibility.Collapsed;

				return false;
			}

			ShowEvents();

			ShowTitle();

			//m_dispatcherTimer.Start();

			return true;
		}

		private void dispatcherTimer_Tick(object sender, EventArgs e)
		{
			int _count = m_unsortedGroup.ContentCount;

			if (_count != Rt.RtData.file_changes.Count)
			{
				btnRefresh.Visibility = Visibility.Visible;
			}

			int _d = (_count - (VideosCount + PhotosCount));

			if (_d > 0)
			{
				btnRefresh.Visibility = Visibility.Visible;
			}
			else
			{
				btnRefresh.Visibility = Visibility.Collapsed;
			}

			string _Refresh = (string) Application.Current.FindResource("Refresh");
			btnRefresh.Content = _Refresh + " (" + _d + ")";
		}

		public void Stop()
		{
			if (m_dispatcherTimer != null)
			{
				m_dispatcherTimer.Stop();
			}
		}

		private void btnRefresh_Click(object sender, RoutedEventArgs e)
		{
			m_dispatcherTimer.Stop();

			Cursor = Cursors.Wait;

			DoEvents();

			Init(m_currentDevice, m_unsortedGroup, m_mainWindow);

			Cursor = Cursors.Arrow;
		}

		private void ShowTitle()
		{
			tbTitle.Text = m_currentDevice.Name;
		}

		private void ShowEvents()
		{
			Cursor = Cursors.Wait;

			m_defaultEventNameCache = new List<string>();

			PhotosCount = 0;
			VideosCount = 0;

			m_eventUCs = new ObservableCollection<EventUC>();
			listBoxEvent.ItemsSource = m_eventUCs;

			Rt.GroupingByEvent();

			Rt.Events.Reverse();

			foreach (List<FileChange> _event in Rt.Events)
			{
				EventUC _ctl = new EventUC
					               {
						               Event = _event
					               };

				_ctl.SetUI();

				PhotosCount += _ctl.PhotosCount;
				VideosCount += _ctl.VideosCount;

				m_eventUCs.Add(_ctl);

				DoEvents();
			}

			ShowInfor();
		}

		private void ShowInfor()
		{
			if (m_eventUCs.Count == 0)
			{
				gridEmptyPanel.Visibility = Visibility.Visible;
				btnRefresh.Visibility = Visibility.Collapsed;
				tbTotalCount.Text = "";
			}
			else
			{
				//string _timeLineInformation = (string) Application.Current.FindResource("TimeLineInformation");
				//string _plural = (string) Application.Current.FindResource("plural");
				//string _tbTotalCount = string.Format(_timeLineInformation, GetCountsString(PhotosCount, VideosCount), m_eventUCs.Count, ((m_eventUCs.Count > 1) ? _plural : ""));
				
				string _tbTotalCount = GetCountsString(PhotosCount, VideosCount);
				gridEmptyPanel.Visibility = Visibility.Collapsed;
				tbTotalCount.Text = _tbTotalCount;
			}
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

		private void listBoxEvent_LayoutUpdated(object sender, EventArgs e)
		{
			Cursor = Cursors.Arrow;
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Cursor = Cursors.Wait;
		}

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
			((DispatcherFrame) f).Continue = false;

			return null;
		}

		#endregion

		#region Misc

		public void Sub_SelectionChanged()
		{
			int _sum = 0;

			foreach (EventUC _eventUc in listBoxEvent.Items)
			{
				_sum += _eventUc.GetSelectedFiles().Count;
			}

			Console.WriteLine("Sub_SelectionChanged: " + _sum);
		}

		public List<FileChange> GetAllSelectedFiles()
		{
			List<FileChange> _allFileChanges = new List<FileChange>();

			foreach (EventUC _eventUc in listBoxEvent.Items)
			{
				_allFileChanges.AddRange(_eventUc.GetSelectedFiles());
			}

			return _allFileChanges;
		}

		public List<Content> Sub_GetAllSelectedFiles_ContentEntitys(bool simple)
		{
			List<FileChange> _allSelectedFiles = GetAllSelectedFiles();

			return ConvertToContentEntities(simple, _allSelectedFiles);
		}

		private List<Content> ConvertToContentEntities(bool simple, IEnumerable<FileChange> _allSelectedFiles)
		{
			List<Content> _contentEntitys = new List<Content>();

			foreach (FileChange _fc in _allSelectedFiles)
			{
				if (simple)
				{
					_contentEntitys.Add(new Content
						                    {
							                    ID = _fc.id,
							                    Service = m_currentDevice
						                    });
				}
				else
				{
					_contentEntitys.Add(new BunnyContent(new Uri(_fc.saved_path), _fc.id, (_fc.type == 0 ? ContentType.Photo : ContentType.Video)) {Service = m_currentDevice});
				}
			}

			return _contentEntitys;
		}

		#endregion
	}
}