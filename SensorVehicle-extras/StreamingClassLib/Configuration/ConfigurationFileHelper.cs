using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorVehicle_extras.Configuration
{
    public enum VideoResolution
    {
        SD640_480
    }

    public enum VideoSubtype
    {
        NV12
    }    

    public class VideoResolutionWidthHeight
    {
        public int Width { get; set; }
        public int Height { get; set; }        
    }

    public class VideoSetting
    {
        public VideoResolution VideoResolution { get; set; }
        public VideoSubtype VideoSubtype { get; set; }
        public double VideoQuality { get; set; }
        public int UsedThreads { get; set; }
    }
}
