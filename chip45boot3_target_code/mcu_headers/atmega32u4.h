/************************************************************************
 * atmega32u4.h
 *
 * (c) 2014, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * header file for ATmega32U4
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
	#if (HOST_USART == 1)
	
		#define myUDR   UDR1
		#define myUBRRH UBRR1H
		#define myUBRRL UBRR1L
		#define myUCSRA UCSR1A
		#define myUCSRB UCSR1B
		#define myUCSRC UCSR1C
		#define myUDRE  UDRE1
		#define myRXC   RXC1
		#define myTXC	TXC1
		#define myRXEN  RXEN1
		#define myTXEN  TXEN1
		#define myUCSZ0 UCSZ10
		#define myUCSZ1 UCSZ11
		#define myURSEL 0          // MCU type does not require URSEL bit to be set
		#define myRXDPIN PIND
		#define myRXDPORT PORTD
		#define myRXD PIN2
		#define myTXD PIN3

	#else

		#error Invalid HOST_USART definition in configure.h!

	#endif


#endif

