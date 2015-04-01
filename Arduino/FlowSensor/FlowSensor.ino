

volatile unsigned int  flow_frequency;
volatile unsigned int  total_flow;
unsigned int  l_hour;          // Calculated litres/hour                      
static int flowmeter = 2;  // Flow Meter Pin number
unsigned long currentTime;
unsigned long cloopTime;

void flow()                  // Interruot function
{
	flow_frequency++;
	total_flow++;
}

void setup()
{
	pinMode(flowmeter, INPUT);
	Serial.begin(9600);
	flow_frequency = 0;
	total_flow = 0;
	attachInterrupt(0, flow, RISING); // Setup Interrupt 

	sei();                            // Enable interrupts  
	currentTime = millis();
	cloopTime = currentTime;
}

void loop()
{
	currentTime = millis();
	// Every second, calculate and print litres/hour
	if (currentTime >= (cloopTime + 1000))
	{
		cloopTime = currentTime;			     // Updates cloopTime
											  	// Pulse frequency (Hz) = 7.5Q, Q is flow rate in L/min. (Results in +/- 3% range)
		l_hour = (flow_frequency * 60 / 7.5);	 // (Pulse frequency x 60 min) / 7.5Q = flow rate in L/hour 
		flow_frequency = 0;						// Reset Counter
		Serial.print(l_hour, DEC);            // Print litres/hour
		Serial.println(" L/hour");
		Serial.print(total_flow, DEC);
		Serial.println(" Flow pulser");
	}
}
