/************************************************************************
 * mcu.h
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


#ifndef MCU_H_

	#define MCU_H_


	/************************************************************************
	 * older ATmega MCUs
	 ************************************************************************/
	
	#if defined (__AVR_ATmega8__)
											#include "atmega8.h" }
	
	#elif defined (__AVR_ATmega16__)
											#include "atmega16.h"
	
	#elif defined (__AVR_ATmega32__)
											#include "atmega32.h"
	
	
	/************************************************************************
	 * newer ATmega MCUs
	 ************************************************************************/

	#elif defined (__AVR_ATmega88__)
											#include "atmega88.h"
	#elif defined (__AVR_ATmega88A__)
											#include "atmega88.h"
	#elif defined (__AVR_ATmega88P__)
											#include "atmega88.h"

	
	#elif defined (__AVR_ATmega168__)
											#include "atmega168.h"
	#elif defined (__AVR_ATmega168A__)
											#include "atmega168.h"
	#elif defined (__AVR_ATmega168P__)
											#include "atmega168.h"
	
	
	#elif defined (__AVR_ATmega328__)
											#include "atmega328.h"
	#elif defined (__AVR_ATmega328P__)
											#include "atmega328.h"
	
	
	#elif defined (__AVR_ATmega164A__)
											#include "atmega164_324_644_1284.h"
	#elif defined (__AVR_ATmega164P__)
											#include "atmega164_324_644_1284.h"
	#elif defined (__AVR_ATmega164PA__)
											#include "atmega164_324_644_1284.h"
	
	
	#elif defined (__AVR_ATmega324A__)
											#include "atmega164_324_644_1284.h"
	#elif defined (__AVR_ATmega324P__)
											#include "atmega164_324_644_1284.h"
	#elif defined (__AVR_ATmega324PA__)
											#include "atmega164_324_644_1284.h"
	
	
	#elif defined (__AVR_ATmega1284A__)
											#include "atmega164_324_644_1284.h"
	#elif defined (__AVR_ATmega1284P__)
											#include "atmega164_324_644_1284.h"
	#elif defined (__AVR_ATmega1284PA__)
											#include "atmega164_324_644_1284.h"
	
	
	#elif defined (__AVR_ATmega644P__)
											#include "atmega164_324_644_1284.h"
	#elif defined (__AVR_ATmega644PA__)		
											#include "atmega164_324_644_1284.h"
	
	
	#elif defined (__AVR_ATmega128__)
											#include "atmega128.h"
	#elif defined (__AVR_ATmega128A__)
											#include "atmega128.h"


	#elif defined (__AVR_ATmega2560__)
											#include "atmega2560.h"
	#elif defined (__AVR_ATmega2561__)
											#include "atmega2560.h"
	
	/************************************************************************
	 * PWM MCUs
	 ************************************************************************/

	#elif defined (__AVR_AT90PWM316__)
											#include "at90pwm316.h"


	/************************************************************************
	 * USB MCUs
	 ************************************************************************/

	#elif defined (__AVR_ATmega32U4__)
											#include "atmega32u4.h"


	/************************************************************************
	 * Xmega MCUs
	 ************************************************************************/
	
	#elif defined (__AVR_ATxmega128A1__)
											#include "atxmega128a1.h"
	
		
	#elif defined (__AVR_ATxmega128A3__)
											#include "atxmega128a3u.h"
	#elif defined (__AVR_ATxmega128A3U__)
											#include "atxmega128a3u.h"
	
	
	#elif defined (__AVR_ATxmega256A3__)
											#include "atxmega256a3u.h"
	#elif defined (__AVR_ATxmega256A3U__)
											#include "atxmega256a3u.h"

	
	#elif defined (__AVR_ATxmega16A4__)
											#include "atxmega16a4.h"
	#elif defined (__AVR_ATxmega16A4U__)
											#include "atxmega16a4.h"
	#elif defined (__AVR_ATxmega128A4U__)
											#include "atxmega16a4.h"

	#elif defined (__AVR_ATxmega32A4__)
											#include "atxmega32a4.h"
	#elif defined (__AVR_ATxmega32A4U__)
											#include "atxmega32a4.h"

	#elif defined (__AVR_ATxmega64C3__)
											#include "atxmega64c3.h"
	#elif defined (__AVR_ATxmega384C3__)
											#include "atxmega64c3.h"


	#else

		#error Header file for selected MCU not found!
		
	#endif

#endif /* MCU_H_ */
