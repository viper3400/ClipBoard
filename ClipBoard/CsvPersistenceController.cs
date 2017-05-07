using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClipBoard
{
    class CsvPersistenceController : IPersistenceController
    {
     
        public void SaveToFile(string FileName, List<ClipBoardRecord> SavedItems, List<ClipBoardRecord> RecentItems)
        {
            // this function is a candadiate for error handling
            string[] lines = new string[SavedItems.Count + Math.Min(RecentItems.Count, 30)];
            int i = 0;
            foreach (ClipBoardRecord s in SavedItems)
            {
                lines[i++] = "|," + s.CoppiedCount + "," + s.PastedCount + "," + "saved: " + Regex.Escape(s.Content);
            }
            foreach (ClipBoardRecord s in RecentItems)
            {
                lines[i++] = "|," + s.CoppiedCount + "," + s.PastedCount + "," + "recent:" + Regex.Escape(s.Content);
                if (i >= SavedItems.Count + 30)
                {
                    break;
                }
            }

            if (!Directory.Exists(Path.GetDirectoryName(FileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FileName));
            }

            File.WriteAllLines(FileName, lines);
        }
    }
}
