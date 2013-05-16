using System.Collections.ObjectModel;
using System.Windows;
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
		public static readonly DependencyProperty _labeledContents = DependencyProperty.Register("LabeledContents", typeof(ObservableCollection<IContentEntity>), typeof(MainWindow), new UIPropertyMetadata(new ObservableCollection<IContentEntity>(), new PropertyChangedCallback(OnLabeledContentsChanged)));
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
			this.lbxLabeledContentContainer.DataContext = LabeledContents;
		}

		private void OnPhotoClick(object sender, RoutedEventArgs e)
		{
			var group = (lbxContentContainer.SelectedItem as IContentGroup);

			if(group == null)
			{
				pvcViewer.SelectedSource = lbxContentContainer.SelectedItem;
				pvcViewer.Source = ((lbxContentContainer.SelectedItem as IContent).Parent as IContentGroup).Contents;
				ChangeToPhotoView();
				return;
			}

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
				if (lbxContentContainer.SelectedItem == null)
					return;

				var currentContentEntity = (lbxContentContainer.SelectedItem as IContentEntity);
				if(currentContentEntity.Parent == null || currentContentEntity.Parent.Parent == null)
				{
					lbxContentContainer.DataContext = (lbxDeviceContainer.SelectedItem as IService).Contents;
					return;
				}

				lbxContentContainer.DataContext = (currentContentEntity.Parent.Parent as IContentGroup).Contents;
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
			lbxContentContainer.DataContext = (lbxDeviceContainer.SelectedItem as IService).Contents;
		}

		private void lbxDeviceContainer_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			lbxContentContainer.DataContext = (lbxDeviceContainer.SelectedItem as IService).Contents;
		}
		
		#region Event Process
		private static void OnLabeledContentsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;

			var control = o as DeviceListItem;
			control.DeviceName = (string)e.NewValue;
		}

		private void onMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			LabeledContents.Add(lbxContentContainer.SelectedItem as IContentEntity);
		}
		#endregion
	}
}
