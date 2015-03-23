#include <OneWire.h>
#include <DallasTemperature.h>


const int ONE_WIRE_BUS = 2;
const String systemDevider = "_";

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
	sendMessage += "HltTe" + String(HltTemperatureTank) + systemDevider;
	float MashTankTemperatureTank = TemperatureSensors.getTempCByIndex(2);
	sendMessage += "MatTe" + String(MashTankTemperatureTank) + systemDevider;
	float MashTankTemperatureHeatingRetur = TemperatureSensors.getTempCByIndex(1);
	sendMessage += "MarTe" + String(MashTankTemperatureHeatingRetur) + systemDevider;
	float BoilTankTemperatureTank = TemperatureSensors.getTempCByIndex(4);
	sendMessage += "BotTe" + String(BoilTankTemperatureTank) + systemDevider;
	float ambientTemperature = TemperatureSensors.getTempCByIndex(3);
	sendMessage += "AmbTe" + String(ambientTemperature) + systemDevider;
	

	Serial.println(sendMessage);
	delay(1);

}
