#define LOWPASS_ANALOG_PIN_AMT 6
#define aref_voltage 3.32 
float   lowpass_prev_out[LOWPASS_ANALOG_PIN_AMT], 
        lowpass_cur_out[LOWPASS_ANALOG_PIN_AMT];
int     lowpass_input[LOWPASS_ANALOG_PIN_AMT];

int adcsample_and_lowpass(int pin, int sample_rate, int samples, float alpha, char use_previous) {
  // pin:            arduino analog pin number to sample on   (should be < LOWPASS_ANALOG_PIN_AMT)
  // sample_rate:    approximate rate to sample at (less than ~9000 for default ADC settings)
  // samples:        how many samples to take in this call
  // alpha:          lowpass alpha
  // use_previous:   If true, we continue adjusting from the most recent output value.
  //                If false, we do one extra analogRead here to prime the value.
  //   On noisy signals this non-priming value can be misleading, 
  //     and with few samples per call it may not quite adjust to a realistic value.
  //   If you want to continue with the value we saw last -- which is most valid when the
  //     value is not expected to change significantly between calls, you can use true.
  //   You may still want one initial sampling, possibly in setup(), to start from something real.
 
  float one_minus_alpha = 1.0-alpha;
  int micro_delay = max(100, (1000000/sample_rate) - 160); //with 160 being our estimate of how long a loop takes 
                      //(~110 for analogRead at the default ~9ksample/sec,  +50 grasped from thin air (TODO: test)  
  if (!use_previous) { 
    //prime with a real value (instead of letting it adjust from the value in the arrays)
    lowpass_input[pin] = analogRead(pin);
    lowpass_prev_out[pin]=lowpass_input[pin]; 
  }
 
  //Do the amount of samples, and lowpass along the way  
  int i;
  for (i=samples;i>0;i--) {
    delayMicroseconds(micro_delay);
    lowpass_input[pin] = analogRead(pin);
    lowpass_cur_out[pin] = alpha*lowpass_input[pin] + one_minus_alpha*lowpass_prev_out[pin];
    lowpass_prev_out[pin]=lowpass_cur_out[pin];
  }
  return lowpass_cur_out[pin];
}

int tempTM36(int pin, char use_previous){
  float resulting_value = adcsample_and_lowpass(pin, 1000, 2000, 0.075, use_previous); 
  float voltage = resulting_value * aref_voltage;
  voltage /= 1024.0;
  float temperatureC = (voltage - 0.5) * 100;
  temperatureC -=1.0;
  return temperatureC;
}

int resulting_value;
float ambiantTemp, coolerTemp, fermTemp;
float Calc;

void setup() {
   Serial.begin(9600);
   analogReference(EXTERNAL);
   ambiantTemp = tempTM36(0, false); //0 meaning A0 
   coolerTemp = tempTM36(1, false); //0 meaning A1 
   resulting_value = tempTM36(2, false); //0 meaning A2 
   //takes  approx. 300ms   (300 samples at approx 1000 samples/sec)
   
}

void loop(){
   ambiantTemp = tempTM36(0, true); //0 meaning A0 
   coolerTemp = tempTM36(1, true); //0 meaning A1 
   resulting_value = tempTM36(2, true); //0 meaning A2
   Serial.println(ambiantTemp);
   delay (10);
}
