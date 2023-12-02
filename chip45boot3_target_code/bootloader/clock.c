/************************************************************************
 * clock.c
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * initializing the clock system (currently only relevant for Xmegas)
 ************************************************************************/


/************************************************************************
 * include header files
 ************************************************************************/
#include <avr/io.h>

#include "configure.h"


/************************************************************************
 * initialize MCU clock subsystem
 * 
 * in : nothing
 * out: nothing
 ************************************************************************/
void clockInit(void) {
	
	#ifdef XMEGA
	
		// change the XMega Clock. The xmegas starts with 2MHz internal as default

		// frequency range for external oscillator is not used, we set to default reset value
		OSC.XOSCCTRL = 0;

		// switch to internal 2MHz oscillator, in case we come from application
		OSC.CTRL = (1<<OSC_RC2MEN_bp) | (1<<OSC_RC32KEN_bp);  // also enable the 32kHz RTC oscillator

		// configure the PLL
		// clock source - internal 2MHz RC clock source / PLL faktor x8
		OSC.PLLCTRL = (0b00<<OSC_PLLSRC_gp) | (8<<OSC_PLLFAC_gp);
		// enable the PLL
		OSC.CTRL |= (1<<OSC_PLLEN_bp);	// enable the PLL

		// wait for the PLL to lock on the external oscillator
		while ((OSC.STATUS & (1<<OSC_PLLRDY_bp)) == 0);

		// switch the system clock from the 2MHz internal to the PLL created 16MHz;
		CCP = CCP_IOREG_gc;						// enable the change of a protected register
		CLK.CTRL = (0b100<<CLK_SCLKSEL_gp);		// PLL - Phase Locked Loop
		
	#else
	
		// For normal AVRs we do not do any clock settings, since there is not much to do in software.
		// Clock setting is done mainly by fusebits.
		// (note: if you have to use a target with CKDIV8 fuse set, you should consider to inrease clock
		// during bootloader to allow for higher baud rates with lower baud rate deviation)

	#endif
}


/************************************************************************
 * de-initialize MCU clock subsystem, i.e. reset used registers to their defaults
 * 
 * in : nothing
 * out: nothing
 ************************************************************************/
void clockDeInit(void) {
	
	#ifdef XMEGA
	
		RTC.CTRL = 0;  // disable RTC timer

		// change the XMega Clock back to 2MHz
		CCP = CCP_IOREG_gc;  // enable the change of a protected register
		CLK.CTRL = 0;  // set clock source to 2MHz RC oscillator
		asm volatile ("nop");
		OSC.CTRL = (1<<OSC_RC2MEN_bp);  // 2MHz RC and no pll
		OSC.PLLCTRL = 0;  // PLL factor reset
	
	#else
	
	#endif
}