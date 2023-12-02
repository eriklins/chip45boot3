/************************************************************************
 * usart.h
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * haeder file for usart.c
 ************************************************************************/


/************************************************************************
 * include header files
 ************************************************************************/
#include <stdint.h>


// make sure to include just once!
#ifndef _USART_

	#define _USART_


	/************************************************************************
	 * macro definitions
	 ************************************************************************/
	#ifdef XMEGA
		// define a BSCALE value (must be set to 4 when using autobaud!!!)
		#define BSCALE_VALUE (-4)
	#endif
		

	/************************************************************************
	 * function prototypes
	 ************************************************************************/
	void usartInit(uint32_t);
	void usartDeInit(void);
	
	void usartPutChar(uint8_t);
	uint8_t usartGetChar(uint8_t *);


#endif
/************************************************************************/
/* end of file                                                                     */
/************************************************************************/
