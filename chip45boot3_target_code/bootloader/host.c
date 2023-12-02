/************************************************************************
 * host.c
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * functions for host communication
 * these functions abstracts the low level communication functions like
 * usart
 * (note: when adding other host interfaces, the low level functions, like
 * spiGetChar() or similar should go into a separate set of .c/.h files,
 * hence the rest of the bootloader should not need any modifications... 
 * theoretically)
 ************************************************************************/


/************************************************************************
 * include header files
 ************************************************************************/
#include <avr/io.h>

#include "configure.h"

#include "usart.h"

#include "crc.h"
#include "host.h"
#include "timeout.h"


uint8_t flagBroadcast = 0;


/************************************************************************
 * initialize the host communication interface
 * 
 * in : some value as uint32_t reference, e.g. USART baud rate
 * out: nothing
 ************************************************************************/
uint8_t hostInit(uint32_t *parameter) {
	
	uint8_t c = 0;  // character variable

	// initialize the USART
	usartInit(*parameter);
	
	// with RS485 we need to enable the direction pin of the transceiver
	#ifdef USE_RS485
		#ifdef XMEGA
			myRS485_PORT.DIRSET = (1 << myRS485_DIRPIN);  // RS485 direction pin is output
			myRS485_PORT.OUTCLR = (1 << myRS485_DIRPIN);  // RS485 receive
		#else
			myRS485_DDR |= (1<<myRS485_DIRPIN);  // RS485 direction pin is output
			myRS485_PORT &= ~(1<<myRS485_DIRPIN);  // RS485 receive
		#endif
	#endif

	// check if baud rate was measured correctly (i.e. we can receive a 'U' at the calculated baud rate)
	do {
		usartGetChar(&c);
		if(timeoutExpired()) {
			return 0;  // if timeout expired, we exit here and application should be started in main program
		}
	} while(c != 'U');

	// with RS485 we have to wait for one other character than a 'U' before we may send our identifier
	// (note: the GUI has to be set to RS485 either!)
	#ifdef USE_RS485
		
		// wait for some other character than 'U'
		do {
			while(!usartGetChar(&c));  // wait for character
		} while(c != 0x80);
			
		// RS485 Addressing...
		do {
			while(!usartGetChar(&c));  // wait for character
				
			// check for address match (valid addresses are 1-127 (0x01-0x7F)
			if(c == RS485_ADDR) {
				#ifdef XMEGA
					myRS485_PORT.OUTSET = (1<<myRS485_DIRPIN);  // transmit
					usartPutChar(c);
					loop_until_bit_is_set(myUSART.STATUS, USART_TXCIF_bp);
					myRS485_PORT.OUTCLR = (1<<myRS485_DIRPIN);  // receive
				#else									
					myRS485_PORT |= ((1<<myRS485_DIRPIN);  // transmit
					usartPutChar(c);
					loop_until_bit_is_set(myUCSRA, myTXC);
					myRS485_PORT &= ~((1<<myRS485_DIRPIN);  // receive
				#endif
			}
		} while( c!= 0x81);
						
		#ifdef XMEGA
			myRS485_PORT.OUTSET = (1 << myRS485_DIRPIN);  // RS485 transmit
		#else
			myRS485_PORT |= (1<<myRS485_DIRPIN);  // transmit
		#endif

	#else  // no RS485
		
		// and send a short identifier (wo do not use hostBufferSend() here, since we don't want a CRC here)
		usartPutChar(STX);
		usartPutChar('c');
		usartPutChar('4');
		usartPutChar('5');
		usartPutChar('b');
		usartPutChar('3');
		usartPutChar(ETX);
	#endif
		
	// everything fine, we return one
	return 1;
}


/************************************************************************
 * de-initialize the host communication interface, i.e. reset all registers
 * to their reset defaults
 * 
 * in : nothing
 * out: nothing
 ************************************************************************/
void hostDeInit(void) {

	// de-initialize the USART
	usartDeInit();
		
	#ifdef USE_RS485
		#ifdef XMEGA
			myRS485_PORT.DIRCLR = (1 << myRS485_DIRPIN);  // reset RS485 direction pin to input
			myRS485_PORT.OUTCLR = (1 << myRS485_DIRPIN);  // low
		#else
			myRS485_DDR &= ~((1<<myRS485_DIRPIN);  // RS485 dir pin input
			myRS485_PORT &= ~((1<<myRS485_DIRPIN);  // no pullup
		#endif
	#endif
		
}

/************************************************************************
 * receive a buffer from the host (but do not wait until full reception,
 * i.e. function has to be called periodically until last character ETX was received
 *
 * if last character ETX was received, protocol characters are removed 
 * from the buffer and crc check is performed and length of buffer is returned
 * 
 * in : pointer to the buffer
 * out: 0 : buffer not completely received
 *      1 : buffer completely received
 ************************************************************************/
int8_t hostBufferReceive(uint8_t *buffer) {
	
	crc_t crc;  // checksum variable (see crc.c for details on checksum)
	uint8_t c;  // receive character
	static uint8_t count;  // byte count
	static uint8_t esc_offset;  // esc'aped offset


	// with RS485 we set direction to input first
	#ifdef USE_RS485
		#ifdef XMEGA
			// wait until transmit complete flag is set, i.e. the last byte was completely shifted out
			loop_until_bit_is_set(myUSART.STATUS, USART_TXCIF_bp);
			myRS485_PORT.OUTCLR = (1 << myRS485_DIRPIN);  // RS485 receive
		#else
			// wait until transmit complete flag is set, i.e. the last byte was completely shifted out
			loop_until_bit_is_set(myUCSRA, myTXC);
			myRS485_PORT &= ~(1<<myRS485_DIRPIN);  // receive
		#endif
	#endif	

	// initially clear a flag
	uint8_t flagBufferReceived = 0;
		
	// check if some character is available from USART (if so, character will be in c)
	if(usartGetChar(&c) != 0) {
	
		if(c == STX) {  // STX character starts a transmission
			count = 0;  // set byte count to zero

		} else if(c == ESC) {  // STX, ETX and ESC character needs to be esc'aped when in normal data bytes
			esc_offset = 0x80;  // esc'aped means, the next character has an offset of 0x80
					
		} else if(c == ETX) {  // ETX character ends a transmission
			flagBufferReceived = 1;  // buffer received
					
		} else {  // a normal data byte has been received
			buffer[count++] = c - esc_offset;  // write data byte into the buffer and increment count
			esc_offset = 0x00;  // clear offset, esc'aped is only valid for one character
		}
			
		// check if we received an ETX (i.e. frame received completely) or if the receive buffer is full (should not happen, the GUI has to take care of this!)
		if( (flagBufferReceived == 1) || (count >= HOST_BUFFER_LENGTH) ) {

			// compute the crc over the buffer minus the last two bytes
			crc = crcCompute(buffer, count-2);
							
			// check for crc error
			if( (buffer[count-2] == (uint8_t)((crc >> 8) & 0xff) ) && (buffer[count-1] == (uint8_t)(crc & 0xff)) ) {
					
				#ifdef USE_RS485
					flagBroadcast = 0;
					// if we didn't receive our address and didn't receive broadcast address, we return zero, i.e. command will not be ignored
					if( (buffer[0] != RS485_ADDR) && (buffer[0] != 0x00) ){
						return 0;
					}
					// if broadcast, we set flag to suppress sending back any answer (since all target would conflict on the bus)
					if(buffer[0] == 0x00) {
						flagBroadcast = 1;
					}
					for (c = 0; c < count; ++c)
					{
						buffer[c] = buffer[c + 1];
					}
					--count;
				#endif
				
				// no error, so we return the number of bytes received minus two, since the checksum in not interesting for the other code parts
				return (count - 2);
								
			} else {
								
				// send a crc-error frame to the host
				hostBufferSend((uint8_t *)"e0", 2);
			}

		}
			
	}

	// frame not yet received completey or no character available at the moment, so we return zero
	return 0;

}


/************************************************************************
 * send a buffer to the host
 * 
 * in : the buffer and it's length
 * out: nothing
 ************************************************************************/
void hostBufferSend(uint8_t *buffer, uint8_t length) {
	
	crc_t crc;  // checksum variable (see crc.c for details on checksum)
	uint8_t c;  // receive character
	uint8_t count = 0;  // reset byte count


	// if last command was broadcast, we do not send anything back to avoid bus conflict
	if(flagBroadcast) {
		return;
	}


	// in RS485 mode we insert our address at the beginning to avoid that other targets will eventually respond to our telegram
	#ifdef USE_RS485
		count = length;
		do {
			buffer[count] = buffer[count-1];
			--count;
		} while(count);
		count = 0;
		++length;
		buffer[0] = RS485_ADDR;
	#endif


	// compute the crc over the buffer
	crc = crcCompute(buffer, length);

	// append the CRC to the buffer
	buffer[length++] = (uint8_t)((crc >> 8) & 0xff);
	buffer[length++] = (uint8_t)(crc & 0xff);

	// with RS485 we set direction to output first
	#ifdef USE_RS485
		#ifdef XMEGA
			myRS485_PORT.OUTSET = (1 << myRS485_DIRPIN);  // transmit
		#else
			myRS485_PORT |= (1<<myRS485_DIRPIN);  // transmit
		#endif
	#endif
			
	// send start of frame
	usartPutChar(STX);
	

	// loop to send the buffer
	do {

		// get next character from the buffer
		c = buffer[count];
		
		// check if current character is one of the protocol characters
		if( (c == STX) || (c == ETX) || (c == ESC) ) {
			
			// if so, we prepend an ESC
			usartPutChar(ESC);
			
			// and set MSB of the character byte
			c |= 0x80;
		}
		
		// send the character
		usartPutChar(c);

		// increment count
		++count;
		
	} while(count < length);  // continue until end of buffer reached

	// when using RS485 we clear the USART transmit complete flag here, because hostReceiveBuffer() will 
	// check this flag before changing RS485 transceiver direction
	#ifdef USE_RS485
		#ifdef XMEGA
			myUSART.STATUS |= (1<<USART_TXCIF_bp);  // clear transmit complete flag
		#else
			myUCSRA |= (1<<myTXC);  // clear transmit complete flag
		#endif
	#endif

	// send end of frame
	usartPutChar(ETX);
	
}
