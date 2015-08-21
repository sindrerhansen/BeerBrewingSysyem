// 
// 
// 

#include "Regulators.h"

unsigned long Ts;
unsigned long Tc;

bool TankTemperaturOnOffRegulator(double setpoint, double actual, bool overElement)
{
	bool output;
	if (overElement)
	{
		if (actual <= setpoint)
		{
			output = true;
		}
		else
		{
			output = false;
		}
	}

	else
	{
		output = false;
		//MessageToUser = "Add water to hot liquor tank!!";
	}

	return output;
}

bool PWM_Reelay(double setpoint, double actual, double ratio)
{
	bool output = false;
	Tc = millis();
	double PWD_Window = 5000;

	if (actual < setpoint)
	{
		if (ratio > 0)
		{
			if (ratio < 1)
			{
				if (Ts + PWD_Window*ratio > Tc)
				{
					output = true;
				}
				else
				{
					output = false;
				}
				if (Ts + PWD_Window < Tc)
				{
					Ts = Tc;
				}
			}
			else
			{
				output = true;
			}
		}
		else
		{
			output = false;
		}
	}

	else
	{
		output = false;
	}
	if (actual < (setpoint - 3.0))
	{
		output = true;
	}

	return output;
}

bool Tank_PWM_ReelayRegulator(double setpoint, double actual, double ratio, bool overElement)
{
	bool output;

	if (overElement)
	{
		output = PWM_Reelay(setpoint, actual, ratio);
	}
	else
	{
		output = false;
	}

	return output;
}

bool RIMS_PWM_ReelayRegulator(double setpoint, double tempInn, double tempOut, double ratio, double RIMS_outesideTemp)
{
	bool output = false;

	if (tempOut < (tempInn + 9.0) && tempOut < (setpoint + 5.0) && RIMS_outesideTemp < tempInn && RIMS_outesideTemp < setpoint)
	{
		output = PWM_Reelay(setpoint, tempInn, ratio);
	}
	else
	{
		output = false;
	}

	return output;
}