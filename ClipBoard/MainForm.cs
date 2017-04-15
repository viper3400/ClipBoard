using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace ClipBoard
{
    public partial class MainForm : Form
    {
        public ListView list;
        private static string contentFileName = Program.ContentFileName;
        private static int maxCopyTextLength = 10000;
        private List<ClipBoardRecord> savedItems;
        private List<ClipBoardRecord> frequentItems;
        private List<ClipBoardRecord> recentItems;
        private bool allowSaveAsNowLoaded = false;
        IntPtr _ClipboardViewerNext;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect, // x-coordinate of upper-left corner
            int nTopRect, // y-coordinate of upper-left corner
            int nRightRect, // x-coordinate of lower-right corner
            int nBottomRect, // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
         );

        public MainForm()
        {
            InitializeComponent();
            list = this.listView;
            this.FormBorderStyle = FormBorderStyle.None;
            savedItems = new List<ClipBoardRecord>(10);
            recentItems = new List<ClipBoardRecord>(10);
            frequentItems = new List<ClipBoardRecord>(10);
            this.MouseDown += new MouseEventHandler(Form_MouseDown);
            this.labelClipBoardManager.MouseDown += new MouseEventHandler(Form_MouseDown);
            _ClipboardViewerNext = ClipBoard.Program.SetClipboardViewer(this.Handle);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            list.TileSize = System.Drawing.Size.Empty;
            loadContent(contentFileName);
            updateList();
            allowSaveAsNowLoaded = true;
        }

        private void MainForm_FormClosing(Object sender, FormClosingEventArgs e)
        {
            ClipBoard.Program.ChangeClipboardChain(this.Handle, _ClipboardViewerNext);
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

            //resize the form to fit the number of items
            resizeForm();
        }

        private void writeToCsv()
        {
            // this function is a candadiate for error handling
            if (allowSaveAsNowLoaded)
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

                if (!Directory.Exists(Path.GetDirectoryName(contentFileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(contentFileName));
                }

                File.WriteAllLines(contentFileName, lines);
            }        
        }

        /// <summary>
        ///  Function to resize the heigh of this WinForm based on the number of itmes in the lists
        /// </summary>
        private void resizeForm()
        {
            this.Height = (listView.Items.Count * listView.Items[0].Bounds.Height)
                                + (listView.Groups.Count * listView.GetItemRect(0).Height)
                                + ((listView.Items.Count) + 100);

            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            linkLabelGitHub.Top = listView.Top + listView.Height + 5;

        }

        private void loadContent(string contentFileName)
        {
            char[] delimiterChars = { ',' };
            string[] fileFields;           
            string[] lines = File.Exists(contentFileName) ? File.ReadAllLines(contentFileName) : new string[] { };
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

        public void keyPressedHandler(Keys keys)
        {
            Keys ModKeys = ModifierKeys; // save locally to aid debugging
            //control-` pressed or control-shift-b
            if ((ModKeys & Keys.Control) == Keys.Control && (keys == Keys.Oemtilde || keys == Keys.Space))
            {
                showScreen();
            }

            //control-v pressed
            if ((ModKeys & Keys.Control) == Keys.Control && keys == Keys.V)
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
                if (recentItems.Count > 25)
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

            resizeForm();
        }

        private void hideScreen()
        {
            this.WindowState = FormWindowState.Minimized;
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

        private void handleClipboardChanged()
        {

            addClipBoardRecord(Clipboard.GetText());
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            copyTextToClipboardAndPaste();
        }

        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                copyTextToClipboardAndPaste();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                //hide after text copied to clipboard
                hideScreen();
            }
        }

        private async void copyTextToClipboardAndPaste()
        {
            copyTextToClipBoard();

            //hide after text copied to clipboard
            hideScreen();

            // paste to curreMonkey talk font cursor
            await Task.Delay(500);
            var inputSimulator = new InputSimulator();
            inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);            
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
                copyTextToClipboardAndPaste();
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


        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                /*
                Constants in Windows API
                0x2 = HTCAPTION - Application Title Bar
                */
                ClipBoard.Program.ReleaseCapture();
                ClipBoard.Program.SendMessage(Handle, (int)ClipBoard.Program.Msgs.WM_NCLBUTTONDOWN, (IntPtr)0x2, (IntPtr)0);
            }
        }
        protected override void WndProc(ref Message m)
        {

            switch ((ClipBoard.Program.Msgs)m.Msg)
            {
                //
                // The WM_DRAWCLIPBOARD message is sent to the first window 
                // in the clipboard viewer chain when the content of the 
                // clipboard changes. This enables a clipboard viewer 
                // window to display the new content of the clipboard. 
                //
                case ClipBoard.Program.Msgs.WM_DRAWCLIPBOARD:

                    handleClipboardChanged();

                    //
                    // Each window that receives the WM_DRAWCLIPBOARD message 
                    // must call the SendMessage function to pass the message 
                    // on to the next window in the clipboard viewer chain.
                    //
                    ClipBoard.Program.SendMessage(_ClipboardViewerNext, m.Msg, m.WParam, m.LParam);
                    break;


                //
                // The WM_CHANGECBCHAIN message is sent to the first window 
                // in the clipboard viewer chain when a window is being 
                // removed from the chain. 
                //
                case ClipBoard.Program.Msgs.WM_CHANGECBCHAIN:

                    // When a clipboard viewer window receives the WM_CHANGECBCHAIN message, 
                    // it should call the SendMessage function to pass the message to the 
                    // next window in the chain, unless the next window is the window 
                    // being removed. In this case, the clipboard viewer should save 
                    // the handle specified by the lParam parameter as the next window in the chain. 

                    //
                    // wParam is the Handle to the window being removed from 
                    // the clipboard viewer chain 
                    // lParam is the Handle to the next window in the chain 
                    // following the window being removed. 
                    if (m.WParam == _ClipboardViewerNext)
                    {
                        //
                        // If wParam is the next clipboard viewer then it
                        // is being removed so update pointer to the next
                        // window in the clipboard chain
                        //
                        _ClipboardViewerNext = m.LParam;
                    }
                    else
                    {
                        ClipBoard.Program.SendMessage(_ClipboardViewerNext, m.Msg, m.WParam, m.LParam);
                    }
                    break;

                default:
                    //
                    // Let the form process the messages that we are
                    // not interested in
                    //
                    base.WndProc(ref m);
                    break;
            }
        }

        private void labelMinimize_Click(object sender, EventArgs e)
        {
            hideScreen();
        }

        private void linkLabelGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabelGitHub.Text);
        }
    }
}
