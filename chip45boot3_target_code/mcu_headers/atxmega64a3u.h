/************************************************************************
 * atxmega128a3u.h
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * header file for ATxmega128A3U
 ************************************************************************/


// make sure to include just once!
#ifndef MCU_HEADER_H_

	#define MCU_HEADER_H_


	/************************************************************************
	 * definitions
	 ************************************************************************/

	// it's an Xmega device
	#define XMEGA


	// definitions for the serial port
	#if(HOST_USART == 0)

		#define myUSART USARTC0
		#define myUSART_PORT PORTC
		#define myPINCTRL_RXD PIN2CTRL
		#define myPINCTRL_TXD PIN3CTRL
		#define myRXD PIN2_bp
		#define myTXD PIN3_bp
		#define myRS485_PORT PORTC
		#define myRS485_DIRPIN PIN1_bp

	#elif(HOST_USART == 1)

		#define myUSART USARTC1
		#define myUSART_PORT PORTC
		#define myPINCTRL_RXD PIN6CTRL
		#define myPINCTRL_TXD PIN7CTRL
		#define myRXD PIN6_bp
		#define myTXD PIN7_bp
		#define myRS485_PORT PORTF
		#define myRS485_DIRPIN PIN5_bp

	#elif(HOST_USART == 2)

		#define myUSART USARTD0
		#define myUSART_PORT PORTD
		#define myPINCTRL_RXD PIN2CTRL
		#define myPINCTRL_TXD PIN3CTRL
		#define myRXD PIN2_bp
		#define myTXD PIN3_bp
		#define myRS485_PORT PORTD
		#define myRS485_DIRPIN PIN1_bp

	#elif(HOST_USART == 3)

		#define myUSART USARTD1
		#define myUSART_PORT PORTD
		#define myPINCTRL_RXD PIN6CTRL
		#define myPINCTRL_TXD PIN7CTRL
		#define myRXD PIN6_bp
		#define myTXD PIN7_bp
		#define myRS485_PORT PORTD
		#define myRS485_DIRPIN PIN5_bp

	#elif(HOST_USART == 4)

		#define myUSART USARTE0
		#define myUSART_PORT PORTE
		#define myPINCTRL_RXD PIN2CTRL
		#define myPINCTRL_TXD PIN3CTRL
		#define myRXD PIN2_bp
		#define myTXD PIN3_bp
		#define myRS485_PORT PORTE
		#define myRS485_DIRPIN PIN1_bp

	#elif(HOST_USART == 5)

		#define myUSART USARTE1
		#define myUSART_PORT PORTE
		#define myPINCTRL_RXD PIN6CTRL
		#define myPINCTRL_TXD PIN7CTRL
		#define myRXD PIN6_bp
		#define myTXD PIN7_bp
		#define myRS485_PORT PORTE
		#define myRS485_DIRPIN PIN5_bp

	#elif(HOST_USART == 6)

		#define myUSART USARTF0
		#define myUSART_PORT PORTF
		#define myPINCTRL_RXD PIN2CTRL
		#define myPINCTRL_TXD PIN3CTRL
		#define myRXD PIN2_bp
		#define myTXD PIN3_bp
		#define myRS485_PORT PORTF
		#define myRS485_DIRPIN PIN1_bp

	#else

		#error Invalid HOST_USART definition in configure.h!

	#endif
	
#endif

