#region

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Net;
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

        public bool Init(IService device)
        {
            m_dispatcherTimer.Stop();
            btnRefresh.Visibility = Visibility.Collapsed;

            m_currentDevice = device;

            string _allPendingFiles = processAllFile(m_currentDevice.ID);

            if(_allPendingFiles == "")
            {
                return false;
            }

            Rt = new RT();

            if (Rt.Init(_allPendingFiles))
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

            Init(m_currentDevice);

            Cursor = Cursors.Arrow;

            GC.Collect();
        }

        #region REST

        private static string HostIP = "http://127.0.0.1:14005";
        private string getPending_cmd0 = HostIP + "/pending/get?device_id=";
        private string getPending_cmd1 = "&seq=";
        private string getPending_cmd2 = "&limit=500";
        private bool m_humbDraging;

        #region ExtendedWebClient

        public class ExtendedWebClient : WebClient
        {
            public int Timeout { get; set; }

            public ExtendedWebClient(int timeOut)
            {
                Timeout = timeOut;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var _objWebRequest = base.GetWebRequest(address);

                _objWebRequest.Timeout = Timeout;
                return _objWebRequest;
            }
        }

        #endregion

        private string processAllFile(string device)
        {
            bool _firstTime = true;
            RtData _rtData = new RtData();

            try
            {
                int files_from_seq = 0;
                int remaining_count = 1;

                while (remaining_count != 0)
                {
                    string getPending_cmd = getPending_cmd0 + device + getPending_cmd1 + files_from_seq + getPending_cmd2;
                    string _getData = HttpGet(getPending_cmd);

                    if(_getData == "-1")
                    {
                        return "";
                    }

                    RtData _tempRTData = JsonConvert.DeserializeObject<RtData>(_getData);
                    remaining_count = _tempRTData.remaining_count;

                    FileChange _last = _tempRTData.file_changes[_tempRTData.file_changes.Count - 1];

                    files_from_seq = _last.seq + 1;

                    if (_firstTime)
                    {
                        _firstTime = false;

                        _rtData = _tempRTData;
                    }
                    else
                    {
                        foreach (FileChange _fileChange in _tempRTData.file_changes)
                        {
                            _rtData.file_changes.Add(_fileChange);
                        }
                    }
                }
            }
            catch
            {
            }

            string _ret = JsonConvert.SerializeObject(_rtData);

            return _ret;
        }

        private string HttpGet(string uri)
        {
            String _ret;
            ExtendedWebClient _webClient = new ExtendedWebClient(5000);

            using (_webClient)
            {
                try
                {
                    // open and read from the supplied URI
                    Stream _stream = _webClient.OpenRead(uri);
                    StreamReader _reader = new StreamReader(_stream);
                    _ret = _reader.ReadToEnd();
                }
                catch (WebException _ex)
                {
                    if (_ex.Response is HttpWebResponse)
                    {
                        // Add you own error handling as required
                        switch (((HttpWebResponse)_ex.Response).StatusCode)
                        {
                            case HttpStatusCode.NotFound:
                            case HttpStatusCode.Unauthorized:
                                _ret = null;
                                break;

                            default:
                                throw _ex;
                        }
                    }

                    _ret = "-1";
                }
            }

            return _ret;
        }

        #endregion

        private void ShowTitle()
        {
            tbTitle.Text = Rt.RtData.file_changes[0].dev_name;
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

            if(!DoImport(eventUC, false))
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
            if(!DoImport(null, true))
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
    }
}
