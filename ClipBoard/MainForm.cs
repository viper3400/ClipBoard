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
        public MainForm()
        {
            InitializeComponent();
            list = this.listView;
            savedItems = new List<string>(10);
            recentItems = new List<string>(10);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            list.Columns[1].Width = this.list.Width - 50;
            loadContent(contentFileName);
            updateList();
        }

        private void updateList()
        {
            removeDuplicates();
            list.Items.Clear();
            int i = 1;
            foreach (string s in savedItems)
            {
                //string content = Regex.Unescape(s);
                ListViewItem lvi = 
                    new ListViewItem(new string[] { (i++).ToString(), s });
                list.Items.Add(lvi);
                list.Groups[0].Items.Add(lvi);
            }
            foreach (string s in recentItems)
            {
                //string content = Regex.Unescape(s);
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

        //[DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();

        public async void keyPressedHandler(Keys keys)
        {
            //control-c pressed
            if ((ModifierKeys & Keys.Control) == Keys.Control && keys == Keys.C)
            {
                await Task.Delay(3000);
                string content = Clipboard.GetText();
                if (content.Length != 0)
                {
                    //accept content only of not empty and not too big
                    if (recentItems.Count != 0 && content.Length < 10000)
                    {
                        //recentItems.Add(content);
                        recentItems.Insert(0, content); //add to top
                        updateList();
                    }
                }
            }

            //control-` pressed
            if ((ModifierKeys & Keys.Control) == Keys.Control && keys == Keys.Oemtilde)
            {
                notifyIcon.Visible = false;
                this.Show();
                this.WindowState = FormWindowState.Normal;
                //if (GetForegroundWindow() != Process.GetCurrentProcess().MainWindowHandle)
                //{
                //    this.TopLevel = true;
                //}
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            list.Columns[1].Width = this.list.Width - 50;
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            copyTextToClipBoard();

            //hide after text copied to clipboard
            this.WindowState = FormWindowState.Minimized;
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
                //notifyIcon.ShowBalloonTip(200);
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
