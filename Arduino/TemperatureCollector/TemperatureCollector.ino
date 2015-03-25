#include <OneWire.h>
#include <DallasTemperature.h>
static int ONE_WIRE_BUS = 2;
static char systemDevider = '_';
static char valueDevider = ':';


// Setup a oneWire instance to communicate with any OneWire devices 
// (not just Maxim/Dallas temperature ICs)
OneWire oneWire(ONE_WIRE_BUS);

// Pass our oneWire reference to Dallas Temperature.
DallasTemperature TemperatureSensors(&oneWire);

bool input_stringcomplete;
String sendMessage;
void setup()
{
	Serial.begin(9600);
	TemperatureSensors.begin();

}

void serialEvent() {
	String inString = Serial.readString();

	if (inString.endsWith("\n")) {
		input_stringcomplete = true;
	}
}

void loop()
{
	// Getting Temperatures
	TemperatureSensors.requestTemperatures();
	sendMessage = "";
	int numberOfSensors = TemperatureSensors.getDeviceCount();
	float HltTemperatureTank = TemperatureSensors.getTempCByIndex(0);
	sendMessage += String(HltTemperatureTank) + valueDevider;
	float MashTankTemperatureTank = TemperatureSensors.getTempCByIndex(2);
	sendMessage += String(MashTankTemperatureTank) + valueDevider;
	float MashTankTemperatureHeatingRetur = TemperatureSensors.getTempCByIndex(1);
	sendMessage += String(MashTankTemperatureHeatingRetur) + valueDevider;
	float BoilTankTemperatureTank = TemperatureSensors.getTempCByIndex(4);
	sendMessage += String(BoilTankTemperatureTank) + valueDevider;
	float ambientTemperature = TemperatureSensors.getTempCByIndex(3);
	sendMessage += String(ambientTemperature) + valueDevider;

	Serial.println(sendMessage);
	//Serial.println(numberOfSensors);
}
