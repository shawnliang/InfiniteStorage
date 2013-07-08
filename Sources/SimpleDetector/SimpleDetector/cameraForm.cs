using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Dolinay;
using WIA;
using System.Threading;
using readWIALibrary;
using System.Runtime.InteropServices;


namespace SimpleDetector
{
    public partial class cameraForm : Form
    {
        private DriveDetector driveDetector = null;

        const string wiaFormatPNG = "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}";
        const string wiaFormatBMP = "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}";
        const string wiaFormatJPEG = "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}";
        const string wiaFormatTIFF = "{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}";
        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public const int WM_CLOSE = 0x10;
        readWIALibrary.wiaService _svr = new wiaService();
        _deviceInfo _dev = new _deviceInfo();
        public cameraForm()
        {
            //this.WindowState = FormWindowState.Minimized;
            InitializeComponent();
            driveDetector = new DriveDetector();
            driveDetector.DeviceArrived += new DriveDetectorEventHandler(OnDriveArrived);
            driveDetector.DeviceRemoved += new DriveDetectorEventHandler(OnDriveRemoved);
            driveDetector.QueryRemove += new DriveDetectorEventHandler(OnQueryRemove);
        }

        // Called by DriveDetector when removable device in inserted 
        private void OnDriveArrived(object sender, DriveDetectorEventArgs e)
        {

            this.WindowState = FormWindowState.Normal;
            //// Report the event in the listbox.
            //// e.Drive is the drive letter for the device which just arrived, e.g. "E:\\"
            string s = "Drive arrived " + e.Drive;
            listBox1.Items.Add(s);

            // If you want to be notified when drive is being removed (and be able to cancel it), 
            // set HookQueryRemove to true 
           //timer1.Enabled=true;   
            Thread thread = new Thread(getWIAservice);          // work in another thread
            thread.IsBackground = true;
            thread.Start();
        }

        // Called by DriveDetector after removable device has been unpluged 
        private void OnDriveRemoved(object sender, DriveDetectorEventArgs e)
        {
            // TODO: do clean up here, etc. Letter of the removed drive is in e.Drive;
            
            // Just add report to the listbox
            string s = "Drive removed " + e.Drive;
            listBox1.Items.Add(s);
        }

        // Called by DriveDetector when removable drive is about to be removed
        private void OnQueryRemove(object sender, DriveDetectorEventArgs e)
        {
            // Should we allow the drive to be unplugged?
         
        }
        

        // User checked the "Ask me before drive can be disconnected box"        
        private void checkBoxAskMe_CheckedChanged(object sender, EventArgs e)
        {
   
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();            
        }

        string DeviceID;
        string _label = null;
        string deviceName = "";
        string[] arr;
        private void getWIAservice()
        {
            try
            {
                char[] delimiterChars = { '~' };

                string result = _svr.doService();
                arr = result.Split(delimiterChars);
                _dev.UID = arr[0];
                _dev.Manufacturer = arr[1];
                _dev.Description = arr[2];
                _dev.Name = arr[3];
                richTextBox1.Text = "Unique Device ID: " + _dev.UID + "\n Manufacturer: " + _dev.Manufacturer + "\n Description:" + _dev.Description + "\n Name: " + _dev.Name;
            }
            catch
            {
                richTextBox1.Text = "failed!";
            }
            //
            if (MessageBox.Show("Copy to Favorite?", "Confirm Yes/No", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                string[] filePaths = Directory.GetFiles(_dev.Name);
                for (int i = 0; i <= filePaths.Length - 1; i++)
                {
                    listBox1.Items.Add(filePaths[i]);
                    richTextBox1.Text = richTextBox1.Text + filePaths[i] + "\n";
                }
                DirectoryInfo di = new DirectoryInfo(_dev.Name);
                DirectoryInfo si = new DirectoryInfo(targetPath);
                DeepCopy(di, si);
                StartKiller();
                MessageBox.Show( "Done!","Favorite Message");
            }
        }

        private void StartKiller()
        {
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           KillMessageBox();
           timer1.Enabled = false;
        }  

        private void Timer_Tick(object sender, EventArgs e)
        {
 
        }

        private void KillMessageBox()
        {
            //依MessageBox的標題,找出MessageBox的視窗
            IntPtr ptr = FindWindow(null, "Favorite Message");
            if (ptr != IntPtr.Zero)
            {
                //找到則關閉MessageBox視窗
                PostMessage(ptr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            getWIAservice();
        }

        // file copy
        string targetPath = "C:/00000000";
        private void button2_Click(object sender, EventArgs e)
        {
            string[] filePaths = Directory.GetFiles(_dev.Name);
            for (int i = 0; i <= filePaths.Length - 1; i++)
            {
                listBox1.Items.Add(filePaths[i]);
                richTextBox1.Text = richTextBox1.Text + filePaths[i]+"\n";
            }
            DirectoryInfo di = new DirectoryInfo(_dev.Name);
            DirectoryInfo si = new DirectoryInfo(targetPath);
            DeepCopy(di, si);
            MessageBox.Show("Done !");
        }
        public void DeepCopy(DirectoryInfo source, DirectoryInfo target)
        {
            try
            {
                // Recursively call the DeepCopy Method for each Directory
                foreach (DirectoryInfo dir in source.GetDirectories())
                    DeepCopy(dir, target.CreateSubdirectory(dir.Name));

                // Go ahead and copy each file in "source" to the "target" directory
                foreach (FileInfo file in source.GetFiles())
                {
                    file.CopyTo(Path.Combine(target.FullName, file.Name), true);
                    listBox1.Items.Add(source.FullName);
                }
            }
            catch
            {
                // log
            }
        }


    }
    public class _deviceInfo
    {
        public string Name { get; set; }
        public string UID { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        //public string
    }
}
