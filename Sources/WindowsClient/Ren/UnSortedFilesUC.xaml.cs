﻿#region

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
using Newtonsoft.Json;
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
        private bool m_init;
        private List<MySliderTick> m_sliderTicks = new List<MySliderTick>();
        private List<string> m_defaultEventNameCache;
        private ObservableCollection<EventUC> m_eventUCs;

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
        }

        public bool Init(IService device)
        {
            m_currentDevice = device;

            string _allPendingFiles = processAllFile(m_currentDevice.ID);

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

                tbTitle0.Visibility = Visibility.Collapsed;
                tbTitle.Visibility = Visibility.Collapsed;
                tbTotalCount.Visibility = Visibility.Collapsed;

                return false;
            }

            ShowEvents();

            m_init = true;

            ShowTitle();

            return true;
        }

        #region REST

        private static string HostIP = "http://127.0.0.1:14005";
        private string getPending_cmd0 = HostIP + "/pending/get?device_id=";
        private string getPending_cmd1 = "&seq=";
        private string getPending_cmd2 = "&limit=500";

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
            WebClient _webClient = new WebClient();

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
            Mouse.OverrideCursor = Cursors.Wait;

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

            DataTemplate _dataTemplate = FindResource("SbPreviewTemplate") as DataTemplate;

            listBoxEvent.SetValue(ScrollingPreviewService.VerticalScrollingPreviewTemplateProperty, _dataTemplate);

            ShowInfor();

            GC.Collect();
        }

        private void ShowInfor()
        {
            if (m_eventUCs.Count == 0)
            {
                gridEmptyPanel.Visibility = Visibility.Visible;
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

        private void sliderEvent_ThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            ShowEvents();
        }

        private void listBoxEvent_LayoutUpdated(object sender, EventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
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
            Mouse.OverrideCursor = Cursors.Wait;

            DoImport(eventUC, false);

            double _h = eventUC.ActualHeight / 10;

            for (int i = 10; i > 0; i--)
            {
                eventUC.Height = i * _h;
                DoEvents();
                Thread.Sleep(50);
            }

            VideosCount -= eventUC.VideosCount;
            PhotosCount -= eventUC.PhotosCount;

            m_eventUCs.Remove(eventUC);

            ShowInfor();

            m_currentDevice.Refresh();

            Mouse.OverrideCursor = Cursors.Arrow;
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
            DoImport(null, true);

            m_eventUCs.Clear();

            ShowInfor();
            m_currentDevice.Refresh();
        }

        private void DoImport(EventUC eventUC, bool all)
        {
            Mouse.OverrideCursor = Cursors.Wait;

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
                return;
            }

            try
            {
                string _url = "http://127.0.0.1:14005" + "/pending/sort";

                string _parms = "how" + "=" + HttpUtility.UrlEncode(_how);

                WebPostHelper _webPos = new WebPostHelper();
                bool _isOK = _webPos.doPost(_url, _parms, null);

                if (!_isOK)
                    return;

                _webPos.getContent();
            }
            catch
            {
            }

            Mouse.OverrideCursor = Cursors.Arrow;
        }
    }
}
