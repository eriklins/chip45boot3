/************************************************************************
 * atmega168.h
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * header file for ATmega168
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


	// definitions for the serial port
	#if (HOST_USART == 0)
	
		#define myUDR   UDR0
		#define myUBRRH UBRR0H
		#define myUBRRL UBRR0L
		#define myUCSRA UCSR0A
		#define myUCSRB UCSR0B
		#define myUCSRC UCSR0C
		#define myUDRE  UDRE0
		#define myRXC   RXC0
		#define myTXC	TXC0
		#define myRXEN  RXEN0
		#define myTXEN  TXEN0
		#define myUCSZ0 UCSZ00
		#define myUCSZ1 UCSZ01
		#define myURSEL 0          // MCU type does not require URSEL bit to be set
		#define myRXDPIN PIND
		#define myRXDPORT PORTD
		#define myRXD PIN0
		#define myTXD PIN1
						
	#else

		#error Invalid HOST_USART definition in configure.h!

	#endif


#endif

