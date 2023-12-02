/************************************************************************
 * xtae.h
 *
 * chip45boot3 versatile AVR ATmega/Xmega bootloader
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * header file for xtae.h
 ************************************************************************/


#ifndef XTAE_H_
#define XTAE_H_

	#include "configure.h"
	
	#ifdef USE_ENCRYPTION
	
		/************************************************************************
		 * function prototypes
		 ************************************************************************/
		void xtaeInit(void);
		void xtaeDecryptBuffer(uint8_t *buffer, uint16_t length);

	#endif
	
#endif /* XTAE_H_ */
