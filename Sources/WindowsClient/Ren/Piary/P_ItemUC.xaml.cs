#region

using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

		private SolidColorBrush m_brush676767;
		private SolidColorBrush m_brush808080;
		private SolidColorBrush m_brush1E1E1E;
		private SolidColorBrush m_brush949494;
		private bool m_openContextMenu;

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

		public bool IsAllImagesLoadOK { get; set; }

		#endregion

		public P_ItemUC()
		{
			InitializeComponent();

			m_brush676767 = (SolidColorBrush)FindResource("Brush676767");
			m_brush808080 = (SolidColorBrush)FindResource("Brush808080");
			m_brush1E1E1E = (SolidColorBrush)FindResource("Brush1E1E1E");
			m_brush949494 = (SolidColorBrush)FindResource("Brush949494");

			IsAllImagesLoadOK = true;
		}

		public void SetUI(double myWidth, double myHeight)
		{
			m_updateUI = true;

			IsAllImagesLoadOK = true;

			foreach (FileEntry _fileEntry in Item.Files)
			{
				if (!_fileEntry.has_origin)
				{
					IsAllImagesLoadOK = false;
					break;
				}
			}

			if (Item.Files.Count == 0)
			{
				IsAllImagesLoadOK = false;
			}

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
			_ret = _ret.Replace("\n", " ");

			return _ret.Trim();
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

			if (IsAllImagesLoadOK)
			{
				rectMask.Visibility = Visibility.Collapsed;
				tbWaiting.Visibility = Visibility.Collapsed;
			}
			else
			{
				rectMask.Visibility = Visibility.Visible;
				tbWaiting.Visibility = Visibility.Visible;
			}

			if (Item.Files.Count == 0)
			{
				return;
			}

			FileEntry _fileEntry = Item.Files[index];

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
				tbTitle.Text = Item.Event.start.ToString("MMMM dd, yyyy");
			}
			else
			{
				tbTitle.Text = GetFlatString(Item.Event.content);
			}

			tbCount.Text = Item.Files.Count + "個項目";
			tbTime.Text = PrettyDate(Item.Event.start);
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

				tbTime.Foreground = m_brush676767;
				tbLocation.Foreground = m_brush676767;
				tbTitle.Foreground = Brushes.Black;
				tbCount.Foreground = m_brush808080;
				tbDevice.Foreground = m_brush808080;

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

				tbTitle.Foreground = m_brush1E1E1E;
				tbCount.Foreground = m_brush949494;
			}
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

		private void ToPhotoDiary2ndLevel()
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

		public string PrettyDate(DateTime dateTime)
		{
			DateTime _now = DateTime.Now;
			TimeSpan _diff = _now - dateTime;

			string _ret = dateTime.ToString("MMM dd, yyyy H:mm");

			if (_diff.Days == 0)
			{
				string _today = FindResource("0_days_ago") as string;

				_ret = _today + " " + dateTime.ToString("h:mm tt");
			}

			else if (_diff.Days == 1)
			{
				string _yesterday = FindResource("res_27") as string;

				_ret = _yesterday + " " + dateTime.ToString("h:mm tt");
			}

			if ((_diff.Days > 1) && (_diff.Days < 4))
			{
				string _x_days_ago = FindResource("x_days_ago") as string;

				_ret = string.Format(_x_days_ago, _diff.Days) + " " + dateTime.ToString("h:mm tt");
			}

			return _ret;
		}

		private void MouseMove_ChangeImage(MouseEventArgs e)
		{
			if (m_updateUI)
				return;

			if (m_openContextMenu)
				return;

			if (((++m_delayMouseMoveCount) % DELAY_MOVE) == 0)
			{
				double _dw = Item.Files.Count / imageArea.ActualWidth;
				int _index = (int)(e.GetPosition(imageArea).X * _dw);

				if (_index >= Item.Files.Count)
				{
					_index = Item.Files.Count - 1;
				}

				if (_index < 0)
				{
					_index = 0;
				}

				tbTime.Text = PrettyDate(Item.Files[_index].taken_time);

				if (_index != m_oldIndex)
				{
					SetImage(_index);
				}
			}
		}

		#region mouse

		private void image_MouseEnter(object sender, MouseEventArgs e)
		{
			image.Stretch = Stretch.Uniform;
		}

		private void image_MouseLeave(object sender, MouseEventArgs e)
		{
			image.Stretch = Stretch.UniformToFill;
		}

		private void image_MouseMove(object sender, MouseEventArgs e)
		{
			MouseMove_ChangeImage(e);
		}

		private void gridMain_MouseEnter(object sender, MouseEventArgs e)
		{
			m_mouseEnter = true;

			RefreshUI();
		}

		private void gridMain_MouseLeave(object sender, MouseEventArgs e)
		{
			if (m_openContextMenu)
			{
				return;
			}

			m_mouseEnter = false;

			SetImage(GetCoverIndex());

			RefreshUI();
		}

		private void gridMain_MouseMove(object sender, MouseEventArgs e)
		{
			MouseMove_ChangeImage(e);
		}

		private void gridMain_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!IsAllImagesLoadOK)
			{
				return;
			}

			if (e.ClickCount == 2)
			{
				ToPhotoDiary2ndLevel();
			}
		}

		private void miToPhotoViewer_Click(object sender, RoutedEventArgs e)
		{
			if (!IsAllImagesLoadOK)
			{
				return;
			}

			PhotoDiaryUC.ToPhotoViewer(Item.Files, m_oldIndex);

			m_openContextMenu = false;
		}

		#endregion

		private void OpenContextMenu(FrameworkElement element)
		{
			m_openContextMenu = true;

			if (element.ContextMenu != null)
			{
				element.ContextMenu.PlacementTarget = element;
				element.ContextMenu.IsOpen = true;
			}
		}

		private void gridMain_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			OpenContextMenu(this);

			e.Handled = true;
		}
	}
}