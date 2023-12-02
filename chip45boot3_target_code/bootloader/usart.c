/************************************************************************
 * usart.c
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * USART communiction functions
 ************************************************************************/


/************************************************************************
 * include header files
 ************************************************************************/
#include <avr/io.h>
#include <avr/pgmspace.h>

#include "configure.h"
#include "usart.h"


/************************************************************************
 * initialize the USART
 *
 * in : measured baud rate factor with (#define USE_AUTOBAUD) or just regular baud rate value
 * out: nothing
 ************************************************************************/
void usartInit(uint32_t ulDiv) {

	#ifdef XMEGA

		#ifdef USE_AUTOBAUD
			// we calculate a bsel value from the measured RXD low times (make sure to have BSCALE_VALUE defined to -4!)
			uint16_t bsel = (uint16_t)(((100000UL*(ulDiv+1UL))/8UL-100000UL)/6250UL-1UL);
		#else
			// without auto baud we calculate bsel according to the regular equation from xmega data sheet
			uint16_t bsel = ((uint32_t)(1<<(-BSCALE_VALUE)) * (uint32_t)F_CPU) / (uint32_t)8 / ulDiv - (uint32_t)(1<<(-BSCALE_VALUE)) + (uint32_t)1;
		#endif

		// set the baud rate registers
		myUSART.BAUDCTRLA = bsel & 0xff;
		myUSART.BAUDCTRLB = (BSCALE_VALUE<<USART_BSCALE_gp) | (bsel >> 8);

		// set the other USART control registers
		myUSART.CTRLA = 0;  // no interrupts used, we write the default reset value here
		myUSART.CTRLC = (myUSART.CTRLC & ~USART_CHSIZE_gm) | USART_CHSIZE_8BIT_gc;  // 8 data bits, 1 stop bit
		myUSART.CTRLB = (1<<USART_RXEN_bp) | (1<<USART_TXEN_bp) | (1<<USART_CLK2X_bp);	// receiver enable, transmitter enable, usart 2x

		// set TXD to output (necessary on xmegas)
		myUSART_PORT.DIRSET = (1 << myTXD);
		
	#else

		#ifdef USE_AUTOBAUD
			// ulDiv contains the value measured from the auto baud detection
			ulDiv >>= 4;  // divide by 16 to get the actual baud rate divider
			myUBRRH = (uint8_t)(ulDiv>>8);
			myUBRRL = (uint8_t)ulDiv;
		#else
			// ulDiv contains the regular baud rate value (like 115200)
			myUBRRH = (F_CPU / (ulDiv * 16L) - 1) >> 8;
			myUBRRL = (uint8_t)(F_CPU / (ulDiv * 16L) - 1);
		#endif

		myUCSRA = 0;  // no interrupts used, we write the default reset value here
		myUCSRB = (1<<myTXEN) | (1<<myRXEN);  // enable both transmitter and receiver
		myUCSRC = myURSEL | (3<<myUCSZ0);  // 8 data bits, 1 stop bit (myURSEL is defined in atmegaXXX.h in case the device is an old one and reguires URSEL bit to be set)

	#endif
}


/************************************************************************
 * de-initialize the USART, i.e. reset all registers to their initial values
 *
 * in : nothing
 * out: nothing
 ************************************************************************/
void usartDeInit(void) {

	#ifdef XMEGA

		// just in case, we wait until the usart data has been completely shifted out
		loop_until_bit_is_set(myUSART.STATUS, USART_TXCIF_bp);  // wait until data register has been shifted out entirely

		// reset the rxd/txd io pins
		myUSART_PORT.myPINCTRL_RXD = PORT_OPC_TOTEM_gc;  // RXD goes back to regular totem pole mode
		myUSART_PORT.myPINCTRL_TXD = PORT_OPC_TOTEM_gc;  // and TXD as well

		#ifdef RS485
			myRS485_PORT.DIRCLR = (1<<myRS485_DIRPIN);  // RS485 direction pin is input
			myRS485_PORT.OUTCLR = (1<<myRS485_DIRPIN);  // pin is low -> RS485 receiver enable
		#endif

		// reset all USART registers
		myUSART.CTRLA = 0x00;
		myUSART.CTRLB = 0x00;
		myUSART.CTRLC = 0x03;
		myUSART.BAUDCTRLA = 0x00;
		myUSART.BAUDCTRLB = 0x00;


	#else  // no Xmega

		// just in case, we wait until the usart data has been completely shifted out
		loop_until_bit_is_set(myUCSRA, myTXC);  // wait until data register has been shifted out entirely

		TCCR1B = 0;  // stop the timer 1 and set register to reset default
		TCNT1 = 0;  // clear counter register

		// reset the rxd/txd io pins
		myRXDPORT &= ~( (1<<myRXD) | (1<<myTXD) );  // disable the pullups
		
		// reset all USART registers
		myUBRRH = 0;
		myUBRRL = 0;
		myUCSRB = 0;
		myUCSRC = myURSEL | 6;

	#endif  // XMEGA

}


/************************************************************************
 * send a character to the USART
 *
 * in : the character
 * out: nothing
 ************************************************************************/
void usartPutChar(uint8_t c) {

	#ifdef XMEGA
		// wait for transmit buffer empty (in case a previous transmission is not yet completed)
		loop_until_bit_is_set(myUSART.STATUS, USART_DREIF_bp);
		// put character into data register and send
		myUSART.DATA = c;
	#else
		loop_until_bit_is_set(myUCSRA, myUDRE);
		myUDR = c;
	#endif
}


/************************************************************************
 * receive a character from the USART
 *
 * in : nothing
 * out: the character
 ************************************************************************/
uint8_t usartGetChar(uint8_t *c) {

	#ifdef XMEGA

		// wait for data to be received
		if(bit_is_set(myUSART.STATUS, USART_RXCIF_bp)) {
			
			// get the received character
			*c = myUSART.DATA;
	
			// and return success			
			return 1;
			
		} else {
			
			// return fail
			return 0;
		}

	#else

		// wait until character is received
		if(bit_is_set(myUCSRA, myRXC)) {
			
			// return the character
			*c = myUDR;

			// and return success
			return 1;

		} else {
		
			// return fail
			return 0;
		}

	#endif
}

// end of file: usart.c
