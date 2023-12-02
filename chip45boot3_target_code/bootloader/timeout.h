/************************************************************************
 * timeout.c
 *
 * (c) 2017, Dr. Erik Lins, chip45 GmbH u. Co. KG
 * info@chip45.com, http://www.chip45.com
 *
 * timeout functions
 ************************************************************************/


#ifndef TIMEOUT_H_

	#define TIMEOUT_H_


	// function prototypes
	void timeoutInit(void);
	void timeoutDeInit(void);

	uint8_t timeoutExpired(void);


#endif /* TIMEOUT_H_ */