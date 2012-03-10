using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BlissInSoftware.Sandcastle.Cobol.PlugIn
{
    public partial class CobolConfigDialog : Form
    {
        private readonly string currentlyConfiguredPath;
        
        public CobolConfigDialog(string currentlyConfiguredPath)
        {
            this.currentlyConfiguredPath = currentlyConfiguredPath;
            InitializeComponent();
        }

        public string CobolProgramsPath {
            get { return txtCobolProgramsPath.Text; }
        }

        private void showFolderBrowser_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
                txtCobolProgramsPath.Text = folderBrowserDialog.SelectedPath;
        }

        private void CobolConfigDialog_Load(object sender, EventArgs e)
        {
            txtCobolProgramsPath.Text = currentlyConfiguredPath;
        }

    
    }
}
