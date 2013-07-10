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
            //if (e.Cameras.Any())
            //    e.SelectedCamera = e.Cameras.Last();
            int j = 0;
            string _name = getSelectDeveice();
            for (int i = 0; i <= e.Cameras.Count - 1; i++)
            {
                if (_name == e.Cameras[i].Name)
                {
                    j = i;
                    break;
                }
            }
            e.SelectedCamera = e.Cameras[j];
		}

        private string getSelectDeveice()
        {
            string result="";
            string Name="";
            string PictureTaken="";
            try{
            		WIA.CommonDialog dlg = new WIA.CommonDialog();

					// show user the WIA device dialog
					Device _camera = dlg.ShowSelectDevice(WiaDeviceType.CameraDeviceType, true, false);

					// check if a device was selected or user press Cancel
					if (_camera != null)
					{
						// Print camera properties               
						foreach (Property p in _camera.Properties)
						{
							switch (p.Name)
							{

								case "Name":									
									Name = p.get_Value();
									break;
								case "Pictures Taken":
									int imageNo = p.get_Value();
									PictureTaken = imageNo.ToString();
									log.Info("Total pictures= "+ imageNo.ToString() );
									break;
								default:									
									break;
							}						
						}
						dlg = null;
					}
					else
					{						
						// log.error("doService() return error!");
					}
                    result = Name;
				}
				catch (Exception ex)
				{
				//	MessageBox.Show(ex.Message, "WIA Error: doService()", MessageBoxButtons.OK, MessageBoxIcon.Error);			
                    result = "";
				}
			
					return result;
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