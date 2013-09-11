#region

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#endregion

namespace Waveface.Client
{
	public partial class P_ItemUC : UserControl, INotifyPropertyChanged
	{
		public double m_myWidth;
		public double m_myHeight;
		public double m_mySquare;
		private int m_oldFileEntrysCount;
		private EventEntry m_item = new EventEntry();
		private bool m_updateUI;
		private int m_oldIndex;
		private int m_delayMouseMoveCount;
		private int DELAY_MOVE = 1;
		private bool m_isSelected;
		private bool m_mouseEnter;

		#region Property

		public int VideosCount { get; set; }
		public int PhotosCount { get; set; }
		public bool Changed { get; set; }
		public PhotoDiaryUC PhotoDiaryUC { get; set; }

		public double MyWidth
		{
			get { return m_myWidth; }
			set
			{
				m_myWidth = value;
				OnPropertyChanged("MyWidth");
			}
		}

		public double MyHeight
		{
			get { return m_myHeight; }
			set
			{
				m_myHeight = value;
				OnPropertyChanged("MyHeight");
			}
		}

		public double MySquare
		{
			get { return m_mySquare; }
			set
			{
				m_mySquare = value;
				OnPropertyChanged("MySquare");
			}
		}

		public EventEntry Item
		{
			get { return m_item; }
			set
			{
				m_oldFileEntrysCount = m_item.Files.Count;

				m_item = value;

				m_item.Files.Sort((ev1, ev2) => ev2.taken_time.CompareTo(ev1.taken_time));

				Changed = m_oldFileEntrysCount != m_item.Files.Count;
			}
		}

		public bool IsSelected
		{
			get { return m_isSelected; }
			set
			{
				bool _changed = (m_isSelected != value);

				m_isSelected = value;

				if (_changed)
				{
					RefreshUI();
				}
			}
		}

		#endregion

		public P_ItemUC()
		{
			InitializeComponent();
		}

		public void SetUI(double myWidth, double myHeight, bool forceShow)
		{
			if (forceShow)
			{
				Visibility = Visibility.Visible;
			}
			else
			{
				Visibility = Visibility.Visible;

				foreach (FileEntry _fileEntry in Item.Files)
				{
					if (!_fileEntry.has_origin)
					{
						Visibility = Visibility.Collapsed;
						break;
					}
				}
			}

			m_updateUI = true;

			MyWidth = myWidth;
			MyHeight = myHeight;
			MySquare = MyWidth - 20;

			GetCounts();

			RefreshUI();

			SetImage(GetCoverIndex());

			m_updateUI = false;
		}

		private string GetFlatString(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return "";
			}

			string _ret = s;

			_ret = _ret.Replace("\r\n", " ");
			_ret = _ret.Replace("\r", " ");

			return _ret;
		}

		private int GetCoverIndex()
		{
			string _coverID = Item.Event.cover.ToString();

			for (int i = 0; i < Item.Files.Count; i++)
			{
				if (Item.Files[i].id == _coverID)
				{
					return i;
				}
			}

			return 0;
		}

		private void SetImage(int index)
		{
			m_oldIndex = index;

			if (Item.Files.Count == 0)
			{
				return;
			}

			FileEntry _fileEntry = Item.Files[index];

			rectMask.Visibility = Visibility.Collapsed;

			switch (_fileEntry.type)
			{
				case 0:
					{
						string _sPath = _fileEntry.tiny_path.Replace(".tiny.", ".medium.");

						if (!File.Exists(_sPath))
						{
							_sPath = _fileEntry.tiny_path.Replace(".tiny.", ".small.");

							if (!File.Exists(_sPath))
							{
								_sPath = _fileEntry.tiny_path;

								if (!File.Exists(_sPath))
								{
									_sPath = _fileEntry.s92_path;
								}
							}

							rectMask.Visibility = Visibility.Visible;
						}

						BitmapImage _bi = new BitmapImage();
						_bi.BeginInit();
						_bi.UriSource = new Uri(_sPath, UriKind.Absolute);
						_bi.EndInit();

						image.Source = _bi;
					}

					break;
				case 1:
					{
						BitmapImage _vidoeThumb = new BitmapImage();
						_vidoeThumb.BeginInit();

						if (File.Exists(_fileEntry.s92_path))
						{
							_vidoeThumb.UriSource = new Uri(_fileEntry.s92_path, UriKind.Absolute);

							rectMask.Visibility = Visibility.Visible;
						}
						else if (File.Exists(_fileEntry.tiny_path))
						{
							_vidoeThumb.UriSource = new Uri(_fileEntry.tiny_path, UriKind.Absolute);
						}
						else
						{
							_vidoeThumb.UriSource = new Uri("pack://application:,,,/Ren/Images/video_130x110.png");
						}

						_vidoeThumb.EndInit();

						image.Source = _vidoeThumb;
					}

					break;
			}
		}

		public void GetCounts()
		{
			VideosCount = 0;
			PhotosCount = 0;

			foreach (FileEntry _file in Item.Files)
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

		private void RefreshUI()
		{
			if (string.IsNullOrEmpty(Item.Event.content))
			{
				tbTitle.Text = Item.Event.start.ToString("M/d, yyyy");
			}
			else
			{
				tbTitle.Text = GetFlatString(Item.Event.content);
			}

			tbCount.Text = Item.Files.Count + "個項目";
			tbTime.Text = PrettyDate(Item.Event.start.ToString("yyyy/MM/dd HH:mm"), false);
			tbLocation.Text = Item.Event.short_address;
			tbDevice.Text = Item.DeviceName;

			if (m_mouseEnter || m_isSelected)
			{
				tbTime.Visibility = Visibility.Visible;
				tbDevice.Visibility = Visibility.Visible;
				imgClock.Visibility = Visibility.Visible;

				if (!string.IsNullOrEmpty(Item.Event.short_address))
				{
					tbLocation.Visibility = Visibility.Visible;
					imgLocation.Visibility = Visibility.Visible;
				}

				tbTime.Foreground = (Brush)FindResource("Brush676767");
				tbLocation.Foreground = (Brush)FindResource("Brush676767");
				tbTitle.Foreground = Brushes.Black;
				tbCount.Foreground = (Brush)FindResource("Brush808080");
				tbDevice.Foreground = (Brush)FindResource("Brush808080");

				if (string.IsNullOrEmpty(Item.Event.content))
				{
					tbTitle.Text = "未命名的事件";
				}
			}
			else
			{
				tbTime.Visibility = Visibility.Collapsed;
				tbLocation.Visibility = Visibility.Collapsed;
				tbDevice.Visibility = Visibility.Collapsed;
				imgClock.Visibility = Visibility.Collapsed;
				imgLocation.Visibility = Visibility.Collapsed;

				tbTitle.Foreground = (Brush)FindResource("Brush1E1E1E");
				tbCount.Foreground = (Brush)FindResource("Brush949494");
			}
		}

		private void image_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (!m_updateUI)
			{
				if (((++m_delayMouseMoveCount) % DELAY_MOVE) == 0)
				{
					double _dw = Item.Files.Count / imageArea.ActualWidth;
					int _index = (int)(e.GetPosition(imageArea).X * _dw);

					if (_index < 0)
					{
						_index = 0;
					}

					if (_index >= Item.Files.Count)
					{
						_index = Item.Files.Count - 1;
					}

					tbTime.Text = Item.Files[_index].taken_time.ToString("yyyy/M/d HH:mm");

					if (_index != m_oldIndex)
					{
						SetImage(_index);
					}
				}
			}
		}

		private void image_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			image.Stretch = Stretch.Uniform;
		}

		private void image_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			image.Stretch = Stretch.UniformToFill;

			SetImage(GetCoverIndex());

			tbTime.Text = PrettyDate(Item.Event.start.ToString("yyyy/M/d HH:mm"), false);
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion

		private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			string _s;

			if (string.IsNullOrEmpty(Item.Event.content))
			{
				_s = Item.Event.start.ToString("M/d, yyyy");
			}
			else
			{
				_s = GetFlatString(Item.Event.content);
			}

			PhotoDiaryUC.ToPhotoDiary2ndLevel(Item.Files, _s);
		}

		public string PrettyDate(String timeSubmitted, bool shortFormat)
		{
			// accepts standard DateTime: 5/12/2011 2:36:00 PM 
			// returns: "# month(s)/week(s)/day(s)/hour(s)/minute(s)/second(s)) ago"
			string _ret;

			DateTime submittedDate = DateTime.Parse(timeSubmitted);
			DateTime _now = DateTime.Now;
			TimeSpan _diff = _now - submittedDate;

			if (shortFormat)
			{
				timeSubmitted = submittedDate.ToString("yyyy-MM-dd");
			}

			switch (CultureInfo.CurrentCulture.Name)
			{
				case "zh-TW":
					{
						if (_diff.Seconds <= 0)
						{
							_ret = timeSubmitted;
						}
						else if (_diff.Days > 30)
						{
							_ret = _diff.Days / 30 + " 個月前";
						}
						else if (_diff.Days > 7)
						{
							_ret = _diff.Days / 7 + " 星期前";
						}
						else if (_diff.Days >= 1)
						{
							if (_diff.Days < 7)
								_ret = _diff.Days + "天前";
							else
								_ret = timeSubmitted;
						}
						else if (_diff.Hours >= 1)
						{
							_ret = _diff.Hours + "小時前";
						}
						else if (_diff.Minutes >= 1)
						{
							_ret = _diff.Minutes + "分鐘前";
						}
						else
						{
							_ret = _diff.Seconds + "秒前";
						}
					}

					break;

				default:
					{
						if (_diff.Seconds <= 0)
						{
							_ret = timeSubmitted;
						}
						else if (_diff.Days > 30)
						{
							_ret = _diff.Days / 30 + " month" + (_diff.Days / 30 >= 2 ? "s " : " ") + "ago";
						}
						else if (_diff.Days > 7)
						{
							_ret = _diff.Days / 7 + " week" + (_diff.Days / 7 >= 2 ? "s " : " ") + "ago";
						}
						else if (_diff.Days >= 1)
						{
							if (_diff.Days < 7)
								_ret = _diff.Days + " day" + (_diff.Days >= 2 ? "s " : " ") + "ago";
							else
								_ret = timeSubmitted;
						}
						else if (_diff.Hours >= 1)
						{
							_ret = _diff.Hours + " hour" + (_diff.Hours >= 2 ? "s " : " ") + "ago";
						}
						else if (_diff.Minutes >= 1)
						{
							_ret = _diff.Minutes + " minute" + (_diff.Minutes >= 2 ? "s " : " ") + "ago";
						}
						else
						{
							_ret = _diff.Seconds + " second" + (_diff.Seconds >= 2 ? "s " : " ") + "ago";
						}
					}

					break;
			}

			return _ret;
		}

		private void gridMain_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			m_mouseEnter = true;

			RefreshUI();
		}

		private void gridMain_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			m_mouseEnter = false;

			RefreshUI();
		}
	}
}