/************************************************************************
 * autobaud.c
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * functions for automatic baud rate adjustment for usart communication
 ************************************************************************/

/************************************************************************
 * include header files
 ************************************************************************/
#include <avr/io.h>

#include "configure.h"
#include "timeout.h"


// we need these functions only if USE_AUTOBAUD is defined in configure.h
#ifdef USE_AUTOBAUD


	/************************************************************************
	 * initialize io pins and timer registers for autobaud detection
	 * 
	 * in : nothing
	 * out: nothing
	 ************************************************************************/
	void autobaudInit(void) {

		// activate the internal IO pin pullup on the RXD pin to make it less sensitive for disturbances in case the pin is floating in the final application
		// accidental high->low transistions after reset might activate the bootloader unwantedly
		// also enable the TXD pullup, this avoids reading of a wrong bootloader version from the PC application due to interpreting low TXD as startbits and so on
		#ifdef XMEGA

			// both RXD and TXD get a pullup
			myUSART_PORT.myPINCTRL_RXD = PORT_OPC_PULLUP_gc;
			myUSART_PORT.myPINCTRL_TXD = PORT_OPC_PULLUP_gc;
		
			#ifdef USE_RS485
				myRS485_PORT.DIRSET = (1 << myRS485_DIRPIN);  // RS485 direction pin is output
				myRS485_PORT.OUTCLR = (1 << myRS485_DIRPIN);  // pin is low -> RS485 receiver enable
			#endif

		#else  // no XMEGA

			// both RXD and TXD get a pullup
			myRXDPORT |= ( (1<<myRXD) | (1<<myTXD) );
	
			#ifdef USE_RS485
				myRS485_DDR |= (1<<myRS485_DIRPIN);  // RS485 dir pin output
				myRS485_PORT &= ~(1<<myRS485_DIRPIN);  // receive
			#endif

		#endif  // XMEGA


		#ifdef XMEGA

			// on xmegas we use timer 0 for baud rate measurement
			TCC0.CTRLA = 1;  // turn timer on and no prescaler
			TCC0.CTRLB = TC_WGMODE_NORMAL_gc;  // normal mode
			TCC0.CTRLC = 0;  // no compare outputs
			TCC0.CTRLD = 0;  // we do not use events
			TCC0.CTRLE = 0;  // no byte mode

		#else  // no Xmega

			// init timer 1 (should be available on almost any AVR)
			// no prescaler, since we want to measure the RXD timing precisely
			TCCR1A = 0;  // normal mode
			TCCR1B = 1;

		#endif  // XMEGA
	}


	/************************************************************************
	 * reset all registers used for autobaud to their initial values
	 * 
	 * in : nothing
	 * out: nothing
	 ************************************************************************/
	void autobaudDeInit(void) {
	
		#ifdef XMEGA

			TCC0.CTRLA = 0;  // stop timer
			TCC0.CTRLB = 0;
			TCC0.CNT = 0;  // clear counter register
			myUSART_PORT.myPINCTRL_RXD = PORT_OPC_TOTEM_gc;  // RXD goes back to regular totem pole mode
			myUSART_PORT.myPINCTRL_TXD = PORT_OPC_TOTEM_gc;  // and TXD as well
		
			#ifdef USE_RS485
				myRS485_PORT.DIRCLR = (1 << myRS485_DIRPIN);  // RS485 direction pin is input
				myRS485_PORT.OUTCLR = (1 << myRS485_DIRPIN);  // pin is low -> RS485 receiver enable
			#endif

		#else

			TCCR1B = 0;  // stop the timer 1 and set register to reset default
			TCNT1 = 0;  // clear counter register
			myRXDPORT &= ~( (1<<myRXD) | (1<<myTXD) );  // disable the pullups
		
			#ifdef USE_RS485
				myRS485_DDR &= ~(1<<myRS485_DIRPIN);  // RS485 dir pin input
				myRS485_PORT &= ~(1<<myRS485_DIRPIN);  // no pullup
			#endif
		
			myUBRRH = 0;
			myUBRRL = 0;
			myUCSRB = 0;
			myUCSRC = myURSEL | 6;
	
		#endif

	}

	/************************************************************************
	 * actually do the automatic baud rate calibration
	 *
	 * This function assumes that 'U' characters are being sent from the host.
	 * A 'U' consists of just alternating 1 and 0 bits, hence each low level duration is a measure for the baud rate.
	 * 
	 * in : pointer to the baud rate division factor variable, which we use later for USART initialization
	 * out: 1 : timeout
	 *      0 : baud rate measured
	 ************************************************************************/
	uint8_t autobaudDo(uint32_t *ulbaudrateDivFactor) {
	
		// we try to measure the baud rate!
		// loop 17 times (i.e. ~four 'U' characters) and measure the low time between falling and rising edges
		// we drop the first measurement, so we divide by 16 later
		uint8_t ucTmp = 17;
		*ulbaudrateDivFactor = 0;  // clear variable
		uint8_t ucFlagTimeout = 0;  // clear timeout flag

		#ifdef XMEGA

			// loop 17 times
			do {

				// wait for RXD going low
				// (note: RXD pin is defined in your desired atmegaXXX.h header file!)
				while(bit_is_set(myUSART_PORT.IN, myRXD)) {
				
					// check if timeout expired
					if(timeoutExpired()) {
						ucFlagTimeout = 1;  // set timeout flag
						break;  // exit loop
					}
				}

				// clear timer counter register
				TCC0.CNT = 0;

				// wait for RXD going high
				while(bit_is_clear(myUSART_PORT.IN, myRXD)) {
				
					// check if timeout expired
					if(timeoutExpired()) {
						ucFlagTimeout = 1;  // set timeout flag
						break;  // exit loop
					}
				}

				// we ignore the first measurement, since it might have started somewhere within the low phase and not at the actual edge
				if(ucTmp < 17) {
					*ulbaudrateDivFactor += TCC0.CNT;  // add timer counter value to variable
				}
								
			} while( (--ucTmp) && (ucFlagTimeout == 0) );  // timeout will terminate the loop also before the 17th

		#else  // no Xmega

			do {

				// wait for RXD going low
				// (note: RXD pin is defined in your desired atmegaXXX.h header file!)
				while(bit_is_set(myRXDPIN, myRXD)) {
				
					// check if timeout expired
					if(timeoutExpired()) {
						ucFlagTimeout = 1;  // set timeout flag
						break;  // exit loop
					}
				}

				// clear timer
				TCNT1 = 0;
					
						
				// wait for RXD going high
				while(bit_is_clear(myRXDPIN, myRXD)) {
							
								
					// check if timeout expired
					if(timeoutExpired()) {
						ucFlagTimeout = 1;  // set timeout flag
						break;  // exit loop
					}
				}

				// we ignore the first measurement, since it might have started somewhere within the low phase and not at the actual edge
				if(ucTmp < 17) {

					*ulbaudrateDivFactor += TCNT1;  // add timer value to variable
				}
								
			} while( (--ucTmp) && (ucFlagTimeout == 0) );  // timeout will terminate the loop also before the 17th
	
		#endif

		// divide the division factor by 16 due to averaging over 16 low periods above
		*ulbaudrateDivFactor >>= 4;
	
		// we return the timeout flag
		return ucFlagTimeout;
	}

#endif  // USE_AUTOBAUD
