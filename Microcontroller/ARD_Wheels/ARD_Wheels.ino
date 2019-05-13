// Code for SensorVehicle v.1.0
// BO19E-15


// Communication
#include <Wire.h>
const int pin_I2C_enable = 14;
const int address = 0x20;
const int size_of_byte_array = 23;
void receive_event(int x);
void assemble_ints_from_byte_array(long number_of_longs, int start_index, byte array[]);
void assemble_data_from_vehicle(byte vehicle_byte_array[]);

// PWM
#include <Servo.h>
Servo wheels_right;
Servo wheels_left;

const int pin_pwm_right = 7;
const int pin_pwm_left = 6;
const int pin_mode_1 = 11;
const int pin_mode_2 = 10;
const int pin_relay_sabertooth = 9;
const int pin_mode_switch = 12;

const int data_standstill = 0;
const int data_full_reverse = -100;
const int data_full_forward = 100;
const int pwm_start_reverse = 1330;
const int pwm_full_reverse = 1130; // 12 v (1000)
const int pwm_start_forward = 1600;
const int pwm_full_forward = 1840;  // 12 v (2000)
const int pwm_standstill = 1500;
const int ramp_up = 20;

int data_left, data_right;
int speed_left, speed_right;
int speed_map(int data);

// Safety mode
int mode = 2;  // 3 = full speed, 0 = stop
int data_check(int data);
int mode_check(int data);

void setup()
{
	Wire.begin(address);

	pinMode(pin_mode_1, INPUT_PULLUP);
	pinMode(pin_mode_2, INPUT_PULLUP);
	pinMode(pin_relay_sabertooth, OUTPUT);
	pinMode(pin_mode_switch, INPUT_PULLUP);
	pinMode(LED_BUILTIN, OUTPUT);

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

	pinMode(pin_I2C_enable, OUTPUT);
	digitalWrite(pin_I2C_enable, HIGH);
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

	/*String s = "Ld: = " + String(data_left) + "\t Rd: " + String(data_right) +
		"\t L: " + String(speed_left) + "\t R: " + String(speed_right) +
		"\t MODE: " + mode +
		"\t SWITCH: " + digitalRead(pin_mode_switch);
	Serial.println(s);
	delay(1000);*/
}

void receive_event(int x)
{
	byte data[size_of_byte_array];
	
	if (Wire.available())
	{
		for (int i = 0; i < size_of_byte_array; i++)
		{
			data[i] = Wire.read();
		}
		assemble_data_from_vehicle(data);
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
	if (digitalRead(pin_mode_switch))
	{
		digitalWrite(LED_BUILTIN, HIGH);
		mode = 2;
		return (2 * data) / 3;
	}
	digitalWrite(LED_BUILTIN, LOW);

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

void assemble_data_from_vehicle(byte vehicle_byte_array[])
{
	int address = vehicle_byte_array[0];
	int code = vehicle_byte_array[1];
	const int number_of_longs = vehicle_byte_array[2];
	const int empty_bytes = 3;
	assemble_ints_from_byte_array(number_of_longs, empty_bytes, vehicle_byte_array);
}

void assemble_ints_from_byte_array(const long number_of_longs, const int start_index, byte array[])
{
	if (size_of_byte_array >= (sizeof(long)*number_of_longs + start_index))
	{
		const int bits_in_long = sizeof(long) * 8;
		long longs[2];

		for (int i = 0; i < number_of_longs; i++)
		{
			int assembled_long = 0;
			const int shift_by_long = sizeof(long) * i;

			for (int j = 1; j <= sizeof(long); j++)
			{
				assembled_long |= array[start_index - 1 + j + shift_by_long] << (bits_in_long - 8 * j);
			}
			longs[i] = assembled_long;
		}

		data_left = data_check(longs[0]);  // Between -100 and 100;
		data_right = data_check(longs[1]);
	}
}