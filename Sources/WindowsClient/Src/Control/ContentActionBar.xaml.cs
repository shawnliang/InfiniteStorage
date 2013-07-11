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
        public static readonly DependencyProperty _hideMoveTo = DependencyProperty.Register("HideMoveTo", typeof(bool), typeof(ContentActionBar), new UIPropertyMetadata(false, new PropertyChangedCallback(OnHideMoveToChanged)));
		public static readonly DependencyProperty _enableMoveTo = DependencyProperty.Register("EnableMoveTo", typeof(bool), typeof(ContentActionBar), new UIPropertyMetadata(true, new PropertyChangedCallback(OnEnableMoveToChanged)));
		public static readonly DependencyProperty _enableCreate = DependencyProperty.Register("EnableCreate", typeof(bool), typeof(ContentActionBar), new UIPropertyMetadata(true, new PropertyChangedCallback(OnEnableCreateChanged)));
		public static readonly DependencyProperty _enableAddTo = DependencyProperty.Register("EnableAddTo", typeof(bool), typeof(ContentActionBar), new UIPropertyMetadata(true, new PropertyChangedCallback(OnEnableAddToChanged)));
		public static readonly DependencyProperty _hideStarredMenuItem = DependencyProperty.Register("HideStarredMenuItem", typeof(bool), typeof(ContentActionBar), new UIPropertyMetadata(false, new PropertyChangedCallback(OnHideStarredMenuItemChanged)));
	
		#endregion

        #region Property
        public bool HideMoveTo
        {
            get
            {
                return (bool)GetValue(_hideMoveTo);
            }
            set
            {
                SetValue(_hideMoveTo, value);
            }
        }

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

		public bool EnableCreate
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

		public bool EnableAddTo
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

		public bool HideStarredMenuItem
		{
			get
			{
				return (bool)GetValue(_hideStarredMenuItem);
			}
			set
			{
				SetValue(_hideStarredMenuItem, value);

				(atbAddTo.ContextMenu.Items[1] as MenuItem).Visibility = value ? Visibility.Collapsed : Visibility.Visible;
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
            cm.VerticalOffset = -3;

            var horizontalOffset = (source.TransformToAncestor(this).Transform(new Point(0d, 0d)).X + cm.ActualWidth > this.ActualWidth) ? (this.ActualWidth - cm.ActualWidth) - source.TransformToAncestor(this).Transform(new Point(0d, 0d)).X : 0;
            cm.HorizontalOffset = horizontalOffset;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
			ShowContextMenu((sender as UIElement), (sender as UserControl).ContextMenu);

        }

        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
			ShowContextMenu((sender as UIElement), (sender as UserControl).ContextMenu);

        }

        private void Image_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
			ShowContextMenu((sender as UIElement), (sender as UserControl).ContextMenu);
        }

        private static void OnHideMoveToChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o == null)
                return;
            var obj = o as ContentActionBar;
            obj.HideMoveTo = (bool)e.NewValue;
        }

		private static void OnEnableMoveToChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as ContentActionBar;
			obj.EnableMoveTo = (bool)e.NewValue;
		}

		private static void OnEnableCreateChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as ContentActionBar;
			obj.EnableCreate = (bool)e.NewValue;
		}

		private static void OnEnableAddToChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as ContentActionBar;
			obj.EnableAddTo = (bool)e.NewValue;
		}

		private static void OnHideStarredMenuItemChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as ContentActionBar;
			obj.HideStarredMenuItem = (bool)e.NewValue;
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