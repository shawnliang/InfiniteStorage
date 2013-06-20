#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#endregion

namespace Waveface.Client
{
    public class EventPhoto
    {
        public BitmapImage BitmapImage { get; set; }
        public double MyWidth { get; set; }
        public double MyHeight { get; set; }
        public bool IsVideo { get; set; }
        public bool IsPhoto { get; set; }
        public BitmapImage MediaSource { get; set; }
    }

    public partial class EventUC : UserControl
    {
        private double WW = 150; //170
        private double HH = 150; //144

        public List<FileChange> Event { get; set; }
        public int VideosCount { get; set; }
        public int PhotosCount { get; set; }

        private string m_defaultEventName;

        public string DescribeText
        {
            set
            {
                tbDescribe.Text = value;
                m_defaultEventName = value;
            }
            get
            {
                if ((tbDescribe.Text == string.Empty) || (tbDescribe.Text == "Describe the event..."))
                {
                    return m_defaultEventName;
                }
                else
                {
                    return tbDescribe.Text;
                }
            }
        }

        public EventUC()
        {
            InitializeComponent();
        }

        public void SetUI()
        {
            GetCounts();

            SetInfor();

            List<EventPhoto> _controls = new List<EventPhoto>();

            foreach (var _file in Event)
            {
                if (_file.type == 0)
                {
                    string _path = _file.tiny_path.Replace("tiny", "small");

                    if (File.Exists(_path))
                    {
                        EventPhoto _eventPhoto = new EventPhoto
                                                     {
                                                         IsVideo = false,
                                                         IsPhoto = true
                                                     };

                        BitmapImage _bi = new BitmapImage();
                        _bi.BeginInit();
                        _bi.UriSource = new Uri(_path, UriKind.Absolute);
                        _bi.EndInit();

                        _eventPhoto.BitmapImage = _bi;

                        _controls.Add(_eventPhoto);
                    }
                }
                else
                {
                    EventPhoto _eventPhoto = new EventPhoto
                                                 {
                                                     IsVideo = true,
                                                     IsPhoto = false
                                                 };

                    BitmapImage _bi = new BitmapImage();
                    _bi.BeginInit();
                    _bi.UriSource = new Uri("pack://application:,,,/Resource/video_ph.png");
                    _bi.EndInit();

                    _eventPhoto.BitmapImage = _bi;


                    BitmapImage _vidoeThumb = new BitmapImage();
                    _vidoeThumb.BeginInit();

                    if (File.Exists(_file.tiny_path))
                        _vidoeThumb.UriSource = new Uri(_file.tiny_path, UriKind.Absolute);
                    else
                        _vidoeThumb.UriSource = new Uri("pack://application:,,,/Ren/Images/video_130x110.png");

                    _vidoeThumb.EndInit();

                    _eventPhoto.MediaSource = _vidoeThumb;

                    _controls.Add(_eventPhoto);
                }
            }

            lbEvent.ItemsSource = _controls;
        }

        private void SetInfor()
        {
            string _timeInterval = GetTimeDisplayString();

            tbTitle.Text = _timeInterval;

            tbTimeAgo.Visibility = Visibility.Collapsed;

            tbTotalCount.Text = UnSortedFilesUC.GetCountsString(PhotosCount, VideosCount);
        }

        private string GetTimeDisplayString()
        {
            string _timeInterval = string.Empty;
            DateTime _startDateTime = UnSortedFilesUC.Current.Rt.DateTimeCache[Event[0].taken_time];
            DateTime _endDateTime = UnSortedFilesUC.Current.Rt.DateTimeCache[Event[Event.Count - 1].taken_time];

            if (UnSortedFilesUC.Current.GroupingEventInterval < 24 * 60)
            {
                if (Event.Count == 1) //只有一筆事件
                {
                    _timeInterval = _startDateTime.ToString("yyyy/MM/dd dddd HH:mm");
                }
                else
                {
                    if (_startDateTime.ToString("yyyy/MM/dd") == _endDateTime.ToString("yyyy/MM/dd")) //同一天
                    {
                        _timeInterval = _startDateTime.ToString("yyyy/MM/dd dddd HH:mm") + " - " + _endDateTime.ToString("HH:mm");
                    }
                    else
                    {
                        _timeInterval = _startDateTime.ToString("yyyy/MM/dd dddd HH:mm") + " - " + _endDateTime.ToString("yyyy/MM/dd dddd HH:mm");
                    }
                }
            }

            if (UnSortedFilesUC.Current.GroupingEventInterval == UnSortedFilesUC.BY_DAY)
            {
                _timeInterval = _startDateTime.ToString("yyyy/MM/dd dddd");
            }

            if (UnSortedFilesUC.Current.GroupingEventInterval == UnSortedFilesUC.BY_WEEK)
            {
                DateTime _startOfWeek = UnSortedFilesUC.StartOfWeek(_startDateTime);
                DateTime _endOfWeek = _startOfWeek.AddDays(6);

                _timeInterval = _startOfWeek.ToString("yyyy/MM/dd") + " - " +
                                ((_startOfWeek.Year != _endOfWeek.Year) ? _endOfWeek.ToString("yyyy/MM/dd") : _endOfWeek.ToString("MM/dd"));
            }

            if (UnSortedFilesUC.Current.GroupingEventInterval == UnSortedFilesUC.BY_MONTH)
            {
                _timeInterval = _startDateTime.ToString("yyyy/MM");
            }

            return _timeInterval;
        }

        public void GetCounts()
        {
            VideosCount = 0;
            PhotosCount = 0;

            foreach (FileChange _file in Event)
            {
                if (_file.type == 0)
                {
                    PhotosCount++;
                }
                else
                {
                    VideosCount++;
                }
            }
        }

        private void lbEvent_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeSize();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChangeSize();
        }

        private void ChangeSize()
        {
            List<Size> _sizes = new List<Size>();

            for (int i = 0; i < lbEvent.Items.Count; i++)
            {
                ListBoxItem _lbi = lbEvent.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;

                Size _s = new Size
                                 {
                                     Height = HH
                                 };

                _lbi.Height = HH;

                if (Event[i].width == 0 || Event[i].height == 0)
                {
                    _lbi.Width = WW;
                }
                else
                {
                    _lbi.Width = _lbi.Height * (Event[i].width / (double)Event[i].height);
                }

                _s.Width = _lbi.Width;

                _sizes.Add(_s);
            }

            int _start = 0;
            int _e = 0;
            int _wc;
            int k = 0;
            double _wTotal = 0;
            double _rs;
            double _h;

            foreach (Size _s in _sizes)
            {
                if ((_wTotal + _s.Width) < (lbEvent.ActualWidth - 8.0))
                {
                    _wTotal += _s.Width;
                    _e++;
                }
                else
                {
                    _rs = 0;
                    _wc = 0;

                    for (int j = _start; j < k; j++)
                    {
                        _rs += _sizes[j].Width / _sizes[j].Height;

                        if (_sizes[j].Width > _sizes[j].Height)
                        {
                            _wc++;
                        }
                    }

                    _h = lbEvent.ActualWidth / _rs;

                    for (int j = _start; j < k; j++)
                    {
                        ListBoxItem _lbi = lbEvent.ItemContainerGenerator.ContainerFromIndex(j) as ListBoxItem;

                        _lbi.Height = _h;
                        _lbi.Width = _h * (_sizes[j].Width / _sizes[j].Height) - (8.0 / _e);

                        if (((_e - _wc) != 0) && (_wc != 0))
                        {
                            if (_sizes[j].Width > _sizes[j].Height)
                            {
                                _lbi.Width -= _h / _wc;
                            }
                            else
                            {
                                _lbi.Width += _h / (_e - _wc);
                            }
                        }
                    }

                    _start = k;
                    _wTotal = _s.Width;
                    _e = 1;
                }

                k++;
            }
        }

        #region Utility

        public string GetPrettyDate(DateTime date)
        {
            // 1. Get time span elapsed since the date.
            TimeSpan _s = DateTime.Now.Subtract(date);

            // 2. Get total number of days elapsed.
            Int32 _dayDiff = (Int32)_s.TotalDays;

            // 3. Get total number of seconds elapsed.
            Int32 _secDiff = (Int32)_s.TotalSeconds;

            // 4. Handle same-day times.
            if (_dayDiff == 0)
            {
                // A. Less than one minute ago.
                if (_secDiff < 60)
                {
                    return "just now";
                }

                // B. Less than 2 minutes ago.
                if (_secDiff < 120)
                {
                    return "1 minute ago";
                }

                // C.Less than one hour ago.
                if (_secDiff < 3600)
                {
                    return String.Format("{0} minutes ago", Math.Floor((double)_secDiff / 60));
                }

                // D. Less than 2 hours ago.
                if (_secDiff < 7200)
                {
                    return "1 hour ago";
                }

                // E. Less than one day ago.
                if (_secDiff < 86400)
                {
                    return String.Format("{0} hours ago", Math.Floor((double)_secDiff / 3600));
                }
            }

            // 6. Handle previous days.
            if (_dayDiff == 1)
            {
                return "yesterday";
            }

            if (_dayDiff == 2)
            {
                return "2 days ago";
            }

            if (_dayDiff < 7)
            {
                return String.Format("{0} days ago", _dayDiff);
            }

            if (_dayDiff < 14)
            {
                return "last week";
            }

            if (_dayDiff < 21)
            {
                return "2 weeks ago";
            }

            return "over 2 weeks ago";
        }

        #endregion

        private void tbDescribe_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox _textbox = sender as TextBox;
            string _invalid = new string(Path.GetInvalidFileNameChars());
            Regex _rex = new Regex("[" + Regex.Escape(_invalid) + "]");
            _textbox.Text = _rex.Replace(_textbox.Text, "");

            _textbox.CaretIndex = _textbox.Text.Length;
        }

        private void gridMain_MouseEnter(object sender, MouseEventArgs e)
        {
            tbDescribe.BorderThickness = new Thickness(1);
            tbDescribe.BorderBrush = new SolidColorBrush(Color.FromRgb(64, 64, 64)); //404040
            tbDescribe.Background = new SolidColorBrush(Color.FromRgb(74, 74, 74));

            gridMain.Background = new SolidColorBrush(Color.FromRgb(79, 79, 79)); //4F4F4F
            lbEvent.Background = new SolidColorBrush(Color.FromRgb(79, 79, 79)); //4F4F4F
        }

        private void gridMain_MouseLeave(object sender, MouseEventArgs e)
        {
            tbDescribe.BorderThickness = new Thickness(0);
            tbDescribe.Background = new SolidColorBrush(Color.FromRgb(63, 63, 63)); //3F3F3F

            gridMain.Background = new SolidColorBrush(Color.FromRgb(63, 63, 63)); //3F3F3F
            lbEvent.Background = new SolidColorBrush(Color.FromRgb(63, 63, 63)); //3F3F3F
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            UnSortedFilesUC.Current.AddEventToFolder(this);
        }

        private void tbDescribe_MouseEnter(object sender, MouseEventArgs e)
        {
            tbDescribe.Background = new SolidColorBrush(Color.FromRgb(110, 109, 109)); //6E6D6D
            tbDescribe.BorderBrush = new SolidColorBrush(Color.FromRgb(64, 64, 64));
        }

        private void tbDescribe_MouseLeave(object sender, MouseEventArgs e)
        {
            tbDescribe.Background = new SolidColorBrush(Color.FromRgb(74, 74, 74));
        }
    }
}