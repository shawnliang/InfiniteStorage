using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WIA;
using System.Management;
using System.Threading;
using System.Runtime.InteropServices;
using log4net;


namespace readCamera
{
	public partial class readCameraForm : Form
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(readCameraForm));
		camerAccess _acc;
		public readCameraForm()
		{
			InitializeComponent();

			_acc = new camerAccess();
			_acc.CameraDetected += _acc_CameraDetected;
			_acc.startListen();
		}

		void _acc_CameraDetected(object sender, CameraDetectedEventArgs e)
		{
			if (e.Cameras.Any())
				e.SelectedCamera = e.Cameras.First();
		}

		private void button1_Click(object sender, EventArgs e)
		{

			_acc = new camerAccess();
			_acc.startListen();
		}


		private void readCameraForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			_acc.stopListen();
			_acc = null;

		}

	}
}