/************************************************************************
 * atxmega16a4_32a4.h
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * header file for ATxmega16A4, 32A4
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
	#ifdef HOST_USART

		#if(HOST_USART == 0)

			#define myUSART USARTC0
			#define myUSART_PORT PORTC
			#define myPINCTRL_RXD PIN2CTRL
			#define myPINCTRL_TXD PIN3CTRL
			#define myRXD PIN2_bp
			#define myTXD PIN3_bp
	
		#elif(HOST_USART == 2)
	
			#define myUSART USARTD0
			#define myUSART_PORT PORTD
			#define myPINCTRL_RXD PIN2CTRL
			#define myPINCTRL_TXD PIN3CTRL
			#define myRXD PIN2_bp
			#define myTXD PIN3_bp
	
		#elif(HOST_USART == 4)
	
			#define myUSART USARTE0
			#define myUSART_PORT PORTE
			#define myPINCTRL_RXD PIN2CTRL
			#define myPINCTRL_TXD PIN3CTRL
			#define myRXD PIN2_bp
			#define myTXD PIN3_bp
	
		#elif(HOST_USART == 6)
	
			#define myUSART USARTF0
			#define myUSART_PORT PORTF
			#define myPINCTRL_RXD PIN2CTRL
			#define myPINCTRL_TXD PIN3CTRL
			#define myRXD PIN2_bp
			#define myTXD PIN3_bp
	
		#else

			#error Invalid HOST_USART definition in configure.h!

		#endif

	#endif  // HOST_USART
	
#endif

