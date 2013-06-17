//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using Waveface.Common;
//using System.Net;
//using Newtonsoft.Json;
//using Waveface.Cloud;

//namespace InfiniteStorage.Cloud
//{
//    class AttachmentApi
//    {
//        public void attachments_upload(string APIKEY, string session_token, string group_id, string fileName, byte[] file_data,
//                                                        string title, string description, string type, string image_meta,
//                                                        string object_id, string post_id, string mime_type)
//        {
//            //TODO: splits to dev and production server
//            string _url = "https://develop.waveface.com:443/v3" + "/pio_attachments/upload";

//            if (string.IsNullOrEmpty(mime_type))
//                mime_type = FileUtility.GetMimeType(new FileInfo(fileName));

//            Dictionary<string, object> _dic = new Dictionary<string, object>();
//            _dic.Add("apikey", APIKEY);
//            _dic.Add("session_token", session_token);
//            _dic.Add("group_id", group_id);
//            _dic.Add("file_name", Path.GetFileName(fileName));
//            _dic.Add("title", title);
//            //
//            DateTime fileCreatedDate = File.GetCreationTime(fileName);
//            string _date = fileCreatedDate.ToString(@"yyyy-MM-ddTHH\:mm\:ssZ");
//            _dic.Add("file_create_time", _date);

//            _dic.Add("description", description);
//            _dic.Add("type", type);

//            if (type == "image")
//                _dic.Add("image_meta", image_meta);

//            if (!string.IsNullOrEmpty(object_id))
//                _dic.Add("object_id", object_id);

//            if (!string.IsNullOrEmpty(post_id))
//                _dic.Add("post_id", post_id);

//            _dic.Add("file", file_data);

//            string _userAgent = "Windows";

//            string _fileName = Path.GetFileName(fileName);

//            HttpWebResponse _webResponse = MultipartFormDataPostHelper.MultipartFormDataPost(_url, _userAgent, _dic,
//                                                                                             _fileName, mime_type);

//            // Process response
//            StreamReader _responseReader = new StreamReader(_webResponse.GetResponseStream());
//            string _r = _responseReader.ReadToEnd();
//            _webResponse.Close();

//            JsonConvert.DeserializeObject<MR_attachments_upload>(_r);
//        }
//    }
//}
