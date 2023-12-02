/************************************************************************
 * configure.h
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * this is the main configuration header file for the chip45boot3 bootloader
 * note: many of the definitions here can be overriden by -DHOST_USART=... symbol
 * in the Studio 6 project settings (Project -> Properties -> C-Compiler -> Defined Symbols)
 ************************************************************************/


#ifndef CONFIGURE_H_

	#define CONFIGURE_H_


	// include MCU specific definitions	
	#include "mcu.h"


	// bootloader version is V1.0, this version can be read out from the PC GUI
	#define VERSION_MAJOR 1
	#define VERSION_MINOR 0

	
	// check if usart for host communication was defined in project settings
	#ifndef HOST_USART
		#error HOST_USART definition missing in project settings (toolchain -> c-compiler -> symbols)
	#endif


	// check if either USE_AUTOBAUD or MANUAL_BAUDRATE was defined in project settings
	#ifndef USE_AUTOBAUD
		#ifndef MANUAL_BAUDRATE
			#error Either USE_AUTOBAUD or MANUAL_BAUDRATE has to be defined in project settings (toolchain -> c-compiler -> symbols)
		#endif
	#endif

	// if MANUAL_BAUDRATE is used, check if F_CPU is defined (necessary for baud rate register calculation)
	#ifdef MANUAL_BAUDRATE
		#ifndef F_CPU
			#error When using MANUAL_BAUDRATE also F_CPU has to be defined in project settings (toolchain -> c-compiler -> symbols)
		#endif
	#endif
		
	// if target should use RS485 half duplex, a pin for RS485 transceiver direction has to be defined
	#ifdef USE_RS485
		#ifdef XMEGA
			#define myRS485_PORT PORTE
			#define myRS485_DIRPIN PIN1_bp
		#else
			#define myRS485_DDR DDRB
			#define myRS485_PORT PORTB
			#define myRS485_DIRPIN PIN7
		#endif
	#endif

	
	// if encryption should be use, check if the key is defined in Project -> Properties -> C-Compiler -> Defined Symbols)
	// example is here: ENCRYPTION_KEY={1936287828,1750365001,1635014757,2036681573}
	#ifdef USE_ENCRYPTION
		#ifndef ENCRYPTION_KEY
			#error When using encrypted communication, ENCRYPTION_KEY has to be defined in project settings (toolchain -> c-compiler -> symbols).
		#endif
	#endif
	

	// we check what features should be provided by the bootloader
	// (note: some of them may be omitted to save code space, but we issue a warning to let the user know.)
	//#ifndef PROVIDE_FIRMWARE_VERSION
		//#warning Firmware version command not provided (not an error, just to let you know)
	//#endif
	//#ifndef PROVIDE_FLASH_WRITE
		//#warning Flash programming not provided (not an error, just to let you know)
	//#endif
	//#ifndef PROVIDE_FLASH_READ
		//#warning Flash readout not provided (not an error, just to let you know)
	//#endif
	//#ifndef PROVIDE_EEPROM_WRITE
		//#warning Eeprom programming not provided (not an error, just to let you know)
	//#endif
	//#ifndef PROVIDE_EEPROM_READ
		//#warning Eeprom readout not provided (not an error, just to let you know)
	//#endif
	
	
	// the bootloader needs to know it's start address, so we check if it was set in Project -> Properties -> C-Compiler -> Defined Symbols)
	#ifdef XMEGA
		// for the Xmegas, this value should be set in the MCU specific toolchain header file (avr/include/avr/...h)
	#else
		#ifndef BOOT_SECTION_START
			#error BOOT_SECTION_START has to be defined in the project settings (toolchain -> c-compiler -> symbols).
		#endif
	#endif	

	// timeout delay in case we cannot use the watchdog timer (only for old AVRs...)
	// newer atmegas use the watchdog timer, xmegas use the rtc timer
	#ifndef WATCHDOG_INT
		#define USART_TIMEOUT 1600000  // value here is roughly F_CPU/20 for 3sec timeout
	#endif

#endif /* CONFIGURE_H_ */
