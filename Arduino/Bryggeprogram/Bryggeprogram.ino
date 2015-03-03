// This code is for a Arduino Mega. By Sindre

#include <OneWire.h>
#include <DallasTemperature.h>

const int ONE_WIRE_BUS = 2;
const unsigned long prePumpeTimeSparge = 60;
// Setup a oneWire instance to communicate with any OneWire devices 
// (not just Maxim/Dallas temperature ICs)
OneWire oneWire(ONE_WIRE_BUS);

// Pass our oneWire reference to Dallas Temperature.
DallasTemperature TemperatureSensors(&oneWire);
struct Sequence
{
	float AddVolumeSP;
	float HltTemperatureSP;
	float TemperatureSP;
	float TimeMinutsSP;
};

struct TankInfo
{
	float Volume;
	float TemperatureTank;
	float TemperatureTankSetPoint;
	float TemperatureHeatingRetur;
	bool LevelOverHatingElementLevel;
	int LevelOverHatingElementLevelPin;
	bool LevelHigh;
	int LevelHighPin;
	bool HeatingElement1;
	int HeatingElement1Pin;
	bool HeatingElement2;
	int HeatingElement2Pin;
	int CirculationPump;
	int CirculationPumpPin;
	int TransferPump;
	int TransferPumpPin;
	bool DrainValve;
	int DrainValvePin;
	bool InnValve;
};

TankInfo Hlt;
TankInfo MashTank;
TankInfo BoilTank;

Sequence MashInn;
Sequence MashStep1;
Sequence MashStep2;
Sequence MashStep3;
Sequence MashStep4;
Sequence Sparge;
Sequence Boil;

float ambientTemperature = 0;

unsigned long refTime = 0;
unsigned long refTime2 = 0;
unsigned long elapsedTimeMinutes = 0;
unsigned long elapsedTimeSeconds = 0;
unsigned long timeSpan = 0;
unsigned long remainingTime = 0;
unsigned long timez = 0;
unsigned long serialOutputTime = 500;
float lastTotVolume = 0;
int state = 0;
bool startBrewing = false;
bool messageConfirmd = false;

String inputString = "";
String sensorString = "";
String sensorStringAll = "";
static String systemDevider = "_";
static String valueDevider = ":";
boolean input_stringcomplete = false;
boolean sensor_stringcomplete = false;
String resivedItems[20];
String sendMessage = "";

// Case variables

bool oneTimeCase30 = true;
bool oneTimeCase31 = true;
bool oneTimeCase32 = true;
bool oneTimeCase33 = true;

void setup() {
	Serial.begin(9600);
//	Serial.setTimeout(10000);
	Serial1.begin(38400);
	
	inputString.reserve(100);
	sensorString.reserve(30);                                                  //set aside some bytes for receiving data from Atlas Scientific product

	// Setting the HLT inn and out pins 
	Hlt.CirculationPumpPin = 4;
	Hlt.TransferPumpPin = 5;
	Hlt.DrainValvePin = 22;
	Hlt.HeatingElement1Pin = 20;
	Hlt.HeatingElement2Pin = 21;
	Hlt.LevelOverHatingElementLevelPin = 26;
	Hlt.LevelHighPin = 27;
	// Setting the HLT inn and out
	pinMode(Hlt.CirculationPumpPin, OUTPUT);
	pinMode(Hlt.TransferPumpPin, OUTPUT);
	pinMode(Hlt.DrainValvePin, OUTPUT);
	pinMode(Hlt.HeatingElement1Pin, OUTPUT);
	pinMode(Hlt.HeatingElement2Pin, OUTPUT);
	pinMode(Hlt.LevelOverHatingElementLevelPin, INPUT);
	pinMode(Hlt.LevelHighPin, INPUT);
	// Setting the  MashTank inn and out pins 

	MashTank.CirculationPumpPin = 30;
	MashTank.TransferPumpPin = 31;
	MashTank.DrainValvePin = 32;
	MashTank.HeatingElement1Pin = 33;
	MashTank.HeatingElement2Pin = 34;
	MashTank.LevelOverHatingElementLevelPin = 35;
	MashTank.LevelHighPin = 36;
	// Setting the MashTank inn and out
	pinMode(MashTank.CirculationPumpPin, OUTPUT);
	pinMode(MashTank.TransferPumpPin, OUTPUT);
	pinMode(MashTank.DrainValvePin, OUTPUT);
	pinMode(MashTank.HeatingElement1Pin, OUTPUT);
	pinMode(MashTank.HeatingElement2Pin, OUTPUT);
	pinMode(MashTank.LevelOverHatingElementLevelPin, INPUT);
	pinMode(MashTank.LevelHighPin, INPUT);


	// Setting the BoilTank inn and out pins 
	BoilTank.CirculationPumpPin = 40;
	BoilTank.TransferPumpPin = 41;
	BoilTank.DrainValvePin = 42;
	BoilTank.HeatingElement1Pin = 43;
	BoilTank.HeatingElement2Pin = 44;
	BoilTank.LevelOverHatingElementLevelPin = 45;
	BoilTank.LevelHighPin = 46;
	// Setting the BoilTank inn and out
	pinMode(BoilTank.CirculationPumpPin, OUTPUT);
	pinMode(BoilTank.TransferPumpPin, OUTPUT);
	pinMode(BoilTank.DrainValvePin, OUTPUT);
	pinMode(BoilTank.HeatingElement1Pin, OUTPUT);
	pinMode(BoilTank.HeatingElement2Pin, OUTPUT);
	pinMode(BoilTank.LevelOverHatingElementLevelPin, INPUT);
	pinMode(BoilTank.LevelHighPin, INPUT);

	timez = millis();

	TemperatureSensors.begin();
}

void serialEvent() {
	String inString = Serial.readString();
	inputString = inString;
	if (inputString.endsWith("\n")) {
		input_stringcomplete = true;
	}
}

void serialEvent1() {
	String sensString = Serial1.readString();
	sensorString = sensString;
	if (sensorString.endsWith("\r")) {
		sensor_stringcomplete = true;
	}
}

void loop() {
	// Getting Temperatures
	TemperatureSensors.requestTemperatures();

	Hlt.HeatingElement1 = false;
	Hlt.HeatingElement2 = false;
	Hlt.CirculationPump = false;
	Hlt.TransferPump = false;
	Hlt.InnValve = false;

	MashTank.HeatingElement1 = false;
	MashTank.HeatingElement2 = false;
	MashTank.CirculationPump = false;
	MashTank.TransferPump = false;
	MashTank.InnValve = false;

	BoilTank.HeatingElement1 = false;
	BoilTank.HeatingElement2 = false;
	BoilTank.CirculationPump = false;
	BoilTank.TransferPump = false;
	BoilTank.InnValve = false;

	timeSpan = 0;
	remainingTime = 0;



	if (input_stringcomplete)
	{
		inputString.trim();
		int index = 0;
		int deviderIndex = 0;
		
		if (inputString.startsWith("CMD"))
		{
			inputString.remove(0, 3);
			int CMD = inputString.toInt();
			if (CMD==0)
			{
				state = 0;
				startBrewing = false;
			}
			
		else if ((CMD == 10) && (state < 10))
			{
				Serial.println("CMD = 10");
				state = 10;
			}
			else if (CMD == 20)
			{
				startBrewing = true;
				Serial.println("CMD = 20");
			}
		}

		else if (inputString.startsWith("CONFIRMED"))
		{
			messageConfirmd = true;
		}

		else if (inputString.startsWith("STA"))
		{
			inputString.remove(0, 3);
			int STA = inputString.toInt();
			state = STA;
		}
		else if (inputString.startsWith("SET"))
		{
			inputString.remove(0, 3);
			for (unsigned int i = 0; i <= inputString.length(); i++)
			{
				if (inputString.substring(i, i + 1) == systemDevider)
				{
					resivedItems[index] = inputString.substring(deviderIndex, i);
					index++;
					deviderIndex = i + 1;

				}

			}

			//for (int i = 0; i < 12; i++)
			//{
			//	Serial.print(i);
			//	Serial.print(":");
			//	Serial.print(resivedItems[i]);
			//	Serial.println("  ");
			//}
			MashInn.TemperatureSP = resivedItems[0].toFloat();					//"MITe"
			MashInn.HltTemperatureSP = resivedItems[1].toFloat();				//"MIHT"
			MashInn.AddVolumeSP = resivedItems[2].toFloat();					//"MIVo"
			MashStep1.TemperatureSP = resivedItems[3].toFloat();				//"M1Te"
			MashStep1.TimeMinutsSP = resivedItems[4].toFloat();					//"M1Ti"
			MashStep2.TemperatureSP = resivedItems[5].toFloat();				//"M2Te"
			MashStep2.TimeMinutsSP = resivedItems[6].toFloat();					//"M2Ti"
			MashStep3.TemperatureSP = resivedItems[7].toFloat();				//"M3Te"
			MashStep3.TimeMinutsSP = resivedItems[8].toFloat();					//"M3Ti"
			Sparge.TemperatureSP = resivedItems[9].toFloat();					//"SpTe"
			Sparge.AddVolumeSP = resivedItems[10].toFloat();					//"SpVo"
			Boil.TimeMinutsSP = resivedItems[11].toFloat();						//"BoTi"

			Sparge.HltTemperatureSP = Sparge.TemperatureSP;

			sendMessage += "ConSe";
			sendMessage += String(MashInn.TemperatureSP) + valueDevider;
			sendMessage += String(MashInn.HltTemperatureSP) + valueDevider;
			sendMessage += String(MashInn.AddVolumeSP) + valueDevider;
			sendMessage += String(MashStep1.TemperatureSP) + valueDevider;
			sendMessage += String(MashStep1.TimeMinutsSP) + valueDevider;
			sendMessage += String(MashStep2.TemperatureSP) + valueDevider;
			sendMessage += String(MashStep2.TimeMinutsSP) + valueDevider;
			sendMessage += String(MashStep3.TemperatureSP) + valueDevider;
			sendMessage += String(MashStep3.TimeMinutsSP) + valueDevider;
			sendMessage += String(Sparge.TemperatureSP) + valueDevider;
			sendMessage += String(Sparge.AddVolumeSP) + valueDevider;
			sendMessage += String(Boil.TimeMinutsSP) + valueDevider;
			sendMessage += systemDevider;
			
		}
		inputString = "";                                                        //clear the string:
		input_stringcomplete = false;                                            //reset the flag used to tell if we have received a completed string from the PC
	}

	if (sensor_stringcomplete){                                                //if a string from the Atlas Scientific product has been received in its entirety
		sensorStringAll = sensorString;
		int commaPosition = sensorString.indexOf(',');
		if (commaPosition != -1)
		{
			String totalVolume = sensorString.substring(0, commaPosition);
			char floatbuf[32];
			totalVolume.toCharArray(floatbuf, sizeof(floatbuf));
			float totVolumeTemp = atof(floatbuf);

			if (abs(lastTotVolume - totVolumeTemp)<0.3)
			{
				MashTank.Volume = totVolumeTemp;
			}
			lastTotVolume = totVolumeTemp;

		}
		sensorString = "";                                                      //clear the string:
		sensor_stringcomplete = false;                                          //reset the flag used to tell if we have received a completed string from the Atlas Scientific product
	}

	Hlt.TemperatureTank = TemperatureSensors.getTempCByIndex(0);
	sendMessage += "HltTe" + String(Hlt.TemperatureTank) + systemDevider;
	MashTank.TemperatureTank = TemperatureSensors.getTempCByIndex(1);
	sendMessage += "MatTe" + String(MashTank.TemperatureTank) + systemDevider;
	MashTank.TemperatureHeatingRetur = TemperatureSensors.getTempCByIndex(2);
	sendMessage += "MarTe" + String(MashTank.TemperatureHeatingRetur) + systemDevider;
	BoilTank.TemperatureTank = TemperatureSensors.getTempCByIndex(3);
	sendMessage += "BotTe" + String(BoilTank.TemperatureTank) + systemDevider;
	ambientTemperature = TemperatureSensors.getTempCByIndex(4);
	sendMessage += "AmbTe" + String(ambientTemperature) + systemDevider;
	sendMessage += "STATE" + String(state) + systemDevider;

	switch (state)
	{
	case 0:
		// ideal state nothing is happening 
		break;

	case 10:
		Hlt.CirculationPump = true;
		Hlt.TemperatureTankSetPoint = MashInn.HltTemperatureSP;

		if ((Hlt.TemperatureTank < Hlt.TemperatureTankSetPoint))
		{
			Hlt.HeatingElement1 = true;
			Hlt.HeatingElement2 = true;
		}
		if (startBrewing){
			state = 20;
		}
		break;

	case 20:// Meshing in

		Hlt.TransferPump = true;
		MashTank.TemperatureTankSetPoint = MashInn.TemperatureSP;
		Hlt.TemperatureTankSetPoint = MashInn.HltTemperatureSP;
		if (Hlt.TemperatureTank<Hlt.TemperatureTankSetPoint)
		{
			Hlt.HeatingElement1 = true;
		}

		if (MashTank.Volume > 15)
		{
			MashTank.CirculationPump = true;
			if (MashTank.TemperatureTank<MashTank.TemperatureTankSetPoint)
			{
				MashTank.HeatingElement1 = true;
			}
		}
		if (MashTank.Volume >= MashInn.AddVolumeSP)
		{
			Hlt.TransferPump = false;
			sendMessage += "MessaAdd corn_";
			if (messageConfirmd)
			{
				state = 30;
				refTime = millis();    // start timer 
				messageConfirmd = false;
				sendMessage += "Messa_"; // Clering message				
			}
		}
		break;

	case 30:

		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		Hlt.TemperatureTankSetPoint = Sparge.HltTemperatureSP;
		MashTank.TemperatureTankSetPoint = MashStep1.TemperatureSP;
		timeSpan = MashStep1.TimeMinutsSP * 60;
		remainingTime = timeSpan - elapsedTimeSeconds;
		Hlt.CirculationPump = true;

		if (oneTimeCase30)
		{
			sendMessage += "TimSp" + String(timeSpan) + systemDevider;
		}
		if (Hlt.TemperatureTank < Hlt.TemperatureTankSetPoint)
		{
			Hlt.HeatingElement1 = true;
			Hlt.HeatingElement2 = true;
		}

		MashTank.CirculationPump = true;
		if (MashTank.TemperatureTank < MashTank.TemperatureTankSetPoint){
			MashTank.HeatingElement1 = true;
		}

		if (remainingTime <= 0)
		{
			refTime = millis();
			state = 31;
		}

		break;

	case 31:
		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		Hlt.TemperatureTankSetPoint = Sparge.HltTemperatureSP;
		MashTank.TemperatureTankSetPoint = MashStep2.TemperatureSP;
		timeSpan = MashStep2.TimeMinutsSP * 60;
		remainingTime = timeSpan - elapsedTimeSeconds;

		Hlt.CirculationPump = true;

		if (oneTimeCase31)
		{
			sendMessage += "TimSp" + String(timeSpan) + systemDevider;
		}

		if (Hlt.TemperatureTank<Hlt.TemperatureTankSetPoint)
		{
			Hlt.HeatingElement1 = true;
			Hlt.HeatingElement2 = true;
		}

		MashTank.CirculationPump = true;
		if (MashTank.TemperatureTank < MashTank.TemperatureTankSetPoint){
			MashTank.HeatingElement1 = true;
		}

		if (remainingTime <= 0)
		{
			refTime = millis();
			state = 32;
		}
		break;

	case 32:
		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		Hlt.TemperatureTankSetPoint = Sparge.HltTemperatureSP;
		MashTank.TemperatureTankSetPoint = MashStep3.TemperatureSP;
		timeSpan = MashStep3.TimeMinutsSP * 60;
		remainingTime = timeSpan - elapsedTimeSeconds;

		Hlt.CirculationPump = true;

		if (oneTimeCase32)
		{
			sendMessage += "TimSp" + String(timeSpan) + systemDevider;
		}

		if (Hlt.TemperatureTank<Hlt.TemperatureTankSetPoint)
		{
			Hlt.HeatingElement1 = true;
			Hlt.HeatingElement2 = true;
		}

		MashTank.CirculationPump = true;
		if (MashTank.TemperatureTank < MashTank.TemperatureTankSetPoint){
			MashTank.HeatingElement1 = true;
		}

		if (remainingTime <= 0)
		{
			refTime = millis();
			state = 33;
		}
		break;

	case 33:
		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		Hlt.TemperatureTankSetPoint = Sparge.HltTemperatureSP;
		MashTank.TemperatureTankSetPoint = MashStep4.TemperatureSP;
		timeSpan = MashStep4.TimeMinutsSP * 60;
		remainingTime = timeSpan - elapsedTimeSeconds;

		Hlt.CirculationPump = true;

		if (oneTimeCase33)
		{
			sendMessage += "TimSp" + String(timeSpan) + systemDevider;
		}

		if (Hlt.TemperatureTank<Hlt.TemperatureTankSetPoint)
		{
			Hlt.HeatingElement1 = true;
			Hlt.HeatingElement2 = true;
		}

		MashTank.CirculationPump = true;
		if (MashTank.TemperatureTank < MashTank.TemperatureTankSetPoint){
			MashTank.HeatingElement1 = true;
		}

		if (remainingTime <= 0)
		{
			refTime = millis();
			state = 40;
		}
		break;

	case 40: // Sparge
		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		Hlt.TemperatureTankSetPoint = Sparge.HltTemperatureSP;
		MashTank.TemperatureTankSetPoint = Sparge.TemperatureSP;
		timeSpan = Sparge.TimeMinutsSP * 60;
		remainingTime = timeSpan - elapsedTimeSeconds;

		if (Hlt.TemperatureTank<Hlt.TemperatureTankSetPoint)
		{
			Hlt.HeatingElement1 = true;
			Hlt.HeatingElement2 = true;
		}
		MashTank.TransferPump = true;

		if (elapsedTimeSeconds >= prePumpeTimeSparge)
		{
			Hlt.TransferPump = true;
		}

		if (MashTank.Volume<(MashInn.AddVolumeSP + Sparge.AddVolumeSP))
		{
			Hlt.TransferPump = true;
		}
		else if (elapsedTimeSeconds>(MashTank.Volume))
		{

		}

		if (BoilTank.LevelOverHatingElementLevel)
		{
			BoilTank.HeatingElement1 = true;
			BoilTank.HeatingElement2 = true;
		}


		break;

	default:
		state = 0;
		break;
	}

	sendMessage += "HltSp" + String(Hlt.TemperatureTankSetPoint) + systemDevider ;
	sendMessage += "HltE1" + String(Hlt.HeatingElement1) + systemDevider;
	sendMessage += "HltCp" + String(Hlt.CirculationPump) + systemDevider;
	sendMessage += "HltTp" + String(Hlt.TransferPump) + systemDevider;

	sendMessage += "MatSp" + String(MashTank.TemperatureTankSetPoint) + systemDevider;
	sendMessage += "MatE1" + String(MashTank.HeatingElement1) + systemDevider;
	sendMessage += "MatCp" + String(MashTank.CirculationPump) + systemDevider;
	sendMessage += "MatTp" + String(MashTank.TransferPump) + systemDevider;
	sendMessage += "MatVo" + String(MashTank.Volume) + systemDevider;

	sendMessage += "BotSp" + String(BoilTank.TemperatureTankSetPoint) + systemDevider;
	sendMessage += "BotE1" + String(BoilTank.HeatingElement1) + systemDevider;
	sendMessage += "BotCp" + String(BoilTank.CirculationPump) + systemDevider;
	sendMessage += "BotTp" + String(BoilTank.TransferPump) + systemDevider;
	sendMessage += "BotVo" + String(BoilTank.Volume) + systemDevider;

	sendMessage += "Timer" + String(elapsedTimeSeconds) + systemDevider;
	sendMessage += "RemTi" + String(remainingTime) + systemDevider;

	Serial.println(sendMessage);
	sendMessage = "";

	digitalWrite(Hlt.CirculationPumpPin, Hlt.CirculationPump);
	digitalWrite(Hlt.TransferPumpPin, Hlt.TransferPump);
	digitalWrite(Hlt.HeatingElement1Pin, Hlt.HeatingElement1);

}

void interfaceConvertion(float *value, String _string, String hedder)
{
	if (_string.startsWith(hedder))
	{
		_string.remove(0, 4);
		*value = (_string.toFloat());
	}
}