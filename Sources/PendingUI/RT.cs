using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Waveface
{
    public class RT
    {
        public List<List<FileChange>> Events { get; set; }
        public RtData RtData { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Dictionary<string, DateTime> DateTimeCache { get; set; }

        public bool Init(string json)
        {
            JsonSerializerSettings _settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            try
            {
                RtData = JsonConvert.DeserializeObject<RtData>(json, _settings);
            }
            catch
            {
                return false;
            }

            SortRowDate();
            ParserDate();

            StartDate = DateTimeCache[RtData.file_changes[0].taken_time].Date;
            EndDate = DateTimeCache[RtData.file_changes[RtData.file_changes.Count - 1].taken_time].Date;

            return true;
        }

        private void SortRowDate()
        {
            RtData.file_changes = RtData.file_changes.OrderBy(o => o.taken_time).ToList();
        }

        private void ParserDate()
        {
            DateTimeCache = new Dictionary<string, DateTime>();

            foreach (FileChange _item in RtData.file_changes)
            {
                string _dt = _item.taken_time;

                if (!DateTimeCache.ContainsKey(_dt))
                {
                    DateTimeCache.Add(_dt, DateTime.Parse(_dt));
                }
            }
        }

        public void GroupingByEvent(int minutes)
        {
            DateTime _ptDT = new DateTime(1970, 1, 1);
            List<FileChange> _currentEvent = new List<FileChange>();

            Events = new List<List<FileChange>>();

            foreach (FileChange _item in RtData.file_changes)
            {
                DateTime _dt = DateTimeCache[_item.taken_time];

                if (_ptDT.AddMinutes(minutes) < _dt)
                {
                    _currentEvent = new List<FileChange>();
                    Events.Add(_currentEvent);
                }

                _currentEvent.Add(_item);

                _ptDT = _dt;
            }
        }
    }
}