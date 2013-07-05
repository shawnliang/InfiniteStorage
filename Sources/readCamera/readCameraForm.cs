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

        const string wiaFormatPNG = "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}";
        const string wiaFormatBMP = "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}";
        const string wiaFormatJPEG = "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}";
        const string wiaFormatTIFF = "{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}";

        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public const int WM_CLOSE = 0x10;
        _deviceInfo _dev = new _deviceInfo();

        System.Management.ManagementEventWatcher watcher;
        NullImportService _svr;

        public readCameraForm()
        {
            InitializeComponent();
            log.Warn("readCameraForm: start");
            _svr = new NullImportService();
        }

        int i = 0;
        private void watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            i++;
            if (i == 3)
            {
                readCameraServiceStart();
                i = 0;
                //if (watcher != null)
                //{
                //    watcher.Stop();
                //    watcher.Dispose();
                //    watcher = null;
                //}
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


        private void button1_Click(object sender, EventArgs e)
        {
            startListen();
        }

        private void readCameraServiceStart()
        {
            log.Warn("readCameraServiceStart()");
            GetPictures();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            readCameraServiceStart();
        }

        public void setWhere(string tempFilePath)
        {
            if (tempFilePath != "")
            {
                where = tempFilePath;
            }
        }



        public string where = @"C:\00000000";
        WIA.Device _Acamera;
        private void GetPictures()
       { 
            List<string> NewNames = new List<string>();

            _svr = new NullImportService();
            //WIA.ImageFile wiaImageFile = null;
            try
            {

                string getUID=doService();
                if (getUID == "")
                {
                    // log.error("doService() return error!");
                    return;
                }
                    // device exist
                bool isdevice_exist=_svr.device_exist(_dev.UID);
                if (isdevice_exist == false)
                {
                    //   create_device            
                    _svr.create_device(_dev.UID, _dev.Name);
                }

                // if(wai == null)
                WIA.Device _camera = _Acamera;  // dialog.ShowSelectDevice(WIA.WiaDeviceType.CameraDeviceType, false, true);             

                if (MessageBox.Show("Copy to Favorite?", "Confirm Yes/No", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
                int no = 0;
                //return;
                string _tempName="";
                string fullitempath=_dev.Root;
                findImageItem(_camera.Items,fullitempath);
            }
            catch( Exception err)
            {
                string error = err.Message;
                MessageBox.Show("error: GetPictures(): " + err.Message);
            }
            StartKiller();
            MessageBox.Show("Done!", "Favorite Message");
            //Thread thread = new Thread(worker_DoWork);          // work in another thread
            //thread.IsBackground = true;
            //thread.Start();
           
            _Acamera = null;
            _dev = null;
            _dev = new _deviceInfo();

        }

       // static List<string> imageName = new List<string>();
        void findImageItem(Items items, string _fullitempath)
        {
            WIA.ImageFile wiaImageFile = null;
            string trueName = "";
            foreach (Item item in items)
            {
                var flag = item.Properties["Item Flags"];
                try
                {
                    if (flag != null)
                    {

                        var val = flag.get_Value();
                        try
                        {
                            if ((val & (int)WiaItemFlag.FolderItemFlag) != 0)
                            {
                                //  Console.WriteLine("Folder: " + item.Properties["Item Name"].get_Value());
                                _fullitempath = _fullitempath + @"\" + item.Properties["Full Item Name"].get_Value();

                                findImageItem(item.Items, _fullitempath);

                                continue;
                            }
                        }
                        catch (Exception err)
                        {
                            //log.error("FindImageItem: WiaItemFlagflag error "+err.Message);
                            MessageBox.Show("FindImageItem: WiaItemFlagflag error " + err.Message);
                            continue;
                        }


                        if ((val & (int)WiaItemFlag.ImageItemFlag) != 0)
                        {
                            foreach (Property itemProp in item.Properties)
                            {
                                Console.WriteLine(itemProp.Name + ":" + itemProp.get_Value());
                                trueName = item.Properties["Item Name"].get_Value();
                                //imageName.Add(_fullitempath + @"\" + trueName + ".jpg");

                                wiaImageFile = (WIA.ImageFile)item.Transfer(wiaFormatJPEG);

                                DateTime dateTime = new DateTime();
                                foreach (IProperty prop in wiaImageFile.Properties)
                                {
                                    // Camera datetime format is a little strange:
                                    // DateTime : 2008:10:28 22:20:45
                                    if (prop.Name == "DateTime")
                                    {
                                        // replace the colons in the date by dashes
                                        StringBuilder sdate = new StringBuilder(prop.get_Value().ToString());
                                        sdate[4] = '-';
                                        sdate[7] = '-';
                                        dateTime = DateTime.Parse(sdate.ToString());
                                        break;
                                    }
                                }
                                bool isfileexist = _svr.is_file_exist(_dev.UID, _fullitempath + @"\" + trueName + ".jpg");
                                if (isfileexist == false)
                                {
                                    // _svr.copy_file(null, _fullitempath + @"\" + trueName + ".jpg", FileType.Image, dateTime.ToString() , _dev.UID);
                                    wiaImageFile.SaveFile(@"C:\00000000\" + trueName + ".jpg");
                                }
                                break;
                            }
                            continue;
                        }
                    }
                }
                catch (Exception err)
                {
                    log.Error("FindImageItem:  "+err.Message);
                   // MessageBox.Show("FindImageItem:  " + err.Message);
                }

            }

        }
        private void worker_DoWork(object obj)
        {
            startListen();
            return;
        }
        private void StartKiller()
        {
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            KillMessageBox();
            timer1.Enabled = false;
            startListen();
        }

        private void KillMessageBox()
        {
            IntPtr ptr = FindWindow(null, "Favorite Message");
            if (ptr != IntPtr.Zero)
            {
                PostMessage(ptr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            startListen();
        }

        string UniqueDeviceID = "";
        string Manufacturer = "";
        string Name = "";
        string Description = "";

        public string doService()
        {
            string _r = "";
            try
            {
                // create a new WIA common dialog box for the user to select a device from
                WIA.CommonDialog dlg = new WIA.CommonDialog();

                // show user the WIA device dialog
                Device d = dlg.ShowSelectDevice(WiaDeviceType.CameraDeviceType, false, false);
                _Acamera = d;
                // check if a device was selected
                if (d != null)
                {
                    // Print camera properties               
                    foreach (Property p in d.Properties)
                    {
                        switch (p.Name)
                        {
                            case "Full Item Name":
                                _dev.Root = p.get_Value();
                                break;
                            case "Unique Device ID":
                                UniqueDeviceID = p.get_Value();
                                int i0 = UniqueDeviceID.IndexOf("}");
                                if (i0 >= 0)
                                {
                                    UniqueDeviceID = UniqueDeviceID.Substring(1, i0 - 2);
                                }
                                _dev.UID = UniqueDeviceID;
                                break;
                            case "Manufacturer":
                                Manufacturer = p.get_Value();
                                _dev.Manufacturer = Manufacturer;
                                break;
                            case "Description":
                                Description = p.get_Value();
                                _dev.Description = Description;
                                break;
                            case "Name":
                                Name = p.get_Value();
                                _dev.Name = Name;
                                break;
                        }
                    }
                }
                else
                {
                    d = null;
                }
                _r = _dev.UID + "~" + _dev.Manufacturer + "~" + _dev.Name + "~" + _dev.Description;
            }
            catch (Exception ex)
            {
                log.Error( "WIA Error: doService(): " + MessageBoxIcon.Error);
                _r = "";
            }
            return _r;
        }
    }
	public class _deviceInfo
	{
		public string Name { get; set; }
		public string UID { get; set; }
		public string Manufacturer { get; set; }
        public string Description { get; set; }
        public string Root { get; set; }
        //public string
	}
	}

