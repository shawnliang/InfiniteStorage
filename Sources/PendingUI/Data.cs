#region

using System.Collections.Generic;

#endregion

namespace Waveface
{
    public class FileChange
    {
        public string id { get; set; }
        public string file_name { get; set; }
        public string tiny_path { get; set; }
        public string taken_time { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int size { get; set; }
        public int type { get; set; }
        public string dev_id { get; set; }
        public string dev_name { get; set; }
        public int dev_type { get; set; }
        public int seq { get; set; }
    }

    public class RtData
    {
        public int remaining_count { get; set; }
        public List<FileChange> file_changes { get; set; }
    }

    public class Event
    {
        public string time_start { get; set; }
        public string time_end { get; set; }
        public List<string> files { get; set; }
        public string title { get; set; }
        public int type { get; set; }

        public Event()
        {
            files = new List<string>();
        }
    }

    public class PendingSort
    {
        public string device_id { get; set; }
        public List<string> discards { get; set; }
        public List<Event> events { get; set; }

        public PendingSort()
        {
            discards = new List<string>();
            events = new List<Event>();
        }
    }
}