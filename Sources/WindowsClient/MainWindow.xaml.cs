using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
			this.LabeledCount.DataContext = LabeledContents;
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
				viewer.Source = lbxContentContainer.DataContext;
				viewer.SelectedIndex = lbxContentContainer.SelectedIndex;

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
				var service = lbxDeviceContainer.SelectedItem as IService;
				if (service == null)
					return;
				lblContentLocation.DataContext = null;
				lbxContentContainer.DataContext = service.Contents;
				return;
			}

			lblContentLocation.DataContext = group;
			lbxContentContainer.DataContext = (group as IContentGroup).Contents;
			return;
		}

		private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, EventArgs e)
		{
			var ti = sender as TreeViewItem;

			if (ti == null)
				return;

			var group = ti.DataContext as IContentGroup;

			if (group == null)
				return;

			lblContentLocation.DataContext = group;
			lbxContentContainer.DataContext = group.Contents;
		}

		#region Event Process
		private void content_TagStatusChanged(object sender, System.EventArgs e)
		{
			//var content = lbxContentContainer.SelectedItem as IContentEntity;
			//ToggleContentTagStatus(content as IContent);
		}

		//private void ToggleContentTagStatus(IContent content)
		//{
		//	if (content == null)
		//		return;

		//	if (content.Liked)
		//	{
		//		ClientFramework.Client.Default.UnTag(content);
		//		return;
		//	}

		//	if (LabeledContents.Contains(content))
		//		return;

		//	ClientFramework.Client.Default.Tag(content);
		//}
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
			//var contentControl = sender as ContentItem;
			//contentControl.Tagged = !contentControl.Tagged;
		}



		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			var arguments = string.Join("~", ClientFramework.Client.Default.TaggedContents.Select(content => content.Uri.LocalPath).ToArray());
			Process.Start("sharedFavorite", "\"" + arguments + "\"");
		}

		private void FavoriteAllButton_Click(object sender, RoutedEventArgs e)
		{
			foreach (IContent content in lbxContentContainer.Items)
			{
				if (!content.Liked)
				{
					content.Liked = true;
				}
			}
		}
	}
}
