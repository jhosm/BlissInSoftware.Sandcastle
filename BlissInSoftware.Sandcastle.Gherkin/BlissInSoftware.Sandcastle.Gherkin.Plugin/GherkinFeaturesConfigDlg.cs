using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BlissInSoftware.Sandcastle.Gherkin.Plugin
{
    public partial class GherkinFeaturesConfigDlg : Form
    {
        private readonly string currentlyConfiguredPath;
        
        public GherkinFeaturesConfigDlg(string currentlyConfiguredPath)
        {
            this.currentlyConfiguredPath = currentlyConfiguredPath;
            InitializeComponent();
        }

        public string GherkinFeaturesPath {
            get { return txtFeaturesPath.Text; }
        }

        private void showFolderBrowser_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
                txtFeaturesPath.Text = folderBrowserDialog.SelectedPath;
        }

        private void GherkinFeaturesConfigDlg_Load(object sender, EventArgs e)
        {
            txtFeaturesPath.Text = currentlyConfiguredPath;
        }

    
    }
}
