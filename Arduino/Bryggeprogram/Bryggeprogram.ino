// This code is for a Arduino Mega. By Sindre

#include <OneWire.h>
#include <DallasTemperature.h>

const int ONE_WIRE_BUS = 2;
// Setup a oneWire instance to communicate with any OneWire devices 
// (not just Maxim/Dallas temperature ICs)
OneWire oneWire(ONE_WIRE_BUS);

// Pass our oneWire reference to Dallas Temperature.
DallasTemperature TemperatureSensors(&oneWire);
struct Sequence
{
	float AddedVolumeSP;
	float HltTemperatureSP;
	float TemperatureSP;
	float TimeMinutsSP;
};

struct TankInfo
{
	float Volume;
	float TemperatureTank;
	float TemperatureHeatingRetur;
	bool LevelOverhHatingElementLevel;
	int LevelOverhHatingElementLevelPin;
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
Sequence Sparge;
Sequence Boil;

float ambientTemperature = 0;

unsigned long refTime = 0;
unsigned long refTime2 = 0;
unsigned long elapsedTimeMinutes = 0;
unsigned long elapsedTimeSeconds =0;
unsigned long timez =0;
unsigned long serialOutputTime = 500; 										
float lastTotVolume=0; 
int state = 0;
bool startBrewing = false;

String inputRAW ="";
String inputString = "";                                                   
String sensorString = "";
String sensorStringAll="";
boolean input_stringcomplete = false;
boolean sensor_stringcomplete = false;
String resivedItems[20];
                                   
void setup() {                                                                 
	Serial.begin(9600); 
	Serial1.begin(38400);  
	inputRAW.reserve(5);
    inputString.reserve(50);
	sensorString.reserve(30);                                                  //set aside some bytes for receiving data from Atlas Scientific product
	
	// Setting the HLT inn and out pins 
	Hlt.CirculationPumpPin = 20;
	Hlt.TransferPumpPin = 21;
	Hlt.DrainValvePin = 23;
	Hlt.HeatingElement1Pin = 24;
	Hlt.HeatingElement2Pin = 25;
	Hlt.LevelOverhHatingElementLevelPin = 26;
	Hlt.LevelHighPin = 27;
	// Setting the HLT inn and out
	pinMode(Hlt.CirculationPumpPin,OUTPUT);
	pinMode(Hlt.TransferPumpPin,OUTPUT);
	pinMode(Hlt.DrainValvePin,OUTPUT);
	pinMode(Hlt.HeatingElement1Pin, OUTPUT);
	pinMode(Hlt.HeatingElement2Pin,OUTPUT);
	pinMode(Hlt.LevelOverhHatingElementLevelPin, INPUT);
	pinMode(Hlt.LevelHighPin, INPUT);
	// Setting the  MashTank inn and out pins 
	
	MashTank.CirculationPumpPin = 30;
	MashTank.TransferPumpPin = 31;
	MashTank.DrainValvePin = 32;
	MashTank.HeatingElement1Pin = 33;
	MashTank.HeatingElement2Pin = 34;
	MashTank.LevelOverhHatingElementLevelPin = 35;
	MashTank.LevelHighPin = 36;
	// Setting the MashTank inn and out
	pinMode(MashTank.CirculationPumpPin, OUTPUT);
	pinMode(MashTank.TransferPumpPin, OUTPUT);
	pinMode(MashTank.DrainValvePin, OUTPUT);
	pinMode(MashTank.HeatingElement1Pin, OUTPUT);
	pinMode(MashTank.HeatingElement2Pin, OUTPUT);
	pinMode(MashTank.LevelOverhHatingElementLevelPin, INPUT);
	pinMode(MashTank.LevelHighPin, INPUT);

	
	// Setting the BoilTank inn and out pins 
	BoilTank.CirculationPumpPin = 40;
	BoilTank.TransferPumpPin = 41;
	BoilTank.DrainValvePin = 42;
	BoilTank.HeatingElement1Pin = 43;
	BoilTank.HeatingElement2Pin = 44;
	BoilTank.LevelOverhHatingElementLevelPin = 45;
	BoilTank.LevelHighPin = 46;
	// Setting the BoilTank inn and out
	pinMode(BoilTank.CirculationPumpPin, OUTPUT);
	pinMode(BoilTank.TransferPumpPin, OUTPUT);
	pinMode(BoilTank.DrainValvePin, OUTPUT);
	pinMode(BoilTank.HeatingElement1Pin, OUTPUT);
	pinMode(BoilTank.HeatingElement2Pin, OUTPUT);
	pinMode(BoilTank.LevelOverhHatingElementLevelPin, INPUT);
	pinMode(BoilTank.LevelHighPin, INPUT);
	
	
	timez=millis(); 
	
	TemperatureSensors.begin();
}

void serialEvent() {                                                          
	char inchar = (char)Serial.read();                                         
	inputString += inchar;
	if(inchar == '\n') {
	input_stringcomplete = true;      
	}                           
}
 

void serialEvent1() {														
	char senschar = (char)Serial1.read();                                         
	sensorString += senschar;
	if(senschar == '\r') {
	sensor_stringcomplete = true;     
	}                           
}

void loop() {
	
	if (input_stringcomplete)
	{
        inputString.trim();
		int index = 0;
		int deviderIndex = 0;
		String systemDevider = "_";

		if (inputString.startsWith("CMD"))
		{
			inputString.remove(0, 2);
			int CMD = inputString.toInt();

			if (CMD = 10 && state < 10)
			{
				state = 10;
			}
			else if (CMD = 11)
			{
				startBrewing = true;
			}
		}
		
		else if (inputString.startsWith("STA"))
		{
			inputString.remove(0, 2);
			int STA = inputString.toInt();
			state = STA;
		}
		else
		{
			for (unsigned int i = 0; i <= inputString.length(); i++)
			{
				if (inputString.substring(i, i + 1) == systemDevider)
				{
					resivedItems[index] = inputString.substring(deviderIndex, i);
					index++;
					deviderIndex = i + 1;
				}
			}

			for (int i = 0; i < 10; i++)
			{
				interfaceConvertion(&MashInn.TemperatureSP, resivedItems[i], "MITe");
				interfaceConvertion(&MashInn.AddedVolumeSP, resivedItems[i], "MIVo");
				interfaceConvertion(&MashStep1.TemperatureSP, resivedItems[i], "M1Te");
				interfaceConvertion(&MashStep1.TimeMinutsSP, resivedItems[i], "M1Ti");
				interfaceConvertion(&MashStep2.TemperatureSP, resivedItems[i], "M2Te");
				interfaceConvertion(&MashStep2.TimeMinutsSP, resivedItems[i], "M2Ti");
				interfaceConvertion(&Sparge.TemperatureSP, resivedItems[i], "SpTi");
				interfaceConvertion(&Sparge.AddedVolumeSP, resivedItems[i], "SpVo");
				interfaceConvertion(&Boil.TimeMinutsSP, resivedItems[i], "BoTi");
			}
		}
		inputString = "";                                                        //clear the string:
		input_stringcomplete = false;                                            //reset the flag used to tell if we have received a completed string from the PC
	}
  
  	if (sensor_stringcomplete){                                                //if a string from the Atlas Scientific product has been received in its entirety
		sensorStringAll=sensorString;
		int commaPosition = sensorString.indexOf(',');
		if(commaPosition != -1)
		{
			String totalVolume = sensorString.substring(0,commaPosition);
			char floatbuf[32];
			totalVolume.toCharArray(floatbuf, sizeof(floatbuf));
			float totVolumeTemp = atof(floatbuf);
			
			if (abs(lastTotVolume-totVolumeTemp)<0.3)
			{
				MashTank.Volume=totVolumeTemp;
			}
			lastTotVolume= totVolumeTemp;

		}
		sensorString = "";                                                      //clear the string:
		sensor_stringcomplete = false;                                          //reset the flag used to tell if we have received a completed string from the Atlas Scientific product
	}
	
	// Getting Temperatures
	TemperatureSensors.requestTemperatures();

	Hlt.TemperatureTank = TemperatureSensors.getTempCByIndex(0);
	MashTank.TemperatureTank = TemperatureSensors.getTempCByIndex(1);
	MashTank.TemperatureHeatingRetur = TemperatureSensors.getTempCByIndex(2);
	BoilTank.TemperatureTank = TemperatureSensors.getTempCByIndex(3);
	ambientTemperature = TemperatureSensors.getTempCByIndex(4);

	switch (state)
	{
		case 0:
			// ideal state nothing is happening 
		break; 
  
		case 10:
			if  (Hlt.TemperatureTank < MashInn.HltTemperatureSP)
			{
				Hlt.HeatingElement1 = true;
				Hlt.HeatingElement2 = true;
				
			}
			else if (startBrewing){			
				state = 20;
			}

																   
			Hlt.CirculationPump = true;							// start circulations pump 100%                                   
		
		break;  
    
	//	case 20:// Meshing in
	//		hltCirculationPump = 255;                                                 // start circulation pump
	//		hltTransferPump = true;                                            // open output valve hwt
	//		info = "Adding water to mash tank at " + String(hltTemp) + "°C";
	//		if (mashTankTemp < mashTempMashTank_SP){
	//			mashHeatElement1 = true;											// mesh tank heater high
	//		} 

	//		if (totVolume > 15){
	//	//		mashPump = 255;
	//			mashCirculationValve = true;
	//		}
	//		if (totVolume > mashVolume_SP){                                       // when mesh volume reached go to case 30
	//			state = 30;
	//			refTime = millis();    // start timer 
	//		}

	//		                                                   
	//	break;     
 //   

 // 
	//	case 30:
	//	elapsedTimeSeconds = (millis()-refTime)/1000;
	//	elapsedTimeMinutes = elapsedTimeSeconds/60;
	//	                                                // start circulation pump 100%   
	////	hwtCirculationPump = true;
	////	mashPump = 150;
	//	mashCirculationValve = true;
	//	                                   
	//	info = "Mash for " + String(elapsedTimeMinutes) + " of " + String(mashTime_SP) + " min. ";
	//	if (mashTankTemp < mashTempMashTank_SP){
	//		mashHeatElement1 = true;
	//	}                                                   			// mesh tank heater high
	//	if (stepMashMode){
	//		if  (hltTemp < spargeTempHwt_SP){                              // heat hwt to mash out rise temp
	//		hltHeatElement = true;
	//		info = info + "Preparing for stepMash";                                      // regulate heating element
	//		}
	//		else {
	//			info = info + "Ready for stepMash";
	//		}
	//	}
	//	else {
	//		if  (hltTemp < mashOutRiseTempHwt_SP){                              // heat hwt to sparge temp
	//		hltHeatElement = true;                                      // regulate heating element
	//		}
	//	}
	//	if  (elapsedTimeMinutes >= mashTime_SP)
	//	{
	//		state = 40;
	//	}
	//	// when timer expired and hwlt is at sparge rise temp, go to case 40 
	//	break;
 //   
	//	case 40:
	//		if (stepMashMode){
	//			state = 41;
	//		}
	//		else {
	//			state = 42;
	//		}
	//		
	//	break;

	//	case 41:  // rise to mash out temp                                               
	//		
	//		hltCirculationPump = 255;   
	////		mashPump = 255;                                              
	//		mashCirculationValve = true;
	//		
	//		if  (hltTemp < spargeTempHwt_SP){                                          // heat hwlt to sparge rise temp
	//			hltHeatElement = true;                                             // regulate heating element
	//		}
	//		if (mashTankTemp < mashOutTempMashTank_SP){
	//			mashHeatElement1 = true;
	//		}                                                      // mesh tank heater high
	//		else{
	//			state = 50;														// when mash out temp is archived, go to case 50
	//		} 
 //                
	//	break;

	//	case 42:
	//		
	//	//	mashPump = 255;
	//		mashCirculationValve = true;
	//	//	hwtCirculationPump = true;

	//		if (mashTankTemp < mashOutTempMashTank_SP){
	//			mashHeatElement1 = true;
	//		}

	//		if (hltTemp < mashOutRiseTempHwt_SP){
	//			hltHeatElement = true;
	//		}

	//		else{
	//			
	//			state = 43; 
	//		}
	//	break;

	//	case 43:
	//		hltCirculationPump = 255;
	//		hltTransferPump = true;
	//	//	mashPump = 255;
	//		mashCirculationValve = true;
	//		
	//		if (mashTankTemp < mashOutTempMashTank_SP)
	//		{
	//			mashHeatElement1 = true;
	//		} 

	//		if (totVolume > (mashVolume_SP + mashOutRiseVolume_SP)){
	//			refTime = millis();    // start timer 
	//			refTime2 = refTime;
	//			state = 50;
	//		}
	//	break;

 //   
	//	case 50:                                                                // prepare sparging
	//	//	hwtCirculationPump = true;                                         // open circulation valve on hwt
	//	//	mashPump = 255;
	//		mashCirculationValve = true;
	//		
	//		elapsedTimeSeconds = (millis()-refTime)/1000;
	//		elapsedTimeMinutes = elapsedTimeSeconds/60; 
	//		
	//		if  (hltTemp < spargeTempHwt_SP)                                          // heat hwt to sparge rise temp
	//		{
	//			hltHeatElement = true;                                             // regulate heating element
	//		}			
	//		else if ((hltTemp - 2.0) > spargeTempHwt_SP){
 //   		// add cold water in pulses on, 10 sec pause until the temp is at sparge time
 //   			unsigned long dif = hltTemp - spargeTempHwt_SP;

	//	    	if (millis() > (refTime2 + 10000)){
 //     				hltInValve = true;
 //     			}
 //     			if (millis()>(refTime2 + 11000 + dif * 100)){
 //     			hltInValve = false;
 //     			refTime2 = millis();
 //     			}
 //   
	//		}

	//		if (mashTankTemp < mashOutTempMashTank_SP)
	//		{
	//			mashHeatElement1 = true;
	//		}                                                     
	//		
	//		                                     
	//		
	//		info = "Mashing out " + String(elapsedTimeMinutes) + " of " + String(mashOutTime_SP) + " min.";

	//		if ((hltTemp < (spargeTempHwt_SP +0.5)) && (hltTemp > (spargeTempHwt_SP - 0.5)))
	//		{
	//			spargeTempInRange = true;
	//			info = info + " HWT ready to sparge";
	//		}
	//		if  ((spargeTempInRange) && (elapsedTimeMinutes >= mashOutTime_SP))// sparge timer and sparge time is ok, go to case 60
	//		{
	//			state = 60;
	//		}
 //   
	//	break;
 //   
	//	case 60:
	//		hltCirculationPump = 255;                                                      // start circulation pump 75%
	//		hltTransferPump = true;                                                  // open output valve hwt
	//	//	mashPump = 190;
	//		
	//		mashOutValve = true;
	//		
 //           if (mashTankTemp < mashOutTempMashTank_SP){
	//			mashHeatElement1 = true;
	//		}                                                       // mesh tank heater high

	//		if (totVolume > (mashVolume_SP + mashOutRiseVolume_SP + spargeVolume_SP))                        // when sparge volume is archived, go to case 70
	//		{
	//			state = 70;
	//		}
	//		addedSpargeVolum = totVolume - mashVolume_SP - mashOutRiseVolume_SP;
	//		info = "Adding " + String(addedSpargeVolum) + " liter of " + String(spargeVolume_SP);
	//	break;
 //    
	//	case 70:
	//	//	mashPump = 255;
	//		mashOutValve = true;
	//		
	//		
 //  
	//	break;
 //  
	//	case 80:
	//		// set boil element
	//		// when boilTank temp > 98 grader, go to case 90
 //  
	//	break;
 //  
	//	case 90:
	//		// set boil element
	//		// Start boil timer
	//		// when boil timer ended, go to case 100
 //  
	//	break;
 //  
	//	case 100:

 //  
	//	break;

 //       case 999: // Override
	//		hltTransferPump = hltTransferPumpOverride;
	//		hltCirculationPump = hltCirculationPumpOverride;
	//	//	mashPump = mashPumpOverride;
	//		mashOutValve = mashOutValveOverride;
	//		mashCirculationValve = mashCirculationValveOverride;
 //       break;
	//	
		default:
			state=0;
		break;
	} // Sequens for brewing beer
}

void interfaceConvertion(float *value, String _string, String hedder)
{
	if (_string.startsWith(hedder))
	{
		_string.remove(0, 3);
		*value = (_string.toFloat() / 10);
	}
}