using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private DispatcherTimer uiDelayTimer;

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

			rspRightSidePane2.tbtnHomeSharing.Checked += rspRightSidePane2_OnAirClick;
			rspRightSidePane2.tbtnHomeSharing.Unchecked += rspRightSidePane2_OnAirClick;

			rspRightSidePane2.tbtnCloudSharing.Checked += tbtnCloudSharing_Checked;
			rspRightSidePane2.tbtnCloudSharing.Unchecked += tbtnCloudSharing_Checked;
            rspRightSidePane2.btnCopyShareLink.Click += btnCopyShareLink_Click;

            rspRightSidePanel.btnClearAll.Click += new RoutedEventHandler(btnClearAll_Click);

            lblContentTypeCount.Content = string.Format("0 photos 0 videos");

            var syncContext = SynchronizationContext.Current;


            Observable.FromEventPattern(
                h => lbxDeviceContainer.TreeViewItemClick += h,
                h => lbxDeviceContainer.TreeViewItemClick -= h
                )
                .Window(TimeSpan.FromMilliseconds(50))
                .SelectMany(x => x.TakeLast(1))
                .Subscribe(ex =>
                {
                    syncContext.Send((o) =>
                        {
                            TreeViewItem_PreviewMouseLeftButtonDown(ex.Sender, ex.EventArgs);
                        }, null);
                });

            Observable.FromEvent<SelectionChangedEventHandler, SelectionChangedEventArgs>(
                handler => (s, ex) => handler(ex),
                h => lbxFavorites.SelectionChanged += h,
                h => lbxFavorites.SelectionChanged -= h
                )
                .Window(TimeSpan.FromMilliseconds(50))
                .SelectMany(x => x.TakeLast(1))
                .Subscribe(ex =>
                {
                    syncContext.Send((o) =>
                    {
                        lbxFavorites_SelectionChanged(lbxFavorites, ex);
                    }, null);
                });

            uiDelayTimer = new DispatcherTimer();
            uiDelayTimer.Tick += uiDelayTimer_Tick;
            uiDelayTimer.Interval = new TimeSpan(0, 0, 1);
            uiDelayTimer.Start();
        }

        void uiDelayTimer_Tick(object sender, EventArgs e)
        {
            uiDelayTimer.Stop();

            if (Properties.Settings.Default.IsFirstUse)
            {
                MessageBoxResult _messageBoxResult = MessageBox.Show("See a quick tour ?", "Favorite*", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

                if (_messageBoxResult == MessageBoxResult.Yes)
                {
                    Process.Start(@"http://waveface.com/");
                }

                WaitForPairingDialog _waitForPairingDialog = new WaitForPairingDialog();
                _waitForPairingDialog.Owner = this;
                _waitForPairingDialog.ShowDialog();

                //測試中, 先不存
                //Properties.Settings.Default.IsFirstUse = false;
                //Properties.Settings.Default.Save();
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
                TryDisplayUnsortedTutorial();

                ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(ti) as TreeViewItem;

                if (parent == null)
                    return;

                var service = parent.DataContext as IService;

                if (service == null)
                    return;

                Cursor = Cursors.Wait;

                unSortedFilesUC.Visibility = Visibility.Visible;
                unSortedFilesUC.Init(service);

                Cursor = Cursors.Arrow;
            }
            else
            {
                unSortedFilesUC.Stop();
                unSortedFilesUC.Visibility = Visibility.Collapsed;
            }

            lbxContentContainer.ContextMenu.Visibility = System.Windows.Visibility.Collapsed;

            lblContentLocation.DataContext = group;
            lbxContentContainer.DataContext = group.Contents;
            SetContentTypeCount(group);


            Grid.SetColumnSpan(gdContentArea, 2);

            btnFavoriteAll.Visibility = Visibility.Visible;
            gdRightSide.Visibility = System.Windows.Visibility.Collapsed;

            rspRightSidePanel.Visibility = System.Windows.Visibility.Collapsed;
            rspRightSidePane2.Visibility = System.Windows.Visibility.Collapsed;

            lbxFavorites.SelectedItem = null;
        }

        private void TryDisplayUnsortedTutorial()
        {
            if (!Properties.Settings.Default.IsFirstSelectUnsorted)
            {
				var result = TakeTourDialog.Show("Organizing thousands of photos on your phone will be breeze from now on. Check out a couple basic tips to get started.", this);
				
				if (result.HasValue && result.Value)
					Process.Start(@"http://waveface.uservoice.com/knowledgebase/articles/215521-step2-organizing-photos-and-videos-in-favorite-");


                Properties.Settings.Default.IsFirstSelectUnsorted = true;
                Properties.Settings.Default.Save();
            }
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
            RefreshStarted();
        }

        private void RefreshStarted()
        {
            RefreshFavorite(lbxFavorites.Items.OfType<IContentGroup>().FirstOrDefault());
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

                SetContentTypeCount(group);
            }
            else
            {
                TryDisplayFavoriteTutorial();

                updateRightSidePanel2(group);
            }

            lbxContentContainer.DataContext = group.Contents;

            var contextMenu = lbxContentContainer.ContextMenu;
            contextMenu.IsOpen = false;
            contextMenu.Visibility = Visibility.Visible;

            gdRightSide.Visibility = Visibility.Visible;
            Grid.SetColumnSpan(gdContentArea, 1);

            btnFavoriteAll.Visibility = Visibility.Collapsed;
            unSortedFilesUC.Visibility = Visibility.Collapsed;
            rspRightSidePane2.Visibility = (group.ID.Equals(ClientFramework.Client.StarredLabelId, StringComparison.CurrentCultureIgnoreCase)) ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            rspRightSidePanel.Visibility = (group.ID.Equals(ClientFramework.Client.StarredLabelId, StringComparison.CurrentCultureIgnoreCase)) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            if (lbxDeviceContainer.SelectedItem != null)
            {
                lbxDeviceContainer.ClearSelection();
            }


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

        private void TryDisplayFavoriteTutorial()
        {
            if (!Properties.Settings.Default.IsFirstSelectFavorite)
            {
				var result = TakeTourDialog.Show("Share precious moments you’ve picked out to your favorite people. Check out a couple basic tips to get started.", this);
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
				var result = TakeTourDialog.Show("View your favorite stuff on your tablet and TV in no time. Check out a couple basic tips to get started.", this);
				if (result.HasValue && result.Value)
					Process.Start(@"http://waveface.uservoice.com/knowledgebase/articles/215522-step3-view-favorite-memories-on-tablets-and-tvs-");

                Properties.Settings.Default.IsFirstSelectStarred = true;
                Properties.Settings.Default.Save();
            }
        }



        private void rspRightSidePanel_SaveToFavorite(object sender, System.EventArgs e)
        {
            if (!lbxContentContainer.HasItems)
            {
                MessageBox.Show("Without any content");
                return;
            }

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
            if (!lbxContentContainer.HasItems)
            {
                MessageBox.Show("Without any content");
                return;
            }

            var favorites = Waveface.ClientFramework.Client.Default.Favorites.Skip(1);

            if (!favorites.Any())
            {
                MessageBox.Show("No existing Favorites");
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
            lbxFavorites.SelectedIndex = dialog.SelectedFavoriteIndex + 1;
            RefreshSelectedFavorite();
        }

        private void RefreshFavorite(IContentGroup favorite)
        {
            if (favorite == null)
                return;
            favorite.Refresh();
        }

        private void RefreshSelectedFavorite()
        {
            RefreshFavorite(lbxFavorites.SelectedItem as IContentGroup);
        }

        private void rspRightSidePane2_CloudSharingClick(object sender, System.EventArgs e)
        {
            Wpf_testHTTP.MainWindow _w = new Wpf_testHTTP.MainWindow();
            var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny");

            var iniFile = System.IO.Path.Combine(path, @"sharefavorite.ini");

            _w.setTitle(this.Title);
            _w.setiniPath(iniFile);

            var files = string.Join("~", (lbxContentContainer.DataContext as IEnumerable<IContentEntity>).Select(content => content.Uri.LocalPath).ToArray());

            _w.setFilename(files);
			_w.setLabelId((lblContentLocation.DataContext as IContentGroup).ID);
            _w.setRun();
            _w.ShowDialog();
        }

        private void RefreshContentArea()
        {
            var group = lblContentLocation.DataContext as IContentGroup;
            if (group == null)
                return;

            group.Refresh();
            SetContentTypeCount(group);
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

        private void lblContentLocation_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (lblContentLocation.DataContext == null)
            {
                btnBack.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }

            btnBack.Visibility = (lblContentLocation.DataContext as IContentGroup).Parent == null ?
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
			WaitForPairingDialog _waitForPairingDialog = new WaitForPairingDialog();
			_waitForPairingDialog.Show();
		}
    }
}
