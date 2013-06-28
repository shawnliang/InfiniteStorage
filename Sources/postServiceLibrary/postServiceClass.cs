using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Web;
using System.IO;
using System.Net;

namespace postServiceLibrary
{
    public class postServiceClass
    {
         
        public string APIKEY { get; set; }
        public string HostIp { get; set; }
        public string session_token { get; set; }
        public string group_id { get; set; }
        public static string serverBaseUrl = "https://develop.waveface.com/v3";

        public void setServerBaseUrl(string url)
        {
            if (url != "" && url != null)
            {
                serverBaseUrl = url;
                    HostIp=url;
            }
        }
        public string renewSharedcode(string session_token, string post_id)
        {
            string result = null;
            if (session_token == "" || post_id == "")
            {
                return null;
            }
            string _url = serverBaseUrl + "/pio_posts/renew_sharedcode";

            string _post_id = System.Web.HttpUtility.UrlEncode(post_id);
            string _session_token = System.Web.HttpUtility.UrlEncode(session_token);
            string _parms = "post_id" + "=" + _post_id + "&" +
                "session_token" + "=" + session_token + "&" +
                "apikey" + "=" + APIKEY;

            WebPostHelper _webPos = new WebPostHelper();
            var response = _webPos.doPost(_url, _parms, null);

			var results = JsonConvert.DeserializeObject<dynamic>(response);
			return results.shared_code.ToString();
        }
        public void tuneOffSharedcode(string session_token, string post_id)
        {
            bool result = false;
            if (session_token == "" || post_id == "")
            {
				throw new ArgumentNullException();
            }
            string _url = serverBaseUrl + "/pio_posts/tuneoff_sharedcode";

            string _post_id = System.Web.HttpUtility.UrlEncode(post_id);
            string _session_token = System.Web.HttpUtility.UrlEncode(session_token);
            string _parms = "post_id" + "=" + _post_id + "&" +
                "session_token" + "=" + HttpUtility.UrlEncode(session_token) + "&" +
                "apikey" + "=" + APIKEY;

            WebPostHelper _webPos = new WebPostHelper();
            _webPos.doPost(_url, _parms, null);
        }

        public void tuneOnSharedcode(string session_token, string post_id)
        {
            bool result = false;
            if (session_token == "" || post_id == "")
            {
				throw new ArgumentNullException();
            }
            string _url = serverBaseUrl + "/pio_posts/tuneon_sharedcode";

            string _parms = "post_id" + "=" + post_id + "&" +
                "session_token" + "=" + HttpUtility.UrlEncode(session_token) + "&" +
                 "apikey" + "=" + APIKEY;

            WebPostHelper _webPos = new WebPostHelper();
            _webPos.doPost(_url, _parms, null);
        }
                                                                            
		#region create new account
        public string createAccount(string user, String password, string nickname)
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

             string _url = serverBaseUrl + "/auth/signup";
            string user_email = user;
            string user_password = password;
            string user_nickname = nickname;

           string _password = System.Web.HttpUtility.UrlEncode(password);

            string _parms = "email" + "=" + user_email + "&" +
                "password" + "=" + _password + "&" +
                "nickname" + "=" + user_nickname + "&" +
                "apikey" + "=" + APIKEY;

            WebPostHelper _webPos = new WebPostHelper();
            var response = _webPos.doPost(_url, _parms, null);

            session_token = getSessionToken(response);
			return session_token;
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
        public void UpdatePost(string session_token,String post_id, List<string> attachments_arr, DateTime lastUpdateTime, List<string> recipientEmailList)
        {
            string result = null;
            // verify
            if (session_token == "" || post_id == null || attachments_arr == null)
            {
				throw new ArgumentNullException();
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
            string attachment_id_array = joinToJsonList(attachments_arr);
			string recipient_email_array = joinToJsonList(recipientEmailList);
            string preview = "";
            string type = "event";
            string coverAttach = "";
            string event_type = "favorite_shared";
            string favorite = "0";

			_ws.posts_update(session_token, group_id, post_id, attachment_id_array, type, event_type, favorite, lastUpdateTime, recipient_email_array);
        }
 

		private static string joinToJsonList(ICollection<string> list)
        {
			return "[" + string.Join(",", list.Select(x => "\"" + x + "\"").ToArray()) + "]";
        }

		#endregion   

		#region create New post
        public string NewPost(string session_token,List<string> object_arr,List<string> email_arr, string post_id)
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
            string attachment_id_array = toJsonArray(object_arr); 
            string preview = "";
            string type = "event";
			string share_email_list = toJsonArray(email_arr); 
            string coverAttach = "";
            string event_type = "favorite_shared";
            string favorite = "0";

            string ret_post = _ws.posts_new(session_token, group_id, content, attachment_id_array, preview, type, coverAttach, share_email_list, event_type, favorite, post_id);

			if (ret_post == null)
				throw new Exception("new post failed");

			var retObj = JsonConvert.DeserializeObject<dynamic>(ret_post);
			return retObj.post.shared_code.ToString();
        }

        private string toJsonArray(List<string> object_arr)
        {
			return "[" + string.Join(",", object_arr.Select(x => "\"" + x + "\"").ToArray()) + "]";
        }
		#endregion

		#region upload attachments
		public static MR_attachments_upload attachments_upload(byte[] file_data, string session_token, string group_id, string fileName,
														string title, string description, string type, string image_meta,
														string object_id, string post_id, string APIKEY, DateTime fileCreateTime)
		{
            string _url = serverBaseUrl + "/pio_attachments/upload";

			string _mimeType = FileUtility.GetMimeType(new FileInfo(fileName));
			byte[] _data;
			if (file_data != null)
			{
				_data = file_data;
			}
			else
			{
				_data = FileUtility.ConvertFileToByteArray(fileName);
			}

			Dictionary<string, object> _dic = new Dictionary<string, object>();
			_dic.Add("apikey", APIKEY);
			_dic.Add("session_token", session_token);
			_dic.Add("group_id", group_id);
			_dic.Add("file_name", fileName);
			_dic.Add("title", title);
			//

			string _date = fileCreateTime.ToUniversalTime().ToString(@"yyyy-MM-ddTHH\:mm\:ssZ");
			_dic.Add("file_create_time", _date);
			_dic.Add("description", description);
			_dic.Add("type", type);

			if (type == "image")
				_dic.Add("image_meta", image_meta);

			if (!string.IsNullOrEmpty(object_id))
				_dic.Add("object_id", object_id);

			if (!string.IsNullOrEmpty(post_id))
				_dic.Add("post_id", post_id);

			_dic.Add("file", _data);

			string _userAgent = "Windows";

			string _fileName = new FileInfo(fileName).Name;

			HttpWebResponse _webResponse = MultipartFormDataPostHelper.MultipartFormDataPost(_url, _userAgent, _dic,
																							 _fileName, _mimeType);


			// Process response
			StreamReader _responseReader = new StreamReader(_webResponse.GetResponseStream());
			string _r = _responseReader.ReadToEnd();
			_webResponse.Close();

			return JsonConvert.DeserializeObject<MR_attachments_upload>(_r);
		}

		#endregion

		public string callLogin(string user, string password)
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

		public static void sendFavoriteEmail(string sessionToken, string apikey, ICollection<string> recipients, string shareCode, string sender, string title, string msg)
		{
			var _url = "https://develop.waveface.com:443/v3" + "/pio_posts/send_favorite_email";


			var postData = string.Format("apikey={0}&session_token={1}&shared_email_list={2}&shared_code={3}&sender_name={4}&title={5}&content={6}",
								HttpUtility.UrlEncode(apikey),
								HttpUtility.UrlEncode(sessionToken),
								HttpUtility.UrlEncode(joinToJsonList(recipients)),
								HttpUtility.UrlEncode(shareCode),
								sender != null ? HttpUtility.UrlEncode(sender) : "",
								title != null ? HttpUtility.UrlEncode(title) : "",
								msg != null ? HttpUtility.UrlEncode(msg) : "");

			var post = new WebPostHelper();
			post.doPost(_url, postData, null);
		}
    }
}
