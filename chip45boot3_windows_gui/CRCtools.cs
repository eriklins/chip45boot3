/****************************************************************************
 * CRCtools
 * --------
 * a collection of methods for cyclic redundancy check (CRC) computations
 * 
 * crcCCITT:
 * computes a 16-bit CRC with 0x1021 as polynome and 0x84cf as start value
 * 
 * copyright 2011, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * http://www.chip45.com, info@chip45.com
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CrcTools
{

    // class for CRC CCITT checksum computation
    public class crcCCITT
    {
        // global variables
        private int crcValue = 0;  // the actual CRC CCITT checksum value

        // constructor
        public crcCCITT ()
        {
            // does nothing
        }

        // initialize the current crc value
        public void Init ()
        {
            crcValue = 0x84cf;  // init crc value
        }

        // get the current crc value
        public int Get ()
        {
            return crcValue;  // return the crc value
        }

        // update CRC CCITT checksum
        // input: byte buffer to the data and number of bytes in the buffer
        public void Update (byte[] data, int dataLength)
        {
            int i, count;
            bool bit;

            for (count = 0; count < dataLength; ++count)  // loop over
            {
                for (i = 0; i < 8; i++)  // loop over eight bits
                {
                    bit = Convert.ToBoolean (crcValue & 0x8000);  // get highest bit value
                    crcValue = (crcValue << 1) | ((data[count] >> (7 - i)) & 0x01);  // process
                    if (bit)
                    {
                        crcValue ^= 0x1021;  // xor value with the polynome
                    }
                }
                crcValue &= 0xffff;  // trunk to 16 bits
            }
        }

        // compute the final CRC value
        public void Complete ()
        {
            int i;
            bool bit;

            for (i = 0; i < 16; i++) {
                bit = Convert.ToBoolean (crcValue & 0x8000);  // get highest bit value
                crcValue = (crcValue << 1) | 0x00;  // shift
                if (bit) {
                    crcValue ^= 0x1021;  // xor with the polynome
                }
            }
            crcValue &= 0xffff;  // trunk to 16 bits
        }

        // other CRC methods may follow as needed
    }
}
