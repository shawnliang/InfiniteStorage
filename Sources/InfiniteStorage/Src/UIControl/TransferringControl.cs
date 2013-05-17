using InfiniteStorage.WebsocketProtocol;
using System;
using System.Windows.Forms;

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
