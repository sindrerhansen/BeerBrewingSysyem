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

bool Serial1_Input_stringcomplete;
String AllInfoString;
void setup()
{
	Serial.begin(9600);
	TemperatureSensors.begin();

}

void serialEvent() {
	String inString = Serial.readString();

	if (inString.endsWith("\n")) {
		Serial1_Input_stringcomplete = true;
	}
}

void loop()
{
	// Getting Temperatures
	TemperatureSensors.requestTemperatures();
	AllInfoString = "";
	int numberOfSensors = TemperatureSensors.getDeviceCount();
	float HltTemperatureTank = TemperatureSensors.getTempCByIndex(1);
	AllInfoString += String(HltTemperatureTank) + valueDevider;

	float MashTankTemperatureTank = TemperatureSensors.getTempCByIndex(3);
	AllInfoString += String(MashTankTemperatureTank) + valueDevider;

	float MashTankTemperatureHeatingRetur = TemperatureSensors.getTempCByIndex(2);
	AllInfoString += String(MashTankTemperatureHeatingRetur) + valueDevider;

	float BoilTankTemperatureTank = TemperatureSensors.getTempCByIndex(5);
	AllInfoString += String(BoilTankTemperatureTank) + valueDevider;

	float ambientTemperature = TemperatureSensors.getTempCByIndex(4);
	AllInfoString += String(ambientTemperature) + valueDevider;

	float rimsOutesideTemperature = TemperatureSensors.getTempCByIndex(0);
	AllInfoString += String(rimsOutesideTemperature) + valueDevider;

	Serial.println(AllInfoString);
	//Serial.println(numberOfSensors);
}