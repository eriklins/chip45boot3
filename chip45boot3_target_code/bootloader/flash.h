/************************************************************************
 * flash.h
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * header file for flash.c
 ************************************************************************/


#ifndef FLASH_H_

	#define FLASH_H_


	/************************************************************************
	 * function prototypes
	 ************************************************************************/
	void flashPageFill(uint32_t address, uint8_t *buffer, unsigned int length);
	void flashPageWrite(uint32_t address);


#endif /* FLASH_H_ */