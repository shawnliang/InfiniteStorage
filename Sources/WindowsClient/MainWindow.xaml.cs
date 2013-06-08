using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
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
			var fav = Waveface.ClientFramework.Client.Default.Favorites;

			//this.rspRightSidePanel.DataContext = LabeledContents;
			this.lbxFavorites.DataContext = Waveface.ClientFramework.Client.Default.Favorites;

			rspRightSidePane2.tbxName.KeyDown += tbxName_KeyDown;
			rspRightSidePane2.tbxName.LostFocus += tbxName_LostFocus;
		}

		void tbxName_LostFocus(object sender, RoutedEventArgs e)
		{
			RenameFavorite();
		}

		void tbxName_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			RenameFavorite();
		}

		private void RenameFavorite()
		{

			try
			{
				StationAPI.RenameLabel((lbxFavorites.SelectedItem as IContentGroup).ID, rspRightSidePane2.tbxName.Text);
			}
			catch (Exception)
			{
			}
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
			SetContentTypeCount(group);
		}

		private void SetContentTypeCount(IContentGroup group)
		{
			lblContentTypeCount.Content = string.Format("{0} photos {1} videos",
						group.Contents.Count(item =>
						{
							var content = item as IContent;
							if (content == null)
								return false;

							return content.Type == ContentType.Photo;
						}),
						group.Contents.Count(item =>
						{
							var content = item as IContent;
							if (content == null)
								return false;

							return content.Type == ContentType.Video;
						}));
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
				lblContentTypeCount.Content = string.Format("0 photos 0 videos");
				return;
			}

			lblContentLocation.DataContext = group;
			lbxContentContainer.DataContext = (group as IContentGroup).Contents;
			SetContentTypeCount(group as IContentGroup);
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

			if (group.ID.Equals("Unsorted", StringComparison.CurrentCultureIgnoreCase))
			{
				ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(ti) as TreeViewItem;

				if (parent == null)
					return;

				var service = parent.DataContext as IService;

				if (service == null)
					return;

				unSortedFilesUC.Visibility = Visibility.Visible;
				unSortedFilesUC.Init(service);
			}
			else
			{
				unSortedFilesUC.Visibility = Visibility.Collapsed;
			}

			lblContentLocation.DataContext = group;
			lbxContentContainer.DataContext = group.Contents;
			SetContentTypeCount(group);


			Grid.SetColumnSpan(gdContentArea, 2);
			
			gdRightSide.Visibility = System.Windows.Visibility.Collapsed;

			rspRightSidePanel.Visibility = System.Windows.Visibility.Collapsed;
			rspRightSidePane2.Visibility = System.Windows.Visibility.Collapsed;
		}


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

		private void lbxFavorites_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			var listBox = sender as ListBox;

			if (listBox == null)
				return;

			var group = listBox.SelectedItem as IContentGroup;

			if (group == null)
				return;

			lblContentLocation.DataContext = group;
			lbxContentContainer.DataContext = group.Contents;
			SetContentTypeCount(group);

			updateRightSidePanel2(group);

			gdRightSide.Visibility = System.Windows.Visibility.Visible;
			Grid.SetColumnSpan(gdContentArea, 1);

			unSortedFilesUC.Visibility = Visibility.Collapsed;
			rspRightSidePane2.Visibility = (group.Name.Equals("Tag", StringComparison.CurrentCultureIgnoreCase)) ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
			rspRightSidePanel.Visibility = (group.Name.Equals("Tag", StringComparison.CurrentCultureIgnoreCase)) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
		}
		
		private void rspRightSidePanel_SaveToFavorite(object sender, System.EventArgs e)
		{
			ClientFramework.Client.Default.SaveToFavorite();
		}

		private void rspRightSidePane2_OnAirClick(object sender, EventArgs e)
		{
			var isOnAir = ClientFramework.Client.Default.IsOnAir((IContentGroup)lblContentLocation.DataContext);

			ClientFramework.Client.Default.OnAir((lblContentLocation.DataContext as IContentEntity).ID, !isOnAir);
		}

		private void updateRightSidePanel2(IContentGroup group)
		{
			//rspRightSidePane2.FavoriteName = group.Name;

			if (!ClientFramework.Client.Default.HomeSharingEnabled)
			{
				rspRightSidePane2.btnAction.IsEnabled = false;
			}
			else
			{
				var isOnAir = ClientFramework.Client.Default.IsOnAir(group);
				rspRightSidePane2.btnAction.IsEnabled = true;
				rspRightSidePane2.btnAction.IsChecked = isOnAir;
			}
		}

		private void rspRightSidePanel_ShareButtonClick(object sender, System.EventArgs e)
		{
			Wpf_testHTTP.MainWindow _w = new Wpf_testHTTP.MainWindow();
			var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny");

			var iniFile = System.IO.Path.Combine(path, @"sharefavorite.ini");

		    System.IO.File.Create(iniFile).Close();

			_w.setTitle(this.Title);
            _w.setiniPath (iniFile);

			var files = string.Join("~", ClientFramework.Client.Default.TaggedContents.Select(content => content.Uri.LocalPath).ToArray());

			_w.setFilename(files);
            _w.setRun();
            _w.ShowDialog();
		}
	}
}
