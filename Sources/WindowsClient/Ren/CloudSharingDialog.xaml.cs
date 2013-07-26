#region

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Waveface.ClientFramework;
using Waveface.Model;

#endregion

namespace Waveface.Client
{
	public partial class CloudSharingDialog : Window
	{
		private IEnumerable<IContentEntity> m_contentEntities;
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

		public CloudSharingDialog(IEnumerable<IContentEntity> contentEntities, string describeText)
		{
			m_contentEntities = contentEntities;
			m_describeText = describeText;

			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			tbName.Text = m_describeText;

			InitUI();

			m_inited = true;

			tbName.Focus();
			tbName.CaretIndex = tbName.Text.Length;

			ShowCountText();
		}

		public void InitUI()
		{
			List<EventItem> _controls = new List<EventItem>();

			foreach (BunnyContent _contentEntity in m_contentEntities)
			{
				if (_contentEntity.Type == ContentType.Photo)
				{
					EventItem _eventItem = new EventItem
											   {
												   FileID = _contentEntity.ID,
												   IsVideo = false,
												   IsPhoto = true
											   };

					_eventItem.BitmapImage = _contentEntity.ThumbnailSource;

					_controls.Add(_eventItem);

				}
				else
				{
					EventItem _eventItem = new EventItem
											   {
												   FileID = _contentEntity.ID,
												   IsVideo = true,
												   IsPhoto = false
											   };

					BitmapImage _bi = new BitmapImage();
					_bi.BeginInit();
					_bi.UriSource = new Uri("pack://application:,,,/Resource/video_ph.png");
					_bi.EndInit();

					_eventItem.BitmapImage = _bi;

					_eventItem.MediaSource = _contentEntity.ThumbnailSource;

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
		}

		private void lbItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ShowCountText();
		}

		private void ShowCountText()
		{
			if (!m_inited)
			{
				return;
			}

			int _v = 0;
			int _p = 0;

			for (int i = 0; i < lbItems.Items.Count; i++)
			{
				ListBoxItem _lbi = lbItems.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;

				if (_lbi.IsSelected)
				{
					EventItem _eventItem = (EventItem)_lbi.Content;

					if (_eventItem.IsPhoto)
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