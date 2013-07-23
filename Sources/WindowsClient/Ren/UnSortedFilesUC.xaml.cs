#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using InfiniteStorage.Model;
using Newtonsoft.Json;
using Waveface.ClientFramework;
using Waveface.Model;

#endregion

namespace Waveface.Client
{
	public partial class UnSortedFilesUC : UserControl
	{
		public static UnSortedFilesUC Current { get; set; }

		internal class MySliderTick
		{
			public string Name { get; set; }
			public int Value { get; set; }
		}

		public static int BY_DAY = 24 * 60;
		public static int BY_WEEK = 7 * 24 * 60;
		public static int BY_MONTH = 30 * 24 * 60;

		private IService m_currentDevice;
		private IContentGroup m_unsortedGroup;
		private List<MySliderTick> m_sliderTicks = new List<MySliderTick>();
		private List<string> m_defaultEventNameCache;
		private ObservableCollection<EventUC> m_eventUCs;
		private DispatcherTimer m_dispatcherTimer;
		private bool m_humbDraging;
		private int m_pendingFilesCount;
		private MainWindow m_mainWindow;
		private EventUC m_startEventUc;
		private EventUC m_endEventUc;
		private int m_startIndex;

		public int VideosCount { get; set; }
		public int PhotosCount { get; set; }
		public RT Rt { get; set; }
		public int GroupingEventInterval { get; set; }

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
			string _Minutes_30 = (string)Application.Current.FindResource("Minutes_30");
			string _Hour_1 = (string)Application.Current.FindResource("Hour_1");
			string _Hours_2 = (string)Application.Current.FindResource("Hours_2");
			string _Hours_4 = (string)Application.Current.FindResource("Hours_4");
			string _Day = (string)Application.Current.FindResource("Day");
			string _Week = (string)Application.Current.FindResource("Week");
			string _Month = (string)Application.Current.FindResource("Month");

			m_sliderTicks.Add(new MySliderTick { Name = _Minutes_30, Value = 30 });
			m_sliderTicks.Add(new MySliderTick { Name = _Hour_1, Value = 1 * 60 });
			m_sliderTicks.Add(new MySliderTick { Name = _Hours_2, Value = 2 * 60 });
			m_sliderTicks.Add(new MySliderTick { Name = _Hours_4, Value = 4 * 60 });
			m_sliderTicks.Add(new MySliderTick { Name = _Day, Value = BY_DAY });
			m_sliderTicks.Add(new MySliderTick { Name = _Week, Value = BY_WEEK });
			m_sliderTicks.Add(new MySliderTick { Name = _Month, Value = BY_MONTH });

			SetEventIntervalTypeText();
		}

		private void SetEventIntervalTypeText()
		{
			if (m_sliderTicks.Count > 0)
			{
				tbEventIntervalType.Text = " " + m_sliderTicks[(int)sliderEvent.Value].Name;
			}
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
			List<PendingFile> m_pendingFiles;

			using (var _db = new MyDbContext())
			{
				var _q = from _f in _db.Object.Files
						 where _f.device_id == device.ID && !_f.deleted
						 select _f;

				m_files = _q.ToList();

				var _q2 = from _f in _db.Object.PendingFiles
						  where _f.device_id == device.ID && !_f.deleted
						  select _f;

				m_pendingFiles = _q2.ToList();
			}

			m_pendingFilesCount = m_pendingFiles.Count;

			if (Rt.Init(m_files, m_pendingFiles, device))
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

			string _Refresh = (string)Application.Current.FindResource("Refresh");
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

			GC.Collect();
		}

		private void ShowTitle()
		{
			tbTitle.Text = m_currentDevice.Name;
		}

		private void ShowEvents()
		{
			BunnyLabelContentGroup _contentEntity = (BunnyLabelContentGroup)ClientFramework.Client.Default.Recent[0];

			List<string> _starredIDs = GetStarredIDs(_contentEntity);

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

				_ctl.SetUI(_starredIDs);

				PhotosCount += _ctl.PhotosCount;
				VideosCount += _ctl.VideosCount;

				_ctl.DescribeText = GetDefaultEventName(Rt.DateTimeCache[_event[0].taken_time]);

				_ctl.lbEvent.SelectionChanged += lbEvent_SelectionChanged;
				_ctl.lbEvent.PreviewMouseDown += lbEvent_PreviewMouseDown;
				_ctl.lbEvent.MouseDoubleClick += lbEvent_MouseDoubleClick;

				m_eventUCs.Add(_ctl);
			}

			DoEvents();

			DataTemplate _dataTemplate = FindResource("SbPreviewTemplate") as DataTemplate;

			listBoxEvent.SetValue(ScrollingPreviewService.VerticalScrollingPreviewTemplateProperty, _dataTemplate);

			ShowInfor();

			GC.Collect();

			SetEventIntervalTypeText();
		}

		private List<string> GetStarredIDs(BunnyLabelContentGroup contentEntity)
		{
			List<string> _starredIDs = new List<string>();

			foreach (IContentEntity _contentEntity in contentEntity.Contents)
			{
				_starredIDs.Add(_contentEntity.ID);
			}

			return _starredIDs;
		}

		private void lbEvent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Sub_SlideShow();
		}

		private static T FindAnchestor<T>(DependencyObject current)
			where T : DependencyObject
		{
			do
			{
				if (current is T)
				{
					return (T)current;
				}

				current = VisualTreeHelper.GetParent(current);
			} while (current != null);

			return null;
		}

		private void lbEvent_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				ListBox _listBox = sender as ListBox;
				ListBoxItem _listBoxItem = FindAnchestor<ListBoxItem>((DependencyObject)e.OriginalSource);

				PropertyInfo _pi = typeof(ListBox).GetProperty("AnchorItem", BindingFlags.NonPublic | BindingFlags.Instance);

				if (_pi != null)
				{
					_pi.SetValue(_listBox, _listBoxItem.DataContext, null);
				}
			}
			catch (Exception)
			{
			}
		}

		private void lbEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (e.AddedItems.Count > 0 && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
				{
					var _firstEventUcFound = false;
					m_endEventUc = listBoxEvent.Items.OfType<EventUC>().Single(item => item.lbEvent == sender as ListBox);
					List<EventUC> middleEventUCs = new List<EventUC>();

					foreach (EventUC _eventUc in listBoxEvent.Items.OfType<EventUC>())
					{
						if (!_firstEventUcFound && (m_startEventUc == _eventUc || m_endEventUc == _eventUc))
						{
							_firstEventUcFound = true;

							if (m_endEventUc == _eventUc)
							{
								var temp = m_endEventUc;
								m_endEventUc = m_startEventUc;
								m_startEventUc = temp;
							}

							continue;
						}

						if (_firstEventUcFound && (m_startEventUc == _eventUc || m_endEventUc == _eventUc))
							break;

						if (!_firstEventUcFound)
							continue;

						middleEventUCs.Add(_eventUc);
					}

					if (m_startEventUc == m_endEventUc)
					{
						int endIndex = m_startEventUc.lbEvent.SelectedIndex;

						if (m_startIndex > endIndex)
						{
							int _temp = m_startIndex;
							m_startIndex = endIndex;
							endIndex = _temp;
						}

						for (int _index = m_startIndex; _index <= endIndex; ++_index)
						{
							ListBoxItem _item = m_startEventUc.lbEvent.ItemContainerGenerator.ContainerFromIndex(_index) as ListBoxItem;
							_item.IsSelected = true;
						}

						return;
					}

					foreach (var _eventUc in middleEventUCs)
					{
						_eventUc.lbEvent.SelectionChanged -= lbEvent_SelectionChanged;
						_eventUc.lbEvent.SelectAll();
						_eventUc.lbEvent.SelectionChanged += lbEvent_SelectionChanged;
					}

					m_startEventUc.Focus();
					m_startEventUc.lbEvent.SelectionChanged -= lbEvent_SelectionChanged;

					int _starEventSelectedIndex = m_startEventUc.lbEvent.SelectedIndex;

					for (int _index = 0; _index < m_startEventUc.lbEvent.Items.Count; ++_index)
					{
						ListBoxItem _item = m_startEventUc.lbEvent.ItemContainerGenerator.ContainerFromIndex(_index) as ListBoxItem;
						_item.IsSelected = _index >= _starEventSelectedIndex;
					}

					m_startEventUc.lbEvent.SelectionChanged += lbEvent_SelectionChanged;

					m_endEventUc.Focus();
					m_endEventUc.lbEvent.SelectionChanged -= lbEvent_SelectionChanged;

					var _endEventSelectedIndex = m_endEventUc.lbEvent.SelectedIndex;

					for (var _index = 0; _index < m_endEventUc.lbEvent.Items.Count; ++_index)
					{
						var _item = m_endEventUc.lbEvent.ItemContainerGenerator.ContainerFromIndex(_index) as ListBoxItem;
						_item.IsSelected = _index <= _endEventSelectedIndex;
					}

					m_endEventUc.lbEvent.SelectionChanged += lbEvent_SelectionChanged;
				}
				else if (e.AddedItems.Count > 0 || e.RemovedItems.Count > 0)
				{
					if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
					{
						foreach (EventUC _eventUc in listBoxEvent.Items.OfType<EventUC>().Where(item => !item.lbEvent.IsMouseCaptureWithin && !item.m_doSelectAll))
							_eventUc.lbEvent.UnselectAll();

						m_startEventUc = null;
						m_endEventUc = null;
						m_startIndex = 0;
					}

					if (m_startEventUc == null)
					{
						m_startEventUc = listBoxEvent.Items.OfType<EventUC>().Single(item => item.lbEvent == sender as ListBox);
						m_startIndex = m_startEventUc.lbEvent.SelectedIndex;
					}
				}
			}
			finally
			{
				int _count = listBoxEvent.Items.OfType<EventUC>().Select(item => item.lbEvent).Sum(item => item.SelectedItems.Count);

				string _format = (string)Application.Current.FindResource("TimelineSelectedCountText");
				lblSelectedCount.Content = string.Format(_format, _count);

				contentActionBar.IsEnabled = _count > 0;
			}
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
				string _timeLineInformation = (string)Application.Current.FindResource("TimeLineInformation");
				string _plural = (string)Application.Current.FindResource("plural");

				string _tbTotalCount = string.Format(_timeLineInformation, GetCountsString(PhotosCount, VideosCount), m_eventUCs.Count, ((m_eventUCs.Count > 1) ? _plural : ""));

				gridEmptyPanel.Visibility = Visibility.Collapsed;
				tbTotalCount.Text = _tbTotalCount;
			}
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

			_s = _date + " " + dt.ToString("HH") + " " + dt.ToString("tt");

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
			string _LateNight = (string)Application.Current.FindResource("LateNight");
			string _Night = (string)Application.Current.FindResource("Night");
			string _Evening = (string)Application.Current.FindResource("Evening");
			string _Afternoon = (string)Application.Current.FindResource("Afternoon");
			string _Noon = (string)Application.Current.FindResource("Noon");
			string _Morning = (string)Application.Current.FindResource("Morning");
			string _EarlyMorning = (string)Application.Current.FindResource("EarlyMorning");

			int _x = hour * 60 + minute;

			if (_x > (23 * 60 + 59))
				return _LateNight;

			if (_x > (20 * 60 + 29))
				return _Night;

			if (_x > (17 * 60 + 59))
				return _Evening;

			if (_x > (12 * 60 + 59))
				return _Afternoon;

			if (_x > (11 * 60 + 59))
				return _Noon;

			if (_x > (7 * 60 + 59))
				return _Morning;

			return _EarlyMorning;
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

		public void Sub_SelectAll(bool flag)
		{
			foreach (EventUC _eventUc in listBoxEvent.Items)
			{
				_eventUc.SelectAll(flag);
			}
		}

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

		public bool Sub_SaveToFavorite()
		{
			List<Content> _contentEntitys = Sub_GetAllSelectedFiles_ContentEntitys(true);

			if (_contentEntitys.Count == 0)
			{
				return false;
			}

			return m_mainWindow.SaveToFavorite(_contentEntitys);
		}

		public void Sub_SlideShow()
		{
			List<Content> _contentEntitys = Sub_GetAllSelectedFiles_ContentEntitys(false);

			int _index = 0;

			if (_contentEntitys.Count == 1)
			{
				List<FileChange> _allContents = listBoxEvent.Items.OfType<EventUC>().SelectMany(item => item.Event).ToList();
				_contentEntitys = ConvertToContentEntities(false, _allContents).ToList();

				foreach (EventUC _euc in listBoxEvent.Items.OfType<EventUC>())
				{
					if (_euc.IsMouseCaptureWithin)
					{
						_index += _euc.lbEvent.SelectedIndex + 1;
						break;
					}

					_index += _euc.lbEvent.Items.Count;
				}
			}

			_index -= 1;

			PhotoViewer _viewer = new PhotoViewer
							  {
								  Owner = m_mainWindow,
								  Source = _contentEntitys,
								  SelectedIndex = _index
							  };

			_viewer.ShowDialog();
		}

		public bool Sub_AddToFavorite()
		{
			List<Content> _contentEntitys = Sub_GetAllSelectedFiles_ContentEntitys(true);

			if (_contentEntitys.Count == 0)
			{
				return false;
			}

			return m_mainWindow.AddToFavorite(_contentEntitys);
		}

		private void StarSelectedContents()
		{
			List<Content> _contentEntitys = Sub_GetAllSelectedFiles_ContentEntitys(true);

			if (_contentEntitys.Count == 0)
			{
				return;
			}

			m_mainWindow.StarContent(_contentEntitys);

			foreach (EventUC _eventUc in listBoxEvent.Items)
			{
				_eventUc.DoAutoStarUI();
			}
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
					_contentEntitys.Add(new BunnyContent(new Uri(_fc.saved_path), _fc.id, (_fc.type == 0 ? ContentType.Photo : ContentType.Video)) { Service = m_currentDevice });
				}
			}

			return _contentEntitys;
		}

		public bool Sub_MoveToNewFolder()
		{
			var _dialog = new CreateFolderDialog
							  {
								  Owner = m_mainWindow,
								  WindowStartupLocation = WindowStartupLocation.CenterOwner
							  };

			if (_dialog.ShowDialog() != true)
				return false;

			if (string.IsNullOrEmpty(_dialog.CreateName))
				return false;

			try
			{
				var _targetPath = Path.Combine(Path.GetDirectoryName(m_unsortedGroup.Uri.LocalPath), _dialog.CreateName);

				StationAPI.Move(GetAllSelectedFiles().Select(x => x.id), _targetPath);
			}
			catch
			{
			}

			m_currentDevice.Refresh();

			return true;
		}

		private bool Sub_MoveToExistingFolder()
		{
			var _dialog = new MoveToFolderDialog
							 {
								 Owner = m_mainWindow,
								 WindowStartupLocation = WindowStartupLocation.CenterOwner
							 };

			List<IContentEntity> _folders = new List<IContentEntity>();

			foreach (IContentEntity _entity in m_currentDevice.Contents)
			{
				if ((_entity.Name == "Timeline") || (_entity.Name == "Unsorted"))
				{
					continue;
				}

				_folders.Add(_entity);
			}

			_dialog.ItemSource = _folders;

			if (_dialog.ShowDialog() != true)
				return false;

			try
			{
				string _targetPath = Path.Combine(Path.GetDirectoryName(m_unsortedGroup.Uri.LocalPath), _dialog.SelectedItem.ToString());

				StationAPI.Move(GetAllSelectedFiles().Select(x => x.id), _targetPath);
			}
			catch
			{
			}

			m_currentDevice.Refresh();

			return true;
		}

		#endregion

		private void ContentActionBar_AddToFavorite(object sender, EventArgs e)
		{
			Sub_AddToFavorite();
		}

		private void ContentActionBar_AddToStarred(object sender, EventArgs e)
		{
			StarSelectedContents();
		}

		private void ContentActionBar_CreateFavorite(object sender, EventArgs e)
		{
			Sub_SaveToFavorite();
		}

		private void ContentActionBar_MoveToNewFolder(object sender, EventArgs e)
		{
			Sub_MoveToNewFolder();
		}

		private void ContentActionBar_MoveToExistingFolder(object sender, EventArgs e)
		{
			Sub_MoveToExistingFolder();
		}

		public void ContextMenuItemAction(string name)
		{
			switch (name)
			{
				case "CreateFavorite":
					Sub_SaveToFavorite();
					break;

				case "CreateNewFolder":
					Sub_MoveToNewFolder();
					break;

				case "AddToFavorite":
					Sub_AddToFavorite();
					break;

				case "AddToStarred":
					StarSelectedContents();
					break;

				case "AddToExistingFolder":
					Sub_MoveToExistingFolder();
					break;
			}
		}

		public void DoStar(string fileID, bool tagged)
		{
			if (tagged)
			{
				List<Content> _contentEntitys = new List<Content>();

				_contentEntitys.Add(new Content
				{
					ID = fileID,
				});

				ClientFramework.Client.Default.Tag(_contentEntitys);
			}
			else
			{
				ClientFramework.Client.Default.UnTag(fileID);
			}
		}

		public void ShareEvent(EventUC eventUc, string describeText)
		{
			ShareEventDialog _dialog = new ShareEventDialog(eventUc, describeText);
			_dialog.Owner = m_mainWindow;
			_dialog.ShowDialog();

			List<string> _fileIDs = _dialog.FileIDs;
			string _name = _dialog.TitleName;

			_dialog = null;

			if ((_fileIDs != null) && (_name != string.Empty))
			{
				List<Content> _contents = new List<Content>();

				foreach (string _fileID in _fileIDs)
				{
					_contents.Add(new Content{ID = _fileID,});
				}			

				m_mainWindow.TimelineShareTo(_contents, _name);				
			}
		}
	}
}