﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ExampleLogic.L1_CenterCorridor;
using ExampleLogic.L2_RightHandSearch;
using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Wheels;

namespace ExampleLogic
{
    public class ExampleLogicService
    {
        private static IEnumerable<ExampleLogicBase> AllExamples(IWheel wheels, ILidarDistance lidarDistance, IUltrasonic ultrasonic)
        {
            var examples = new ObservableCollection<ExampleLogicBase>
            {
                new CenterCorridorMain(wheels, ultrasonic),
                new RightHandSearchMain(wheels, ultrasonic)
            };

            return examples;
        }

        public async Task<IEnumerable<ExampleLogicBase>> GetExampleLogicAsync(IWheel wheels, ILidarDistance lidarDistance, IUltrasonic ultrasonic)
        {
            return await Task.FromResult(AllExamples(wheels, lidarDistance, ultrasonic));
        }
    }
}
