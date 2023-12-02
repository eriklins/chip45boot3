/************************************************************************
 * startup.c
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * this function can handle a possible startup condition, which either
 * starts the bootloader or starts the application
 ************************************************************************/


/************************************************************************
 * include header files
 ************************************************************************/
#include <avr/io.h>

#include "configure.h"


/************************************************************************
 * check a startup condition
 * 
 * in : nothing
 * out: 1 : bootloader should be started
 *      0 : don't start bootloader, jump to application (actually in chip45boot3.c)
 ************************************************************************/
uint8_t startupCheckStartCondition(void) {
	
	// here comes some code, that checks at the very beginning, if the bootloader may be started
	
	// one of the most simple solutions is checking if an IO pin is driven externally to a specific logic level
	
	// another possibility could be a certain value in an EEPROM cell, which was set by the application
	// and only if this cell is set, the bootloader may start
	
	
	// the standard solution with automatic baud rate calibration is, that the RXD pin is being high
	#ifdef USE_AUTOBAUD

		#ifdef XMEGA
		
			// with RS485 we need to switch the transceiver to input before we can check a valid RXD high level
			#ifdef USE_RS485
				myRS485_PORT.DIRSET = (1 << myRS485_DIRPIN);  // RS485 direction pin is output
				myRS485_PORT.OUTCLR = (1 << myRS485_DIRPIN);  // RS485 receive
			#endif
			
			// check if RXD pin (defined in atxmegaXXX.h) is high
			if(bit_is_set(myUSART_PORT.IN, myRXD)) {
				return 1;
			} else {
				return 0;
			}

		#else

			// with RS485 we need to switch the transceiver to input before we can check a valid RXD high level
			#ifdef USE_RS485
				myRS485_DDR |= (1<<myRS485_DIRPIN);  // RS485 dir pin output
				myRS485_PORT &= ~(1<<myRS485_DIRPIN);  // receive
			#endif
			
			// check if RXD pin (defined in atmegaXXX.h) is high
			if(bit_is_set(myRXDPIN, myRXD)) {
				return 1;
			} else {
				return 0;
			}
		#endif
		
	#else  // no autobaud

		// otherwise we return 1 to make sure the bootloader will start
		return 1;
	
	#endif  // USE_AUTOBAUD

}
