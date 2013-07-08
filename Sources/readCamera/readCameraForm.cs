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
using System.Security.Cryptography;
using System.Management;
using System.Threading;
using System.Runtime.InteropServices;
using log4net;


namespace readCamera
{
	public partial class readCameraForm : Form
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(readCameraForm));

		const string wiaFormatPNG = "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}";
		const string wiaFormatBMP = "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}";
		const string wiaFormatJPEG = "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}";
		const string wiaFormatTIFF = "{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}";

		[DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
		private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
		public const int WM_CLOSE = 0x10;

		System.Management.ManagementEventWatcher watcher;
		NullImportService _svr = new NullImportService();


		public event EventHandler<CameraDetectedEventArgs> CameraDetected;


		public readCameraForm()
		{
			InitializeComponent();
			log.Warn("readCameraForm: start");
		}

		int i = 0;
		private void watcher_EventArrived(object sender, EventArrivedEventArgs e)
		{
			i++;
			if (i == 3)
			{
				readCameraServiceStart();
				i = 0;
			}
		}

		public void startListen()
		{
			if (watcher != null)
			{
				watcher.Dispose();
				watcher = null;
				Thread.Sleep(1000);
			}
			watcher = new System.Management.ManagementEventWatcher();
			var query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
			watcher.EventArrived += new EventArrivedEventHandler(watcher_EventArrived);
			watcher.Query = query;
			watcher.Start();
		}
		
		public void StopListen()
		{
			if (watcher != null)
			{
				watcher.Stop();
				watcher.Dispose();
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			startListen();
		}

		private void readCameraServiceStart()
		{
			var cameras = populateCameraDevice();

			var handler = CameraDetected;
			if (handler == null)
				return;

			var args = new CameraDetectedEventArgs(cameras);
			handler(this, args);

			if (args.SelectedCamera != null)
				GetPictures(args.SelectedCamera);
		}

		private void button5_Click(object sender, EventArgs e)
		{
			readCameraServiceStart();
		}

		private void GetPictures(_deviceInfo dev)
		{
			
			try
			{
				if (!_svr.device_exist(dev.UID))
					_svr.create_device(dev.UID, dev.Name);

				findImageItem(_camera.Items, fullitempath);

			}
			catch (Exception err)
			{
				string error = err.Message;
			}
		}

		static List<string> imageName = new List<string>();
		//void findImageItem(Items items, string _fullitempath)
		//{
		//    WIA.ImageFile wiaImageFile = null;
		//    string trueName = "";
		//    foreach (Item item in items)
		//    {
		//        var flag = item.Properties["Item Flags"];

		//        if (flag != null)
		//        {
		//            var val = flag.get_Value();
		//            try
		//            {
		//                if ((val & (int)WiaItemFlag.FolderItemFlag) != 0)
		//                {
		//                    Console.WriteLine("Folder: " + item.Properties["Item Name"].get_Value());
		//                    _fullitempath = _fullitempath + @"\" + item.Properties["Full Item Name"].get_Value();

		//                    findImageItem(item.Items, _fullitempath);

		//                    continue;
		//                }
		//            }
		//            catch (Exception err)
		//            {
		//                log.Error("findImageItem:  flag.get_Value(), error: " + err.Message);
		//                continue;
		//            }
		//            if ((val & (int)WiaItemFlag.ImageItemFlag) != 0)
		//            {
		//                foreach (Property itemProp in item.Properties)
		//                {
		//                    //Console.WriteLine(itemProp.Name + ":" + itemProp.get_Value());
		//                    trueName = item.Properties["Item Name"].get_Value();
		//                    imageName.Add(_fullitempath + @"\" + trueName + ".jpg");

		//                    //}

		//                    wiaImageFile = (WIA.ImageFile)item.Transfer(wiaFormatJPEG);

		//                    DateTime dateTime = new DateTime();
		//                    foreach (IProperty prop in wiaImageFile.Properties)
		//                    {
		//                        // Camera datetime format is a little strange:
		//                        // DateTime : 2008:10:28 22:20:45
		//                        if (prop.Name == "DateTime")
		//                        {
		//                            // replace the colons in the date by dashes
		//                            StringBuilder sdate = new StringBuilder(prop.get_Value().ToString());
		//                            sdate[4] = '-';
		//                            sdate[7] = '-';
		//                            dateTime = DateTime.Parse(sdate.ToString());
		//                            break;
		//                        }
		//                    }
		//                    bool isfileexist = _svr.is_file_exist(_dev.UID, _fullitempath + @"\" + trueName + ".jpg");
		//                    if (isfileexist == false)
		//                    {
		//                        _svr.copy_file(null, _fullitempath + @"\" + trueName + ".jpg", FileType.Image, dateTime, _dev.UID);
		//                        wiaImageFile.SaveFile(@"C:\00000000\" + trueName + ".jpg");
		//                        log.Info("copy file: " + _fullitempath + @"\" + trueName + ".jpg" + " / " + FileType.Image + "/ " + dateTime.ToString() + " / " + _dev.UID);
		//                    }
		//                    break;
		//                }
		//                continue;
		//            }

		//        }
		//    }
		//}
	
		private void button2_Click_1(object sender, EventArgs e)
		{
			startListen();
		}


		private List<_deviceInfo> populateCameraDevice()
		{
			var allCameras = new List<_deviceInfo>();

			var devMgr = new DeviceManager();
			if (devMgr.DeviceInfos != null)
			{
				foreach (IDeviceInfo dev in devMgr.DeviceInfos)
				{
					if (dev.Type != WiaDeviceType.CameraDeviceType && dev.Type != WiaDeviceType.VideoDeviceType)
						continue;

					var devId = stringToGuid(dev.DeviceID);
					var devName = dev.Properties.Exists("Name") ? dev.Properties["Name"].get_Value().ToString() : "Unknown device name";
					var description = dev.Properties.Exists("Description") ? dev.Properties["Description"].get_Value().ToString() : "";
					var manufacrurer = dev.Properties.Exists("Manufacturer") ? dev.Properties["Manufacturer"].get_Value().ToString() : "";

					allCameras.Add(new _deviceInfo { UID = devId, Name = devName, Description = description, Manufacturer = manufacrurer });
				}
			}

			return allCameras;
		}


		private static string stringToGuid(string str)
		{
			using (var md5 = MD5.Create())
			{
				var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
				var byte16 = hash.Take(16).ToArray();
				return new Guid(byte16).ToString();
			}
		}
	}



	public class _deviceInfo
	{
		public string Name { get; set; }
		public string UID { get; set; }
		public string Manufacturer { get; set; }
		public string Description { get; set; }
		public string Root { get; set; }

		public override string ToString()
		{
			return Name.ToString();
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}
	}

	public class CameraDetectedEventArgs : EventArgs
	{
		public List<_deviceInfo> Cameras { get; set; }
		public _deviceInfo SelectedCamera { get; set; }
		
		public CameraDetectedEventArgs(List<_deviceInfo> devices)
		{
			Cameras = devices;
		}
	}

}

