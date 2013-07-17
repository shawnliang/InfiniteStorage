#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

#endregion

namespace Waveface.Client
{
	public partial class ContentActionBar : UserControl
	{
		#region Var

		public static readonly DependencyProperty s_showMoveTo = DependencyProperty.Register("ShowMoveTo", typeof (bool), typeof (ContentActionBar),
		                                                                                    new UIPropertyMetadata(false, OnShowMoveToChanged));

		public static readonly DependencyProperty s_enableMoveTo = DependencyProperty.Register("EnableMoveTo", typeof (bool), typeof (ContentActionBar),
		                                                                                      new UIPropertyMetadata(false, OnEnableMoveToChanged));

		public static readonly DependencyProperty s_enableCreate = DependencyProperty.Register("EnableCreate", typeof (bool), typeof (ContentActionBar),
		                                                                                      new UIPropertyMetadata(true, OnEnableCreateChanged));

		public static readonly DependencyProperty s_enableAddTo = DependencyProperty.Register("EnableAddTo", typeof (bool), typeof (ContentActionBar),
		                                                                                     new UIPropertyMetadata(true, OnEnableAddToChanged));

		public static readonly DependencyProperty s_hideStarredMenuItem = DependencyProperty.Register("HideStarredMenuItem", typeof (bool), typeof (ContentActionBar),
		                                                                                             new UIPropertyMetadata(false, OnHideStarredMenuItemChanged));

		#endregion

		#region Property

		public bool ShowMoveTo
		{
			get { return (bool) GetValue(s_showMoveTo); }
			set { SetValue(s_showMoveTo, value); }
		}

		public bool EnableMoveTo
		{
			get { return (bool) GetValue(s_enableMoveTo); }
			set { SetValue(s_enableMoveTo, value); }
		}

		public bool EnableCreate
		{
			get { return (bool) GetValue(s_enableMoveTo); }
			set { SetValue(s_enableMoveTo, value); }
		}

		public bool EnableAddTo
		{
			get { return (bool) GetValue(s_enableMoveTo); }
			set { SetValue(s_enableMoveTo, value); }
		}

		public bool HideStarredMenuItem
		{
			get { return (bool) GetValue(s_hideStarredMenuItem); }
			set
			{
				SetValue(s_hideStarredMenuItem, value);

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
			InitializeComponent();
		}

		#region Protected Method

		public void OnMoverToNewFolder(EventArgs e)
		{
			if (MoveToNewFolder == null)
				return;

			MoveToNewFolder(this, e);
		}

		public void OnMoveToExistingFolder(EventArgs e)
		{
			if (MoveToExistingFolder == null)
				return;

			MoveToExistingFolder(this, e);
		}

		public void OnCreateFavorite(EventArgs e)
		{
			if (CreateFavorite == null)
				return;

			CreateFavorite(this, e);
		}

		public void OnAddToFavorite(EventArgs e)
		{
			if (AddToFavorite == null)
				return;

			AddToFavorite(this, e);
		}

		public void OnAddToStarred(EventArgs e)
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

			var horizontalOffset = (source.TransformToAncestor(this).Transform(new Point(0d, 0d)).X + cm.ActualWidth > ActualWidth)
				                       ? (ActualWidth - cm.ActualWidth) - source.TransformToAncestor(this).Transform(new Point(0d, 0d)).X
				                       : 0;

			cm.HorizontalOffset = horizontalOffset;
		}

		private void Image_MouseDown(object sender, MouseButtonEventArgs e)
		{
			ShowContextMenu((sender as UIElement), (sender as UserControl).ContextMenu);
		}

		private void Create_MouseDown(object sender, MouseButtonEventArgs e)
		{
			ShowContextMenu((sender as UIElement), (sender as UserControl).ContextMenu);
		}

		private void AddTo_MouseDown(object sender, MouseButtonEventArgs e)
		{
			ShowContextMenu((sender as UIElement), (sender as UserControl).ContextMenu);
		}

		private static void OnShowMoveToChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;

			var obj = o as ContentActionBar;
			obj.ShowMoveTo = (bool) e.NewValue;
		}

		private static void OnEnableMoveToChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;

			var obj = o as ContentActionBar;
			obj.EnableMoveTo = (bool) e.NewValue;
		}

		private static void OnEnableCreateChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;

			var obj = o as ContentActionBar;
			obj.EnableCreate = (bool) e.NewValue;
		}

		private static void OnEnableAddToChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;

			var obj = o as ContentActionBar;
			obj.EnableAddTo = (bool) e.NewValue;
		}

		private static void OnHideStarredMenuItemChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;

			var obj = o as ContentActionBar;
			obj.HideStarredMenuItem = (bool) e.NewValue;
		}

		private void miMoveToNewFolder_Click(object sender, RoutedEventArgs e)
		{
			OnMoverToNewFolder(EventArgs.Empty);
		}

		private void miMoveToExistingFolder_Click(object sender, RoutedEventArgs e)
		{
			OnMoveToExistingFolder(EventArgs.Empty);
		}

		private void miCreate_Click(object sender, RoutedEventArgs e)
		{
			OnCreateFavorite(EventArgs.Empty);
		}

		private void miAddToFavorite_Click(object sender, RoutedEventArgs e)
		{
			OnAddToFavorite(EventArgs.Empty);
		}

		private void miAddToStarred_Click(object sender, RoutedEventArgs e)
		{
			OnAddToStarred(EventArgs.Empty);
		}
	}
}