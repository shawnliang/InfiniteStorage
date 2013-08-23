#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Waveface.Model;

#endregion

namespace Waveface.Client
{
	public class EventItem : FrameworkElement, INotifyPropertyChanged
	{
		private bool m_hasOrigin;

		public BitmapSource BitmapImage { get; set; }
		public bool IsVideo { get; set; }
		public bool IsPhoto { get; set; }

		public bool IsMore { get; set; }
		public bool IsLess { get; set; }
		public string MoreText { get; set; }

		public BitmapSource MediaSource { get; set; }
		public string FileID { get; set; }

		public bool HasOrigin
		{
			get { return m_hasOrigin; }
			set
			{
				m_hasOrigin = value;
				OnPropertyChanged("HasOrigin");
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
	}

	public partial class EventUC : UserControl
	{
		public string YM { get; set; }
		public int VideosCount { get; set; }
		public int PhotosCount { get; set; }
		public bool Changed { get; set; }
		public MainWindow MyMainWindow{ get; set; }
		public IService CurrentDevice{ get; set; }

		public List<FileEntry> FileEntrys
		{
			get { return m_fileEntrys; }
			set
			{
				m_oldFileEntrysCount = m_fileEntrys.Count;

				m_fileEntrys = value;

				Changed = m_oldFileEntrysCount != m_fileEntrys.Count;
			}
		}

		private int m_oldFileEntrysCount;
		private List<FileEntry> m_fileEntrys = new List<FileEntry>();
		private const int More = 100;
		private bool m_showMoreButton;
		private List<EventItem> m_eventItems;
		private int m_hasOriginCount;

		public EventUC()
		{
			InitializeComponent();

			m_showMoreButton = true;
		}

		public void SetUI()
		{
			GetCounts();

			SetInfor();

			FileEntrys.Reverse();

			List<EventItem> _ctlItems = new List<EventItem>();

			// 若改變項目個數, 則重新產生UI
			if (Changed)
			{
				int _idx = 0;

				foreach (FileEntry _file in FileEntrys)
				{
					if (_idx == More)
					{
						EventItem _eventItem = new EventItem
												   {
													   IsMore = true,
													   MoreText = FileEntrys.Count - 100 + " More"
												   };

						_ctlItems.Add(_eventItem);
					}

					switch (_file.type)
					{
						case 0:
							{
								string _path = _file.s92_path;

								if (!File.Exists(_path))
								{
									_path = _file.tiny_path;
								}

								if (_file.has_origin)
								{
									_path = _file.tiny_path;
								}

								EventItem _eventItem = new EventItem
														   {
															   FileID = _file.id,
															   IsVideo = false,
															   IsPhoto = true,
															   HasOrigin = !_file.has_origin
														   };

								try
								{
									BitmapImage _bi = new BitmapImage();
									_bi.BeginInit();
									_bi.UriSource = new Uri(_path, UriKind.Absolute);
									_bi.EndInit();

									_eventItem.BitmapImage = _bi;

									if (_idx == 0)
									{
										string _sPath = _file.tiny_path.Replace(".tiny.", ".small.");

										if (File.Exists(_sPath))
										{
											_bi = new BitmapImage();
											_bi.BeginInit();
											_bi.UriSource = new Uri(_sPath, UriKind.Absolute);
											_bi.EndInit();
										}

										imgHead.Source = _bi;
									}
								}
								catch
								{
								}

								_ctlItems.Add(_eventItem);
							}

							break;
						case 1:
							{
								EventItem _eventItem = new EventItem
														   {
															   FileID = _file.id,
															   IsVideo = true,
															   IsPhoto = false,
															   HasOrigin = !_file.has_origin
														   };

								try
								{
									BitmapImage _bi = new BitmapImage();
									_bi.BeginInit();
									_bi.UriSource = new Uri("pack://application:,,,/Resource/video_ph.png");
									_bi.EndInit();

									_eventItem.BitmapImage = _bi;
								}
								catch
								{
								}

								BitmapImage _vidoeThumb = new BitmapImage();
								_vidoeThumb.BeginInit();

								if (File.Exists(_file.s92_path))
								{
									_vidoeThumb.UriSource = new Uri(_file.s92_path, UriKind.Absolute);
								}
								else if (File.Exists(_file.tiny_path))
								{
									_vidoeThumb.UriSource = new Uri(_file.tiny_path, UriKind.Absolute);
								}
								else
								{
									_vidoeThumb.UriSource = new Uri("pack://application:,,,/Ren/Images/video_130x110.png");
								}

								_vidoeThumb.EndInit();

								_eventItem.MediaSource = _vidoeThumb;

								if (_idx == 0)
								{
									imgHead.Source = _vidoeThumb;
								}

								_ctlItems.Add(_eventItem);
							}

							break;
					}

					_idx++;
				}
			}
			else // 若沒改變項目個數, 則只重設HasOrigin欄位
			{
				Dictionary<string, FileEntry> _id_FileEntrys = new Dictionary<string, FileEntry>();

				foreach (FileEntry _entry in FileEntrys)
				{
					_id_FileEntrys.Add(_entry.id, _entry);
				}

				foreach (EventItem _item in m_eventItems)
				{
					if (!string.IsNullOrEmpty(_item.FileID)) //排除有可能More跟Less
						_item.HasOrigin = !_id_FileEntrys[_item.FileID].has_origin;
				}

				return;
			}

			//若少於More項
			if (_ctlItems.Count < More)
			{
				m_eventItems = _ctlItems;
				lbEvent.ItemsSource = m_eventItems;
			}
			else
			{
				_ctlItems.Add(new EventItem { IsLess = true });
				m_eventItems = _ctlItems;
				UpdateShowMoreUI();
			}
		}

		private void UpdateShowMoreUI()
		{
			if (m_showMoreButton)
			{
				List<EventItem> _temp = new List<EventItem>();

				for (int i = 0; i <= More; i++)
				{
					_temp.Add(m_eventItems[i]);
				}

				lbEvent.ItemsSource = _temp;
			}
			else
			{
				List<EventItem> _temp = new List<EventItem>();

				for (int i = 0; i < m_eventItems.Count; i++)
				{
					if (i != More)
						_temp.Add(m_eventItems[i]);
				}

				lbEvent.ItemsSource = _temp;
			}
		}

		private void ListBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			EventItem _eventItem = (EventItem)((ListBoxItem)sender).Content;

			if (_eventItem != null)
			{
				if (_eventItem.IsMore || _eventItem.IsLess)
				{
					m_showMoreButton = !m_showMoreButton;

					UpdateShowMoreUI();
				}
			}
		}

		private void SetInfor()
		{
			tbTitleMonth.Text = FileEntrys[0].taken_time.ToString("MMM");
			tbTitleYear.Text = FileEntrys[0].taken_time.ToString("yyyy");

			tbTimeAgo.Visibility = Visibility.Collapsed;

			tbTotalCount.Text = SourceAllFilesUC.GetCountsString(PhotosCount, VideosCount);

			if (m_hasOriginCount == 0)
			{
				rectMonthAreaAll.Visibility = Visibility.Visible;
				rectMonthAreaImage.Visibility = Visibility.Collapsed;
			}
			else
			{
				rectMonthAreaAll.Visibility = Visibility.Collapsed;
				rectMonthAreaImage.Visibility = Visibility.Visible;
			}
		}

		public void GetCounts()
		{
			VideosCount = 0;
			PhotosCount = 0;
			m_hasOriginCount = 0;

			foreach (FileEntry _file in FileEntrys)
			{
				if (_file.type == 0)
				{
					PhotosCount++;
				}
				else
				{
					VideosCount++;
				}

				if (_file.has_origin)
				{
					m_hasOriginCount++;
				}
			}
		}

		private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			//已經有照片
			if (m_hasOriginCount != 0)
			{
				MyMainWindow.JumpToDevice(CurrentDevice.ID, false, YM);
			}
		}
	}
}