using System;
using System.Diagnostics;
using Windows.Foundation.Collections;

namespace SimulatorUwpXaml.SensorVehicleApp_interface
{
    public class SimulatedEncoderSensor
    {
        private readonly VehicleSprite _vehicle;
        private readonly bool _leftWheel;
        private DateTime _timeAtLastRequestLeftWheel;
        private DateTime _timeAtLastRequestRightWheel;

        public SimulatedEncoderSensor(VehicleSprite vehicle, bool leftWeelNotRight)
        {
            _vehicle = vehicle;
            _leftWheel = leftWeelNotRight;
            _timeAtLastRequestLeftWheel = DateTime.Now;
            _timeAtLastRequestRightWheel = DateTime.Now;
        }

        private int MillisecondsAccumelatedForLastRequest
        {
            get
            {
                TimeSpan timeSinceLastRequest;
                if (_leftWheel)
                {
                    timeSinceLastRequest = DateTime.Now - _timeAtLastRequestLeftWheel;
                    _timeAtLastRequestLeftWheel = DateTime.Now;
                }
                else
                {
                    timeSinceLastRequest = DateTime.Now - _timeAtLastRequestRightWheel;
                    _timeAtLastRequestRightWheel = DateTime.Now;
                }
                return (int)timeSinceLastRequest.TotalMilliseconds;
            }
        }
        private int CmSinceLastRequest
        {
            get
            {
                int distance;
                try
                {
                    if (_leftWheel)
                    {
                        distance = (int)Math.Truncate(_vehicle.CmTraveledLeftWheel);
                        _vehicle.CmTraveledLeftWheel = _vehicle.CmTraveledLeftWheel - distance;
                    }
                    else
                    {
                        distance = (int)Math.Truncate(_vehicle.CmTraveledRightWheel);
                        _vehicle.CmTraveledRightWheel = _vehicle.CmTraveledRightWheel - distance;
                    }
                }
                catch (OverflowException)
                {
                    distance = 0;
                    _vehicle.CmTraveledLeftWheel = 0;
                    _vehicle.CmTraveledRightWheel = 0;
                    Debug.WriteLine("Overflow occured for simulated encoder when CmSinceLastRequest property getter was called.");
                }

                return distance;
            }
        }

        public ValueSet ReturnData()
        {
            ValueSet data = new ValueSet();

            data.Add("ADDRESS", (int)(_leftWheel ? Device.EncoderLeft : Device.EncoderRight));
            data.Add("MESSAGE", 0);
            data.Add("NUMBER_OF_INTS", 2);

            data.Add("DATA", DataFromWheel());

            return data;
        }

        private int[] DataFromWheel()
        {
            int[] data = new int[2];

            data[0] = CmSinceLastRequest;
            data[1] = MillisecondsAccumelatedForLastRequest;

            return data;
        }
    }
}
