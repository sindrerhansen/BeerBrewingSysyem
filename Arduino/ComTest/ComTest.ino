

String inputString = "";         // a string to hold incoming data
boolean stringComplete = false;  // whether the string is complete
unsigned long timez = 0;
int state =0;
float knobReturn = 0;

void setup() {
	// initialize serial:
	Serial.begin(9600);
	// reserve 200 bytes for the inputString:
	
	timez = millis();
}

void loop() {
	// print the string when a newline arrives:

	if ((timez + 200) < millis())
	{
		timez = millis();               
                String outString = ""; 
                outString = outString + random(100) + "\t" + random(100) + "\t" + state +"\t" + knobReturn;		
                Serial.println(outString);

	}
		if (stringComplete) {
                        inputString.trim();
			if (inputString == "start")
                        {
                          state = 1;
                        }
                        else if (inputString == "stop")
                        {
                          state = 0;
                        }
		        else if (inputString.startsWith("SCP"))
		        {
                          inputString.trim();
			  inputString.remove(0, 3);
                          inputString.replace(',', '.');
			  knobReturn = inputString.toFloat();
			  
		        }
			// clear the string:
			inputString = "";
			stringComplete = false;
		}
      delay(10);  
      }

void serialEvent() {
	while (Serial.available()) {
		// get the new byte:
		char inChar = (char)Serial.read();
		// add it to the inputString:
		inputString += inChar;
		// if the incoming character is a newline, set a flag
		// so the main loop can do something about it:
		if (inChar == '\n') {
			stringComplete = true;
		}
	}
}


