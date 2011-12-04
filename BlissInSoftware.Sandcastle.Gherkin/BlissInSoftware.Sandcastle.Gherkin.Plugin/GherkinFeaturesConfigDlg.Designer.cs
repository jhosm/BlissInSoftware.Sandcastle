namespace BlissInSoftware.Sandcastle.Gherkin.Plugin
{
    partial class GherkinFeaturesConfigDlg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BtOk = new System.Windows.Forms.Button();
            this.BtCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.txtFeaturesPath = new System.Windows.Forms.TextBox();
            this.txtShowFolderBrowser = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFeaturesLanguage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // BtOk
            // 
            this.BtOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtOk.Location = new System.Drawing.Point(374, 57);
            this.BtOk.Name = "BtOk";
            this.BtOk.Size = new System.Drawing.Size(75, 23);
            this.BtOk.TabIndex = 0;
            this.BtOk.Text = "Ok";
            this.BtOk.UseVisualStyleBackColor = true;
            // 
            // BtCancel
            // 
            this.BtCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtCancel.Location = new System.Drawing.Point(455, 57);
            this.BtCancel.Name = "BtCancel";
            this.BtCancel.Size = new System.Drawing.Size(75, 23);
            this.BtCancel.TabIndex = 1;
            this.BtCancel.Text = "Cancel";
            this.BtCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Features Path:";
            // 
            // txtFeaturesPath
            // 
            this.txtFeaturesPath.Location = new System.Drawing.Point(120, 6);
            this.txtFeaturesPath.Name = "txtFeaturesPath";
            this.txtFeaturesPath.Size = new System.Drawing.Size(367, 20);
            this.txtFeaturesPath.TabIndex = 3;
            // 
            // txtShowFolderBrowser
            // 
            this.txtShowFolderBrowser.Location = new System.Drawing.Point(493, 6);
            this.txtShowFolderBrowser.Name = "txtShowFolderBrowser";
            this.txtShowFolderBrowser.Size = new System.Drawing.Size(37, 23);
            this.txtShowFolderBrowser.TabIndex = 4;
            this.txtShowFolderBrowser.Text = "...";
            this.txtShowFolderBrowser.UseVisualStyleBackColor = true;
            this.txtShowFolderBrowser.Click += new System.EventHandler(this.showFolderBrowser_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Features Language:";
            // 
            // txtFeaturesLanguage
            // 
            this.txtFeaturesLanguage.Location = new System.Drawing.Point(120, 35);
            this.txtFeaturesLanguage.Name = "txtFeaturesLanguage";
            this.txtFeaturesLanguage.Size = new System.Drawing.Size(48, 20);
            this.txtFeaturesLanguage.TabIndex = 6;
            // 
            // GherkinFeaturesConfigDlg
            // 
            this.AcceptButton = this.BtOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 91);
            this.Controls.Add(this.txtFeaturesLanguage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtShowFolderBrowser);
            this.Controls.Add(this.txtFeaturesPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtCancel);
            this.Controls.Add(this.BtOk);
            this.Name = "GherkinFeaturesConfigDlg";
            this.Text = "Gherkin Features Configuration";
            this.Load += new System.EventHandler(this.GherkinFeaturesConfigDlg_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtOk;
        private System.Windows.Forms.Button BtCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TextBox txtFeaturesPath;
        private System.Windows.Forms.Button txtShowFolderBrowser;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFeaturesLanguage;
    }
}