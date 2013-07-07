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
            //
            _acc = new camerAccess();
            _acc.startListen();
        }

        public void startListen()
        {
            if (_acc == null)
            {
                _acc = new camerAccess();
                _acc.startListen();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
           
            _acc = new camerAccess();
            _acc.startListen(); 
        }

        private void readCameraServiceStart()
        {
            try
            {
                bool result = _acc.GetPictures();
                if (result == false)
                {
                    _acc = new camerAccess();
                    _acc.startListen();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            _acc = null;
            _acc = new camerAccess();
        }

        public void setWhere(string tempFilePath)
        {
            if (tempFilePath != "")
            {
                where = tempFilePath;
                _acc.where = where;
            }
        }

        public static string where = @"C:\00000000";

        private void readCameraForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _acc = null;

        }

    }


}

