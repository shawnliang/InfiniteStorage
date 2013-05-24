using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace Wpf_getRest
{
    // Services:
    // 1. initial() -- setup timer function & some basic parms
    // 2. 
    class localService
    {
        #region reset command
        public static string HostIP = "http://127.0.0.1:14005";
        public static string BaseURL { get { return HostIP; } }
        public string current_device { get; set; }
        string getAllLabel_cmd = HostIP + "/label/list_all";
        string getPending_cmd0 = HostIP + "/pending/get?device_id=";  //
        string getPending_cmd1 = "&seq=0&limit=500";
        string getNextFiles_cmd0 = HostIP + "/pending/get?device_id=";    //10234&limit=500
        string getNextFiles_cmd1 = "&seq=0&limit=500";

        string tagLabel_cmd = HostIP + "/label/tag?file_id=";  // + file_guid + "&label_id=" + label_id;"; 
        string clearLabel_cmd = HostIP + "/label/clear?" + "label_id="; // + label_id;
        string sendSort_cmd = HostIP + "/pending/sort?" + "how="; // + label_id;
        #endregion

        public int remaining_count { get; set; }
        public int files_from_seq { get; set; }
        public string _jsonStr = "";

        #region initial service
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public string initial(string _current_device, [Optional]int timeinterval)
        {

            string result = "";
            //
            if (_current_device == "")
            {
                return "";       // must have device_id
            }
            current_device = _current_device;
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);              // every ten seconds

            // made the 1st call
            string _response = processCommand("pendingFiles");
            if (_response != "{\"remaining_count\":0,\"file_changes\":[]}" && _response != "-1")      // -1: for error
            {
                _jsonStr = _response;
                // onpollRest(EventArgs.Empty);   no event
                result = _response;
            }

            return result;
        }

        private string getSampleData()
        {

            //  string filename = @"C:\Users\ruddyl.lee\Pictures\json\sample.json"; //2013-01-01.json";
            string text = System.IO.File.ReadAllText(@"C:\Users\ruddy\Pictures\json\sample.json");
            var result1 = JsonConvert.DeserializeObject<dynamic>(text);

            dynamic array = result1;

            return result1.ToString();

        }
        public void active_timer()
        {
            dispatcherTimer.Start();
        }
        public void deactive_timer()
        {
            dispatcherTimer.Stop();
        }

        bool inTimer = false;
        public void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (inTimer == true) return;

            string _response = "";
            _response = processCommand("nextFiles");
            var labelList = _response;
            if (_response == "{\"remaining_count\":0,\"file_changes\":[]}")         // no advance data
            {
                _response = "-1";
            }
            if (_response != "-1")  // files_from_seq not change(-1: exception, 0: no data)
            {
                _jsonStr = _response;
                onpollRest(EventArgs.Empty);            // call event
            }
            else
            {
                _json = "{}";
            }

            inTimer = false;
        }

        public event EventHandler _pollRest = delegate { };
        object sender = null;
        // public event EventHandler rest_poll = delegate { };
        protected virtual void onpollRest(EventArgs e)
        {
            if (_pollRest != null)
            {
                Wpf_getRest.ObjectExtension.MyEventArgs _v = new Wpf_getRest.ObjectExtension.MyEventArgs();
                _v.dataString = _jsonStr;
                _pollRest(this, _v);
            }
        }
        #endregion

        public string processCommand(string cmd, [Optional] string parm1)
        {
            string result = "";
            string _response = "";
            if (cmd == "pendingFiles")
            {
                remaining_count = 0;
                _response = processAllFile();

            }
            else if (cmd == "Labels")
            {
                _response = HttpGet(getAllLabel_cmd);
                var labelList = _response;
            }
            else if (cmd == "nextFiles")
            {
                string getnextFile_cmd = getNextFiles_cmd0 + current_device + "&seq=" + files_from_seq.ToString() + "&limit=500"; // getNextFiles_cmd1;   //getNextFiles_cmd + files_from_seq.ToString() + "&limit=500";
                _response = HttpGet(getnextFile_cmd);
                var labelList = _response;
            }
            else if (cmd == "updateLabel")
            {
                service_updateLabel(parm1);
            }
            else if (cmd == "sort")
            {
                string inData = parm1;
                bool result1 = service_sort(inData);
            }
            else
            {
                result = "";
            }
            //var remaining_count = result1.remaining_count;
            result = _response;

            return result;
        }

        // Tell Station event layout
        private bool service_sort(string inData)
        {
            bool result = false;
            if (inData != "")
            {
                send2Station(inData);
            }
            else
            {
                result = false;
            }

            return result;
        }

        private void send2Station(string inData)
        {
            using (WebClient webClient = new WebClient())
            {
                string address = HostIP + "/pending/sort/";   // sort/how=...
                //address = "http://localhost";
                byte[] postData = Encoding.ASCII.GetBytes(inData);
                webClient.Headers[HttpRequestHeader.Accept] = "text/html, application/xhtml+xml, */*";
                webClient.Headers[HttpRequestHeader.AcceptLanguage] = "ru-RU";
                webClient.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
                webClient.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                webClient.UploadData(address, postData);
            }
        }

        // update all tag file
        // 1. clear current label
        // 2. tag all file one by one
        // {'label_id':'67307b5d-49d3-4af6-b112-c57ee1794e52','label_name':'TAG','files':['4295ed89-a162-41a5-b3a8-f829bc5832de','fe49fb4f-27a2-4470-bac5-e95118f3ed51']}
        private void service_updateLabel(string jsonStr)
        {
            try
            {
                var result1 = JsonConvert.DeserializeObject<dynamic>(jsonStr);
                var allCollections = result1.files;
                var labelid = result1.label_id;
                // 1. 
                clearTag(labelid.ToString());
                // 2. 
                // the last page: get the [files_from_seq]
                dynamic array = allCollections;
                foreach (var token in array)
                {
                    tagLabel_call(token, labelid.ToString());
                }
            }
            catch (Exception err)
            {
            }
        }

        private void tagLabel_call(dynamic token, string label_id)
        {
            string file_guid = token.ToString();
            service_tagLabel(file_guid, label_id);
        }

        private void service_tagLabel(string file_guid, string label_id)
        {
            //  "/label/tag?file_id=";  // + file_guid + "&label_id=" + label_id;"; 
            tagLabel_cmd = tagLabel_cmd + "/label/tag?file_id=" + file_guid + "&label_id=" + label_id;
            string _response = HttpGet(tagLabel_cmd);
        }

        private void clearTag(string label_id)
        {
            try
            {
                string _url = clearLabel_cmd + label_id;
                HttpGet(tagLabel_cmd);
            }
            catch (Exception err)
            {
                // clear fail
            }
        }
        string _data = "{ 'remaining_count': 000,'file_changes':[{ 'id': 'C2DEC3E8-DF1C-4074-8AEE-D03E355A5988',file_name: 'IMG_0001.jpg', 'taken_time': '2010-10-20 30:40:50','width': 1024,'height': 768,'size': 10234532,'type': 0,'dev_id': '725EBFFA-0BCB-4EB6-B0AE-A5C2B558A383','dev_name': 'GT-I9300','dev_type': 0,'seq': 11234}]}";
        public string _json = "";
        private string processAllFile()
        {
            string result = "";
            string _json = "";
            int i = 0;
            try
            {
                remaining_count = 1;
                while (remaining_count != 0)
                {
                    string getPending_cmd = getPending_cmd0 + current_device + getPending_cmd1;
                    _data = HttpGet(getPending_cmd);
                    var result1 = JsonConvert.DeserializeObject<dynamic>(_data);
                    remaining_count = (int)result1.remaining_count;
                    var result2 = result1.file_changes;
                    if (i == 0)
                    {
                        dynamic array = result2;
                        var last = array.Last;
                        var count = last.seq;                         // get the file_from_seq here!
                        files_from_seq = (int)count + 1;

                        _json = _data;
                    }
                    else
                    {
                        join2json(_data);      // update into _json
                    }
                    i++;
                }
            }
            catch (Exception err)
            {
                _json = "-1";
            }
            result = _json;
            return result;
        }


        private void join2json(string temp)
        {
            try
            {
                var result1 = JsonConvert.DeserializeObject<dynamic>(temp);
                var allCollections = result1.file_changes;

                // the last page: get the [files_from_seq]
                dynamic array = allCollections;
                var last = array.Last;
                var count = last.seq;                         // get the file_from_seq here!
                files_from_seq = (int)count + 1;
                var result2 = JsonConvert.DeserializeObject<dynamic>(_json);
                JArray _a = JArray.Parse(_json);
                //
                for (int i = 0; i < allCollections.Count; i++)
                {
                    _a.Add(allCollections[i]);
                }

                _json = _a.ToString();
            }
            catch (Exception err)
            {

            }
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
        #endregion
    }

    public static class ObjectExtension
    {
        public static void RaiseEvent(this object obj, EventHandler handler, EventArgs e)
        {
            if (handler == null)
                return;
            handler(obj, e);
        }

        public static void RaiseEvent<TEventArgs>(this object obj, EventHandler<TEventArgs> handler, TEventArgs e) where TEventArgs : EventArgs
        {
            if (handler == null)
                return;
            handler(obj, e);
        }

        public class MyEventArgs : EventArgs { public string dataString;}     // pass data by Eventargs
    }
}
