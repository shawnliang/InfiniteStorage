using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
	}
}