/************************************************************************
 * commands.h
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * header file for command.c
 ************************************************************************/


#ifndef COMMANDS_H_

	#define COMMANDS_H_


	/************************************************************************
	 * host communication commands
	 * (note: these commands must match the commands in BootloaderTools.cs of the GUI!)
	 ************************************************************************/

	// basic commands
	#define CMD_READ_VERSION_BOOTLOADER 0x11
	#define CMD_READ_VERSION_FIRMWARE   0x12
	#define CMD_READ_VERSION_EEPROM     0x13

	// application commands
	#define CMD_START_APPLICATION       0x18

	// programming commands
	#define CMD_SET_ADDRESS				0x21
	#define CMD_FLASH_WRITE_DATA		0x22
	#define CMD_FLASH_READ_DATA			0x23
	#define CMD_EEPROM_WRITE_DATA		0x24
	#define CMD_EEPROM_READ_DATA		0x25


	/************************************************************************
	 * function prototypes
	 ************************************************************************/
	uint8_t commandsParseBuffer(uint8_t *buffer, unsigned int length);


#endif /* COMMANDS_H_ */
