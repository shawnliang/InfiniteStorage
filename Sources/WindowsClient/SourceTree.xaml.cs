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
	public partial class SourceTree : UserControl
	{
		public object SelectedItem 
		{
			get
			{
				return lbxDeviceContainer.SelectedItem;
			}
		}

		public SourceTree()
		{
			this.InitializeComponent();
		}
	}
}