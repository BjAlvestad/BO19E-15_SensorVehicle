using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleEquipment.Locomotion.Encoder
{
    public class Encoder : IEncoder
    {
        private readonly IVehicleCommunication vehicleCommunication ;

        private int secSinceLastMessage;
        private double totalDistanceTraveled;
        private double cmTravelled;

        public int SecScinceLastMessage
        {
            get { return secSinceLastMessage; }
            set { secSinceLastMessage = value; }
        }
        public double TotalDistanceTravelled
        {
            get { return this.totalDistanceTraveled; }
            set { totalDistanceTraveled = value; }
        }
        public double CmTravelled
        {
            get { return this.cmTravelled; }
            set { cmTravelled = value; }
        }
        public double AvgVel
        {
            get { return cmTravelled / secSinceLastMessage; }
            set { AvgVel = value; }
        }

        public Encoder(IVehicleCommunication comWithEncoder)
        {
            vehicleCommunication = comWithEncoder;
        }

        public double[] GetEncoderData()
        {
            byte[] response = new byte[20];
            double[] returnArray = new double[3];
            bool positiveNum;
            byte firstByte;
            byte secondByte;

            try
            {
                response = vehicleCommunication.Read(); // this funtion will request data from Arduino and read it

                if (response[4] == 0)   //BUG: No one knows why, but the first element is always set to 0
                {
                    positiveNum = true;
                }
                else
                {
                    positiveNum = false;
                }

                firstByte = response[1];
                secondByte = response[2];
                secSinceLastMessage = response[3];
                if (positiveNum)
                {
                    CmTravelled = Convert.ToInt16((firstByte << 8) | secondByte);
                }
                else
                {
                    cmTravelled = Convert.ToInt16((firstByte << 8) | secondByte);
                    cmTravelled = -cmTravelled;
                }
                AvgVel = (cmTravelled / ((500.0) / (1000))); //Enhet: cm/s   Her er 500 hentet fra timer.intervall
            }
            catch (Exception p)
            {
                cmTravelled = double.NaN;
                AvgVel = double.NaN;
            }

            totalDistanceTraveled += cmTravelled;
            returnArray[0] = cmTravelled;
            returnArray[1] = AvgVel;
            returnArray[2] = response[3];
            return returnArray;
        }
    }
}
