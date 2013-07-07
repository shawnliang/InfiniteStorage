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


namespace readCamera
{
    public partial class camerAccess : Form
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(camerAccess));
        public camerAccess()
        {
            InitializeComponent();
        }
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
        NullImportService _svr;

        public void startListen()
        {
            System.Management.ManagementEventWatcher watcher = new System.Management.ManagementEventWatcher();

            var query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
            watcher.EventArrived += new EventArrivedEventHandler(watcher_EventArrived);
            watcher.Query = query;
            watcher.Start();
        }

        int i = 0;
        private void watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            i++;
            if (i == 3)
            {
                GetPictures();
                i = 0;
            }
        }
        public string where = @"C:\00000000";
        public bool GetPictures()
        {
            bool result = false;
            _svr = new  NullImportService();
            Device _camera = null;

            try
            {
               
                try
                {
                    // create a new WIA common dialog box for the user to select a device from
                    WIA.CommonDialog dlg = new WIA.CommonDialog();

                    // show user the WIA device dialog
                    _camera = dlg.ShowSelectDevice(WiaDeviceType.CameraDeviceType, true, false);

                    // check if a device was selected or user press Cancel
                    if (_camera != null)
                    {
                        bool breakflag = false;
                        // Print camera properties               
                        foreach (Property p in _camera.Properties)
                        {
                            switch (p.Name)
                            {
                                case "Full Item Name":
                                    _dev.Root = p.get_Value();
                                    break;
                                case "Unique Device ID":
                                    _dev.UID = p.get_Value();
                                    int i0 = _dev.UID.IndexOf("}");
                                    if (i0 >= 0)
                                    {
                                        _dev.UID = _dev.UID.Substring(1, i0 - 2);
                                    }
                                    break;
                                case "Manufacturer":
                                    _dev.Manufacturer = p.get_Value();
                                    break;
                                case "Description":
                                    _dev.Description = p.get_Value();
                                    break;
                                case "Name":

                                    _dev.Name = p.get_Value();
                                    break;
                                case "Pictures Taken":
                                    int imageNo = p.get_Value();
                                    _dev.PictureTaken = imageNo.ToString();
                                    break;
                                default:
                                    breakflag = true;
                                    break;
                            }
                            if (breakflag == true)
                                break;
                        }
                        dlg = null;
                    }
                    else
                    {
                        
                        log.Error("doService() return error!");
                    }
                }
                catch (Exception ex)
                {
                   // MessageBox.Show(ex.Message, "WIA Error: doService()", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    log.Error("WIA Error: doService() "+ ex.Message);
                   
                    _camera = null;
                }

                // user press Cancel
                if (_camera == null)
                {
                    _camera = null;
                    _dev = null;
                    _dev = new _deviceInfo();
                    result = false;
                    return result;
                }

                //API> device exist
                bool isdevice_exist = _svr.device_exist(_dev.UID);
                if (isdevice_exist == false)
                {
                    //API>   create_device            
                    _svr.create_device(_dev.UID, _dev.Name);
                }

                // prompt copy to favorite or not?
                if (MessageBox.Show("Copy to Favorite?", "Confirm Yes/No", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return result;
                }
                result = true;
            }
            catch (Exception err)
            {
                //MessageBox.Show("error: GetPictures(): " + err.Message);
                log.Error("error: GetPictures(): " + err.Message);
                result = false;
            }
            
            try
            {
                string fullitempath = _dev.Root;
                findImageItem(_camera.Items, fullitempath);
            }
            catch
            {
                result = false;
            }
            StartKiller();
            MessageBox.Show("Done!", "Favorite Message");

            _camera = null;
            _dev = null;
            _dev = new _deviceInfo();
            result = true;
            return result;
        }

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
                            if (item.Properties.Exists(WiaItemFlag.FolderItemFlag))
                            {
                                if ((val & (int)WiaItemFlag.FolderItemFlag) != 0)
                                {
                                    _fullitempath = _fullitempath + @"\" + item.Properties["Full Item Name"].get_Value();
                                    findImageItem(item.Items, _fullitempath);

                                    continue;
                                }
                            }
                        }
                        catch (Exception err)
                        {
                            log.Error("FindImageItem: WiaItemFlagflag error "+err.Message);
                            //MessageBox.Show("FindImageItem: WiaItemFlagflag error " + err.Message);
                            continue;
                        }

                        if (item.Properties.Exists(WiaItemFlag.ImageItemFlag))
                        {
                            if ((val & (int)WiaItemFlag.ImageItemFlag) != 0)
                            {
                                foreach (Property itemProp in item.Properties)
                                {
                                    if (item.Properties.Exists("Item Name"))
                                    {
                                        trueName = item.Properties["Item Name"].get_Value();                               

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
                                            if (!File.Exists(where + @"\" + trueName + ".jpg"))
                                            {
                                                wiaImageFile.SaveFile(where + @"\" + trueName + ".jpg");
                                            }
                                            else
                                            {
                                               // MessageBox.Show("Image alread exist:(" + where + @"\" + trueName + ".jpg)");
                                                log.Warn("Image alread exist:(" + where + @"\" + trueName + ".jpg)");
                                            }
                                        }
                                        break;
                                    }
                                }
                                continue;
                            }
                        }
                    }

                }
                catch (Exception err)
                {
                    // log.error(err.Message);
                    MessageBox.Show("FindImageItem:  " + err.Message);
                }

            }
            wiaImageFile = null;
        }
        private void StartKiller()
        {
            timer1.Enabled = true;
        }

        private void KillMessageBox()
        {
            IntPtr ptr = FindWindow(null, "Favorite Message");
            if (ptr != IntPtr.Zero)
            {
                PostMessage(ptr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            KillMessageBox();
            timer1.Enabled = false;
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
        //public string
    }

}
