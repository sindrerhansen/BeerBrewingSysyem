#include <OneWire.h>
#include <DallasTemperature.h>
static int ONE_WIRE_BUS = 2;



// Setup a oneWire instance to communicate with any OneWire devices 
// (not just Maxim/Dallas temperature ICs)
OneWire oneWire(ONE_WIRE_BUS);

// Pass our oneWire reference to Dallas Temperature.
DallasTemperature TemperatureSensors(&oneWire);

bool Serial1_Input_stringcomplete;
String AllInfoString;
float maxtemperature;
float mintemperature;
float temperature;
void setup()
{
  
  
	Serial.begin(9600);
	TemperatureSensors.begin();
  TemperatureSensors.requestTemperatures();
  temperature=TemperatureSensors.getTempCByIndex(0);
  maxtemperature=temperature;
  mintemperature=temperature;
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

	float temperature = TemperatureSensors.getTempCByIndex(0);
  if(temperature < mintemperature)
  {
   mintemperature=temperature; 
    }
  
  
  if(temperature > maxtemperature)
  {
   maxtemperature = temperature;
  } 
   
    
	//Serial.println(temperature);
  Serial.print("Max ");
  Serial.println(maxtemperature);
  Serial.print("Min ");
  Serial.println(mintemperature);
  Serial.println();
}
