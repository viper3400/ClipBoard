using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClipBoard
{
    public partial class MainForm : Form
    {
        public ListView list;
        private static string contentFileName = "../../content.csv";
        private static int maxCopyTextLength = 10000;
        private List<ClipBoardRecord> savedItems;
        private List<ClipBoardRecord> frequentItems;
        private List<ClipBoardRecord> recentItems;
        
        public MainForm()
        {
            InitializeComponent();
            list = this.listView;
            savedItems = new List<ClipBoardRecord>(10);
            recentItems = new List<ClipBoardRecord>(10);
            frequentItems = new List<ClipBoardRecord>(10);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            list.Columns[3].Width = this.list.Width - 50;
            loadContent(contentFileName);
            updateList();
        }

        private void updateList()
        {
            list.Items.Clear();
            ClipBoardRecord mostCoppiedRecord = new ClipBoardRecord();
            ClipBoardRecord mostPastedRecord = new ClipBoardRecord();
            int i = 1;

            // Add in saved records
            foreach (ClipBoardRecord s in savedItems)
            {
                ListViewItem lvi = 
                    new ListViewItem(new string[] { (i++).ToString(),
                                                    s.CoppiedCount.ToString(),
                                                    s.PastedCount.ToString(),
                                                    s.Content
                                                  });
                list.Items.Add(lvi);
                list.Groups[0].Items.Add(lvi);
            }

            // Calcualte both the most coppied and most pasted records
            foreach (ClipBoardRecord s in recentItems)
            {
                // work out for use later on the most coppied record
                if (mostCoppiedRecord.CoppiedCount < s.CoppiedCount)
                {
                    mostCoppiedRecord = s;
                }
                // work out for use later on the most pasted record
                if (mostPastedRecord.PastedCount < s.PastedCount)
                {
                    mostPastedRecord = s;
                }
            }

            frequentItems.Clear();
            if (mostCoppiedRecord.CoppiedCount > 0)
            {
                frequentItems.Add(mostCoppiedRecord);
            }
            // is mostCoppied is the same as mostPasted then only add one instance
            if (mostPastedRecord.PastedCount > 0 && mostCoppiedRecord != mostPastedRecord)
            {
                frequentItems.Add(mostPastedRecord);
            }
            foreach (ClipBoardRecord s in frequentItems)
            {
                ListViewItem lvi =
                    new ListViewItem(new string[] { (i++).ToString(),
                                                    s.CoppiedCount.ToString(),
                                                    s.PastedCount.ToString(),
                                                    s.Content
                                                  });

                list.Items.Add(lvi);
                list.Groups[1].Items.Add(lvi);
            }

            // finally populate the recent list
            foreach (ClipBoardRecord s in recentItems)
            {
                ListViewItem lvi =
                    new ListViewItem(new string[] { (i++).ToString(),
                                                    s.CoppiedCount.ToString(),
                                                    s.PastedCount.ToString(),
                                                    s.Content
                                                  });
                list.Items.Add(lvi);
                list.Groups[2].Items.Add(lvi);
            }

            //save to csv
            writeToCsv();
        }
         
        private void writeToCsv()
        {
            string[] lines = new string[savedItems.Count + Math.Min(recentItems.Count, 30)];
            int i = 0;
            foreach (ClipBoardRecord s in savedItems)
            {
                lines[i++] = "|," + s.CoppiedCount + "," + s.PastedCount + "," + "saved: " + Regex.Escape(s.Content);
            }
            foreach (ClipBoardRecord s in recentItems)
            {
                lines[i++] = "|," + s.CoppiedCount + "," + s.PastedCount + "," + "recent:" + Regex.Escape(s.Content);
                if (i >= savedItems.Count + 30)
                {
                    break;
                }
            }
            File.WriteAllLines(contentFileName, lines);
        }

        private void loadContent(string contentFileName)
        {
            char[] delimiterChars = { ',' };
            string[] fileFields;
            string[] lines = File.ReadAllLines(contentFileName);
            string type;

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
        }

        public async void keyPressedHandler(Keys keys)
        {
            //control-c pressed
            if ((ModifierKeys & Keys.Control) == Keys.Control && keys == Keys.C)
            {
                await Task.Delay(3000);
                addClipBoardRecord(Clipboard.GetText());
            }

            //control-` pressed
            if ((ModifierKeys & Keys.Control) == Keys.Control && keys == Keys.Oemtilde)
            {
                showScreen();
            }

            //control-p pressed
            if ((ModifierKeys & Keys.Control) == Keys.Control && keys == Keys.V)
            {
                recordPaste();
            }
        }

        // Add either new record or increment existing record counter
        private void addClipBoardRecord(string content)
        {
            ClipBoardRecord rec;

            //accept content only of not empty and not too big
            if (content.Length != 0 && content.Length < maxCopyTextLength)
            {
                rec = getClipBoardRecordViaContent(content);

                if (rec == null) // this is a new content
                {
                    // add a new record to the list
                    rec = new ClipBoardRecord(content, 1, 0);
                    recentItems.Insert(0, rec);
                }
                else
                {
                    // increment the existing matching record
                    rec.CoppiedCount++;
                }

                //limit number of recent items
                if (recentItems.Count > 100)
                {
                    recentItems.RemoveAt(recentItems.Count - 1);
                }
                updateList();
            }
        }

        // code so show list screen
        private void showScreen()
        {
            notifyIcon.Visible = false;
            this.Show();
            this.WindowState = FormWindowState.Normal;

            //bring to front if not
            this.TopMost = true;
            this.TopMost = false;
        }

        // increment the pasted counter for current clipboard content
        private void recordPaste()
        {            
            ClipBoardRecord clipBoardRecord;
            if (Clipboard.ContainsText() && Clipboard.GetText().Length < maxCopyTextLength)
            {
                clipBoardRecord = getClipBoardRecordViaContent(Clipboard.GetText());
                if (clipBoardRecord != null)
                {
                    clipBoardRecord.PastedCount++;
                    updateList();
                }
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            list.Columns[3].Width = this.list.Width - 50;
        }

        private async void listView_DoubleClick(object sender, EventArgs e)
        {
            copyTextToClipBoard();

            //hide after text copied to clipboard
            this.WindowState = FormWindowState.Minimized;

            // paste to curreMonkey talk font cursor
            await Task.Delay(500);
            SendKeys.Send("^V");
        }

        private void copyTextToClipBoard()
        {
            if (this.list.SelectedIndices.Count > 0)
            {
                int index = this.list.SelectedIndices[0];
                string content = this.list.Items[index].SubItems[3].Text;
                incrementPasted(content);
                Clipboard.SetText(content);
            }
        }

        private void incrementPasted(string content)
        {
            foreach (ClipBoardRecord s in savedItems)
            {
                if (s.Content == content)
                    s.PastedCount++;
            }
            foreach (ClipBoardRecord s in recentItems)
            {
                if(s.Content == content)
                    s.PastedCount++;
            }
            updateList();
        }

        private void listView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip.Show(list, e.X, e.Y);

                if (list.SelectedItems[0].Group.Equals(list.Groups[0])) // in Saved Group
                {
                    saveToolStripMenuItem.Enabled = false;
                }
                else // in Recent group
                {
                    saveToolStripMenuItem.Enabled = true;
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.listView_DoubleClick(sender, e);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipBoardRecord rec;
            rec = getClipBoardRecordViaContent(list.Items[list.SelectedIndices[0]].SubItems[3].Text);
            savedItems.Add(rec);
            recentItems.Remove(rec);
            updateList();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeClipBoardRecordViaContent(list.SelectedItems[0].SubItems[3].Text);            
            updateList();
        }

        // Given a content this function will remove a clipboard 
        // record if it exists in either the saved or recent list
        private void removeClipBoardRecordViaContent(string content)
        {
            // if ti exists it will only be in one list. 
            // so its safe to try and remove from both.
            savedItems.Remove(getClipBoardRecordViaContent(content));
            recentItems.Remove(getClipBoardRecordViaContent(content));
        }

        private ClipBoardRecord getClipBoardRecordViaContent(string content)
        {
            ClipBoardRecord foundRecord = null;
            foreach (ClipBoardRecord rec in savedItems)
            {
                if (rec.Content == content)
                    foundRecord = rec;
            }
            foreach (ClipBoardRecord rec in recentItems)
            {
                if (rec.Content == content)
                    foundRecord = rec;
            }
            return foundRecord;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon.Visible = true;
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon.Visible = false;
            }
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            notifyIcon.Visible = false;
            this.WindowState = FormWindowState.Normal;
            this.Show();
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            this.WindowState = FormWindowState.Normal;
            this.Show();
        }

    }
}
