#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

#endregion

namespace Waveface.Client
{
	public partial class ShareEventDialog : Window
	{
		private EventUC m_eventUC;
		private string m_describeText;
		private bool m_inited;
		private int m_videosCount;
		private int m_photosCount;

		public List<string> FileIDs { get; set; }
		
		public string TitleName
		{
			get
			{
				if (tbName.Text == string.Empty)
				{
					return m_describeText;
				}
				else
				{
					return tbName.Text;
				}
			}
		}

		public ShareEventDialog(EventUC eventUc, string describeText)
		{
			m_eventUC = eventUc;
			m_describeText = describeText;

			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			tbName.Text = m_describeText;
			m_videosCount = m_eventUC.VideosCount;
			m_photosCount = m_eventUC.PhotosCount;

			InitUI();

			m_inited = true;

			tbName.Focus();
			tbName.CaretIndex = tbName.Text.Length;
		}

		public void InitUI()
		{
			bool _smallFileExists;

			List<EventItem> _controls = new List<EventItem>();

			foreach (FileEntry _file in m_eventUC.Event)
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
						EventItem _eventItem = new EventItem
							                       {
								                       FileID = _file.id,
								                       IsVideo = false,
								                       IsPhoto = true
							                       };

						BitmapImage _bi = new BitmapImage();
						_bi.BeginInit();
						_bi.UriSource = new Uri(_path, UriKind.Absolute);
						_bi.EndInit();

						_eventItem.BitmapImage = _bi;

						_controls.Add(_eventItem);
					}
				}
				else
				{
					EventItem _eventItem = new EventItem
						                       {
							                       FileID = _file.id,
							                       IsVideo = true,
							                       IsPhoto = false
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

			lbItems.Items.Clear();
			lbItems.ItemsSource = _controls;

			for (int i = 0; i < lbItems.Items.Count; i++)
			{
				ListBoxItem _lbi = lbItems.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
				_lbi.IsSelected = true;
			}

			ShowCountText();
		}

		private void lbItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			int _v = 0;
			int _p = 0;

			if(!m_inited)
			{
				return;
			}

			for (int i = 0; i < lbItems.Items.Count; i++)
			{
				ListBoxItem _lbi = lbItems.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;

				if (_lbi.IsSelected)
				{
					EventItem _eventItem = (EventItem)_lbi.Content;
					
					if(_eventItem.IsPhoto)
					{
						_p++;
					}
					else
					{
						_v++;
					}
				}
			}

			m_photosCount = _p;
			m_videosCount = _v;

			ShowCountText();
		}

		private void ShowCountText()
		{
			tbCounts.Text = GetCountsString(m_photosCount, m_videosCount);
		}

		private string GetCountsString(int photosCount, int videosCount)
		{
			string _c = string.Empty;

			string _photo = " " + (string)Application.Current.FindResource("photo");
			string _photos = " " + (string)Application.Current.FindResource("photos");
			string _video = " " + (string)Application.Current.FindResource("video");
			string _videos = " " + (string)Application.Current.FindResource("videos");

			if (photosCount > 0)
			{
				_c = photosCount + ((photosCount == 1) ? _photo : _photos);
			}

			if (videosCount > 0)
			{
				if (photosCount > 0)
				{
					_c = _c + ", ";
				}

				_c = _c + videosCount + ((videosCount == 1) ? _video : _videos);
			}

			return _c;
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void btnNext_Click(object sender, RoutedEventArgs e)
		{
			FileIDs = new List<string>();

			for (int i = 0; i < lbItems.Items.Count; i++)
			{
				ListBoxItem _lbi = lbItems.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;

				if (_lbi.IsSelected)
				{
					EventItem _eventItem = (EventItem)_lbi.Content;

					FileIDs.Add(_eventItem.FileID);
				}
			}

			Close();
		}
	}
}