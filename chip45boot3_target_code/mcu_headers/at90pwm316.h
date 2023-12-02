/************************************************************************
 * at90pwm316.h
 *
 * (c) 2014, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * header file for AT90PWM316
 ************************************************************************/


// make sure to include just once!
#ifndef MCU_HEADER_H_

	#define MCU_HEADER_H_


	/************************************************************************
	 * definitions
	 ************************************************************************/

	// if the device provides to use the watchdog as interrupt timer, we define this
	// if not, comment the line out and a software delay will be used for timeout functions
	#define WATCHDOG_INT

	// timeout for startup
	#define USART_TIMEOUT 1600000  // ~3 seconds at 8MHz
	
	// definitions for the serial port
	#if (HOST_USART == 0)
	
		#define myMCUSR MCUSR
		#define myWDCE WDCE
		#define myUDR   UDR
		#define myUBRRH UBRRH
		#define myUBRRL UBRRL
		#define myUCSRA UCSRA
		#define myUCSRB UCSRB
		#define myUCSRC UCSRC
		#define myUDRE  UDRE
		#define myRXC   RXC
		#define myTXC	TXC
		#define myRXEN  RXEN
		#define myTXEN  TXEN
		#define myUCSZ0 UCSZ0
		#define myUCSZ1 UCSZ1
		#define myURSEL 0          // MCU type does not require URSEL bit to be set
		#define myRXDPIN PIND
		#define myRXDPORT PORTD
		#define myRXD PIN4
		#define myTXD PIN3

	#else

		#error Invalid HOST_USART definition in configure.h!

	#endif


#endif

