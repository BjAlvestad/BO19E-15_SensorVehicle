using System;
using System.Collections.Generic;
using Communication.ExternalCommunication.Handler.Constants;
using ExampleLogic;
using StudentLogic;
using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Encoder;
using VehicleEquipment.Locomotion.Wheels;

namespace Communication.ExternalCommunication.Handler
{
    public class RequestHandler
    {
        private IUltrasonic _ultrasonic;
        private ILidarDistance _lidar;
        private IWheel _wheel;
        private IEncoders _encoders;
        private ExampleLogicService _exampleLogic;
        private StudentLogicService _studentLogicService;

        public RequestHandler(IWheel wheel, IUltrasonic ultrasonic, ILidarDistance lidar, IEncoders encoders, ExampleLogicService exampleLogicService, StudentLogicService studentLogicService)
        {
            _wheel = wheel;
            _ultrasonic = ultrasonic;
            _lidar = lidar;
            _encoders = encoders;
            _exampleLogic = exampleLogicService;
            _studentLogicService = studentLogicService;
        }

        public Dictionary<string, string> HandleRequest(Dictionary<string, string> request)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();

            try
            {
                if(!request.ContainsKey(Key.RequestType)) throw new Exception($"Request did not contain the required key --> {Key.RequestType}");

                switch (request[Key.RequestType])
                {
                    case RequestType.Command:
                        if(!request.ContainsKey(Key.Component)) throw new Exception($"Command request did not contain the required key --> {Key.Component}");
                        response.Add(request[Key.Component], HandleComponentCommand(request));
                        break;;
                    case RequestType.Data:
                        if(!request.ContainsKey(Key.Component)) throw new Exception($"Data request did not contain the required key --> {Key.Component}");
                        List<string> components = new List<string>(request[Key.Component].Split(' '));
                        foreach (string component in components)
                        {
                            response.Add(component, GetComponentData(component));
                        }
                        break;
                    case RequestType.Exit:
                        response.Add(Key.ExitConfirmation, "Client exiting confirmed ...");
                        break;
                    default:
                        response.Add(Key.Error, $"UNSUPPORTED REQUEST TYPE --> {request[Key.RequestType]}\n" +
                                                $"Examples of valid Request types are: {RequestType.Command} {RequestType.Data} {RequestType.Exit}");
                        break;
                }
            }
            catch (Exception e)
            {
                response.Add(Key.Error, $"Unable to handle request. \nException message: \n{e.Message}");
            }

            return response;
        }

        private string GetComponentData(string component)
        {
            switch (component)
            {
                case Component.Ultrasound:
                    return $"Left: {_ultrasonic.Left},  Fwd: {_ultrasonic.Fwd},  Right: {_ultrasonic.Right}.";
                case Component.Wheel:
                    return $"Left: {_wheel.CurrentSpeedLeft},  Right: {_wheel.CurrentSpeedRight}.";
                case Component.Lidar:
                    if (_lidar.RunCollector) return $"Left: {_lidar.Left},  Fwd: {_lidar.Fwd},  Right: {_lidar.Right},  Largest distance: {_lidar.LargestDistance}";
                    else return "You must start Lidar Collector before you can collect data from it.";
                case Component.Encoder:
                    return $"Encoders has not been configured for data collection via socket";
                default:
                    return $"THIS COMPONENT DOES NOT SUPPORT DATA REQUESTS --> {component}.";
            }
        }

        private string HandleComponentCommand(Dictionary<string, string> request)
        {
            switch (request["COMPONENT"])
            {
                case Component.Wheel:
                    return HandleWheelCommand(request);
                default:
                    return $"THIS COMPONENT DOES NOT SUPPORT COMMAND REQUESTS --> {request["COMPONENT"]}.";
            }
        }

        private string HandleWheelCommand(Dictionary<string, string> request)
        {
            if (!request.ContainsKey(Key.Left) || !request.ContainsKey(Key.Right))
            {
                throw new Exception("Commands to the Wheel components requires the following two keys:\n" +
                                    $"{Key.Left}, {Key.Right}");
            }

            int requestedLeftSpeed = Int32.Parse(request[Key.Left]);
            int requestedRightSpeed = Int32.Parse(request[Key.Right]);

            bool requestStop = requestedLeftSpeed == 0 && requestedRightSpeed == 0;

            switch (requestStop)
            {
                case true when _exampleLogic.ActiveExampleLogic.RunExampleLogic:
                    _exampleLogic.ActiveExampleLogic.RunExampleLogic = false;
                    _wheel.SetSpeed(0, 0, false);
                    return $"Stopped demo logic: \'{_exampleLogic.ActiveExampleLogic.Details.Title}\'";
                case true when _studentLogicService.ActiveStudentLogic.RunStudentLogic:
                    _studentLogicService.ActiveStudentLogic.RunStudentLogic = false;
                    _wheel.SetSpeed(0, 0, false);
                    return $"Stopped student logic: \'{_studentLogicService.ActiveStudentLogic.Details.Title}\'";
                case false when _exampleLogic.ActiveExampleLogic.RunExampleLogic || _studentLogicService.ActiveStudentLogic.RunStudentLogic:
                    return "Can't give command while control logic is running. Push stop button first.";
                default:
                    _wheel.SetSpeed(requestedLeftSpeed, requestedRightSpeed);
                    return $"Left: {_wheel.CurrentSpeedLeft},  Right: {_wheel.CurrentSpeedRight}.";
            }
        }
    }
}
