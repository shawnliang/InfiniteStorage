using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Drawing;
using System.Web;
using log4net;


namespace Wpf_testHTTP
{
    class newPostClass
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(newPostClass));

        public string APIKEY { get; set; }
        public string HostIp { get; set; }
        public string session_token { get; set; }
        public string group_id { get; set; }
        public string _object_id { get; set; }
        public string _responseMessage { get; set; }

        private string genGuid()
        {
            Guid g;
            // Create and display the value of two GUIDs.
            g = Guid.NewGuid();
            return g.ToString();
        }

#region new post

        public MR_posts_new posts_new(string session_token,  string group_id, string content, string attachment_id_array,
                               string preview, string type, string coverAttach, string share_email_list, string event_type, string favorite)
        {
            log.Info("start Post_new");
            MR_posts_new _re = null;
            string post_id=genGuid();
            post_id = System.Web.HttpUtility.UrlEncode(post_id);
            session_token = System.Web.HttpUtility.UrlEncode(session_token);
            group_id = System.Web.HttpUtility.UrlEncode(group_id);
            content = System.Web.HttpUtility.UrlEncode(content);
            attachment_id_array = System.Web.HttpUtility.UrlEncode(attachment_id_array);
            preview = System.Web.HttpUtility.UrlEncode(preview);
            share_email_list = System.Web.HttpUtility.UrlEncode(share_email_list);
            event_type = System.Web.HttpUtility.UrlEncode(event_type);
            favorite = System.Web.HttpUtility.UrlEncode(favorite);

            try
            {
                string _url = "https://develop.waveface.com:443/v3" + "/pio_posts/new";

                string _parms = "post_id"+"="+post_id+"&"+
                    "apikey" + "=" + APIKEY + "&" +
                    "session_token" + "=" + session_token + "&" +
                    "content" + "=" + content + "&" +
                    "type" + "=" + type + "&"+
                    "favorite" + "=" + favorite + "&" +
                    "event_type" + "=" + event_type + "&";

                if (share_email_list != string.Empty)
                    _parms += "shared_email_list" + "=" + share_email_list + "&";

                if (attachment_id_array != string.Empty)
                    _parms += "attachment_id_array" + "=" + attachment_id_array + "&";

                if (preview != string.Empty)
                    _parms += "preview" + "=" + preview + "&";

                if (type == "image")
                {
                    if (coverAttach != string.Empty)
                    {
                        _parms += "cover_attach" + "=" + coverAttach + "&";
                    }
                }

                _parms += "group_id" + "=" + group_id;


                WebPostHelper _webPos = new WebPostHelper();
                bool _isOK = _webPos.doPost(_url, _parms, null);

                if (!_isOK)
                    return null;

                string _r = _webPos.getContent();

                _responseMessage = _r;
                MR_posts_new _ret = JsonConvert.DeserializeObject<MR_posts_new>(_r);

                return _ret;
            }
            catch (WebException _e)
            {
                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;
                    log.Error("Create POst_new, failed: " + _e.Data);
                    log.Error("HttpWebResponse, return: " + _res);
                }
            }
            catch (Exception _e)
            {               
                log.Error("Create POst_new, failed: " + _e.Data);              
            }
            return _re;
        }

#endregion

        #region login
        // public string email { get; set; }
        public string auth_login(string user, string password)
        {
            string result = "";
            string email = user;
            Dictionary<string, object> _args = new Dictionary<string, object>();
            {
                _args.Add("apikey", APIKEY);
                _args.Add("email", email);
                _args.Add("password", password);
            };
            try
            {
                string uri = "https://develop.waveface.com:443/v3/pio_auth/login";
                result = HttpPost(uri, _args);
                // get session_token
                MR_auth_login result12 = JsonConvert.DeserializeObject<MR_auth_login>(result);
                var session_token1 = result12.session_token; // result12.session_token;
                var group = result12.groups;
                var group_id1 = group[0].group_id;            // assume one group
                session_token = session_token1.ToString();
                group_id = group_id1.ToString();

            }
            catch (Exception err)
            {
                log.Error("Login error " + err.Message);
            }
            return result;
        }
        #endregion

        #region attachments

        public string callUploadAttachment(string filename, string user_email)
        {
            string result = "";
           string title="";
            string description="";
           string type="image";
            string image_data="medium";
            string object_id = genGuid();
            string post_id = "";
            try
            {
                MR_attachments_upload _attachment = attachments_upload(session_token, group_id, filename, title, description, type, image_data, object_id, post_id);
                result = _attachment.object_id;
            }
            catch (Exception err)
            {
                log.Error("upload error: " + err.Message);
                Console.WriteLine("Error: " + err.Message);
            }
            return result;
        }

        public MR_attachments_upload attachments_upload(string session_token, string group_id, string fileName,
                                                        string title, string description, string type, string image_meta,
                                                        string object_id, string post_id)
        {
            MR_attachments_upload _ret = null;

            try
            {
                string _url = "https://develop.waveface.com:443/v3" + "/pio_attachments/upload";

                string _mimeType = FileUtility.GetMimeType(new FileInfo(fileName));

                byte[] _data = FileUtility.ConvertFileToByteArray(fileName);

                Dictionary<string, object> _dic = new Dictionary<string, object>();
                _dic.Add("apikey", APIKEY);
                _dic.Add("session_token", session_token);
                _dic.Add("group_id", group_id);
                _dic.Add("file_name", "image_001.jpg");
                _dic.Add("title", title);
                //
                DateTime fileCreatedDate = File.GetCreationTime(fileName);
                string _date = fileCreatedDate.ToString(@"yyyy-MM-ddTHH\:mm\:ssZ");
                _dic.Add("file_create_time", _date);

                //if (description == string.Empty)
                //   description = title;

                _dic.Add("description", description);
                _dic.Add("type", type);

                if (type == "image")
                    _dic.Add("image_meta", image_meta);

                if (object_id != string.Empty)
                    _dic.Add("object_id", object_id);

                if (post_id != string.Empty)
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

                _ret = JsonConvert.DeserializeObject<MR_attachments_upload>(_r);
            }
            catch (WebException _e)
            {
                //NLogUtility.WebException(s_logger, _e, "attachments_upload", true);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse res = (HttpWebResponse)_e.Response;
                    log.Error("upload attachments error response: " + res);
                }

            }
            catch (Exception _e)
            {

            }

            return _ret;
        }
        #region
        public string UploadAttachment(string filename, string user_email)
        {
            string result = "";
            string email = user_email;
            DateTime fileCreatedDate = File.GetCreationTime(filename);
            string _date = fileCreatedDate.ToString(@"yyyy-MM-ddTHH\:mm\:ssZ");

            // handle image
            Bitmap _Img = new Bitmap(filename);

            byte[] _imageArr = imageToByteArray(_Img);
            string imageData = Convert.ToBase64String(_imageArr);


            Dictionary<string, object> _args = new Dictionary<string, object>();
            {
                _args.Add("apikey", APIKEY);
                _args.Add("group_id", group_id);
                _args.Add("session_token", session_token);
                _args.Add("title", "");
                _args.Add("type", "image");
                _args.Add("email", email);
                _args.Add("file_create_time", _date);
                _args.Add("filename", filename);
                _args.Add("image_meta", "origin");
                _args.Add("file", _imageArr);
            };
            try
            {
                string uri = "https://develop.waveface.com:443/v3/attachments/upload";
                result = HttpPost(uri, _args);
                // get session_token
                var result1 = JsonConvert.DeserializeObject<dynamic>(result);
                var session_token = result1.session_token;
                var group = result1.groups;
                var group_id = group[0].group_id;            // assume one group
            }
            catch (Exception err)
            {
            }
            return result;
        }

        #endregion

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }


        #region http utility, using WebClient
        string HttpGet(string uri)
        {
            Stream stream;
            StreamReader reader;
            String response = null;
            WebClient webClient = new WebClient();
            using (webClient)
            {
                try
                {
                    // open and read from the supplied URI
                    stream = webClient.OpenRead(uri);
                    reader = new StreamReader(stream);
                    response = reader.ReadToEnd();
                    // if no data return 0
                }
                catch (WebException ex)
                {
                    if (ex.Response is HttpWebResponse)
                    {
                        // Add you own error handling as required
                        switch (((HttpWebResponse)ex.Response).StatusCode)
                        {
                            case HttpStatusCode.NotFound:
                            case HttpStatusCode.Unauthorized:
                                response = null;
                                break;

                            default:
                                throw ex;
                        }
                    }
                    response = "-1";
                }
            }
            return response;
        }

        string HttpPost(string uri, Dictionary<string, object> postData)
        {
            string result = "";

            try
            {
                using (MyWebClient webClient = new MyWebClient())
                {
                    string address = uri;   // sort/how=...
                    //
                    var nameValueCollection = new NameValueCollection();
                    foreach (var kvp in postData)
                    {
                        if (kvp.Value != null)
                            nameValueCollection.Add(kvp.Key.ToString(), kvp.Value.ToString());
                        else
                        {
                            nameValueCollection.Add(kvp.Key.ToString(), "");
                        }
                    }
                    byte[] _postData = webClient.UploadValues(uri, nameValueCollection);

                    // TODO: Convert each member to bytearray
                    webClient.Headers[HttpRequestHeader.Accept] = "text/html, application/xhtml+xml, */*";
                    webClient.Headers[HttpRequestHeader.AcceptLanguage] = "zh-tw";
                    webClient.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
                    webClient.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    //webClient.UploadData(address, _postData);
                    byte[] s=   webClient.UploadValues(address, "post", nameValueCollection);

                    var str =  System.Text.Encoding.Default.GetString(s);
                    result = str;

                }
            }
            catch (Exception err)
            {
                result = "";
            }
            return result;
        }


        #endregion



        class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                return request;
            }
        }
        public class UploadFile
        {
            public UploadFile()
            {
                ContentType = "application/octet-stream";
            }
            public string Name { get; set; }
            public string Filename { get; set; }
            public string ContentType { get; set; }
            public Stream Stream { get; set; }
        }
    }
}
#endregion