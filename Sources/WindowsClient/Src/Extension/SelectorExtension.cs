using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;

public static class SelectorExtension
{
	public static DependencyObject ContainerFromSelectedItem(Selector selector)
	{
		var selectedItem = selector.SelectedItem;

		if (selectedItem == null)
			return null;

		return selector.ContainerFromItem(selectedItem);
	}

	public static T ContainerFromSelectedItem<T>(Selector selector) where T : class
	{
		return ContainerFromSelectedItem(selector) as T;
	}
}
