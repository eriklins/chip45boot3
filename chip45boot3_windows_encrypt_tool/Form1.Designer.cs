namespace hexfileEncrypt
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.comboBoxInputHexfileName = new System.Windows.Forms.ComboBox();
            this.buttonInputHexfileSelect = new System.Windows.Forms.Button();
            this.labelInputHexfile = new System.Windows.Forms.Label();
            this.openFileDialogInputHexfileSelect = new System.Windows.Forms.OpenFileDialog();
            this.buttonEncryptHexfile = new System.Windows.Forms.Button();
            this.progressBarHexEncrypt = new System.Windows.Forms.ProgressBar();
            this.labelProgName = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelProgressBar = new System.Windows.Forms.Label();
            this.labelXtaeKey = new System.Windows.Forms.Label();
            this.textBoxXtaeKey = new System.Windows.Forms.TextBox();
            this.labelOutputHexfile = new System.Windows.Forms.Label();
            this.buttonOutputHexfileSelect = new System.Windows.Forms.Button();
            this.comboBoxOutputHexfileName = new System.Windows.Forms.ComboBox();
            this.openFileDialogOutputHexfileSelect = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxConvertedArray = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // comboBoxInputHexfileName
            // 
            this.comboBoxInputHexfileName.DropDownWidth = 300;
            this.comboBoxInputHexfileName.FormattingEnabled = true;
            this.comboBoxInputHexfileName.Location = new System.Drawing.Point(12, 97);
            this.comboBoxInputHexfileName.Name = "comboBoxInputHexfileName";
            this.comboBoxInputHexfileName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.comboBoxInputHexfileName.Size = new System.Drawing.Size(287, 21);
            this.comboBoxInputHexfileName.TabIndex = 1;
            this.comboBoxInputHexfileName.SelectedIndexChanged += new System.EventHandler(this.comboBoxInputHexfileName_SelectedIndexChanged);
            // 
            // buttonInputHexfileSelect
            // 
            this.buttonInputHexfileSelect.Location = new System.Drawing.Point(308, 94);
            this.buttonInputHexfileSelect.Name = "buttonInputHexfileSelect";
            this.buttonInputHexfileSelect.Size = new System.Drawing.Size(120, 25);
            this.buttonInputHexfileSelect.TabIndex = 2;
            this.buttonInputHexfileSelect.Text = "Select Input Hexfile";
            this.buttonInputHexfileSelect.UseVisualStyleBackColor = true;
            this.buttonInputHexfileSelect.Click += new System.EventHandler(this.buttonInputHexfileSelect_Click);
            // 
            // labelInputHexfile
            // 
            this.labelInputHexfile.AutoSize = true;
            this.labelInputHexfile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInputHexfile.Location = new System.Drawing.Point(12, 81);
            this.labelInputHexfile.Name = "labelInputHexfile";
            this.labelInputHexfile.Size = new System.Drawing.Size(92, 13);
            this.labelInputHexfile.TabIndex = 2;
            this.labelInputHexfile.Text = "Plain Input Hexfile";
            // 
            // openFileDialogInputHexfileSelect
            // 
            this.openFileDialogInputHexfileSelect.FileName = "*.hex";
            this.openFileDialogInputHexfileSelect.Filter = "Intel Hex|*.hex|All Files|*.*";
            this.openFileDialogInputHexfileSelect.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialogInputHexfileSelect_FileOk);
            // 
            // buttonEncryptHexfile
            // 
            this.buttonEncryptHexfile.Location = new System.Drawing.Point(308, 288);
            this.buttonEncryptHexfile.Name = "buttonEncryptHexfile";
            this.buttonEncryptHexfile.Size = new System.Drawing.Size(120, 25);
            this.buttonEncryptHexfile.TabIndex = 7;
            this.buttonEncryptHexfile.Text = "Encrypt Hexfile";
            this.buttonEncryptHexfile.UseVisualStyleBackColor = true;
            this.buttonEncryptHexfile.Click += new System.EventHandler(this.buttonEncryptHexfile_Click);
            // 
            // progressBarHexEncrypt
            // 
            this.progressBarHexEncrypt.Location = new System.Drawing.Point(12, 288);
            this.progressBarHexEncrypt.Maximum = 1000;
            this.progressBarHexEncrypt.Name = "progressBarHexEncrypt";
            this.progressBarHexEncrypt.Size = new System.Drawing.Size(287, 25);
            this.progressBarHexEncrypt.TabIndex = 24;
            // 
            // labelProgName
            // 
            this.labelProgName.AutoSize = true;
            this.labelProgName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProgName.Location = new System.Drawing.Point(7, 13);
            this.labelProgName.Name = "labelProgName";
            this.labelProgName.Size = new System.Drawing.Size(308, 25);
            this.labelProgName.TabIndex = 29;
            this.labelProgName.Text = "chip45boot3 Hexfile Encrypt";
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(9, 38);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(60, 13);
            this.labelVersion.TabIndex = 30;
            this.labelVersion.Text = "Version 1.1";
            this.labelVersion.Click += new System.EventHandler(this.labelVersion_Click);
            // 
            // labelProgressBar
            // 
            this.labelProgressBar.AutoSize = true;
            this.labelProgressBar.Location = new System.Drawing.Point(12, 272);
            this.labelProgressBar.Name = "labelProgressBar";
            this.labelProgressBar.Size = new System.Drawing.Size(48, 13);
            this.labelProgressBar.TabIndex = 31;
            this.labelProgressBar.Text = "Progress";
            // 
            // labelXtaeKey
            // 
            this.labelXtaeKey.AutoSize = true;
            this.labelXtaeKey.Location = new System.Drawing.Point(12, 178);
            this.labelXtaeKey.Name = "labelXtaeKey";
            this.labelXtaeKey.Size = new System.Drawing.Size(219, 13);
            this.labelXtaeKey.TabIndex = 45;
            this.labelXtaeKey.Text = "128 Bit Encryption Key (16 ASCII Characters)";
            // 
            // textBoxXtaeKey
            // 
            this.textBoxXtaeKey.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxXtaeKey.Location = new System.Drawing.Point(12, 194);
            this.textBoxXtaeKey.MaxLength = 16;
            this.textBoxXtaeKey.Name = "textBoxXtaeKey";
            this.textBoxXtaeKey.Size = new System.Drawing.Size(184, 20);
            this.textBoxXtaeKey.TabIndex = 44;
            this.textBoxXtaeKey.TextChanged += new System.EventHandler(this.textBoxXtaeKey_TextChanged);
            // 
            // labelOutputHexfile
            // 
            this.labelOutputHexfile.AutoSize = true;
            this.labelOutputHexfile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOutputHexfile.Location = new System.Drawing.Point(12, 129);
            this.labelOutputHexfile.Name = "labelOutputHexfile";
            this.labelOutputHexfile.Size = new System.Drawing.Size(125, 13);
            this.labelOutputHexfile.TabIndex = 47;
            this.labelOutputHexfile.Text = "Encrypted Output Hexfile";
            // 
            // buttonOutputHexfileSelect
            // 
            this.buttonOutputHexfileSelect.Location = new System.Drawing.Point(308, 142);
            this.buttonOutputHexfileSelect.Name = "buttonOutputHexfileSelect";
            this.buttonOutputHexfileSelect.Size = new System.Drawing.Size(120, 25);
            this.buttonOutputHexfileSelect.TabIndex = 48;
            this.buttonOutputHexfileSelect.Text = "Select Output Hexfile";
            this.buttonOutputHexfileSelect.UseVisualStyleBackColor = true;
            this.buttonOutputHexfileSelect.Click += new System.EventHandler(this.buttonOutputHexfileSelect_Click);
            // 
            // comboBoxOutputHexfileName
            // 
            this.comboBoxOutputHexfileName.DropDownWidth = 300;
            this.comboBoxOutputHexfileName.FormattingEnabled = true;
            this.comboBoxOutputHexfileName.Location = new System.Drawing.Point(12, 145);
            this.comboBoxOutputHexfileName.Name = "comboBoxOutputHexfileName";
            this.comboBoxOutputHexfileName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.comboBoxOutputHexfileName.Size = new System.Drawing.Size(287, 21);
            this.comboBoxOutputHexfileName.TabIndex = 46;
            this.comboBoxOutputHexfileName.SelectedIndexChanged += new System.EventHandler(this.comboBoxOutputHexfileName_SelectedIndexChanged);
            // 
            // openFileDialogOutputHexfileSelect
            // 
            this.openFileDialogOutputHexfileSelect.CheckFileExists = false;
            this.openFileDialogOutputHexfileSelect.FileName = "*.hex";
            this.openFileDialogOutputHexfileSelect.Filter = "Intel Hex|*.hex|All Files|*.*";
            this.openFileDialogOutputHexfileSelect.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialogOutputHexfileSelect_FileOk);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 225);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(287, 13);
            this.label1.TabIndex = 50;
            this.label1.Text = "Key converted to uint32_t array (copy into bootloader code)";
            // 
            // textBoxConvertedArray
            // 
            this.textBoxConvertedArray.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxConvertedArray.Location = new System.Drawing.Point(12, 241);
            this.textBoxConvertedArray.MaxLength = 16;
            this.textBoxConvertedArray.Name = "textBoxConvertedArray";
            this.textBoxConvertedArray.ReadOnly = true;
            this.textBoxConvertedArray.Size = new System.Drawing.Size(416, 20);
            this.textBoxConvertedArray.TabIndex = 49;
            this.textBoxConvertedArray.TextChanged += new System.EventHandler(this.textBoxConvertedArray_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 327);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxConvertedArray);
            this.Controls.Add(this.labelOutputHexfile);
            this.Controls.Add(this.buttonOutputHexfileSelect);
            this.Controls.Add(this.comboBoxOutputHexfileName);
            this.Controls.Add(this.labelXtaeKey);
            this.Controls.Add(this.textBoxXtaeKey);
            this.Controls.Add(this.labelProgressBar);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.labelProgName);
            this.Controls.Add(this.progressBarHexEncrypt);
            this.Controls.Add(this.buttonEncryptHexfile);
            this.Controls.Add(this.labelInputHexfile);
            this.Controls.Add(this.buttonInputHexfileSelect);
            this.Controls.Add(this.comboBoxInputHexfileName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "chip45boot3 Hexfile Encrypt";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxInputHexfileName;
        private System.Windows.Forms.Button buttonInputHexfileSelect;
        private System.Windows.Forms.Label labelInputHexfile;
        private System.Windows.Forms.OpenFileDialog openFileDialogInputHexfileSelect;
        private System.Windows.Forms.Button buttonEncryptHexfile;
        public System.Windows.Forms.ProgressBar progressBarHexEncrypt;
        private System.Windows.Forms.Label labelProgName;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelProgressBar;
        private System.Windows.Forms.Label labelXtaeKey;
        public System.Windows.Forms.TextBox textBoxXtaeKey;
        private System.Windows.Forms.Label labelOutputHexfile;
        private System.Windows.Forms.Button buttonOutputHexfileSelect;
        private System.Windows.Forms.ComboBox comboBoxOutputHexfileName;
        private System.Windows.Forms.OpenFileDialog openFileDialogOutputHexfileSelect;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox textBoxConvertedArray;
    }
}

