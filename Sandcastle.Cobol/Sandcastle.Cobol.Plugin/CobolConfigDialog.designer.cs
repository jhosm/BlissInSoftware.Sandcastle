namespace BlissInSoftware.Sandcastle.Cobol.PlugIn
{
    partial class CobolConfigDialog
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
            this.txtCobolProgramsPath = new System.Windows.Forms.TextBox();
            this.txtShowFolderBrowser = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtOk
            // 
            this.BtOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtOk.Location = new System.Drawing.Point(374, 35);
            this.BtOk.Name = "BtOk";
            this.BtOk.Size = new System.Drawing.Size(75, 23);
            this.BtOk.TabIndex = 0;
            this.BtOk.Text = "Ok";
            this.BtOk.UseVisualStyleBackColor = true;
            // 
            // BtCancel
            // 
            this.BtCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtCancel.Location = new System.Drawing.Point(455, 35);
            this.BtCancel.Name = "BtCancel";
            this.BtCancel.Size = new System.Drawing.Size(75, 23);
            this.BtCancel.TabIndex = 1;
            this.BtCancel.Text = "Cancel";
            this.BtCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Cobol Programs Path:";
            // 
            // txtCobolProgramsPath
            // 
            this.txtCobolProgramsPath.Location = new System.Drawing.Point(127, 6);
            this.txtCobolProgramsPath.Name = "txtCobolProgramsPath";
            this.txtCobolProgramsPath.Size = new System.Drawing.Size(360, 20);
            this.txtCobolProgramsPath.TabIndex = 3;
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
            // CobolConfigDialog
            // 
            this.AcceptButton = this.BtOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 65);
            this.Controls.Add(this.txtShowFolderBrowser);
            this.Controls.Add(this.txtCobolProgramsPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtCancel);
            this.Controls.Add(this.BtOk);
            this.Name = "CobolConfigDialog";
            this.Text = "Cobol Configuration";
            this.Load += new System.EventHandler(this.CobolConfigDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtOk;
        private System.Windows.Forms.Button BtCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TextBox txtCobolProgramsPath;
        private System.Windows.Forms.Button txtShowFolderBrowser;
    }
}