using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Waveface.ClientFramework;
using Waveface.Model;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Var
		public static readonly DependencyProperty _labeledContents = DependencyProperty.Register("LabeledContents", typeof(ObservableCollection<IContentEntity>), typeof(MainWindow), new UIPropertyMetadata(new ObservableCollection<IContentEntity>(), null));
		#endregion

		#region Property
		public ObservableCollection<IContentEntity> LabeledContents
		{
			get
			{
				return (ObservableCollection<IContentEntity>)GetValue(_labeledContents);
			}
			set
			{
				SetValue(_labeledContents, value);
			}
		}
		#endregion


		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.lbxDeviceContainer.DataContext = BunnyServiceSupplier.Instance.Services;
			this.lblLabeledCount.DataContext = LabeledContents;
		}

		private void OnPhotoClick(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton != MouseButton.Left)
				return;

			var group = (lbxContentContainer.SelectedItem as IContentGroup);

			if (group == null)
			{
				pvcViewer.SelectedSource = lbxContentContainer.SelectedItem;
				pvcViewer.Source = ((lbxContentContainer.SelectedItem as IContent).Parent as IContentGroup).Contents;
				ChangeToPhotoView();
				return;
			}

			lblContentLocation.DataContext = group;
			lbxContentContainer.DataContext = group.Contents;
		}

		private void ChangeToPhotoView()
		{
			if (IsPhotoView())
				return;
			pvcViewer.Visibility = System.Windows.Visibility.Visible;
			svContentContainer.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void BackButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (IsGalleryView())
			{
				var group = lblContentLocation.DataContext as IContentEntity;
				if (group == null)
					return;

				group = group.Parent;

				if (group == null)
				{
					lblContentLocation.DataContext = null;
					lbxContentContainer.DataContext = (lbxDeviceContainer.SelectedItem as IService).Contents;
					return;
				}
				
				lblContentLocation.DataContext = group;
				lbxContentContainer.DataContext = (group as IContentGroup).Contents;
				return;
			}
			ChangeToGalleryView();
		}

		private bool IsPhotoView()
		{
			return pvcViewer.Visibility == System.Windows.Visibility.Visible;
		}

		private bool IsGalleryView()
		{
			return svContentContainer.Visibility == System.Windows.Visibility.Visible;
		}


		private void ChangeToGalleryView()
		{
			if (IsGalleryView())
				return;
			pvcViewer.Source = null;
			pvcViewer.Visibility = System.Windows.Visibility.Collapsed;
			svContentContainer.Visibility = System.Windows.Visibility.Visible;
		}

		private void lbxDeviceContainer_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			lblContentLocation.DataContext = null;
			lbxContentContainer.DataContext = (lbxDeviceContainer.SelectedItem as IService).Contents;
		}

		private void lbxDeviceContainer_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			lblContentLocation.DataContext = null;
			lbxContentContainer.DataContext = (lbxDeviceContainer.SelectedItem as IService).Contents;
		}

		#region Event Process
		private void content_TagStatusChanged(object sender, System.EventArgs e)
		{
			//TODO: 待重構
			var content = lbxContentContainer.SelectedItem as IContentEntity;
			var contentControl = sender as ContentItem;

			if (!contentControl.Tagged)
			{
				LabeledContents.Remove(content);
				ClientFramework.Client.Default.UnTag(content);
				return;
			}

			if (LabeledContents.Contains(content))
				return;

			LabeledContents.Add(content);
			ClientFramework.Client.Default.Tag(content);
		}
		#endregion
	}
}
