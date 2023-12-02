/****************************************************************************
 * chip45boot3 bootloader GUI
 * --------------------------
 * a pc application for communicating with the chip45boot3 bootloader
 * 
 * copyright 2013, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * http://www.chip45.com, info@chip45.com
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Threading;
using HexFileTools;
using BootloaderTools;
using chip45boot3GUI;


namespace chip45boot3GUI
{
    public partial class GuiForm : Form
    {

        // a public reference to the form for accessing controls from other classes
        public static GuiForm myForm;

        
        // some private variables
        private HexFile myFlashWriteHexFile = new HexFile();
        private HexFile myEepromWriteHexFile = new HexFile();
        private HexFile myFlashReadHexFile = new HexFile();
        private string flashReadHexFileName;
        private HexFile myEepromReadHexFile = new HexFile();
        private string eepromReadHexFileName;
        private Bootloader myBootloader = new Bootloader();
        private int numberOfComPorts = 0;


        ////////////////////////////////////////////////////////////////////
        //
        // the class constructor with some initialization code
        //
        ////////////////////////////////////////////////////////////////////
        public GuiForm()
        {
            InitializeComponent();

            // get the available com ports
            updateComPortList();

            // update the baud rate list
            initFormControls();

            // we keep a reference to the GuiForm for accessing progress bar, textBoxLog etc. from BootloaderTools.cs
            myForm = this;

        }
        

        ////////////////////////////////////////////////////////////////////
        //
        // loading the form
        //
        ////////////////////////////////////////////////////////////////////
        private void GuiForm_Load(object sender, EventArgs e)
        {
            this.Width = 360;
            this.Height = 255;
        }


        ////////////////////////////////////////////////////////////////////
        //
        // 1msec timer (used for live updating of COM port list)
        //
        ////////////////////////////////////////////////////////////////////
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            updateComPortList();
        }


        ////////////////////////////////////////////////////////////////////
        //
        // initialize the form controls content
        //
        ////////////////////////////////////////////////////////////////////
        private void initFormControls ()
        {

            // preset the baud rate list
            List<String> tList = new List<String>();
            comboBoxSelectBaudRate.Items.Clear();
            tList.Add("9600");
            tList.Add("19200");
            tList.Add("38400");
            tList.Add("57600");
            tList.Add("115200");
            tList.Add("230400");
            tList.Add("921600");
            tList.Add("62500");
            tList.Add("125000");
            tList.Add("250000");
            tList.Add("500000");
            if (tList.Count > 0)
            {
                comboBoxSelectBaudRate.Items.AddRange(tList.ToArray());
                comboBoxSelectBaudRate.SelectedIndex = 4;
            }

            // enable/disable/change form controls, since we may now do things and don't do other things
            comboBoxSelectComPort.Enabled = true;
            comboBoxSelectBaudRate.Enabled = true;
            buttonFlashProgram.Enabled = false;
            buttonFlashRead.Enabled = false;
            buttonEepromProgram.Enabled = false;
            buttonEepromRead.Enabled = false;
            buttonBootloaderStart.Text = "Start Bootloader";
            buttonBootloaderStart.BackColor = Color.FromName("Transparent");
            buttonFlashProgram.BackColor = Color.FromName("Transparent");
            buttonFlashRead.BackColor = Color.FromName("Transparent");
            buttonEepromProgram.BackColor = Color.FromName("Transparent");
            buttonEepromRead.BackColor = Color.FromName("Transparent");

        }


        ////////////////////////////////////////////////////////////////////
        //
        // get the available COM ports and put them into the list box
        //
        ////////////////////////////////////////////////////////////////////
        private void updateComPortList()
        {
            // check if the number of COM ports has changed, if not, no need to touch the list box
            if (SerialPort.GetPortNames().GetLength(0) != numberOfComPorts)
            {   
                // create a list variable
                List<String> comPortList = new List<String>();

                // clear the list box
                comboBoxSelectComPort.Items.Clear();

                // get the available com ports
                foreach (string s in SerialPort.GetPortNames())
                {
                    comPortList.Add(s);
                }

                // sort them
                comPortList.Sort();

                // if there is at least one COM port, we update the list
                if (comPortList.Count > 0)
                {
                    comboBoxSelectComPort.Items.AddRange(comPortList.ToArray());
                    comboBoxSelectComPort.SelectedIndex = 0;
                }
                else
                {
                    // we need to clear the current text to avoid opening of non-existing COM port
                    comboBoxSelectComPort.Text = "";
                }

                // remember the current number of COM ports for next check
                numberOfComPorts = SerialPort.GetPortNames().GetLength(0);
            }
        }



        ////////////////////////////////////////////////////////////////////
        //
        // bootloader connect/start and exit
        //
        ////////////////////////////////////////////////////////////////////
        private void buttonBootloaderStart_Click(object sender, EventArgs e)
        {

            // check if there is at least one COM port
            if (comboBoxSelectComPort.Items.Count == 0)
            {
                // if not, we show a message box to notify user
                DialogResult result = MessageBox.Show("No COM port available! Connect device first.", "Missing COM Port", MessageBoxButtons.OK);
            }
            else
            {
                // get the desired COM port, baud rate and rs485 bus address from the list boxes
                string port = comboBoxSelectComPort.SelectedItem.ToString();
                int baudrate = Convert.ToInt32(comboBoxSelectBaudRate.SelectedItem.ToString());

                // check if we are not already connected
                if (myBootloader.IsConnected() == false)
                {
                    // we clear the log box
                    textBoxLog.Clear();
                    textBoxLog.Refresh();

                    // log output
                    textBoxLog.AppendText(String.Format("\r\n\n==> Connecting bootloader at {0} with {1} baud", port, baudrate));
                    buttonBootloaderStart.Text = "Connecting...";
   
                    // try to connect to bootloader
                    if (myBootloader.Connect(port, baudrate) == true)
                    {
                        // enable/disable/change form controls, since we may now do things and don't do other things
                        comboBoxSelectComPort.Enabled = false;
                        comboBoxSelectBaudRate.Enabled = false;
                        buttonFlashProgram.Enabled = true;
                        buttonFlashRead.Enabled = true;
                        buttonEepromProgram.Enabled = true;
                        buttonEepromRead.Enabled = true;
                        buttonBootloaderStart.Text = "Exit Bootloader";
                        buttonBootloaderStart.BackColor = Color.FromName("LightGreen");

                        // if we are not is RS485 mode, we read bootloader version and firmware version here
                        // if RS485 mode this happens automatically when address select field changes (i.e. also when field is filled with addresses)
                        if (!myBootloader.flagRs485)
                        {
                            // read bootloader version from target
                            if (myBootloader.ReadBootloaderVersion() == true)
                            {
                                textBoxBootloaderVersion.Text = String.Format("\r\n{0}.{1}", myBootloader.versionBootloaderMajor, myBootloader.versionBootloaderMinor);
                            }
                            else
                            {
                                textBoxBootloaderVersion.Text = "n.a.";
                            }

                            // read firmware version from target
                            if (myBootloader.ReadTargetFirmwareVersion() == true)
                            {
                                textBoxFirmwareTarget.Text = String.Format("\r\n{0}.{1}", myBootloader.versionFirmwareTargetMajor, myBootloader.versionFirmwareTargetMinor);
                            }
                            else
                            {
                                textBoxFirmwareTarget.Text = "n.a.";
                            }
                        }
                    }
                    else
                    {
                        // enable/disable/change form controls, since we may now do things and don't do other things
                        comboBoxSelectComPort.Enabled = true;
                        comboBoxSelectBaudRate.Enabled = true;
                        buttonFlashProgram.Enabled = false;
                        buttonFlashRead.Enabled = false;
                        buttonEepromProgram.Enabled = false;
                        buttonEepromRead.Enabled = false;
                        buttonBootloaderStart.Text = "Start Bootloader";
                        buttonBootloaderStart.BackColor = Color.FromName("Transparent");
                        buttonFlashProgram.BackColor = Color.FromName("Transparent");
                        buttonFlashRead.BackColor = Color.FromName("Transparent");
                        buttonEepromProgram.BackColor = Color.FromName("Transparent");
                        buttonEepromRead.BackColor = Color.FromName("Transparent");
                    }
                }
                else  // we are already connected, so we do a disconnect
                {
                    // log output
                    textBoxLog.AppendText("\r\n==> Exiting bootloader");

                    // exit the bootloader
                    myBootloader.Exit();

                    // enable/disable/change form controls, since we may now do things and don't do other things
                    comboBoxSelectComPort.Enabled = true;
                    comboBoxSelectBaudRate.Enabled = true;
                    buttonFlashProgram.Enabled = false;
                    buttonFlashRead.Enabled = false;
                    buttonEepromProgram.Enabled = false;
                    buttonEepromRead.Enabled = false;
                    buttonBootloaderStart.Text = "Start Bootloader";
                    buttonBootloaderStart.BackColor = Color.FromName("Transparent");
                    buttonFlashProgram.BackColor = Color.FromName("Transparent");
                    buttonFlashRead.BackColor = Color.FromName("Transparent");
                    buttonEepromProgram.BackColor = Color.FromName("Transparent");
                    buttonEepromRead.BackColor = Color.FromName("Transparent");
                    textBoxBootloaderVersion.Clear();
                    textBoxFirmwareTarget.Clear();
                    progressBarFlashProgram.Value = 0;
                    progressBarEepromProgram.Value = 0;
                    comboBoxBusAddress.Items.Clear();
                    comboBoxBusAddress.Text = "";
                }
            }
        }



        //////////////////////////////////////////////////////////////////////
        //
        // program flash
        //
        //////////////////////////////////////////////////////////////////////
        private void buttonFlashProgram_Click(object sender, EventArgs e)
        {

            if (myFlashWriteHexFile.bufferLength == 0)
            {
                DialogResult result = MessageBox.Show("Please select a flash hexfile first!", "Hexfile not yet selected!", MessageBoxButtons.OK);
            }
            else
            {
                // enable/disable/change form controls, since we may now do things and don't do other things
                buttonBootloaderStart.Enabled = false;
                buttonEepromHexfileSelect.Enabled = false;
                buttonEepromProgram.Enabled = false;
                buttonEepromRead.Enabled = false;
                buttonFlashHexfileSelect.Enabled = false;
                buttonFlashProgram.Enabled = false;
                buttonFlashRead.Enabled = false;
                buttonFlashProgram.BackColor = Color.FromName("Transparent");
                progressBarFlashProgram.Value = 0;


                bool versionConflict = false;

                if ((myFlashWriteHexFile.versionHexfileMajor != -1) && (myFlashWriteHexFile.versionHexfileMinor != 1))
                {
                    if (myBootloader.versionFirmwareTargetMajor > myFlashWriteHexFile.versionHexfileMajor)
                    {
                        versionConflict = true;
                    }
                    else if (myBootloader.versionFirmwareTargetMinor > myFlashWriteHexFile.versionHexfileMinor)
                    {
                        versionConflict = true;
                    }
                }

                if (versionConflict)
                {
                    DialogResult result = MessageBox.Show("Existing target firmware version seems to be newer than \r\nfirmware version of hexfile!\r\n\r\nDo you really want to program this hexfile?", "Conflicting Versions!", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        versionConflict = false;
                    }
                }

                if (!versionConflict)
                {
                    // send start address for programming
                    if (myBootloader.SetAddress(0x0000))
                    {

                        // program flash
                        if (myBootloader.FlashWrite(myFlashWriteHexFile))
                        {
                            buttonFlashProgram.BackColor = Color.FromName("LightGreen");

                            // read bootloader version from target
                            if (myBootloader.ReadBootloaderVersion() == true)
                            {
                                textBoxBootloaderVersion.Text = String.Format("\r\n{0}.{1}", myBootloader.versionBootloaderMajor, myBootloader.versionBootloaderMinor);
                            }
                            else
                            {
                                textBoxBootloaderVersion.Text = "n.a.";
                            }

                            // read firmware version from target
                            if (myBootloader.ReadTargetFirmwareVersion() == true)
                            {
                                textBoxFirmwareTarget.Text = String.Format("\r\n{0}.{1}", myBootloader.versionFirmwareTargetMajor, myBootloader.versionFirmwareTargetMinor);
                            }
                            else
                            {
                                textBoxFirmwareTarget.Text = "n.a.";
                            }

                        }
                        else
                        {
                            buttonFlashProgram.BackColor = Color.FromName("LightCoral");
                        }
                    }

                }

                // enable/disable/change form controls, since we may now do things and don't do other things
                buttonBootloaderStart.Enabled = true;
                buttonEepromHexfileSelect.Enabled = true;
                buttonEepromProgram.Enabled = true;
                buttonEepromRead.Enabled = true;
                buttonFlashHexfileSelect.Enabled = true;
                buttonFlashProgram.Enabled = true;
                buttonFlashRead.Enabled = true;


            }

        }


        //////////////////////////////////////////////////////////////////////
        //
        // read flash
        //
        //////////////////////////////////////////////////////////////////////
        private void buttonFlashRead_Click(object sender, EventArgs e)
        {
            // we open a file select dialog
            openFileDialogReadFlashHexfileSelect.ShowDialog();

            if (flashReadHexFileName.Length > 0)
            {
                // enable/disable/change form controls, since we may now do things and don't do other things
                buttonBootloaderStart.Enabled = false;
                buttonEepromHexfileSelect.Enabled = false;
                buttonEepromProgram.Enabled = false;
                buttonEepromRead.Enabled = false;
                buttonFlashHexfileSelect.Enabled = false;
                buttonFlashProgram.Enabled = false;
                buttonFlashRead.Enabled = false;
                buttonFlashRead.BackColor = Color.FromName("Transparent");
                progressBarFlashProgram.Value = 0;


                // send start address for programming
                if (myBootloader.SetAddress(0x0000))
                {

                    // program flash
                    if (myBootloader.FlashRead(myFlashReadHexFile))
                    {
                        buttonFlashRead.BackColor = Color.FromName("LightGreen");

                        myFlashReadHexFile.WriteFile(flashReadHexFileName);
                    }
                    else
                    {
                        buttonFlashRead.BackColor = Color.FromName("LightCoral");
                    }
                }

                // reset controls
                buttonBootloaderStart.Enabled = true;
                buttonEepromHexfileSelect.Enabled = true;
                buttonEepromProgram.Enabled = true;
                buttonEepromRead.Enabled = true;
                buttonFlashHexfileSelect.Enabled = true;
                buttonFlashProgram.Enabled = true;
                buttonFlashRead.Enabled = true;

            }

        }


        //////////////////////////////////////////////////////////////////////
        //
        // program eeprom
        //
        //////////////////////////////////////////////////////////////////////
        private void buttonEepromProgram_Click(object sender, EventArgs e)
        {

            if (myEepromWriteHexFile.bufferLength == 0)
            {
                DialogResult result = MessageBox.Show("Please select an eeprom hexfile first!", "Hexfile not yet selected!", MessageBoxButtons.OK);
            }
            else
            {
                // enable/disable/change form controls, since we may now do things and don't do other things
                buttonBootloaderStart.Enabled = false;
                buttonFlashHexfileSelect.Enabled = false;
                buttonFlashProgram.Enabled = false;
                buttonFlashRead.Enabled = false;
                buttonEepromHexfileSelect.Enabled = false;
                buttonEepromProgram.Enabled = false;
                buttonEepromRead.Enabled = false;
                buttonEepromProgram.BackColor = Color.FromName("Transparent");
                progressBarEepromProgram.Value = 0;


                // send start address for programming
                if (myBootloader.SetAddress(0x0000))
                {

                    // program flash
                    if (myBootloader.EepromWrite(myEepromWriteHexFile))
                    {
                        buttonEepromProgram.BackColor = Color.FromName("LightGreen");

                    }
                    else
                    {
                        buttonFlashProgram.BackColor = Color.FromName("LightCoral");
                    }
                }

                // enable/disable/change form controls, since we may now do things and don't do other things
                buttonBootloaderStart.Enabled = true;
                buttonEepromHexfileSelect.Enabled = true;
                buttonEepromProgram.Enabled = true;
                buttonEepromRead.Enabled = true;
                buttonFlashHexfileSelect.Enabled = true;
                buttonFlashProgram.Enabled = true;
                buttonFlashRead.Enabled = true;


            }

        }



        //////////////////////////////////////////////////////////////////////
        //
        // read eeprom
        //
        //////////////////////////////////////////////////////////////////////
        private void buttonEepromRead_Click(object sender, EventArgs e)
        {
            // we open a file select dialog
            openFileDialogReadEepromHexfileSelect.ShowDialog();

            if (eepromReadHexFileName.Length > 0)
            {
                // enable/disable/change form controls, since we may now do things and don't do other things
                buttonBootloaderStart.Enabled = false;
                buttonEepromHexfileSelect.Enabled = false;
                buttonEepromProgram.Enabled = false;
                buttonEepromRead.Enabled = false;
                buttonFlashHexfileSelect.Enabled = false;
                buttonFlashProgram.Enabled = false;
                buttonFlashRead.Enabled = false;
                buttonEepromRead.BackColor = Color.FromName("Transparent");
                progressBarEepromProgram.Value = 0;


                // send start address for programming
                if (myBootloader.SetAddress(0x0000))
                {

                    // program flash
                    if (myBootloader.EepromRead(myEepromReadHexFile))
                    {
                        buttonEepromRead.BackColor = Color.FromName("LightGreen");

                        myEepromReadHexFile.WriteFile(eepromReadHexFileName);
                    }
                    else
                    {
                        buttonEepromRead.BackColor = Color.FromName("LightCoral");
                    }
                }

                // enable/disable/change form controls, since we may now do things and don't do other things
                buttonBootloaderStart.Enabled = true;
                buttonEepromHexfileSelect.Enabled = true;
                buttonEepromProgram.Enabled = true;
                buttonEepromRead.Enabled = true;
                buttonFlashHexfileSelect.Enabled = true;
                buttonFlashProgram.Enabled = true;
                buttonFlashRead.Enabled = true;

            }

        }

        
        
        //////////////////////////////////////////////////////////////////////
        //
        // the file select stuff
        //
        //////////////////////////////////////////////////////////////////////

        private void buttonWriteFlashHexfileSelect_Click(object sender, EventArgs e)
        {
            // we open a file select dialog
            openFileDialogWriteFlashHexfileSelect.ShowDialog();

        }

        private void buttonWriteEepromHexfileSelect_Click(object sender, EventArgs e)
        {
            // we open a file select dialog
            openFileDialogWriteEepromHexfileSelect.ShowDialog();
        }

        private void openFileDialogWriteFlashHexfileSelect_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = openFileDialogWriteFlashHexfileSelect.FileName;  // read selected filename from dialog box
            comboBoxFlashHexfileName.Items.Remove(fileName);  // remove the (possibly) existing items
            comboBoxFlashHexfileName.Items.Add(fileName);  // add the item
            comboBoxFlashHexfileName.Text = fileName;  // and show the item as selected item
        }

        private void openFileDialogWriteEepromHexfileSelect_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = openFileDialogWriteEepromHexfileSelect.FileName;  // read selected filename from dialog box
            comboBoxEepromHexfileName.Items.Remove(fileName);  // remove the (possibly) existing items
            comboBoxEepromHexfileName.Items.Add(fileName);  // add the item
            comboBoxEepromHexfileName.Text = fileName;  // and show the item as selected item
        }

        private void openFileDialogReadFlashHexfileSelect_FileOk(object sender, CancelEventArgs e)
        {
            flashReadHexFileName = openFileDialogReadFlashHexfileSelect.FileName;  // put selected filename in variable
        }

        private void openFileDialogReadEepromHexfileSelect_FileOk(object sender, CancelEventArgs e)
        {
            eepromReadHexFileName = openFileDialogReadEepromHexfileSelect.FileName;  // put selected filename in variable
        }

        private void comboBoxWriteFlashHexfileName_SelectedIndexChanged(object sender, EventArgs e)
        {

            // get the path from the currently selected item in the drop down list
            string path = comboBoxFlashHexfileName.GetItemText(comboBoxFlashHexfileName.SelectedItem);

            // status output
            textBoxLog.AppendText(String.Format("\r\n\n==> Reading Flash Hexfile {0}... ", path));

            int i = myFlashWriteHexFile.ReadFile(path);

            if (i < 0)
            {  // we had some error
                textBoxLog.AppendText(String.Format("fail: {0}", i));
            }
            else
            {  // success
                if (myFlashWriteHexFile.bufferLength == 0)
                {
                    textBoxLog.AppendText("done. Warning: Hexfile is empty!");
                }
                else
                {
                    textBoxLog.AppendText(String.Format("done. Size is {0} bytes.", myFlashWriteHexFile.bufferLength));
                }

                // read hexfile version
                if (myFlashWriteHexFile.ReadHexfileVersion())
                {
                    textBoxFirmwareHexfile.Text = String.Format("\r\n{0}.{1}", myFlashWriteHexFile.versionHexfileMajor, myFlashWriteHexFile.versionHexfileMinor);
                }
                else
                {
                    textBoxFirmwareHexfile.Text = "n.a.";
                }
            }
            progressBarFlashProgram.Value = 0;
            buttonFlashProgram.BackColor = Color.FromName("Transparent");
        }

        private void comboBoxWriteEepromHexfileName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get the path from the currently selected item in the drop down list
            string path = comboBoxEepromHexfileName.GetItemText(comboBoxEepromHexfileName.SelectedItem);

            // status output
            textBoxLog.AppendText(String.Format("\r\n\n==> Reading Eeprom Hexfile {0}... ", path));

            int i = myEepromWriteHexFile.ReadFile(path);

            if (i < 0)
            {  // we had some error
                textBoxLog.AppendText(String.Format("fail: {0}", i));
            }
            else
            {  // success
                if (myEepromWriteHexFile.bufferLength == 0)
                {
                    textBoxLog.AppendText("done. Warning: Hexfile is empty!");
                }
                else
                {
                    textBoxLog.AppendText(String.Format("done. Size is {0} bytes.", myEepromWriteHexFile.bufferLength));
                }


                // read hexfile version
                if (myEepromWriteHexFile.ReadHexfileVersion())
                {
                    textBoxFirmwareHexfile.Text = String.Format("\r\n{0}.{1}", myEepromWriteHexFile.versionHexfileMajor, myEepromWriteHexFile.versionHexfileMinor);
                }
                else
                {
                    textBoxFirmwareHexfile.Text = "n.a.";
                }
            }
            progressBarEepromProgram.Value = 0;
            buttonEepromProgram.BackColor = Color.FromName("Transparent");


        }



        //////////////////////////////////////////////////////////////////////
        //
        // the com port and baud rate select stuff
        //
        //////////////////////////////////////////////////////////////////////

        private void comboBoxSelectBaudRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            // nothing to do here, we access the control's value at other place
        }

        private void comboBoxSelectComPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            // nothing to do here, we access the control's value at other place
        }



        //////////////////////////////////////////////////////////////////////
        //
        // the log window stuff
        //
        //////////////////////////////////////////////////////////////////////
        
        private void buttonClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetData("Text", textBoxLog.Text);
        }

        private void buttonClearLog_Click(object sender, EventArgs e)
        {
            textBoxLog.Clear();
        }



        //////////////////////////////////////////////////////////////////////
        //
        // toggeling between simple and expert mode
        //
        //////////////////////////////////////////////////////////////////////
        private void checkBoxExpertMode_CheckedChanged(object sender, EventArgs e)
        {
            // we simply change the size of the form for expert mode
            if (checkBoxExpertMode.Checked)
            {
                // enlarge the form
                ActiveForm.Width = 720;
                ActiveForm.Height = 535;

                // change color of check box
                //checkBoxExpertMode.ForeColor = Color.FromName("ControlText");

            }
            else
            {
                // shrink the form
                ActiveForm.Width = 360;
                ActiveForm.Height = 255;

                // change color of check box
                //checkBoxExpertMode.ForeColor = Color.FromName("ControlDark");

                // in simple mode disable RS485 in case the user forgot this
                checkBoxRs485.Checked = false;

            }
        }


        //////////////////////////////////////////////////////////////////////
        //
        // RS485 stuff
        //
        //////////////////////////////////////////////////////////////////////

        private void checkBoxRs485_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRs485.Checked)
            {
                myBootloader.flagRs485 = true;
            }
            else
            {
                myBootloader.flagRs485 = false;
            }
        }

        private void comboBoxBusAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            myBootloader.busTargetsActiveAddress = Convert.ToInt32(comboBoxBusAddress.SelectedItem.ToString());
            textBoxLog.AppendText(String.Format("\r\nConnecting Target Address {0}", myBootloader.busTargetsActiveAddress));

            //buttonBootloaderStart.Enabled = false;
            //buttonEepromHexfileSelect.Enabled = false;
            //buttonEepromProgram.Enabled = false;
            //buttonEepromRead.Enabled = false;
            //buttonFlashHexfileSelect.Enabled = false;
            buttonFlashProgram.Enabled = true;
            //buttonFlashRead.Enabled = false;
            buttonFlashProgram.BackColor = Color.FromName("Transparent");
            progressBarFlashProgram.Value = 0;

            // read bootloader version from target
            if (myBootloader.ReadBootloaderVersion() == true)
            {
                textBoxBootloaderVersion.Text = String.Format("\r\n{0}.{1}", myBootloader.versionBootloaderMajor, myBootloader.versionBootloaderMinor);
            }
            else
            {
                textBoxBootloaderVersion.Text = "n.a.";
            }

            // read firmware version from target
            if (myBootloader.ReadTargetFirmwareVersion() == true)
            {
                textBoxFirmwareTarget.Text = String.Format("\r\n{0}.{1}", myBootloader.versionFirmwareTargetMajor, myBootloader.versionFirmwareTargetMinor);
            }
            else
            {
                textBoxFirmwareTarget.Text = "n.a.";
            }

            // nothing to do here, we access the control's value at other place
        }

        private void labelVersion_Click(object sender, EventArgs e)
        {

        }
    }
}
