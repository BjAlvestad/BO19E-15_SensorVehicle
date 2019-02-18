
//30.01.19 FUNGERER FOR abs(Ticks)<255

/*
   Sketch by max wolf / www.meso.net
   v. 0.1 - very basic functions - mw 20061220
*/
#include <TimerOne.h>
#include <Wire.h>
const double TICKS_PER_CM = 10.6;
const int ADDRESS = 0x30;
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

void loop() 
{
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
	byte myArray[9];
	cmTravelled = encoder0Pos / TICKS_PER_CM;

	myArray[0] = ADDRESS;
	myArray[1] = (cmTravelled >> 24);
	myArray[2] = (cmTravelled >> 16);
	myArray[3] = (cmTravelled >> 8);
	myArray[4] = (cmTravelled);

	myArray[5] = (millisecond >> 24); //Most significant byte
	myArray[6] = (millisecond >> 16);
	myArray[7] = (millisecond >> 8);
	myArray[8] = (millisecond);		  //Least significant byte

	Wire.write(myArray, 9);

	encoder0Pos = 0;
	millisecond = 0;
}

void increaseCounter()
{
	millisecond++;
}
