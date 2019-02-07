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

const byte ADDRESS = 0x25;

// defines variables
String distanceLeft;
String distanceRight;
String distanceForward;

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
String Ultrasonic(int trigPin, int echoPin);
// the setup function runs once when you press reset or power the board
void setup() {
	pinMode(trigPinLeft, OUTPUT); // Sets the trigPin as an Output
	pinMode(echoPinLeft, INPUT); // Sets the echoPin as an Input
	pinMode(trigPinRight, OUTPUT);
	pinMode(echoPinRight, INPUT);
	pinMode(trigPinForward, OUTPUT);
	pinMode(echoPinForward, INPUT);


	Serial.begin(9600); // Starts the serial communication

	Wire.begin(ADDRESS); // Set the address of this slave on I2C

	Wire.onRequest(I2CRequest);
}

// the loop function runs over and over again until power down or reset
void loop() {

	distanceLeft = Ultrasonic(trigPinLeft, echoPinLeft);
	//Sdl = static_cast<String>(distanceLeft);

	/*distanceLeft.toCharArray(cL, 4);
	for (char c_l : cL)
	{
		Serial.print(c_l);
	}
	Serial.println();*/
	delay(60);

	distanceForward = Ultrasonic(trigPinForward, echoPinForward);
	//Sdf = static_cast<String>(distanceForward);
	/*distanceForward.toCharArray(cF, 4);
	for (char c_l : cF)
	{
		Serial.print(c_l);
	}
	Serial.println();*/
	delay(60);

	distanceRight = Ultrasonic(trigPinRight, echoPinRight);
	//Sdr = static_cast<String>(distanceRight);
	/*distanceRight.toCharArray(cR, 4);
	for (char c_l : cR)
	{
		Serial.print(c_l);
	}
	Serial.println();*/
	delay(60);

	


	s = distanceLeft +"-" + distanceForward + "-" + distanceRight+ "-";
	Serial.println(s);
	
	x = s.length()+1;

	for (char a1 : a)
	{
		a1 = ' ';
	}

	s.toCharArray(a, x);
	for (char c : a)
	{
		Serial.print(c);
	}
	Serial.println();

	delay(10);
}

// function that executes whenever data is requested by master
// this function is registered as an event, see setup()
void I2CRequest() {
	Serial.println("I2C-Request");
	Wire.write(a); // return data to PI
}

String Ultrasonic(int trigPin, int echoPin )
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
		return "n+n";
	}
	else if (distance < 2)
	{
		return "n-n";
	}
	else
	{
		return static_cast<String>(distance);
	}

	
}

