
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
	Wire.begin(ADDRESS);
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
	byte myArray[10];
	cmTravelled = encoder0Pos / TICKS_PER_CM;

	myArray[0] = ADDRESS;
	myArray[1] = 22; //TODO: Change this to valid message code
	myArray[2] = 2;
	myArray[3] = (cmTravelled >> 24);
	myArray[4] = (cmTravelled >> 16);
	myArray[5] = (cmTravelled >> 8);
	myArray[6] = (cmTravelled);

	myArray[7] = (millisecond >> 24); //Most significant byte
	myArray[8] = (millisecond >> 16);
	myArray[9] = (millisecond >> 8);
	myArray[10] = (millisecond);		  //Least significant byte

	Wire.write(myArray, 11);

	encoder0Pos = 0;
	millisecond = 0;
}

void increaseCounter()
{
	millisecond++;
}
