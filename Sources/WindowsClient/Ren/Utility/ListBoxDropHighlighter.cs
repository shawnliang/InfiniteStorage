#region

using System.Windows;
using System.Windows.Controls;

#endregion

namespace Waveface.Client.Utility
{
	public static class ListBoxDropHighlighter
	{
		// the ListBoxItem that is the current drop target
		private static ListBoxItem _currentItem;

		// Indicates whether the current ListBoxItem is a possible drop target
		private static bool _dropPossible;

		#region IsPossibleDropTarget

		private static readonly DependencyPropertyKey IsPossibleDropTargetKey =
			DependencyProperty.RegisterAttachedReadOnly(
				"IsPossibleDropTarget",
				typeof(bool),
				typeof(ListBoxDropHighlighter),
				new FrameworkPropertyMetadata(null, CalculateIsPossibleDropTarget));

		public static readonly DependencyProperty IsPossibleDropTargetProperty = IsPossibleDropTargetKey.DependencyProperty;

		public static bool GetIsPossibleDropTarget(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsPossibleDropTargetProperty);
		}

		private static object CalculateIsPossibleDropTarget(DependencyObject item, object value)
		{
			if ((item == _currentItem) && (_dropPossible))
				return true;
			else
				return false;
		}

		#endregion

		static ListBoxDropHighlighter()
		{
			EventManager.RegisterClassHandler(typeof(ListBoxItem),
											  UIElement.PreviewDragEnterEvent,
											  new DragEventHandler(OnDragEvent), true);

			EventManager.RegisterClassHandler(typeof(ListBoxItem),
											  UIElement.PreviewDragLeaveEvent,
											  new DragEventHandler(OnDragLeave), true);

			EventManager.RegisterClassHandler(typeof(ListBoxItem),
											  UIElement.PreviewDragOverEvent,
											  new DragEventHandler(OnDragEvent), true);

			EventManager.RegisterClassHandler(typeof(ListBoxItem),
											  UIElement.PreviewDropEvent,
											  new DragEventHandler(OnDragLeave), true);
		}

		#region event handlers

		private static void OnDragEvent(object sender, DragEventArgs args)
		{
			lock (IsPossibleDropTargetProperty)
			{
				_dropPossible = false;

				if (_currentItem != null)
				{
					DependencyObject _oldItem = _currentItem;
					_currentItem = null;
					_oldItem.InvalidateProperty(IsPossibleDropTargetProperty);
				}

				if (args.Effects != DragDropEffects.None)
				{
					_dropPossible = true;
				}

				ListBoxItem _lbi = sender as ListBoxItem;

				if (_lbi != null)
				{
					_currentItem = _lbi;
					_currentItem.InvalidateProperty(IsPossibleDropTargetProperty);
				}
			}
		}

		private static void OnDragLeave(object sender, DragEventArgs args)
		{
			lock (IsPossibleDropTargetProperty)
			{
				_dropPossible = false;

				if (_currentItem != null)
				{
					DependencyObject _oldItem = _currentItem;
					_currentItem = null;
					_oldItem.InvalidateProperty(IsPossibleDropTargetProperty);
				}

				ListBoxItem _lbi = sender as ListBoxItem;

				if (_lbi != null)
				{
					_currentItem = _lbi;
					_lbi.InvalidateProperty(IsPossibleDropTargetProperty);
				}
			}
		}

		#endregion
	}
}