using System;
using System.Windows;
using System.Windows.Controls;

public static class TreeViewExtension
{
	public static void ClearSelection(this TreeView treeview)
	{
		var selectedItem = ContainerFromSelectedItem<TreeViewItem>(treeview);

		if (selectedItem == null) return;

		selectedItem.IsSelected = false;

	}

	public static DependencyObject ContainerFromSelectedItem(this TreeView treeview)
	{
		var selectedItem = treeview.SelectedItem;

		if (selectedItem == null)
			return null;

		return treeview.ContainerFromItem(selectedItem);
	}

	public static T ContainerFromSelectedItem<T>(this TreeView treeview) where T : class
	{
		return ContainerFromSelectedItem(treeview) as T;
	}
}
