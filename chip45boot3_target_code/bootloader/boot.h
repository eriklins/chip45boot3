/************************************************************************
 * boot.h
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2013, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * header file for boot.c
 ************************************************************************/

// make sure to include just once!
#ifndef _BOOT_H_
#define _BOOT_H_

// include the spm function of AVR1605 application note
#include "sp_driver.h"


// prototypes

#ifdef WORKAROUND  // on A3 devices with die revision B a workaround is required
	void PMIC_SetVectorLocationToBoot( void );
	void PMIC_SetVectorLocationToApplication( void );
#endif

	void boot_page_fill(uint32_t ulAddress, uint16_t usData);
	void boot_page_erase(uint32_t ulAddress);  // do a page erase
	void boot_spm_busy_wait();  // wait for page erase done
	void boot_page_write(uint32_t ulAddress);  // do a page write
	void boot_rww_enable();  // reenable rww section again

#endif


// end of file: boot.h
