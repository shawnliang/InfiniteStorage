#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Web;
using System.Windows.Input;
using Newtonsoft.Json;
using Wpf_getRest;

#endregion

namespace Waveface
{
    class SliderTick
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    public partial class MainWindow : Window
    {
        public static MainWindow Current { get; set; }

        private localService m_locService;
        private bool m_init;
        private List<SliderTick> m_sliderTicks = new List<SliderTick>();
        private List<string> m_defaultEventNameCache;

        public int VideosCount { get; set; }
        public int PhotosCount { get; set; }
        public RT Rt { get; set; }

        public MainWindow()
        {
            Current = this;

            InitializeComponent();

            InitSliderTicks();

            Init();
        }

        private void InitSliderTicks()
        {
            m_sliderTicks.Add(new SliderTick { Name = "1 Minute", Value = 1 });
            m_sliderTicks.Add(new SliderTick { Name = "5 Minutes", Value = 5 });
            m_sliderTicks.Add(new SliderTick { Name = "10 Minutes", Value = 10 });
            m_sliderTicks.Add(new SliderTick { Name = "15 Minutes", Value = 15 });
            m_sliderTicks.Add(new SliderTick { Name = "20 Minutes", Value = 20 });
            m_sliderTicks.Add(new SliderTick { Name = "30 Minutes", Value = 30 });
            m_sliderTicks.Add(new SliderTick { Name = "1 Hour", Value = 1 * 60 });
            m_sliderTicks.Add(new SliderTick { Name = "2 Hours", Value = 2 * 60 });
            m_sliderTicks.Add(new SliderTick { Name = "4 Hours", Value = 4 * 60 });
            m_sliderTicks.Add(new SliderTick { Name = "5 Hours", Value = 5 * 60 });
            m_sliderTicks.Add(new SliderTick { Name = "8 Hours", Value = 8 * 60 });
            m_sliderTicks.Add(new SliderTick { Name = "12 Hours", Value = 12 * 60 });
            m_sliderTicks.Add(new SliderTick { Name = "1 Day", Value = 24 * 60 });
            m_sliderTicks.Add(new SliderTick { Name = "2 Days", Value = 2 * 24 * 60 });
            m_sliderTicks.Add(new SliderTick { Name = "3 Days", Value = 3 * 24 * 60 });
            m_sliderTicks.Add(new SliderTick { Name = "5 Days", Value = 5 * 24 * 60 });
            m_sliderTicks.Add(new SliderTick { Name = "1 Week", Value = 7 * 24 * 60 });
            m_sliderTicks.Add(new SliderTick { Name = "10 Days", Value = 10 * 24 * 60 });
            m_sliderTicks.Add(new SliderTick { Name = "2 Weeks", Value = 2 * 7 * 24 * 60 });
            m_sliderTicks.Add(new SliderTick { Name = "1 Month", Value = 30 * 24 * 60 });
        }

        private void Init()
        {
            string _deviceID = App.Args[0];

            m_locService = new localService();
            string _allPendingFiles = m_locService.initial(_deviceID, 10);

            if (_allPendingFiles == "")
            {
                Application.Current.Shutdown();
                return;
            }
            else
            {
                Rt = new RT();

                if (!Rt.Init(_allPendingFiles))
                {
                    Application.Current.Shutdown();
                    return;
                }
            }

            ShowEvents();

            m_init = true;

            ShowTitle();
        }

        private void ShowTitle()
        {
            tbTitle.Text = Rt.RtData.file_changes[0].dev_name;

            if (Rt.StartDate != Rt.EndDate)
            {
                tbTotalInterval.Text = Rt.StartDate.ToShortDateString() + " - " + Rt.EndDate.ToShortDateString();
            }
            else
            {
                tbTotalInterval.Text = Rt.StartDate.ToShortDateString();
            }
        }

        private void ShowInfor()
        {
            tbTotalCount.Text = GetCountsString(PhotosCount, VideosCount) + " in " + Rt.Events.Count + " event" + ((Rt.Events.Count > 1) ? "s" : "");
        }

        private void ShowEvents()
        {
            m_defaultEventNameCache = new List<string>();

            PhotosCount = 0;
            VideosCount = 0;

            ObservableCollection<EventUC> _controls = new ObservableCollection<EventUC>();

            Rt.GroupingByEvent(m_sliderTicks.ToArray()[(int)sliderEvent.Value].Value);

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

                _controls.Add(_ctl);
            }

            listBoxEvent.ItemsSource = _controls;

            ShowInfor();
        }

        private void rbByEvent_MouseEnter(object sender, MouseEventArgs e)
        {
            tbSplitByHint.Visibility = Visibility.Visible;
            tbSplitByHint.Text = "Split events by date token. Pictures without date token will be placed into the Month folder.";
        }

        private void rbByEvent_MouseLeave(object sender, MouseEventArgs e)
        {
            tbSplitByHint.Visibility = Visibility.Collapsed;
            tbSplitByHint.Text = "";
        }

        private void rbByMonth_MouseEnter(object sender, MouseEventArgs e)
        {
            tbSplitByHint.Visibility = Visibility.Visible;
            tbSplitByHint.Text = "";
        }

        private void rbByMonth_MouseLeave(object sender, MouseEventArgs e)
        {
            tbSplitByHint.Visibility = Visibility.Collapsed;
            tbSplitByHint.Text = "";
        }

        private void rbByType_Checked(object sender, RoutedEventArgs e)
        {
            if ((rbByMonth == null) || (rbByEvent == null))
            {
                return;
            }

            if (rbByMonth.IsChecked == true)
            {
                listBoxEvent.Visibility = Visibility.Collapsed;
                listBoxMonth.Visibility = Visibility.Visible;
            }
            else
            {
                listBoxEvent.Visibility = Visibility.Visible;
                listBoxMonth.Visibility = Visibility.Collapsed;

                ShowEvents();
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
                    _c = _c + ",";
                }

                _c = _c + videosCount + ((videosCount == 1) ? " video" : " videos");
            }

            return _c;
        }

        private void sliderEvent_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowSliderHint();
        }

        private void sliderEvent_MouseLeave(object sender, MouseEventArgs e)
        {
            tbSliderHint.Visibility = Visibility.Collapsed;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_init)
            {
                ShowEvents();
                ShowSliderHint();
            }
        }

        private void ShowSliderHint()
        {
            tbSliderHint.Visibility = Visibility.Visible;

            tbSliderHint.Text = "By " + m_sliderTicks.ToArray()[(int)sliderEvent.Value].Name; //sliderEvent.Value
        }

        #region Default Event Name

        private string GetDefaultEventName(DateTime dt)
        {
            string _s = string.Empty;

            string _date = dt.ToString("yyyy-MM-dd");
            string _period = GetTimePeriod(dt.Hour);

            _s = _date + " " + _period;

            if (!m_defaultEventNameCache.Contains(_s))
            {
                m_defaultEventNameCache.Add(_s);
                return _s;
            }

            _s = _date + " " + dt.ToString("hh") + " " + (dt.Hour > 12 ? "PM":"AM");

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

        private string GetTimePeriod(int hour)
        {
            if (hour > 18)
                return "Evening";

            if (hour > 12)
                return "Afternoon";

            if (hour > 3)
                return "Morning";

            return "Night";
        }

        #endregion

        private void btnFinishLater_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            PendingSort _pendingSort = new PendingSort();

            _pendingSort.device_id = Rt.RtData.file_changes[0].dev_id;

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

                string _r = _webPos.getContent();

                Application.Current.Shutdown();
            }
            catch
            {
            }
        }
    }
}