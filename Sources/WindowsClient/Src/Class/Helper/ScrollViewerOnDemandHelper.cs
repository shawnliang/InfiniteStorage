using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Waveface.Client
{
	public static class ScrollViewerOnDemandHelper
	{
		#region Private Static Method
		private static bool IsContentControlVisible(FrameworkElement child, ScrollViewer scrollViewer)
		{
			var childTransform = child.TransformToAncestor(scrollViewer);
			var childRectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), child.RenderSize));
			var ownerRectangle = new Rect(new Point(0, 0), scrollViewer.RenderSize);
			return ownerRectangle.IntersectsWith(childRectangle);
		}

		private static T GetVisualChild<T>(DependencyObject parent) where T : Visual
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

		private static ChildControl FindVisualChild<ChildControl>(DependencyObject DependencyObj)
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
		#endregion

		#region Public Static Method
		public static void TryDisplayVisibleContentControls<T>(ScrollViewer scrollViewer, ItemsControl itemsControl, Action<T> displayAction, int prepareInvisibleContentControlCount = 15) where T : Visual
		{
			var visibleAreaEntered = false;
			var visibleAreaLeft = false;
			var invisibleItemDisplayed = 0;

			foreach (var item in itemsControl.Items)
			{
				var itemContainer = (FrameworkElement)itemsControl.ItemContainerGenerator.ContainerFromItem(item);

				if (visibleAreaLeft == false && IsContentControlVisible(itemContainer, scrollViewer))
				{
					visibleAreaEntered = true;
				}
				else if (visibleAreaEntered)
				{
					visibleAreaLeft = true;
				}

				if (visibleAreaEntered)
				{
					if (visibleAreaLeft && ++invisibleItemDisplayed > prepareInvisibleContentControlCount)
						break;

					ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(itemContainer);

					displayAction(GetVisualChild<T>(contentPresenter));
				}
			}
		}
		#endregion
	}
}
