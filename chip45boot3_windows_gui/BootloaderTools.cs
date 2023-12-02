/****************************************************************************
 * BootloaderTools
 * --------------
 * a collection of methods for working with the chip45boot3 bootloader
 * 
 * copyright 2013, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * http://www.chip45.com, info@chip45.com
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using chip45boot3GUI;
using CrcTools;
using HexFileTools;
using System.Windows.Forms;


namespace BootloaderTools
{
    class Bootloader
    {
        // some public variables for the bootloader and firmware version numbers
        public int versionBootloaderMajor = -1;
        public int versionBootloaderMinor = -1;
        public int versionFirmwareTargetMajor = -1;
        public int versionFirmwareTargetMinor = -1;

        // variables for COM port communication
        public byte[] sendBuffer;  // a send buffer
        public int sendBufferLength = 0;
        public byte[] receiveBuffer;  // and a receive buffer
        public int receiveBufferLength = 0;
        public const int cBUFFERLENGTH = 1024;  // buffer length
        public const int cCRC_RETRIES = 3;  // number of CRC retries
        private const int cBYTES_PER_FRAME = 64;  // maximum number of bytes per frame

        public bool flagRs485 = false;  // flag indicates if bootloader communication over RS485 half duplex
        public int busTargetsActiveAddress = 0;
        private int busTargetsNumberOf = 0;
        private int[] busTargetsAvailableAddresses;

        // objects we need for the bootloader communication
        private SerialPort myComPort;
        private crcCCITT myCrc;


        // protocol characters
        private byte[] autobaudCharacter = { 0x55 };  // 'U'
        private byte[] autobaudEndCharacter = { 0x80 };  // ends autobaud (if active) and starts the addressing sequence in RS485 mode (if RS485 active)
        private byte[] addressingEndCharacter = { 0x81 };  // ends addressing sequence in RS485 mode (if RS485 active)
        private const int cAUTOBAUDREPEAT = 16;  // repeat the autobaud character that often
        private byte[] startOfFrame = { 0x02 };  // STX
        private byte[] endOfFrame = { 0x03 };  // ETX
        private byte[] escapeCharacter = { 0x1B };  // ETX
        private byte[] bootloaderIdentifier = { 0x02, (byte)'c', (byte)'4', (byte)'5', (byte)'b', (byte)'3', 0x03 };  // STX, c45b3, ETX

        // version commands
        private const byte cCMD_READ_VERSION_BOOTLOADER = 0x11;
        private const byte cCMD_READ_VERSION_FIRMWARE   = 0x12;

        // application commands
        private const byte cCMD_START_APPLICATION       = 0x18;

        // programming stuff
        private const byte cCMD_SET_ADDRESS             = 0x21;
        private const byte cCMD_FLASH_WRITE_DATA        = 0x22;
        private const byte cCMD_FLASH_READ_DATA         = 0x23;
        private const byte cCMD_EEPROM_WRITE_DATA       = 0x24;
        private const byte cCMD_EEPROM_READ_DATA        = 0x25;

        // status flag      
        private bool statusConnected = false;


        private string foo;

        /**********************************************************************
         * This is the constructor for the Bootloader class
         * 
         * creates objects for COM port access, CRC tools, etc.
         * also initializes some buffers etc.
         **********************************************************************/
        public Bootloader()
        {
            // create a serial port object
            myComPort = new SerialPort();

            // create a CRC object
            myCrc = new crcCCITT();

            // create buffers
            sendBuffer = new byte[cBUFFERLENGTH];
            receiveBuffer = new byte[cBUFFERLENGTH];

            busTargetsAvailableAddresses = new int[128];
        }


        public bool IsConnected()
        {
            return statusConnected;
        }


        /**********************************************************************
         * This function connects to the bootloader.
         * 
         * opening COM port, sending autobaud character and reading back the identifier
         **********************************************************************/
        public bool Connect(string port, int baudrate)
        {
            GuiForm.myForm.progressBarFlashProgram.Value = 0;  // reset progess bar

            // open the COM port
            if (!myComPort.IsOpen)  // check if COM port is not yet open
            {
                // initialize the COM port parameters
                myComPort.BaudRate = baudrate;
                myComPort.DataBits = 8;
                myComPort.Handshake = Handshake.None;
                myComPort.StopBits = StopBits.One;
                myComPort.Parity = Parity.None;
                myComPort.PortName = port;
                myComPort.DtrEnable = true;  // we can use DTR going low to reset target on many boards
                myComPort.NewLine = System.Text.Encoding.ASCII.GetString(endOfFrame);  // we set the NewLine character to ETX, so that we may simply use ReadLine() function for serial port reception 
                myComPort.ReadTimeout = 3000;  // two seconds timeout

                //myComPort.DataReceived += new SerialDataReceivedEventHandler(eventDataReceived);

                GuiForm.myForm.textBoxLog.AppendText("\r\nOpening COM port... ");
                try
                {
                    myComPort.Open();  // open it
                }
                catch(Exception e) {
                    GuiForm.myForm.textBoxLog.AppendText(String.Format("{0}... ", e.GetType().Name));
                }

            }
            else
            {
                GuiForm.myForm.textBoxLog.AppendText("\r\nCOM port is already open... ");
            }

            // check if COM port is open so that we can use it
            if (!myComPort.IsOpen)
            {
                // if not, we return zero
                GuiForm.myForm.textBoxLog.AppendText("Could not open COM port.");

                return false;
            }
            else
            {
                GuiForm.myForm.textBoxLog.AppendText("done.");

            }

            // wait 250msec before sending the autobaud characters
            Thread.Sleep(250);

            // here comes the RS485 target addressing scheme
            if (flagRs485)
            {
                byte[] busAddress = { 0x00 };
                byte c;
                int saveTimeout;


                // we are going to let the bootloader do automatic baudrate detection
                GuiForm.myForm.textBoxLog.AppendText("\r\nSending autobaud characters... ");


                // send 8x autobaud character
                for (int i = 0; i < cAUTOBAUDREPEAT; ++i)
                {
                    myComPort.Write(autobaudCharacter, 0, 1);
                }
                if (flagRs485)
                {
                    myComPort.Write(autobaudEndCharacter, 0, 1);
                }

                GuiForm.myForm.textBoxLog.AppendText("done.");

                
                GuiForm.myForm.textBoxLog.AppendText("\r\nTrying to connect to RS485 bus targets... ");

                // save old timeout value, since we need a shorter timeout during addressing
                saveTimeout = myComPort.ReadTimeout;

                myComPort.ReadTimeout = 10;  // 10msec timeout here

                busTargetsNumberOf = 0;

                // send all valid addresses
                for (byte i = 1; i <= 127; ++i)
                {
                    busAddress[0] = i;
                    myComPort.Write(busAddress, 0, 1);

                    try
                    {
                        // read one byte
                        c = (byte)myComPort.ReadByte();
                        busTargetsAvailableAddresses[busTargetsNumberOf] = (int)c;
                        GuiForm.myForm.textBoxLog.AppendText(String.Format("[{0}] ", busTargetsAvailableAddresses[busTargetsNumberOf]));
                        ++busTargetsNumberOf;
                    }

                    catch (TimeoutException e)
                    {
                        // we do nothing here, just catch
                    }

                }

                // restore the previous timeout value
                myComPort.ReadTimeout = saveTimeout;

                // end addressing sequence
                myComPort.Write(addressingEndCharacter, 0, 1);

                if (busTargetsNumberOf > 0)
                {
                    GuiForm.myForm.textBoxLog.AppendText("connected.");

                    List<String> tList = new List<String>();
                    GuiForm.myForm.comboBoxBusAddress.Items.Clear();
                    for (int i = 0; i < busTargetsNumberOf; ++i)
                    {
                        tList.Add(String.Format("{0}", busTargetsAvailableAddresses[i]));
                    }
                    if (tList.Count > 0)
                    {
                        GuiForm.myForm.comboBoxBusAddress.Items.AddRange(tList.ToArray());
                        GuiForm.myForm.comboBoxBusAddress.SelectedIndex = 0;
                    }

                    busTargetsNumberOf = 0;

                    // set connected flag
                    statusConnected = true;
                }
                else
                {
                    GuiForm.myForm.textBoxLog.AppendText("no targets found.");

                    return false;

                }


            }
            else  // not in RS485 mode
            {
                int counterConnectRetries = 10; // number of retries...

                do
                {

                    // we are going to let the bootloader do automatic baudrate detection
                    GuiForm.myForm.textBoxLog.AppendText("\r\nSending autobaud characters... ");

                    // send 8x autobaud character
                    for (int i = 0; i < cAUTOBAUDREPEAT; ++i)
                    {
                        myComPort.Write(autobaudCharacter, 0, 1);
                    }
                    if (flagRs485)
                    {
                        myComPort.Write(autobaudEndCharacter, 0, 1);
                    }

                    // preset flag
                    statusConnected = false;

                    // try to read back the bootloader identifier string
                    try
                    {
                        GuiForm.myForm.textBoxLog.AppendText("\r\nTrying to connect bootloader... ");

                        // tweak the new line string with our bootloader identifier and let windows do the work for us
                        myComPort.NewLine = System.Text.Encoding.ASCII.GetString(bootloaderIdentifier);

                        // read a line from serial port (until bootloader identifier was received or timed out)
                        string s = myComPort.ReadLine();

                        GuiForm.myForm.textBoxLog.AppendText("connected.");

                        counterConnectRetries = 0;

                        // clear connected flag
                        statusConnected = true;

                    }

                    // catch timeout in case bootloader does not respond
                    catch (TimeoutException e)
                    {
                        --counterConnectRetries;  // decrement retry counter

                        // show failed message in log window
                        GuiForm.myForm.textBoxLog.AppendText(String.Format("timeout #{0}. \"{1}\"", counterConnectRetries, myComPort.ReadExisting()));

                    }

                } while (counterConnectRetries > 0);


                if (!statusConnected)
                {
                    // close the COM port
                    GuiForm.myForm.textBoxLog.AppendText("\r\nClosing COM port... ");
                    try
                    {
                        myComPort.Close();
                    }
                    catch (Exception f)
                    {
                        // we ignrore the exception here, since it is due to the target MCU leaving the bootloader and hence no valid USB code running
                        GuiForm.myForm.textBoxLog.AppendText(String.Format("{0}... ", f.GetType().Name));
                    }
                    GuiForm.myForm.textBoxLog.AppendText("\r\ndone.");

                    // set back new line character to ETX
                    myComPort.NewLine = System.Text.Encoding.ASCII.GetString(endOfFrame);

                    // return false
                    return false;
                }

            }


            // flushing input buffer
            myComPort.DiscardInBuffer();


            // set back new line character to ETX
            myComPort.NewLine = System.Text.Encoding.ASCII.GetString(endOfFrame);

            // return true
            return true;
        }


        /**********************************************************************
         * This function exits the bootloader.
         * 
         * sending start application command and close COM port
         **********************************************************************/
        public bool Exit()
        {
            GuiForm.myForm.textBoxLog.AppendText("\r\nStarting application... ");

            sendBuffer[0] = cCMD_START_APPLICATION;
            sendBufferLength = 1;

            int saveAddress = busTargetsActiveAddress;  // save current active address

            // in RS485 mode we send the start application command as broadcast to all targets
            if (flagRs485)
            {
                busTargetsActiveAddress = 0x00;  // the exit command will be sent as broadcast
            }

            SendFrame();

            int i = ReceiveFrame();

            busTargetsActiveAddress = saveAddress;  // restore active address

            try
            {
                myComPort.Close();
            }
            catch (Exception e)
            {
                // we ignrore the exception here, since it is due to the target MCU leaving the bootloader and hence no valid USB code running
                GuiForm.myForm.textBoxLog.AppendText(String.Format("{0}... ", e.GetType().Name));
            }

            if (flagRs485)
            {
                GuiForm.myForm.textBoxLog.AppendText("done.");

                // clear connected flag
                statusConnected = false;
                
                return true;
            }

            if (i == -2)
            {
                GuiForm.myForm.textBoxLog.AppendText("CRC error.");
                
                // clear connected flag
                statusConnected = false;

                return false;
            }
            else if (i == -1)
            {
                GuiForm.myForm.textBoxLog.AppendText("timeout.");

                // clear connected flag
                statusConnected = false;

                return false;
            }
            else
            {

                if (receiveBuffer[0] == (cCMD_START_APPLICATION | 0x80))
                {
                    GuiForm.myForm.textBoxLog.AppendText("done.");

                    // clear connected flag
                    statusConnected = false;

                    return true;
                }
                else if (receiveBuffer[0] == cCMD_START_APPLICATION)  // check if answer is same as sent (i.e. sent address must been wrong)
                {
                    GuiForm.myForm.textBoxLog.AppendText("failed.");

                    return false;

                }
                else
                {
                    GuiForm.myForm.textBoxLog.AppendText(String.Format("wrong answer: {0} {1,2}", (int)receiveBuffer[0], System.Text.Encoding.ASCII.GetString(receiveBuffer)));

                    // clear connected flag
                    statusConnected = false;

                    return false;
                }
            }
        }


        /**********************************************************************
         * This function queries the version of the bootloader
         * 
         * sending command cCMD_READ_VERSION_BOOTLOADER
         **********************************************************************/
        public bool ReadBootloaderVersion()
        {
            GuiForm.myForm.textBoxLog.AppendText("\r\nReading bootloader version... ");

            sendBuffer[0] = cCMD_READ_VERSION_BOOTLOADER;
            sendBufferLength = 1;

            SendFrame();

            int i = ReceiveFrame();

            if (i == -2)
            {
                GuiForm.myForm.textBoxLog.AppendText("CRC error.");
                return false;
            }
            else if (i == -1)
            {
                GuiForm.myForm.textBoxLog.AppendText("timeout.");
                return false;
            }
            else
            {

                if (receiveBuffer[0] == (cCMD_READ_VERSION_BOOTLOADER | 0x80))
                {
                    // extract major and minor version from the buffer
                    versionBootloaderMajor = receiveBuffer[1];
                    versionBootloaderMinor = receiveBuffer[2];

                    GuiForm.myForm.textBoxLog.AppendText("done.");

                    return true;
                }
                else if (receiveBuffer[0] == cCMD_READ_VERSION_BOOTLOADER)  // check if answer is same as sent (i.e. sent address must been wrong)
                {
                    GuiForm.myForm.textBoxLog.AppendText("failed.");

                    return false;

                }
                else
                {
                    GuiForm.myForm.textBoxLog.AppendText(String.Format("wrong answer: {0} {1,2}", (int)receiveBuffer[0], System.Text.Encoding.ASCII.GetString(receiveBuffer)));
                    return false;
                }

            }


        }


        /**********************************************************************
         * This function queries the version of the installed target firmware
         * 
         * sending command cCMD_READ_VERSION_FIRMWARE
         **********************************************************************/
        public bool ReadTargetFirmwareVersion()
        {
            GuiForm.myForm.textBoxLog.AppendText("\r\nReading target firmware version... ");

            sendBuffer[0] = cCMD_READ_VERSION_FIRMWARE;
            sendBufferLength = 1;

            SendFrame();

            int i = ReceiveFrame();

            if (i == -2)
            {
                GuiForm.myForm.textBoxLog.AppendText("CRC error.");
                return false;
            }
            else if (i == -1)
            {
                GuiForm.myForm.textBoxLog.AppendText("timeout.");
                versionFirmwareTargetMajor = -1;
                versionFirmwareTargetMinor = -1;
                return false;
            }
            else
            {

                if (receiveBuffer[0] == (cCMD_READ_VERSION_FIRMWARE | 0x80))
                {
                    GuiForm.myForm.textBoxLog.AppendText("done.");

                    // extract major and minor version from the buffer
                    versionFirmwareTargetMajor = receiveBuffer[1];
                    versionFirmwareTargetMinor = receiveBuffer[2];

                    return true;

                }
                else if (receiveBuffer[0] == cCMD_READ_VERSION_FIRMWARE)  // check if answer is same as sent (i.e. sent address must been wrong)
                {
                    GuiForm.myForm.textBoxLog.AppendText("failed.");

                    // clear variables
                    versionFirmwareTargetMajor = -1;
                    versionFirmwareTargetMinor = -1;

                    return false;

                }
                else
                {
                    receiveBuffer[receiveBufferLength] = 0;

                    GuiForm.myForm.textBoxLog.AppendText(String.Format("wrong answer: {0} {1,2}", (int)receiveBuffer[0], System.Text.Encoding.ASCII.GetString(receiveBuffer)));

                    return false;
                }

            }


        }


        /**********************************************************************
         * This function queries the version of the installed target firmware
         * 
         * sending command cCMD_FLASH_SET_ADDRESS
         **********************************************************************/
        public bool SetAddress(UInt32 address)
        {
            GuiForm.myForm.textBoxLog.AppendText("\r\nSending memory address... ");

            sendBuffer[0] = cCMD_SET_ADDRESS;
            sendBuffer[1] = (byte)((address >> 24) & 0xff);
            sendBuffer[2] = (byte)((address >> 16) & 0xff);
            sendBuffer[3] = (byte)((address >> 8) & 0xff);
            sendBuffer[4] = (byte)((address >> 0) & 0xff);
            sendBufferLength = 5;

            SendFrame();

            int i = ReceiveFrame();

            if (i == -2)
            {
                GuiForm.myForm.textBoxLog.AppendText("CRC error.");
                return false;
            }
            else if (i == -1)
            {
                GuiForm.myForm.textBoxLog.AppendText("timeout.");
                return false;
            }
            else
            {
                if (receiveBuffer[0] == (cCMD_SET_ADDRESS | 0x80))  // check if answer is same as sent command with MSB set
                {
                    GuiForm.myForm.textBoxLog.AppendText("done.");

                    return true;

                }
                else if (receiveBuffer[0] == cCMD_SET_ADDRESS)  // check if answer is same as sent (i.e. sent address must been wrong)
                {
                    GuiForm.myForm.textBoxLog.AppendText("failed.");

                    return false;

                }
                else  // answer has completely wrong value
                {
                    receiveBuffer[receiveBufferLength] = 0;

                    GuiForm.myForm.textBoxLog.AppendText(String.Format("wrong answer: {0} {1,2}", (int)receiveBuffer[0], System.Text.Encoding.ASCII.GetString(receiveBuffer)));

                    return false;
                }

            }


        }


        /**********************************************************************
         * This function uploads data to the target to program flash
         * 
         * sending command cCMD_FLASH_WRITE_DATA
         **********************************************************************/
        public bool FlashWrite(HexFile hex)
        {
            GuiForm.myForm.textBoxLog.AppendText("\r\nProgramming flash memory... ");

            bool finished = false;  // flag to indicate flash programming is done
            int sendBufferIndex = 0;
            int hexFileBufferIndex = hex.hexStartSegmentAddress;
            int i;

            do
            {
                sendBufferIndex = 0;  // clear send buffer index
                sendBuffer[sendBufferIndex++] = cCMD_FLASH_WRITE_DATA;  // set command

                // transfer 64 bytes from hexfile buffer into send buffer
                for (i = 0; i < cBYTES_PER_FRAME; i++)
                {
                    sendBuffer[sendBufferIndex++] = hex.buffer[hexFileBufferIndex++];  // copy byte

                    // check if we reached end of hexfile buffer
                    if (hexFileBufferIndex >= hex.bufferLength)
                    {
                        finished = true;  // set finished flag
                        break;
                    }
                }

                sendBufferLength = sendBufferIndex;  // set length
                SendFrame();  // and send frame

                // receive the answer
                i = ReceiveFrame();

                if (i == -2)
                {
                    GuiForm.myForm.textBoxLog.AppendText("CRC error during flash programming.");
                    return false;
                }
                else if (i == -1)
                {
                    GuiForm.myForm.textBoxLog.AppendText("timeout during flash programming.");
                    return false;
                }
                else
                {
                    if (receiveBuffer[0] == (cCMD_FLASH_WRITE_DATA | 0x80))  // check if answer is same as sent command with MSB set
                    {
                        GuiForm.myForm.textBoxLog.AppendText(".");
                        GuiForm.myForm.progressBarFlashProgram.Increment(GuiForm.myForm.progressBarFlashProgram.Maximum / (hex.bufferLength / cBYTES_PER_FRAME));

                    }
                    else if (receiveBuffer[0] == cCMD_FLASH_WRITE_DATA)  // check if answer is same as sent (i.e. sent address must been wrong)
                    {
                        GuiForm.myForm.textBoxLog.AppendText("failed during flash programming.");

                        return false;

                    }
                    else  // answer has completely wrong value
                    {
                        receiveBuffer[receiveBufferLength] = 0;

                        GuiForm.myForm.textBoxLog.AppendText(String.Format("wrong answer: {0} {1,2}", (int)receiveBuffer[0], System.Text.Encoding.ASCII.GetString(receiveBuffer)));

                        return false;
                    }

                }

            } while (!finished);


            // send command to write last target flash page in case it was not filled until SPM_PAGESIZE
            sendBuffer[0] = cCMD_FLASH_WRITE_DATA;
            sendBufferLength = 1;
            SendFrame();

            // receive the answer
            i = ReceiveFrame();

            if (i == -2)
            {
                GuiForm.myForm.textBoxLog.AppendText("CRC error during flash finish.");
                return false;
            }
            else if (i == -1)
            {
                GuiForm.myForm.textBoxLog.AppendText("timeout during flash finish.");
                return false;
            }
            else
            {
                if (receiveBuffer[0] == (cCMD_FLASH_WRITE_DATA | 0x80))  // check if answer is same as sent command with MSB set
                {
                    GuiForm.myForm.textBoxLog.AppendText(".");
                    GuiForm.myForm.progressBarFlashProgram.Increment(GuiForm.myForm.progressBarFlashProgram.Maximum / (hex.bufferLength / cBYTES_PER_FRAME + 1));

                }
                else if (receiveBuffer[0] == cCMD_FLASH_WRITE_DATA)  // check if answer is same as sent (i.e. sent address must been wrong)
                {
                    GuiForm.myForm.textBoxLog.AppendText("failed during flash finish.");

                    return false;

                }
                else  // answer has completely wrong value
                {
                    receiveBuffer[receiveBufferLength] = 0;

                    GuiForm.myForm.textBoxLog.AppendText(String.Format("wrong answer: {0} {1,2}", (int)receiveBuffer[0], System.Text.Encoding.ASCII.GetString(receiveBuffer)));

                    return false;
                }
            }

            GuiForm.myForm.textBoxLog.AppendText(" done.");

            return true;
        }


        /**********************************************************************
         * This function reads the target flash memory
         * 
         * sending command cCMD_FLASH_READ_DATA
         **********************************************************************/
        public bool FlashRead(HexFile hex)
        {
            GuiForm.myForm.textBoxLog.AppendText("\r\nReading flash memory... ");

            int sendBufferIndex = 0;
            int i, count;

            int hexFileBufferIndex = 0;

            do
            {
                sendBufferIndex = 0;  // clear send buffer index
                sendBuffer[sendBufferIndex++] = cCMD_FLASH_READ_DATA;  // put command into buffer 
                sendBuffer[sendBufferIndex++] = 64;  // we (try to) read 64 bytes (which is the maximum for one read command)
                
                sendBufferLength = sendBufferIndex;  // set length
                SendFrame();  // and send frame

                count = ReceiveFrame();  // receive the answer

                if (count == -2)
                {
                    GuiForm.myForm.textBoxLog.AppendText("CRC error during flash reading.");
                    return false;
                }
                else if (count == -1)
                {
                    GuiForm.myForm.textBoxLog.AppendText("timeout during flash reading.");
                    return false;
                }
                else
                {
                    if (receiveBuffer[0] == (cCMD_FLASH_READ_DATA | 0x80))  // check if answer is same as sent command with MSB set
                    {
                        GuiForm.myForm.textBoxLog.AppendText(".");
                        GuiForm.myForm.progressBarFlashProgram.Increment((hexFileBufferIndex * 1000) / (384 * 1024));

                        // check if we have received any bytes
                        if (count > 1)
                        {
                            // transfer received bytes from receive buffer into hexfile buffer
                            for (i = 1; i < count; i++)
                            {
                                hex.buffer[hexFileBufferIndex++] = receiveBuffer[i];  // copy byte

                            }

                        }
                        else
                        {
                            // we haven't received any bytes, i.e. we reached end of application flash memory
                            GuiForm.myForm.textBoxLog.AppendText("done.");
                            GuiForm.myForm.progressBarFlashProgram.Increment(1000000);  // push to right end
                            hex.bufferLength = hexFileBufferIndex;  // set the buffer length
                            return true;
                        }




                    }
                    else if (receiveBuffer[0] == cCMD_FLASH_READ_DATA)  // check if answer is same as sent (i.e. no more flash data to read available)
                    {
                        GuiForm.myForm.textBoxLog.AppendText("failed during flash reading.");
                        return true;

                    }
                    else  // answer has completely wrong value
                    {
                        receiveBuffer[receiveBufferLength] = 0;

                        GuiForm.myForm.textBoxLog.AppendText(String.Format("wrong answer: {0} {1,2}", (int)receiveBuffer[0], System.Text.Encoding.ASCII.GetString(receiveBuffer)));

                        return false;
                    }

                }


            } while (hexFileBufferIndex <= hex.cBUFFERSIZE);  // do loop until we overrun the buffer

            return false;  // we should not come here
        }


        /**********************************************************************
         * This function uploads data to the target to program eeprom
         * 
         * sending command cCMD_EEPROM_WRITE_DATA
         **********************************************************************/
        public bool EepromWrite(HexFile hex)
        {
            GuiForm.myForm.textBoxLog.AppendText("\r\nProgramming eeprom memory... ");

            bool finished = false;  // flag to indicate flash programming is done
            int sendBufferIndex = 0;
            int hexFileBufferIndex = 0;
            int i;


            do
            {
                sendBufferIndex = 0;  // clear send buffer index
                sendBuffer[sendBufferIndex++] = cCMD_EEPROM_WRITE_DATA;  // set command

                // transfer 64 bytes from hexfile buffer into send buffer
                for (i = 0; i < cBYTES_PER_FRAME; i++)
                {
                    sendBuffer[sendBufferIndex++] = hex.buffer[hexFileBufferIndex++];  // copy byte

                    // check if we reached end of hexfile buffer
                    if (hexFileBufferIndex >= hex.bufferLength)
                    {
                        finished = true;  // set finished flag
                        break;
                    }
                }

                sendBufferLength = sendBufferIndex;  // set length
                SendFrame();  // and send frame

                // receive the answer
                i = ReceiveFrame();

                if (i == -2)
                {
                    GuiForm.myForm.textBoxLog.AppendText("CRC error during eeprom programming.");
                    return false;
                }
                else if (i == -1)
                {
                    GuiForm.myForm.textBoxLog.AppendText("timeout during eeprom programming.");
                    return false;
                }
                else
                {
                    if (receiveBuffer[0] == (cCMD_EEPROM_WRITE_DATA | 0x80))  // check if answer is same as sent command with MSB set
                    {
                        GuiForm.myForm.textBoxLog.AppendText(".");
                        GuiForm.myForm.progressBarEepromProgram.Increment(GuiForm.myForm.progressBarEepromProgram.Maximum / (hex.bufferLength / cBYTES_PER_FRAME + 1));

                    }
                    else if (receiveBuffer[0] == cCMD_EEPROM_WRITE_DATA)  // check if answer is same as sent (i.e. sent address must been wrong)
                    {
                        GuiForm.myForm.textBoxLog.AppendText("failed during flash programming.");

                        return false;

                    }
                    else  // answer has completely wrong value
                    {
                        receiveBuffer[receiveBufferLength] = 0;

                        GuiForm.myForm.textBoxLog.AppendText(String.Format("wrong answer: {0} {1,2}", (int)receiveBuffer[0], System.Text.Encoding.ASCII.GetString(receiveBuffer)));

                        return false;
                    }

                }

            } while (!finished);

            GuiForm.myForm.textBoxLog.AppendText(" done.");

            return true;
        }


        /**********************************************************************
         * This function reads the target eeprom memory
         * 
         * sending command cCMD_EEPROM_READ_DATA
         **********************************************************************/
        public bool EepromRead(HexFile hex)
        {
            GuiForm.myForm.textBoxLog.AppendText("\r\nReading eeprom memory... ");

            int sendBufferIndex = 0;
            int i, count;

            int hexFileBufferIndex = 0;

            do
            {
                sendBufferIndex = 0;  // clear send buffer index
                sendBuffer[sendBufferIndex++] = cCMD_EEPROM_READ_DATA;  // put command into buffer 
                sendBuffer[sendBufferIndex++] = 64;  // we (try to) read 64 bytes (which is the maximum for one read command)

                sendBufferLength = sendBufferIndex;  // set length
                SendFrame();  // and send frame

                count = ReceiveFrame();  // receive the answer

                if (count == -2)
                {
                    GuiForm.myForm.textBoxLog.AppendText("CRC error during eeprom reading.");
                    return false;
                }
                else if (count == -1)
                {
                    GuiForm.myForm.textBoxLog.AppendText("timeout during eeprom reading.");
                    return false;
                }
                else
                {
                    if (receiveBuffer[0] == (cCMD_EEPROM_READ_DATA | 0x80))  // check if answer is same as sent command with MSB set
                    {
                        GuiForm.myForm.textBoxLog.AppendText(".");
                        GuiForm.myForm.progressBarEepromProgram.Increment((hexFileBufferIndex * 1000) / (384 * 1024));

                        // check if we have received any bytes
                        if (count > 1)
                        {
                            // transfer received bytes from receive buffer into hexfile buffer
                            for (i = 1; i < count; i++)
                            {
                                hex.buffer[hexFileBufferIndex++] = receiveBuffer[i];  // copy byte

                            }

                        }
                        else
                        {
                            // we haven't received any bytes, i.e. we reached end of application flash memory
                            GuiForm.myForm.textBoxLog.AppendText("done.");
                            GuiForm.myForm.progressBarEepromProgram.Increment(1000000);  // push to right end
                            hex.bufferLength = hexFileBufferIndex;  // set the buffer length
                            return true;
                        }




                    }
                    else if (receiveBuffer[0] == cCMD_EEPROM_READ_DATA)  // check if answer is same as sent (i.e. no more flash data to read available)
                    {
                        GuiForm.myForm.textBoxLog.AppendText("failed during eeprom reading.");
                        return true;

                    }
                    else  // answer has completely wrong value
                    {
                        receiveBuffer[receiveBufferLength] = 0;

                        GuiForm.myForm.textBoxLog.AppendText(String.Format("wrong answer: {0} {1,2}", (int)receiveBuffer[0], System.Text.Encoding.ASCII.GetString(receiveBuffer)));

                        return false;
                    }

                }


            } while (hexFileBufferIndex <= hex.cBUFFERSIZE);  // do loop until we overrun the buffer

            return false;  // we should not come here
        }

        
        /**********************************************************************
         * This function sends a frame to the bootloader
         * 
         * 
         **********************************************************************/
        private void SendFrame()
        {
            int crc = 0;
            int count = 0;
            byte[] tmpBuffer = new byte[cBUFFERLENGTH * 2];
            int tmpBufferIndex = 0;
            int length = sendBufferLength;


            if (flagRs485)
            {
                for (int i = length; i >= 0; --i)
                {
                    sendBuffer[i + 1] = sendBuffer[i];
                }
                sendBuffer[0] = (byte)busTargetsActiveAddress;
                ++length;
            }

            // compute the crc over the buffer minus the last two bytes
            myCrc.Init();
            myCrc.Update(sendBuffer, length);
            myCrc.Complete();
            crc = myCrc.Get();

            // append the CRC to the buffer
            sendBuffer[length++] = (byte)((crc >> 8) & 0xff);
            sendBuffer[length++] = (byte)(crc & 0xff);


            // we start with STX
            tmpBuffer[tmpBufferIndex++] = startOfFrame[0];


            // loop to fill the tempräry buffer with sendBuffer data plus protocol characters
            // this preprocessing is necessarry to use just one call to myComPort.Write() later, instead of sending each bytes separately
            // (if using a USB UART converter, this would introduce a 1msec delay for each character due to 1kHz USB cycle time)
            do
            {

                byte c = sendBuffer[count];  // get next character

                // check if current character is one of the protocol characters
                if ((c == startOfFrame[0]) || (c == endOfFrame[0]) || (c == escapeCharacter[0]))
                {

                    // if so, we prepend an ESC
                    tmpBuffer[tmpBufferIndex++] = escapeCharacter[0];

                    // and add 0x80 to the character
                    c += 0x80;
                }

                // send the character
                tmpBuffer[tmpBufferIndex++] = c;

                // increment count		
                ++count;

            } while (count < length);  // check for end of buffer

            // finally add ETX
            tmpBuffer[tmpBufferIndex++] = endOfFrame[0];

            // send frame
            try
            {
                myComPort.Write(tmpBuffer, 0, tmpBufferIndex);
            }
            catch (Exception e)
            {
                // we ignrore the exception here, since it is due to the target MCU leaving the bootloader and hence no valid USB code running
                GuiForm.myForm.textBoxLog.AppendText(String.Format("{0}... ", e.GetType().Name));
            }

        }


        /**********************************************************************
         * This function receives a frame from the bootloader
         * 
         * 
         **********************************************************************/
        private int ReceiveFrame()
        {
            byte c;  // receive character
	        int count = 0;  // reset byte count
	        byte esc_offset = 0;  // clear esc'aped offset


            // read a line from serial port (until ETX received or timed out)
            try
            {
                
                // loop for receiving a frame
                do
                {
                    // read one byte from COM port
                    c = (byte)myComPort.ReadByte();

                    // check for protocol characters
                    if (c == startOfFrame[0])  // STX character starts a transmission
                    {
                        count = 0;  // set byte count to zero
                    }
                    else if (c == escapeCharacter[0])  // STX, ETX and ESC character needs to be esc'aped when in normal data bytes
                    {
                        esc_offset = 0x80;  // esc'aped means, the next character has an offset of 0x80
                    }
                    else if (c == endOfFrame[0])  // we exit loop on ETX
                    {  
                        break;

                    }
                    else  // a normal data byte has been received
                    {  
                        receiveBuffer[count++] = (byte)(c - esc_offset);  // write data byte into the buffer and increment count
                        esc_offset = 0x00;  // clear offset, esc'aped is only valid for one character
                    }

                } while (count < cBUFFERLENGTH);  // check for buffer overflow


                // compute the crc over the buffer minus the last two bytes
                myCrc.Init();
                myCrc.Update(receiveBuffer, count - 2);  // received CRC is not included in the CRC -> -2
                myCrc.Complete();

                // check received CRC against calculated CRC
                if ((receiveBuffer[count - 2] * 256 + receiveBuffer[count - 1]) == myCrc.Get())
                {
                    receiveBufferLength = count - 2;  // set receive buffer length without CRC

                    if (flagRs485)
                    {
                        if (receiveBuffer[0] != (byte)busTargetsActiveAddress)
                        {
                            GuiForm.myForm.textBoxLog.AppendText("\r\naddress doesn't match."); 
                            return -1;  // address came from wrong target (shoud not happen anyway...)
                        }
                        for (int i = 0; i < receiveBufferLength; ++i)
                        {
                            receiveBuffer[i] = receiveBuffer[i + 1];
                        }
                        --receiveBufferLength;
                    }

                    return receiveBufferLength;  // return number received characters (we subtract two for the CRC)
                }
                else
                {
                    
                    return -2;  // CRC error
                }


            }
            catch (Exception e)
            {
                // we ignore the exception here, since it is due to the target MCU leaving the bootloader and hence no valid USB code running
                //GuiForm.myForm.textBoxLog.AppendText(String.Format("{0}... ", e.GetType().Name)); 
                return -1;  // timeout error

            }
        }

        
    }


}