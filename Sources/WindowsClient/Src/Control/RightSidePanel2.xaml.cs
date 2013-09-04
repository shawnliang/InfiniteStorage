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

		private String m_oldName;
		private String m_oldFavoriteID;

		#region FavoriteName

		public static readonly DependencyProperty _favoriteName = DependencyProperty.Register("FavoriteName", typeof(String), typeof(RightSidePanel2),
																							  new UIPropertyMetadata(String.Empty, OnFavoriteNameChanged));
		public String FavoriteName
		{
			get { return (String)GetValue(_favoriteName); }
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

			m_oldName = FavoriteName;
			m_oldFavoriteID = labelGroup.ID;
		}

		void m_progressBarTimer_Tick(Object sender, EventArgs e)
		{
			m_progressBarTimer.Stop();

			CheckUploadProgress();
		}

		private void CheckUploadProgress()
		{
			Int32 _uploadFilesCount = m_bunnyLabelContentGroup.QueryAlbumUploadFilesCount(m_bunnyLabelContentGroup.ID);

			Int32 _total = m_bunnyLabelContentGroup.Contents.Count;

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
			control.FavoriteName = (String)e.NewValue;
		}

		private void tbxName_TextChanged(Object sender, TextChangedEventArgs e)
		{
			FavoriteName = tbxName.Text;
		}

		private void btnEmail_Click(Object sender, RoutedEventArgs e)
		{
			OnCloudSharingClick(EventArgs.Empty);
		}

		private void tbtnCloudSharing_Checked(Object sender, RoutedEventArgs e)
		{
			tbtnCloudSharing.Background = m_solidColorBrush;
			tbtnCloudSharing.Content = "關閉";
			sbCloudSharing.Visibility = Visibility.Visible;
			tbLinkOpenClose.Text = "已開啟";
			tipText.Visibility = Visibility.Collapsed;
		}

		private void tbtnCloudSharing_Unchecked(Object sender, RoutedEventArgs e)
		{
			tbtnCloudSharing.Background = Brushes.DodgerBlue;
			tbtnCloudSharing.Content = "開啟";
			sbCloudSharing.Visibility = Visibility.Collapsed;
			spProgressBar.Visibility = Visibility.Collapsed;
			tbLinkOpenClose.Text = "已關閉";
			tipText.Visibility = Visibility.Visible;
		}

		//private void tbxName_LostFocus(object sender, RoutedEventArgs e)
		//{
		//	if (m_oldName != tbxName.Text)
		//	{
		//		string _name = tbxName.Text;

		//		while (IsNewFavoriteNameExist(_name))
		//		{
		//			_name += " (1)";
		//		}

		//		StationAPI.RenameLabel(m_oldFavoriteID, _name);

		//		tbxName.Text = _name;
		//	}
		//}

		//private bool IsNewFavoriteNameExist(string name)
		//{
		//	foreach (IContentGroup _group in ClientFramework.Client.Default.GetFavorites(true))
		//	{
		//		if (_group.Name == name)
		//		{
		//			return true;
		//		}
		//	}

		//	return false;
		//}
	}
}