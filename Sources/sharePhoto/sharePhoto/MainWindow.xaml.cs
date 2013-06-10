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

namespace Wpf_testHTTP
{
    public partial class MainWindow : Window
    {
        public static string HostIP = "https://api.waveface.com/v3/";
        public static string BaseURL { get { return HostIP; } }
        public static string APIKEY = "a23f9491-ba70-5075-b625-b8fb5d9ecd90";       // win8 viewer: station
        public string iniPath = @"C:\ykk\sharefavorite.ini";
        public string favoriteTitle = "Waveface Office";

        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));
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
            if (File.Exists(iniPath) == false)
            {
                using (FileStream FS = File.Create(path))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes("[Setup]\r\n refreshKey=");
                    FS.Write(info, 0, info.Length);
                    FS.Close();
                }
            }
            //MessageBox.Show(path);
        }
        public void setRun()
        {
        }

        public void setRun_button()
        {

            // get ini for refreshkey
            iniParser parser = new iniParser();
            String appStartPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            parser.IniParser(iniPath);  //appStartPath + @"\sharefavorite.ini");
            RefreshKey_saved = parser.GetSetting("Setup", "refreshkey");
            //
            AutoCompleteBox.Focusable = true;
            Keyboard.Focus(AutoCompleteBox);
            //
            if (RefreshKey_saved == "")
            {
                myTabControl.SelectedIndex = 1;
            }
            service_oauth();
        }
        public void setFilename(string filestring)
        {
            filename = filestring;              // inpuy filename list
        }
        public MainWindow()
        {
            string[] args = Environment.GetCommandLineArgs();
            // args = null;
            if (args.Length >= 2)
            {
                filename = args[1];
                log.Info("loading parmeters=" + args[1]);
            }
            InitializeComponent();

            AutoCompleteBox.Text = email;

            char[] delimiterChars = { '~' };
            arr = filename.Split(delimiterChars);
            no_of_attachments = arr.Length;

            // work at another thread
            Thread thread = new Thread(worker_DoWork);
            thread.IsBackground = true;
            thread.Start();

            log.Info("start another Thread");

            //// get ini for refreshkey
            //iniParser parser = new iniParser();
            //String appStartPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            //parser.IniParser(iniPath);  //appStartPath + @"\sharefavorite.ini");
            //RefreshKey_saved = parser.GetSetting("Setup", "refreshkey");
            ////
            //AutoCompleteBox.Focusable = true;
            //Keyboard.Focus(AutoCompleteBox);
            ////
            //if (RefreshKey_saved == "")
            //{
            //    myTabControl.SelectedIndex = 1;
            //}
            //service_oauth();

        }

        static string RefreshKey_saved;
        // do background loading
        //  private readonly BackgroundWorker worker = new BackgroundWorker();
        private void worker_DoWork()
        {

            log.Info("1. Login with: " + user + " / " + password);            //
            // 1. login 
            string data = callLogin(user, password);

            // 2. upload attachments
            char[] delimiterChars = { '~' };
            arr = filename.Split(delimiterChars);
            no_of_attachments = arr.Length;

            log.Info("2. start Add attachments ");                      //

            foreach (string _mail in arr)
            {
                log.Info("Add attachments: " + _mail);
                string _data = callUploadAttachment(_mail);
            }
            worker_RunWorkerCompleted();

            log.Info("End of attachments!");                            //
        }

        private void worker_RunWorkerCompleted()
        {
            uploadFinished = true;
        }

        bool uploadFinished = false;
        string user = "ruddytest29@gmail.com";
        string password = "a+123456";
        newPostClass _ws = new newPostClass();
        string group_id = "";
        string session_token = "";
        string filename = @"C:/Users/Ruddy/Pictures/IMG_2299.jpg~C:/Users/Ruddy/Pictures/IMG_2317.jpg~C:/Users/Ruddy/Pictures/IMG_2288.jpg";
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
            setup_emailList();
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
                //MessageBox.Show("email address ERROR!");
                label_invalid.Visibility = Visibility.Visible;
                return;
            }
            if (_emailStr == "")
            {
                // MessageBox.Show("Plaese input shared email address!");
                label_invalid.Visibility = Visibility.Visible;
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

            log.Info("3. create New Post ");                               //

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
            string event_type = "favorite_shared";
            string favorite = "0";
            try
            {
                MR_posts_new ret_post = _ws.posts_new(session_token, group_id, content, attachment_id_array, preview, type, coverAttach, share_email_list, event_type, favorite);
                textBox_return.Text = "Upload 完畢! \r\n Create post_id= " + ret_post.post.post_id + ", " + ret_post.api_ret_message + " !";
                textBox_return.Text += "\r\n\r\n" + _ws._responseMessage;
                //email_list.Items.Clear();
                AutoCompleteBox.Text = "";
            }
            catch (Exception err)
            {
                // to log error
                textBox_return.Text = "return NULL, get image error!";
                log.Error("Create New Post, return NULL: " + err.Message);                  //
            }
            //busy_flag.Visibility = Visibility.Visible;
            label_favorite.Visibility = Visibility.Visible;
            label_pass.Visibility = Visibility.Visible;
            log.Info("end of create new Post ");
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
            i = strIn.IndexOf('.');
            if (i < 0) return false;

            return true;

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

        //private void AutoCompleteBox_MouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    if ((label_invalid.Visibility == Visibility.Visible) && (AutoCompleteBox.Text == ""))
        //    {
        //        label_invalid.Visibility = Visibility.Collapsed;
        //    }

        //}
        //private void AutoCompleteBox_MouseUp(object sender, MouseEventArgs e)
        //{

        //    if (IsValidEmail(AutoCompleteBox.Text) == false)
        //        return;
        //    bool result = checkAvailable(AutoCompleteBox.Text);
        //    if (result == true)
        //    {
        //        result = checkAvailable_repeat();
        //        if (result == true)
        //        {
        //            email_list.Items.Add(AutoCompleteBox.Text);
        //            AutoCompleteBox.Text = "";                          // $$$
        //        }
        //    }
        //}

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
                    myTabControl.SelectedIndex = 1;              // access token expire
                }
                // set focus to AutoCompleteBox
                AutoCompleteBox.Focusable = true;
                Keyboard.Focus(AutoCompleteBox);
                // change code immediately
                //string _accesstoken= consumer.GetScopedAccessToken(RefreshKey_saved,_set1);
                //consumer.RefreshAuthorization(grantedAccess, TimeSpan.FromMinutes(20));
                //accessToken = grantedAccess.AccessToken;
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
                String appStartPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
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

        //private static string LoadRefreshToken()
        //{
        //    return Encoding.Unicode.GetString(ProtectedData.Unprotect(Convert.FromBase64String(Properties.Settings.Default.RefreshToken), aditionalEntropy, DataProtectionScope.CurrentUser));
        //}

        //private static void StoreRefreshToken(IAuthorizationState state)
        //{
        //    Properties.Settings.Default.RefreshToken = Convert.ToBase64String(ProtectedData.Protect(Encoding.Unicode.GetBytes(state.RefreshToken), aditionalEntropy, DataProtectionScope.CurrentUser));
        //    Properties.Settings.Default.Save();
        //}

        int emailCount = 0;
        List<string> mail_arr = new List<string>();
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
                bool result = consumer.RefreshAuthorization(grantedAccess1, TimeSpan.FromHours(10));

                accessToken = grantedAccess1.AccessToken;

                // save key
                iniParser parser = new iniParser();
                String appStartPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                parser.IniParser(iniPath);
                string _r = grantedAccess1.AccessToken;   //.RefreshToken;
                parser.AddSetting("Setup", "refreshkey", _r);
                parser.SaveSettings();
                myTabControl.SelectedIndex = 0;
            }
            else
            {
                // change code immediately
                //  consumer.RefreshAuthorization(grantedAccess1, TimeSpan.FromDays(30));
                //accessToken = grantedAccess1.AccessToken;
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
                        mail_arr.Add(email.Attributes["address"].Value);
                        emailCount++;
                    }
                    //   listbox1.Items.Add(title.InnerText + " , " + email.Attributes["address"].Value);
                }
                getSuccess = true;
                button_import.Visibility = Visibility.Collapsed;
                myTabControl.SelectedIndex = 0;
            }
            catch (Exception err)
            {
                // MessageBox.Show("error: " + err.Message);
                Console.WriteLine("Error: " + err.Message);
                getSuccess = false;
                return getSuccess;
            }
            // everything is good, goto input : autocomplete
            contactData.inc(emailCount);
            int i = 0;
            foreach (string emailAddress in mail_arr)
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

            //------------------------
            //    try
            //    {
            //        // get inbox mail content
            //       // #region get InBox
            //        string user = "ruddyl.lee@waveface.com"; // api.GetEmail();
            //        using (Imap imap = new Imap())
            //        {
            //            imap.ConnectSSL("imap.gmail.com");
            //            imap.LoginOAUTH2(user, accessToken);

            //            imap.SelectInbox();
            //            List<long> uids = imap.Search(Flag.Unseen);

            //            foreach (long uid in uids)
            //            {
            //                string eml = imap.GetMessageByUID(uid);
            //                IMail email = new MailBuilder().CreateFromEml(eml);

            //               // listbox1.Items.Add(email.From);
            //            }
            //            imap.Close();
            //        }
            //    }
            //    catch (Exception err)
            //    {
            //        MessageBox.Show("error: " + err.Message);
            //    }

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

        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myTabControl.SelectedIndex = 0;
        }

        private void button_import_Click(object sender, RoutedEventArgs e)
        {
            setRun_button();
        }

        private void AutoCompleteBox_GotFocus(object sender, RoutedEventArgs e)
        {
            label_favorite.Visibility = Visibility.Collapsed;
            label_pass.Visibility = Visibility.Collapsed;
        }




    }
}
