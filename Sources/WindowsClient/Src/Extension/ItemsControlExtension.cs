using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

public static class ItemsControlExtension
{
	public static DependencyObject ContainerFromItem(this ItemsControl itemsControl, object value)
	{
		foreach (var item in itemsControl.Items)
		{
			var dp = itemsControl.ItemContainerGenerator.ContainerFromItem(value);

			if (dp != null)
				return dp;

			var currentTreeViewItem = itemsControl.ItemContainerGenerator.ContainerFromItem(item);

			var childDp = ContainerFromItem(currentTreeViewItem as ItemsControl, value);

			if (childDp != null)
				return childDp;
		}
		return null;
	}

	public static T ContainerFromItem<T>(this ItemsControl itemsControl, object value) where T : class
	{
		return ContainerFromItem(itemsControl, value) as T;
	}
}
