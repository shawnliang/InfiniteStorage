using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Waveface.ClientFramework;
using Waveface.Model;

namespace Waveface.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string HELP_URL = "http://waveface.uservoice.com/knowledgebase/articles/214932-step1-import-photos-videos-from-your-phone";
        
        private DispatcherTimer uiDelayTimer;

        public MainWindow()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
        }

		#region Private Method 
		private void RefreshStarFavorite()
		{
			RefreshFavorite(lbxFavorites.Items.OfType<IContentGroup>().FirstOrDefault());
		}

		private void TryDisplayUnsortedTutorial()
		{
			if (!Properties.Settings.Default.IsFirstSelectUnsorted)
			{
				var result = TakeTourDialog.ShowWithDynamicResource("TakeTourMsgOranize", this);

				if (result.HasValue && result.Value)
					Process.Start(@"http://waveface.uservoice.com/knowledgebase/articles/215521-step2-organizing-photos-and-videos-in-favorite-");


				Properties.Settings.Default.IsFirstSelectUnsorted = true;
				Properties.Settings.Default.Save();
			}
		}

		private void TryDisplayFavoriteTutorial()
		{
			if (!Properties.Settings.Default.IsFirstSelectFavorite)
			{
				var result = TakeTourDialog.ShowWithDynamicResource("TakeTourMsgShare", this);
				if (result.HasValue && result.Value)
					Process.Start(@"http://waveface.uservoice.com/knowledgebase/articles/215523-step4-share-favorites-with-your-favorite-people");

				Properties.Settings.Default.IsFirstSelectFavorite = true;
				Properties.Settings.Default.Save();
			}
		}

		private void TryDisplayStarredTutorial()
		{
			if (!Properties.Settings.Default.IsFirstSelectStarred)
			{
				var result = TakeTourDialog.ShowWithDynamicResource("TakeTourMSgHomeShare", this);
				if (result.HasValue && result.Value)
					Process.Start(@"http://waveface.uservoice.com/knowledgebase/articles/215522-step3-view-favorite-memories-on-tablets-and-tvs-");

				Properties.Settings.Default.IsFirstSelectStarred = true;
				Properties.Settings.Default.Save();
			}
		}


		public void StarContent(IEnumerable<IContentEntity> contentEntities)
		{
            var contents = contentEntities.OfType<IContent>();

            ClientFramework.Client.Default.Tag(contents);

			RefreshContentArea();
			RefreshStarFavorite();
		}


		private void RefreshFavorites()
		{
			foreach (var favorite in Waveface.ClientFramework.Client.Default.Favorites.OfType<IContentGroup>())
			{
				favorite.Refresh();
			}
		}


		private void RefreshFavorite(IContentGroup favorite)
		{
			if (favorite == null)
				return;
			favorite.Refresh();
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

			var favorites = Waveface.ClientFramework.Client.Default.Favorites.Skip(1);

			if (!favorites.Any())
			{
				string text = (string)Application.Current.FindResource("NoExistingFavoriteMessageText");

				MessageBox.Show(Application.Current.MainWindow, text);
				return false;
			}


			var dialog = new AddToDialog();
			dialog.Owner = this;
			dialog.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

			dialog.ItemSource = favorites;

			if (dialog.ShowDialog() != true)
				return false;

			var selectedFavorite = (dialog.SelectedItem as IContentGroup);
			AddToFavorite(selectedFavorite.ID, contents);

			return true;
		}

		private void AddToFavorite(string favoriteID, IEnumerable<IContentEntity> contents)
		{
			var favorites = Waveface.ClientFramework.Client.Default.Favorites.Skip(1);

			if (!favorites.Any())
			{
				string text = (string)Application.Current.FindResource("NoExistingFavoriteMessageText");

				MessageBox.Show(Application.Current.MainWindow, text);
				return;
			}

			ClientFramework.Client.Default.AddToFavorite(contents, favoriteID);

            (GetFavorite(favoriteID) as IContentGroup).Refresh();
            //SelectToFavorite(favoriteID);
            //RefreshSelectedFavorite();
		}


		private void SelectToStarFavorite()
		{
			SelectToFavorite("00000000-0000-0000-0000-000000000000");
		}

		private void SelectToFavorite(string favoriteID)
		{
			var favorites = Waveface.ClientFramework.Client.Default.Favorites;

			var index = 0;
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


        private IContentEntity GetFavorite(string favoriteID)
        {
            var favorites = Waveface.ClientFramework.Client.Default.Favorites;

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
			var group = GetCurrentContentGroup();
			return lbxContentContainer.SelectedItems.OfType<IContentEntity>().ToArray();
		}


		private void DelectSelectedFolder()
		{
			var folder = lbxDeviceContainer.SelectedItem as IContentGroup;

			if (folder == null)
				return;

			if (MessageBox.Show(Application.Current.MainWindow, "Are you sure you want to delete?", "Confirm", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
				return;

			var service = folder.Service;
			Waveface.ClientFramework.Client.Default.Delete(null, new string[] { folder.Uri.LocalPath });
			RefreshFavorites();

			service.Refresh();

			var contents = service.Contents;

			EmptyContentArea();
		}

		private void EmptyContentArea()
		{
			lbxContentContainer.DataContext = null;
			lblContentLocation.DataContext = null;
		}

		private void DelectSelectedContents()
		{
			var selectedContents = GetSelectedContents();

			DelectContents(selectedContents);
		}

		private void DelectContents(IEnumerable<IContentEntity> contents)
		{
			var contentIDs = contents.Select(content => content.ID);

			DeleteContents(contentIDs);
		}

		private void DeleteContents(IEnumerable<string> contentIDs)
		{
			if (MessageBox.Show(Application.Current.MainWindow, "Are you sure you want to delete?", "Confirm", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
				return;

			Waveface.ClientFramework.Client.Default.Delete(contentIDs);
			RefreshContentArea();
			RefreshFavorites();
		}

		private void DeleteCurrentFavorite()
		{
			var group = GetCurrentContentGroup();
			ClientFramework.Client.Default.RemoveFavorite(group.ID);

			lbxFavorites.SelectedIndex = 0;
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
            var selectedContents = GetSelectedContents();

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

			var dialog = new CreateDialog();
            dialog.DefaultName = defaultName;
			dialog.Owner = this;
			dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;

			if (dialog.ShowDialog() != true)
				return false;

			ClientFramework.Client.Default.SaveToFavorite(contents, dialog.CreateName);
			lbxFavorites.SelectedIndex = lbxFavorites.Items.Count - 1;

			return true;
		}
		#endregion


		private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.lbxDeviceContainer.DataContext = Waveface.ClientFramework.Client.Default.Services;

            this.lbxFavorites.DataContext = Waveface.ClientFramework.Client.Default.Favorites;

            rspRightSidePane2.tbxName.KeyDown += tbxName_KeyDown;
            rspRightSidePane2.tbxName.LostFocus += tbxName_LostFocus;

			rspRightSidePane2.tbtnHomeSharing.Checked += rspRightSidePane2_OnAirClick;
			rspRightSidePane2.tbtnHomeSharing.Unchecked += rspRightSidePane2_OnAirClick;

			rspRightSidePane2.tbtnCloudSharing.Checked += tbtnCloudSharing_Checked;
			rspRightSidePane2.tbtnCloudSharing.Unchecked += tbtnCloudSharing_Checked;
            rspRightSidePane2.btnCopyShareLink.Click += btnCopyShareLink_Click;

            rspRightSidePanel.btnClearAll.Click += new RoutedEventHandler(btnClearAll_Click);

			rspRightSidePane2.lblHomeSharingTutorialTip.MouseDown += lblHomeSharingTutorialTip_MouseDown;

            lblContentTypeCount.Content = string.Format("0 photos 0 videos");

			Observable.FromEventPattern(
				h => lbxDeviceContainer.TreeViewItemClick += h,
				h => lbxDeviceContainer.TreeViewItemClick -= h
				)
				.Window(TimeSpan.FromMilliseconds(50))
				.SelectMany(x => x.TakeLast(1))
				.SubscribeOn(ThreadPoolScheduler.Instance)
				.ObserveOn(DispatcherScheduler.Current)
				.Subscribe(ex =>
				{
					TreeViewItem_PreviewMouseLeftButtonDown(ex.Sender, ex.EventArgs);
				});

            Observable.FromEvent<SelectionChangedEventHandler, SelectionChangedEventArgs>(
                handler => (s, ex) => handler(ex),
                h => lbxFavorites.SelectionChanged += h,
                h => lbxFavorites.SelectionChanged -= h
                )
                .Window(TimeSpan.FromMilliseconds(50))
                .SelectMany(x => x.TakeLast(1))
				.SubscribeOn(ThreadPoolScheduler.Instance)
				.ObserveOn(DispatcherScheduler.Current)
                .Subscribe(ex =>
                {
					lbxFavorites_SelectionChanged(lbxFavorites, ex);
                });

		

            uiDelayTimer = new DispatcherTimer();
            uiDelayTimer.Tick += uiDelayTimer_Tick;
            uiDelayTimer.Interval = new TimeSpan(0, 0, 1);
            uiDelayTimer.Start();
        }

		void lblHomeSharingTutorialTip_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Process.Start("http://waveface.uservoice.com/knowledgebase/articles/215523-step4-share-favorites-with-your-favorite-people");
		}

        void uiDelayTimer_Tick(object sender, EventArgs e)
        {
            uiDelayTimer.Stop();

            if (Properties.Settings.Default.IsFirstUse)
            {
				MessageBoxResult _messageBoxResult = MessageBox.Show(Application.Current.MainWindow, "See a quick tour ?", "Favorite*", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

                if (_messageBoxResult == MessageBoxResult.Yes)
                {
                    Process.Start(HELP_URL);
                }

                WaitForPairingDialog _waitForPairingDialog = new WaitForPairingDialog();
                _waitForPairingDialog.Owner = this;
                _waitForPairingDialog.ShowDialog();

                Properties.Settings.Default.IsFirstUse = false;
                Properties.Settings.Default.Save();
            }
        }

        void btnCopyShareLink_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText((lblContentLocation.DataContext as BunnyLabelContentGroup).ShareURL);
        }

		void tbtnCloudSharing_Checked(object sender, EventArgs e)
        {
            var labelGroup = lblContentLocation.DataContext as BunnyLabelContentGroup;

            //Waveface.ClientFramework.Client.Default.ShareLabel(labelGroup.ID, rspRightSidePane2.swbCloudSharing.IsOn);
			Waveface.ClientFramework.Client.Default.ShareLabel(labelGroup.ID, rspRightSidePane2.tbtnCloudSharing.IsChecked.Value);

            labelGroup.RefreshShareProperties();

			rspRightSidePane2.tbxShareLink.Text = labelGroup.ShareURL;
        }

        void btnClearAll_Click(object sender, RoutedEventArgs e)
        {
            ClientFramework.Client.Default.ClearTaggedContents();
            RefreshContentArea();
            RefreshSelectedFavorite();
			TryUpdateRightSidePanelContentCount();
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
                //TODO: 待重構
                var viewer = new PhotoViewer();
                viewer.Owner = this;
                viewer.Source = (lbxContentContainer.SelectedItems.Count > 1) ? lbxContentContainer.SelectedItems.OfType<IContentEntity>() : lbxContentContainer.DataContext;
                viewer.SelectedIndex = (lbxContentContainer.SelectedItems.Count > 1) ? 0 : lbxContentContainer.SelectedIndex;

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
                TryDisplayUnsortedTutorial();

                ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(ti) as TreeViewItem;

                if (parent == null)
                    return;

                var service = parent.DataContext as IService;

                if (service == null)
                    return;

                Cursor = Cursors.Wait;

                ContentAreaToolBar.Visibility = System.Windows.Visibility.Collapsed;
                lbxContentContainer.Visibility = System.Windows.Visibility.Collapsed;

                unSortedFilesUC.Visibility = Visibility.Visible;
                unSortedFilesUC.Init(service, group, this);

                Cursor = Cursors.Arrow;
            }
            else
            {
                unSortedFilesUC.Stop();
                unSortedFilesUC.Visibility = Visibility.Collapsed;
                ContentAreaToolBar.Visibility = System.Windows.Visibility.Visible;
                lbxContentContainer.Visibility = System.Windows.Visibility.Visible;
            }

			lbxContentContainer.ContextMenu = this.Resources["SourceContentContextMenu"] as ContextMenu;

            Grid.SetColumnSpan(gdContentArea, 2);

            cabContentActionBar.EnableMoveTo = true;

            //btnFavoriteAll.Visibility = Visibility.Visible;
            gdRightSide.Visibility = System.Windows.Visibility.Collapsed;

            rspRightSidePanel.Visibility = System.Windows.Visibility.Collapsed;
            rspRightSidePane2.Visibility = System.Windows.Visibility.Collapsed;


            lbxFavorites.SelectedItem = null;

			lblContentLocation.DataContext = group;
			lbxContentContainer.DataContext = group.Contents;
			SetContentTypeCount(group);
        }

				


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

        private void FavoriteAllButton_Click(object sender, RoutedEventArgs e)
        {
            ClientFramework.Client.Default.Tag(lbxContentContainer.Items.OfType<IContent>());
            RefreshContentArea();
            RefreshStarFavorite();
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

          

			lbxContentContainer.ContextMenu = this.Resources["cm"] as ContextMenu;
			lbxContentContainer.ContextMenu.IsOpen = false;
			lbxContentContainer.ContextMenu.Visibility = Visibility.Visible;

            gdRightSide.Visibility = Visibility.Visible;
            Grid.SetColumnSpan(gdContentArea, 1);

            cabContentActionBar.EnableMoveTo = false;

            ContentAreaToolBar.Visibility = System.Windows.Visibility.Visible;
            lbxContentContainer.Visibility = System.Windows.Visibility.Visible;
            //btnFavoriteAll.Visibility = Visibility.Collapsed;
            unSortedFilesUC.Visibility = Visibility.Collapsed;
            rspRightSidePane2.Visibility = (group.ID.Equals(ClientFramework.Client.StarredLabelId, StringComparison.CurrentCultureIgnoreCase)) ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            rspRightSidePanel.Visibility = (group.ID.Equals(ClientFramework.Client.StarredLabelId, StringComparison.CurrentCultureIgnoreCase)) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            if (lbxDeviceContainer.SelectedItem != null)
            {
                lbxDeviceContainer.ClearSelection();
            }

			lbxContentContainer.DataContext = group.Contents;

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







        private void rspRightSidePanel_SaveToFavorite(object sender, System.EventArgs e)
        {
			SaveToFavorite(lbxContentContainer.Items.OfType<IContentEntity>());
        }




        private void rspRightSidePane2_OnAirClick(object sender, EventArgs e)
        {
            ClientFramework.Client.Default.OnAir((lblContentLocation.DataContext as IContentEntity).ID, rspRightSidePane2.tbtnHomeSharing.IsChecked.Value);
        }

        private void updateRightSidePanel2(IContentGroup group)
        {
            var isOnAir = ClientFramework.Client.Default.IsOnAir(group);

			rspRightSidePane2.tbtnHomeSharing.IsEnabled = ClientFramework.Client.Default.HomeSharingEnabled;
            rspRightSidePane2.tbtnHomeSharing.IsChecked = isOnAir;

            rspRightSidePane2.tbtnCloudSharing.IsChecked = (group as BunnyLabelContentGroup).ShareEnabled;

			rspRightSidePane2.tbxShareLink.Text = (lblContentLocation.DataContext as BunnyLabelContentGroup).ShareURL;
		}

        private void rspRightSidePanel_AddToFavorite(object sender, System.EventArgs e)
        {
			AddToFavorite(lbxContentContainer.Items.OfType<IContentEntity>());
            }





        private void rspRightSidePane2_CloudSharingClick(object sender, System.EventArgs e)
        {
            Wpf_testHTTP.MainWindow _w = new Wpf_testHTTP.MainWindow();
            var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny");

            var iniFile = System.IO.Path.Combine(path, @"sharefavorite.ini");

			var group = GetCurrentContentGroup();
			_w.setTitle(group.Name);
            _w.setiniPath(iniFile);

            var files = string.Join("~", (lbxContentContainer.DataContext as IEnumerable<IContentEntity>).Select(content => content.Uri.LocalPath).ToArray());

            _w.setFilename(files);
			_w.setLabelId(group.ID);
            _w.setRun();
            _w.ShowDialog();
        }



        private void rspRightSidePane2_DeleteButtonClick(object sender, System.EventArgs e)
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
			DelectSelectedContents();
        }


		private void UnTagMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var group = GetCurrentContentGroup();
			var selectedContents = GetSelectedContents();

			foreach (var content in selectedContents)
            ClientFramework.Client.Default.UnTag(group.ID, content.ID);

            RefreshContentArea();
        }

        private void lblContentLocation_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (lblContentLocation.DataContext == null)
            {
                btnBack.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }

			btnBack.Visibility = GetCurrentContentGroup().Parent == null ?
            System.Windows.Visibility.Collapsed :
            System.Windows.Visibility.Visible;
        }

        private void btnAddNewSource_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
			WaitForPairingDialog _waitForPairingDialog = new WaitForPairingDialog();
			_waitForPairingDialog.Show();
        }

		private void Label_MouseDown(object sender, MouseButtonEventArgs e)
		{
			WaitForPairingDialog _waitForPairingDialog = new WaitForPairingDialog
			                                                 {
			                                                     Owner = this
			                                                 };
		    _waitForPairingDialog.ShowDialog();
		}

		Point startPoint;
		Boolean needSpecialMulitSelectProcess;
		DateTime lastMouseLeftButtonDown;
		private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var now = DateTime.Now;
			ListBox list = sender as ListBox;
			ListBoxItem item =
				FindAnchestor<ListBoxItem>((DependencyObject)e.OriginalSource);

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

			ListBox list = sender as ListBox;
			ListBoxItem item =
				FindAnchestor<ListBoxItem>((DependencyObject)e.OriginalSource);

			var dataContext = item.DataContext;

			if (!(lbxContentContainer.SelectedItems.Count == 1 && lbxContentContainer.SelectedItem == dataContext))
			{
                lbxContentContainer.UnselectAll();
                lbxContentContainer.SelectedItem = dataContext;

                PropertyInfo pi = typeof(ListBox).GetProperty("AnchorItem", BindingFlags.NonPublic | BindingFlags.Instance);
                if (pi != null)
                {
                    pi.SetValue(lbxContentContainer, dataContext, null);
                }
			};

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
				ListBox list = sender as ListBox;
				ListBoxItem item =
					FindAnchestor<ListBoxItem>((DependencyObject)e.OriginalSource);
				
				var contents = GetSelectedContents();

				// Initialize the drag & drop operation
				DataObject dragData = new DataObject(typeof(IEnumerable<IContentEntity>), contents);
				DragDrop.DoDragDrop(item, dragData, DragDropEffects.Move);
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
			}
			while (current != null);
			return null;
		}

		private void Sources_DragEnter(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(typeof(IEnumerable<IContentEntity>)) || lbxFavorites.SelectedItem != null)
			{
				e.Effects = DragDropEffects.None;
                e.Handled = true;
			}
		}


        private void lbxDeviceContainer_DragLeave(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(IEnumerable<IContentEntity>)) || lbxFavorites.SelectedItem != null)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void lbxDeviceContainer_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(IEnumerable<IContentEntity>)) || lbxFavorites.SelectedItem != null)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

		private void Sources_Drop(object sender, DragEventArgs e)
		{
			if (e.Effects == DragDropEffects.Move && e.Data.GetDataPresent(typeof(IEnumerable<IContentEntity>)))
			{
				var control = sender as TreeView;
				var controlItem =
					FindAnchestor<TreeViewItem>((DependencyObject)e.OriginalSource);

				if (controlItem == null)
					return;

				var sourceGroup = controlItem.DataContext as IContentGroup;
				var contents = e.Data.GetData(typeof(IEnumerable<IContentEntity>)) as IEnumerable<IContentEntity>;
				
                MoveToFolder(sourceGroup, contents);
			}
		}

        private void MoveToFolder(IContentGroup targetGroup, IEnumerable<IContentEntity> contents)
        {
            var contentIDs = contents.Select(content => content.ID);

            MoveToFolder(targetGroup, contentIDs);
        }

        private void MoveToFolder(IContentGroup targetGroup, IEnumerable<string> contentIDs)
        {

            Waveface.ClientFramework.Client.Default.Move(contentIDs, targetGroup.Uri.LocalPath);

            RefreshContentArea();

            var service = targetGroup.Service;
            service.Refresh();

            var tempcontents = service.Contents;
        }

		private void Favorites_DragEnter(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(typeof(IEnumerable<IContentEntity>)) ||
				sender == e.Source)
			{
				e.Effects = DragDropEffects.None;
			}
		}



		private void Favorites_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(IEnumerable<IContentEntity>)))
			{
				var list = sender as ListBox;
				var controlItem =
					FindAnchestor<ListBoxItem>((DependencyObject)e.OriginalSource);

				if (controlItem == null)
					return;

				var contents = e.Data.GetData(typeof(IEnumerable<IContentEntity>)) as IEnumerable<IContentEntity>;
				var favoriteGroup = controlItem.DataContext as IContentGroup;

				if (favoriteGroup.ID.Equals("00000000-0000-0000-0000-000000000000", StringComparison.CurrentCultureIgnoreCase))
				{
					StarContent(contents);
					SelectToStarFavorite();
					return;
				}

				AddToFavorite(favoriteGroup.ID, contents);
			}
		}

		private void lbxFavorites_DeleteFavoriteInvoked(object sender, EventArgs e)
		{
			DeleteCurrentFavorite();
		}

		private void lbxDeviceContainer_DeleteSourceInvoked(object sender, EventArgs e)
		{
			DelectSelectedFolder();
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

        private void ContentActionBar_AddToFavorite(object sender, EventArgs e)
        {
            AddSelectedToFavorite();
        }

        private void ContentActionBar_AddToStarred(object sender, EventArgs e)
        {
            StarSelectedContents();
        }

        private void ContentActionBar_CreateFavorite(object sender, EventArgs e)
        {
            SaveSelectedContentsToFavorite();
        }

        private void ContentActionBar_MoveToNewFolder(object sender, EventArgs e)
        {

        }

        private void ContentActionBar_MoveToExistingFolder(object sender, EventArgs e)
        {
            var dialog = new AddToDialog();
            dialog.Owner = this;
            dialog.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            var currentGroup = GetCurrentContentGroup();
            dialog.ItemSource = currentGroup.Service.Contents.Skip(1).Except(new IContentEntity[] { currentGroup });

            if (dialog.ShowDialog() != true)
                return;

            var selectedGroup = (dialog.SelectedItem as IContentGroup);
            MoveToFolder(selectedGroup, GetSelectedContents());
        }

    }
}
