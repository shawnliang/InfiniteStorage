#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

#endregion

namespace Waveface.Client
{
	public partial class SourceTree : TreeView
	{
		#region Event

		public event EventHandler TreeViewItemClick;
		public event EventHandler StarInvoked;
		public event EventHandler CreateFavoriteInvoked;
		public event EventHandler AddToFavoriteInvoked;
		public event EventHandler DeleteSourceInvoked;

		#endregion

		public SourceTree()
		{
			InitializeComponent();
		}

		private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (TreeViewItemClick == null)
				return;

			TreeViewItemClick(sender, EventArgs.Empty);
		}

		private void recvingIcon_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var recvIcon = (Image) sender;

			if (recvIcon.Visibility == Visibility.Visible)
			{
				var da = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(1.0)));
				var rotate = new RotateTransform();

				recvIcon.RenderTransform = rotate;
				recvIcon.RenderTransformOrigin = new Point(0.5, 0.5);
				da.RepeatBehavior = RepeatBehavior.Forever;
				rotate.BeginAnimation(RotateTransform.AngleProperty, da);
			}
			else
			{
				var rotate = recvIcon.RenderTransform as RotateTransform;

				if (rotate == null)
					return;

				rotate.BeginAnimation(RotateTransform.AngleProperty, null);

				recvIcon.RenderTransform = null;
			}
		}

		private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton != MouseButton.Left)
				return;

			var tv = sender as TreeView;

			var item = tv.ItemContainerGenerator.ContainerFromItem(tv.SelectedItem) as TreeViewItem;

			if (item != null && item.IsMouseOver)
				item.IsExpanded = !item.IsExpanded;

			e.Handled = true;
		}

		private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
		}

		protected void OnStarInvoked(EventArgs e)
		{
			if (StarInvoked == null)
				return;

			StarInvoked(this, e);
		}

		protected void OnCreateFavoriteInvoked(EventArgs e)
		{
			if (CreateFavoriteInvoked == null)
				return;

			CreateFavoriteInvoked(this, e);
		}

		protected void OnAddToFavoriteInvoked(EventArgs e)
		{
			if (AddToFavoriteInvoked == null)
				return;

			AddToFavoriteInvoked(this, e);
		}

		protected void OnDeleteSourceInvoked(EventArgs e)
		{
			if (DeleteSourceInvoked == null)
				return;

			DeleteSourceInvoked(this, e);
		}

		private void StarMenuItem_Click(object sender, RoutedEventArgs e)
		{
			OnStarInvoked(EventArgs.Empty);
		}

		private void CreateFavoriteMenuItem_Click(object sender, RoutedEventArgs e)
		{
			OnCreateFavoriteInvoked(EventArgs.Empty);
		}

		private void AddToFavoriteMenuItem_Click(object sender, RoutedEventArgs e)
		{
			OnAddToFavoriteInvoked(EventArgs.Empty);
		}

		private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
		{
			OnDeleteSourceInvoked(EventArgs.Empty);
		}
	}
}