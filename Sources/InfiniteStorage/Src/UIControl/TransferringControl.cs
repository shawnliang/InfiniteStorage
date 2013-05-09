using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InfiniteStorage.WebsocketProtocol;
using InfiniteStorage.Properties;

namespace InfiniteStorage
{
	public partial class TransferringControl : UserControl
	{
		public ProtocolContext WebSocketContext { get; set; }


		public TransferringControl()
		{
			InitializeComponent();
		}

		

		private void TransferringControl_Load(object sender, EventArgs e)
		{
			//timer1.Start();
		}

		public void StopUpdateUI()
		{
			//timer1.Stop();
		}
	}
}
