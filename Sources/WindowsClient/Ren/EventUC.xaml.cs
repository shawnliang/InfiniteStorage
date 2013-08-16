#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;
using Image = System.Drawing.Image;

#endregion

namespace Waveface.Client
{
	public class EventItem : FrameworkElement, INotifyPropertyChanged
	{
		private bool m_hasOrigin;

		public BitmapSource BitmapImage { get; set; }
		public bool IsVideo { get; set; }
		public bool IsPhoto { get; set; }
		public bool IsGIF { get; set; }

		public bool IsMore { get; set; }
		public bool IsLess { get; set; }

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

		public List<FileEntry> FileEntrys
		{
			get { return m_fileEntrys; }
			set
			{
				m_oldFileEntrysCount = m_fileEntrys.Count;

				m_fileEntrys = value;

				m_changed = m_oldFileEntrysCount != m_fileEntrys.Count;
			}
		}

		private int m_oldFileEntrysCount;
		private List<FileEntry> m_fileEntrys = new List<FileEntry>();
		private const int More = 100;
		private bool m_showMoreButton;
		private bool m_changed;
		private List<EventItem> m_eventItems;

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

			//試驗連張轉Gif
			//List<FileEntry> _FileEntrys = GenGif();
			List<FileEntry> _FileEntrys = FileEntrys;
			List<EventItem> _ctlItems = new List<EventItem>();

			// 若改變項目個數, 則重新產生UI
			if (m_changed)
			{
				int _idx = 0;

				foreach (FileEntry _file in _FileEntrys)
				{
					if (_idx == More)
					{
						EventItem _eventItem = new EventItem
												   {
													   IsMore = true
												   };

						_ctlItems.Add(_eventItem);
					}

					switch (_file.type)
					{
						case 0:
							{
								string _path = _file.tiny_path;


								EventItem _eventItem = new EventItem
														   {
															   FileID = _file.id,
															   IsVideo = false,
															   IsPhoto = true,
															   IsGIF = false,
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
															   IsGIF = false,
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

								if (File.Exists(_file.tiny_path))
									_vidoeThumb.UriSource = new Uri(_file.tiny_path, UriKind.Absolute);
								else
									_vidoeThumb.UriSource = new Uri("pack://application:,,,/Ren/Images/video_130x110.png");

								_vidoeThumb.EndInit();

								_eventItem.MediaSource = _vidoeThumb;

								if (_idx == 0)
								{
									imgHead.Source = _vidoeThumb;
								}

								_ctlItems.Add(_eventItem);
							}

							break;
						case 999:
							{
								string _path = _file.tiny_path;

								try
								{
									EventItem _eventItem = new EventItem
															   {
																   FileID = _file.id,
																   IsVideo = false,
																   IsPhoto = false,
																   IsGIF = true,
																   HasOrigin = false
															   };

									BitmapImage _bi = new BitmapImage();
									_bi.BeginInit();
									_bi.UriSource = new Uri(_path, UriKind.Absolute);
									_bi.EndInit();

									_eventItem.BitmapImage = _bi;

									_ctlItems.Add(_eventItem);
								}
								catch
								{
								}
							}

							break;
					}

					_idx++;
				}
			}
			else // 若沒改變項目個數, 則只重設HasOrigin欄位
			{
				Dictionary<string, FileEntry> _id_FileEntrys = new Dictionary<string, FileEntry>();

				foreach (FileEntry _entry in _FileEntrys)
				{
					_id_FileEntrys.Add(_entry.id, _entry);
				}

				foreach (EventItem _item in m_eventItems)
				{
					if (_item.FileID != string.Empty) //排除有可能More跟Less
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
				if (m_changed) //重新產生UI狀況, 加入Less
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

		private List<FileEntry> GenGif()
		{
			Dictionary<int, int> _temp = new Dictionary<int, int>();

			int _idx = 0;
			bool _preOK = false;

			for (int i = 0; i < FileEntrys.Count; i++)
			{
				if (i < FileEntrys.Count - 1)
				{
					if (Math.Abs(FileEntrys[i + 1].taken_time.Second - FileEntrys[i].taken_time.Second) < 8)
					{
						if (!_preOK && !_temp.Keys.Contains(_idx))
						{
							_temp.Add(_idx, 0);
						}

						_temp[_idx]++;

						_preOK = true;
					}
					else
					{
						_idx = i + 1;

						_preOK = false;
					}
				}
			}


			List<FileEntry> _ret = new List<FileEntry>();
			int _skip = 0;

			for (int i = 0; i < FileEntrys.Count; i++)
			{
				if (_temp.Keys.Contains(i) && _temp[i] > 2)
				{
					FileEntry _fileEntry = GetGifFile(i, _temp[i]);
					_ret.Add(_fileEntry);
					_skip = _temp[i];
				}

				if (_skip-- <= 0)
					_ret.Add(FileEntrys[i]);
			}

			return _ret;
		}

		private FileEntry GetGifFile(int idx, int length)
		{
			string _tempPathBase = Path.GetTempPath() + "Waveface Photos" + "\\";

			if (!Directory.Exists(_tempPathBase))
				Directory.CreateDirectory(_tempPathBase);

			string _gif = Path.Combine(_tempPathBase, Path.GetFileName(FileEntrys[idx].tiny_path)) + ".gif";

			if (!File.Exists(_gif))
			{
				List<string> imageFilePaths = new List<string>();

				for (int k = 0; k < length; k++)
				{
					imageFilePaths.Add(FileEntrys[idx + k].tiny_path);
				}

				AnimatedGifEncoder _gifEncoder = new AnimatedGifEncoder();
				_gifEncoder.Start(_gif);
				_gifEncoder.SetDelay(666);
				_gifEncoder.SetRepeat(0); //-1:no repeat,0:always repeat

				for (int i = 0; i < imageFilePaths.Count; i++)
				{
					_gifEncoder.AddFrame(Image.FromFile(imageFilePaths[i]));
				}

				_gifEncoder.Finish();
			}

			FileEntry _ret = (FileEntry)FileEntrys[idx].Clone();
			_ret.tiny_path = _gif;
			_ret.type = 999;

			return _ret;
		}

		private void SetInfor()
		{
			tbTitleMonth.Text = FileEntrys[0].taken_time.ToString("MMM");
			tbTitleYear.Text = FileEntrys[0].taken_time.ToString("yyyy");

			tbTimeAgo.Visibility = Visibility.Collapsed;

			tbTotalCount.Text = SourceAllFilesUC.GetCountsString(PhotosCount, VideosCount);
		}

		public void GetCounts()
		{
			VideosCount = 0;
			PhotosCount = 0;

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
			}
		}
	}
}