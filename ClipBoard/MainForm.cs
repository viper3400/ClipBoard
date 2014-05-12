using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClipBoard
{
    public partial class MainForm : Form
    {
        public ListView list;
        public MainForm()
        {
            InitializeComponent();
            list = this.listView;
            list.Columns[1].Width = this.list.Width - 50;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
        public void updateList(Keys keys)
        {
            int index = this.list.Items.Count + 1;
            //control-c pressed
            if ((ModifierKeys & Keys.Control) == Keys.Control && keys == Keys.C)
            {
                string content = Clipboard.GetText();
                if (content.Length != 0)
                {
                    if (list.Items.Count == 0) 
                    {
                        list.Items.Add(new ListViewItem(new string[] { index.ToString(), content }));                    
                    }
                    else
                    {
                        string prevCoutent = list.Items[list.Items.Count-1].SubItems[1].Text;
                        if (!content.Equals(prevCoutent))
                        {
                            list.Items.Add(new ListViewItem(new string[] { index.ToString(), content })); 
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
