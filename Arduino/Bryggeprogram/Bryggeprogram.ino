// This code is for a Arduino Mega. By Sindre

#pragma region Constants
const long prePumpeTimeSparge = 20;
const int flowOfSet = 0.2;
const int MashCirculationStartTreshold = 2;
const float BoilTempThreshold = 97.0;
const int SerialSendingRate = 500; // Milliseconds

#pragma endregion Constants

#pragma region Structs
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
#pragma endregion Structs

#pragma region Declering Tanks
TankInfo Hlt;
TankInfo MashTank;
TankInfo BoilTank;
TankInfo AllTanks[3];
#pragma endregion Declering Tanks

#pragma region Declering Sequences
Sequence MashInn;
Sequence MashStep1;
Sequence MashStep2;
Sequence MashStep3;
Sequence MashStep4;
Sequence Sparge;
Sequence Boil;
#pragma endregion Declering Sequences

#pragma region Declaring Variables
float ambientTemperature = 0;
long refTime = 0;
long refTime2 = 0;
long elapsedTimeMinutes = 0;
long elapsedTimeSeconds = 0;
long timeSpan = 0;
long remainingTime = 0;
unsigned long timez = 0;
unsigned long cloopTime;
float lastTotVolume = 0;
float totalAddedVolume = 0;
int state = 0;
bool startBrewing = false;
bool messageConfirmd = false;

String input_0_String = "";
boolean input_0_StringComplete = false;

String input_1_String = "";
boolean input_1_StringComplete = false;

String input_2_String = "";
boolean input_2_StringComplete = false;

String input_3_String = "";
boolean input_3_StringComplete = false;

static char systemDevider = '_';
static char valueDevider = ':';

String resivedItems[20];
String AllInfoString = "";
String MessageToUser = "";

int previouslyState = 0;

#pragma endregion Declaring Variables

void setup() {
	
	Serial.begin(9600);
	input_0_String.reserve(200);

	Serial1.begin(38400);
	input_1_String.reserve(200);

	Serial2.begin(9600);
	input_2_String.reserve(200);

	Serial3.begin(9600);
	input_3_String.reserve(200);

	timez = millis();
	cloopTime = millis();


#pragma region Init_HLT
	// Setting the HLT inn and out pins 
	Hlt.CirculationPump.OutputPin = 4;
	Hlt.TransferPump.OutputPin = 5;
	Hlt.Element1.OutputPin = 20;
	Hlt.Element2.OutputPin = 21;
	Hlt.DrainValve.OutputPin = 26;
	Hlt.LevelOverHeatingElements.InputPin = 22;
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

void serialEvent(){
	while (Serial.available()) {
		char inChar = (char)Serial.read();
		input_0_String += inChar;
		if (inChar == '\n') {
			input_0_StringComplete = true;
		}
	}
}

void serialEvent1(){
	while (Serial1.available()) {
		char inChar = (char)Serial1.read();
		input_1_String += inChar;
		if (inChar == '\r') {
			input_1_StringComplete = true;
		}
	}
}

void serialEvent2(){
	while (Serial2.available()) {
		char inChar = (char)Serial2.read();
		input_2_String += inChar;
		if (inChar == '\n') {
			input_2_StringComplete = true;
		}
	}
}

void serialEvent3(){
	while (Serial3.available()) {
		char inChar = (char)Serial3.read();
		input_3_String += inChar;
		if (inChar == '\n') {
			input_3_StringComplete = true;
		}
	}
}

void loop() {
#pragma region Resetting outputs 
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
	totalAddedVolume = MashInn.AddVolumeSP + MashStep1.AddVolumeSP + MashStep2.AddVolumeSP + MashStep3.AddVolumeSP + MashStep4.AddVolumeSP + Sparge.AddVolumeSP;

	MessageToUser = "";
#pragma endregion Resetting outputs

	if (input_0_StringComplete)
	{
		input_0_String.trim();
		int index = 0;
		int deviderIndex = 0;
		
		if (input_0_String.startsWith("CMD"))
		{
			input_0_String.remove(0, 3);
			int CMD = input_0_String.toInt();
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

		else if (input_0_String.startsWith("CONFIRMED"))
		{
			messageConfirmd = true;
		}

		else if (input_0_String.startsWith("STA"))
		{
			input_0_String.remove(0, 3);
			int STA = input_0_String.toInt();
			state = STA;
		}

		else if (input_0_String.startsWith("SET"))
		{
			input_0_String.remove(0, 3);
			for (unsigned int i = 0; i <= input_0_String.length(); i++)
			{
				if (systemDevider == input_0_String.charAt(i))
				{
					resivedItems[index] = input_0_String.substring(deviderIndex, i);
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

			AllInfoString += "ConSe";
			AllInfoString += String(MashInn.TemperatureSP) + valueDevider;
			AllInfoString += String(MashInn.HltTemperatureSP) + valueDevider;
			AllInfoString += String(MashInn.AddVolumeSP) + valueDevider;
			AllInfoString += String(MashStep1.TemperatureSP) + valueDevider;
			AllInfoString += String(MashStep1.TimeMinutsSP) + valueDevider;
			AllInfoString += String(MashStep2.TemperatureSP) + valueDevider;
			AllInfoString += String(MashStep2.TimeMinutsSP) + valueDevider;
			AllInfoString += String(MashStep3.TemperatureSP) + valueDevider;
			AllInfoString += String(MashStep3.TimeMinutsSP) + valueDevider;
			AllInfoString += String(Sparge.TemperatureSP) + valueDevider;
			AllInfoString += String(Sparge.AddVolumeSP) + valueDevider;
			AllInfoString += String(Boil.TimeMinutsSP) + valueDevider;
			AllInfoString += systemDevider;
		//	Serial.println(sendMessage);
		}

		else if (input_0_String.startsWith("OVERRIDE"))
		{
			input_0_String.remove(0, 8);
			input_0_String.trim();
			int _overrideCMD = input_0_String.toInt();
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

		else if (input_0_String.startsWith("FLOW")) //String FLOW_
		{
			input_0_String.remove(0, 5);
			if (input_0_String.startsWith("RES"))
			{
				Serial1.println("x");
			}
		}

		input_0_String = "";                                                        //clear the string:
		input_0_StringComplete = false;                                            //reset the flag used to tell if we have received a completed string from the PC
	}

	if (input_1_StringComplete)
	{
		input_1_String.trim();
		String _totalVolume = input_1_String.substring(0, 4);
		_totalVolume.trim();
		float _volume = _totalVolume.toFloat();
		if (abs(lastTotVolume - _volume) < 0.5)
		{
			MashTank.Volume = _volume;
		}
		lastTotVolume = _volume;
		input_1_String = "";
		input_1_StringComplete = false;
	}

	if (input_2_StringComplete)
	{
		int valueStartIndex = 0;
		int conter = 0;
		String _resiveArray[5];
		input_2_String.trim();
		for (int i = 0; i <= input_2_String.length(); i++)
		{ 
			if (valueDevider == input_2_String.charAt(i))
			{
				_resiveArray[conter] = input_2_String.substring(valueStartIndex, i);
				valueStartIndex = i + 1;
				conter++;
			}
		}
		if (_resiveArray[0].toFloat()>=0)
		{
			Hlt.TemperatureTank = _resiveArray[0].toFloat();
		}

		if (_resiveArray[1].toFloat() >= 0)
		{
			MashTank.TemperatureTank = _resiveArray[1].toFloat();
		}

		if (_resiveArray[2].toFloat() >= 0)
		{
			MashTank.TemperatureHeatingRetur = _resiveArray[2].toFloat();
		}

		if (_resiveArray[3].toFloat() >= 0)
		{
			BoilTank.TemperatureTank = _resiveArray[3].toFloat();
		}

		if (_resiveArray[4].toFloat() >= 0)
		{
			ambientTemperature = _resiveArray[4].toFloat();
		}

		input_2_String = "";
		input_2_StringComplete = false;
	}

	if (input_3_StringComplete)
	{
		int valueStartIndex = 0;
		int conter = 0;
		String _resiveArray[5];
		input_3_String.trim();
		for (int i = 0; i <= input_3_String.length(); i++)
		{
			if (valueDevider == input_3_String.charAt(i))
			{
				_resiveArray[conter] = input_2_String.substring(valueStartIndex, i);
				valueStartIndex = i + 1;
				conter++;
			}
		}

		input_3_String = "";
		input_3_StringComplete = false;
	}
#pragma region Reading Digital Sensors

	BoilTank.LevelOverHeatingElements.State = digitalRead(BoilTank.LevelOverHeatingElements.InputPin);
	Hlt.LevelOverHeatingElements.State = digitalRead(Hlt.LevelOverHeatingElements.InputPin);

#pragma endregion Reading Digital Sensors

	switch (state)
	{
	case 0:
		
		if (previouslyState!=state)
		{
			previouslyState = state;
		}

		// ideal state nothing is happening 
		
		break;
		
	case 10: // Prepar HLT tank temperature

		if (previouslyState != state)
		{
			previouslyState = state;
		}
		Hlt.CirculationPump.Value = true;
		Hlt.TemperatureTankSetPoint = MashInn.HltTemperatureSP;

		Hlt.Element1.Value = TankTemperaturRegulator(Hlt.TemperatureTankSetPoint, Hlt.TemperatureTank, Hlt.LevelOverHeatingElements.State);

		if (startBrewing){
			state = 20;
		}
		
		break;

	case 20: // Transfering water from HLT to Mash tank, waiting for grain
		
		if (previouslyState != state)
		{
			previouslyState = state;
		}
		Hlt.CirculationPump.Value = true;
		
		MashTank.TemperatureTankSetPoint = MashInn.TemperatureSP;
		Hlt.TemperatureTankSetPoint = MashInn.HltTemperatureSP;

		Hlt.Element1.Value = TankTemperaturRegulator(Hlt.TemperatureTankSetPoint, Hlt.TemperatureTank, Hlt.LevelOverHeatingElements.State);

		if (MashTank.Volume > MashCirculationStartTreshold)
		{
			MashTank.CirculationPump.Value = true;
			MashTank.Element1.Value = TankTemperaturRegulator(MashTank.TemperatureTankSetPoint, MashTank.TemperatureTank, true);			
		}
		if (MashTank.Volume + flowOfSet < MashInn.AddVolumeSP)
		{
			Hlt.TransferPump.Value = true;
		}


		if ((MashTank.Volume + flowOfSet >= MashInn.AddVolumeSP) && (MashTank.TemperatureTank >= MashTank.TemperatureTankSetPoint))
		{
			
			MessageToUser += "Add grain";
			if (messageConfirmd)
			{
				state = 30;
				messageConfirmd = false;			
			}
		}
		
		break;

	case 30://Mash step 1 timer and temp regulator
		
		if (previouslyState != state)
		{
			previouslyState = state;
			refTime = millis();    // start timer 
		}

		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		Hlt.TemperatureTankSetPoint = Sparge.HltTemperatureSP;
		MashTank.TemperatureTankSetPoint = MashStep1.TemperatureSP;
		timeSpan = MashStep1.TimeMinutsSP * 60;
		remainingTime = timeSpan - elapsedTimeSeconds;
		Hlt.CirculationPump.Value = true;

		//sendMessage += "TimSp" + String(timeSpan) + systemDevider;

		Hlt.Element1.Value = TankTemperaturRegulator(Hlt.TemperatureTankSetPoint, Hlt.TemperatureTank, Hlt.LevelOverHeatingElements.State);

		MashTank.CirculationPump.Value = true;
		if (MashTank.TemperatureTank < MashTank.TemperatureTankSetPoint){
			MashTank.Element1.Value = true;
		}
		
		
		if (remainingTime <= 0)
		{
			state = 31;
		}
		
		break;

	case 31: //Heating Mash to next setpoint (Step 2)
		
		if (previouslyState != state)
		{
			previouslyState = state;
			refTime = millis();
		}

		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		remainingTime = -elapsedTimeSeconds;
		Hlt.TemperatureTankSetPoint = Sparge.HltTemperatureSP;
		MashTank.TemperatureTankSetPoint = MashStep2.TemperatureSP;
		Hlt.CirculationPump.Value = true;

		Hlt.Element1.Value = TankTemperaturRegulator(Hlt.TemperatureTankSetPoint, Hlt.TemperatureTank, Hlt.LevelOverHeatingElements.State);

		MashTank.CirculationPump.Value = true;
		if (MashTank.TemperatureTank < MashTank.TemperatureTankSetPoint){
			MashTank.Element1.Value = true;
			MashTank.Element2.Value = true;
		}

		if (MashTank.TemperatureTank >= MashTank.TemperatureTankSetPoint)
		{
			state = 32;
		}
		
		break;
	
	case 32://Mash step 2 timer and temp regulator
		
		if (previouslyState != state)
		{
			previouslyState = state;
			refTime = millis();
		}

		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		Hlt.TemperatureTankSetPoint = Sparge.HltTemperatureSP;
		MashTank.TemperatureTankSetPoint = MashStep2.TemperatureSP;
		timeSpan = MashStep2.TimeMinutsSP * 60;
		remainingTime = timeSpan - elapsedTimeSeconds;

		Hlt.CirculationPump.Value = true;

		Hlt.Element1.Value = TankTemperaturRegulator(Hlt.TemperatureTankSetPoint, Hlt.TemperatureTank, Hlt.LevelOverHeatingElements.State);

		MashTank.CirculationPump.Value = true;
		if (MashTank.TemperatureTank < MashTank.TemperatureTankSetPoint){
			MashTank.Element1.Value = true;
		}

		if (remainingTime <= 0)
		{
			state = 33;
		}
		previouslyState = state;
		break;
	
	case 33://Heating Mash to next setpoint (Step 3)
		
		if (previouslyState != state)
		{
			previouslyState = state;
			refTime = millis();
		}

		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		remainingTime = -elapsedTimeSeconds;
		Hlt.TemperatureTankSetPoint = Sparge.HltTemperatureSP;
		MashTank.TemperatureTankSetPoint = MashStep3.TemperatureSP;

		Hlt.CirculationPump.Value = true;
		Hlt.Element1.Value = TankTemperaturRegulator(Hlt.TemperatureTankSetPoint, Hlt.TemperatureTank, Hlt.LevelOverHeatingElements.State);

		MashTank.CirculationPump.Value = true;
		if (MashTank.TemperatureTank < MashTank.TemperatureTankSetPoint){
			MashTank.Element1.Value = true;
			MashTank.Element2.Value = true;
		}

		if (MashTank.TemperatureTank >= MashTank.TemperatureTankSetPoint)
		{
			state = 34;
		}
		
		break;

	case 34://Mash step 3 timer and temp regulator
		
		if (previouslyState != state)
		{
			previouslyState = state;
			refTime = millis();
		}

		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		Hlt.TemperatureTankSetPoint = Sparge.HltTemperatureSP;
		MashTank.TemperatureTankSetPoint = MashStep3.TemperatureSP;
		timeSpan = MashStep3.TimeMinutsSP * 60;
		remainingTime = timeSpan - elapsedTimeSeconds;

		Hlt.CirculationPump.Value = true;

		Hlt.Element1.Value = TankTemperaturRegulator(Hlt.TemperatureTankSetPoint, Hlt.TemperatureTank, Hlt.LevelOverHeatingElements.State);

		MashTank.CirculationPump.Value = true;
		if (MashTank.TemperatureTank < MashTank.TemperatureTankSetPoint){
			MashTank.Element1.Value = true;
		}

		if (remainingTime <= 0)
		{
			state = 35;
		}
		
		break;
	
	case 35: //Heating Mash to next setpoint (Step 4)
		
		if (previouslyState != state)
		{
			previouslyState = state;
			refTime = millis();

		}
		previouslyState = state;

		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		remainingTime = -elapsedTimeSeconds;
		Hlt.TemperatureTankSetPoint = Sparge.HltTemperatureSP;
		MashTank.TemperatureTankSetPoint = MashStep4.TemperatureSP;

		Hlt.CirculationPump.Value = true;
		Hlt.Element1.Value = TankTemperaturRegulator(Hlt.TemperatureTankSetPoint, Hlt.TemperatureTank, Hlt.LevelOverHeatingElements.State);

		MashTank.CirculationPump.Value = true;
		if (MashTank.TemperatureTank < MashTank.TemperatureTankSetPoint){
			MashTank.Element1.Value = true;
			MashTank.Element2.Value = true;
		}

		if (MashTank.TemperatureTank >= MashTank.TemperatureTankSetPoint)
		{
			state = 36;
		}
		
		break;

	case 36: //Mash step 4 timer and temp regulator
	
		if (previouslyState != state)
		{
			previouslyState = state;
			refTime = millis();
		}

		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		Hlt.TemperatureTankSetPoint = Sparge.HltTemperatureSP;
		MashTank.TemperatureTankSetPoint = MashStep4.TemperatureSP;
		timeSpan = MashStep4.TimeMinutsSP * 60;
		remainingTime = timeSpan - elapsedTimeSeconds;

		Hlt.CirculationPump.Value = true;

		Hlt.Element1.Value = TankTemperaturRegulator(Hlt.TemperatureTankSetPoint, Hlt.TemperatureTank, Hlt.LevelOverHeatingElements.State);

		MashTank.CirculationPump.Value = true;
		if (MashTank.TemperatureTank < MashTank.TemperatureTankSetPoint){
			MashTank.Element1.Value = true;
		}

		if (remainingTime <= 0)
		{
			state = 40;
		}
		
		break;

	case 40: //Pre sparge transfer
		if (previouslyState != state)
		{
			previouslyState = state;
			refTime = millis();
		}

		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		Hlt.TemperatureTankSetPoint = Sparge.HltTemperatureSP;
		MashTank.TemperatureTankSetPoint = Sparge.TemperatureSP;
		timeSpan = MashTank.Volume * 2;
		remainingTime = timeSpan - elapsedTimeSeconds;

		Hlt.CirculationPump.Value = true;
		Hlt.Element1.Value = TankTemperaturRegulator(Hlt.TemperatureTankSetPoint, Hlt.TemperatureTank, Hlt.LevelOverHeatingElements.State);

		
		MashTank.TransferPump.Value = true;

		if (remainingTime < 0)
		{			
			state = 41;
		}
		
		break;

	case 41: // Sparge

		if (previouslyState != state)
		{
			previouslyState = state;
			refTime = millis();		
		}

		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		Hlt.TemperatureTankSetPoint = Sparge.HltTemperatureSP;
		MashTank.TemperatureTankSetPoint = Sparge.TemperatureSP;
		timeSpan = totalAddedVolume * 10;
		remainingTime = timeSpan - elapsedTimeSeconds;
		

		MashTank.TransferPump.Value = true;			
		if (MashTank.Volume<(MashInn.AddVolumeSP + Sparge.AddVolumeSP))
		{
			Hlt.TransferPump.Value = true;
		}
				
		if (BoilTank.LevelOverHeatingElements.State)
		{
			BoilTank.Element1.Value = true;
			BoilTank.Element2.Value = true;
		}

		if (remainingTime <= 0)
		{
			state = 50;
		}
		
		break;

	case 50://Pre boil getting up to boil temp
		if (previouslyState != state)
		{
			previouslyState = state;
			refTime = millis();
		}

		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;	
		remainingTime = - elapsedTimeSeconds;
		Boil.TemperatureSP = BoilTempThreshold;
		BoilTank.TemperatureTankSetPoint = Boil.TemperatureSP;
		

		
		if (BoilTank.LevelOverHeatingElements.State)
		{
			BoilTank.Element1.Value = true;
			BoilTank.Element2.Value = true;
		}

		if (BoilTank.TemperatureTank>Boil.TemperatureSP)
		{
			state = 51;
		}
		
		break;

	case 51:  	

		if (previouslyState != state)
		{
			previouslyState = state;
			refTime = millis();
		}

		elapsedTimeSeconds = (millis() - refTime) / 1000;
		elapsedTimeMinutes = elapsedTimeSeconds / 60;
		timeSpan = Boil.TimeMinutsSP * 60;
		remainingTime = timeSpan - elapsedTimeSeconds;

		if (BoilTank.LevelOverHeatingElements.State)
		{
			BoilTank.Element1.Value = true;
			if (BoilTank.TemperatureTank<BoilTank.TemperatureTankSetPoint+0.5)
			{
				BoilTank.Element2.Value = true;
			}
			
		}
		
		if (remainingTime <= 600)
		{
			BoilTank.TransferPump.Value = true;
			if (BoilTank.LevelOverHeatingElements.State)
			{
				BoilTank.Element2.Value = true;
			}
			
		}

		if (remainingTime <= 0)
		{
			refTime = millis();
			state = 52;
		}
		
		break;
	case 52:
		
		if (previouslyState != state)
		{
			previouslyState = state;
			refTime = millis();
		}
		
		BoilTank.TransferPump.Value = true;

		break;
	default:
		state = 0;
		break;
	}

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

#pragma region SendingMessageToSerial
	if (millis() >= (cloopTime + SerialSendingRate))
	{
		cloopTime = millis();			     // Updates cloopTime

		AllInfoString += "HltTe" + String(Hlt.TemperatureTank) + systemDevider;
		AllInfoString += "MatTe" + String(MashTank.TemperatureTank) + systemDevider;
		AllInfoString += "MarTe" + String(MashTank.TemperatureHeatingRetur) + systemDevider;
		AllInfoString += "BotTe" + String(BoilTank.TemperatureTank) + systemDevider;
		AllInfoString += "AmbTe" + String(ambientTemperature) + systemDevider;

		AllInfoString += "STATE" + String(state) + systemDevider;
		AllInfoString += "Messa" + MessageToUser + systemDevider;;

		AllInfoString += "HltSp" + String(Hlt.TemperatureTankSetPoint) + systemDevider;
		AllInfoString += "HltE1" + String(Hlt.Element1.Value) + systemDevider;
		AllInfoString += "HltCp" + String(Hlt.CirculationPump.Value) + systemDevider;
		AllInfoString += "HltTp" + String(Hlt.TransferPump.Value) + systemDevider;

		AllInfoString += "MatSp" + String(MashTank.TemperatureTankSetPoint) + systemDevider;
		AllInfoString += "MatE1" + String(MashTank.Element1.Value) + systemDevider;
		AllInfoString += "MatCp" + String(MashTank.CirculationPump.Value) + systemDevider;
		AllInfoString += "MatTp" + String(MashTank.TransferPump.Value) + systemDevider;
		AllInfoString += "MatVo" + String(MashTank.Volume) + systemDevider;

		AllInfoString += "BotSp" + String(BoilTank.TemperatureTankSetPoint) + systemDevider;
		AllInfoString += "BotE1" + String(BoilTank.Element1.Value) + systemDevider;
		AllInfoString += "BotCp" + String(BoilTank.CirculationPump.Value) + systemDevider;
		AllInfoString += "BotTp" + String(BoilTank.TransferPump.Value) + systemDevider;
		AllInfoString += "BotVo" + String(BoilTank.Volume) + systemDevider;

		AllInfoString += "TimSp" + String(timeSpan) + systemDevider;
		AllInfoString += "Timer" + String(elapsedTimeSeconds) + systemDevider;
		AllInfoString += "RemTi" + String(remainingTime) + systemDevider;

		Serial.println(AllInfoString);
		AllInfoString = "";

	}

#pragma endregion SendingMessageToSerial
	delay(5);
}

bool TankTemperaturRegulator(double setpoint, double actual, bool overElement)
{
	bool ret;
	if (actual<setpoint)
	{
		if (overElement)
		{
			ret= true;
		}
		else
		{
			ret= false;
		}
	}
	else
	{
		ret= false;
	}

	if (!overElement)
	{
		MessageToUser = "Add water to hot liquor tank!!";
	}
	return ret;
}
