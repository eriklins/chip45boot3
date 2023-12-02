/****************************************************************************
 * LPU Bootloader GUI
 * ------------------
 * test application for LPU bootloader implementation
 * 
 * copyright 2011, Dr. Erik Lins, chip45 GmbH u. Co. KG
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


namespace hexfileEncrypt
{
    public partial class Form1 : Form
    {
        // some private variables
        private HexFile myHexFile = new HexFile();
        private XtaeTools myXtaeTools = new XtaeTools();


        // start here, do some initialization stuff
        public Form1()
        {
            InitializeComponent();

            buttonEncryptHexfile.Enabled = false;

        }


        //////////////////////////////////////////////////////////////////////
        // loading form
        private void Form1_Load(object sender, EventArgs e)
        {
        }




        /////////////////////////////////////////////////////////////////////
        // controls for hexfile encryption
        /////////////////////////////////////////////////////////////////////

        private void textBoxXtaeKey_TextChanged(object sender, EventArgs e)
        {
            if (textBoxXtaeKey.Text.Length != 16)
            {
                textBoxConvertedArray.Text = "";

            }
            else
            {
                // get text from control
                string s = textBoxXtaeKey.Text;

                // convert the string to 4 UInt32's
                myXtaeTools.ConvertStringToUInt32(s);

                textBoxConvertedArray.Text = String.Format("ENCRYPTION_KEY={{{0},{1},{2},{3}}}",
                    myXtaeTools.xtaeKey[0], myXtaeTools.xtaeKey[1], myXtaeTools.xtaeKey[2], myXtaeTools.xtaeKey[3]);

            }
        }


        private void buttonEncryptHexfile_Click(object sender, EventArgs e)
        {
            // make the encrypt button green to indicate success
            buttonEncryptHexfile.BackColor = Color.FromName("Transparent");
            // reset progress bar
            progressBarHexEncrypt.Value = 0; 
            
            
            string s = textBoxXtaeKey.Text;

            // check if length of key is 32 characters
            if (s.Length != 16)
            {
                DialogResult result = MessageBox.Show("Please check XTAE key.", "XTAE Key Error!", MessageBoxButtons.OK);
            }
            else
            {
                // convert the string to 4 UInt32's
                myXtaeTools.ConvertStringToUInt32(s);

                // encrypt the buffer
                myXtaeTools.EncryptBuffer(myHexFile.buffer, myHexFile.bufferLength);

                // write the buffer as hex file
                myHexFile.WriteFile(myXtaeTools.buffer, myXtaeTools.bufferLength, comboBoxOutputHexfileName.Text);

                // make the encrypt button green to indicate success
                buttonEncryptHexfile.BackColor = Color.FromName("LightGreen");
            }

        }



        /////////////////////////////////////////////////////////////////////
        // controls for the input hexfile
        /////////////////////////////////////////////////////////////////////

        private void comboBoxInputHexfileName_SelectedIndexChanged(object sender, EventArgs e)
        {

            // get the path from the currently selected item in the drop down list
            string path = comboBoxInputHexfileName.GetItemText(comboBoxInputHexfileName.SelectedItem);

            int i = myHexFile.ReadFile(path);

            if (i < 0)
            {  // we had some error
                DialogResult result = MessageBox.Show("Please check hexfile.", "Error reading Hexfile!", MessageBoxButtons.OK);
            }

            // make the encrypt button green to indicate success
            buttonEncryptHexfile.BackColor = Color.FromName("Transparent");
            // reset progress bar
            progressBarHexEncrypt.Value = 0;

        }

        private void openFileDialogInputHexfileSelect_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = "";

            fileName = openFileDialogInputHexfileSelect.FileName;  // read selected filename from dialog box
            comboBoxInputHexfileName.Items.Remove(fileName);  // remove the (possibly) existing items
            comboBoxInputHexfileName.Items.Add(fileName);  // add the item
            comboBoxInputHexfileName.Text = fileName;  // and show the item as selected item

            // make the encrypt button green to indicate success
            buttonEncryptHexfile.BackColor = Color.FromName("Transparent");
            // reset progress bar
            progressBarHexEncrypt.Value = 0;

        }

        private void buttonInputHexfileSelect_Click(object sender, EventArgs e)
        {
            // we open a file select dialog
            openFileDialogInputHexfileSelect.ShowDialog();

            // check if both hexfile names are valid, if so we enable the encrypt buttom
            if ((comboBoxInputHexfileName.Text.Length != 0) && (comboBoxOutputHexfileName.Text.Length != 0))
            {
                buttonEncryptHexfile.Enabled = true;
            }

            // make the encrypt button green to indicate success
            buttonEncryptHexfile.BackColor = Color.FromName("Transparent");
            // reset progress bar
            progressBarHexEncrypt.Value = 0;
        }



        /////////////////////////////////////////////////////////////////////
        // controls for the output hexfile
        /////////////////////////////////////////////////////////////////////

        private void comboBoxOutputHexfileName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get the path from the currently selected item in the drop down list
            string path = comboBoxOutputHexfileName.GetItemText(comboBoxOutputHexfileName.SelectedItem);

            // make the encrypt button green to indicate success
            buttonEncryptHexfile.BackColor = Color.FromName("Transparent");
            // reset progress bar
            progressBarHexEncrypt.Value = 0;

        }

        private void buttonOutputHexfileSelect_Click(object sender, EventArgs e)
        {
            // we open a file select dialog
            openFileDialogOutputHexfileSelect.ShowDialog();

            // check if both hexfile names are valid, if so we enable the encrypt buttom
            if ((comboBoxInputHexfileName.Text.Length != 0) && (comboBoxOutputHexfileName.Text.Length != 0))
            {
                buttonEncryptHexfile.Enabled = true;
            }

            // make the encrypt button green to indicate success
            buttonEncryptHexfile.BackColor = Color.FromName("Transparent");
            // reset progress bar
            progressBarHexEncrypt.Value = 0;
        }


        private void openFileDialogOutputHexfileSelect_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = "";

            fileName = openFileDialogOutputHexfileSelect.FileName;  // read selected filename from dialog box
            comboBoxOutputHexfileName.Items.Remove(fileName);  // remove the (possibly) existing items
            comboBoxOutputHexfileName.Items.Add(fileName);  // add the item
            comboBoxOutputHexfileName.Text = fileName;  // and show the item as selected item

            // make the encrypt button green to indicate success
            buttonEncryptHexfile.BackColor = Color.FromName("Transparent");
            // reset progress bar
            progressBarHexEncrypt.Value = 0;
        }

        private void openFileDialogOutputHexfileSelect_FileNotOk(object sender, CancelEventArgs e)
        {
            string fileName = "";

            fileName = openFileDialogOutputHexfileSelect.FileName;  // read selected filename from dialog box
            comboBoxOutputHexfileName.Items.Remove(fileName);  // remove the (possibly) existing items
            comboBoxOutputHexfileName.Items.Add(fileName);  // add the item
            comboBoxOutputHexfileName.Text = fileName;  // and show the item as selected item

            // make the encrypt button green to indicate success
            buttonEncryptHexfile.BackColor = Color.FromName("Transparent");
            // reset progress bar
            progressBarHexEncrypt.Value = 0;

        }

        private void textBoxConvertedArray_TextChanged(object sender, EventArgs e)
        {

        }

        private void labelVersion_Click(object sender, EventArgs e)
        {

        }


    }
}
