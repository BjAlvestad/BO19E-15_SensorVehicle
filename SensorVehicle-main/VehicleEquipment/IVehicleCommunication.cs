using System;
using System.Threading;
using System.Threading.Tasks;

namespace VehicleEquipment
{
    public interface IVehicleCommunication
    {
        void Write(byte[] data);

        byte[] Read();
    }
}

