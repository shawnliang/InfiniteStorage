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
using System.Xml;
using log4net;
//
using DotNetOpenAuth.OAuth2;
using Limilabs.Client.Authentication.Google;
using Limilabs.Client.IMAP;
using Limilabs.Mail;
using System.Diagnostics;
using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json;

namespace Wpf_testHTTP
{
    public partial class MainWindow : Window
    {
        public static string APIKEY = "a23f9491-ba70-5075-b625-b8fb5d9ecd90";       // win8 viewer: station
        public static string machine_user =Environment.UserName;
        public string iniPath = @"C:\Users\"+machine_user+@"\AppData\Roaming\Bunny\temp\sharefavorite.ini";
        public string favoriteTitle = "Waveface Office";
        public string RefreshKey_real = "";                                         // kept the real refresh token
        public string access_token = "";
        public string label_id = "716cfce5-93d9-4b86-a384-b643f0aa9d31";
        public string sender_name = "";
        public string sender_msg = "";

        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

        public void setLabelId(string labelid)
        {
            label_id = labelid;
            iniParser parser = new iniParser();
            parser.IniParser(iniPath);
            parser.AddSetting("Setup", "label", label_id);
            parser.SaveSettings();
           
        }
        
        public void setTitle(string title)
        {
            favoriteTitle = title;
            if (favoriteTitle == "")
            {
                label_title.Content = "Share  Favorite with:";
            }
            else
            {
                label_title.Content = "Share  Favorite \"" + favoriteTitle + "\" with:";
            }
        }
        public void setiniPath(string path)
        {
            iniPath = path;

            int i0 = iniPath.IndexOf(@"\temp");
            if (i0 < 0)
            {
                iniPath = iniPath.Replace(@"\sharefavorite.ini", "");
                if (!Directory.Exists(iniPath + @"\temp"))
                {
                    string _p = iniPath + @"\temp";
                    Directory.CreateDirectory(_p);
                    iniPath = iniPath + @"\temp\sharefavorite.ini";
                }
                else
                {
                    iniPath = iniPath + @"\temp\sharefavorite.ini";
                }
            }
            if (File.Exists(iniPath) == false)
            {
                using (FileStream FS = File.Create(iniPath))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes("[Setup]\r\n refreshKey=");
                    FS.Write(info, 0, info.Length);
                    FS.Close();
                }
            }
        }
        private bool checkTempExist()
        {
            bool result = false;
            int i0 = iniPath.IndexOf(@"\temp");
            if (i0 < 0)
            {
                iniPath = iniPath.Replace(@"\sharefavorite.ini", "");
                if (!Directory.Exists(iniPath + @"\temp"))
                {

                    string _p = iniPath + @"\temp";
                    Directory.CreateDirectory(_p);
                    iniPath = iniPath + @"\sharefavorite.ini";
                    result = true;
                }
            }
            return result;
        }
        private bool checkIniExist()
        {
            bool result = false;
            if (!File.Exists(iniPath))
            {
                using (FileStream FS = File.Create(iniPath))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes("[Setup]\r\n refreshKey=");
                    FS.Write(info, 0, info.Length);
                    FS.Close();
                }
            }
            return result;
        }
        public void setRun()
        {
        }
        public bool initialState = true;

        public List<string> getMailList()
        {
            List<string> mailList = new List<string>();
            mailList = email_arr;

            return mailList;
        }  

        public void setRun_button()
        {
            // add state: if initialState = true; then first time link to google no show screen if failed

            // get ini for refreshkey
            iniParser parser = new iniParser();
           
            parser.IniParser(iniPath);  //appStartPath + @"\sharefavorite.ini");
            RefreshKey_saved = parser.GetSetting("Setup", "refreshkey");
            //
            RefreshKey_real = parser.GetSetting("Setup", "refreshkey_real");
            if (RefreshKey_real != "")
            {
                bool result = get_accesstokenfromrefreshtoken();
                if (result == true)              // exchange refresh token with refresh token
                {
                    RefreshKey_saved = access_token;
                }
            }
            AutoCompleteBox.Focusable = true;
            Keyboard.Focus(AutoCompleteBox);
            //
            if (RefreshKey_saved == "")
            {
                if (initialState == true)
                {
                    myTabControl.SelectedIndex = 0;
                }
                else
                {
                    myTabControl.SelectedIndex = 1;
                }
            } service_oauth();
        }


        //--- using Refresh token to get the Access token
        private bool get_accesstokenfromrefreshtoken()
        {

            string _url = "https://accounts.google.com/o/oauth2/token";

            string _parms = "refresh_token" + "=" + RefreshKey_real + "&" +
                "client_id" + "=" + clientID + "&" +
                "client_secret" + "=" + clientSecret + "&" +
                "grant_type" + "=" + "refresh_token";


            WebPostHelper _webPos = new WebPostHelper();
            bool _isOK = _webPos.doPost(_url, _parms, null);

            if (_isOK)
            {
                string _r = _webPos.getContent();
                access_token = parseAccessToken(_r);

                //
                accesstoken = access_token;
            }
            return _isOK;
        }

        // using Dynamic to get the token
        private string parseAccessToken(string _r)
        {
            string result = "";
            var results = JsonConvert.DeserializeObject<dynamic>(_r);
            var token = results.access_token;
            result = token.ToString();
            return result;
        }

        //---
        public void setFilename(string filestring)
        {
            filename = filestring;              // inpuy filename list
        }
        // ####
        public bool sendEmailList()
        {
            bool result = false;
            if (label_id == "")
            {
                return false;
            }
            sender_msg = textbox_message.Text;
            sender_name = textbox_name.Text;
            string recipients_json = getEmailListinJson();
            string rel = Wpf_testHTTP.labelCommand.inviteShared(label_id, sender_name, sender_msg, recipients_json);
            if (rel != null && rel != "")
            {
                result = true;
            }
            return result;
        }

        private string getEmailListinJson()
        {
            string result = null;
            string a = "[";
            int i = 0;
            string _title = "";
            try
            {
                foreach (string _mail in email_arr)
                {
                    _title = getTitle(_mail);
                    a = a + "{'name':" + "'" + _title + "'," + "'email':'" + _mail + "'},";
                }
                string _json = a;
                _json = _json.Substring(0, _json.Length - 1) + "]";
                email_arr.Clear();
                result = _json;
            }
            catch
            {
                result = "[]";              // empty email then send []
            }
            return result;
        }
 
        private string getTitle(string _mail)
        {
            string result = "";
            int i = 0;
            try
            {
                foreach (string mail in gmail_arr)
                {
                    if (_mail == mail)
                    {
                        result = title_arr[i];
                        break;
                    }
                    i++;
                }
            }
            catch
            {
                result = "";
            }
            return result;
        }

        
        public MainWindow()
        {
            initialState = true;
            string[] args = Environment.GetCommandLineArgs();
            // args = null;
            if (args.Length >= 2)
            {
                filename = args[1];
                log.Info("loading parmeters=" + args[1]);
            }
            InitializeComponent();

            if (email.Length != 0)
            {
                AutoCompleteBox.Text = email;
            }
            char[] delimiterChars = { '~' };
            arr = filename.Split(delimiterChars);
            no_of_attachments = arr.Length;

            // work at another thread
            Thread thread = new Thread(worker_DoWork);
            thread.IsBackground = true;
            thread.Start();

            // use timer to send New Post
            System.Windows.Threading.DispatcherTimer dispatcherTimer1 = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer1.Tick += new EventHandler(dispatcherTimer1_Tick);
            dispatcherTimer1.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer1.Start();

            log.Info("start another Thread");
            setRun_button();
            initialState = false;
        }

        static string RefreshKey_saved;
        // do background loading
        //  private readonly BackgroundWorker worker = new BackgroundWorker();
        private void worker_DoWork()
        {
            worker_RunWorkerCompleted();

        }

        public event EventHandler _sendDataCompleted = delegate { };
        private void worker_RunWorkerCompleted()
        {
            complete = true;
            uploadFinished = true;
            _sendDataCompleted(this, EventArgs.Empty);
        }

        bool uploadFinished = false;
        string user = "isserverId0.anonymous@waveface.com";  // "ruddytest29@gmail.com";
        string password = "anonymous123456"; // "a+123456";
        //newPostClass _ws = new newPostClass();
        string group_id = "";
        string session_token_key = "";
        string filename = @"C:/Users/" + machine_user + "/Pictures/2.jpg~C:/Users/" + machine_user + "/Pictures/video.mp4~C:/Users/" + machine_user + "/Pictures/3.jpg";
        string email = "";
        int no_of_attachments = 0;
        string[] arr;
        List<string> object_arr = new List<string>();           // Attachments object_id
        List<string> email_arr = new List<string>();            // shared email address

        // save google api return contacts in gmail_arr & title_arr
        List<string> gmail_arr = new List<string>();                // contacts data email
        List<string> title_arr = new List<string>();                //  contacts data title
        // share
        bool complete = false;
        bool shareButtonClick = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            char[] delimiterChars0 = { ';' };
            // 0 get email setup
            string _emailStr = setup_emailList();
            if (email_list.Items.Count <= 0)
            {
                bool result = checkAvailable(_emailStr);
                if (result == false)
                {
                    // MessageBox.Show("email address ERROR!");
                    label_invalid.Visibility = Visibility.Visible;
                    return;
                }
            }
            else if (AutoCompleteBox.Text != "")
            {             
                label_invalid.Visibility = Visibility.Visible;
                return;
            }
            if (_emailStr == "")
            {              
                label_invalid.Visibility = Visibility.Visible;
                return;
            }
            arr = _emailStr.Split(delimiterChars0);
            foreach (string _mail in arr)
            {
                email_arr.Add(_mail);
            }
            shareButtonClick = true;

            sendEmailList();

            label_favorite.Visibility = Visibility.Visible;
            label_pass.Visibility = Visibility.Visible;

            return;
        }

        private void service_run()
        {
            shareButtonClick = false;
            label_favorite.Visibility = Visibility.Visible;
            label_pass.Visibility = Visibility.Visible;
            log.Info("end of create new Post ");

           // sendEmailList();            // send email list to server
        }
        private string setup_emailList()
        {
            string _t = AutoCompleteBox.Text;
            if (email_list.Items.Count <= 0) return _t;
            string result = "";
            for (int i = 0; i <= email_list.Items.Count - 1; i++)
            {
                result += email_list.Items[i] + ";";
            }
            result = result.Substring(0, result.Length - 1);
            return result;
        }

        bool already_start = false;
        private void dispatcherTimer1_Tick(object sender, EventArgs e)
        {
            if (already_start == true) return;
            already_start = true;
            if (complete == true && shareButtonClick == true)
            {
                service_run();
                complete = false;
                shareButtonClick = false;
            }
            already_start = false;
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

            // must had "@"
            int i = strIn.IndexOf('@');
            if (i < 0) return false;

            // must had "."
            int i0 = strIn.LastIndexOf('.');
            if (i0 < 0) return false;
            if (i0 < i) return false;           // . behind @

            return true;
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
            // busy_flag.Visibility = Visibility.Collapsed;
            label_favorite.Visibility = Visibility.Collapsed;
            label_pass.Visibility = Visibility.Collapsed;
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

        private bool checkAvailable_repeat()
        {

            string _cur = AutoCompleteBox.Text;
            bool result = false;
            if (AutoCompleteBox.Text == "")
            {
                result = false;
            }
            else
            {
                result = true;
                foreach (string _m in email_list.Items)
                {
                    if (_cur == _m)
                    {
                        result = false;
                        return result;
                    }
                }
            }

            return result;
        }
        private void AutoCompleteBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && AutoCompleteBox.Text == "")
            {
                if (email_list.Items.Count > 0)
                {
                    RoutedEventArgs _e = new RoutedEventArgs();
                    // do "send"
                    Button_Click(this, _e);
                }
            }
            if (e.Key == Key.Return || e.Key == Key.Space || e.Key == Key.Tab)
            {
                if (IsValidEmail(AutoCompleteBox.Text) == false)
                    return;
                bool result = checkAvailable(AutoCompleteBox.Text);
                if (result == true)
                {
                    result = checkAvailable_repeat();
                    if (result == true)
                    {
                        email_list.Items.Add(AutoCompleteBox.Text);
                    }
                }
                AutoCompleteBox.Text = "";
            }
        }
        #region get Google API [ user: android@waveface.com  pw:xxxxxxxx53521916 ] prpoject name= Favorite Api Project
        const string clientID = "598605053299.apps.googleusercontent.com";      // company Google App ID
        const string clientSecret = "xJdRvYwEGED18wk-I5Ou2_SN";                 // Secret key

        AuthorizationServerDescription server = new AuthorizationServerDescription
        {
            AuthorizationEndpoint = new Uri("https://accounts.google.com/o/oauth2/auth"),
            TokenEndpoint = new Uri("https://accounts.google.com/o/oauth2/token"),
            ProtocolVersion = ProtocolVersion.V20,
        };

        List<string> scope1 = new List<string>  
        {
            GoogleScope.ImapAndSmtp.Name, 
            GoogleScope.EmailAddressScope.Name  
        };

        List<string> scope2 = new List<string>
    {
        GoogleScope.ImapAndSmtp.Name,
        GoogleScope.EmailAddressScope.Name,
        GoogleScope.ContactsScope.Name
    };

        // string[] _arr = new string[] { GoogleScope.ImapAndSmtp.Name };
        HashSet<string> _set1 = new HashSet<string>() { GoogleScope.ImapAndSmtp.Name };
        NativeApplicationClient consumer;
        private void service_oauth()
        {
            // MessageBox.Show(RefreshKey_saved);
            if (RefreshKey_saved != "")
            {
                bool result = getAccessToken("aa");
                if (result == true)
                {
                    myTabControl.SelectedIndex = 0;
                    // set focus to AutoCompleteBox
                    AutoCompleteBox.Focusable = true;
                    Keyboard.Focus(AutoCompleteBox);
                    return;
                }
                else
                {
                    iniParser parser = new iniParser();
                    String appStartPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                    parser.IniParser(iniPath);
                    parser.AddSetting("Setup", "refreshkey", "");
                    parser.SaveSettings();
                    RefreshKey_saved = "";
                    if (initialState == true)
                    {
                        // no switch screen
                        myTabControl.SelectedIndex = 0;
                        return;
                    }
                    myTabControl.SelectedIndex = 1;              // access token expire
                }
                // set focus to AutoCompleteBox
                AutoCompleteBox.Focusable = true;
                Keyboard.Focus(AutoCompleteBox);
 
            }

            //-------
            consumer = new NativeApplicationClient(server, clientID, clientSecret);
            Uri userAuthorizationUri = consumer.RequestUserAuthorization(scope2);
            //label_msg.DataContext = GoogleScope.ImapAndSmtp.Name;  //GoogleScope.EmailAddressScope.Name 
            string address =
                "https://accounts.google.com/o/oauth2/auth" +
                "?client_id=" + clientID +
                "&response_type=code" +
                "&access_type=offline" +
                "&scope=" + GoogleScope.ContactsScope.Name +  //GoogleScope.ImapAndSmtp.Name +
                "&redirect_uri=" + "urn:ietf:wg:oauth:2.0:oob";

            wb1.Navigate(new Uri(address, UriKind.Absolute));
            wb1.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(wb1_LoadCompleted);
        }
        string accesstoken = "";
        private void wb1_LoadCompleted(object sender, NavigationEventArgs e)
        {
            dynamic doc = wb1.Document;
            var htmlText = doc.documentElement.InnerHtml;
            string _str = htmlText;
            _str = _str.Replace(@"\\", "");
            int i0 = _str.IndexOf("<title>");
            i0 = _str.IndexOf("Success code=", i0 + 1);

            if (i0 == -1)
            {
                i0 = _str.IndexOf("Denied error", i0 + 1);
                if (i0 >= 0)
                {
                    myTabControl.SelectedIndex = 0;
                    image_gmail.Visibility = System.Windows.Visibility.Collapsed;
                    connected_gmail.Visibility = image_gmail.Visibility;
                    button_import.Visibility = Visibility.Visible;
                    // user say no, just return to input page
                    // myTabControl.SelectedIndex = 0;
                }
                return;       // not the last pagr or error
            }
            int i1 = _str.IndexOf("</title>", i0 + 1);
            string token = _str.Substring(i0 + 13, i1 - i0 - 13);

            myTabControl.SelectedIndex = 0;  // $$$

            bool result = getAccessToken(token);
            if (result == false)
            {
                connected_gmail.Visibility = Visibility.Collapsed;
                image_gmail.Visibility = Visibility.Collapsed;
                // clear Setup refresh key 
                iniParser parser = new iniParser();
                // String appStartPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                parser.IniParser(iniPath);
                parser.AddSetting("Setup", "refreshkey", "");
                parser.SaveSettings();

            }
            else
            {
                connected_gmail.Visibility = Visibility.Visible;
                image_gmail.Visibility = Visibility.Visible;
            }

        }

        int emailCount = 0;
        IAuthorizationState grantedAccess1;
        private bool getAccessToken(string authCode)
        {
            bool getSuccess = false;
            string accessToken = "";
            if (RefreshKey_saved == "")
            {
                consumer.ClientCredentialApplicator =
                         ClientCredentialApplicator.PostParameter(clientSecret);

                IAuthorizationState grantedAccess1 = consumer.ProcessUserAuthorization(authCode);
 
                accessToken = grantedAccess1.AccessToken;
                RefreshKey_real = grantedAccess1.RefreshToken;

                // save key
                iniParser parser = new iniParser();
              
                parser.IniParser(iniPath);              
                parser.AddSetting("Setup", "refreshkey", grantedAccess1.AccessToken);
                parser.AddSetting("Setup", "refreshkey_real", grantedAccess1.RefreshToken);
                parser.SaveSettings();
                myTabControl.SelectedIndex = 0;
            }
            else
            {
                accessToken = RefreshKey_saved;
                myTabControl.SelectedIndex = 0;
            }
            try
            {
                GoogleApi api = new GoogleApi(accessToken);

                // string user = "ruddyl.lee@waveface.com"; // api.GetEmail();
                // GoogleApi api = new GoogleApi(accessToken);

                XmlDocument contacts = api.GetContacts();
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(contacts.NameTable);
                //

                XmlNodeList _title = contacts.GetElementsByTagName("title");
                string temp = _title.Item(0).InnerText;
                temp = temp.Replace("'s Contacts", "");
                sender_name = temp;
                if (textbox_name.Text == "")
                {
                    textbox_name.Text = temp;
                }

                nsmgr.AddNamespace("gd", "http://schemas.google.com/g/2005");
                nsmgr.AddNamespace("a", "http://www.w3.org/2005/Atom");
                emailCount = 0;

                foreach (XmlNode contact in contacts.GetElementsByTagName("entry"))
                {
                    XmlNode title = contact.SelectSingleNode("a:title", nsmgr);
                    XmlNode email = contact.SelectSingleNode("gd:email", nsmgr);

                    // Console.WriteLine("{0}: {1}",title.InnerText, email.Attributes["address"].Value);
                    if (email != null)
                    {
                        title_arr.Add(title.InnerText);
                        gmail_arr.Add(email.Attributes["address"].Value);
                        emailCount++;
                    }
                }
                getSuccess = true;
                button_import.Visibility = Visibility.Collapsed;
                image_gmail.Visibility = System.Windows.Visibility.Visible;
                connected_gmail.Visibility = image_gmail.Visibility;
                myTabControl.SelectedIndex = 0;
            }
            catch (Exception err)
            {
                Console.WriteLine("Error: " + err.Message);
                getSuccess = false;
                return getSuccess;
            }
            // everything is good, goto input : autocomplete
            contactData.inc(emailCount);
            int i = 0;
            foreach (string emailAddress in gmail_arr)
            {
                contactData.States.SetValue(emailAddress, i);
                i++;
            }
            int where = contactData.States.Length;
            //
            AutoCompleteBox.ItemsSource = contactData.States;
            //email_list.Items.Clear();
            label_invalid.Visibility = Visibility.Collapsed;
            // myTabControl.SelectedIndex = 0;
            return getSuccess;
        }

        # endregion

        // editing the listbox(delete)
        private void email_list_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (myTabControl.SelectedIndex == 0)
            {
                myTabControl.SelectedIndex = 1;
            }
            else
            {
                myTabControl.SelectedIndex = 0;
            }
        }
        // delete select listbox item
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            while (email_list.SelectedItems.Count > 0)
            {
                email_list.Items.Remove(email_list.SelectedItem);
            }
        }

        private void AutoCompleteBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            label_favorite.Visibility = Visibility.Collapsed;
            label_pass.Visibility = Visibility.Collapsed;
            label_invalid.Visibility = Visibility.Collapsed;
        }

        private void AutoCompleteBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            label_favorite.Visibility = Visibility.Collapsed;
            label_pass.Visibility = Visibility.Collapsed;
            if (IsValidEmail(AutoCompleteBox.Text) == false)
                return;
            bool result = checkAvailable(AutoCompleteBox.Text);
            if (result == true)
            {
                result = checkAvailable_repeat();
                if (result == true)
                {
                    email_list.Items.Add(AutoCompleteBox.Text);
                    label_invalid.Visibility = Visibility.Collapsed;
                    AutoCompleteBox.Text = "";                          // $$$
                }
            }
        }

        private void button_import_Click(object sender, RoutedEventArgs e)
        {
            initialState = false;
            setRun_button();
        }

        private void AutoCompleteBox_GotFocus(object sender, RoutedEventArgs e)
        {
            label_favorite.Visibility = Visibility.Collapsed;
            label_pass.Visibility = Visibility.Collapsed;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            myTabControl.SelectedIndex = 0;
            tab2.Visibility = Visibility.Collapsed;
        }

        private void AutoCompleteBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            label_invalid.Visibility = Visibility.Collapsed;
        }

        private void label_title_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //sendEmailList();
        }
    }
}
