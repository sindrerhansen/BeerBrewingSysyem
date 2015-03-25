#include <OneWire.h>
#include <DallasTemperature.h>


const int ONE_WIRE_BUS = 2;
const char systemDevider = '|';

OneWire oneWire(ONE_WIRE_BUS);

// Pass our oneWire reference to Dallas Temperature.
DallasTemperature TemperatureSensors(&oneWire);

void setup()
{
	
	Serial.begin(9600);
	TemperatureSensors.begin();
}

void loop()
{

	TemperatureSensors.requestTemperatures();
	String sendMessage = "";
	float HltTemperatureTank = TemperatureSensors.getTempCByIndex(0);
	sendMessage += String(HltTemperatureTank) + systemDevider;
	float MashTankTemperatureTank = TemperatureSensors.getTempCByIndex(2);
	sendMessage += String(MashTankTemperatureTank) + systemDevider;
	float MashTankTemperatureHeatingRetur = TemperatureSensors.getTempCByIndex(1);
	sendMessage += String(MashTankTemperatureHeatingRetur) + systemDevider;
	float BoilTankTemperatureTank = TemperatureSensors.getTempCByIndex(4);
	sendMessage += String(BoilTankTemperatureTank) + systemDevider;
	float ambientTemperature = TemperatureSensors.getTempCByIndex(3);
	sendMessage += String(ambientTemperature) + systemDevider;
	

	Serial.println(sendMessage);
	delay(1);

}
