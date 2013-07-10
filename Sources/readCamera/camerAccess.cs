using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using WIA;
using System.Management;
using System.IO;
using log4net;
using System.Security.Cryptography;


namespace readCamera
{
	public partial class camerAccess : Form
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(camerAccess));
		
		const string wiaFormatPNG = "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}";
		const string wiaFormatBMP = "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}";
		const string wiaFormatJPEG = "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}";
		const string wiaFormatTIFF = "{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}";


		public ImportService ImportService { get; set; }
		public event EventHandler<CameraDetectedEventArgs> CameraDetected;

		private IStorage storage = null;
		private System.Management.ManagementEventWatcher watcher;


		public camerAccess()
		{
			InitializeComponent();
			ImportService = new NullImportService();
		}

		public void startListen()
		{
			watcher = new System.Management.ManagementEventWatcher();
			var query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
			watcher.EventArrived += new EventArrivedEventHandler(watcher_EventArrived);
			watcher.Query = query;
			watcher.Start();
		}

		public void stopListen()
		{
			if (watcher != null)
			{
				watcher.Stop();
				watcher.Dispose();
				watcher = null;
			}
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


		public void readCameraServiceStart()
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
					var picturesTaken = dev.Properties.Exists("Pictures Taken") ? dev.Properties["Pictures Taken"].get_Value().ToString() : "";

					allCameras.Add(new _deviceInfo { UID = devId, Name = devName, Description = description, Manufacturer = manufacrurer, DeviceInfo = dev, PictureTaken = picturesTaken });
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


		private void GetPictures(_deviceInfo dev)
		{
			storage = ImportService.GetStorage(dev.UID, dev.Name);

			var device = dev.DeviceInfo.Connect();

			findImageVideoItems(device.Items, itemFound);
		}

		private delegate void ItemCallback(Item item, string path);


		static void findImageVideoItems(Items items, ItemCallback cb)
		{
			var queue = new Queue<QueueItem>();
			var curPath = @"";

			foreach (Item item in items)
			{
				procItem(queue, item, curPath, cb);
			}


			while (queue.Count > 0)
			{
				var item = queue.Dequeue();
				foreach (Item subIten in item.item.Items)
					procItem(queue, subIten, item.parent, cb);
			}
		}

		private static void procItem(Queue<QueueItem> queue, Item item, string parent, ItemCallback cb)
		{
			Property flag;
			try
			{
				flag = item.Properties["Item Flags"];
			}
			catch (Exception err)
			{
				return;
			}

			if (flag != null)
			{
				var val = flag.get_Value();

				if ((val & (int)WiaItemFlag.FolderItemFlag) != 0)
				{
					string itemName;
					try
					{
						itemName = item.Properties["Item Name"].get_Value().ToString();
					}
					catch (Exception err)
					{
						return;
					}

					queue.Enqueue(new QueueItem { item = item, parent = Path.Combine(parent, itemName) });
				}
				else if ((val & (int)WiaItemFlag.ImageItemFlag) != 0)
				{
					cb(item, parent);
				}
				else if ((val & (int)WiaItemFlag.VideoItemFlag) != 0)
				{
					cb(item, parent);
				}
			}
		}

		void itemFound(Item item, string parent)
		{
			try
			{
				
				if (!item.Properties.Exists("Item Name"))
					return;

				var name = item.Properties["Item Name"].get_Value().ToString();
				var format = item.Properties["Format"].get_Value().ToString();
				var extension = item.Properties["Filename extension"].get_Value().ToString();
				var path = Path.Combine(parent, name);

				if (storage.IsFileExist(path))
					return;

				var flags = item.Properties["Item Flags"].get_Value();

				FileType type = getFileTypeFromItemFlags(flags);

				var tranferItem = (WIA.ImageFile)item.Transfer(format);

				var tempFile = Path.Combine(storage.TempFolder, Guid.NewGuid().ToString() + "." + extension);
				tranferItem.SaveFile(tempFile);

				storage.AddToStorage(tempFile, type, DateTime.Now, Path.Combine(parent, name + "." + extension));
			}
			catch (Exception err)
			{
				Console.WriteLine(err);
			}
		}

		private static FileType getFileTypeFromItemFlags(dynamic flags)
		{
			if ((flags & (int)WiaItemFlag.ImageItemFlag) != 0)
				return FileType.Image;
			else if ((flags & (int)WiaItemFlag.VideoItemFlag) != 0)
				return FileType.Video;
			else
				throw new ArgumentException("Nither image nor video flags: " + flags.ToString());
		}

	}

	public class _deviceInfo
	{
		public string Name { get; set; }
		public string UID { get; set; }
		public string Manufacturer { get; set; }
		public string Description { get; set; }
		public string Root { get; set; }
		public string PictureTaken { get; set; }
		public IDeviceInfo DeviceInfo { get; set; }
		

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

	class QueueItem
	{
		public Item item { get; set; }
		public string parent { get; set; }
	}

}
