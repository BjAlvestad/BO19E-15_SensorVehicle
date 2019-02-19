
//30.01.19 FUNGERER FOR abs(Ticks)<255

/*
   Sketch by max wolf / www.meso.net
   v. 0.1 - very basic functions - mw 20061220
*/
#include <TimerOne.h>
#include <Wire.h>
const double TICKS_PER_CM = 10.6;
const int ADDRESS = 0x30;
const int SIZE_OF_BYTE_ARRAY = 23;
int val;
int encoder0PinA = 3;
int encoder0PinB = 4;
int encoder0Pos = 0;
int encoder0PinALast = LOW;
int n = LOW;
long cmTravelled = 0;
long millisecond = 0;
long longsToBeSent[] = { cmTravelled, millisecond };
int arrayLength = sizeof(longsToBeSent) / sizeof(long);

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
void increaseCounter()
{
	millisecond++;	
}

void onRequestEvent()
{
	cmTravelled = encoder0Pos / TICKS_PER_CM;
	long longsToBeSent[] = { cmTravelled, millisecond };
	int arrayLength = sizeof(longsToBeSent) / sizeof(long);
	SendByteArray(0, arrayLength, longsToBeSent);
}
void SendByteArray(int message, int arrayLength, long longsToBeSent[])
{
	byte byteArray[SIZE_OF_BYTE_ARRAY];
	byteArray[0] = (byte)ADDRESS;
	byteArray[1] = (byte)message;
	byteArray[2] = (byte)(arrayLength);
	int elementsBeforeLong = 3;

	const int bitSizeOfLong = sizeof(long) * 8;

	for (int i = 0; i < sizeof(longsToBeSent); i++)
	{

		int shiftByLong = sizeof(long) * i;
		for (int j = 1; j <= sizeof(long); j++)
		{
			byteArray[(elementsBeforeLong - 1) + shiftByLong + j] = (byte)(longsToBeSent[i] >> (bitSizeOfLong - 8 * j));
		}
	}
	Wire.write(byteArray, SIZE_OF_BYTE_ARRAY);
	encoder0Pos = 0;
	millisecond = 0;
}