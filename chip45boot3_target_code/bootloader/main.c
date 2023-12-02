/************************************************************************
 * chip45boot3.c
 * =============
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader featuring:
 * - USART automatic baud rate detection (comfortable)
 * - lightweight binary protocol (fast)
 * - strong 16 bit CRC checksum (reliable)
 * - optional 128 bit XTAE decryption (safe)
 * - can use standard USART
 * - bootloader can read out application firmware version
 *
 * see http://go.chip45.com/c45b3 for details
 *
 *
 * (c) 2013, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 *
 * COMPILER (TOOLCHAIN)
 * Atmel Studio 6 (Version: 6.1.2674 - Service Pack 1)
 *
 ************************************************************************/


/************************************************************************
 * includes
 ************************************************************************/
#include <avr/pgmspace.h>
#include <avr/interrupt.h>

#include "configure.h"
#include "startup.h"
#include "clock.h"
#include "host.h"
#include "commands.h"
#include "timeout.h"

#ifdef USE_ENCRYPTION
	#include "xtae.h"
#endif

#include "autobaud.h"
#include "usart.h"


/************************************************************************
 * global variables
 ************************************************************************/
uint8_t buffer[HOST_BUFFER_LENGTH] __attribute__ ((section (".noinit")));


/************************************************************************
 * pointer to application at flash start
 ************************************************************************/
void (*startApplication)( void ) = (void *)0x0000;


/************************************************************************
 * the following code is needed to init the device with (-nostartfiles, -nodefaultlibs) custom init routines
 * this suppresses linking of startup code, like clearing of uninitialized variables etc., which saves us code space
 ************************************************************************/
#ifdef XMEGA
	// no memory saving stuff for xmega at the moment
	
#else

	void _jumpMain(void) __attribute__ ((naked)) __attribute__ ((section (".init9")));

	void _jumpMain(void)
	{   
		// set stack to end of RAM
		asm volatile ( ".set __stack, %0" :: "i" (RAMEND) );

		// initialize  stack pointer 
		asm volatile ("ldi r28, lo8(__stack)");
		asm volatile ("ldi r29, hi8(__stack)");
		asm volatile ("out %0, r29" :: "i" (_SFR_IO_ADDR(SPH)) );
		asm volatile ("out %0, r28" :: "i" (_SFR_IO_ADDR(SPL)) );

		// GCC depends on register r1 set to 0
		asm volatile ( "clr __zero_reg__" );   

		// set SREG to 0
		asm volatile ( "out %0, __zero_reg__" :: "i" (_SFR_IO_ADDR(SREG)) );

		// jump to main()
		asm volatile ( "rjmp main");
	}

#endif


/************************************************************************
 * main program, here we go
 *
 * in: nothing
 * out: nothing
 ************************************************************************/
int main(void) {

	uint8_t length;
	uint8_t c;
	
	
	// disable interrupts globally, in case the bootloader was called from an application and not due to a reset
	cli();

	// init the clock system
	clockInit();

	// check bootloader start condition
	if(!startupCheckStartCondition()) {
		
		clockDeInit();  // de-initialize the clock system
		
		#if (BOOT_SECTION_START >= 0x20000)
			EIND = 0x00;  // set Extended Indirect Jump register to zero
		#endif
		startApplication();  // and start the application, i.e. jump to address 0x0000
	}

	// initialize and start the timeout function
	timeoutInit();

	
	// check if we use automatic baud rate calibration
	#ifdef USE_AUTOBAUD

		uint32_t ulbaudrateDivFactor;
		uint8_t ucFlagTimeout;
		
		// init the autobaud system, do autobaud detection and de-init autobaud
		autobaudInit();
		ucFlagTimeout = autobaudDo(&ulbaudrateDivFactor);
		autobaudDeInit();
		
		if(ucFlagTimeout == 1) {  // autobaud timed out

			timeoutDeInit();
			clockDeInit();  // de-initialize the clock system

			#if (BOOT_SECTION_START >= 0x20000)
				EIND = 0x00;  // set Extended Indirect Jump register to zero
			#endif
			startApplication();  // and start the application, i.e. jump to address 0x0000
			
		} else {
			
		// when automatic baud rate calibration succeeded, we set a division factor
			if(hostInit(&ulbaudrateDivFactor) == 1) {
				// everything ok
				
			} else {
				// some error handling or we start the application

				timeoutDeInit();
				clockDeInit();  // de-initialize the clock system

				#if (BOOT_SECTION_START >= 0x20000)
					EIND = 0x00;  // set Extended Indirect Jump register to zero
				#endif
				startApplication();  // and start the application, i.e. jump to address 0x0000
			}
			
		}
	
	#else  // no autobaud

		// without automatic baud rate calibration, we set the baud rate directly
		uint32_t ulbaudrate = MANUAL_BAUDRATE;

		if(hostInit(&ulbaudrate) == 1) {
			// everything ok
			
		} else {

			// some error handling or we start the application

			timeoutDeInit();
			clockDeInit();  // de-initialize the clock system

			#if (BOOT_SECTION_START >= 0x20000)
				EIND = 0x00;  // set Extended Indirect Jump register to zero
			#endif
			startApplication();  // and start the application, i.e. jump to address 0x0000
		}
			
	#endif
		
	// if we use encryption, we need to initialize the XTAE functions
	#ifdef USE_ENCRYPTION
		xtaeInit();
	#endif
	
	// forever...
	for(;;) {
		
		// wait for and receive a frame from the host
		length = hostBufferReceive(buffer);

		// did we really receive something?
		if(length > 0) {

			// if so, we parse the content for known commands and receive back the command byte incl. error bit (MSB)
			c = commandsParseBuffer(buffer, length);
			
			// shall we exit bootloader and start the application?
			if(c == (CMD_START_APPLICATION | 0x80)) {
				
				hostDeInit();  // de-initialize host interface
				timeoutDeInit();
				clockDeInit();  // de-initialize the clock system
				
				// we start the application
				#if (BOOT_SECTION_START >= 0x20000)
					EIND = 0x00;  // set Extended Indirect Jump register to zero
				#endif
				startApplication();  // and start the application, i.e. jump to address 0x0000				
			}
			
		}
		
		
	}

	
	return 0;  // program should never come here...
}


// end of file: main.c
