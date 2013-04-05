using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Gui
{
	public partial class FatalErrorStep : SharpSetup.UI.Forms.Modern.ModernInfoStep
	{
		public FatalErrorStep(string message)
		{
			InitializeComponent();
			lblDetails.Text = message;
		}

		public FatalErrorStep(string message, string title)
		{
			InitializeComponent();
			lblDetails.Text = message;
			lblTitle.Text = title;
		}
	}
}
