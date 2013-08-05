#region

using System;
using System.Windows.Controls;
using System.Windows.Input;

#endregion

namespace Waveface.Client
{
	public partial class AddSourceTip : UserControl
	{
		public event EventHandler<MouseButtonEventArgs> TipOneMouseDown;
		public event EventHandler<MouseButtonEventArgs> TipThreeMouseDown;

		public AddSourceTip()
		{
			InitializeComponent();
		}

		private void tbHelpItem3_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var handler = TipThreeMouseDown;
			if (handler != null)
				handler(this, e);
		}

		private void tbHelpItem1_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var handler = TipOneMouseDown;
			if (handler != null)
				handler(this, e);
		}
	}
}