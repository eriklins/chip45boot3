/************************************************************************
 * timeout.c
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * initializing the timeout system
 ************************************************************************/


/************************************************************************
 * include header files
 ************************************************************************/
#include <avr/io.h>

#include "configure.h"


// global timeout counter variable
#ifndef WATCHDOG_INT
	uint32_t ulTimeoutCounter;
#endif


/************************************************************************
 * initialize timeout
 * 
 * in : nothing
 * out: nothing
 ************************************************************************/
void timeoutInit(void) {
	
	#ifdef XMEGA  // on Xmegas we do not use the watchdog for timeout, but the RTC clock
	
		CLK.RTCCTRL = (1<<CLK_RTCEN_bp) | (CLK_RTCSRC_RCOSC_gc);  // enable RTC clock and run with 1kHz
		
		#ifdef _AVR_ATxmega256A3B_H_
		
			RTC32.COMP = 3000;  // ~3secs
			RTC32.CTRL = 1;  // no prescaler
			RTC32.CNT = 0;  // clear counter
			
		#else
		
			RTC.COMP = 3000;  // ~3secs
			RTC.CTRL = 1;  // no prescaler
			RTC.CNT = 0;  // clear counter
			
		#endif
		
		// but nevertheless we try to disable a software activated watchdog!
		CCP = 0xD8;  // magic number for protected IO register
		WDT.CTRL = (0<<WDT_ENABLE_bp) | (1<<WDT_CEN_bp);  // set enable bit to zero at the same time with change enable bit
		
	#else
	
		#ifdef WATCHDOG_INT  // we may use the watchdog timer for timeout of RXD synchronization only on modern CPUs with watchdog interrupt
		
			// init watchdog timer
			MCUSR &= ~(1<<WDRF);  // disable the WDRF flag, otherwise disabling WDE won't work
			WDTCSR |= (1<<WDCE) | (1<<WDE);	// timed sequence
			WDTCSR = (1<<WDIE) | (6<<WDP0);  // let the watchdog timer run with 2 seconds timeout and enable watchdog interrupt

		#else
		
			myMCUSR &= ~(1<<WDRF);  // disable the WDRF flag, otherwise disabling WDE won't work
			WDTCR |= (1<<myWDCE) | (1<<WDE);	// timed sequence
			WDTCR = 0;  // disable the watchdog
			ulTimeoutCounter = 0;  // we need a long timeout counter
						
		#endif
		
	#endif

}


/************************************************************************
 * de-initialize timeout
 * 
 * in : nothing
 * out: nothing
 ************************************************************************/
void timeoutDeInit(void) {
	
	#ifdef XMEGA
	
		RTC.CTRL = 0;  // disable RTC timer

	#else
	
		#ifdef WATCHDOG_INT  // watchdog only on newer MCUs, see above
		
			WDTCSR |= (1<<WDCE) | (1<<WDE);  // timed sequence
			WDTCSR = 0;  // clear watchdog activities
			
		#else
		
			// nothing to be done for deinit here...

		#endif
	
	#endif
}



/************************************************************************
 * check if timeout expired
 * 
 * in : nothing
 * out: 1=expired, 0=not expired
 ************************************************************************/
uint8_t timeoutExpired(void) {
	
	#ifdef XMEGA
	
		#ifdef _AVR_ATxmega256A3B_H_
			// Xmega256A3B has slightly different register name...
		
			if(bit_is_set(RTC32.INTFLAGS, RTC32_COMPIF_bp)) {  // on xmegas we use RTC timer for 3sec timeout
				return 1;  // timeout expired
			} else {
				return 0;  // timeout not expired
			}
			
		#else
			// regular Xmegas...
			
			if(bit_is_set(RTC.INTFLAGS, RTC_COMPIF_bp)) {  // on xmegas we use RTC timer for 3sec timeout
				return 1;  // timeout expired
			} else {
				return 0;  // timeout not expired
			}
			
		#endif
		
	#else
	
		#ifdef WATCHDOG_INT
		
			if(bit_is_set(WDTCSR, WDIF)) {  // if we can use watchdog timer, we check it's interrupt bit
				return 1;  // timeout expired
			} else {
				return 0;  // timeout not expired
			}
			
		#else
		
			if( (++ulTimeoutCounter) > (uint32_t)USART_TIMEOUT) {  // if not, we use a simple counter
				return 1;
			} else {
				return 0;
			}

		#endif

	#endif

}