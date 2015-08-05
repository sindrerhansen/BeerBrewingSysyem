// Fermentor.h

#ifndef _FERMENTOR_h
#define _FERMENTOR_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

class FermentorClass
{
 protected:

private:
	double temperature;
	double temperatureSetPoint;
	double hysterese;
	int heatingOutputPin, colingOutputPin;

 public:
	 void init(double TemperatureSetPoint, double Hysterese, int HeatingOutputPin, int ColingOutputPin);
	 int Run(double Temperature);

};

extern FermentorClass Fermentor;

#endif

