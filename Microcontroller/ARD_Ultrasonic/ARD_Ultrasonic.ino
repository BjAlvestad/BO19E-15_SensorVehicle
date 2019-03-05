#include <Wire.h>

//byte response[6]; // this data is sent to PI

// defines pins numbers
const int pin_trig_left = 8;
const int pin_echo_left = 9;

const int pin_trig_right = 6;
const int pin_echo_right = 7;

const int pin_trig_forward_left = 2;
const int pin_echo_forward_left = 3;

const int pin_trig_forward_right = 4;
const int pin_echo_forward_right = 5;

const int pin_new_message = 10;
const int pin_mode_1 = 12;
const int pin_mode_2 = 11;

const int size_of_byte_array = 23;
const byte address = 0x25;
const int pause = 40;

// defines variables
long distance_left;
long distance_right;
long distance_forward_left;
long distance_forward_right;

//long distance_shortest;
int count;

////byte bDistanceLeft;
//byte bDistanceRight[2];
//byte bDistanceForward[2];
//
////String Sdl;
//String Sdr;
//String Sdf;
//String s;

//char cL[3];
//char cF[3];
//char cR[3];

//char a[20];
//int x;


void i2c_request();
long ultrasonic(int trig_pin, int echo_pin);
void check_distance(long distance);
void set_mode(int i);
void send_byte_array(int message, int array_length, long longs_to_be_sent[]);



void setup() {

	pinMode(pin_trig_left, OUTPUT);
	pinMode(pin_echo_left, INPUT);

	pinMode(pin_trig_right, OUTPUT);
	pinMode(pin_echo_right, INPUT);

	pinMode(pin_trig_forward_left, OUTPUT);
	pinMode(pin_echo_forward_left, INPUT);

	pinMode(pin_trig_forward_right, OUTPUT);
	pinMode(pin_echo_forward_right, INPUT);

	pinMode(pin_mode_1, OUTPUT);
	pinMode(pin_mode_2, OUTPUT);

	digitalWrite(pin_mode_1, HIGH);
	digitalWrite(pin_mode_2, LOW);

	pinMode(pin_new_message, OUTPUT);


	Serial.begin(9600);

	Wire.begin(address);

	Wire.onRequest(i2c_request);
}



void loop() {
	count = 0;

	distance_forward_right = ultrasonic(pin_trig_forward_right, pin_echo_forward_right);
	check_distance(distance_forward_right);
	delay(pause);

	distance_left = ultrasonic(pin_trig_left, pin_echo_left);
	check_distance(distance_left);
	delay(pause);

	distance_forward_left = ultrasonic(pin_trig_forward_left, pin_echo_forward_left);
	check_distance(distance_forward_left);
	delay(pause);

	distance_right = ultrasonic(pin_trig_right, pin_echo_right);
	check_distance(distance_right);
	delay(pause);

	digitalWrite(pin_new_message, HIGH);

	if (count > 3)
	{
		// mode 3
		set_mode(3);
	}
}

void i2c_request() {

	Serial.println("I2C-Request");

	long longs_to_be_sent[] = { distance_left, distance_forward_left, distance_right, distance_forward_right };
	const int array_length = sizeof(longs_to_be_sent) / sizeof(long);
	send_byte_array(0, array_length, longs_to_be_sent);

	digitalWrite(pin_new_message, LOW);
}

long ultrasonic(const int trig_pin, const int echo_pin)
{
	digitalWrite(trig_pin, LOW);
	delayMicroseconds(2);
	// Sets the trigPin on HIGH state for 10 micro seconds
	digitalWrite(trig_pin, HIGH);
	delayMicroseconds(10);
	digitalWrite(trig_pin, LOW);
	// Reads the echoPin, returns the sound wave travel time in microseconds
	const long duration = pulseIn(echo_pin, HIGH);
	// Calculating the distance
	const int distance = duration * 0.034 / 2;


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

void check_distance(const long distance)
{
	if (distance < 20)
	{
		// mode 2
		set_mode(2);
	}
	else if (distance < 15)
	{
		// mode 1
		set_mode(1);
	}
	else if (distance < 10)
	{
		// mode 0
		set_mode(0);
	}
	else
	{
		count++;
	}

}

void set_mode(const int i)
{
	switch (i)
	{
	case 0:
		digitalWrite(pin_mode_1, LOW);
		digitalWrite(pin_mode_2, LOW);
		break;
	case 1:
		digitalWrite(pin_mode_1, LOW);
		digitalWrite(pin_mode_2, HIGH);
		break;
	case 2:
		digitalWrite(pin_mode_1, HIGH);
		digitalWrite(pin_mode_2, LOW);
		break;
	case 3:
		digitalWrite(pin_mode_1, HIGH);
		digitalWrite(pin_mode_2, HIGH);
		break;
	default:
		digitalWrite(pin_mode_1, LOW);
		digitalWrite(pin_mode_2, HIGH);
		break;
	}
}

void send_byte_array(const int message, const int array_length, long longs_to_be_sent[])
{
	byte byte_array[size_of_byte_array];
	byte_array[0] = byte(address);
	byte_array[1] = byte(message);
	byte_array[2] = byte(array_length);
	const int elements_before_long = 3;

	const int bit_size_of_long = sizeof(long) * 8;

	for (int i = 0; i < array_length; i++)
	{
		const int shift_by_long = sizeof(long) * i;
		for (int j = 1; j <= sizeof(long); j++)
		{
			byte_array[(elements_before_long - 1) + shift_by_long + j] = byte(longs_to_be_sent[i] >> (bit_size_of_long - 8 * j));
		}
	}

	Wire.write(byte_array, size_of_byte_array);
}

