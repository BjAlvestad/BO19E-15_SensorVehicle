﻿namespace Communication
{
    public interface IPower
    {
        bool Lidar { get; set; }
        bool Ultrasound { get; set; }
        bool Wheels { get; set; }
        bool Encoder { get; set; }
        bool Spare1 { get; set; }
        bool Spare2 { get; set; }
        bool Spare3 { get; set; }
    }
}