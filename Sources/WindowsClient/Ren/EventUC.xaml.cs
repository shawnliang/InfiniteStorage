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
using Waveface.Model;

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
		public bool m_doSelectAll;

		Point startPoint;

		public string DescribeText
		{
			set
			{
				tbDescribe.Text = value;
				m_defaultEventName = value;
			}
			get
			{
				string _dteText = (string)Application.Current.FindResource("DescribeTheEvent");

				if ((tbDescribe.Text == string.Empty) || (tbDescribe.Text == _dteText))
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

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ChangeSize();
		}

		public void SetUI()
		{
			bool _smallFileExists;

			GetCounts();

			SetInfor();

			List<EventPhoto> _controls = new List<EventPhoto>();

			foreach (FileChange _file in Event)
			{
				_smallFileExists = false;

				if (_file.type == 0)
				{
					string _path = _file.tiny_path.Replace("tiny", "small");

					if (File.Exists(_path))
					{
						_smallFileExists = true;
					}
					else
					{
						_path = _file.tiny_path;
					}

					if (_smallFileExists || File.Exists(_path))
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

			lbEvent.Items.Clear();
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

		#region gridMain

		private void gridMain_MouseEnter(object sender, MouseEventArgs e)
		{
			spToolBar.Visibility = Visibility.Visible;

			tbDescribe.BorderThickness = new Thickness(1);
			tbDescribe.BorderBrush = new SolidColorBrush(Color.FromRgb(64, 64, 64)); //404040
			tbDescribe.Background = new SolidColorBrush(Color.FromRgb(74, 74, 74));

			gridMain.Background = new SolidColorBrush(Color.FromRgb(79, 79, 79)); //4F4F4F
			lbEvent.Background = new SolidColorBrush(Color.FromRgb(79, 79, 79)); //4F4F4F
		}

		private void gridMain_MouseLeave(object sender, MouseEventArgs e)
		{
			spToolBar.Visibility = Visibility.Collapsed;

			tbDescribe.BorderThickness = new Thickness(0);
			tbDescribe.Background = new SolidColorBrush(Color.FromRgb(63, 63, 63)); //3F3F3F

			gridMain.Background = new SolidColorBrush(Color.FromRgb(63, 63, 63)); //3F3F3F
			lbEvent.Background = new SolidColorBrush(Color.FromRgb(63, 63, 63)); //3F3F3F
		}

		private void gridMain_MouseDown(object sender, MouseButtonEventArgs e)
		{
			UnSortedFilesUC.Current.Sub_SelectAll(false);
		}

		#endregion

		#region tbDescribe

		private void tbDescribe_MouseEnter(object sender, MouseEventArgs e)
		{
			tbDescribe.Background = new SolidColorBrush(Color.FromRgb(110, 109, 109)); //6E6D6D
			tbDescribe.BorderBrush = new SolidColorBrush(Color.FromRgb(64, 64, 64));
		}

		private void tbDescribe_MouseLeave(object sender, MouseEventArgs e)
		{
			tbDescribe.Background = new SolidColorBrush(Color.FromRgb(74, 74, 74));
		}

		private void tbDescribe_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox _textbox = sender as TextBox;
			string _invalid = new string(Path.GetInvalidFileNameChars());
			Regex _rex = new Regex("[" + Regex.Escape(_invalid) + "]");
			_textbox.Text = _rex.Replace(_textbox.Text, "");

			_textbox.CaretIndex = _textbox.Text.Length;
		}

		#endregion

		#region lbEvent

		private void lbEvent_Loaded(object sender, RoutedEventArgs e)
		{
			ChangeSize();
		}

		private void lbEvent_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			Point _clickPoint = e.GetPosition(lbEvent);

			object _htElement = lbEvent.InputHitTest(_clickPoint);

			if (_htElement != null)
			{
				ListBoxItem _clickedListBoxItem = GetVisualParent<ListBoxItem>(_htElement, 6);

				if (_clickedListBoxItem != null)
				{
					if (_clickedListBoxItem.IsSelected)
					{
					}
				}
			}
		}

		private void lbEvent_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		private void lbEvent_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			startPoint = e.GetPosition(null);

			/*
			if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
			{
				UnSortedFilesUC.Current.Sub_SelectAll(false);
			}
			*/
		}

		private void lbEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!m_doSelectAll)
			{
				UnSortedFilesUC.Current.Sub_SelectionChanged();
			}
		}

		private void lbEvent_MouseMove(object sender, MouseEventArgs e)
		{
			// Get the current mouse position
			Point _mousePos = e.GetPosition(null);
			Vector _diff = startPoint - _mousePos;

			if (e.LeftButton == MouseButtonState.Pressed &&
				(Math.Abs(_diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
				Math.Abs(_diff.Y) > SystemParameters.MinimumVerticalDragDistance))
			{
				Point _clickPoint = e.GetPosition(lbEvent);

				object _htElement = lbEvent.InputHitTest(_clickPoint);

				if (_htElement != null)
				{
					ListBoxItem _clickedListBoxItem = GetVisualParent<ListBoxItem>(_htElement, 6);

					if (_clickedListBoxItem != null)
					{
						_clickedListBoxItem.IsSelected = true;
					}
				}

				List<ContentEntity> _contents = UnSortedFilesUC.Current.Sub_GetAllSelectedFiles_ContentEntitys(true);

				DataObject _dragData = new DataObject(typeof(IEnumerable<IContentEntity>), _contents);
				DragDrop.DoDragDrop(this, _dragData, DragDropEffects.Move);
			}
		}

		#endregion

		public void SelectAll(bool flag)
		{
			m_doSelectAll = true;

			for (int i = 0; i < lbEvent.Items.Count; i++)
			{
				ListBoxItem _lbi = lbEvent.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
				_lbi.IsSelected = flag;
			}

			m_doSelectAll = false;

			UnSortedFilesUC.Current.Sub_SelectionChanged();
		}

		public List<FileChange> GetSelectedFiles()
		{
			List<FileChange> _fileChanges = new List<FileChange>();

			for (int i = 0; i < lbEvent.Items.Count; i++)
			{
				ListBoxItem _lbi = lbEvent.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;

				if (_lbi.IsSelected)
				{
					_fileChanges.Add(Event[i]);
				}
			}

			return _fileChanges;
		}

		#region Misc

		public T GetVisualParent<T>(object childObject, int level) where T : Visual
		{
			DependencyObject _child = childObject as DependencyObject;

			int k = 0;

			while ((_child != null) && !(_child is T))
			{
				_child = VisualTreeHelper.GetParent(_child);

				if (++k == level)
					break;
			}

			return _child as T;
		}

		#endregion

		private void btnSelectAll_Click(object sender, RoutedEventArgs e)
		{
			SelectAll(true);
		}

		private void btnUnselectAll_Click(object sender, RoutedEventArgs e)
		{
			SelectAll(false);
		}
	}
}