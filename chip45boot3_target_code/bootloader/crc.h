/************************************************************************
 * crc.h
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * header file for crc.c
 * (note: the functions were generated with pycrc script, see below)
 ************************************************************************/


/**
 * \file crc_ccitt.h
 * Functions and types for CRC checks.
 *
 * Generated on Wed Apr 13 16:27:41 2011,
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
#ifndef __CRC_CCITT_H__
#define __CRC_CCITT_H__

#include <stdint.h>


/**
 * The type of the CRC values.
 *
 * This type must be big enough to contain at least 16 bits.
 *****************************************************************************/
typedef uint16_t crc_t;


/**
 * Update the crc value with new data.
 *
 * \param data     Pointer to a buffer of \a data_len bytes.
 * \param data_len Number of bytes in the \a data buffer.
 * \return         The updated crc value.
 *****************************************************************************/
crc_t crcCompute(uint8_t *data, unsigned int data_len);


#endif      /* __CRC_CCITT_H__ */
