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
    public partial class ClipBoardConfigurator : Form
    {
        public ClipBoardConfigurator()
        {
            InitializeComponent();
        }

        private ClipBoardUserSettings settings;

        private void loadConfigurationFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settings = new ClipBoardUserSettings();
            propertyGrid1.SelectedObject = settings;
            propertyGrid1.Enabled = true;
        }

        private void saveConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settings.Save();
        }
    }
}
