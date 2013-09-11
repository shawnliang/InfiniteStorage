using System;
using System.Windows;
using System.Windows.Controls;

public static class ItemsControlExtension
{
	public static DependencyObject ContainerFromItem(this ItemsControl itemsControl, Object value)
	{
		if(itemsControl == null)
		{
			return null;
		}

		var dp = itemsControl.ItemContainerGenerator.ContainerFromItem(value);

		if (dp != null)
			return dp;

		foreach (var item in itemsControl.Items)
		{
			var currentTreeViewItem = itemsControl.ItemContainerGenerator.ContainerFromItem(item);

			var childDp = ContainerFromItem(currentTreeViewItem as ItemsControl, value);

			if (childDp != null)
				return childDp;
		}

		return null;
	}

	public static T ContainerFromItem<T>(this ItemsControl itemsControl, Object value) where T : class
	{
		return ContainerFromItem(itemsControl, value) as T;
	}
}
