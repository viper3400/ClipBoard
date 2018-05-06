using Dapplo.Log;
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
        private static readonly LogSource Log = new LogSource();

        public Dictionary<string, List<ClipBoardRecord>> LoadFromFile(string FileName)
        {
            char[] delimiterChars = { ',' };
            string[] fileFields;
            string[] lines = File.Exists(FileName) ? File.ReadAllLines(FileName) : new string[] { };
            string type;
            List<ClipBoardRecord> savedItems = new List<ClipBoardRecord>();
            List<ClipBoardRecord> recentItems = new List<ClipBoardRecord>();

            foreach (string s in lines)
            {
                ClipBoardRecord rec = new ClipBoardRecord();

                // Find out if we have a saved file containing counts
                if (s.StartsWith("|")) // then new file format 
                {
                    fileFields = s.Split(delimiterChars, 4);
                    rec.CoppiedCount = int.Parse(fileFields[1]);
                    rec.PastedCount = int.Parse(fileFields[2]);
                    rec.Content = Regex.Unescape(fileFields[3].Substring(7));
                    type = fileFields[3].Substring(0, 7);
                }
                else // handle previous file format
                {
                    rec.Content = Regex.Unescape(s.Substring(7));
                    rec.CoppiedCount = 0;
                    rec.PastedCount = 0;
                    type = s.Substring(0, 7);
                }

                // now have have the data add it to the relevent list
                if (type.StartsWith("saved:"))
                {
                    savedItems.Add(rec);
                }
                else if (type.StartsWith("recent:"))
                {
                    recentItems.Add(rec);
                }
            }

            var items = new Dictionary<string, List<ClipBoardRecord>>();
            items.Add("saved", savedItems);
            items.Add("recent", recentItems);

            return items;
        }

        public void SaveToFile(string FileName, List<ClipBoardRecord> SavedItems, List<ClipBoardRecord> RecentItems)
        {
            try
            {                
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
            catch (Exception e)
            {
                Log.Error().Write($"Error while saving file: {FileName}.");
                Log.Error().Write(e.Message);
                throw;
            }
        }
    }
}
