#region

using System.Windows;
using System.Windows.Controls;

#endregion

namespace Waveface.Client.Utility
{
	public static class ListBoxDropHighlighter
	{
		// the ListBoxItem that is the current drop target
		private static ListBoxItem s_currentItem;

		// Indicates whether the current ListBoxItem is a possible drop target
		private static bool s_dropPossible;

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
			return (item == s_currentItem) && (s_dropPossible);
		}

		#endregion

		static ListBoxDropHighlighter()
		{
			EventManager.RegisterClassHandler(typeof(ListBoxItem),
											  UIElement.DragEnterEvent,
											  new DragEventHandler(OnDragEvent), true);

			EventManager.RegisterClassHandler(typeof(ListBoxItem),
											  UIElement.DragLeaveEvent,
											  new DragEventHandler(OnDragLeave), true);

			EventManager.RegisterClassHandler(typeof(ListBoxItem),
											  UIElement.DragOverEvent,
											  new DragEventHandler(OnDragEvent), true);

			EventManager.RegisterClassHandler(typeof(ListBoxItem),
											  UIElement.DropEvent,
											  new DragEventHandler(OnDragLeave), true);
		}

		#region event handlers

		private static void OnDragEvent(object sender, DragEventArgs args)
		{
			lock (IsPossibleDropTargetProperty)
			{
				s_dropPossible = false;

				if (s_currentItem != null)
				{
					DependencyObject _oldItem = s_currentItem;
					s_currentItem = null;
					_oldItem.InvalidateProperty(IsPossibleDropTargetProperty);
				}

				if (args.Effects != DragDropEffects.None)
				{
					s_dropPossible = true;
				}

				ListBoxItem _lbi = sender as ListBoxItem;

				if (_lbi != null)
				{
					s_currentItem = _lbi;
					s_currentItem.InvalidateProperty(IsPossibleDropTargetProperty);
				}
			}
		}

		private static void OnDragLeave(object sender, DragEventArgs args)
		{
			lock (IsPossibleDropTargetProperty)
			{
				s_dropPossible = false;

				if (s_currentItem != null)
				{
					DependencyObject _oldItem = s_currentItem;
					s_currentItem = null;
					_oldItem.InvalidateProperty(IsPossibleDropTargetProperty);
				}

				ListBoxItem _lbi = sender as ListBoxItem;

				if (_lbi != null)
				{
					s_currentItem = _lbi;
					_lbi.InvalidateProperty(IsPossibleDropTargetProperty);
				}
			}
		}

		#endregion
	}
}