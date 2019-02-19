#include <Wire.h>
#include "../../../../../../../../Program Files (x86)/Arduino/hardware/arduino/avr/libraries/Wire/src/Wire.h"

byte response[6]; // this data is sent to PI

// defines pins numbers
const int trigPinLeft = 8;
const int echoPinLeft = 9;
const int trigPinRight = 6;
const int echoPinRight = 7;
const int trigPinForward = 4;
const int echoPinForward = 5;

const int size_of_byte_array = 23;
const byte address = 0x25;

// defines variables
long distanceLeft;
long distanceRight;
long distanceForward;

byte bDistanceLeft;
byte bDistanceRight[2];
byte bDistanceForward[2];

String Sdl;
String Sdr;
String Sdf;
String s;

char cL[3];
char cF[3];
char cR[3];

char a[20];
int x;
void I2CRequest();
long Ultrasonic(int trigPin, int echoPin);
// the setup function runs once when you press reset or power the board
void setup() {
	pinMode(trigPinLeft, OUTPUT); // Sets the trigPin as an Output
	pinMode(echoPinLeft, INPUT); // Sets the echoPin as an Input
	pinMode(trigPinRight, OUTPUT);
	pinMode(echoPinRight, INPUT);
	pinMode(trigPinForward, OUTPUT);
	pinMode(echoPinForward, INPUT);


	Serial.begin(9600); // Starts the serial communication

	Wire.begin(address); // Set the address of this slave on I2C

	Wire.onRequest(I2CRequest);
}

// the loop function runs over and over again until power down or reset
void loop() {

	distanceLeft = Ultrasonic(trigPinLeft, echoPinLeft);
	delay(60);

	distanceForward = Ultrasonic(trigPinForward, echoPinForward);
	delay(60);

	distanceRight = Ultrasonic(trigPinRight, echoPinRight);
	delay(60);
}

// function that executes whenever data is requested by master
// this function is registered as an event, see setup()
void I2CRequest() {
	Serial.println("I2C-Request");
	long longsToBeSent[] = { distanceLeft, distanceForward, distanceRight };
	int arrayLength = sizeof(longsToBeSent) / sizeof(long);
	SendByteArray(0, arrayLength, longsToBeSent);
}

long Ultrasonic(int trigPin, int echoPin )
{
	digitalWrite(trigPin, LOW);
	delayMicroseconds(2);
	// Sets the trigPin on HIGH state for 10 micro seconds
	digitalWrite(trigPin, HIGH);
	delayMicroseconds(10);
	digitalWrite(trigPin, LOW);
	// Reads the echoPin, returns the sound wave travel time in microseconds
	long duration = pulseIn(echoPin, HIGH);
	// Calculating the distance
	int distance = duration * 0.034 / 2;
	// Prints the distance on the Serial Monitor


	if (distance > 400)
	{
		return 400;
	}
	else if (distance < 2)
	{
		return 0;
	}
	else
	{
		return distance;
	}	
}

void SendByteArray(int message, int arrayLength, long longsToBeSent[])
{
	byte byteArray[size_of_byte_array];
	byteArray[0] = (byte)address;
	byteArray[1] = (byte)message;
	byteArray[2] = (byte)(arrayLength);
	int elementsBeforeLong = 3;

	const int bitSizeOfLong = sizeof(long) * 8;

	for (int i = 0; i < arrayLength; i++)
	{

		int shiftByLong = sizeof(long) * i;
		for (int j = 1; j <= sizeof(long); j++)
		{
			byteArray[(elementsBeforeLong - 1) + shiftByLong + j] = (byte)(longsToBeSent[i] >> (bitSizeOfLong - 8 * j));
		}
	}

	Wire.write(byteArray, size_of_byte_array);
}

