/************************************************************************
 * atmega8.h
 *
 * (c) 2014, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * header file for ATmega8
 ************************************************************************/


// make sure to include just once!
#ifndef MCU_HEADER_H_

	#define MCU_HEADER_H_


	/************************************************************************
	 * definitions
	 ************************************************************************/

	// if the device provides to use the watchdog as interrupt timer, we define this
	// if not, comment the line out and a software delay will be used for timeout functions
	//#define WATCHDOG_INT

	// timeout for startup
	#define USART_TIMEOUT 1600000  // ~3 seconds at 8MHz
	
	// definitions for the serial port
	#if (HOST_USART == 0)
	
		#define myMCUSR MCUCSR
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
		#define myURSEL (1<<URSEL)          // old MCU type requires URSEL bit to be set
		#define myRXDPIN PIND
		#define myRXDPORT PORTD
		#define myRXD PIN0
		#define myTXD PIN1
						
	#else

		#error Invalid HOST_USART definition in configure.h!

	#endif


#endif

