
String input_0_String = "";
boolean input_0_StringComplete = false;

String input_1_String = "";
boolean input_1_StringComplete = false;

String input_2_String = "";
boolean input_2_StringComplete = false;

String input_3_String = "";
boolean input_3_StringComplete = false;
bool Start = true;

void setup()
{
	Start = true;
	Serial.begin(9600);
	input_0_String.reserve(100);
	
	Serial1.begin(9600);
	input_1_String.reserve(100);

	Serial2.begin(9600);
	input_2_String.reserve(100);

	Serial3.begin(9600);
	input_3_String.reserve(100);
}

void serialEvent(){
	while (Serial.available()) {
		// get the new byte:
		char inChar = (char)Serial.read();
		// add it to the inputString:
		input_0_String += inChar;
		// if the incoming character is a newline, set a flag
		// so the main loop can do something about it:
		if (inChar == '\n') {
			input_0_StringComplete = true;
		}
	}
}

void serialEvent1(){
	while (Serial1.available()) {
		// get the new byte:
		char inChar = (char)Serial1.read();
		// add it to the inputString:
		input_1_String += inChar;
		// if the incoming character is a newline, set a flag
		// so the main loop can do something about it:
		if (inChar == '\n') {
			input_1_StringComplete = true;
		}
	}
}

void serialEvent2(){
	while (Serial2.available()) {
		// get the new byte:
		char inChar = (char)Serial2.read();
		// add it to the inputString:
		input_2_String += inChar;
		// if the incoming character is a newline, set a flag
		// so the main loop can do something about it:
		if (inChar == '\n') {
			input_2_StringComplete = true;
		}
	}
}

void serialEvent3(){
	while (Serial3.available()) {
		// get the new byte:
		char inChar = (char)Serial3.read();
		// add it to the inputString:
		input_3_String += inChar;
		// if the incoming character is a newline, set a flag
		// so the main loop can do something about it:
		if (inChar == '\n') {
			input_3_StringComplete = true;
		}
	}
}


void loop()
{
	if (input_0_StringComplete)
	{
		input_0_String.trim();
		Serial.println(input_0_String);
		if (input_0_String.startsWith("Stop"))
		{
			Start = false;
		}
		else if (input_0_String.startsWith("Start"))
		{
			Start = true;
		}
		input_0_String = "";
		input_0_StringComplete = false;
	}
	
	if (input_1_StringComplete)
	{
		input_1_String.trim();
		Serial.println(input_1_String);
		input_1_String = "";
		input_1_StringComplete = false;
	}

	if (input_2_StringComplete)
	{
		input_2_String.trim();
		Serial.println(input_2_String);
		input_2_String = "";
		input_2_StringComplete = false;
	}

	if (input_3_StringComplete)
	{
		input_3_String.trim();
		Serial.println(input_3_String);
		input_3_String = "";
		input_3_StringComplete = false;
	}
	if (Start)
	{
		Serial.println("Jobber");
	}
	
	delay(500);

  /* add main program code here */

}
