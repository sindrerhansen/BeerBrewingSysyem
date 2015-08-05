// 
// 
// 

#include "Fermentor.h"

void FermentorClass::init(double TemperatureSetPoint, double Hysterese, int HeatingOutputPin, int ColingOutputPin)
{
	temperatureSetPoint = TemperatureSetPoint;
	hysterese = Hysterese;
	heatingOutputPin = HeatingOutputPin;
	colingOutputPin = ColingOutputPin;
	pinMode(heatingOutputPin, OUTPUT);
	pinMode(colingOutputPin, OUTPUT);


}

int FermentorClass::Run(double currentTemperature)
{
	if (currentTemperature>temperatureSetPoint)
	{
		return true;
	}
}


FermentorClass Fermentor;

