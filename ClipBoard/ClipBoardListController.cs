using Dapplo.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipBoard
{
    class ClipBoardListController
    {
        private List<ClipBoardRecord> _savedItems;
        private List<ClipBoardRecord> _recentItems;
        private ClipBoardUserSettings _settings;
        private static int _maxCopyTextLength;
        private static readonly LogSource Log = new LogSource();

        public ClipBoardListController(ClipBoardUserSettings SettingsProvider)
        {
            _savedItems = new List<ClipBoardRecord>();
            _recentItems = new List<ClipBoardRecord>();
            _settings = SettingsProvider;
            _maxCopyTextLength = _settings.MaxCopyTextLength;
        }

        public List<ClipBoardRecord> SavedItems
        {
            get { return _savedItems; }
            set { _savedItems = value; }
        }

        public List<ClipBoardRecord> FrequentItems
        {
            get
            {
                var frequentItems = _recentItems
                    .Where(rec => rec.PastedCount > 0)
                    .OrderByDescending(rec => rec.PastedCount).Take(_settings.MaxItemsInFrequentList);
                return frequentItems.ToList();
            }
        }
        public List<ClipBoardRecord> RecentItems
        {
            get { return _recentItems; }
            set { _recentItems = value; }
        }

        // Add either new record or increment existing record counter
        public void AddClipBoardRecord(string content)
        {
            Log.Verbose().Write("Add content to clipboard.");
            ClipBoardRecord rec;

            //accept content only of not empty and not too big
            if (content.Length != 0 && content.Length < _maxCopyTextLength)
            {
                rec = GetClipBoardRecordViaContent(content);

                if (rec == null) // this is a new content
                {
                    // add a new record to the list
                    rec = new ClipBoardRecord(content, 1, 0);
                    _recentItems.Insert(0, rec);
                }
                else
                {
                    // increment the existing matching record
                    rec.CoppiedCount++;
                }

                //limit number of recent items
                if (_recentItems.Count > 25)
                {
                    Log.Debug().Write("Recent items exceeded max size. Remove last item.");
                    _recentItems.RemoveAt(_recentItems.Count - 1);
                }
            }
            else Log.Warn().Write("Content emtpy or longer than defined max length.");
        }

        // Given a content this function will remove a clipboard 
        // record if it exists in either the saved or recent list
        public void RemoveClipBoardRecordViaContent(string content)
        {
            // if ti exists it will only be in one list. 
            // so its safe to try and remove from both.
            _savedItems.Remove(GetClipBoardRecordViaContent(content));
            _recentItems.Remove(GetClipBoardRecordViaContent(content));
        }

        public ClipBoardRecord GetClipBoardRecordViaContent(string content)
        {
            ClipBoardRecord foundRecord = null;
            foreach (ClipBoardRecord rec in _savedItems)
            {
                if (rec.Content == content)
                    foundRecord = rec;
            }
            foreach (ClipBoardRecord rec in _recentItems)
            {
                if (rec.Content == content)
                    foundRecord = rec;
            }
            return foundRecord;
        }

        public void IncrementPasted(string content)
        {
            foreach (ClipBoardRecord s in _savedItems)
            {
                if (s.Content == content)
                    s.PastedCount++;
            }
            foreach (ClipBoardRecord s in _recentItems)
            {
                if (s.Content == content)
                    s.PastedCount++;
            }
        }


    }
}
