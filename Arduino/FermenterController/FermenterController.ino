
#include "Fermentor.h"
#include <Time.h>  
#define TIME_HEADER  "T"   // Header tag for serial time sync message
#define TIME_REQUEST  7    // ASCII bell character requests a time sync message 


static char systemDevider = '_';
static char valueDevider = ':';

long unsigned int CurrentTime, DifTime, SetTime = millis();

FermentorClass Ferm1;

bool Serial0_Input_stringcomplete, Serial1_Input_stringcomplete, Serial2_Input_stringcomplete, Serial3_Input_stringcomplete;
String AllInfoString;

void setup()
{

	Serial.begin(9600);
	Serial1.begin(9600);
	setSyncProvider(requestSync);  //set function to call when sync required
	Ferm1.init();
}

void serialEvent() {
	String Serial0_InString = Serial.readString();

	if (Serial0_InString.endsWith("\n")) {
		Serial0_Input_stringcomplete = true;
	}
}

void serial1Event(){
	String Serial1_InString = Serial1.readString();

	if (Serial1_InString.endsWith("\n")) {
		Serial1_Input_stringcomplete = true;
	}
}

void serial2Event(){
	String Serial2_InString = Serial2.readString();

	if (Serial2_InString.endsWith("\n")) {
		Serial2_Input_stringcomplete = true;
	}
}

void serial3Event(){
	String Serial3_InString = Serial3.readString();

	if (Serial3_InString.endsWith("\n")) {
		Serial3_Input_stringcomplete = true;
	}
}
void loop()
{
	// Getting Temperatures
	TemperatureSensors.requestTemperatures();
	AllInfoString = "";
	Temperature = TemperatureSensors.getTempCByIndex(0);

}


void processSyncMessage() {
	unsigned long pctime;
	const unsigned long DEFAULT_TIME = 1357041600; // Jan 1 2013

	if (Serial.find(TIME_HEADER)) {
		pctime = Serial.parseInt();
		if (pctime >= DEFAULT_TIME) { // check the integer is a valid time (greater than Jan 1 2013)
			setTime(pctime); // Sync Arduino clock to the time received on the serial port
		}
	}
}

time_t requestSync()
{
	Serial.write(TIME_REQUEST);
	return 0; // the time will be sent later in response to serial mesg
}
