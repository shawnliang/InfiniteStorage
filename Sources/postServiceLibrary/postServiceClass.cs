using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace postServiceLibrary
{
    public class postServiceClass
    {
         
        public string APIKEY { get; set; }
        public string HostIp { get; set; }
        public string session_token { get; set; }
        public string group_id { get; set; }

        public string renewSharedcode(string session_token, string post_id)
        {
            string result = null;
            if (session_token == "" || post_id == "")
            {
                return null;
            }
            string _url = "https://develop.waveface.com/v3/pio_posts/renew_sharedcode";

            string _post_id = System.Web.HttpUtility.UrlEncode(post_id);
            string _session_token = System.Web.HttpUtility.UrlEncode(session_token);
            string _parms = "post_id" + "=" + _post_id + "&" +
                "session_token" + "=" + session_token + "&" +
                "apikey" + "=" + APIKEY;

            WebPostHelper _webPos = new WebPostHelper();
            bool _isOK = _webPos.doPost(_url, _parms, null);

            if (_isOK)
            {
                string _r = _webPos.getContent();
                var results = JsonConvert.DeserializeObject<dynamic>(_r);
                var sharedcode = results.shared_code;
                result = sharedcode.ToString();
            }
            else
            {
                result = null;
            }
            return result;
        }
        public bool tuneOffSharedcode(string session_token, string post_id)
        {
            bool result = false;
            if (session_token == "" || post_id == "")
            {
                return false;
            }
            string _url = "https://develop.waveface.com/v3/pio_posts/tuneoff_sharedcode";

            string _post_id = System.Web.HttpUtility.UrlEncode(post_id);
            string _session_token = System.Web.HttpUtility.UrlEncode(session_token);
            string _parms = "post_id" + "=" + _post_id + "&" +
                "session_token" + "=" + session_token + "&" +
                "apikey" + "=" + APIKEY;

            WebPostHelper _webPos = new WebPostHelper();
            bool _isOK = _webPos.doPost(_url, _parms, null);

            if (_isOK)
            {
                string _r = _webPos.getContent();      
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public bool tuneOnSharedcode(string session_token, string post_id)
        {
            bool result = false;
            if (session_token == "" || post_id == "")
            {
                return false;
            }
            string _url = "https://develop.waveface.com/v3/pio_posts/tuneon_sharedcode";

            string _parms = "post_id" + "=" + post_id + "&" +
                "session_token" + "=" + session_token + "&" +
                 "apikey" + "=" + APIKEY;

            WebPostHelper _webPos = new WebPostHelper();
            bool _isOK = _webPos.doPost(_url, _parms, null);

            if (_isOK)
            {
                string _r = _webPos.getContent();
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
                                                                            
#region create new account
        public  string createAccount(string user, String password, string nickname)
        {
            string result = null;

            if (user == "" || password == "")
            {
                return null;
            }

            bool _result = IsValidEmail(user);
             if (_result == false)
             {
                 return null;
             }

            string _url = "https://develop.waveface.com/v3/auth/signup";
            string user_email = user;
            string user_password = password;
            string user_nickname = nickname;

           string _password = System.Web.HttpUtility.UrlEncode(password);

            string _parms = "email" + "=" + user_email + "&" +
                "password" + "=" + _password + "&" +
                "nickname" + "=" + user_nickname + "&" +
                "apikey" + "=" + APIKEY;

            WebPostHelper _webPos = new WebPostHelper();
            bool _isOK = _webPos.doPost(_url, _parms, null);

            if (_isOK)
            {
                string _r = _webPos.getContent();
                session_token = getSessionToken(_r);
                result = session_token;
            }
            else
            {
                result = null;
            }
            return result;
        }

        public bool IsValidEmail(string strIn)
        {
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

        private string getSessionToken(string _r)
        {
            string result = "";
            var results = JsonConvert.DeserializeObject<dynamic>(_r);
            var token = results.session_token;
            result = token.ToString();
            return result;
        }
#endregion

#region Update Post
        public string UpdatePost(string session_token,String post_id, List<string> attachments_arr,string lastUpdateTime)
        {
            string result = null;
            // verify
            if (session_token == "" || post_id == null || attachments_arr == null || lastUpdateTime==null)
            {
                result = null;
                // log for parmeters error
            }
            //--
            APIKEY = "a23f9491-ba70-5075-b625-b8fb5d9ecd90";
            //
            newPostClass _ws = new newPostClass();
            _ws.group_id = group_id;
            _ws.session_token = session_token;
            _ws.APIKEY = APIKEY;
            //string object_id = _ws._object_id;
            string content = "";
            string attachment_id_array = countattachments_arr(attachments_arr);
            string preview = "";
            string type = "event";
            string coverAttach = "";
            string event_type = "favorite_shared";
            string favorite = "0";
            try
            {
                string ret_post = _ws.posts_update(session_token, group_id, post_id, attachment_id_array, lastUpdateTime, type, event_type, favorite);
                if (ret_post != null)
                    result = ret_post;
                else
                    result = null;
            }
            catch (Exception err)
            {
                result = null;
            }

            return result;
        }
 

        private string countattachments_arr(List<string> attachments_arr)
        {
            string result = "";
            string att0 = "[";
            foreach (string _att in attachments_arr)
            {
                att0 += '"' + _att + '"' + ",";
            }
            att0 = att0.Substring(0, att0.Length - 1) + "]";
            result = att0.Replace(@"\\", "/");
            return result;
        }   
 #endregion   

#region create New post
        public string NewPost(string session_token,List<string> object_arr,List<string> email_arr )
        {
            string result = null;
            // verify
            if (session_token == "" || object_arr == null || email_arr == null)
            {
                result = null;
                // log for parmeters error
            }
            //--
            APIKEY = "a23f9491-ba70-5075-b625-b8fb5d9ecd90";
            //
            newPostClass _ws = new newPostClass();
            _ws.group_id = group_id;
            _ws.session_token = session_token;
            _ws.APIKEY = APIKEY;
            string object_id = _ws._object_id;
            string content = "";
            string attachment_id_array = count_attachments(object_arr); 
            string preview = "";
            string type = "event";
            string share_email_list = count_emails(email_arr); 
            string coverAttach = "";
            string event_type = "favorite_shared";
            string favorite = "0";
            try
            {
               string ret_post = _ws.posts_new(session_token, group_id, content, attachment_id_array, preview, type, coverAttach, share_email_list, event_type, favorite);
                if (ret_post != null)       // exception return null
                    result = ret_post;
                else
                    result = null;
            }
            catch (Exception err)
            {
                result = null;
            }

            return result;
        }

        private string count_emails(List<string> email_arr)
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

        private string count_attachments(List<string> object_arr)
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
#endregion

 #region upload attachments
        // do Multi attachments
        // input: in [ xxxx~yyyy~zzzz ] format
        // output: return List<string> of object id
        // return null for error
        public List<string> addAttachments(string session_token,string filenames)
        {
            List<string> object_arr = new List<string>();

            try
            {
                char[] delimiterChars = { '~' };
                string[] arr = filenames.Split(delimiterChars);
                int no_of_attachments = arr.Length;

                // log.Info("2. start Add attachments ");                      //
              
                foreach (string _file in arr)
                {
                    // log.Info("Add attachments: " + _file);
                    string _data = callUploadAttachments(session_token,_file);
                    object_arr.Add(_data);
                }             
            }
            catch (Exception err)
            {               
                object_arr = null;
            }
            
            return object_arr;
        }

        private string callUploadAttachments(string session_token,string filename)
        {
            if (session_token == "" || filename == "")
            {
                return null;
            }
            newPostClass _ws = new newPostClass();
                    
            _ws.group_id = group_id;
            _ws.session_token = session_token;
            _ws.APIKEY = APIKEY;
            string result = _ws.callUploadAttachment(session_token,filename);
            _ws._object_id = result;
            
            return result;
        }

        string user = "ruddytest36@gmail.com";
        string password =  "a+123456";
        public string _initial()
        {           
            string result = callLogin(user, password);
            session_token = result;
            return result;
        }

        private string callLogin(string user, string password)
        {
            string result = "";
            if (user == "" || password == "")
            {
                return null;
            }
            newPostClass _ws = new newPostClass();
            _ws.APIKEY = "a23f9491-ba70-5075-b625-b8fb5d9ecd90";
            
            result = _ws.auth_login(user, password);
            group_id = _ws.group_id;
            session_token = _ws.session_token;
            result = session_token;
            return result;
        }
 #endregion
    }
}
