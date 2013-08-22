#region

using System;
using System.Collections.Generic;
using System.Linq;
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
		private List<string> m_selectedEntitieIDs;
		private List<IContentEntity> m_allEntities;

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
					return m_describeText + " [" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "]";
				}
				else
				{
					return tbName.Text;
				}
			}
		}

		public CloudSharingDialog(IEnumerable<IContentEntity> allEntities, IEnumerable<IContentEntity> selectedEntities, string describeText)
		{
			m_allEntities = allEntities.ToList();
			m_describeText = describeText;

			GetSelectedEntitieIDs(selectedEntities);

			InitializeComponent();

			CenterWindowOnScreen();
		}

		private void CenterWindowOnScreen()
		{
			double _screenWidth = SystemParameters.PrimaryScreenWidth;
			double _screenHeight = SystemParameters.PrimaryScreenHeight;

			Width = _screenWidth * 0.75;
			Height = _screenHeight * 0.75;

			Left = (_screenWidth / 2) - (Width / 2);
			Top = (_screenHeight / 2) - (Height / 2);
		}

		private void GetSelectedEntitieIDs(IEnumerable<IContentEntity> selectedEntities)
		{
			m_selectedEntitieIDs = new List<string>();

			foreach (IContentEntity _entity in selectedEntities)
			{
				m_selectedEntitieIDs.Add(_entity.ID);
			}
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

			foreach (BunnyContent _contentEntity in m_allEntities)
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

				if (m_selectedEntitieIDs.Contains(m_allEntities[i].ID))
				{
					_lbi.IsSelected = true;
				}
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

			selectionText.Content = string.Format((string)FindResource("selection_text"), m_photosCount + m_videosCount);

			btnNext.IsEnabled = !((m_photosCount == 0) && (m_videosCount == 0));
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

		private void btnSelectAll_Click(object sender, RoutedEventArgs e)
		{
			lbItems.SelectAll();
		}

		private void lbItems_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			//lbItems.UnselectAll();
		}

		private void btnUnSelectAll_Click(object sender, RoutedEventArgs e)
		{
			lbItems.UnselectAll();
		}
	}
}