#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Waveface.Model;

#endregion

namespace Waveface.Client
{
	public partial class SourceTree : TreeView
	{
		private Point m_startPoint;

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

		private void TreeViewItem_PreviewMouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
		{
			if (TreeViewItemClick == null)
				return;

			//if (e.Source is ToggleButton)
			//    return;

			TreeViewItemClick(sender, EventArgs.Empty);
		}

		private void recvingIcon_IsVisibleChanged(Object sender, DependencyPropertyChangedEventArgs e)
		{
			var recvIcon = (Image)sender;

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

		private void UserControl_MouseUp(Object sender, MouseButtonEventArgs e)
		{
			//if (e.ChangedButton != MouseButton.Left)
			//    return;

			//var tv = sender as TreeView;

			//var item = tv.ItemContainerGenerator.ContainerFromItem(tv.SelectedItem) as TreeViewItem;

			//if (item != null && item.IsMouseOver)
			//    item.IsExpanded = !item.IsExpanded;

			//e.Handled = true;
		}

		private void UserControl_PreviewKeyDown(Object sender, KeyEventArgs e)
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

		private void miStar_Click(Object sender, RoutedEventArgs e)
		{
			OnStarInvoked(EventArgs.Empty);
		}

		private void miCreateFavorite_Click(Object sender, RoutedEventArgs e)
		{
			OnCreateFavoriteInvoked(EventArgs.Empty);
		}

		private void miAddToFavorite_Click(Object sender, RoutedEventArgs e)
		{
			OnAddToFavoriteInvoked(EventArgs.Empty);
		}

		private void miDelete_Click(Object sender, RoutedEventArgs e)
		{
			OnDeleteSourceInvoked(EventArgs.Empty);
		}

		private void UserControl_PreviewMouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
		{
			m_startPoint = e.GetPosition(null);
		}

		private void UserControl_MouseMove(Object sender, MouseEventArgs e)
		{
			Point _mousePos = e.GetPosition(null);
			Vector _diff = m_startPoint - _mousePos;

			if (e.LeftButton == MouseButtonState.Pressed &&
				(Math.Abs(_diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
				 Math.Abs(_diff.Y) > SystemParameters.MinimumVerticalDragDistance))
			{
				if (SelectedItem != null)
				{
					IContentEntity _entity = SelectedItem as IContentEntity;

					if (_entity != null)
					{
						String[] _files = new String[1];
						_files[0] = _entity.Uri.LocalPath;

						DataObject _dragData = new DataObject();
						_dragData.SetData(DataFormats.FileDrop, _files);
						DragDrop.DoDragDrop(this, _dragData, DragDropEffects.Move);
					}
				}
			}
		}

		private void miLocateOnDisk_Click(Object sender, RoutedEventArgs e)
		{
			if (SelectedItem != null)
			{
				IContentEntity _entity = SelectedItem as IContentEntity;

				if (_entity != null)
				{
					String _dir = _entity.Uri.LocalPath;
					String _arg = @"/select, " + _dir;
					System.Diagnostics.Process.Start("explorer.exe", _arg);
				}
			}
		}
	}
}