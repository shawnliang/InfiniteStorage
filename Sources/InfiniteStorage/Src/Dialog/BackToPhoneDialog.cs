using InfiniteStorage.Properties;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using InfiniteStorage.WebsocketProtocol;
using InfiniteStorage.Win32;


namespace InfiniteStorage
{
	public partial class BackToPhoneDialog : Form
	{
		private static object cs = new object();
		private static List<BackToPhoneDialog> OpenDialogs = new List<BackToPhoneDialog>();

		public BackToPhoneDialog()
		{
			InitializeComponent();
		}

		private void BackToPhoneDialog_Load(object sender, EventArgs e)
		{
			this.Icon = Resources.ProductIcon;
			this.Text = Resources.ProductName;
		}

		private void BackToPhoneDialog_Shown(object sender, EventArgs e)
		{
			lock (cs)
			{
				OpenDialogs.Add(this);
			}
		}

		private void BackToPhoneDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			lock (cs)
			{
				OpenDialogs.Remove(this);
			}
		}

		public ProtocolContext Ctx
		{
			get;
			set;
		}

		public static void CloseOpenedWindow(ProtocolContext ctx)
		{
			List<BackToPhoneDialog> opendDialogs;

			lock (cs)
			{
				opendDialogs = OpenDialogs.Where(x => x.Ctx == ctx).ToList();
			}
			
			foreach (var dialog in opendDialogs)
			{
				SynchronizationContextHelper.SendMainSyncContext(() => { dialog.Close(); });
			}
			
		}
	}
}
