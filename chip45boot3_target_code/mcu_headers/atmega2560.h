/************************************************************************
 * atmega2560.h
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * header file for ATmega2560
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


	// define if we must use pgm_read_byte_far for reading from flash memory. On target with >64k flash, we need to use the _far function!
	#define PGM_FAR	


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
		#define myRXDPIN PINE
		#define myRXDPORT PORTE
		#define myRXD PIN0
		#define myTXD PIN1
		
	#elif (HOST_USART == 1)

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
	
	#elif (HOST_USART == 2)

		#define myUDR   UDR2
		#define myUBRRH UBRR2H
		#define myUBRRL UBRR2L
		#define myUCSRA UCSR2A
		#define myUCSRB UCSR2B
		#define myUCSRC UCSR2C
		#define myUDRE  UDRE2
		#define myRXC   RXC2
		#define myTXC	TXC2
		#define myRXEN  RXEN2
		#define myTXEN  TXEN2
		#define myUCSZ0 UCSZ20
		#define myUCSZ1 UCSZ21
		#define myURSEL 0          // MCU type does not require URSEL bit to be set
		#define myRXDPIN PINH
		#define myRXDPORT PORTH
		#define myRXD PIN0
		#define myTXD PIN1
	
	#elif (HOST_USART == 3)

		#define myUDR   UDR3
		#define myUBRRH UBRR3H
		#define myUBRRL UBRR3L
		#define myUCSRA UCSR3A
		#define myUCSRB UCSR3B
		#define myUCSRC UCSR3C
		#define myUDRE  UDRE3
		#define myRXC   RXC3
		#define myTXC	TXC3
		#define myRXEN  RXEN3
		#define myTXEN  TXEN3
		#define myUCSZ0 UCSZ30
		#define myUCSZ1 UCSZ31
		#define myURSEL 0          // MCU type does not require URSEL bit to be set
		#define myRXDPIN PINJ
		#define myRXDPORT PORTJ
		#define myRXD PIN0
		#define myTXD PIN1
	
	#else

		#error Invalid HOST_USART definition in configure.h!

	#endif


#endif

