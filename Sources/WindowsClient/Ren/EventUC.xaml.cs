#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

#endregion

namespace Waveface.Client
{
	public class EventItem : FrameworkElement, INotifyPropertyChanged
	{
		public BitmapSource BitmapImage { get; set; }
		public bool IsVideo { get; set; }
		public bool IsPhoto { get; set; }
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

			List<EventItem> _controls = new List<EventItem>();

			foreach (FileEntry _file in Event)
			{
				if (_file.type == 0)
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
				else
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
			}

			//lbEvent.Items.Clear();
			lbEvent.ItemsSource = _controls;
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