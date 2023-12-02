/****************************************************************************
 * HexFileTools
 * --------------
 * a collection of methods for working with intel hex files
 * 
 * InitBuffer:
 * initializes a buffer of given size with given fill value
 * 
 * ReadFile:
 * read an intel hex file from file system at a given path
 * 
 * copyright 2011, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * http://www.chip45.com, info@chip45.com
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using hexfileEncrypt;

namespace HexFileTools
{
    class HexFile
    {
        // public buffer and length of buffer
        public byte[] buffer;  // a buffer for the hex file content
        public int bufferLength = 0;  // end of buffer when file was read
    

        private const int cBUFFERSIZE = 1024*1024;


        // constructor
        public HexFile()
        {
            buffer = new byte[cBUFFERSIZE];  // create a buffer of size bytes

            for (int i = 0; i < cBUFFERSIZE; ++i)  // preset the buffer with fill value
            {
                buffer[i] = 0xff;
            }
        }


        // open -> read -> parse hex file and write data to binarybuffer
        public int ReadFile(string path)
        {
            // some local variables
            byte[] hexBuffer = new byte[525];  // buffer for one line (record) of the hex file

            // some hex record related variables
            byte hexByteCount = 0;
            UInt16 hexAddress = 0;
            byte hexRecordType = 0;
            byte hexChecksum = 0;
            UInt16 hexExtendedSegmentAddress = 0;

            // some flags
            bool flagEndOfFile = false;
            bool flagEndOfFileRecordMissing = false;


            // open file for reading
            FileStream fs = File.OpenRead(path);

            // reset flash buffer end address
            bufferLength = 0;

            // Read and verify the data
            int r = 0;
            do
            {
                // init some hex record variables

                byte myChecksum = 0;

                // read one hex record from file
                int readIndex = 0;
                do
                {
                    r = fs.ReadByte();  // read next byte from file
                    if (r == -1)
                    {
                        flagEndOfFile = true;
                        break;
                    }
                    hexBuffer[readIndex] = (byte)r;
                    if (hexBuffer[readIndex] == '\r')  // if it's a carriage return, we exit the do while loop
                        break;
                    if (hexBuffer[readIndex] != '\n')  // if it's a line feed, we skip it
                        ++readIndex;  // otherwise we increment the buffer counter
                } while (readIndex < 1024);

                // check for valid hex record identifier and not end of file
                if (hexBuffer[0] == ':' && flagEndOfFile == false)
                {
                    // parse the hex record
                    hexByteCount = asciiToHex2(hexBuffer[1], hexBuffer[2]);  // get the byte count
                    myChecksum = hexByteCount;  // start the own checksum
                    hexAddress = asciiToHex2(hexBuffer[3], hexBuffer[4]);  // get the address high byte
                    myChecksum += (byte)hexAddress;  // compute owm checksum
                    hexAddress = (UInt16)((hexAddress << 8) + asciiToHex2(hexBuffer[5], hexBuffer[6]));  // get the address low byte
                    myChecksum += (byte)(hexAddress & 0x00ff);  // compute own checksum
                    hexRecordType = asciiToHex2(hexBuffer[7], hexBuffer[8]);  // get the record type
                    myChecksum += hexRecordType;  // compute owm checksum
                    hexChecksum = asciiToHex2(hexBuffer[hexByteCount * 2 + 9], hexBuffer[hexByteCount * 2 + 10]);  // get the checksum from the record

                    // now we check the type of the record
                    switch (hexRecordType)
                    {
                        case 0:  // normal data record
                            int i;
                            // parse the data bytes and write to flash buffer
                            for (i = 0; i < (2 * hexByteCount); i += 2)
                            {  // increment by two
                                byte dataByte = asciiToHex2(hexBuffer[i + 9], hexBuffer[i + 10]);  // get the low data byte
                                myChecksum += dataByte;  // compute owm checksum
                                buffer[hexAddress + (hexExtendedSegmentAddress * 16) + (i >> 1)] = dataByte;
                            }

                            // in case the hex file is unordered, 
                            if (bufferLength < hexAddress + (hexExtendedSegmentAddress * 16) + (i >> 1))
                            {
                                bufferLength = hexAddress + (hexExtendedSegmentAddress * 16) + (i >> 1);
                            }

                            break;

                        case 1:  // end of file record
                            flagEndOfFile = true;
                            break;

                        case 2:  // extended segment address record
                            hexExtendedSegmentAddress = (UInt16)(asciiToHex2(hexBuffer[9], hexBuffer[10]) << 8);  // get high byte of extended segment address
                            myChecksum += (byte)(hexExtendedSegmentAddress >> 8);  // compute owm checksum
                            hexExtendedSegmentAddress += asciiToHex2(hexBuffer[11], hexBuffer[12]);  // get low byte of extended segment address
                            myChecksum += (byte)(hexExtendedSegmentAddress & 0xff);  // compute owm checksum

                            break;

                        default:  // unknown hex record type
                            bufferLength = 0;
                            return -1;  // return error code

                    }

                    // check XOR checksum
                    if (((myChecksum + hexChecksum) & 0xff) != 0)
                    {
                        bufferLength = 0;
                        return -2;  // return error code
                    }

                }
            } while (flagEndOfFile == false);

            // if we had not end of file record, we return
            if (flagEndOfFileRecordMissing == true)
            {
                bufferLength = 0;
                return -3;  // return error code 
            }

            // close file
            fs.Close();

            // return without error
            return 0;
        }


        // write a buffer of "length" bytes into a file named "path"
        public void WriteFile(byte[] writeBuffer, int writeLength, string path)
        {
            var form = Form.ActiveForm as Form1;  // get object to access form controls

            int hexFileExtSegmentAddress = 0x00000000;
            int hexFileByteCount = 0x10;  // byte count is fixed to 16 bytes
            int flashBufferAddress = 0;  // we always start at bottom of flash
            int hexFileChecksum = 0;
            string hexLine;


            // open file for writing
            FileStream fs = File.Open(path, FileMode.Create);

 
            // outer loop for each 16 byte hex file lines
            for (int hexFileLineCounter = 0; hexFileLineCounter <= ((writeLength - 1) / hexFileByteCount); ++hexFileLineCounter)
            {

                flashBufferAddress = hexFileLineCounter * hexFileByteCount;

                // start printing normal data record

                hexLine = String.Format(":{0,2:X2}{1,4:X4}{2,2:X2}", hexFileByteCount, (UInt16)(flashBufferAddress & 0xffff), 0x00);

                // checksum...
                hexFileChecksum = hexFileByteCount;
                hexFileChecksum += (flashBufferAddress >> 8);
                hexFileChecksum += (flashBufferAddress & 0xff);

                // inner loop for the 16 data bytes
                for (int hexFileDataByteCounter = 0; hexFileDataByteCounter < 16; ++hexFileDataByteCounter)
                {

                    byte c = writeBuffer[flashBufferAddress + hexFileDataByteCounter];

                    hexLine += String.Format("{0,2:X2}", c);

                    hexFileChecksum += c;

                }

                // append the calculated checksum and line ending
                hexLine += String.Format("{0,2:X2}\r\n", (0x100 - hexFileChecksum) & 0xff);

                // write hex line to file
                fs.Write(System.Text.Encoding.ASCII.GetBytes(hexLine), 0, hexLine.Length);


                // check if we would exceed the 64k page address range with the next hex file line and insert a 
                if (((flashBufferAddress & 0xffff) + 16) == 0x10000)
                {
                    hexFileExtSegmentAddress += 0x1000;  // the segment address is later multiplied by 16 and added to the hex record address
                    hexLine = String.Format(":02000002{0,4:X4}{1,2:X2}\r\n", hexFileExtSegmentAddress, (byte)(0x100 - (4 + (hexFileExtSegmentAddress >> 8) + (hexFileExtSegmentAddress & 0xff))));

                    // write hex line to file
                    fs.Write(System.Text.Encoding.ASCII.GetBytes(hexLine), 0, hexLine.Length);

                }

                // move progress bar
                form.progressBarHexEncrypt.Increment(((flashBufferAddress + 1) * 1000) / (writeLength + 1));

            }

            // last line is an end of file hexrecord
            hexLine = ":00000001FF\r\n";

            // write hex line to file
            fs.Write(System.Text.Encoding.ASCII.GetBytes(hexLine), 0, hexLine.Length);

            // close file
            fs.Close();
        }


        // convert one ascii character into a byte (nibble in fact)
        private byte asciiToHex(byte ucNibble)
        {

            byte ucHex = 0;

            // check if ascii character is
            if (ucNibble >= 'a')  // a lower case letter "a-f"
            {
                ucHex = (byte)((ucNibble - 'a') + 0x0a);
            }
            else if (ucNibble >= 'A')  // or an upper case letter "A-F"
            {
                ucHex = (byte)((ucNibble - 'A') + 0x0a);
            }
            else if ((ucNibble >= '0'))  // or a number "0-9"
            {
                ucHex = (byte)(ucNibble - '0');
            }

            return ucHex;  // return the hex number
        }


        // convert two ascii characters into a byte
        private byte asciiToHex2(byte ucNibbleHigh, byte ucNibbleLow)
        {
            return (byte)((asciiToHex(ucNibbleHigh) << 4) + asciiToHex(ucNibbleLow));  // just call the function for one nibble twice!
        }

    }
}