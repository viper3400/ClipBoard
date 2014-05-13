using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClipBoard
{
    public partial class MainForm : Form
    {
        public ListView list;
        private static string contentFileName = "../../content.csv";
        public MainForm()
        {
            InitializeComponent();
            list = this.listView;
            list.Columns[1].Width = this.list.Width - 50;
            loadContent(contentFileName);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void loadContent(string contentFileName)
        {
            string[] lines = File.ReadAllLines(contentFileName);
            foreach (string s in lines)
            {
                if (s.StartsWith("saved:"))
                {
                    int cnt = list.Groups[0].Items.Count;
                    ListViewItem lvi = new ListViewItem(new string[]{(cnt+1).ToString(), s.Substring(7)});
                    list.Items.Add(lvi);
                    list.Groups[0].Items.Add(lvi);
                }
                else if (s.StartsWith("recent:"))
                {
                    int cnt1 = list.Groups[0].Items.Count;
                    int cnt2 = list.Groups[1].Items.Count;
                    ListViewItem lvi = new ListViewItem(new string[] { (cnt1+cnt2+1).ToString(), s.Substring(7) });
                    list.Items.Add(lvi);
                    list.Groups[1].Items.Add(lvi);
                    
                }
            }
        }
        public void updateList(Keys keys)
        {
            int index = this.list.Items.Count + 1;
            //control-c pressed
            if ((ModifierKeys & Keys.Control) == Keys.Control && keys == Keys.C)
            {
             // Thread.Sleep(2000);
                string content = Clipboard.GetText();
                if (content.Length != 0)
                {
                    if (list.Items.Count == 0) 
                    {
                        ListViewItem lvi = new ListViewItem(new string[] { index.ToString(), content });
                        list.Items.Add(lvi);
                        list.Groups[1].Items.Add(lvi);
                    }
                    else
                    {
                        string prevCoutent = list.Items[list.Items.Count-1].SubItems[1].Text;
                        if (!content.Equals(prevCoutent))
                        {
                            ListViewItem lvi = new ListViewItem(new string[] { index.ToString(), content });
                            list.Items.Add(lvi);
                            list.Groups[1].Items.Add(lvi);
                        }
                    }
                }
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            list.Columns[1].Width = this.list.Width - 50;
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            copyTextToClipBoard();
        }

        private void copyTextToClipBoard()
        {
            int index = this.list.SelectedIndices[0];
            string content = this.list.Items[index].SubItems[1].Text;
            Clipboard.SetText(content);
        }
    }
}
