/************************************************************************
 * host.h
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * header file for host.c
 ************************************************************************/


#ifndef HOST_H_

	#define HOST_H_


	/************************************************************************
	 * definitions
	 ************************************************************************/
	// set the maximum buffer length, buffer consists of:
	// 1 command byte
	// 64 data bytes (theoretically each byte could be esc'aped in the protocol leading to 128 byte maximum)
	// 2 byte checksum
	#define HOST_BUFFER_LENGTH 131

	// the protocol characters
	#define STX 0x02
	#define ETX 0x03
	#define ESC 0x1B


	/************************************************************************
	 * function prototypes
	 ************************************************************************/
	uint8_t hostInit(uint32_t *);
	void hostDeInit(void);
	int8_t hostBufferReceive(uint8_t *);
	void hostBufferSend(uint8_t *, uint8_t);


#endif /* HOST_H_ */