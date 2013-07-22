#region

using System.Windows.Controls;
using System.Windows.Input;

#endregion

namespace Waveface.Client
{
	public partial class RecentListBox : ListBox
	{
		public RecentListBox()
		{
			InitializeComponent();
		}

		private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
		}
	}
}