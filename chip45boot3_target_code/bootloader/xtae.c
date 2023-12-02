/************************************************************************
 * xtae.c
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * XTAE decryption functions
 ************************************************************************/


/************************************************************************
 * include header files
 ************************************************************************/
#include <avr/pgmspace.h>

#include "configure.h"


#ifdef USE_ENCRYPTION


	/************************************************************************
	 * global variables
	 ************************************************************************/
	uint32_t xtaeKey[] = ENCRYPTION_KEY;  // this is the 128 bit key represented as four 32 bit int (defined in configure.h)


	/************************************************************************
	 * initialize the encryption system
	 * 
	 * in : nothing
	 * out: nothing
	 ************************************************************************/
	void xtaeInit(void) {

		// nothing to do...
	}


	/************************************************************************
	 * decrypt a buffer (maximum 64k due to length being uint16_t)
	 * 
	 * in : nothing
	 * out: nothing
	 ************************************************************************/
	void xtaeDecryptBuffer(uint8_t *buffer, uint16_t length) {
	
		uint16_t i;
		uint8_t rounds;
		uint32_t delta, sum, v0, v1;


		// walk through the buffer in steps of eight bytes (64 bit)
		for (i = 0; i < length; i += 8) {

			// compose a block of 64 bits into two 32 bit variables
			v0 = ((uint32_t)buffer[i+0] << 24) | ((uint32_t)buffer[i+1] << 16) | ((uint32_t)buffer[i+2] << 8) | ((uint32_t)buffer[i+3] << 0);
			v1 = ((uint32_t)buffer[i+4] << 24) | ((uint32_t)buffer[i+5] << 16) | ((uint32_t)buffer[i+6] << 8) | ((uint32_t)buffer[i+7] << 0);

			// decrypt this block
			rounds = 32;
			delta = 0x9E3779B9;
			sum = delta * rounds;

			while(rounds-- > 0) {
			
				v1 -= (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + xtaeKey[(sum >> 11) & 3]);
				sum -= delta;
				v0 -= (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + xtaeKey[sum & 3]);
			}

			// and write the result into the global buffer
			buffer[i + 0] = (uint8_t)((v0 >> 24) & 0xff);
			buffer[i + 1] = (uint8_t)((v0 >> 16) & 0xff);
			buffer[i + 2] = (uint8_t)((v0 >>  8) & 0xff);
			buffer[i + 3] = (uint8_t)((v0 >>  0) & 0xff);
			buffer[i + 4] = (uint8_t)((v1 >> 24) & 0xff);
			buffer[i + 5] = (uint8_t)((v1 >> 16) & 0xff);
			buffer[i + 6] = (uint8_t)((v1 >>  8) & 0xff);
			buffer[i + 7] = (uint8_t)((v1 >>  0) & 0xff);
		}

	}
	
#endif  // USE_ENCRYPTION