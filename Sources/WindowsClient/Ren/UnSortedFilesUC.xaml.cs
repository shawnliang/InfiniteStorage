#region

using System.Linq;
using InfiniteStorage.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Permissions;
using System.Threading;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Waveface.ClientFramework;
using Waveface.Model;
using Image = System.Drawing.Image;

#endregion

namespace Waveface.Client
{
	public partial class UnSortedFilesUC : UserControl
	{
		internal class MySliderTick
		{
			public string Name { get; set; }
			public int Value { get; set; }
		}

		public static int BY_DAY = 24 * 60;
		public static int BY_WEEK = 7 * 24 * 60;
		public static int BY_MONTH = 30 * 24 * 60;

		public static UnSortedFilesUC Current { get; set; }

		private IService m_currentDevice;
		private List<MySliderTick> m_sliderTicks = new List<MySliderTick>();
		private List<string> m_defaultEventNameCache;
		private ObservableCollection<EventUC> m_eventUCs;

		public int VideosCount { get; set; }
		public int PhotosCount { get; set; }
		public RT Rt { get; set; }
		public int GroupingEventInterval { get; set; }

		private DispatcherTimer m_dispatcherTimer;
		private bool m_humbDraging;
		private int m_pendingFilesCount;
		private MainWindow m_mainWindow;

		public UnSortedFilesUC()
		{
			Current = this;

			InitializeComponent();

			Cursor = Cursors.Wait;

			InitSliderTicks();

			InitTimer();
		}

		private void InitTimer()
		{
			m_dispatcherTimer = new DispatcherTimer();
			m_dispatcherTimer.Tick += dispatcherTimer_Tick;
			m_dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
		}

		private void InitSliderTicks()
		{
			m_sliderTicks.Add(new MySliderTick { Name = "30 Minutes", Value = 30 });
			m_sliderTicks.Add(new MySliderTick { Name = "1 Hour", Value = 1 * 60 });
			m_sliderTicks.Add(new MySliderTick { Name = "2 Hours", Value = 2 * 60 });
			m_sliderTicks.Add(new MySliderTick { Name = "4 Hours", Value = 4 * 60 });
			m_sliderTicks.Add(new MySliderTick { Name = "Day", Value = BY_DAY });
			m_sliderTicks.Add(new MySliderTick { Name = "Week", Value = BY_WEEK });
			m_sliderTicks.Add(new MySliderTick { Name = "Month", Value = BY_MONTH });

			SetEventIntervalTypeText();
		}

		private void SetEventIntervalTypeText()
		{
			if (m_sliderTicks.Count > 0)
			{
				tbEventIntervalType.Text = m_sliderTicks[(int)sliderEvent.Value].Name;
			}
		}

		public bool Init(IService device, MainWindow mainWindow)
		{
			m_mainWindow = mainWindow;

			m_dispatcherTimer.Stop();
			btnRefresh.Visibility = Visibility.Collapsed;

			m_currentDevice = device;

			Rt = new RT();

			List<FileAsset> _files;
			List<PendingFile> _pendingFiles;

			using (var db = new MyDbContext())
			{
				var q = from f in db.Object.Files
						where f.device_id == device.ID
						select f;

				_files = q.ToList();

				var q2 = from f in db.Object.PendingFiles
						 where f.device_id == device.ID
						 select f;

				_pendingFiles = q2.ToList();
			}

			m_pendingFilesCount = _pendingFiles.Count;

			if (Rt.Init(_files, _pendingFiles, device))
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

			m_dispatcherTimer.Start();

			return true;
		}

		void dispatcherTimer_Tick(object sender, EventArgs e)
		{
			int _count = BunnyUnsortedContentGroup.countUnsortedItems(m_currentDevice.ID);

			if (_count != Rt.RtData.file_changes.Count) //m_pendingFilesCount
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

			btnRefresh.Content = "Refresh " + "(" + _d + ")";
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

			Init(m_currentDevice, m_mainWindow);

			Cursor = Cursors.Arrow;

			GC.Collect();
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

			GroupingEventInterval = m_sliderTicks.ToArray()[(int)sliderEvent.Value].Value;
			Rt.GroupingByEvent(GroupingEventInterval);

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

				_ctl.DescribeText = GetDefaultEventName(Rt.DateTimeCache[_event[0].taken_time]);

				m_eventUCs.Add(_ctl);
			}

			DoEvents();

			DataTemplate _dataTemplate = FindResource("SbPreviewTemplate") as DataTemplate;

			listBoxEvent.SetValue(ScrollingPreviewService.VerticalScrollingPreviewTemplateProperty, _dataTemplate);

			ShowInfor();

			GC.Collect();

			SetEventIntervalTypeText();
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
				gridEmptyPanel.Visibility = Visibility.Collapsed;
				tbTotalCount.Text = GetCountsString(PhotosCount, VideosCount) + " in " + m_eventUCs.Count + " event" + ((m_eventUCs.Count > 1) ? "s" : "");
			}
		}

		public static string GetCountsString(int photosCount, int videosCount)
		{
			string _c = string.Empty;

			if (photosCount > 0)
			{
				_c = photosCount + ((photosCount == 1) ? " photo" : " photos");
			}

			if (videosCount > 0)
			{
				if (photosCount > 0)
				{
					_c = _c + ", ";
				}

				_c = _c + videosCount + ((videosCount == 1) ? " video" : " videos");
			}

			return _c;
		}

		#region Default Event Name

		private string GetDefaultEventName(DateTime dt)
		{
			if (GroupingEventInterval < 24 * 60)
			{
				return DefaultEventNameBy_Minutes(dt);
			}

			if (GroupingEventInterval == BY_DAY)
			{
				return DefaultEventNameBy_Day(dt);
			}

			if (GroupingEventInterval == BY_WEEK)
			{
				return DefaultEventNameBy_Week(dt);
			}

			if (GroupingEventInterval == BY_MONTH)
			{
				return DefaultEventNameBy_Month(dt);
			}

			return string.Empty;
		}

		private string DefaultEventNameBy_Month(DateTime dt)
		{
			return dt.ToString("yyyy.MM");
		}

		private string DefaultEventNameBy_Week(DateTime dt)
		{
			DateTime _startOfWeek = StartOfWeek(dt);
			DateTime _endOfWeek = _startOfWeek.AddDays(6);

			return _startOfWeek.ToString("yyyy.MM.dd") + "-" + ((_startOfWeek.Year != _endOfWeek.Year) ? _endOfWeek.ToString("yyyy.MM.dd") : _endOfWeek.ToString("MM.dd"));
		}

		private string DefaultEventNameBy_Day(DateTime dt)
		{
			return dt.ToString("yyyy.MM.dd");
		}

		private string DefaultEventNameBy_Minutes(DateTime dt)
		{
			string _s = string.Empty;

			string _date = dt.ToString("yyyy.MM.dd");
			string _period = GetTimePeriod(dt.Hour, dt.Minute);

			_s = _date + " " + _period;

			if (!m_defaultEventNameCache.Contains(_s))
			{
				m_defaultEventNameCache.Add(_s);
				return _s;
			}

			_s = _date + " " + dt.ToString("HH") + " " + (dt.Hour > 12 ? "PM" : "AM");

			if (!m_defaultEventNameCache.Contains(_s))
			{
				m_defaultEventNameCache.Add(_s);
				return _s;
			}

			_s = _date + " " + dt.ToString("HHmm");

			if (!m_defaultEventNameCache.Contains(_s))
			{
				m_defaultEventNameCache.Add(_s);
				return _s;
			}

			_s = _date + " " + dt.ToString("HHmmss");

			return _s;
		}

		private string GetTimePeriod(int hour, int minute)
		{
			int _x = hour * 60 + minute;

			if (_x > (23 * 60 + 59))
				return "Late Night";

			if (_x > (20 * 60 + 29))
				return "Night";

			if (_x > (17 * 60 + 59))
				return "Evening";

			if (_x > (12 * 60 + 59))
				return "Afternoon";

			if (_x > (11 * 60 + 59))
				return "Noon";

			if (_x > (7 * 60 + 59))
				return "Morning";

			return "Early Morning";
		}

		#endregion

		private void sliderEvent_ThumbDragStarted(object sender, DragStartedEventArgs e)
		{
			m_humbDraging = true;
		}

		private void sliderEvent_ThumbDragCompleted(object sender, DragCompletedEventArgs e)
		{
			m_humbDraging = false;

			ShowEvents();
		}

		private void sliderEvent_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (m_sliderTicks.Count == 0)
			{
				return;
			}

			if (m_humbDraging)
			{
				return;
			}

			ShowEvents();
		}

		private void listBoxEvent_LayoutUpdated(object sender, EventArgs e)
		{
			Cursor = Cursors.Arrow;
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Cursor = Cursors.Wait;
		}

		public static DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Sunday)
		{
			int _diff = dt.DayOfWeek - startOfWeek;

			if (_diff < 0)
			{
				_diff += 7;
			}

			return dt.AddDays(-1 * _diff).Date;
		}

		public int WeekOfYear(DateTime dt)
		{
			CultureInfo _cul = CultureInfo.CurrentCulture;

			int _weekNum = _cul.Calendar.GetWeekOfYear(
				dt,
				CalendarWeekRule.FirstFourDayWeek,
				DayOfWeek.Monday);

			return _weekNum;
		}

		public void AddEventToFolder(EventUC eventUC)
		{
			Cursor = Cursors.Wait;

			if (!DoImport(eventUC, false))
			{
				Cursor = Cursors.Arrow;
				return;
			}

			double _h = eventUC.ActualHeight / 10;

			for (int i = 10; i > 0; i--)
			{
				eventUC.Height = i * _h;
				DoEvents();
				Thread.Sleep(50);
			}

			VideosCount -= eventUC.VideosCount;
			PhotosCount -= eventUC.PhotosCount;

			Rt.RemoveFileChanges(eventUC.Event);

			m_eventUCs.Remove(eventUC);

			ShowInfor();

			m_currentDevice.Refresh();

			Cursor = Cursors.Arrow;
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
			((DispatcherFrame)f).Continue = false;

			return null;
		}

		#endregion

		private void btnImportAll_Click(object sender, RoutedEventArgs e)
		{
			if (!DoImport(null, true))
			{
				return;
			}

			m_eventUCs.Clear();

			ShowInfor();

			m_currentDevice.Refresh();
		}

		private bool DoImport(EventUC eventUC, bool all)
		{
			Cursor = Cursors.Wait;

			m_dispatcherTimer.Stop();

			sliderEvent.IsEnabled = false;

			PendingSort _pendingSort = new PendingSort
			{
				device_id = m_currentDevice.ID
			};

			if (all)
			{
				foreach (EventUC _item in listBoxEvent.Items)
				{
					Event _event = new Event
									   {
										   type = 1,
										   title = _item.DescribeText,
										   time_start = Rt.DateTimeCache[_item.Event[0].taken_time].ToString("yyyy-MM-dd HH:mm:ss"),
										   time_end = Rt.DateTimeCache[_item.Event[_item.Event.Count - 1].taken_time].ToString("yyyy-MM-dd HH:mm:ss")
									   };

					foreach (FileChange _fileChange in _item.Event)
					{
						_event.files.Add(_fileChange.id);
					}

					_pendingSort.events.Add(_event);
				}
			}
			else
			{
				Event _event = new Event
								   {
									   type = 1,
									   title = eventUC.DescribeText,
									   time_start = Rt.DateTimeCache[eventUC.Event[0].taken_time].ToString("yyyy-MM-dd HH:mm:ss"),
									   time_end = Rt.DateTimeCache[eventUC.Event[eventUC.Event.Count - 1].taken_time].ToString("yyyy-MM-dd HH:mm:ss")
								   };

				foreach (FileChange _fileChange in eventUC.Event)
				{
					_event.files.Add(_fileChange.id);
				}

				_pendingSort.events.Add(_event);
			}

			string _how = string.Empty;

			try
			{
				_how = JsonConvert.SerializeObject(_pendingSort);
			}
			catch
			{
				return false;
			}

			try
			{
				string _url = "http://127.0.0.1:14005" + "/pending/sort";

				string _parms = "how" + "=" + HttpUtility.UrlEncode(_how);

				WebPostHelper _webPos = new WebPostHelper();
				bool _isOK = _webPos.doPost(_url, _parms, null, 10000);

				if (!_isOK)
					return false;

				_webPos.getContent();
			}
			catch
			{
			}

			sliderEvent.IsEnabled = true;

			m_dispatcherTimer.Start();

			Cursor = Cursors.Arrow;

			return true;
		}

		#region Misc

		public void SelectAll(bool flag)
		{
			foreach (EventUC _eventUc in listBoxEvent.Items)
			{
				_eventUc.SelectAll(flag);
			}
		}

		public bool SaveToFavorite()
		{
			List<FileChange> _allFileChanges = new List<FileChange>();
			List<ContentEntity> _contentEntitys = new List<ContentEntity>();

			foreach (EventUC _eventUc in listBoxEvent.Items)
			{
				_allFileChanges.AddRange(_eventUc.GetSelectedFiles());
			}

			foreach (FileChange _fc in _allFileChanges)
			{
				_contentEntitys.Add(new ContentEntity { ID = _fc.id });
			}

			return m_mainWindow.SaveToFavorite(_contentEntitys);
		}

		public bool MoveToFolder()
		{
			List<FileChange> _allFileChanges = new List<FileChange>();

			foreach (EventUC _eventUc in listBoxEvent.Items)
			{
				_allFileChanges.AddRange(_eventUc.GetSelectedFiles());
			}

			if (_allFileChanges.Count == 0)
				return false;

			PendingSort _pendingSort = new PendingSort
			{
				device_id = m_currentDevice.ID
			};

			var _dialog = new CreateFolderDialog
				              {
					              Owner = m_mainWindow, 
								  WindowStartupLocation = WindowStartupLocation.CenterOwner
				              };

			if (_dialog.ShowDialog() != true)
				return false;

			if (_dialog.FolderName == string.Empty)
				return false;

			_allFileChanges.Sort((x, y) => String.Compare(x.taken_time, y.taken_time, StringComparison.Ordinal));

			Event _event = new Event
			{
				type = 1,
				title = _dialog.FolderName,
				time_start = _allFileChanges[0].taken_time ,
				time_end = _allFileChanges[_allFileChanges.Count - 1].taken_time
			};

			foreach (FileChange _fileChange in _allFileChanges)
			{
				_event.files.Add(_fileChange.id);
			}

			_pendingSort.events.Add(_event);

			string _how = string.Empty;

			try
			{
				_how = JsonConvert.SerializeObject(_pendingSort);
			}
			catch
			{
				return false;
			}

			try
			{
				string _url = "http://127.0.0.1:14005" + "/pending/sort";

				string _parms = "how" + "=" + HttpUtility.UrlEncode(_how);

				WebPostHelper _webPos = new WebPostHelper();
				bool _isOK = _webPos.doPost(_url, _parms, null, 10000);

				if (!_isOK)
					return false;

				_webPos.getContent();
			}
			catch
			{
			}

			m_currentDevice.Refresh();

			return true;
		}

		#endregion

		#region ContentEntity

		public class ContentEntity : IContentEntity
		{
			public string ID { get; set; }
			public string Name { get; set; }
			public Uri Uri { get; set; }
			public IContentEntity Parent { get; set; }
			public string ContentPath { get; set; }
			public Image Image { get; set; }
			public Image Thumbnail { get; set; }
			public long Size { get; set; }
			public DateTime CreateTime { get; set; }
			public DateTime ModifyTime { get; set; }
			public string Description { get; set; }
			public Dictionary<string, string> Memo { get; set; }
		}

		#endregion


	}
}
