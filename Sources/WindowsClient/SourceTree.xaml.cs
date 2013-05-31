using System;
using System.Windows;
using System.Windows.Controls;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for SourceTree.xaml
	/// </summary>
	public partial class SourceTree : TreeView
	{
		#region Event
		public event EventHandler TreeViewItemClick;
		#endregion

		public SourceTree()
		{
			this.InitializeComponent();
		}


		private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (TreeViewItemClick == null)
				return;

			TreeViewItemClick(sender, EventArgs.Empty);
		}

		private void unsortedItem_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// TODO: Add event handler implementation here.
			MessageBox.Show("unsortedItem_PreviewMouseDown");
		}
	}
}