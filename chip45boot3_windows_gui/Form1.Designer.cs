namespace chip45boot3GUI
{
    partial class GuiForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GuiForm));
            this.comboBoxFlashHexfileName = new System.Windows.Forms.ComboBox();
            this.buttonFlashHexfileSelect = new System.Windows.Forms.Button();
            this.labelHexfile = new System.Windows.Forms.Label();
            this.openFileDialogWriteFlashHexfileSelect = new System.Windows.Forms.OpenFileDialog();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.buttonBootloaderStart = new System.Windows.Forms.Button();
            this.comboBoxSelectComPort = new System.Windows.Forms.ComboBox();
            this.labelComPortSelect = new System.Windows.Forms.Label();
            this.buttonClipboard = new System.Windows.Forms.Button();
            this.buttonClearLog = new System.Windows.Forms.Button();
            this.buttonFlashProgram = new System.Windows.Forms.Button();
            this.progressBarFlashProgram = new System.Windows.Forms.ProgressBar();
            this.labelStatusLog = new System.Windows.Forms.Label();
            this.comboBoxSelectBaudRate = new System.Windows.Forms.ComboBox();
            this.labelSelectBaudRate = new System.Windows.Forms.Label();
            this.checkBoxExpertMode = new System.Windows.Forms.CheckBox();
            this.labelProgName = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelProgressBar = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBarEepromProgram = new System.Windows.Forms.ProgressBar();
            this.buttonEepromProgram = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonEepromHexfileSelect = new System.Windows.Forms.Button();
            this.comboBoxEepromHexfileName = new System.Windows.Forms.ComboBox();
            this.buttonFlashRead = new System.Windows.Forms.Button();
            this.buttonEepromRead = new System.Windows.Forms.Button();
            this.textBoxFirmwareHexfile = new System.Windows.Forms.TextBox();
            this.labelFirmwareHexfile = new System.Windows.Forms.Label();
            this.labelFirmwareTarget = new System.Windows.Forms.Label();
            this.textBoxFirmwareTarget = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxBootloaderVersion = new System.Windows.Forms.TextBox();
            this.openFileDialogReadFlashHexfileSelect = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialogWriteEepromHexfileSelect = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialogReadEepromHexfileSelect = new System.Windows.Forms.OpenFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.checkBoxRs485 = new System.Windows.Forms.CheckBox();
            this.comboBoxBusAddress = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxFlashHexfileName
            // 
            this.comboBoxFlashHexfileName.DropDownWidth = 300;
            this.comboBoxFlashHexfileName.FormattingEnabled = true;
            this.comboBoxFlashHexfileName.Location = new System.Drawing.Point(12, 138);
            this.comboBoxFlashHexfileName.Name = "comboBoxFlashHexfileName";
            this.comboBoxFlashHexfileName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.comboBoxFlashHexfileName.Size = new System.Drawing.Size(184, 21);
            this.comboBoxFlashHexfileName.TabIndex = 1;
            this.comboBoxFlashHexfileName.SelectedIndexChanged += new System.EventHandler(this.comboBoxWriteFlashHexfileName_SelectedIndexChanged);
            // 
            // buttonFlashHexfileSelect
            // 
            this.buttonFlashHexfileSelect.Location = new System.Drawing.Point(219, 135);
            this.buttonFlashHexfileSelect.Name = "buttonFlashHexfileSelect";
            this.buttonFlashHexfileSelect.Size = new System.Drawing.Size(120, 25);
            this.buttonFlashHexfileSelect.TabIndex = 2;
            this.buttonFlashHexfileSelect.Text = "Select Flash Hexfile";
            this.buttonFlashHexfileSelect.UseVisualStyleBackColor = true;
            this.buttonFlashHexfileSelect.Click += new System.EventHandler(this.buttonWriteFlashHexfileSelect_Click);
            // 
            // labelHexfile
            // 
            this.labelHexfile.AutoSize = true;
            this.labelHexfile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHexfile.Location = new System.Drawing.Point(9, 122);
            this.labelHexfile.Name = "labelHexfile";
            this.labelHexfile.Size = new System.Drawing.Size(67, 13);
            this.labelHexfile.TabIndex = 2;
            this.labelHexfile.Text = "Flash Hexfile";
            // 
            // openFileDialogWriteFlashHexfileSelect
            // 
            this.openFileDialogWriteFlashHexfileSelect.FileName = "*.hex";
            this.openFileDialogWriteFlashHexfileSelect.Filter = "Intel Hex|*.hex|All Files|*.*";
            this.openFileDialogWriteFlashHexfileSelect.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialogWriteFlashHexfileSelect_FileOk);
            // 
            // textBoxLog
            // 
            this.textBoxLog.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxLog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxLog.Location = new System.Drawing.Point(8, 272);
            this.textBoxLog.MaxLength = 1048072;
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(690, 200);
            this.textBoxLog.TabIndex = 10;
            // 
            // buttonBootloaderStart
            // 
            this.buttonBootloaderStart.Location = new System.Drawing.Point(219, 85);
            this.buttonBootloaderStart.Name = "buttonBootloaderStart";
            this.buttonBootloaderStart.Size = new System.Drawing.Size(120, 25);
            this.buttonBootloaderStart.TabIndex = 6;
            this.buttonBootloaderStart.Text = "Connect Bootloader";
            this.buttonBootloaderStart.UseVisualStyleBackColor = true;
            this.buttonBootloaderStart.Click += new System.EventHandler(this.buttonBootloaderStart_Click);
            // 
            // comboBoxSelectComPort
            // 
            this.comboBoxSelectComPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSelectComPort.FormattingEnabled = true;
            this.comboBoxSelectComPort.Location = new System.Drawing.Point(12, 88);
            this.comboBoxSelectComPort.Name = "comboBoxSelectComPort";
            this.comboBoxSelectComPort.Size = new System.Drawing.Size(80, 21);
            this.comboBoxSelectComPort.TabIndex = 3;
            this.comboBoxSelectComPort.SelectedIndexChanged += new System.EventHandler(this.comboBoxSelectComPort_SelectedIndexChanged);
            // 
            // labelComPortSelect
            // 
            this.labelComPortSelect.AutoSize = true;
            this.labelComPortSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelComPortSelect.Location = new System.Drawing.Point(9, 72);
            this.labelComPortSelect.Name = "labelComPortSelect";
            this.labelComPortSelect.Size = new System.Drawing.Size(86, 13);
            this.labelComPortSelect.TabIndex = 10;
            this.labelComPortSelect.Text = "Select COM Port";
            // 
            // buttonClipboard
            // 
            this.buttonClipboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClipboard.Location = new System.Drawing.Point(593, 478);
            this.buttonClipboard.Name = "buttonClipboard";
            this.buttonClipboard.Size = new System.Drawing.Size(105, 20);
            this.buttonClipboard.TabIndex = 9;
            this.buttonClipboard.Text = "Copy to Clipboard";
            this.buttonClipboard.UseVisualStyleBackColor = true;
            this.buttonClipboard.Click += new System.EventHandler(this.buttonClipboard_Click);
            // 
            // buttonClearLog
            // 
            this.buttonClearLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.buttonClearLog.Location = new System.Drawing.Point(522, 478);
            this.buttonClearLog.Name = "buttonClearLog";
            this.buttonClearLog.Size = new System.Drawing.Size(65, 20);
            this.buttonClearLog.TabIndex = 8;
            this.buttonClearLog.Text = "Clear Log";
            this.buttonClearLog.UseVisualStyleBackColor = true;
            this.buttonClearLog.Click += new System.EventHandler(this.buttonClearLog_Click);
            // 
            // buttonFlashProgram
            // 
            this.buttonFlashProgram.Location = new System.Drawing.Point(219, 185);
            this.buttonFlashProgram.Name = "buttonFlashProgram";
            this.buttonFlashProgram.Size = new System.Drawing.Size(120, 25);
            this.buttonFlashProgram.TabIndex = 7;
            this.buttonFlashProgram.Text = "Program Flash";
            this.buttonFlashProgram.UseVisualStyleBackColor = true;
            this.buttonFlashProgram.Click += new System.EventHandler(this.buttonFlashProgram_Click);
            // 
            // progressBarFlashProgram
            // 
            this.progressBarFlashProgram.Location = new System.Drawing.Point(12, 185);
            this.progressBarFlashProgram.Maximum = 1000000;
            this.progressBarFlashProgram.Name = "progressBarFlashProgram";
            this.progressBarFlashProgram.Size = new System.Drawing.Size(184, 25);
            this.progressBarFlashProgram.TabIndex = 24;
            // 
            // labelStatusLog
            // 
            this.labelStatusLog.AutoSize = true;
            this.labelStatusLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatusLog.Location = new System.Drawing.Point(12, 256);
            this.labelStatusLog.Name = "labelStatusLog";
            this.labelStatusLog.Size = new System.Drawing.Size(93, 13);
            this.labelStatusLog.TabIndex = 25;
            this.labelStatusLog.Text = "Status Log Output";
            // 
            // comboBoxSelectBaudRate
            // 
            this.comboBoxSelectBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSelectBaudRate.FormattingEnabled = true;
            this.comboBoxSelectBaudRate.Location = new System.Drawing.Point(113, 88);
            this.comboBoxSelectBaudRate.Name = "comboBoxSelectBaudRate";
            this.comboBoxSelectBaudRate.Size = new System.Drawing.Size(83, 21);
            this.comboBoxSelectBaudRate.TabIndex = 26;
            this.comboBoxSelectBaudRate.SelectedIndexChanged += new System.EventHandler(this.comboBoxSelectBaudRate_SelectedIndexChanged);
            // 
            // labelSelectBaudRate
            // 
            this.labelSelectBaudRate.AutoSize = true;
            this.labelSelectBaudRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSelectBaudRate.Location = new System.Drawing.Point(110, 72);
            this.labelSelectBaudRate.Name = "labelSelectBaudRate";
            this.labelSelectBaudRate.Size = new System.Drawing.Size(91, 13);
            this.labelSelectBaudRate.TabIndex = 27;
            this.labelSelectBaudRate.Text = "Select Baud Rate";
            // 
            // checkBoxExpertMode
            // 
            this.checkBoxExpertMode.AutoSize = true;
            this.checkBoxExpertMode.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxExpertMode.Location = new System.Drawing.Point(253, 11);
            this.checkBoxExpertMode.Name = "checkBoxExpertMode";
            this.checkBoxExpertMode.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxExpertMode.Size = new System.Drawing.Size(86, 17);
            this.checkBoxExpertMode.TabIndex = 28;
            this.checkBoxExpertMode.Text = "Expert Mode";
            this.checkBoxExpertMode.UseVisualStyleBackColor = true;
            this.checkBoxExpertMode.CheckedChanged += new System.EventHandler(this.checkBoxExpertMode_CheckedChanged);
            // 
            // labelProgName
            // 
            this.labelProgName.AutoSize = true;
            this.labelProgName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProgName.Location = new System.Drawing.Point(7, 13);
            this.labelProgName.Name = "labelProgName";
            this.labelProgName.Size = new System.Drawing.Size(187, 25);
            this.labelProgName.TabIndex = 29;
            this.labelProgName.Text = "chip45boot3 GUI";
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(9, 38);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(60, 13);
            this.labelVersion.TabIndex = 30;
            this.labelVersion.Text = "Version 1.4";
            this.labelVersion.Click += new System.EventHandler(this.labelVersion_Click);
            // 
            // labelProgressBar
            // 
            this.labelProgressBar.AutoSize = true;
            this.labelProgressBar.Location = new System.Drawing.Point(9, 169);
            this.labelProgressBar.Name = "labelProgressBar";
            this.labelProgressBar.Size = new System.Drawing.Size(48, 13);
            this.labelProgressBar.TabIndex = 31;
            this.labelProgressBar.Text = "Progress";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(372, 169);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 37;
            this.label1.Text = "Progress";
            // 
            // progressBarEepromProgram
            // 
            this.progressBarEepromProgram.Location = new System.Drawing.Point(375, 185);
            this.progressBarEepromProgram.Maximum = 1000000;
            this.progressBarEepromProgram.Name = "progressBarEepromProgram";
            this.progressBarEepromProgram.Size = new System.Drawing.Size(184, 25);
            this.progressBarEepromProgram.TabIndex = 36;
            // 
            // buttonEepromProgram
            // 
            this.buttonEepromProgram.Location = new System.Drawing.Point(582, 185);
            this.buttonEepromProgram.Name = "buttonEepromProgram";
            this.buttonEepromProgram.Size = new System.Drawing.Size(120, 25);
            this.buttonEepromProgram.TabIndex = 35;
            this.buttonEepromProgram.Text = "Program Eeprom";
            this.buttonEepromProgram.UseVisualStyleBackColor = true;
            this.buttonEepromProgram.Click += new System.EventHandler(this.buttonEepromProgram_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(372, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "Eeprom Hexfile";
            // 
            // buttonEepromHexfileSelect
            // 
            this.buttonEepromHexfileSelect.Location = new System.Drawing.Point(582, 135);
            this.buttonEepromHexfileSelect.Name = "buttonEepromHexfileSelect";
            this.buttonEepromHexfileSelect.Size = new System.Drawing.Size(120, 25);
            this.buttonEepromHexfileSelect.TabIndex = 34;
            this.buttonEepromHexfileSelect.Text = "Select Eeprom Hexfile";
            this.buttonEepromHexfileSelect.UseVisualStyleBackColor = true;
            this.buttonEepromHexfileSelect.Click += new System.EventHandler(this.buttonWriteEepromHexfileSelect_Click);
            // 
            // comboBoxEepromHexfileName
            // 
            this.comboBoxEepromHexfileName.DropDownWidth = 300;
            this.comboBoxEepromHexfileName.FormattingEnabled = true;
            this.comboBoxEepromHexfileName.Location = new System.Drawing.Point(375, 138);
            this.comboBoxEepromHexfileName.Name = "comboBoxEepromHexfileName";
            this.comboBoxEepromHexfileName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.comboBoxEepromHexfileName.Size = new System.Drawing.Size(184, 21);
            this.comboBoxEepromHexfileName.TabIndex = 32;
            this.comboBoxEepromHexfileName.SelectedIndexChanged += new System.EventHandler(this.comboBoxWriteEepromHexfileName_SelectedIndexChanged);
            // 
            // buttonFlashRead
            // 
            this.buttonFlashRead.Location = new System.Drawing.Point(219, 226);
            this.buttonFlashRead.Name = "buttonFlashRead";
            this.buttonFlashRead.Size = new System.Drawing.Size(120, 25);
            this.buttonFlashRead.TabIndex = 38;
            this.buttonFlashRead.Text = "Read Flash";
            this.buttonFlashRead.UseVisualStyleBackColor = true;
            this.buttonFlashRead.Click += new System.EventHandler(this.buttonFlashRead_Click);
            // 
            // buttonEepromRead
            // 
            this.buttonEepromRead.Location = new System.Drawing.Point(582, 226);
            this.buttonEepromRead.Name = "buttonEepromRead";
            this.buttonEepromRead.Size = new System.Drawing.Size(120, 25);
            this.buttonEepromRead.TabIndex = 39;
            this.buttonEepromRead.Text = "Read Eeprom";
            this.buttonEepromRead.UseVisualStyleBackColor = true;
            this.buttonEepromRead.Click += new System.EventHandler(this.buttonEepromRead_Click);
            // 
            // textBoxFirmwareHexfile
            // 
            this.textBoxFirmwareHexfile.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxFirmwareHexfile.Location = new System.Drawing.Point(658, 61);
            this.textBoxFirmwareHexfile.Name = "textBoxFirmwareHexfile";
            this.textBoxFirmwareHexfile.ReadOnly = true;
            this.textBoxFirmwareHexfile.Size = new System.Drawing.Size(40, 20);
            this.textBoxFirmwareHexfile.TabIndex = 40;
            // 
            // labelFirmwareHexfile
            // 
            this.labelFirmwareHexfile.AutoSize = true;
            this.labelFirmwareHexfile.Location = new System.Drawing.Point(548, 64);
            this.labelFirmwareHexfile.Name = "labelFirmwareHexfile";
            this.labelFirmwareHexfile.Size = new System.Drawing.Size(108, 13);
            this.labelFirmwareHexfile.TabIndex = 41;
            this.labelFirmwareHexfile.Text = "Flash Hexfile Version:";
            // 
            // labelFirmwareTarget
            // 
            this.labelFirmwareTarget.AutoSize = true;
            this.labelFirmwareTarget.Location = new System.Drawing.Point(532, 38);
            this.labelFirmwareTarget.Name = "labelFirmwareTarget";
            this.labelFirmwareTarget.Size = new System.Drawing.Size(124, 13);
            this.labelFirmwareTarget.TabIndex = 43;
            this.labelFirmwareTarget.Text = "Target Firmware Version:";
            // 
            // textBoxFirmwareTarget
            // 
            this.textBoxFirmwareTarget.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxFirmwareTarget.Location = new System.Drawing.Point(658, 35);
            this.textBoxFirmwareTarget.Name = "textBoxFirmwareTarget";
            this.textBoxFirmwareTarget.ReadOnly = true;
            this.textBoxFirmwareTarget.Size = new System.Drawing.Size(40, 20);
            this.textBoxFirmwareTarget.TabIndex = 42;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(557, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 45;
            this.label3.Text = "Bootloader Version:";
            // 
            // textBoxBootloaderVersion
            // 
            this.textBoxBootloaderVersion.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxBootloaderVersion.Location = new System.Drawing.Point(658, 9);
            this.textBoxBootloaderVersion.Name = "textBoxBootloaderVersion";
            this.textBoxBootloaderVersion.ReadOnly = true;
            this.textBoxBootloaderVersion.Size = new System.Drawing.Size(40, 20);
            this.textBoxBootloaderVersion.TabIndex = 44;
            // 
            // openFileDialogReadFlashHexfileSelect
            // 
            this.openFileDialogReadFlashHexfileSelect.CheckFileExists = false;
            this.openFileDialogReadFlashHexfileSelect.FileName = "*.hex";
            this.openFileDialogReadFlashHexfileSelect.Filter = "Intel Hex|*.hex|All Files|*.*";
            this.openFileDialogReadFlashHexfileSelect.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialogReadFlashHexfileSelect_FileOk);
            // 
            // openFileDialogWriteEepromHexfileSelect
            // 
            this.openFileDialogWriteEepromHexfileSelect.FileName = "*.eep";
            this.openFileDialogWriteEepromHexfileSelect.Filter = "Eeprom|*.eep|Intel Hex|*.hex|All Files|*.*";
            this.openFileDialogWriteEepromHexfileSelect.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialogWriteEepromHexfileSelect_FileOk);
            // 
            // openFileDialogReadEepromHexfileSelect
            // 
            this.openFileDialogReadEepromHexfileSelect.CheckFileExists = false;
            this.openFileDialogReadEepromHexfileSelect.FileName = "*.eep";
            this.openFileDialogReadEepromHexfileSelect.Filter = "Eeprom|*.eep|Intel Hex|*.hex|All Files|*.*";
            this.openFileDialogReadEepromHexfileSelect.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialogReadEepromHexfileSelect_FileOk);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // checkBoxRs485
            // 
            this.checkBoxRs485.AutoSize = true;
            this.checkBoxRs485.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxRs485.Location = new System.Drawing.Point(375, 11);
            this.checkBoxRs485.Name = "checkBoxRs485";
            this.checkBoxRs485.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.checkBoxRs485.Size = new System.Drawing.Size(117, 17);
            this.checkBoxRs485.TabIndex = 46;
            this.checkBoxRs485.Text = "RS485 Half-Duplex";
            this.checkBoxRs485.UseVisualStyleBackColor = true;
            this.checkBoxRs485.CheckedChanged += new System.EventHandler(this.checkBoxRs485_CheckedChanged);
            // 
            // comboBoxBusAddress
            // 
            this.comboBoxBusAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBusAddress.DropDownWidth = 300;
            this.comboBoxBusAddress.FormattingEnabled = true;
            this.comboBoxBusAddress.Location = new System.Drawing.Point(375, 54);
            this.comboBoxBusAddress.MaxDropDownItems = 100;
            this.comboBoxBusAddress.Name = "comboBoxBusAddress";
            this.comboBoxBusAddress.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.comboBoxBusAddress.Size = new System.Drawing.Size(120, 21);
            this.comboBoxBusAddress.TabIndex = 47;
            this.comboBoxBusAddress.SelectedIndexChanged += new System.EventHandler(this.comboBoxBusAddress_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(372, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 13);
            this.label4.TabIndex = 48;
            this.label4.Text = "RS485 Bus Address";
            // 
            // GuiForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 503);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBoxBusAddress);
            this.Controls.Add(this.checkBoxRs485);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxBootloaderVersion);
            this.Controls.Add(this.labelFirmwareTarget);
            this.Controls.Add(this.textBoxFirmwareTarget);
            this.Controls.Add(this.labelFirmwareHexfile);
            this.Controls.Add(this.textBoxFirmwareHexfile);
            this.Controls.Add(this.buttonEepromRead);
            this.Controls.Add(this.buttonFlashRead);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBarEepromProgram);
            this.Controls.Add(this.buttonEepromProgram);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonEepromHexfileSelect);
            this.Controls.Add(this.comboBoxEepromHexfileName);
            this.Controls.Add(this.labelProgressBar);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.labelProgName);
            this.Controls.Add(this.checkBoxExpertMode);
            this.Controls.Add(this.labelSelectBaudRate);
            this.Controls.Add(this.comboBoxSelectBaudRate);
            this.Controls.Add(this.labelStatusLog);
            this.Controls.Add(this.progressBarFlashProgram);
            this.Controls.Add(this.buttonFlashProgram);
            this.Controls.Add(this.buttonClearLog);
            this.Controls.Add(this.buttonClipboard);
            this.Controls.Add(this.labelComPortSelect);
            this.Controls.Add(this.comboBoxSelectComPort);
            this.Controls.Add(this.buttonBootloaderStart);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.labelHexfile);
            this.Controls.Add(this.buttonFlashHexfileSelect);
            this.Controls.Add(this.comboBoxFlashHexfileName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "GuiForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "chip45boot3 GUI";
            this.Load += new System.EventHandler(this.GuiForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxFlashHexfileName;
        private System.Windows.Forms.Button buttonFlashHexfileSelect;
        private System.Windows.Forms.Label labelHexfile;
        private System.Windows.Forms.OpenFileDialog openFileDialogWriteFlashHexfileSelect;
        private System.Windows.Forms.Button buttonBootloaderStart;
        private System.Windows.Forms.ComboBox comboBoxSelectComPort;
        private System.Windows.Forms.Label labelComPortSelect;
        private System.Windows.Forms.Button buttonClipboard;
        private System.Windows.Forms.Button buttonClearLog;
        private System.Windows.Forms.Button buttonFlashProgram;
        private System.Windows.Forms.Label labelStatusLog;
        public System.Windows.Forms.ProgressBar progressBarFlashProgram;
        public System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.ComboBox comboBoxSelectBaudRate;
        private System.Windows.Forms.Label labelSelectBaudRate;
        private System.Windows.Forms.CheckBox checkBoxExpertMode;
        private System.Windows.Forms.Label labelProgName;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelProgressBar;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ProgressBar progressBarEepromProgram;
        private System.Windows.Forms.Button buttonEepromProgram;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonEepromHexfileSelect;
        private System.Windows.Forms.ComboBox comboBoxEepromHexfileName;
        private System.Windows.Forms.Button buttonFlashRead;
        private System.Windows.Forms.Button buttonEepromRead;
        private System.Windows.Forms.Label labelFirmwareHexfile;
        private System.Windows.Forms.Label labelFirmwareTarget;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox textBoxFirmwareHexfile;
        public System.Windows.Forms.TextBox textBoxFirmwareTarget;
        public System.Windows.Forms.TextBox textBoxBootloaderVersion;
        private System.Windows.Forms.OpenFileDialog openFileDialogReadFlashHexfileSelect;
        private System.Windows.Forms.OpenFileDialog openFileDialogWriteEepromHexfileSelect;
        private System.Windows.Forms.OpenFileDialog openFileDialogReadEepromHexfileSelect;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox checkBoxRs485;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.ComboBox comboBoxBusAddress;
    }
}

