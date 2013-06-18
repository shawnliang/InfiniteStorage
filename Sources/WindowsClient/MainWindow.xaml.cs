using System;
using System.Collections.Generic;
using System.Linq;
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
		public MainWindow()
		{
			InitializeComponent();

			AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.lbxDeviceContainer.DataContext = Waveface.ClientFramework.Client.Default.Services;

			this.lbxFavorites.DataContext = Waveface.ClientFramework.Client.Default.Favorites;

			rspRightSidePane2.tbxName.KeyDown += tbxName_KeyDown;
			rspRightSidePane2.tbxName.LostFocus += tbxName_LostFocus;

			rspRightSidePanel.btnClearAll.Click += new RoutedEventHandler(btnClearAll_Click);

			lblContentTypeCount.Content = string.Format("0 photos 0 videos");
		}

		void btnClearAll_Click(object sender, RoutedEventArgs e)
		{
			ClientFramework.Client.Default.ClearTaggedContents();
			ShowSelectedFavoriteContents(lbxFavorites);
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

			group.Refresh();

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

			lbxContentContainer.ContextMenu.Visibility = System.Windows.Visibility.Collapsed;

			lblContentLocation.DataContext = group;
			lbxContentContainer.DataContext = group.Contents;
			SetContentTypeCount(group);


			Grid.SetColumnSpan(gdContentArea, 2);

			btnFavoriteAll.Visibility = Visibility.Visible;
			btnBack.Visibility = Visibility.Visible;
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
					if (lbxContentContainer.SelectedItem == null)
						return;
					Enter();
					break;
			}
		}

		private void FavoriteAllButton_Click(object sender, RoutedEventArgs e)
		{
			ClientFramework.Client.Default.Tag(lbxContentContainer.Items.OfType<IContent>());
			RefreshContentArea();
		}

		private void lbxFavorites_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			ShowSelectedFavoriteContents(sender);
		}

		private void ShowSelectedFavoriteContents(object sender)
		{
			var listBox = sender as ListBox;

			if (listBox == null)
				return;

			var group = listBox.SelectedItem as IContentGroup;

			if (group == null)
				return;

			group.Refresh();

			lblContentLocation.DataContext = group;
			lbxContentContainer.DataContext = group.Contents;
			SetContentTypeCount(group);

			updateRightSidePanel2(group);

			lbxContentContainer.ContextMenu.IsOpen = false;
			lbxContentContainer.ContextMenu.Visibility = System.Windows.Visibility.Visible;

			gdRightSide.Visibility = System.Windows.Visibility.Visible;
			Grid.SetColumnSpan(gdContentArea, 1);

			btnFavoriteAll.Visibility = Visibility.Collapsed;
			btnBack.Visibility = Visibility.Collapsed;
			unSortedFilesUC.Visibility = Visibility.Collapsed;
			rspRightSidePane2.Visibility = (group.Name.Equals("STARRED", StringComparison.CurrentCultureIgnoreCase)) ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
			rspRightSidePanel.Visibility = (group.Name.Equals("STARRED", StringComparison.CurrentCultureIgnoreCase)) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

			if (rspRightSidePanel.Visibility == System.Windows.Visibility.Visible)
			{
				var contentEntities = lbxContentContainer.DataContext as IEnumerable<IContentEntity>;

				if (contentEntities != null)
				{
					rspRightSidePanel.PhotoCount = contentEntities.Count(item =>
					{
						var content = item as IContent;
						if (content == null)
							return false;

						return content.Type == ContentType.Photo;
					});

					rspRightSidePanel.VideoCount = contentEntities.Count(item =>
					{
						var content = item as IContent;
						if (content == null)
							return false;

						return content.Type == ContentType.Video;
					});
				}
			}
		}

		private void rspRightSidePanel_SaveToFavorite(object sender, System.EventArgs e)
		{
			var dialog = new CreateFavoriteDialog();
			dialog.Owner = this;
			dialog.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

			if (dialog.ShowDialog() != true)
				return;

			ClientFramework.Client.Default.SaveToFavorite(dialog.FavoriteName);
			lbxFavorites.SelectedIndex = lbxFavorites.Items.Count - 1;
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

		private void rspRightSidePanel_AddToFavorite(object sender, System.EventArgs e)
		{
			var favorites = Waveface.ClientFramework.Client.Default.Favorites.Skip(1);

			if (!favorites.Any())
			{
				MessageBox.Show("Without any favorite can be added!");
				return;
			}

			var dialog = new AddToFavoriteDialog();
			dialog.Owner = this;
			dialog.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

			dialog.FavoriteItemSource = favorites;

			if (dialog.ShowDialog() != true)
				return;

			var selectedFavorite = (dialog.SelectedFavorite as IContentGroup);
			ClientFramework.Client.Default.AddToFavorite(selectedFavorite.ID);
			selectedFavorite.Refresh();
			lbxFavorites.SelectedItem = selectedFavorite;
		}

		private void rspRightSidePane2_CloudSharingClick(object sender, System.EventArgs e)
		{
			Wpf_testHTTP.MainWindow _w = new Wpf_testHTTP.MainWindow();
			var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny");

			var iniFile = System.IO.Path.Combine(path, @"sharefavorite.ini");

			System.IO.File.Create(iniFile).Close();

			_w.setTitle(this.Title);
			_w.setiniPath(iniFile);

			var files = string.Join("~", (lbxContentContainer.DataContext as IEnumerable<IContentEntity>).Select(content => content.Uri.LocalPath).ToArray());

			_w.setFilename(files);
			_w.setRun();
			_w.Show();
		}

		private void RefreshContentArea()
		{
			var group = lblContentLocation.DataContext as IContentGroup;
			if (group == null)
				return;

			group.Refresh();
			SetContentTypeCount(group);
		}

		private void lbxFavorites_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			ShowSelectedFavoriteContents(sender);
		}

		private void rspRightSidePane2_DeleteButtonClick(object sender, System.EventArgs e)
		{
			var group = (lblContentLocation.DataContext as IContentGroup);
			ClientFramework.Client.Default.RemoveFavorite(group.ID);

			lbxFavorites.SelectedIndex = 0;
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			var group = (lblContentLocation.DataContext as IContentGroup);
			var content = lbxContentContainer.SelectedItem as IContentEntity;
			ClientFramework.Client.Default.UnTag(group.ID, content.ID);
			RefreshContentArea();
		}
	}
}
