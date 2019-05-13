// Code for SensorVehicle v.1.0
// BO19E-15

// https://create.arduino.cc/projecthub/MisterBotBreak/how-to-use-an-ultrasonic-sensor-181cee

// Communication
#include <Wire.h>
const int i2c_enable_pin = 15;  //A1
const int pin_new_message = 10;
const int size_of_byte_array = 23;
const byte address = 0x25;
void i2c_request();
void send_byte_array(int message, int array_length, long longs_to_be_sent[]);

// Ultrasonic
const int pin_trig_left = 5;
const int pin_echo_left = 4;
const int pin_trig_right = 7; 
const int pin_echo_right = 6; 
const int pin_trig_forward_left = 3;
const int pin_echo_forward_left = 2;
const int pin_trig_forward_right =9; 
const int pin_echo_forward_right =8; 
long ultrasonic(int trig_pin, int echo_pin);

// Defines variables
long distance_left;
long distance_right;
long distance_forward_left;
long distance_forward_right;
const int pause = 30;

// Safety mode
const int pin_mode_1 = 12;
const int pin_mode_2 = 11;
const int pin_mode_switch = 14;
const int safe_distance_reduce = 5;
void check_distance(long distance);
void set_mode(int i);
int count;
int mode;


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
	digitalWrite(pin_new_message, LOW);

	Wire.begin(address);
	Wire.onRequest(i2c_request);

	mode = 1;

	pinMode(pin_mode_switch, INPUT_PULLUP);
	pinMode(LED_BUILTIN, OUTPUT);

	pinMode(i2c_enable_pin, OUTPUT);
	digitalWrite(i2c_enable_pin, HIGH);
}

void loop() {
	count = 0;

	delay(4); // hack

	distance_forward_right = ultrasonic(pin_trig_forward_right, pin_echo_forward_right);
	check_distance(distance_forward_right);
	delay(pause);

	distance_left = ultrasonic(pin_trig_left, pin_echo_left);
	check_distance(distance_left + 5);
	delay(pause);

	distance_forward_left = ultrasonic(pin_trig_forward_left, pin_echo_forward_left);
	check_distance(distance_forward_left);
	delay(pause);

	distance_right = ultrasonic(pin_trig_right, pin_echo_right);
	check_distance(distance_right + 5);
	delay(pause);

	digitalWrite(pin_new_message, HIGH);


	/*const String text = "L: " + String(distance_left) + "\t FL: " +String(distance_forward_left) + "\t FR: " +
		String(distance_forward_right) + "\t R: " + String(distance_right) + "\t Mode: " + String(mode) + "\t Count: " + String(count);
	Serial.println(text);*/

	if (count > 3)
	{
		if (mode < 3)
		{
			mode++;
		}
		set_mode(mode);
	}
}

void i2c_request() {
	long longs_to_be_sent[] = { distance_left, distance_forward_right, distance_right, distance_forward_left  };
	const int array_length = sizeof(longs_to_be_sent) / sizeof(long);
	send_byte_array(0, array_length, longs_to_be_sent);

	digitalWrite(pin_new_message, LOW);
}

long ultrasonic(const int trig_pin, const int echo_pin)
{
	digitalWrite(trig_pin, LOW);
	delayMicroseconds(2);
	digitalWrite(trig_pin, HIGH);		// Sets the trigPin on HIGH state for 10 micro seconds
	delayMicroseconds(10);
	digitalWrite(trig_pin, LOW);
	const long duration = pulseIn(echo_pin, HIGH, 24000);  // Reads the echoPin, returns the sound wave travel time in microseconds

	const int distance = duration * 0.034 / 2;

	if (distance > 400)
	{
		return 400;
	}
	else if (distance == 0)
	{
		return 401;
	}
	else
	{
		return distance;
	}
}

void check_distance(const long distance)
{
	auto reduced_safety = 0;

	if (digitalRead(pin_mode_switch) == HIGH)
	{
		reduced_safety = safe_distance_reduce;
		digitalWrite(LED_BUILTIN, HIGH);
	}
	else
	{
		digitalWrite(LED_BUILTIN, LOW);
	}

	if (distance < 15 - reduced_safety)
	{
		mode = 0;
		set_mode(mode);
	}
	else if (distance < 20 - reduced_safety)
	{
		if (mode < 1)
		{
			count++;
		}
		else
		{
			mode = 1;
			set_mode(mode);
		}
	}
	else if (distance < 25 - reduced_safety)
	{
		if (mode < 2)
		{
			count++;
		}
		else
		{
			mode = 2;
			set_mode(mode);
		}
	}
	else
	{
		if (mode < 3)
		{
			count++;
		}
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

