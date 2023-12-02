/************************************************************************
 * commands.c
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * parsing and executing the commands received from the bootloader PC GUI
 ************************************************************************/


/************************************************************************
 * include header files
 ************************************************************************/
#include <avr/pgmspace.h>
#include <avr/eeprom.h>

#include "host.h"
#include "configure.h"
#include "commands.h"
#include "flash.h"
#include "xtae.h"


/************************************************************************
 * global variables
 ************************************************************************/
// the search pattern for reading firmware version number and it's length
const uint8_t ucFirmwarePattern[] PROGMEM = { "FW_VERSION:" };
#define PATTERNLENGTH 11

// flash buffer and address variables
uint8_t flashBuffer[SPM_PAGESIZE] __attribute__ ((section (".noinit")));  // a buffer as large as SPM_PAGESIZE, since we want to buffer the full page to verify it after writing
uint32_t memoryAddress;
uint16_t flashAddressOffset;

	
/************************************************************************
 * parse a buffer received from the host
 *
 * in : buffer with command and length of buffer
 * out: 0   : no valid command found
 *      !=0 : received command code (see commands.h)
************************************************************************/
uint8_t commandsParseBuffer(uint8_t *buffer, unsigned int length) {
	
	uint8_t answerBuffer[HOST_BUFFER_LENGTH];  // in case new commands are being added to the protocol, check if tmpBuffer size is still sufficient


	// we always send back the receive commands (depending on success, we set the MSB below)
	answerBuffer[0] = buffer[0];

	
	// we check command byte for any known command value
	
	// host wants to read the bootloader version
	if(buffer[0] == CMD_READ_VERSION_BOOTLOADER) {
		
		answerBuffer[0] |= 0x80;  // echo back the command with MSB set
		answerBuffer[1] = VERSION_MAJOR;  // major version number
		answerBuffer[2] = VERSION_MINOR;  // minor version number
		
		hostBufferSend(answerBuffer, 3);  // send back answer


	#ifdef PROVIDE_FIRMWARE_VERSION

		// host wants to read the current installed firmware version
		} else if(buffer[0] == CMD_READ_VERSION_FIRMWARE) {

			uint32_t flashIndex;
			uint8_t patternIndex;
			uint8_t ucPatternFound;
			
			// try to find a version string in the application code
			for (flashIndex = 0; flashIndex < (BOOT_SECTION_START - PATTERNLENGTH); flashIndex++) {

				// clear flags each time before comparing the pattern
				patternIndex = 0;
				ucPatternFound = 1;
			
				// loop through the pattern string and see if it matches
				do {
							
					#ifdef PGM_FAR
						// since a pointer in avr-gcc is a 16 bit value and pgm_read_byte_far expects a 32 bit value, we need to add some offset to get a 32 bit address inside bootloader memory
						if (pgm_read_byte_far(flashIndex + patternIndex) != pgm_read_byte_far((uint32_t)&(ucFirmwarePattern[patternIndex]))) {
					#else
						if (pgm_read_byte(flashIndex + patternIndex) != pgm_read_byte(&(ucFirmwarePattern[patternIndex]))) {
					#endif
							ucPatternFound = 0;  // clear flag, pattern doesn't match
							break;
						}
				} while(++patternIndex < PATTERNLENGTH);
			
				// if pattern was found, we can read out the version numbers
				if (ucPatternFound == 1) {

					flashIndex += PATTERNLENGTH;  // adjust index to right behind the pattern
					#ifdef PGM_FAR
						answerBuffer[1] = (pgm_read_byte_far(flashIndex+0) - '0') * 10 + (pgm_read_byte_far(flashIndex+1) - '0');
						answerBuffer[2] = (pgm_read_byte_far(flashIndex+3) - '0') * 10 + (pgm_read_byte_far(flashIndex+4) - '0');
					#else
						answerBuffer[1] = (pgm_read_byte(flashIndex+0) - '0') * 10 + (pgm_read_byte(flashIndex+1) - '0');
						answerBuffer[2] = (pgm_read_byte(flashIndex+3) - '0') * 10 + (pgm_read_byte(flashIndex+4) - '0');
					#endif
					answerBuffer[0] |= 0x80;  // set MSB in command byte to indicate success
					break;
				}
			}
		
			// we send back the answer to the host
			hostBufferSend(answerBuffer, 3);
		
	#endif

	// bootloader should be exited and application firmware should be started
	// (note: the actual start happens in chip45boot3.c, since we first need to call some de-init functions)
	} else if(buffer[0] == CMD_START_APPLICATION) {
		
		answerBuffer[0] |= 0x80;  // echo back the command with MSB set
		
		hostBufferSend(answerBuffer, 1);  // send back answer


	// set the memory start address for the upcoming flash/eeprom write/read commands
	} else if(buffer[0] == CMD_SET_ADDRESS) {
		
		// get flash address from the buffer
		memoryAddress = ((uint32_t)buffer[1] << 24) | ((uint32_t)buffer[2] << 16) | ((uint32_t)buffer[3] << 8) | ((uint32_t)buffer[4] << 0);

		// clear offset
		flashAddressOffset = 0;
			
		// check if address is within valid range
		if(memoryAddress < BOOT_SECTION_START) {
			answerBuffer[0] |= 0x80;  // we set MSB in command byte to indicate success
		}

		// we send back the answer to the host
		hostBufferSend(answerBuffer, 1);
		
	
	// writing of flash memory can be enabled/disabled in configure.h to save code space (or as -D... in project setting -> toolchain -> compiler -> symbols)
	#ifdef PROVIDE_FLASH_WRITE
	
		// host sends data for flash programming
		} else if(buffer[0] == CMD_FLASH_WRITE_DATA) {
		
			uint16_t i = 0;
			uint8_t flagWriteOk;

			++buffer;  // skip the command byte
			--length;  // correct length

			// transfer the received data into our flash buffer (we keep it there until a full flash page of SPM_PAGESIZE has been received)
			do {
				flashBuffer[flashAddressOffset + i] = buffer[i];
			} while(++i < length);
		
			// adjust address offset
			flashAddressOffset += length;
			
			// check if the address is still within the application memory range
			if( (memoryAddress + flashAddressOffset) > (uint32_t)BOOT_SECTION_START) {
				
				flagWriteOk = 0;  // if not, we clear that flag to indicate an error
				
			} else {  // address is valid below bootblock
		
				flagWriteOk = 1;  // preset ok flag

				// check if the flash page is full or if we received an empty data packet to force writing of last page
				if( (flashAddressOffset >= SPM_PAGESIZE) || (length == 0) ) {
			
					// if we received XTAE encrypted flash data, we need to decrypt it first
					#ifdef USE_ENCRYPTION
						xtaeDecryptBuffer(flashBuffer, flashAddressOffset);  // decrypt the buffer, see xtae.c for details
					#endif
			
					// then we fill the page write buffer, write the buffer and verify the actual flash content against our buffer
					flashPageFill(memoryAddress, flashBuffer, flashAddressOffset);  // fill page buffer with new data bytes (flashAddressOffset contains the number of bytes here)
					flashPageWrite(memoryAddress);  // write the flash page
				
					// verify the flash content against buffer
					for(i = 0; i < flashAddressOffset; ++i) {
						#ifdef PGM_FAR
							if(flashBuffer[i] != pgm_read_byte_far(memoryAddress+i)) {
						#else
							if(flashBuffer[i] != pgm_read_byte(memoryAddress+i)) {
						#endif
							flagWriteOk = 0;  // verify error, clear OK flag!
							break;
						}
					}

					memoryAddress += flashAddressOffset;  // add the offset to the address
					flashAddressOffset = 0;  // clear offset
				}
			}
			
			// check if we had a verify error or the address was outside valid application memory
			if(flagWriteOk == 1) {
				answerBuffer[0] |= 0x80;  // we set MSB in command byte to indicate success
			} else {
				// if there was a verify error, we send back the original command byte, which tells the host there was some error
			}

			// we send back the answer to the host
			hostBufferSend(answerBuffer, 1);
			
	#endif  // COMMAND_FLASH_WRITE	
		
		
	// reading of flash memory can be enabled/disabled in configure.h to save code space (or as -D... in projet setting -> toolchain -> compiler -> symbols)
	// (note: for security reasons flash reading should be disabled if the bootloader is used with encrypted transmission!)
	#ifdef PROVIDE_FLASH_READ
	
		// host wants to read a 64 byte packet of flash data
		// (note: depending of address, we might not be able to read full 64 bytes, if so, we just send less, the host takes care of this)
		} else if(buffer[0] == CMD_FLASH_READ_DATA) {

			uint8_t length = 0;  // clear buffer length counter
			uint8_t count = buffer[1];  // get number of bytes to read from buffer
		
			// maximum number of bytes to read is 64, due to buffer size and CRC constraints
			if(count > 64) {
				count = 64;  // crop to 64 bytes, if larger number was specified
			}

			// set MSB in command byte to indicates success
			answerBuffer[length++] |= 0x80;

			// read 64 bytes from flash and put into buffer
			while(count--) {
			
				// check if current address is still below bootloader address
				if(memoryAddress < BOOT_SECTION_START) {

					#ifdef PGM_FAR
						answerBuffer[length++] = pgm_read_byte_far(memoryAddress++);
					#else
						answerBuffer[length++] = pgm_read_byte((uint16_t)memoryAddress++);
					#endif
				
				} else {

					break;  // if address exceeded application area, we exit the loop
				}

			}

			// we send back the answer to the host
			hostBufferSend(answerBuffer, length);
			
	#endif  // COMMAND_FLASH_READ
	
	
	// writing of eeprom memory can be enabled/disabled in configure.h to save code space (or as -D... in projet setting -> toolchain -> compiler -> symbols)
	#ifdef PROVIDE_EEPROM_WRITE  // ALLOW_EEPROM_WRITE
	
		// host sends data for eeprom programming
		} else if(buffer[0] == CMD_EEPROM_WRITE_DATA) {
		
			uint16_t i;
			uint8_t flagWriteOk = 1;

			++buffer;  // skip the command byte
			--length;  // correct length

			// write the received data to eeprom (we simply write this byte by byte
			eeprom_busy_wait();
			eeprom_write_block(buffer, (void *)memoryAddress, length);
		
			// verify the written data
			for(i = 0; i < length; ++i) {
			
				eeprom_busy_wait();
				if(eeprom_read_byte((uint8_t *)((uint16_t)memoryAddress + i)) != buffer[i]) {
					flagWriteOk = 0;
					break;
				}
			}
		
			// increase memory address
			memoryAddress += length;

			// check if we had a verify error
			if(flagWriteOk == 1) {
				answerBuffer[0] |= 0x80;  // we set MSB in command byte to indicate success
			} else {
				// if there was a verify error, we send back the original command byte, which tells the host there was some error
			}

			// we send back the answer to the host
			hostBufferSend(answerBuffer, 1);
			
	#endif  // COMMAND_EEPROM_WRITE
			
			
	// reading of eeprom memory can be enabled/disabled in configure.h to save code space (or as -D... in projet setting -> toolchain -> compiler -> symbols)
	#ifdef PROVIDE_EEPROM_READ
	
		// host wants to read a 64 byte packet of eeprom data
		} else if(buffer[0] == CMD_EEPROM_READ_DATA) {

			uint8_t length = 0;
			uint8_t count = buffer[1];  // get number of bytes to read from buffer
	
			// maximum number of bytes to read is 64, due to buffer size and CRC constraints
			if(count > 64) {
				count = 64;  // crop to 64 bytes, if larger number was specified
			}

			// set MSB in command byte to indicates success
			answerBuffer[length++] |= 0x80;

			// read 64 bytes from flash and put into answer buffer
			while(count--) {
		
				// check if current address is inside EEPROM address range
				if(memoryAddress <= E2END) {
					answerBuffer[length++] = eeprom_read_byte((uint8_t *)((uint16_t)(memoryAddress++)));
				} else {
					break;  // if address exceeded eeprom space, we exit loop
				}

			}

			// we send back the answer to the host
			hostBufferSend(answerBuffer, length);
			
	#endif // COMMAND_EEPROM_READ


	// didn't find a valid command
	} else {

		return 0;  // error
	}


	// found a valid command and did execute it
	return answerBuffer[0];  // we return the command code
}
