#region

using System;
using System.ComponentModel;
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
		private int m_currentIndex;
		private int m_oldFileEntrysCount;
		private EventEntry m_item = new EventEntry();
		private bool m_updateUI;
		private int m_oldIndex;
		private bool m_mouseLeave;
		private int m_delayMouseMoveCount;
		private int DELAY_MOVE = 1;

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

			m_currentIndex = GetCoverIndex();

			MyWidth = myWidth;
			MyHeight = myHeight;
			MySquare = MyWidth - 16;

			GetCounts();

			tbTitle.Text = Item.Event.content;
			tbTime.Text = Item.Event.start.ToString("yyyy/MM/dd HH:mm");
			tbLocation.Text = Item.Event.short_address;

			SetImage(m_currentIndex);

			m_updateUI = false;
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

			UpdateCountText();

			if(Item.Files.Count == 0)
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

		private void UpdateCountText()
		{
			if (m_mouseLeave)
			{
				tbCount.Text = "" + Item.Files.Count;
			}
			else
			{
				tbCount.Text = (m_oldIndex + 1) + "/" + Item.Files.Count;
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

					if (_index != m_oldIndex)
					{
						SetImage(_index);
					}
				}
			}
		}

		private void image_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			m_mouseLeave = false;

			image.Stretch = Stretch.Uniform;

			UpdateCountText();
		}

		private void image_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			m_mouseLeave = true;

			image.Stretch = Stretch.UniformToFill;

			UpdateCountText();
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
			//Todo:
			PhotoDiaryUC.ToPhotoDiary2ndLevel(Item.Files, Item.Event.content);
		}
	}
}