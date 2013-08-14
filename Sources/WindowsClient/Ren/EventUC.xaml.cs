#region

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

#endregion

namespace Waveface.Client
{
	public class EventItem : FrameworkElement, INotifyPropertyChanged
	{
		public BitmapSource BitmapImage { get; set; }
		public bool IsVideo { get; set; }
		public bool IsPhoto { get; set; }
		public bool IsGIF { get; set; }
		public BitmapSource MediaSource { get; set; }
		public string FileID { get; set; }
		public bool HasOrigin { get; set; }

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
		public List<FileEntry> Event { get; set; }
		public string YM { get; set; }
		public int VideosCount { get; set; }
		public int PhotosCount { get; set; }

		public EventUC()
		{
			InitializeComponent();
		}

		public void SetUI()
		{
			GetCounts();

			SetInfor();

			//Event.Reverse();

			//試驗連張轉Gif
			//List<FileEntry> _temp = GenGif();
			List<FileEntry> _temp = Event;

			List<EventItem> _controls = new List<EventItem>();

			foreach (FileEntry _file in _temp)
			{
				switch (_file.type)
				{
					case 0:
						{
							string _path = _file.tiny_path;

							try
							{
								EventItem _eventItem = new EventItem
														   {
															   FileID = _file.id,
															   IsVideo = false,
															   IsPhoto = true,
															   HasOrigin = !_file.has_origin
														   };

								BitmapImage _bi = new BitmapImage();
								_bi.BeginInit();
								_bi.UriSource = new Uri(_path, UriKind.Absolute);
								_bi.EndInit();

								_eventItem.BitmapImage = _bi;

								_controls.Add(_eventItem);
							}
							catch
							{
							}
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

							BitmapImage _bi = new BitmapImage();
							_bi.BeginInit();
							_bi.UriSource = new Uri("pack://application:,,,/Resource/video_ph.png");
							_bi.EndInit();

							_eventItem.BitmapImage = _bi;

							BitmapImage _vidoeThumb = new BitmapImage();
							_vidoeThumb.BeginInit();

							if (File.Exists(_file.tiny_path))
								_vidoeThumb.UriSource = new Uri(_file.tiny_path, UriKind.Absolute);
							else
								_vidoeThumb.UriSource = new Uri("pack://application:,,,/Ren/Images/video_130x110.png");

							_vidoeThumb.EndInit();

							_eventItem.MediaSource = _vidoeThumb;

							_controls.Add(_eventItem);
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

								_controls.Add(_eventItem);
							}
							catch
							{
							}
						}
						break;
				}
			}

			//lbEvent.Items.Clear();
			lbEvent.ItemsSource = _controls;
		}

		private List<FileEntry> GenGif()
		{
			Dictionary<int, int> _temp = new Dictionary<int, int>();

			int _idx = 0;
			bool _preOK = false;

			for (int i = 0; i < Event.Count; i++)
			{
				if (i < Event.Count - 1)
				{
					if (Math.Abs(Event[i + 1].taken_time.Second - Event[i].taken_time.Second) < 8)
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

			for (int i = 0; i < Event.Count; i++)
			{
				if (_temp.Keys.Contains(i) && _temp[i] > 2)
				{
					FileEntry _fileEntry = GetGifFile(i, _temp[i]);
					_ret.Add(_fileEntry);
					_skip = _temp[i];
				}

				if (_skip-- <= 0)
					_ret.Add(Event[i]);
			}

			return _ret;
		}

		private FileEntry GetGifFile(int idx, int length)
		{
			string _tempPathBase = Path.GetTempPath() + "Waveface Photos" + "\\";

			if (!Directory.Exists(_tempPathBase))
				Directory.CreateDirectory(_tempPathBase);

			string _gif = Path.Combine(_tempPathBase, Path.GetFileName(Event[idx].tiny_path)) + ".gif";

			if (!File.Exists(_gif))
			{
				List<string> imageFilePaths = new List<string>();

				for (int k = 0; k < length; k++)
				{
					imageFilePaths.Add(Event[idx + k].tiny_path);
				}

				AnimatedGifEncoder _gifEncoder = new AnimatedGifEncoder();
				_gifEncoder.Start(_gif);
				_gifEncoder.SetDelay(666);
				_gifEncoder.SetRepeat(0); //-1:no repeat,0:always repeat

				for (int i = 0; i < imageFilePaths.Count; i++)
				{
					_gifEncoder.AddFrame(System.Drawing.Image.FromFile(imageFilePaths[i]));
				}

				_gifEncoder.Finish();
			}

			FileEntry _ret = (FileEntry)Event[idx].Clone();
			_ret.tiny_path = _gif;
			_ret.type = 999;

			return _ret;
		}

		private void SetInfor()
		{
			string _timeInterval = GetTimeDisplayString();

			tbTitle.Text = _timeInterval;

			tbTimeAgo.Visibility = Visibility.Collapsed;

			tbTotalCount.Text = SourceAllFilesUC.GetCountsString(PhotosCount, VideosCount);
		}

		private string GetTimeDisplayString()
		{
			return Event[0].taken_time.ToString("MMM yyyy");
		}

		public void GetCounts()
		{
			VideosCount = 0;
			PhotosCount = 0;

			foreach (FileEntry _file in Event)
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