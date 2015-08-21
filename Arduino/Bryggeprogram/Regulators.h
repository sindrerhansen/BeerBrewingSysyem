// Regulators.h

#ifndef _REGULATORS_h
#define _REGULATORS_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif
extern unsigned long Ts;
extern unsigned long Tc;
bool TankTemperaturOnOffRegulator(double setpoint, double actual, bool overElement);

bool PWM_Reelay(double setpoint, double actual, double ratio);

bool Tank_PWM_ReelayRegulator(double setpoint, double actual, double ratio, bool overElement);

bool RIMS_PWM_ReelayRegulator(double setpoint, double tempInn, double tempOut, double ratio, double RIMS_outesideTemp);


#endif

