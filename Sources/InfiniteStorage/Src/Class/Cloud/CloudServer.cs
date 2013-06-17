//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Waveface.Common;
//using Newtonsoft.Json;
//using Waveface.Cloud;

//namespace InfiniteStorage.Cloud
//{
//    class CloudServer
//    {
//        private static string _session_token;

//        public static string SessionToken
//        {
//            get {
//                if (string.IsNullOrEmpty(_session_token))
//                    _session_token = autheticate("ruddytest29@gmail.com", "a+123456");

//                return _session_token;
//            }
//        }

//        private static string autheticate(string email, string password)
//        {
//            Dictionary<string, object> _args = new Dictionary<string, object>();
//            {
//                _args.Add("apikey", APIKEY);
//                _args.Add("email", email);
//                _args.Add("password", password);
//            };
//            try
//            {
//                string uri = "https://develop.waveface.com:443/v3/pio_auth/login";
//                HttpHelper.Post(uri, )
//                // get session_token
//                MR_auth_login result12 = JsonConvert.DeserializeObject<MR_auth_login>(result);
//                var session_token1 = result12.session_token; // result12.session_token;
//                var group = result12.groups;
//                var group_id1 = group[0].group_id;            // assume one group
//                session_token = session_token1.ToString();
//                group_id = group_id1.ToString();

//            }
//            catch (Exception err)
//            {
//                log.Error("Login error " + err.Message);
//            }
//            return result;
//        }

//        public static string APIKEY
//        {
//            get { return "a23f9491-ba70-5075-b625-b8fb5d9ecd90"; }
//        }
//    }
//}
