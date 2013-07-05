using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace caller
{
    public partial class serviceForm : Form
    {
        public serviceForm()
        {
            InitializeComponent();
            //
            fillSpec();
        }
        public string curDeviceId = "";
        public string curDeviceName = "";
        public bool device_exist(string device_id)
        {
            bool result = false;
            if (curDeviceId == device_id)
                result = true;
            return result;
        }

        public void create_device(string device_id, string device_name)
        {
            if (curDeviceId != device_id)    // already exist
            {
                curDeviceName = device_name;
            }
        }

        string old_filePath = "";
        public bool is_file_exist(string device_id, string file_path)
        {
            bool result = false;
            if (file_path == old_filePath)
                result = true;

            return result;
        }

        public void copy_file(Stream input, string file_path)
        {

            int bufferSize = 1024 * 64;
            try
            {
                using (FileStream fileStream = new FileStream(file_path, FileMode.OpenOrCreate, FileAccess.Write))
                {

                    int bytesRead = -1;
                    byte[] bytes = new byte[bufferSize];

                    while ((bytesRead = input.Read(bytes, 0, bufferSize)) > 0)
                    {
                        fileStream.Write(bytes, 0, bytesRead);
                        fileStream.Flush();
                    }
                }
            }
            catch
            {

            }
        }

        private void fillSpec()
        {
            richTextBox1.Text = "1. bool device_exist(string device_id)\n" +
                                "2. void create_device(string device_id, string device_name)\n" +
                                "3. bool is_file_exist(string device_id, string file_path)\n" +
                                "4. void copy_file(stream input, string file_path)";
        }


    }
}
