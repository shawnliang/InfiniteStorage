#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Waveface.Client.Properties;
using Waveface.ClientFramework;
using Waveface.Model;
using log4net;
using CommandLine;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows.Data;

#endregion

namespace Waveface.Client
{
	public partial class MainWindow : Window
	{
		public static string HELP_URL = "http://waveface.uservoice.com/knowledgebase/articles/214932-step1-import-photos-videos-from-your-phone";

		private DispatcherTimer uiDelayTimer;
		private DispatcherTimer recentTimer;
		private DispatcherTimer timelineShareToDelayTimer;
		private DispatcherTimer jumpToDeviceTimer;

		private Point startPoint;
		private Boolean needSpecialMulitSelectProcess;
		private DateTime lastMouseLeftButtonDown;
		private long sourceTreeClickCount;

		public MainWindow()
		{
			InitializeComponent();

#if !DEBUG
			AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
#endif
		}

        private bool CloudAlbumFilter(object item)
        {
            var group = item as BunnyLabelContentGroup;
            return group.ShareEnabled;
        }

        private bool LocalAlbumFilter(object item)
        {
            var group = item as BunnyLabelContentGroup;
            return !group.ShareEnabled;
        }

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			lbxDeviceContainer.DataContext = ClientFramework.Client.Default.Services;

            ICollectionView cloudAlbums = new ListCollectionView(ClientFramework.Client.Default.Favorites);
            cloudAlbums.Filter = CloudAlbumFilter;

            lbxCloudAlbums.DataContext = cloudAlbums;

            ICollectionView localAlbums = new ListCollectionView(ClientFramework.Client.Default.Favorites);
            localAlbums.Filter = LocalAlbumFilter;


            lbxFavorites.DataContext = localAlbums;

           
			lbxRecent.DataContext = ClientFramework.Client.Default.Recent;

			rspRightSidePane2.tbxName.KeyDown += tbxName_KeyDown;
			rspRightSidePane2.tbxName.LostFocus += tbxName_LostFocus;

			//rspRightSidePane2.tbtnHomeSharing.Checked += rspRightSidePane2_OnAirClick;
			//rspRightSidePane2.tbtnHomeSharing.Unchecked += rspRightSidePane2_OnAirClick;

			rspRightSidePane2.tbtnCloudSharing.Checked += tbtnCloudSharing_Checked;
			rspRightSidePane2.tbtnCloudSharing.Unchecked += tbtnCloudSharing_Checked;
			rspRightSidePane2.btnCopyShareLink.Click += btnCopyShareLink_Click;
			rspRightSidePane2.btnOpenShareLink.Click += btnOpenShareLink_Click;

			rspRightSidePanel.btnClearAll.Click += btnClearAll_Click;

			//rspRightSidePane2.lblHomeSharingTutorialTip.MouseDown += lblHomeSharingTutorialTip_MouseDown;

			lblContentTypeCount.Content = string.Format("0 photos 0 videos");

            //Observable.FromEventPattern(
            //    h => lbxDeviceContainer.TreeViewItemClick += h,
            //    h => lbxDeviceContainer.TreeViewItemClick -= h
            //    )
            //    .Window(TimeSpan.FromMilliseconds(50))
            //    .SelectMany(x => x.TakeLast(1))
            //    .SubscribeOn(ThreadPoolScheduler.Instance)
            //    .ObserveOn(DispatcherScheduler.Current)
            //    .Subscribe(ex => TreeViewItem_PreviewMouseLeftButtonDown(ex.Sender, ex.EventArgs));

            Observable.FromEvent<RoutedPropertyChangedEventHandler<object>, RoutedPropertyChangedEventArgs<object>>(
            handler => (s, ex) => handler(ex),
            h => lbxDeviceContainer.SelectedItemChanged += h,
            h => lbxDeviceContainer.SelectedItemChanged -= h
            )
            .Window(TimeSpan.FromMilliseconds(50))
            .SelectMany(x => x.TakeLast(1))
            .SubscribeOn(ThreadPoolScheduler.Instance)
            .ObserveOn(DispatcherScheduler.Current)
            .Subscribe(ex => TreeViewItem_SelectedItemChanged(ex.OriginalSource, ex));

            Observable.FromEvent<SelectionChangedEventHandler, SelectionChangedEventArgs>(
            handler => (s, ex) => handler(ex),
            h => lbxCloudAlbums.SelectionChanged += h,
            h => lbxCloudAlbums.SelectionChanged -= h
            )
            .Window(TimeSpan.FromMilliseconds(50))
            .SelectMany(x => x.TakeLast(1))
            .SubscribeOn(ThreadPoolScheduler.Instance)
            .ObserveOn(DispatcherScheduler.Current)
            .Subscribe(ex => lbxFavorites_SelectionChanged(lbxCloudAlbums, ex));

			Observable.FromEvent<SelectionChangedEventHandler, SelectionChangedEventArgs>(
				handler => (s, ex) => handler(ex),
				h => lbxFavorites.SelectionChanged += h,
				h => lbxFavorites.SelectionChanged -= h
				)
				.Window(TimeSpan.FromMilliseconds(50))
				.SelectMany(x => x.TakeLast(1))
				.SubscribeOn(ThreadPoolScheduler.Instance)
				.ObserveOn(DispatcherScheduler.Current)
				.Subscribe(ex => lbxFavorites_SelectionChanged(lbxFavorites, ex));

			Observable.FromEvent<SelectionChangedEventHandler, SelectionChangedEventArgs>(
				handler => (s, ex) => handler(ex),
				h => lbxRecent.SelectionChanged += h,
				h => lbxRecent.SelectionChanged -= h
				)
				.Window(TimeSpan.FromMilliseconds(50))
				.SelectMany(x => x.TakeLast(1))
				.SubscribeOn(ThreadPoolScheduler.Instance)
				.ObserveOn(DispatcherScheduler.Current)
				.Subscribe(ex => lbxRecent_SelectionChanged(lbxRecent, ex));

			uiDelayTimer = new DispatcherTimer();
			uiDelayTimer.Tick += uiDelayTimer_Tick;
			uiDelayTimer.Interval = new TimeSpan(0, 0, 1);
			uiDelayTimer.Start();

			recentTimer = new DispatcherTimer();
			recentTimer.Tick += recentTimer_Tick;
			recentTimer.Interval = new TimeSpan(0, 0, 2);
			recentTimer.Start();


			var cmdArgs = Environment.GetCommandLineArgs();
			var options = new CmdLineOptions();
			CommandLine.Parser.Default.ParseArguments(cmdArgs, options);

			if (!string.IsNullOrEmpty(options.select_device))
				JumpToDevice(options.select_device);
			else if (ClientFramework.Client.Default.Services.Any())
				JumpToDevice(ClientFramework.Client.Default.Services.First().ID);
		}

		#region Private Method

		private void RefreshStarFavorite()
		{
			RefreshFavorite(lbxFavorites.Items.OfType<IContentGroup>().FirstOrDefault());
		}

		private void TryDisplayUnsortedTutorial()
		{
			if (!Settings.Default.IsFirstSelectUnsorted)
			{
				var result = TakeTourDialog.ShowWithDynamicResource("TakeTourMsgOranize", this);

				if (result.HasValue && result.Value)
					Process.Start(@"http://waveface.uservoice.com/knowledgebase/articles/215521-step2-organizing-photos-and-videos-in-favorite-");

				Settings.Default.IsFirstSelectUnsorted = true;
				Settings.Default.Save();
			}
		}

		private void TryDisplayFavoriteTutorial()
		{
			if (!Settings.Default.IsFirstSelectFavorite)
			{
				var result = TakeTourDialog.ShowWithDynamicResource("TakeTourMsgShare", this);

				if (result.HasValue && result.Value)
					Process.Start(@"http://waveface.uservoice.com/knowledgebase/articles/215523-step4-share-favorites-with-your-favorite-people");

				Settings.Default.IsFirstSelectFavorite = true;
				Settings.Default.Save();
			}
		}

		private void TryDisplayStarredTutorial()
		{
			if (!Settings.Default.IsFirstSelectStarred)
			{
				var result = TakeTourDialog.ShowWithDynamicResource("TakeTourMSgHomeShare", this);

				if (result.HasValue && result.Value)
					Process.Start(@"http://waveface.uservoice.com/knowledgebase/articles/215522-step3-view-favorite-memories-on-tablets-and-tvs-");

				Settings.Default.IsFirstSelectStarred = true;
				Settings.Default.Save();
			}
		}

		public void StarContent(IEnumerable<IContentEntity> contentEntities)
		{
			IEnumerable<IContent> contents = contentEntities.OfType<IContent>();

			ClientFramework.Client.Default.Tag(contents);

			RefreshContentArea();
			RefreshStarFavorite();
		}

		private void RefreshFavorites()
		{
			foreach (var favorite in ClientFramework.Client.Default.Favorites.OfType<IContentGroup>())
			{
                RefreshFavorite(favorite);
			}
		}

		private void RefreshFavorite(IContentGroup favorite)
		{
			if (favorite == null)
				return;

			favorite.Refresh();
            (lbxCloudAlbums.ItemsSource as ICollectionView).Refresh();
            (lbxFavorites.ItemsSource as ICollectionView).Refresh();

            System.Windows.Forms.Application.DoEvents();

            lbxCloudAlbums.SelectedItem = lbxFavorites.SelectedItem = GetCurrentContentGroup();
		}

		private IContentGroup GetSelectedFavoriteGroup()
		{
			return lbxFavorites.SelectedItem as IContentGroup;
		}

		private void RefreshSelectedFavorite()
		{
			RefreshFavorite(GetSelectedFavoriteGroup());
		}

		private void RefreshContentArea()
		{
			var group = GetCurrentContentGroup();

			if (group == null)
				return;

			group.Refresh();

			SetContentTypeCount(group);
		}

		public bool AddToFavorite(IEnumerable<IContentEntity> contents)
		{
			if (!contents.Any())
			{
				string text = (string)Application.Current.FindResource("WithoutContentMessageText");

				MessageBox.Show(Application.Current.MainWindow, text);
				return false;
			}

			var favorites = ClientFramework.Client.Default.Favorites;

			if (!favorites.Any())
			{
				string text = (string)Application.Current.FindResource("NoExistingFavoriteMessageText");

				MessageBox.Show(Application.Current.MainWindow, text);
				return false;
			}

			var dialog = new AddToAlbumDialog
							 {
								 Owner = this,
								 WindowStartupLocation = WindowStartupLocation.CenterOwner,
								 ItemSource = favorites
							 };

			if (dialog.ShowDialog() != true)
				return false;

			var selectedFavorite = (dialog.SelectedItem as IContentGroup);
			AddToFavorite(selectedFavorite.ID, contents);

			return true;
		}

		private void AddToFavorite(string favoriteID, IEnumerable<IContentEntity> contents)
		{
			var favorites = ClientFramework.Client.Default.Favorites;

			if (!favorites.Any())
			{
				string text = (string)Application.Current.FindResource("NoExistingFavoriteMessageText");

				MessageBox.Show(Application.Current.MainWindow, text);
				return;
			}

			ClientFramework.Client.Default.AddToFavorite(contents, favoriteID);

			(GetFavorite(favoriteID) as IContentGroup).Refresh();
		}

		private void SelectToStarFavorite()
		{
			SelectToFavorite("00000000-0000-0000-0000-000000000000");
		}

		private void SelectToFavorite(string favoriteID)
		{
			var favorites = ClientFramework.Client.Default.Favorites;

			var index = 0;

			if (favoriteID == "00000000-0000-0000-0000-000000000000")
			{
				lbxRecent.SelectedIndex = 0;
			}
			else
			{
				foreach (var favorite in favorites)
				{
					if (favorite.ID != favoriteID)
					{
						++index;
						continue;
					}

					lbxFavorites.SelectedIndex = index;
				}
			}
		}

		private IContentEntity GetFavorite(string favoriteID)
		{
			var favorites = ClientFramework.Client.Default.Favorites;

			foreach (IContentEntity favorite in favorites)
			{
				if (favorite.ID == favoriteID)
					return favorite;
			}

			return null;
		}

		private IContentGroup GetCurrentContentGroup()
		{
			return lblContentLocation.DataContext as IContentGroup;
		}

		private IEnumerable<IContentEntity> GetSelectedContents()
		{
			return lbxContentContainer.SelectedItems.OfType<IContentEntity>().ToArray();
		}

		private void DeleteSelectedFolder()
		{
			var folder = lbxDeviceContainer.SelectedItem as IContentGroup;

			if (folder == null)
				return;

			if (MessageBox.Show(Application.Current.MainWindow, "Are you sure you want to delete?", "Confirm", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
				return;

			var service = folder.Service;
			ClientFramework.Client.Default.Delete(null, new[] { folder.Uri.LocalPath });
			RefreshFavorites();

			service.Refresh();

			EmptyContentArea();
		}

		private void EmptyContentArea()
		{
			lbxContentContainer.DataContext = null;
			lblContentLocation.DataContext = null;
		}

		private void DelectSelectedSourceContents()
		{
			var _selectedContents = GetSelectedContents();

			DelectSourceContents(_selectedContents);
		}

		private void DelectSourceContents(IEnumerable<IContentEntity> contents)
		{
			var contentIDs = contents.Select(content => content.ID);

			DeleteSourceContents(contentIDs, true);
		}

		public void DeleteSourceContents(IEnumerable<string> contentIDs, bool ask)
		{
			if (ask)
			{
				if (MessageBox.Show(Application.Current.MainWindow, "Are you sure you want to delete?", "Confirm", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
					return;
			}

			ClientFramework.Client.Default.Delete(contentIDs);
			RefreshContentArea();
			RefreshFavorites();
		}

		private void DeleteCurrentFavorite()
		{
			var group = GetCurrentContentGroup();
			ClientFramework.Client.Default.RemoveFavorite(group.ID);

			if (lbxFavorites.HasItems)
				lbxFavorites.SelectedIndex = 0;
			else
			{
                lbxCloudAlbums.SelectedIndex = -1;
				lbxFavorites.SelectedIndex = -1;
				lbxContentContainer.DataContext = null;
				lblContentLocation.DataContext = null;
				SetContentTypeCount(null);

				Grid.SetColumnSpan(gdContentArea, 2);

				gdRightSide.Visibility = Visibility.Collapsed;
				rspRightSidePanel.Visibility = Visibility.Collapsed;
				rspRightSidePane2.Visibility = Visibility.Collapsed;
			}
		}

		private void AddSelectedToFavorite()
		{
			var selectedContents = GetSelectedContents();

			AddToFavorite(selectedContents);
		}

		private void StarSelectedContents()
		{
			var selectedContents = GetSelectedContents();

			StarContent(selectedContents);
		}

		private void SaveSelectedContentsToFavorite()
		{
			IEnumerable<IContentEntity> selectedContents = GetSelectedContents();

			SaveToFavorite(selectedContents);
		}

		#endregion

		#region Public Method

		public bool SaveToFavorite(IEnumerable<IContentEntity> contents)
		{
			if (!contents.Any())
			{
				string text = (string)Application.Current.FindResource("WithoutContentMessageText");

				MessageBox.Show(Application.Current.MainWindow, text);
				return false;
			}

			string defaultName = (string)Application.Current.FindResource("DefaultFavoriteName");

			var dialog = new CreateAlbumDialog
							 {
								 DefaultName = defaultName,
								 Owner = this,
								 WindowStartupLocation = WindowStartupLocation.CenterOwner
							 };

			if (dialog.ShowDialog() != true)
				return false;

			SaveToFavorite(contents, dialog.CreateName);

			return true;
		}

		public void SaveToFavorite(IEnumerable<IContentEntity> contents, string name)
		{
			ClientFramework.Client.Default.SaveToFavorite(contents, name);
			lbxFavorites.SelectedIndex = lbxFavorites.Items.Count - 1;
		}

		public void TimelineShareTo(IEnumerable<IContentEntity> contents, string name)
		{
			SaveToFavorite(contents, name);

			timelineShareToDelayTimer = new DispatcherTimer();
			timelineShareToDelayTimer.Interval = new TimeSpan(0, 0, 1);
			timelineShareToDelayTimer.Tick += timelineShareToTimer_Tick;
			timelineShareToDelayTimer.Start();
		}

		private void timelineShareToTimer_Tick(object sender, EventArgs e)
		{
			timelineShareToDelayTimer.Stop();

			ToggleButtonAutomationPeer _peer = new ToggleButtonAutomationPeer(rspRightSidePane2.tbtnCloudSharing);
			IToggleProvider _toggleProvider = _peer.GetPattern(PatternInterface.Toggle) as IToggleProvider;
			_toggleProvider.Toggle();
		}

		#endregion

		private void recentTimer_Tick(object sender, EventArgs e)
		{
			ClientFramework.Client.Default.RefreshRecent(); // - ?

			foreach (IContentGroup _contentGroup in lbxRecent.Items.OfType<IContentGroup>())
			{
				_contentGroup.Refresh();
			}
		}

		/*
		private void lblHomeSharingTutorialTip_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Process.Start("http://waveface.uservoice.com/knowledgebase/articles/215523-step4-share-favorites-with-your-favorite-people");
		}
		*/

		private void uiDelayTimer_Tick(object sender, EventArgs e)
		{
			uiDelayTimer.Stop();

			if (Settings.Default.IsFirstUse)
			{
				/*
				MessageBoxResult _messageBoxResult = MessageBox.Show(Application.Current.MainWindow, "See a quick tour ?", "Favorite*", MessageBoxButton.YesNo, MessageBoxImage.Question,
																	 MessageBoxResult.Yes);

				if (_messageBoxResult == MessageBoxResult.Yes)
				{
					Process.Start(HELP_URL);
				}
				*/

				showWaitForPairingDialog();

				Settings.Default.IsFirstUse = false;
				Settings.Default.Save();
			}
		}

		private void showWaitForPairingDialog()
		{
			var dialog = new WaitForPairingDialog
							 {
								 Owner = this
							 };
			dialog.ShowDialog();


			if (dialog.PairedDevices.Any())
			{
				var device_id = dialog.PairedDevices.LastOrDefault().device_id;
				JumpToDevice(device_id);
			}
		}

		public void JumpToDevice(string device_id)
		{
			if (jumpToDeviceTimer != null)
			{
				jumpToDeviceTimer.Stop();
			}

			jumpToDeviceTimer = new DispatcherTimer();
			jumpToDeviceTimer.Interval = TimeSpan.FromMilliseconds(500);
			jumpToDeviceTimer.Tick += (s, e) => Timer_JumpToDevice(device_id);
			jumpToDeviceTimer.Start();
		}

		private void Timer_JumpToDevice(string device_id)
		{
			try
			{
				var sourceTree = lbxDeviceContainer;

				if (!sourceTree.HasItems)
					return;

				foreach (IService dev in sourceTree.Items)
				{
					if (dev.ID.Equals(device_id, StringComparison.InvariantCultureIgnoreCase))
					{
						var devNode = (TreeViewItem)sourceTree.ItemContainerGenerator.ContainerFromItem(dev);

						if (devNode != null && devNode.Items.Count > 0)
						{
							devNode.IsExpanded = true;

							var folderNode = (TreeViewItem)devNode.ItemContainerGenerator.ContainerFromIndex(0);

							if (folderNode != null)
							{
								folderNode.IsSelected = true;
								lbxRecent.SelectedIndex = -1;
                                lbxCloudAlbums.SelectedIndex = -1;
								lbxFavorites.SelectedIndex = -1;

								var group = (IContentGroup)devNode.Items[0];

								lbxContentContainer.DataContext = group.Contents;
								lbxContentContainer.ContextMenu = Resources["SourceContentContextMenu"] as ContextMenu;
								lblContentLocation.DataContext = group;

								SetContentTypeCount(group);

								jumpToDeviceTimer.Stop();
							}
						}
					}
				}
			}
			catch (Exception err)
			{
				LogManager.GetLogger(GetType()).Warn("Unable to jump to " + device_id, err);
			}
		}

		private void btnCopyShareLink_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText((lblContentLocation.DataContext as BunnyLabelContentGroup).ShareURL);
		}

		void btnOpenShareLink_Click(object sender, RoutedEventArgs e)
		{
			Process.Start((lblContentLocation.DataContext as BunnyLabelContentGroup).ShareURL);
		}

		private void tbtnCloudSharing_Checked(object sender, EventArgs e)
		{
			try
			{
				Mouse.OverrideCursor = Cursors.Wait;
				CloudSharing(rspRightSidePane2.tbtnCloudSharing.IsChecked.Value);
			}
			catch (Exception err)
			{
				Mouse.OverrideCursor = null;
				rspRightSidePane2.tbtnCloudSharing.IsChecked = !rspRightSidePane2.tbtnCloudSharing.IsChecked;
				MessageBox.Show(this, err.Message + "\r\n" + "Please check your network connection.", "Unable to open/close online album", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			finally
			{
				Mouse.OverrideCursor = null;
			}
		}

		private void btnClearAll_Click(object sender, RoutedEventArgs e)
		{
			ClientFramework.Client.Default.ClearTaggedContents();
			RefreshContentArea();
			RefreshSelectedFavorite();
			TryUpdateRightSidePanelContentCount();
		}

		private void tbxName_LostFocus(object sender, RoutedEventArgs e)
		{
			RenameFavorite();
		}

		private void tbxName_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;

			RenameFavorite();
		}

		private void RenameFavorite()
		{
			try
			{
				StationAPI.RenameLabel(GetSelectedFavoriteGroup().ID, rspRightSidePane2.tbxName.Text);
			}
			catch (Exception)
			{
			}
		}

		private void Enter()
		{
			var group = (lbxContentContainer.SelectedItem as IContentGroup);

			if (group == null)
			{
				var viewer = new PhotoViewer
								 {
									 Owner = this,
									 Source = (lbxContentContainer.SelectedItems.Count > 1) ? lbxContentContainer.SelectedItems.OfType<IContentEntity>() : lbxContentContainer.DataContext,
									 SelectedIndex = (lbxContentContainer.SelectedItems.Count > 1) ? 0 : lbxContentContainer.SelectedIndex
								 };

				viewer.ShowDialog();

				RefreshContentArea();
				RefreshFavorites();

				return;
			}

			lblContentLocation.DataContext = group;
			lbxContentContainer.DataContext = null;

			DoEvents();

			lbxContentContainer.DataContext = group.Contents;
			lbxContentContainer.SelectedIndex = -1;
			SetContentTypeCount(group);
		}

		private void SetContentTypeCount(IContentGroup group)
		{
			if (group == null)
				lblContentTypeCount.Content = "";
			else
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

		private void BackButton_Click(object sender, RoutedEventArgs e)
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
				lbxContentContainer.DataContext = null;

				DoEvents();

				lbxContentContainer.DataContext = service.Contents;
				lbxContentContainer.SelectedIndex = -1;
				return;
			}

			lblContentLocation.DataContext = group;
			lbxContentContainer.DataContext = null;

			DoEvents();

			lbxContentContainer.DataContext = (group as IContentGroup).Contents;
			lbxContentContainer.SelectedIndex = -1;

			SetContentTypeCount(group as IContentGroup);
		}

        private DependencyObject GetControlItem(ItemsControl itemsControl, object value)
        {
            foreach (var item in itemsControl.Items)
            {
                var dp = itemsControl.ItemContainerGenerator.ContainerFromItem(value);

                if (dp != null)
                    return dp;

                var currentTreeViewItem = itemsControl.ItemContainerGenerator.ContainerFromItem(item);

                var childDp = GetControlItem(currentTreeViewItem as ItemsControl, value);

                if (childDp != null)
                    return childDp;
            }
            return null;
        }

        private void TreeViewItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
            var treeView = sender as TreeView;
            var ti = GetControlItem(treeView, treeView.SelectedItem) as TreeViewItem;

			if (ti == null)
				return;

            lbxContentContainer.SelectedIndex = -1;
            lbxCloudAlbums.SelectedIndex = -1;
            lbxFavorites.SelectedIndex = -1;
            lbxRecent.SelectedIndex = -1;

			var group = ti.DataContext as IContentGroup;

			if (group == null)
			{
				IService _service = ti.DataContext as IService;

				if (_service == null)
					return;

				Cursor = Cursors.Wait;

				ContentAreaToolBar.Visibility = Visibility.Collapsed;
				lbxContentContainer.Visibility = Visibility.Collapsed;

				allFilesUC.Visibility = Visibility.Visible;
				allFilesUC.Load(_service, this);

				Cursor = Cursors.Arrow;

				return;
			}

			group.Refresh();

			allFilesUC.Stop();
			allFilesUC.Visibility = Visibility.Collapsed;
			ContentAreaToolBar.Visibility = Visibility.Visible;
			lbxContentContainer.Visibility = Visibility.Visible;

			lbxContentContainer.ContextMenu = Resources["SourceContentContextMenu"] as ContextMenu;
			//btnDelete.IsEnabled = false;
			//btnCreateAlbum.IsEnabled = false;

			Grid.SetColumnSpan(gdContentArea, 2);

			gdRightSide.Visibility = Visibility.Collapsed;

			rspRightSidePanel.Visibility = Visibility.Collapsed;
			rspRightSidePane2.Visibility = Visibility.Collapsed;

			lblContentLocation.DataContext = group;
			lbxContentContainer.DataContext = null;

			DoEvents();

			lbxContentContainer.DataContext = group.Contents;

			SetContentTypeCount(group);

			GC.Collect();
		}

    //private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, EventArgs e)
    //    {
    //        var ti = sender as TreeViewItem;

    //        if (ti == null)
    //            return;

    //        lbxContentContainer.SelectedIndex = -1;
    //        lbxCloudAlbums.SelectedIndex = -1;
    //        lbxFavorites.SelectedIndex = -1;
    //        lbxRecent.SelectedIndex = -1;

    //        var group = ti.DataContext as IContentGroup;

    //        if (group == null)
    //        {
    //            IService _service = ti.DataContext as IService;

    //            if (_service == null)
    //                return;

    //            Cursor = Cursors.Wait;

    //            ContentAreaToolBar.Visibility = Visibility.Collapsed;
    //            lbxContentContainer.Visibility = Visibility.Collapsed;

    //            allFilesUC.Visibility = Visibility.Visible;
    //            allFilesUC.Load(_service, this);

    //            Cursor = Cursors.Arrow;

    //            return;
    //        }

    //        group.Refresh();

    //        allFilesUC.Stop();
    //        allFilesUC.Visibility = Visibility.Collapsed;
    //        ContentAreaToolBar.Visibility = Visibility.Visible;
    //        lbxContentContainer.Visibility = Visibility.Visible;

    //        lbxContentContainer.ContextMenu = Resources["SourceContentContextMenu"] as ContextMenu;
    //        //btnDelete.IsEnabled = false;
    //        //btnCreateAlbum.IsEnabled = false;

    //        Grid.SetColumnSpan(gdContentArea, 2);

    //        gdRightSide.Visibility = Visibility.Collapsed;

    //        rspRightSidePanel.Visibility = Visibility.Collapsed;
    //        rspRightSidePane2.Visibility = Visibility.Collapsed;

    //        lblContentLocation.DataContext = group;
    //        lbxContentContainer.DataContext = null;

    //        DoEvents();

    //        lbxContentContainer.DataContext = group.Contents;

    //        SetContentTypeCount(group);

    //        GC.Collect();
    //    }

		private void lbxContentContainer_KeyDown(object sender, KeyEventArgs e)
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

		private void lbxFavorites_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            var listbox = sender as ListBox;
            if (listbox.SelectedIndex >= 0)
			{
				ShowSelectedFavoriteContents(sender, false);

                ((listbox == lbxCloudAlbums) ? lbxFavorites : lbxCloudAlbums).SelectedItem = null;
				lbxRecent.SelectedItem = null;
			}
		}

		private void lbxRecent_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (lbxRecent.SelectedIndex >= 0)
			{
				ShowSelectedFavoriteContents(sender, true);

                lbxCloudAlbums.SelectedItem = null;
				lbxFavorites.SelectedItem = null;
			}
		}

		private void ShowSelectedFavoriteContents(object sender, bool isRecent)
		{
			var listBox = sender as ListBox;

			if (listBox == null)
				return;

			var group = listBox.SelectedItem as IContentGroup;

			if (group == null)
				return;

			lblContentLocation.DataContext = group;

			if (group.ID.Equals(ClientFramework.Client.StarredLabelId, StringComparison.CurrentCultureIgnoreCase))
			{
				TryDisplayStarredTutorial();
			}
			else
			{
				TryDisplayFavoriteTutorial();

				updateRightSidePanel2(group);
			}

			SetContentTypeCount(group);

			lbxContentContainer.ContextMenu = Resources["ContentContextMenu"] as ContextMenu;
			lbxContentContainer.ContextMenu.IsOpen = false;
			lbxContentContainer.ContextMenu.Visibility = Visibility.Visible;
			//btnDelete.IsEnabled = false;

			gdRightSide.Visibility = Visibility.Visible;
			Grid.SetColumnSpan(gdContentArea, 1);

			ContentAreaToolBar.Visibility = Visibility.Visible;
			lbxContentContainer.Visibility = Visibility.Visible;
			allFilesUC.Visibility = Visibility.Collapsed;

			bool _isStarredLabel = group.ID.Equals(ClientFramework.Client.StarredLabelId, StringComparison.CurrentCultureIgnoreCase);

			rspRightSidePane2.Visibility = _isStarredLabel ? Visibility.Collapsed : Visibility.Visible;
			rspRightSidePanel.Visibility = _isStarredLabel ? Visibility.Visible : Visibility.Collapsed;

			if (isRecent && !_isStarredLabel)
			{
				gdRightSide.Visibility = Visibility.Collapsed;
				Grid.SetColumnSpan(gdContentArea, 3);
			}

			if (lbxDeviceContainer.SelectedItem != null)
			{
				lbxDeviceContainer.ClearSelection();
			}

			lbxContentContainer.DataContext = null;

			DoEvents();

			lbxContentContainer.DataContext = group.Contents;
			lbxContentContainer.SelectedIndex = -1;

			TryUpdateRightSidePanelContentCount();
		}

		private void TryUpdateRightSidePanelContentCount()
		{
			if (rspRightSidePanel.Visibility == Visibility.Visible)
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

		private void rspRightSidePanel_SaveToFavorite(object sender, EventArgs e)
		{
			SaveToFavorite(lbxContentContainer.Items.OfType<IContentEntity>());
		}

		/*
		// XAML: OnAirClick="rspRightSidePane2_OnAirClick"
		//
		private void rspRightSidePane2_OnAirClick(object sender, EventArgs e)
		{
			ClientFramework.Client.Default.OnAir((lblContentLocation.DataContext as IContentEntity).ID, rspRightSidePane2.tbtnHomeSharing.IsChecked.Value);
		}
		*/

		private void updateRightSidePanel2(IContentGroup group)
		{
			//var isOnAir = ClientFramework.Client.Default.IsOnAir(group);
			//rspRightSidePane2.tbtnHomeSharing.IsEnabled = ClientFramework.Client.Default.HomeSharingEnabled;
			//rspRightSidePane2.tbtnHomeSharing.IsChecked = isOnAir;

            rspRightSidePane2.DataContext = group;

			rspRightSidePane2.tbtnCloudSharing.IsChecked = (group as BunnyLabelContentGroup).ShareEnabled;

			rspRightSidePane2.Update(lblContentLocation.DataContext as BunnyLabelContentGroup);
		}

		private void rspRightSidePanel_AddToFavorite(object sender, EventArgs e)
		{
			AddToFavorite(lbxContentContainer.Items.OfType<IContentEntity>());
		}

		public void CloudSharing(bool isShared)
		{
			BunnyLabelContentGroup labelGroup = lblContentLocation.DataContext as BunnyLabelContentGroup;

			ClientFramework.Client.Default.ShareLabel(labelGroup.ID, isShared);

            RefreshFavorites();

            labelGroup.RefreshShareProperties();

            rspRightSidePane2.Update(labelGroup);
		}

		public void EmailCloudSharing()
		{
			Wpf_testHTTP.MainWindow _w = new Wpf_testHTTP.MainWindow();
			var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny");

			var iniFile = Path.Combine(path, @"sharefavorite.ini");

			var group = GetCurrentContentGroup();
			_w.setTitle(@group.Name);
			_w.setiniPath(iniFile);

			var files = string.Join("~", (lbxContentContainer.DataContext as IEnumerable<IContentEntity>).Select(content => content.Uri.LocalPath).ToArray());

			_w.setFilename(files);
			_w.setLabelId(@group.ID);
			_w.setRun();
			_w.ShowDialog();
		}

		private void rspRightSidePane2_CloudSharingClick(object sender, EventArgs e)
		{
			EmailCloudSharing();
		}

		private void rspRightSidePane2_DeleteButtonClick(object sender, EventArgs e)
		{
			DeleteCurrentFavorite();
		}

		private void StarMenuItem_Click(object sender, RoutedEventArgs e)
		{
			StarSelectedContents();
		}

		private void CreateFavoriteMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SaveSelectedContentsToFavorite();
		}

		private void AddToFavoriteMenuItem_Click(object sender, RoutedEventArgs e)
		{
			AddSelectedToFavorite();
		}

		private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
		{
			DelectSelectedSourceContents();
		}

		private void UnTagMenuItem_Click(object sender, RoutedEventArgs e)
		{
			UnTag();
		}

		private void UnTag()
		{
			var group = GetCurrentContentGroup();
			var selectedContents = GetSelectedContents();

			foreach (var content in selectedContents)
				ClientFramework.Client.Default.UnTag(@group.ID, content.ID);

			RefreshContentArea();
		}

		private void lblContentLocation_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (lblContentLocation.DataContext == null)
			{
				btnBack.Visibility = Visibility.Collapsed;
				return;
			}

			btnBack.Visibility = GetCurrentContentGroup().Parent == null
									 ? Visibility.Collapsed
									 : Visibility.Visible;
		}

		private void btnAddNewSource_Click(object sender, RoutedEventArgs e)
		{
			showWaitForPairingDialog();
		}

		private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var now = DateTime.Now;
			ListBoxItem item = FindAnchestor<ListBoxItem>((DependencyObject)e.OriginalSource);

			var dataContext = item.DataContext;

			if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && lbxContentContainer.SelectedItems.Contains(dataContext))
			{
				needSpecialMulitSelectProcess = true;

				e.Handled = lbxContentContainer.SelectedItems.Count > 1;

				if (now.Subtract(lastMouseLeftButtonDown).TotalMilliseconds <= 500)
					Enter();
			}

			startPoint = e.GetPosition(null);

			lastMouseLeftButtonDown = now;
		}

		private void lbxContentContainer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (!needSpecialMulitSelectProcess)
				return;

			ListBoxItem item = FindAnchestor<ListBoxItem>((DependencyObject)e.OriginalSource);

			var dataContext = item.DataContext;

			if (!(lbxContentContainer.SelectedItems.Count == 1 && lbxContentContainer.SelectedItem == dataContext))
			{
				lbxContentContainer.UnselectAll();
				lbxContentContainer.SelectedItem = dataContext;

				try
				{
					PropertyInfo pi = typeof(ListBox).GetProperty("AnchorItem", BindingFlags.NonPublic | BindingFlags.Instance);

					if (pi != null)
					{
						pi.SetValue(lbxContentContainer, dataContext, null);
					}
				}
				catch (Exception)
				{
				}
			}

			needSpecialMulitSelectProcess = false;

			e.Handled = true;
		}

		private void List_MouseMove(object sender, MouseEventArgs e)
		{
			// Get the current mouse position
			Point mousePos = e.GetPosition(null);
			Vector diff = startPoint - mousePos;

			if (e.LeftButton == MouseButtonState.Pressed &&
				(Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
				 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
			{
				IEnumerable<IContentEntity> _contents = GetSelectedContents();

				List<string> _filePaths = new List<string>();

				foreach (IContentEntity _entity in _contents)
				{
					_filePaths.Add(_entity.Uri.LocalPath);
				}

				// Initialize the drag & drop operation
				DataObject _dragData = new DataObject();
				_dragData.SetData(typeof(IEnumerable<IContentEntity>), _contents);
				_dragData.SetData(DataFormats.FileDrop, _filePaths.ToArray());
				DragDrop.DoDragDrop(this, _dragData, DragDropEffects.Copy);
			}
		}

		// Helper to search up the VisualTree
		private static T FindAnchestor<T>(DependencyObject current)
			where T : DependencyObject
		{
			do
			{
				if (current is T)
				{
					return (T)current;
				}
				current = VisualTreeHelper.GetParent(current);
			} while (current != null);

			return null;
		}

		private void Sources_DragOver(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(typeof(IEnumerable<IContentEntity>)) || lbxFavorites.SelectedItem != null)
			{
				e.Effects = DragDropEffects.None;
				e.Handled = true;
				return;
			}

			var controlItem = FindAnchestor<TreeViewItem>((DependencyObject)e.OriginalSource);

			if (controlItem == null)
				return;

			var ctx = controlItem.DataContext as IContentGroup;

			if (ctx == null || ctx.ID == "Unsorted")
			{
				e.Effects = DragDropEffects.None;
				e.Handled = true;
				return;
			}

			var contents = e.Data.GetData(typeof(IEnumerable<IContentEntity>)) as IEnumerable<IContentEntity>;

			if (contents.Any() && contents.First().Service != ctx.Service)
			{
				e.Effects = DragDropEffects.None;
				e.Handled = true;
			}
		}

		private void Sources_Drop(object sender, DragEventArgs e)
		{
			if (e.Effects == DragDropEffects.Copy && e.Data.GetDataPresent(typeof(IEnumerable<IContentEntity>)))
			{
				var controlItem = FindAnchestor<TreeViewItem>((DependencyObject)e.OriginalSource);

				if (controlItem == null)
					return;

				var sourceGroup = controlItem.DataContext as IContentGroup;
				var contents = e.Data.GetData(typeof(IEnumerable<IContentEntity>)) as IEnumerable<IContentEntity>;

				MoveToFolder(sourceGroup, contents);
			}
		}

		private void MoveToFolder(IContentGroup targetGroup, IEnumerable<IContentEntity> contents)
		{
			MoveToFolder(targetGroup.Uri.LocalPath, contents);
			targetGroup.Refresh();
		}

		private void MoveToFolder(string targetFullPath, IEnumerable<IContentEntity> contents)
		{
			if (contents.Any())
			{
				var service = contents.First().Service;

				ClientFramework.Client.Default.Move(contents.Select(x => x.ID), targetFullPath);

				RefreshContentArea();

				service.Refresh();
			}
		}

		private void Favorites_DragEnter(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(typeof(IEnumerable<IContentEntity>)) || sender == e.Source)
			{
				e.Effects = DragDropEffects.None;
			}
		}

		private void Favorites_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(IEnumerable<IContentEntity>)))
			{
				var controlItem = FindAnchestor<ListBoxItem>((DependencyObject)e.OriginalSource);

				if (controlItem == null)
					return;

				var contents = e.Data.GetData(typeof(IEnumerable<IContentEntity>)) as IEnumerable<IContentEntity>;
				var favoriteGroup = controlItem.DataContext as IContentGroup;

				if (!favoriteGroup.ID.Equals("00000000-0000-0000-0000-000000000000", StringComparison.CurrentCultureIgnoreCase))
				{
					AddToFavorite(favoriteGroup.ID, contents);
				}
			}
		}

		private void Recent_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(IEnumerable<IContentEntity>)))
			{
				var controlItem = FindAnchestor<ListBoxItem>((DependencyObject)e.OriginalSource);

				if (controlItem == null)
					return;

				var contents = e.Data.GetData(typeof(IEnumerable<IContentEntity>)) as IEnumerable<IContentEntity>;
				var favoriteGroup = controlItem.DataContext as IContentGroup;

				if (favoriteGroup.ID.Equals("00000000-0000-0000-0000-000000000000", StringComparison.CurrentCultureIgnoreCase))
				{
					StarContent(contents);
					SelectToStarFavorite();
				}
			}
		}

		private void lbxFavorites_DeleteFavoriteInvoked(object sender, EventArgs e)
		{
			DeleteCurrentFavorite();
		}

		private void lbxDeviceContainer_DeleteSourceInvoked(object sender, EventArgs e)
		{
			DeleteSelectedFolder();
		}

		private void lbxDeviceContainer_CreateFavoriteInvoked(object sender, EventArgs e)
		{
			SaveToFavorite(lbxContentContainer.Items.OfType<IContentEntity>());
		}

		private void lbxDeviceContainer_AddToFavoriteInvoked(object sender, EventArgs e)
		{
			AddToFavorite(lbxContentContainer.Items.OfType<IContentEntity>());
		}

		private void lbxDeviceContainer_StarInvoked(object sender, EventArgs e)
		{
			StarContent(lbxContentContainer.Items.OfType<IContentEntity>());
		}

		public void MoveToExistingFolder(IEnumerable<IContentEntity> contents)
		{
			var _dialog = new MoveToFolderDialog
							  {
								  Owner = this,
								  WindowStartupLocation = WindowStartupLocation.CenterOwner
							  };

			var _currentGroup = GetCurrentContentGroup();

			_dialog.ItemSource = _currentGroup.Service.Contents.Skip(1).Except(new IContentEntity[] { _currentGroup });

			if (_dialog.ShowDialog() != true)
				return;

			var _selectedGroup = (_dialog.SelectedItem as IContentGroup);
			MoveToFolder(_selectedGroup, contents);
		}

		private void svContentContainer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			var scrollViewer = (FrameworkElement)sender;
			var visibleAreaEntered = false;
			var visibleAreaLeft = false;
			var invisibleItemDisplayed = 0;

			foreach (var item in lbxContentContainer.Items)
			{
				var listBoxItem = (FrameworkElement)lbxContentContainer.ItemContainerGenerator.ContainerFromItem(item);

				if (visibleAreaLeft == false && IsFullyOrPartiallyVisible(listBoxItem, scrollViewer))
				{
					visibleAreaEntered = true;
				}
				else if (visibleAreaEntered)
				{
					visibleAreaLeft = true;
				}

				if (visibleAreaEntered)
				{
					if (visibleAreaLeft && ++invisibleItemDisplayed > 10)
						break;

					ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(listBoxItem);

					var control = GetVisualChild<WrapperControl>(contentPresenter);
					control.Display();
				}
			}
		}

		protected bool IsFullyOrPartiallyVisible(FrameworkElement child, FrameworkElement scrollViewer)
		{
			var childTransform = child.TransformToAncestor(scrollViewer);
			var childRectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), child.RenderSize));
			var ownerRectangle = new Rect(new Point(0, 0), scrollViewer.RenderSize);
			return ownerRectangle.IntersectsWith(childRectangle);
		}

		private T GetVisualChild<T>(DependencyObject parent) where T : Visual
		{
			T child = default(T);
			int numVisuals = VisualTreeHelper.GetChildrenCount(parent);

			for (int i = 0; i < numVisuals; i++)
			{
				Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
				child = v as T;

				if (child == null)
				{
					child = GetVisualChild<T>(v);
				}
				if (child != null)
				{
					break;
				}
			}

			return child;
		}

		private ChildControl FindVisualChild<ChildControl>(DependencyObject DependencyObj)
			where ChildControl : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(DependencyObj); i++)
			{
				DependencyObject Child = VisualTreeHelper.GetChild(DependencyObj, i);

				if (Child != null && Child is ChildControl)
				{
					return (ChildControl)Child;
				}
				else
				{
					ChildControl ChildOfChild = FindVisualChild<ChildControl>(Child);

					if (ChildOfChild != null)
					{
						return ChildOfChild;
					}
				}
			}

			return null;
		}

		private void btnHelpPanelClose_Click(object sender, RoutedEventArgs e)
		{
		}

		#region DoEvents

		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void DoEvents()
		{
			var _frame = new DispatcherFrame();

			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), _frame);

			Dispatcher.PushFrame(_frame);
		}

		public object ExitFrame(object f)
		{
			((DispatcherFrame)f).Continue = false;

			return null;
		}

		#endregion

		private IEnumerable<IContentEntity> GetContents()
		{
			IEnumerable<IContentEntity> _entities = GetSelectedContents();

			if (!_entities.Any())
			{
				_entities = lbxContentContainer.Items.OfType<IContentEntity>().ToArray(); //Get All
			}

			return _entities;
		}

		private void btnCreateAlbum_Click(object sender, RoutedEventArgs e)
		{
			IEnumerable<IContentEntity> _allEntities = lbxContentContainer.Items.OfType<IContentEntity>().ToArray();
			IEnumerable<IContentEntity> _selectedEntities = GetSelectedContents();

			string _title = lblContentLocation.Content.ToString();

			CloudSharingDialog _dialog = new CloudSharingDialog(_allEntities, _selectedEntities, _title)
			{
				Owner = this
			};
			_dialog.Title = (string)this.FindResource("createAlbumDialogTitle");
			_dialog.ShowDialog();

			List<string> _fileIDs = _dialog.FileIDs;
			_title = _dialog.TitleName;

			foreach (IContentGroup _group in ClientFramework.Client.Default.GetFavorites(true))
			{
				if (_group.Name == _title)
				{
					_title += " (1) ";
					break;
				}
			}

			_dialog = null;

			if ((_fileIDs != null) && (_title != string.Empty))
			{
				List<Content> _contents = new List<Content>();

				foreach (string _fileID in _fileIDs)
				{
					_contents.Add(new Content { ID = _fileID, });
				}

				if (_contents.Count > 0)
				{
					SaveToFavorite(_contents, _title);
				}
			}
		}

		private void btnCreateCloudAlbum_Click(object sender, RoutedEventArgs e)
		{

			IEnumerable<IContentEntity> _allEntities = lbxContentContainer.Items.OfType<IContentEntity>().ToArray();
			IEnumerable<IContentEntity> _selectedEntities = GetSelectedContents();

			string _title = lblContentLocation.Content.ToString();

			CloudSharingDialog _dialog = new CloudSharingDialog(_allEntities, _selectedEntities, _title)
											 {
												 Owner = this
											 };
			_dialog.ShowDialog();

			List<string> _fileIDs = _dialog.FileIDs;
			_title = _dialog.TitleName;

			foreach (IContentGroup _group in ClientFramework.Client.Default.GetFavorites(true))
			{
				if (_group.Name == _title)
				{
					_title += " [" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]";
					break;
				}
			}

			_dialog = null;

			if ((_fileIDs != null) && (_title != string.Empty))
			{
				List<Content> _contents = new List<Content>();

				foreach (string _fileID in _fileIDs)
				{
					_contents.Add(new Content { ID = _fileID, });
				}

				if (_contents.Count > 0)
				{
					TimelineShareTo(_contents, _title);
				}
			}
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			switch (lbxContentContainer.ContextMenu.Name)
			{
				case "SourceContentContextMenu":
					DelectSelectedSourceContents();
					break;

				case "ContentContextMenu":
					UnTag();
					break;
			}
		}

		private void ShowToolBarButtons(bool flag)
		{
			zoomPanel.Visibility = Visibility.Visible;
			btnDelete.Visibility = Visibility.Visible;

			if (flag)
			{
				middleBtnPanel.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{
				middleBtnPanel.Visibility = Visibility.Collapsed;
				helpPanel.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		private void ShowContextMenu(UIElement source, ContextMenu cm)
		{
			cm.IsEnabled = true;
			cm.IsOpen = true;
			cm.PlacementTarget = source;
			cm.Placement = PlacementMode.Top;
			cm.VerticalOffset = -3;

			var horizontalOffset = (source.TransformToAncestor(this).Transform(new Point(0d, 0d)).X + cm.ActualWidth > ActualWidth)
									   ? (ActualWidth - cm.ActualWidth) - source.TransformToAncestor(this).Transform(new Point(0d, 0d)).X
									   : 0;

			cm.HorizontalOffset = horizontalOffset;
		}

		private void miAddToFavorite_Click(object sender, RoutedEventArgs e)
		{
			IEnumerable<IContentEntity> _entities = GetContents();

			if (!_entities.Any())
			{
				return;
			}

			AddToFavorite(_entities);
		}

		private void miAddToStarred_Click(object sender, RoutedEventArgs e)
		{
			IEnumerable<IContentEntity> _entities = GetContents();

			if (!_entities.Any())
			{
				return;
			}

			StarContent(_entities);
		}

		private void miAddtoNewFavorite_Click(object sender, RoutedEventArgs e)
		{
			IEnumerable<IContentEntity> _entities = GetContents();

			if (!_entities.Any())
			{
				return;
			}

			SaveToFavorite(_entities);
		}

		private void miLocateOnDisk_Click(object sender, RoutedEventArgs e)
		{
			var selectedContents = GetSelectedContents();

			if (selectedContents.Count() > 0)
			{
				IContentEntity _entity = selectedContents.ElementAt(0);
				string _dir = _entity.Uri.LocalPath;
				string _arg = @"/select, " + _dir;
				Process.Start("explorer.exe", _arg);
			}
		}

		private void lbxContentContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//btnDelete.IsEnabled = lbxContentContainer.SelectedItems.Count != 0;

			if (lbxContentContainer.SelectedItems.Count == 0)
				selectionText.Content = "";
			else
				selectionText.Content = string.Format((string)lbxContentContainer.FindResource("selection_text"), lbxContentContainer.SelectedItems.Count);
		}

		private void lbxContentContainer_MouseDown(object sender, MouseButtonEventArgs e)
		{
			lbxContentContainer.UnselectAll();
		}

		private void GettingStarted_Tip1Clicked(object sender, MouseButtonEventArgs e)
		{
			showWaitForPairingDialog();
		}

		private void GettingStarted_Tip3Clicked(object sender, MouseButtonEventArgs e)
		{
			Process.Start("https://play.google.com/store/apps/details?id=com.waveface.favoriteplayer");
		}

		private void btnAddToAlbum_Click_1(object sender, RoutedEventArgs e)
		{
			addToAlbumPopup.IsOpen = !addToAlbumPopup.IsOpen;

			if (addToAlbumPopup.IsOpen)
			{
				var albums = new List<TestData>() {
								new TestData { 
									IsAddToNewAlbum = true,
									AlbumName = "New Album",
									Image = BitmapFrame.Create(new Uri("pack://application:,,,/Resource/bar2_source_0.png"))
								}
				};

				albums.AddRange(
					ClientFramework.Client.Default.Favorites.Select(x =>
					{
						var firstPic = (x as IContentGroup).Contents.FirstOrDefault() as BunnyContent;

						return new TestData
						{
							AlbumID = x.ID,
							AlbumName = x.Name,
							Image = (firstPic != null) ? firstPic.ImageSource : null
						};
					}).OrderBy(x => x.AlbumName));

				addToAlbumPopup.DataContext = albums;		  
			}
		}
	}
}
