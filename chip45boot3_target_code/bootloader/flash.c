/************************************************************************
 * flash.c
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * functions for flash page fill and write
 * (these are more or less just wrapper functions for the avr-libc functions
 * for convenience reasons and to make porting to other targets than AVR
 * easier. If code space is an issue, these functions may be included in
 * the respective commands in command.c)
 ************************************************************************/


/************************************************************************
 * include header files
 ************************************************************************/
#include <avr/pgmspace.h>
#include "configure.h"

#ifdef XMEGA
	#include "boot.h"
#else
	#include <avr/boot.h>
#endif


/************************************************************************
 * fill the temporary flash page buffer with the received data
 * 
 * in : address, the data buffer and its length
 * out: nothing
 ************************************************************************/
void flashPageFill(uint32_t address, uint8_t *buffer, unsigned int length) {
	
	uint16_t i;
	
	for(i = 0; i < length; i+=2) {

		// write two bytes to the temporary flash page buffer	
		boot_spm_busy_wait();
		boot_page_fill(address+i, (buffer[i+1] << 8) | buffer[i]);
		
	}

}


/************************************************************************
 * program the temporary flash page buffer with the received data
 * 
 * in : address
 * out: nothing
 ************************************************************************/
void flashPageWrite(uint32_t address) {
	
	boot_page_erase(address);  // do a page erase
	boot_spm_busy_wait();  // wait for page erase done
	boot_page_write(address);  // do a page write
	boot_spm_busy_wait();  // wait for write completed
	boot_rww_enable();  // re enable rww section again

}


