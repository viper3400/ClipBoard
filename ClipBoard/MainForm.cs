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
        private List<string> savedItems;
        private List<string> recentItems;
        IntPtr _ClipboardViewerNext;
        public MainForm()
        {
            InitializeComponent();
            list = this.listView;
            savedItems = new List<string>(10);
            recentItems = new List<string>(10);
            _ClipboardViewerNext = ClipBoard.Program.SetClipboardViewer(this.Handle);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            list.Columns[1].Width = this.list.Width - 50;
            loadContent(contentFileName);
            updateList();
        }

        private void MainForm_FormClosing(Object sender, FormClosingEventArgs e)
        {
            ClipBoard.Program.ChangeClipboardChain(this.Handle, _ClipboardViewerNext);
        }

        private void updateList()
        {
            removeDuplicates();
            list.Items.Clear();
            int i = 1;
            foreach (string s in savedItems)
            {
                ListViewItem lvi = 
                    new ListViewItem(new string[] { (i++).ToString(), s });
                list.Items.Add(lvi);
                list.Groups[0].Items.Add(lvi);
            }
            foreach (string s in recentItems)
            {
                ListViewItem lvi = 
                    new ListViewItem(new string[] { (i++).ToString(), s });
                list.Items.Add(lvi);
                list.Groups[1].Items.Add(lvi);
            }

            //frequently write to csv
            writeToCsv();
        }

        private void removeDuplicates()
        {
            for (int i = savedItems.Count - 1; i >= 0; i--)
            {
                if (savedItems.IndexOf(savedItems[i]) != i)
                {
                    savedItems.RemoveAt(i);
                }
            }
            for (int i = recentItems.Count - 1; i >= 0; i--)
            {
                if (recentItems.IndexOf(recentItems[i]) != i 
                    || savedItems.IndexOf(recentItems[i]) >= 0 )
                {
                    recentItems.RemoveAt(i);
                }
            }
        }

        private void writeToCsv()
        {
            string[] lines = new string[savedItems.Count + Math.Min(recentItems.Count, 30)];
            int i = 0;
            foreach (string s in savedItems)
            {
                lines[i++] = "saved: " + Regex.Escape(s);
            }
            foreach (string s in recentItems)
            {
                lines[i++] = "recent:" + Regex.Escape(s);
                if (i >= savedItems.Count + 30)
                {
                    break;
                }
            }
            File.WriteAllLines(contentFileName, lines);
        }

        private void loadContent(string contentFileName)
        {
            string[] lines = File.ReadAllLines(contentFileName);
            foreach (string s in lines)
            {
                if (s.StartsWith("saved:"))
                {
                    savedItems.Add(Regex.Unescape(s.Substring(7)));
                }
                else if (s.StartsWith("recent:"))
                {
                    recentItems.Add(Regex.Unescape(s.Substring(7)));
                }
            }
        }

        public void keyPressedHandler(Keys keys)
        {
            //control-` pressed
            if ((ModifierKeys & Keys.Control) == Keys.Control && keys == Keys.Oemtilde)
            {
                notifyIcon.Visible = false;
                this.Show();
                this.WindowState = FormWindowState.Normal;

                //bring to front if not
                this.TopMost = true;
                this.TopMost = false;
            }
        }

        private void handleClipboardChanged()
        {
            string content = Clipboard.GetText();
            if (content.Length != 0)
            {
                //accept content only of not empty and not too big
                if (recentItems.Count != 0 && content.Length < 10000)
                {
                    recentItems.Insert(0, content); //add to top

                    //limit number of recent items
                    if (recentItems.Count > 100)
                    {
                        recentItems.RemoveAt(recentItems.Count - 1);
                    }
                    updateList();
                }
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            list.Columns[1].Width = this.list.Width - 50;
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
                string content = this.list.Items[index].SubItems[1].Text;
                Clipboard.SetText(content);
            }
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
            int index = list.SelectedIndices[0] - list.Groups[0].Items.Count;
            savedItems.Add(recentItems[index]);
            updateList();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (list.SelectedItems[0].Group.Equals(list.Groups[0])) // in Saved Group
            {
                int index = list.SelectedIndices[0];
                savedItems.RemoveAt(index);
            }
            else // in Recent group
            {
                int index = list.SelectedIndices[0] - list.Groups[0].Items.Count;
                recentItems.RemoveAt(index);
            }
            updateList();
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
    }
}
