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
        private static int _maxCopyTextLength = 10000;

        public ClipBoardListController()
        {
            _savedItems = new List<ClipBoardRecord>();
            _recentItems = new List<ClipBoardRecord>();
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
                    .OrderByDescending(rec => rec.PastedCount).Take(10);
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
                    _recentItems.RemoveAt(_recentItems.Count - 1);
                }
            }
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
