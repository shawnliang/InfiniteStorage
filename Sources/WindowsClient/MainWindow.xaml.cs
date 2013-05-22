using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
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
		#region Property
		public ReadOnlyObservableCollection<IContentEntity> LabeledContents
		{
			get
			{
				return Waveface.ClientFramework.Client.Default.TaggedContents;
			}
		}
		#endregion


		public MainWindow()
		{
			InitializeComponent();

			AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.lbxDeviceContainer.DataContext = Waveface.ClientFramework.Client.Default.Services;
			this.lblLabeledCount.DataContext = LabeledContents;
		}

		private void OnPhotoClick(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton != MouseButton.Left)
				return;

			Enter();
		}

		private void Enter()
		{
			var group = (lbxContentContainer.SelectedItem as IContentGroup);

			if (group == null)
			{
				var viewer = new PhotoViewer();
				viewer.Owner = this;
				viewer.pvcViewer.Source = lbxContentContainer.DataContext;
				viewer.pvcViewer.SelectedIndex = lbxContentContainer.SelectedIndex;

				viewer.ShowDialog();
				return;
			}

			lblContentLocation.DataContext = group;
			lbxContentContainer.DataContext = group.Contents;
		}

		private void BackButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Back();
		}

		private void Back()
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

		//private void lbxDeviceContainer_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		//{
		//	lblContentLocation.DataContext = null;
		//	lbxContentContainer.DataContext = (lbxDeviceContainer.SelectedItem as IService).Contents;
		//}

		private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var ti = sender as TreeViewItem;
			if (ti != null)
			{
				//ToDo: 待重構
				var service = ti.DataContext as IService;

				if (service != null)
				{
					lblContentLocation.DataContext = null;
					lbxContentContainer.DataContext = service.Contents;
				}
				else
				{
					lblContentLocation.DataContext = ti.DataContext;
					lbxContentContainer.DataContext = (ti.DataContext as IContentGroup).Contents;
				}
			}

		}

		#region Event Process
		private void content_TagStatusChanged(object sender, System.EventArgs e)
		{
			//TODO: 待重構
			var content = lbxContentContainer.SelectedItem as IContentEntity;
			var contentControl = sender as ContentItem;

			if (!contentControl.Tagged)
			{
				ClientFramework.Client.Default.UnTag(content);
				return;
			}

			if (LabeledContents.Contains(content))
				return;

			ClientFramework.Client.Default.Tag(content);
		}
		#endregion

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Back:
					Back();
					break;
				case Key.Enter:
					Enter();
					break;
			}
		}

		private void content_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			var contentControl = sender as ContentItem;
			contentControl.Tagged = !contentControl.Tagged;
		}
	}
}
