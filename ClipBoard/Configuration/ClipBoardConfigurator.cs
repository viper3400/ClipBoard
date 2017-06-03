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
            InitForm();
        }

        public ClipBoardConfigurator(ClipBoardUserSettings Settings, bool SaveOnExit = false)
        {
            InitForm();
            _settings = Settings;
            propertyGrid1.SelectedObject = _settings;
            propertyGrid1.Enabled = true;
        }

        private void InitForm()
        {
            InitializeComponent();
        }

        private ClipBoardUserSettings _settings;

        private void loadConfigurationFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //settings = new ClipBoardUserSettings();
            //propertyGrid1.SelectedObject = _settings;
            //propertyGrid1.Enabled = true;
        }

        private void saveConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _settings.Save();
        }

        private void ClipBoardConfigurator_FormClosed(object sender, FormClosedEventArgs e)
        {
            _settings.Save();
        }
    }
}
