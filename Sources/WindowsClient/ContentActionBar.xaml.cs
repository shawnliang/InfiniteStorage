using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Waveface.Client
{
	/// <summary>
	/// ContentActionBar.xaml 的互動邏輯
	/// </summary>
	public partial class ContentActionBar : UserControl
	{
        #region Var
        public static readonly DependencyProperty _enableMoveTo = DependencyProperty.Register("EnableMoveTo", typeof(bool), typeof(ContentActionBar), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEnableMoveToChanged)));
        #endregion

        #region Property
        public bool EnableMoveTo
        {
            get
            {
                return (bool)GetValue(_enableMoveTo);
            }
            set
            {
                SetValue(_enableMoveTo, value);
            }
        }
        #endregion


        #region Event
        public event EventHandler MoveToNewFolder;
        public event EventHandler MoveToExistingFolder;
        public event EventHandler CreateFavorite;
        public event EventHandler AddToFavorite;
        public event EventHandler AddToStarred;
        #endregion


        public ContentActionBar()
		{
			this.InitializeComponent();
		}


        #region Protected Method
        protected void OnMoverToNewFolder(EventArgs e)
        {
            if (MoveToNewFolder == null)
                return;

            MoveToNewFolder(this, e);
        }

        protected void OnMoveToExistingFolder(EventArgs e)
        {
            if (MoveToExistingFolder == null)
                return;

            MoveToExistingFolder(this, e);
        }

        protected void OnCreateFavorite(EventArgs e)
        {
            if (CreateFavorite == null)
                return;

            CreateFavorite(this, e);
        }

        protected void OnAddToFavorite(EventArgs e)
        {
            if (AddToFavorite == null)
                return;

            AddToFavorite(this, e);
        }

        protected void OnAddToStarred(EventArgs e)
        {
            if (AddToStarred == null)
                return;

            AddToStarred(this, e);
        }
        #endregion

        private void ShowContextMenu(UIElement source, ContextMenu cm)
        {
            cm.IsEnabled = true;
            cm.IsOpen = true;
            cm.PlacementTarget = source;
            cm.Placement = PlacementMode.Top;
            cm.VerticalOffset = -15;

            var horizontalOffset = (source.TransformToAncestor(this).Transform(new Point(0d, 0d)).X + cm.ActualWidth > this.ActualWidth) ? (this.ActualWidth - cm.ActualWidth) - source.TransformToAncestor(this).Transform(new Point(0d, 0d)).X : 0;
            cm.HorizontalOffset = horizontalOffset;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowContextMenu((sender as Image), (sender as Image).ContextMenu);

        }

        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            ShowContextMenu((sender as Image), (sender as Image).ContextMenu);

        }

        private void Image_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            ShowContextMenu((sender as Image), (sender as Image).ContextMenu);
        }

        private static void OnEnableMoveToChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o == null)
                return;
            var obj = o as ContentActionBar;
            obj.EnableMoveTo = (bool)e.NewValue;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OnMoverToNewFolder(EventArgs.Empty);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            OnMoveToExistingFolder(EventArgs.Empty);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            OnCreateFavorite(EventArgs.Empty);
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            OnAddToFavorite(EventArgs.Empty);
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            OnAddToStarred(EventArgs.Empty);
        }
	}
}