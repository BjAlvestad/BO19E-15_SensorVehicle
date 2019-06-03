// Code for SensorVehicle v.1.0
// BO19E-15

// https://playground.arduino.cc/Main/RotaryEncoders/


// Communication
#include <Wire.h>
#define I2C_ENABLE_PIN  14  //A0
const int size_of_byte_array = 23;
const byte address = 0x30;   // 0x31 for right side
void i2c_request();
void send_byte_array(int message, int array_length, long longs_to_be_sent[]);


// Encoder
#define ENCODER_PIN_A  3
#define ENCODER_PIN_B  2
volatile int encoder0Pos = 0;
volatile boolean last_a = false;
volatile boolean last_b = false;

void do_encoder_a();
void do_encoder_b();
const double ticks_per_cm = 18.38;

// 6250 ticks, 10 turns
// ca 34 cm circumference 
// 625 / 34 = 18,38

int sign = 1; // -1 right side

// Timer
#include <TimerOne.h>
long millisecond = 0;
void increase_counter();

void setup()
{
	pinMode(ENCODER_PIN_A, INPUT_PULLUP);
	pinMode(ENCODER_PIN_B, INPUT_PULLUP);

	attachInterrupt(0, do_encoder_a, CHANGE);
	attachInterrupt(1, do_encoder_b, CHANGE);

	Timer1.initialize(1000);
	Timer1.attachInterrupt(increase_counter);

	pinMode(LED_BUILTIN, OUTPUT);

	Wire.begin(address);
	Wire.onRequest(i2c_request);
	pinMode(I2C_ENABLE_PIN, OUTPUT);
	digitalWrite(I2C_ENABLE_PIN, HIGH);
}

void loop()
{
}

void i2c_request() {
	digitalWrite(LED_BUILTIN, HIGH);
	/*Serial.print(" pos:  ");

	Serial.print(encoder0Pos);
	Serial.print(" tick_per_cm:  ");

	Serial.print(ticks_per_cm);
	Serial.print("   ");

	Serial.println(((-1 * encoder0Pos) / ticks_per_cm));*/

	long longs_to_be_sent[] = { (sign * encoder0Pos) / ticks_per_cm, millisecond };
	encoder0Pos = 0;
	millisecond = 0;
	const int array_length = sizeof(longs_to_be_sent) / sizeof(long);
	send_byte_array(0, array_length, longs_to_be_sent);

	digitalWrite(LED_BUILTIN, LOW);
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
void increase_counter()
{
	millisecond++;
}

void do_encoder_a()
{
	if (digitalRead(ENCODER_PIN_A) != last_a) {
		last_a = !last_a;

		if (last_a && !last_b) encoder0Pos += 1;
	}
}

void do_encoder_b()
{
	if (digitalRead(ENCODER_PIN_B) != last_b) {
		last_b = !last_b;
		if (last_b && !last_a) encoder0Pos -= 1;
	}
}