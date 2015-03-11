// This code is for a Arduino Mega. By Sindre
float tempVolume;
#include <OneWire.h>
#include <DallasTemperature.h>
#pragma region Init
const int ONE_WIRE_BUS = 2;
const long prePumpeTimeSparge = 20;
const int flowOfSet = 0.2;
const int MashCirculationStartTreshold = 2;

#pragma endregion Init
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
	struct Pump
	{
		int OutputPin;
		bool Value;
		bool ValueOverride;
		bool OverrideEnable;
		bool ValueOut;
	};
	struct HeatingElement
	{
		int OutputPin;
		bool Value;
		bool ValueOverride;
		bool OverrideEnable;
		bool ValueOut;
	};
	struct Valve
	{
		int OutputPin;
		bool Value;
		bool ValueOverride;
		bool OverrideEnable;
		bool ValueOut;
	};
	struct LevelSwitch
	{
		int InputPin;
		bool State;
	};
	
	
	float Volume;
	float TemperatureTank;
	float TemperatureTankSetPoint;
	float TemperatureHeatingRetur;

	HeatingElement Element1;
	HeatingElement Element2;
	Pump CirculationPump;
	Pump TransferPump;
	Valve DrainValve;
	Valve InnValve;
	LevelSwitch LevelHigh;
	LevelSwitch LevelOverHeatingElements;
};

TankInfo Hlt;
TankInfo MashTank;
TankInfo BoilTank;
TankInfo AllTanks[3];

Sequence MashInn;
Sequence MashStep1;
Sequence MashStep2;
Sequence MashStep3;
Sequence MashStep4;
Sequence Sparge;
Sequence Boil;

float ambientTemperature = 0;

long refTime = 0;
long refTime2 = 0;
long elapsedTimeMinutes = 0;
long elapsedTimeSeconds = 0;
long timeSpan = 0;
long remainingTime = 0;
long timez = 0;
float lastTotVolume = 0;
int state = 0;
bool startBrewing = false;
bool messageConfirmd = false;

String inputString = "";
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

	Serial1.begin(38400);	
	inputString.reserve(100);

	timez = millis();
	TemperatureSensors.begin();

#pragma region Init_HLT
	// Setting the HLT inn and out pins 
	Hlt.CirculationPump.OutputPin = 4;
	Hlt.TransferPump.OutputPin = 5;
	Hlt.Element1.OutputPin = 20;
	Hlt.Element2.OutputPin = 21;
	Hlt.DrainValve.OutputPin = 22;
	Hlt.LevelOverHeatingElements.InputPin = 26;
	Hlt.LevelHigh.InputPin = 27;
	// Setting the HLT inn and out
	pinMode(Hlt.CirculationPump.OutputPin, OUTPUT);
	pinMode(Hlt.TransferPump.OutputPin, OUTPUT);
	pinMode(Hlt.DrainValve.OutputPin, OUTPUT);
	pinMode(Hlt.Element1.OutputPin, OUTPUT);
	pinMode(Hlt.Element2.OutputPin, OUTPUT);
	pinMode(Hlt.LevelOverHeatingElements.InputPin, INPUT);
	pinMode(Hlt.LevelHigh.InputPin, INPUT);
#pragma endregion Init_HLT

#pragma region Init_MashTank
	// Setting the  MashTank inn and out pins 
	MashTank.CirculationPump.OutputPin = 6;
	MashTank.TransferPump.OutputPin = 7;
	MashTank.Element1.OutputPin = 30;
	MashTank.Element2.OutputPin = 31;
	MashTank.DrainValve.OutputPin = 32;
	MashTank.LevelOverHeatingElements.InputPin = 35;
	MashTank.LevelHigh.InputPin = 36;
	// Setting the MashTank inn and out
	pinMode(MashTank.CirculationPump.OutputPin, OUTPUT);
	pinMode(MashTank.TransferPump.OutputPin, OUTPUT);
	pinMode(MashTank.DrainValve.OutputPin, OUTPUT);
	pinMode(MashTank.Element1.OutputPin, OUTPUT);
	pinMode(MashTank.Element2.OutputPin, OUTPUT);
	pinMode(MashTank.LevelOverHeatingElements.InputPin, INPUT);
	pinMode(MashTank.LevelHigh.InputPin, INPUT);
#pragma endregion Init_MashTank

#pragma region Init_BoilTank
	// Setting the BoilTank inn and out pins 
	BoilTank.CirculationPump.OutputPin = 8;
	BoilTank.TransferPump.OutputPin = 9;
	BoilTank.Element1.OutputPin = 40;
	BoilTank.Element2.OutputPin = 41;
	BoilTank.DrainValve.OutputPin = 42;
	BoilTank.LevelOverHeatingElements.InputPin = 45;
	BoilTank.LevelHigh.InputPin = 46;
	// Setting the BoilTank inn and out
	pinMode(BoilTank.CirculationPump.OutputPin, OUTPUT);
	pinMode(BoilTank.TransferPump.OutputPin, OUTPUT);
	pinMode(BoilTank.DrainValve.OutputPin, OUTPUT);
	pinMode(BoilTank.Element1.OutputPin, OUTPUT);
	pinMode(BoilTank.Element2.OutputPin, OUTPUT);
	pinMode(BoilTank.LevelOverHeatingElements.InputPin, INPUT);
	pinMode(BoilTank.LevelHigh.InputPin, INPUT);
#pragma endregion Init_BoilTank

}

void serialEvent() {
	String inString = Serial.readString();
	inputString = inString;
	if (inputString.endsWith("\n")) {
		input_stringcomplete = true;
	}
}

void serialEvent1() {
	String _sensString = Serial1.readStringUntil('\r');
	String _totalVolume = _sensString.substring(0, 5);
	_totalVolume.trim();
	float _volume = _totalVolume.toFloat();
	if (abs(lastTotVolume - _volume) < 0.5)
	{
		MashTank.Volume = _volume;
	}
	lastTotVolume = _volume;
}

void loop() {
	// Getting Temperatures
	TemperatureSensors.requestTemperatures();

	Hlt.Element1.Value = false;
	Hlt.Element2.Value = false;
	Hlt.CirculationPump.Value = false;
	Hlt.TransferPump.Value = false;
	Hlt.InnValve.Value = false;

	MashTank.Element1.Value = false;
	MashTank.Element2.Value = false;
	MashTank.CirculationPump.Value = false;
	MashTank.TransferPump.Value = false;
	MashTank.InnValve.Value = false;

	BoilTank.Element1.Value = false;
	BoilTank.Element2.Value = false;
	BoilTank.CirculationPump.Value = false;
	BoilTank.TransferPump.Value = false;
	BoilTank.InnValve.Value = false;

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
		else if ((CMD == 20) && (state<20))
			{
				startBrewing = true;
				state = 20;
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
		//	Serial.println(sendMessage);
		}

		else if (inputString.startsWith("OVERRIDE"))
		{
			inputString.remove(0, 8);
			inputString.trim();
			int _overrideCMD = inputString.toInt();
			if (_overrideCMD<10)
			{

			}
			else if (_overrideCMD>=10 && _overrideCMD<20)
			{

			}

			else if (_overrideCMD>=20 && _overrideCMD<30)
			{

			}

			else if (_overrideCMD >= 30 && _overrideCMD<40)
			{

			}
		}

		inputString = "";                                                        //clear the string:
		input_stringcomplete = false;                                            //reset the flag used to tell if we have received a completed string from the PC
	}
//	int devises = TemperatureSensors.getDeviceCount();
//	Serial.println(devises);
	Hlt.TemperatureTank = TemperatureSensors.getTempCByIndex(0);
	sendMessage += "HltTe" + String(Hlt.TemperatureTank) + systemDevider;
	MashTank.TemperatureTank = TemperatureSensors.getTempCByIndex(2);
	sendMessage += "MatTe" + String(MashTank.TemperatureTank) + systemDevider;
	MashTank.TemperatureHeatingRetur = TemperatureSensors.getTempCByIndex(1);
	sendMessage += "MarTe" + String(MashTank.TemperatureHeatingRetur) + systemDevider;
	BoilTank.TemperatureTank = TemperatureSensors.getTempCByIndex(4);
	sendMessage += "BotTe" + String(BoilTank.TemperatureTank) + systemDevider;
	ambientTemperature = TemperatureSensors.getTempCByIndex(3);
	sendMessage += "AmbTe" + String(ambientTemperature) + systemDevider;
	sendMessage += "STATE" + String(state) + systemDevider;

	switch (state)
	{
	case 0:
		// ideal state nothing is happening 
		sendMessage += "Messa_"; // Clering message	
		break;

	case 10:
		Hlt.CirculationPump.Value = true;
		Hlt.TemperatureTankSetPoint = MashInn.HltTemperatureSP;

		if ((Hlt.TemperatureTank < Hlt.TemperatureTankSetPoint))
		{
			Hlt.Element1.Value = true;
			Hlt.Element2.Value = true;
		}
		if (startBrewing){
			state = 20;
		}
		break;

	case 20:// Meshing in
		Hlt.CirculationPump.Value = true;
		Hlt.TransferPump.Value = true;
		MashTank.TemperatureTankSetPoint = MashInn.TemperatureSP;
		Hlt.TemperatureTankSetPoint = MashInn.HltTemperatureSP;
		if (Hlt.TemperatureTank<Hlt.TemperatureTankSetPoint)
		{
			Hlt.Element1.Value = true;
		}

		if (MashTank.Volume > MashCirculationStartTreshold)
		{
			MashTank.CirculationPump.Value = true;
			if (MashTank.TemperatureTank<MashTank.TemperatureTankSetPoint)
			{
				MashTank.Element1.Value = true;
			}
		}
		if ((MashTank.Volume+flowOfSet) >= MashInn.AddVolumeSP)
		{
			Hlt.TransferPump.Value = false;
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
		if (MashStep1.TimeMinutsSP <= 0){ state = 31; }
		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		Hlt.TemperatureTankSetPoint = Sparge.HltTemperatureSP;
		MashTank.TemperatureTankSetPoint = MashStep1.TemperatureSP;
		timeSpan = MashStep1.TimeMinutsSP * 60;
		remainingTime = timeSpan - elapsedTimeSeconds;
		Hlt.CirculationPump.Value = true;

		if (oneTimeCase30)
		{
			sendMessage += "TimSp" + String(timeSpan) + systemDevider;
		}
		if (Hlt.TemperatureTank < Hlt.TemperatureTankSetPoint)
		{
			Hlt.Element1.Value = true;
			Hlt.Element2.Value = true;
		}

		MashTank.CirculationPump.Value = true;
		if (MashTank.TemperatureTank < MashTank.TemperatureTankSetPoint){
			MashTank.Element1.Value = true;
		}

		if (remainingTime <= 0)
		{
			refTime = millis();
			state = 31;
		}

		break;

	case 31:
		if (MashStep2.TimeMinutsSP <= 0){ state = 32; }
		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		Hlt.TemperatureTankSetPoint = Sparge.HltTemperatureSP;
		MashTank.TemperatureTankSetPoint = MashStep2.TemperatureSP;
		timeSpan = MashStep2.TimeMinutsSP * 60;
		remainingTime = timeSpan - elapsedTimeSeconds;

		Hlt.CirculationPump.Value = true;

		if (oneTimeCase31)
		{
			sendMessage += "TimSp" + String(timeSpan) + systemDevider;
		}

		if (Hlt.TemperatureTank<Hlt.TemperatureTankSetPoint)
		{
			Hlt.Element1.Value = true;
			Hlt.Element2.Value = true;
		}

		MashTank.CirculationPump.Value = true;
		if (MashTank.TemperatureTank < MashTank.TemperatureTankSetPoint){
			MashTank.Element1.Value = true;
		}

		if (remainingTime <= 0)
		{
			refTime = millis();
			state = 32;
		}
		break;

	case 32:
		
		if (MashStep3.TimeMinutsSP <= 0){state = 33;}

		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		Hlt.TemperatureTankSetPoint = Sparge.HltTemperatureSP;
		MashTank.TemperatureTankSetPoint = MashStep3.TemperatureSP;
		timeSpan = MashStep3.TimeMinutsSP * 60;
		remainingTime = timeSpan - elapsedTimeSeconds;

		Hlt.CirculationPump.Value = true;

		if (oneTimeCase32)
		{
			sendMessage += "TimSp" + String(timeSpan) + systemDevider;
		}

		if (Hlt.TemperatureTank<Hlt.TemperatureTankSetPoint)
		{
			Hlt.Element1.Value = true;
			Hlt.Element2.Value = true;
		}

		MashTank.CirculationPump.Value = true;
		if (MashTank.TemperatureTank < MashTank.TemperatureTankSetPoint){
			MashTank.Element1.Value = true;
		}

		if (remainingTime <= 0)
		{
			refTime = millis();
			state = 33;
		}
		break;

	case 33:
		if (MashStep4.TimeMinutsSP <= 0){ state = 34; }
		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		Hlt.TemperatureTankSetPoint = Sparge.HltTemperatureSP;
		MashTank.TemperatureTankSetPoint = MashStep4.TemperatureSP;
		timeSpan = MashStep4.TimeMinutsSP * 60;
		remainingTime = timeSpan - elapsedTimeSeconds;

		Hlt.CirculationPump.Value = true;

		if (oneTimeCase33)
		{
			sendMessage += "TimSp" + String(timeSpan) + systemDevider;
		}

		if (Hlt.TemperatureTank<Hlt.TemperatureTankSetPoint)
		{
			Hlt.Element1.Value = true;
			Hlt.Element2.Value = true;
		}

		MashTank.CirculationPump.Value = true;
		if (MashTank.TemperatureTank < MashTank.TemperatureTankSetPoint){
			MashTank.Element1.Value = true;
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
			Hlt.Element1.Value = true;
			Hlt.Element2.Value = true;
		}
		MashTank.TransferPump.Value = true;
		Serial.println(tempVolume);
		if (elapsedTimeSeconds >= prePumpeTimeSparge)
		{
			
			if (MashTank.Volume<(MashInn.AddVolumeSP + Sparge.AddVolumeSP))
			{
				Hlt.TransferPump.Value = true;
			}
			
		}

		if (BoilTank.LevelOverHeatingElements.State)
		{
			BoilTank.Element1.Value = true;
			BoilTank.Element2.Value = true;
		}


		break;

	default:
		state = 0;
		break;
	}

#pragma region SendingMessageToSerial 
	sendMessage += "HltSp" + String(Hlt.TemperatureTankSetPoint) + systemDevider ;
	sendMessage += "HltE1" + String(Hlt.Element1.Value) + systemDevider;
	sendMessage += "HltCp" + String(Hlt.CirculationPump.Value) + systemDevider;
	sendMessage += "HltTp" + String(Hlt.TransferPump.Value) + systemDevider;

	sendMessage += "MatSp" + String(MashTank.TemperatureTankSetPoint) + systemDevider;
	sendMessage += "MatE1" + String(MashTank.Element1.Value) + systemDevider;
	sendMessage += "MatCp" + String(MashTank.CirculationPump.Value) + systemDevider;
	sendMessage += "MatTp" + String(MashTank.TransferPump.Value) + systemDevider;
	sendMessage += "MatVo" + String(MashTank.Volume) + systemDevider;

	sendMessage += "BotSp" + String(BoilTank.TemperatureTankSetPoint) + systemDevider;
	sendMessage += "BotE1" + String(BoilTank.Element1.Value) + systemDevider;
	sendMessage += "BotCp" + String(BoilTank.CirculationPump.Value) + systemDevider;
	sendMessage += "BotTp" + String(BoilTank.TransferPump.Value) + systemDevider;
	sendMessage += "BotVo" + String(BoilTank.Volume) + systemDevider;

	sendMessage += "Timer" + String(elapsedTimeSeconds) + systemDevider;
	sendMessage += "RemTi" + String(remainingTime) + systemDevider;

	Serial.println(sendMessage);
	sendMessage = "";
#pragma endregion SendingMessageToSerial

#pragma region Setting_Outputs 
	AllTanks[1] = Hlt;
	AllTanks[2] = MashTank;
	AllTanks[3]= BoilTank;

	for (int tank = 1; tank <= (sizeof(AllTanks) / sizeof(TankInfo)); tank++)
	{
		if (AllTanks[tank].Element1.OverrideEnable){ AllTanks[tank].Element1.ValueOut = AllTanks[tank].Element1.ValueOverride; }
		else{ AllTanks[tank].Element1.ValueOut = AllTanks[tank].Element1.Value; }
		digitalWrite(AllTanks[tank].Element1.OutputPin, AllTanks[tank].Element1.ValueOut);

		if (AllTanks[tank].Element2.OverrideEnable){ AllTanks[tank].Element2.ValueOut = AllTanks[tank].Element2.ValueOverride; }
		else{ AllTanks[tank].Element2.ValueOut = AllTanks[tank].Element2.Value; }
		digitalWrite(AllTanks[tank].Element2.OutputPin, AllTanks[tank].Element2.ValueOut);

		if (AllTanks[tank].CirculationPump.OverrideEnable){ AllTanks[tank].CirculationPump.ValueOut = AllTanks[tank].CirculationPump.ValueOverride; }
		else{ AllTanks[tank].CirculationPump.ValueOut = AllTanks[tank].CirculationPump.Value; }
		digitalWrite(AllTanks[tank].CirculationPump.OutputPin, AllTanks[tank].CirculationPump.ValueOut);

		if (AllTanks[tank].TransferPump.OverrideEnable){ AllTanks[tank].TransferPump.ValueOut = AllTanks[tank].TransferPump.ValueOverride; }
		else{ AllTanks[tank].TransferPump.ValueOut = AllTanks[tank].TransferPump.Value; }
		digitalWrite(AllTanks[tank].TransferPump.OutputPin, AllTanks[tank].TransferPump.Value);

		if (AllTanks[tank].DrainValve.OverrideEnable){ AllTanks[tank].DrainValve.ValueOut = AllTanks[tank].DrainValve.ValueOverride; }
		else{ AllTanks[tank].DrainValve.ValueOut = AllTanks[tank].DrainValve.Value; }
		digitalWrite(AllTanks[tank].DrainValve.OutputPin, AllTanks[tank].DrainValve.ValueOut);

		//if (AllTanks[tank].InnValve.OverrideEnable){ AllTanks[tank].InnValve.ValueOut = AllTanks[tank].InnValve.ValueOverride; }
		//else { AllTanks[tank].InnValve.ValueOut = AllTanks[tank].InnValve.Value; }
		//digitalWrite(AllTanks[tank].InnValve.OutputPin, AllTanks[tank].InnValve.ValueOut);
	}
#pragma endregion Setting_Outputs

}
