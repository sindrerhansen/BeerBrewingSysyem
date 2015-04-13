
volatile unsigned int  total_flow;
float totalLiter;
unsigned int  l_hour;          // Calculated litres/hour                      
static int flowmeter = 2;  // Flow Meter Pin number
unsigned long cloopTime;
String input_String;
bool input_StringComplete;

void flow()                  // Interruot function
{
	total_flow++;
}


void setup()
{
	pinMode(flowmeter, INPUT);
	Serial.begin(38400);
	total_flow = 0;
	attachInterrupt(0, flow, RISING); // Setup Interrupt 
	sei();                            // Enable interrupts  
	cloopTime = millis();
}

void serialEvent(){
	while (Serial.available()) {
		char inChar = (char)Serial.read();
		input_String += inChar;
		if (inChar == '\n') {
			input_StringComplete = true;
		}
	}
}
void loop()
{
	if (millis() >= (cloopTime + 500))
	{
		cloopTime = millis();			     // Updates cloopTime
        totalLiter = total_flow / 444.444 ;
        Serial.println(totalLiter);						

	}

	if (input_StringComplete)
	{
		input_String.trim();
		
		if (input_String=="x")
		{
			total_flow = 0;
		}
		input_String = "";
		input_StringComplete = false;
	}
}
