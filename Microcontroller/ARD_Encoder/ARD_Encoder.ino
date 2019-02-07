
//30.01.19 FUNGERER FOR abs(Ticks)<255

/*
   Sketch by max wolf / www.meso.net
   v. 0.1 - very basic functions - mw 20061220
*/
#include <TimerOne.h>
#include <Wire.h>
const double TICKS_PER_CM = 10.6;
int val;
int encoder0PinA = 3;
int encoder0PinB = 4;
int encoder0Pos = 0;
int encoder0PinALast = LOW;
int n = LOW;
int millisecond = 0;
int cmTravelled = 0;

void setup() {
	Wire.begin(0x30);
	Wire.onRequest(onRequestEvent);
	Serial.begin(9600);
	Timer1.initialize(1000);
	Timer1.attachInterrupt(increaseCounter);
	pinMode(encoder0PinA, INPUT);
	pinMode(encoder0PinB, INPUT);
}

void loop() {
  Serial.println("Topp av loop()");
	n = digitalRead(encoder0PinA);
	if ((encoder0PinALast == LOW) && (n == HIGH))
	{
		if (digitalRead(encoder0PinB) == LOW)
		{
			encoder0Pos--;
		}
		else
		{
			encoder0Pos++;
		}
	}
	encoder0PinALast = n;
}

void onRequestEvent()
{
	//Timer1.stop();
	byte myArray[4];
	cmTravelled = encoder0Pos/TICKS_PER_CM;
	if (cmTravelled >= 0)
	{
		myArray[0] = 0;
		myArray[1] = (cmTravelled << 8) && 0xFF;
		myArray[2] = cmTravelled & 0xFF;
	}
	else
	{
		myArray[0] = 1;
		myArray[1] = (abs(cmTravelled) << 8) && 0xFF;
		myArray[2] = abs(cmTravelled) & 0xFF;
	}
	myArray[3] = millisecond / 1000.0;
	Wire.write(myArray, 4);
	Serial.print("Encoder: ");
	Serial.println(encoder0Pos);
	Serial.print("Cm: ");
	Serial.println(cmTravelled);
	encoder0Pos = 0;
	cmTravelled = 0;
	millisecond = 0;
	
	//Timer1.restart();
}

void increaseCounter()
{
	millisecond++;
	//Serial.print("Verdi: ");
	//Serial.println(millisecond);
}
