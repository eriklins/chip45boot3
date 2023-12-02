/************************************************************************
 * autobaud.h
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * header file for autobaud.c
 ************************************************************************/


#ifndef AUTOBAUD_H_
#define AUTOBAUD_H_


	#ifdef USE_AUTOBAUD

		// function prototypes
		void autobaudInit(void);
		void autobaudDeInit(void);
		uint8_t autobaudDo(uint32_t *);
	
	#endif // USE_AUTOBAUD


#endif /* AUTOBAUD_H_ */