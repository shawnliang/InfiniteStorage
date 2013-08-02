#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Waveface.ClientFramework;

#endregion

namespace Waveface.Client
{
	public partial class RightSidePanel2 : UserControl
	{
		public event EventHandler OnAirClick;
		public event EventHandler CloudSharingClick;
		public event EventHandler DeleteButtonClick;

		private SolidColorBrush m_solidColorBrush;
		private DispatcherTimer m_progressBarTimer;
		private BunnyLabelContentGroup m_bunnyLabelContentGroup;

		#region FavoriteName

		public static readonly DependencyProperty _favoriteName = DependencyProperty.Register("FavoriteName", typeof(string), typeof(RightSidePanel2),
																							  new UIPropertyMetadata(string.Empty, OnFavoriteNameChanged));

		public string FavoriteName
		{
			get { return (string)GetValue(_favoriteName); }
			set
			{
				SetValue(_favoriteName, value);
				tbxName.Text = value;
			}
		}

		#endregion

		public RightSidePanel2()
		{
			m_solidColorBrush = new SolidColorBrush(Color.FromArgb(255, 120, 0, 34));

			InitializeComponent();

			m_progressBarTimer = new DispatcherTimer();
			m_progressBarTimer.Tick += m_progressBarTimer_Tick;
			m_progressBarTimer.Interval = new TimeSpan(0, 0, 2);
		}

		public void Update(BunnyLabelContentGroup labelGroup)
		{
			m_progressBarTimer.Stop();
			spProgressBar.Visibility = Visibility.Collapsed;

			m_bunnyLabelContentGroup = labelGroup;

			tbxShareLink.Text = labelGroup.ShareURL;

			CheckUploadProgress();
		}

		void m_progressBarTimer_Tick(object sender, EventArgs e)
		{
			m_progressBarTimer.Stop();

			CheckUploadProgress();
		}

		private void CheckUploadProgress()
		{
			int _uploadFilesCount = m_bunnyLabelContentGroup.QueryAlbumUploadFilesCount(m_bunnyLabelContentGroup.Name);

			int _total = m_bunnyLabelContentGroup.Contents.Count;

			if (_uploadFilesCount == _total)
			{
				spProgressBar.Visibility = Visibility.Collapsed;
			}
			else
			{
				tbProgress.Text = "(" + _uploadFilesCount + "/" + _total + ")";

				if (sbCloudSharing.Visibility == Visibility.Visible)
				{
					spProgressBar.Visibility = Visibility.Visible;
				}

				m_progressBarTimer.Start();
			}
		}

		#region Protected Method

		protected void OnOnAirClick(EventArgs e)
		{
			if (OnAirClick == null)
				return;

			OnAirClick(this, e);
		}

		protected void OnCloudSharingClick(EventArgs e)
		{
			if (CloudSharingClick == null)
				return;

			CloudSharingClick(this, e);
		}

		protected void OnDeleteButtonClick(EventArgs e)
		{
			if (DeleteButtonClick == null)
				return;

			DeleteButtonClick(this, e);
		}

		#endregion

		private static void OnFavoriteNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;

			var control = o as RightSidePanel2;
			control.FavoriteName = (string)e.NewValue;
		}

		private void tbxName_TextChanged(object sender, TextChangedEventArgs e)
		{
			FavoriteName = tbxName.Text;
		}

		private void btnEmail_Click(object sender, RoutedEventArgs e)
		{
			OnCloudSharingClick(EventArgs.Empty);
		}

		private void tbtnCloudSharing_Checked(object sender, RoutedEventArgs e)
		{
			tbtnCloudSharing.Background = m_solidColorBrush;
			tbtnCloudSharing.Content = "關閉";
			sbCloudSharing.Visibility = Visibility.Visible;
			tbLinkOpenClose.Text = "已開啟";
		}

		private void tbtnCloudSharing_Unchecked(object sender, RoutedEventArgs e)
		{
			tbtnCloudSharing.Background = Brushes.DodgerBlue;
			tbtnCloudSharing.Content = "開啟";
			sbCloudSharing.Visibility = Visibility.Collapsed;
			spProgressBar.Visibility = Visibility.Collapsed;
			tbLinkOpenClose.Text = "已關閉";
		}
	}
}