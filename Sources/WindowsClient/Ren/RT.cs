#region

using System.IO;
using InfiniteStorage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using Waveface.Model;

#endregion

namespace Waveface.Client
{
    public class RT
    {
        public List<List<FileChange>> Events { get; set; }
        public RtData RtData { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Dictionary<string, DateTime> DateTimeCache { get; set; }

        public bool Init(List<FileAsset> files, List<PendingFile> pendingFiles, IService device)
        {
            RtData = new RtData();

            string _ThumbsPath = (string) Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "ResourceFolder", "");
            _ThumbsPath= Path.Combine(_ThumbsPath, ".thumbs");

            List<FileChange> _pFCs = pendingFiles.Select(x => new FileChange
                                                                  {
                                                                      id = x.file_id.ToString(),
                                                                      file_name = x.file_name,
                                                                      tiny_path =
                                                                      (x.type == (int)FileAssetType.image)
                                                                        ? Path.Combine(_ThumbsPath, x.file_id + ".tiny.thumb")
                                                                        : Path.Combine(_ThumbsPath, x.file_id + ".medium.thumb"),
                                                                      taken_time = x.event_time.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                      width = x.width,
                                                                      height = x.height,
                                                                      size = x.file_size,
                                                                      type = x.type,
                                                                  }).ToList();


            List<FileChange> _fCs = files.Select(x => new FileChange
                                                                  {
                                                                      id = x.file_id.ToString(),
                                                                      file_name = x.file_name,
                                                                      tiny_path =
                                                                      (x.type == (int)FileAssetType.image)
                                                                        ? Path.Combine(_ThumbsPath, x.file_id + ".tiny.thumb")
                                                                        : Path.Combine(_ThumbsPath, x.file_id + ".medium.thumb"),
                                                                      taken_time = x.event_time.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                      width = x.width,
                                                                      height = x.height,
                                                                      size = x.file_size,
                                                                      type = x.type,
                                                                  }).ToList();


            RtData.file_changes = _fCs;
            RtData.file_changes.AddRange(_pFCs);

            SortRowDate();
            ParserDate();

            if (RtData.file_changes.Count == 0)
            {
                return false;
            }

            StartDate = DateTimeCache[RtData.file_changes[0].taken_time].Date;
            EndDate = DateTimeCache[RtData.file_changes[RtData.file_changes.Count - 1].taken_time].Date;

            GC.Collect();

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
            if (minutes < 24 * 60)
            {
                ByMinutes(minutes);
            }

            if (minutes == (UnSortedFilesUC.BY_DAY))
            {
                By_Day(UnSortedFilesUC.BY_DAY);
            }

            if (minutes == (UnSortedFilesUC.BY_WEEK))
            {
                By_Day(UnSortedFilesUC.BY_WEEK);
            }

            if (minutes == (UnSortedFilesUC.BY_MONTH))
            {
                By_Day(UnSortedFilesUC.BY_MONTH);
            }
        }

        private void ByMinutes(int minutes)
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

        private void By_Day(int dayType)
        {
            Dictionary<string, List<FileChange>> _day_Files = new Dictionary<string, List<FileChange>>();

            Events = new List<List<FileChange>>();

            foreach (FileChange _item in RtData.file_changes)
            {
                DateTime _dt = DateTimeCache[_item.taken_time];

                string _by = string.Empty;

                if (dayType == UnSortedFilesUC.BY_DAY)
                {
                    _by = _dt.ToString("yyyy-MM-dd");
                }

                if (dayType == UnSortedFilesUC.BY_WEEK)
                {
                    _by = UnSortedFilesUC.StartOfWeek(_dt).ToString("yyyy-MM-dd");
                }

                if (dayType == UnSortedFilesUC.BY_MONTH)
                {
                    _by = _dt.ToString("yyyy-MM");
                }

                if (!_day_Files.ContainsKey(_by))
                {
                    _day_Files.Add(_by, new List<FileChange>());
                }

                _day_Files[_by].Add(_item);
            }

            _day_Files.Keys.ToList().Sort();

            foreach (string _day in _day_Files.Keys)
            {
                Events.Add(_day_Files[_day]);
            }
        }

        public void RemoveFileChanges(List<FileChange> fileChanges)
        {
            List<FileChange> _rms = new List<FileChange>();

            foreach (FileChange _fc in fileChanges)
            {
                foreach (FileChange _fileChange in RtData.file_changes)
                {
                    if (_fc.id == _fileChange.id)
                    {
                        _rms.Add(_fileChange);
                        break;
                    }
                }
            }

            foreach (FileChange _fc in _rms)
            {
                RtData.file_changes.Remove(_fc);
            }
        }
    }
}