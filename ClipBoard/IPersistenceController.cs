using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipBoard
{
    interface IPersistenceController
    {
        void SaveToFile(string FileName, List<ClipBoardRecord> SavedItems, List<ClipBoardRecord> RecentItems);
        Dictionary<string, List<ClipBoardRecord>> LoadFromFile(string FileName);
    }
}
