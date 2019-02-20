using System.Collections.Generic;

namespace VehicleEquipment
{
    public struct VehicleDataPacket
    {
        public Device DeviceAddress { get; set; }
        public MessageCode Code { get; set; }
        public List<int> Integers { get; set; }
    }
}
