using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Wpf_testHTTP
{
    public partial class MainWindow : Window
    {
        public static string HostIP = "https://api.waveface.com/v3/";
        public static string BaseURL { get { return HostIP; } }
        public static string APIKEY = "a23f9491-ba70-5075-b625-b8fb5d9ecd90";       // win8 viewer: station


        public MainWindow()
        {
            string[] args = Environment.GetCommandLineArgs();
            // args = null;
            if (args.Length >= 2)
            {
                filename = args[1];
            }
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainContainer_Loaded);
            textBox_email.Text = email;

            char[] delimiterChars = { '~' };
            arr = filename.Split(delimiterChars);
            no_of_attachments = arr.Length;
            label_display.Content = "Ready to send  "+arr.Length.ToString() + "  items";
            //
            Thread thread = new Thread(worker_DoWork);          // work in another thread
            thread.IsBackground = true;
            thread.Start();
        }

 

   void MainContainer_Loaded(object sender, RoutedEventArgs e)
  {
     if (Application.Current.Properties["ArbitraryArgName"] != null)
    {
        string fname = Application.Current.Properties["ArbitraryArgName"].ToString();
    }
 }

 // do background loading
 //  private readonly BackgroundWorker worker = new BackgroundWorker();
   private void worker_DoWork()
   {

       // 1. login 
       string data = callLogin(user, password);
       // textBox_return.Text = data;

       // 2. upload attachments
       char[] delimiterChars = { '~' };
       arr = filename.Split(delimiterChars);
       no_of_attachments = arr.Length;

       foreach (string _mail in arr)
       {
           string _data = callUploadAttachment(_mail);
           if (_data == "")
           {
               MessageBox.Show("Read file error!");
           }
           worker_RunWorkerCompleted();
       }
   }

   private void worker_RunWorkerCompleted()
   { 
       uploadFinished = true;
   }

        bool uploadFinished = false;
        bool already_login = false;
        string user = "ruddytest29@gmail.com";
        string password = "a+123456";
        newPostClass _ws = new newPostClass();
        string group_id = "";
        string session_token = "";
        string filename = @"C:/Users/Ruddy/Pictures/IMG_2293.jpg~C:/Users/Ruddy/Pictures/Cherubim_1.jpg~C:/Users/Ruddy/Pictures/IMG_2299.jpg~C:/Users/Ruddy/Pictures/IMG_2317.jpg~C:/Users/Ruddy/Pictures/IMG_2288.jpg";
        string email = "";
        int no_of_attachments = 0;
        string[] arr;
        List<string> object_arr = new List<string>();
        List<string> email_arr = new List<string>();

        // share
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            char[] delimiterChars0 = { ';' };
            // 0 get email setup
            string _emailStr = textBox_email.Text;
  
            bool result = checkAvailable(_emailStr);
            if (result == false)
            {
                MessageBox.Show("email address ERROR!");
                return;
            }
            if (_emailStr == "")
            {
                MessageBox.Show("Plaese input shared email address!");
                return;
            }
            arr = _emailStr.Split(delimiterChars0);
            foreach (string _mail in arr)
            {
                email_arr.Add(_mail);
            }
            SetBusyState();

            int i = 0;      // handle user press share button before upload finished
            do
            {
                System.Threading.Thread.Sleep(100);
                i++;
            } while (uploadFinished == false && i <= 1000);

 #region step 1& 2 (do by background worker)
            ////// 1. login 
            //if (already_login == false)
            //{
            //    string data = callLogin(user, password);
            //    textBox_return.Text = data;
            //    already_login = true;
            //}
            //// 2. upload attachments
            ////Configure the ProgressBar

            //char[] delimiterChars = { '~' };
            //arr = filename.Split(delimiterChars);
            //no_of_attachments = arr.Length;

            //foreach (string _mail in arr)
            //{
            //    string _data = callUploadAttachment(_mail);
            //    textBox_return.Text += "object id= " + _data + "\r\n";
            //}
 #endregion
            //// 3. new POST
            _ws.group_id = group_id;
            _ws.session_token = session_token;
            _ws.APIKEY = APIKEY;
            string object_id = _ws._object_id;
            string content = "";
            string attachment_id_array = count_attachments(); //  "[" + '"' + object_id.ToString() + '"' + "]";
            string preview = "";
            string type = "event";
            string share_email_list = count_emails(); // "[" + '"' + email + '"' + "]";
            string coverAttach = "";
            string event_type = "share";
            string favorite = "0";
            try
            {
                MR_posts_new ret_post = _ws.posts_new(session_token, group_id, content, attachment_id_array, preview, type, coverAttach, share_email_list, event_type, favorite);
                textBox_return.Text = "Upload 完畢! \r\n Create post_id= " + ret_post.post.post_id + ", " + ret_post.api_ret_message + " !";
                textBox_return.Text += "\r\n\r\n" + _ws._responseMessage;
              
            }
            catch (Exception err)
            {
                // to log error
                textBox_return.Text = "return NULL, get image error!";
            }
            busy_flag.Visibility = Visibility.Visible;
        }
        #region emailavailable
        private bool checkAvailable(string _emailStr)
        {
            bool result = true;
            string[] _m = _emailStr.Split(';');
            foreach (string m in _m)
            {
                result = IsValidEmail(m);
                if (result == false)
                {
                    return false;
                }
            }
            return result;

        }
        bool invalid = false;
        public bool IsValidEmail(string strIn)
        {
            bool invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names. 
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format. 
            try
            {
                return Regex.IsMatch(strIn, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$");
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
        #endregion
        private string count_emails()
        {
            string result = "";
            string att0 = "[";
            foreach (string _mail in email_arr)
            {
                att0 += '"' + _mail + '"' + ",";
            }
            att0 = att0.Substring(0, att0.Length - 1) + "]";
            result = att0.Replace(@"\\", "/");
            return result;
        
        }
        private string count_attachments()
        {
            string result = "";
            string att0 = "[";
            foreach (string _id in object_arr)
            {
                att0 += '"' + _id + '"' + ",";
            }
            att0 = att0.Substring(0, att0.Length - 1) + "]";
            result = att0.Replace(@"\\", "/");
            return result;
        }
        private string callLogin(string user, string password)
        {
            string result = "";

            _ws.HostIp = HostIP;
            _ws.APIKEY = APIKEY;

            result = _ws.auth_login(user, password);
            group_id = _ws.group_id;
            session_token = _ws.session_token;
            return result;
        }

        string user_email = "ruddyl.lee@waveface.com";
        private string callUploadAttachment(string filename)
        {
            string result = "";

            _ws.group_id = group_id;
            _ws.session_token = session_token;
            _ws.APIKEY = APIKEY;
            result = _ws.callUploadAttachment(filename, user_email);
            _ws._object_id = result;
            object_arr.Add(result);
            return result;
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (textBox_return.Visibility == Visibility.Collapsed)
            {
                textBox_return.Visibility = Visibility.Visible;
            }
            else
            {
                textBox_return.Visibility = Visibility.Collapsed;
            }
        }
        private static bool IsBusy;

        /// <summary>
        /// Sets the busystate as busy.
        /// </summary>
        public static void SetBusyState()
        {
            SetBusyState(true);
        }
        private static void SetBusyState(bool busy)
        {
            if (busy != IsBusy)
            {
                IsBusy = busy;
                Mouse.OverrideCursor = busy ? Cursors.Wait : null;

                if (IsBusy)
                {
                    new DispatcherTimer(TimeSpan.FromSeconds(0), DispatcherPriority.ApplicationIdle, dispatcherTimer_Tick, Application.Current.Dispatcher);
                }
            }
        }
        private static void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var dispatcherTimer = sender as DispatcherTimer;
            if (dispatcherTimer != null)
            {
                SetBusyState(false);
                dispatcherTimer.Stop();
                
            }
        }

        private void busy_flag_MouseEnter(object sender, MouseEventArgs e)
        {
            busy_flag.Visibility = Visibility.Collapsed;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void faviorate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            label_debug.Visibility = Visibility.Visible;
        }
 
    }
}
