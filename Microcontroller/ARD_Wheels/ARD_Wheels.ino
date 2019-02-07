#include <Servo.h>
#include <Wire.h>

/// Const
const int right_pwm_pin = 9;
const int left_pwm_pin = 10;
const int mode_pin_1 = 2;
const int mode_pin_2 = 3;
const int safety_pin = 4;
const int motor_controller_switch = 5;
const int address = 0x20;

const int full_reverse_data = 28;
const int full_forward_data = 228;
const int full_reverse_pwm = 1000;
const int full_forward_pwm = 2000;
const int standstill_pwm = 1500;

Servo wheels_right;
Servo wheels_left;

int speed_left, speed_right;
int new_speed_left, new_speed_right;

bool standstill = false;
bool safe = false;
bool new_value = false;
int mode = 3;

void request_event();
void receive_event(int x);
int set_speed(int new_speed, int speed);
int data_check(int data);
int mode_check(int data);

void setup()
{
	Serial.begin(9600);
	Wire.begin(address);

	pinMode(mode_pin_1, INPUT_PULLUP);
	pinMode(mode_pin_2, INPUT_PULLUP);
	pinMode(safety_pin, INPUT);
	pinMode(motor_controller_switch, OUTPUT);

	wheels_right.attach(right_pwm_pin);
	wheels_left.attach(left_pwm_pin);
	wheels_left.writeMicroseconds(standstill_pwm);
	wheels_right.writeMicroseconds(standstill_pwm);
	Wire.onReceive(receive_event);
	Wire.onRequest(request_event);

	const bool two = digitalRead(mode_pin_2);
	const bool on = digitalRead(mode_pin_1);
	if (!on && !two)
	{
		mode = 1;
	}
	else if (on && !two)
	{
		mode = 2;
	}
	else if(!on && two)
	{
		mode = 3;
	}
	Serial.print("MODE: ");
	mode = 2;
	Serial.println(mode);
	Serial.println(digitalRead(safety_pin));

	/*for (int i = 1500; i< 2000; i++)
	{
		wheels_left.writeMicroseconds(i);
	}
	for (int i = 2000; i > 1500; i--)
	{
		wheels_left.writeMicroseconds(i);
	}*/


}

void loop()
{
	if (wheels_left.readMicroseconds() == standstill_pwm && wheels_right.readMicroseconds() == standstill_pwm)
	{
		standstill = true;
		digitalWrite(motor_controller_switch, LOW);
	}

	if (standstill)
	{
		digitalWrite(motor_controller_switch, LOW);
	}

	safe = digitalRead(safety_pin);
	if (!safe)
	{
		digitalWrite(motor_controller_switch, LOW);
		wheels_right.writeMicroseconds(standstill_pwm);
		wheels_left.writeMicroseconds(standstill_pwm);
		standstill = true;
	}
	if (standstill && safe && new_value)
	{
		digitalWrite(motor_controller_switch, HIGH);
		speed_left = set_speed(new_speed_left, speed_left);
		speed_right = set_speed(new_speed_right, speed_right);
		wheels_right.writeMicroseconds(speed_right);
		wheels_left.writeMicroseconds(speed_left);
		new_value = false;
		standstill = false;
	}
	if (!standstill && safe && new_value)
	{
		speed_left = set_speed(new_speed_left, speed_left);
		speed_right = set_speed(new_speed_right, speed_right);
		wheels_right.writeMicroseconds(speed_right);
		wheels_left.writeMicroseconds(speed_left);
		new_value = false;
		standstill = false;
	}
	/*wheels_right.writeMicroseconds(speed_right);
	wheels_left.writeMicroseconds(speed_left);*/
	speed_left = set_speed(new_speed_left, speed_left);
	speed_right = set_speed(new_speed_right, speed_right);
	wheels_right.writeMicroseconds(speed_right);
	wheels_left.writeMicroseconds(speed_left);
	Serial.print("   Left: ");
	Serial.println(speed_left); 
	Serial.print("Right: ");
	Serial.print(speed_right);
	

}

void request_event()
{
	if (safe)
	{
		Wire.write(0x50);
	}
	else
	{
		Wire.write(0x55);
	}
}

void receive_event(int x)
{
	int data[3];

	if (Wire.available())
	{
		data[0] = Wire.read();
		data[1] = Wire.read();
		data[2] = Wire.read();

		Wire.flush();  // slette rester i buffer?
	
		Serial.print("Data from I2C: ");
		Serial.print(data[1]);
		Serial.print(" --- ");
		Serial.println(data[2]);

		int data_left = data_check(data[1]);  // Between 28 and 228;
		int data_right = data_check(data[2]);

		Serial.println(data_left);

		data_left = mode_check(data_left);  // Divide value with mode
		data_right = mode_check(data_right);

		Serial.println(data_left);

		new_speed_left = map(data_left, full_reverse_data, full_forward_data, full_reverse_pwm, full_forward_pwm);
		new_speed_right = map(data_right, full_reverse_data, full_forward_data, full_reverse_pwm, full_forward_pwm);

		Serial.println(new_speed_left);

		new_value = true;
	}
}

int set_speed(const int new_speed, int speed)
{

	if (new_speed != speed)
	{
		if (new_speed > standstill_pwm + 20)
		{
			// Forward
			if (new_speed > speed)
			{
				// Increase speed
				speed++;
			}
			else
			{
				// Decrease speed
				speed = new_speed;
			}
		}
		else if (new_speed < standstill_pwm - 20)
		{
			// Backward
			if (new_speed < speed)
			{
				// Increase speed
				speed--;
			}
			else
			{
				// Decrease speed
				speed = new_speed;
			}
		}
		else
		{
			speed = standstill_pwm;
			standstill = true;
		}
	}

	return speed;
}

int data_check(const int data)
{
	if (data < 28)
	{
		return  28;
	}
	else if (data > 228)
	{
		return 228;
	}
	else
	{
		return data;
	}
}

int mode_check(int data)
{
	data = data - 128;
	data = data / mode;
	data = data + 128;
	return data;
}


