using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using HexFileTools;

namespace hexfileEncrypt
{
    class XtaeTools
    {
        // a public variable for storing the 128 bit XTAE key in four 32 bit unsigned integers
        public UInt32[] xtaeKey = { 0, 0, 0, 0 };

        // public buffer and length of buffer
        public byte[] buffer;  // a buffer for the hex file content
        public int bufferLength = 0;  // end of buffer when file was read

        private const int cBUFFERSIZE = 1024 * 1024;


        // class constructor
        public XtaeTools()
        {
            buffer = new byte[cBUFFERSIZE];  // create a buffer of size bytes

        }


        // convert the key from 16 character string to 4 UInt32's
        public void ConvertStringToUInt32(string s) {

            // convert xtae key string into byte array (we use ASCII encoding here, so only ASCII characters should be used)
            byte[] b = System.Text.Encoding.ASCII.GetBytes(s);//"0123456789012345");

            // split the key string into four Xtae.Tools 32 bit integer variables
            for (int i = 0; i < 4; ++i)
            {
                xtaeKey[i] = 0;

                for (int j = 0; j < 4; ++j) {

                    xtaeKey[i] |= (UInt32)(b[(i * 4) + j] << (j * 8));
                }
            }

        }


        // encrypt a buffer with XTAE
        public void EncryptBuffer(byte[] buf, int length) {

            UInt32[] v = { 0, 0 };
            UInt32[] w = { 0, 0 };
            int i;

            for (i = 0; i < length; i += 8) {

                // compose a block of 64 bits into two 32 bit variables
                v[0] = ((UInt32)buf[i+0] << 24) + ((UInt32)buf[i+1] << 16) + ((UInt32)buf[i+2] << 8) + ((UInt32)buf[i+3] << 0);
                v[1] = ((UInt32)buf[i+4] << 24) + ((UInt32)buf[i+5] << 16) + ((UInt32)buf[i+6] << 8) + ((UInt32)buf[i+7] << 0);

                // encrypt this block
                w = encipherBlock(v);

                // and write the result into the global buffer
                buffer[i + 0] = (byte)((w[0] >> 24) & 0xff);
                buffer[i + 1] = (byte)((w[0] >> 16) & 0xff);
                buffer[i + 2] = (byte)((w[0] >> 08) & 0xff);
                buffer[i + 3] = (byte)((w[0] >> 00) & 0xff);
                buffer[i + 4] = (byte)((w[1] >> 24) & 0xff);
                buffer[i + 5] = (byte)((w[1] >> 16) & 0xff);
                buffer[i + 6] = (byte)((w[1] >> 08) & 0xff);
                buffer[i + 7] = (byte)((w[1] >> 00) & 0xff);
            }

            bufferLength = length;
        }


        // encrypt a block of 64 bits
        private UInt32[] encipherBlock(UInt32[] v)
        {

            UInt32[] w = { 0, 0 };

            UInt32 v0 = v[0];
            UInt32 v1 = v[1];
            UInt32 sum = 0;
            UInt32 delta = 0x9E3779B9;

			int rounds = 32;

            while(rounds-- > 0)
            {
                v0 += (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + xtaeKey[sum & 3]);
                sum += delta;
                v1 += (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + xtaeKey[(sum >> 11) & 3]);
            }

            w[0] = v0;
            w[1] = v1;

            return w;
        }


    }
}


