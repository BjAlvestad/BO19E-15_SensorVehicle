#include <Wire.h>
#include <Servo.h>
#include "../../../../../../../../Program Files (x86)/Arduino/hardware/arduino/avr/libraries/Wire/src/Wire.h"

/// Const
const int pin_pwm_right = 6;
const int pin_pwm_left = 5;
const int pin_mode_1 = 11;
const int pin_mode_2 = 10;
const int pin_relay_sabertooth = 9;

const int address = 0x20;
const int size_of_byte_array = 23;

const int data_standstill = 0;
const int data_full_reverse = -100;
const int data_full_forward = 100;

const int pwm_start_reverse = 1330;
const int pwm_full_reverse = 1130; // 12 v (1000)
const int pwm_start_forward = 1600;
const int pwm_full_forward = 1840;  // 12 v (2000)
const int pwm_standstill = 1500;
const int ramp_up = 20;

Servo wheels_right;
Servo wheels_left;

int data_left, data_right;
int speed_left, speed_right;

//bool standstill = false;
//bool safe = false;
//bool new_value = false;

/// mode
// 3 = full speed
// 0 = stop
int mode = 2;

void receive_event(int x);
int data_check(int data);
int mode_check(int data);

int speed_map(int data);

void setup()
{
	Serial.begin(9600);
	Wire.begin(address);

	pinMode(pin_mode_1, INPUT_PULLUP);
	pinMode(pin_mode_2, INPUT_PULLUP);
	pinMode(pin_relay_sabertooth, OUTPUT);

	digitalWrite(pin_relay_sabertooth, LOW);


	wheels_right.attach(pin_pwm_right);
	wheels_left.attach(pin_pwm_left);
	wheels_left.writeMicroseconds(pwm_standstill);
	wheels_right.writeMicroseconds(pwm_standstill);
	Wire.onReceive(receive_event);


	data_left = data_standstill;
	data_right = data_standstill;
	speed_left = pwm_standstill;
	speed_right = pwm_standstill;
}

void loop()
{
	if (data_left==0 && data_right == 0)
	{
		digitalWrite(pin_relay_sabertooth, LOW);
	}
	else
	{
		digitalWrite(pin_relay_sabertooth, HIGH);
	}

	speed_left = speed_map(mode_check(data_left));
	speed_right = speed_map(mode_check(data_right));
	
	wheels_right.writeMicroseconds(speed_right);
	wheels_left.writeMicroseconds(speed_left);


	//Serial.print("Left: ");
	//Serial.print(wheels_left.readMicroseconds());
	//Serial.print("  Right: ");
	//Serial.println(wheels_right.readMicroseconds());
	delay(1000);
}

void receive_event(int x)
{
	Serial.println("Inne i receive-event");
	byte data[size_of_byte_array];

	if (Wire.available())
	{
		for (int i = 0; i < size_of_byte_array; i++)
		{
			data[i] = Wire.read();
		}
		Serial.print("Element 5: ");
		Serial.println(data[5]);
		Serial.print("Element 6: ");
		Serial.println(data[6]);
		AssembleDataFromVehicle(data);
	}
}

int speed_map(int data)
{
	if (data > 0)
	{

		data = map(data, data_standstill, data_full_forward, pwm_start_forward, pwm_full_forward);
		return data;
	}
	else if (data < 0)
	{

		data = map(data, data_standstill, data_full_reverse, pwm_start_reverse, pwm_full_reverse);
		return data;
	}
	else
	{
		return pwm_standstill;
	}
}

int data_check(const int data)
{
	if (data < data_full_reverse)
	{
		return  data_full_reverse;
	}
	else if (data > data_full_forward)
	{
		return data_full_forward;
	}
	else
	{
		return data;
	}
}

int mode_check(int data)
{
	/*Serial.print("mode_check - -  data:  ");
	Serial.print(data);
	Serial.print("    pin 1:  ");
	Serial.print(digitalRead(pin_mode_1));
	Serial.print("    pin 2:  ");
	Serial.println(digitalRead(pin_mode_2));*/

	if (!digitalRead(pin_mode_1) && !digitalRead(pin_mode_2) )
	{
		mode = 0;
		return 0;
	}
	else if (!digitalRead(pin_mode_1) && digitalRead(pin_mode_2))
	{
		mode = 1;
		data = data / 3;
		return data;
	}
	else if (digitalRead(pin_mode_1) && !digitalRead(pin_mode_2))
	{
		mode = 2;
		data = data / 2;
		return data;
	}
	else if (digitalRead(pin_mode_1) && digitalRead(pin_mode_2))
	{
		mode = 3;
		return data;
	}
	return 0;
}

void AssembleDataFromVehicle(byte vehicleByteArray[])
{
	int address = vehicleByteArray[0];
	int code = vehicleByteArray[1];
	int numberOfLongs = vehicleByteArray[2];
	int emptyBytes = 3;
	Serial.print("NumOfLongs: ");
	Serial.println(numberOfLongs);
	AssembleIntsFromByteArray(numberOfLongs, emptyBytes, vehicleByteArray);
}

void AssembleIntsFromByteArray(long numberOfLongs, int startIndex, byte array[])
{
	Serial.println("Ouside if");
	if (size_of_byte_array >= (sizeof(long)*numberOfLongs + startIndex))
	{
		Serial.println("inside if");
		const int bitsInLong = sizeof(long) * 8;
		long longs[2];

		for (int i = 0; i < numberOfLongs; i++)
		{
			int assembledLong = 0;
			int shiftByLong = sizeof(long) * i;

			for (int j = 1; j <= sizeof(long); j++)
			{
				assembledLong |= array[startIndex - 1 + j + shiftByLong] << (bitsInLong - 8 * j);
			}
			longs[i] = assembledLong;
		}
		Serial.print("Long[0]: ");
		Serial.println(longs[0]);
		Serial.print("Long[1]: ");
		Serial.println(longs[1]);
		data_left = data_check(longs[0]);  // Between -100 and 100;
		data_right = data_check(longs[1]);
		Serial.println("Etter data_check: ");
		Serial.println(data_left);
		Serial.println(data_right);
		Serial.println("---------");


		//data_left = mode_check(data_left);  // Divide value with mode
		//data_right = mode_check(data_right);
		//Serial.println("Etter mode_check: ");
		//Serial.println(data_left);
		//Serial.println(data_right);
		//Serial.println("---------");


		/*new_speed_left = speed_map(data_left);
		new_speed_right = speed_map(data_right);
		Serial.println("new_speed_left: ");
		Serial.println(new_speed_left);
		Serial.println("---------");
		Serial.println("new_speed_right: ");
		Serial.println(new_speed_right);
		Serial.println("---------");*/
		
	}
}