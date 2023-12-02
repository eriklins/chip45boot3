/************************************************************************
 * crc.c
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * a function to calculate the CRC CCITT checksum of a buffer
 * (note: the functions were generated with pycrc script, see below)
 ************************************************************************/


/**
 * \file crc_ccitt.c
 * Functions and types for CRC checks.
 *
 * Generated on Wed Apr 13 16:27:47 2011,
 * by pycrc v0.7.7, http://www.tty1.net/pycrc/
 * using the configuration:
 *    Width        = 16
 *    Poly         = 0x1021
 *    XorIn        = 0xffff
 *    ReflectIn    = False
 *    XorOut       = 0x0000
 *    ReflectOut   = False
 *    Algorithm    = bit-by-bit
 *****************************************************************************/
#include "crc.h"


/**
 * Update the crc value with new data.
 *
 * \param crc      The current crc value.
 * \param data     Pointer to a buffer of \a data_len bytes.
 * \param data_len Number of bytes in the \a data buffer.
 * \return         The updated crc value.
 *****************************************************************************/
crc_t crcCompute(uint8_t *data, unsigned int data_len)
{
    uint8_t i;
    uint8_t b;
    uint8_t c;
	crc_t crc = 0x84cf;

    while (data_len--) {
        c = *data++;
        for (i = 0; i < 8; i++) {
            b = 0;
            if(crc & 0x8000)
                b = 1;
            crc = (crc << 1) | ((c >> (7 - i)) & 0x01);
            if (b) {
                crc ^= 0x1021;
            }
        }
    }

	for (i = 0; i < 16; i++) {
		b = 0;
		if(crc & 0x8000)
		b = 1;
		crc = (crc << 1) | 0x00;
		if (b) {
			crc ^= 0x1021;
		}
	}
	return crc;
		
}

