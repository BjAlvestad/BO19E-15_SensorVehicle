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

const int data_full_reverse = 28;
const int data_full_forward = 228;
const int pwm_start_reverse = 1330;
const int pwm_full_reverse = 1000;
const int pwm_start_forward = 1600;
const int pwm_full_forward = 2000;
const int pwm_standstill = 1500;
const int ramp_up = 20;

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

int speed_map(int data);

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
	wheels_left.writeMicroseconds(pwm_standstill);
	wheels_right.writeMicroseconds(pwm_standstill);
	Wire.onReceive(receive_event);
	Wire.onRequest(request_event);

	Serial.print("MODE: ");
	mode = 1;
	Serial.println(mode);
	Serial.println(digitalRead(safety_pin));
	
	speed_left = pwm_standstill;
	speed_right = pwm_standstill;
	new_speed_left = pwm_standstill;
	new_speed_right = pwm_standstill;
}

void loop()
{
	wheels_right.writeMicroseconds(new_speed_right);
	wheels_left.writeMicroseconds(new_speed_left);


	//wheels_right.writeMicroseconds(speed_right);
	//wheels_left.writeMicroseconds(speed_left);
	

	speed_left = set_speed(new_speed_left, speed_left);
	speed_right = set_speed(new_speed_right, speed_right);



	Serial.print("Left: ");
	Serial.print(wheels_left.readMicroseconds()); 
	Serial.print("  Right: ");
	Serial.println(wheels_right.readMicroseconds());
	Serial.println(new_speed_left);
	Serial.println(speed_left);
	

	//delay(1000);
	

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

		data_left = mode_check(data_left);  // Divide value with mode
		data_right = mode_check(data_right);
		Serial.println(data_left);

		new_speed_left = speed_map(data_left);
		new_speed_right = speed_map(data_right);
		
	}
}

int speed_map(int data)
{
	if (data > 128)
	{
		Serial.println(data);
		data = data - 128;
		Serial.println(data);

		data = map(data, 0, 100, pwm_start_forward, pwm_full_forward);
		Serial.println(data);

		return data;
	}
	else if (data < 128)
	{

		data = data - 28;
		data = map(data, 100, 0, pwm_start_reverse, pwm_full_reverse);
		return data;
	}
	else
	{
		return pwm_standstill;
	}
}

int set_speed(const int new_speed, int speed)
{

	if (new_speed != speed)
	{
		if (new_speed > pwm_standstill)
		{
			// Forward
			if (new_speed > speed)
			{
				// Increase speed
				speed+= ramp_up;
			}
			else
			{
				// Decrease speed
				speed = new_speed;
			}
		}
		else if (new_speed < pwm_standstill)
		{
			// Backward
			if (new_speed < speed)
			{
				// Increase speed
				speed-=ramp_up;
			}
			else
			{
				// Decrease speed
				speed = new_speed;
			}
		}
		else
		{
			speed = pwm_standstill;
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


