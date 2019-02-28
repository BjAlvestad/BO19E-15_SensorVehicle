using System;
using System.Collections.Generic;
using VehicleEquipment;

namespace VehicleEquipment
{
    public static class ArrayConverter
    {
        public static byte[] ToByteArray(int byteArraySize, Device deviceAddress, MessageCode message, params int[] integers)
        {
            byte[] byteArray = DisassembleIntsToByteArray(byteArraySize, 3, integers);
            byteArray[0] = (byte) deviceAddress;
            byteArray[1] = (byte) message;
            byteArray[2] = (byte) integers.Length;

            return byteArray;
        }

        private static byte[] DisassembleIntsToByteArray( int byteArraySize, int emptyElements, params int[] intsToDisassemble)
        {
            const int bitSizeOfInt = sizeof(int) * 8;
            byte[] byteArray = new byte[byteArraySize];

            for (int i = 0; i < intsToDisassemble.Length; i++)
            {
                int shiftByInteger = sizeof(int) * i;

                for (int j = 1; j <= sizeof(int); j++)
                {
                    byteArray[(emptyElements - 1) + shiftByInteger + j] = (byte) (intsToDisassemble[i] >> (bitSizeOfInt - 8 * j));
                }
            }

            return byteArray;
        }

        public static VehicleDataPacket AssembleDataFromVehicle(byte[] vehicleByteArray)
        {
            VehicleDataPacket assembledDataPack = new VehicleDataPacket
            {
                DeviceAddress = (Device) vehicleByteArray[0],
                Code = (MessageCode) vehicleByteArray[1]
            };
            int numberOfInts = vehicleByteArray[2];
            assembledDataPack.Integers = AssembleIntsFromByteArray(numberOfInts, 3, vehicleByteArray);

            return assembledDataPack;
        }

        private static List<int> AssembleIntsFromByteArray(int numberOfInts, int startIndex, byte[] array)
        {
            if(array.Length < (sizeof(int)*numberOfInts + startIndex)) throw new IndexOutOfRangeException($"Not possible to assemble {numberOfInts} ints starting from index {startIndex} from an array of length {array.Length}");

            const int bitsInInteger = sizeof(int) * 8;
            List<int> integers = new List<int>();

            for (int i = 0; i < numberOfInts; i++)
            {
                int assembledInteger = 0;
                int shiftByInteger = sizeof(int) * i;

                for (int j = 1; j <= sizeof(int); j++)
                {
                    assembledInteger |= array[startIndex -1 + j + shiftByInteger] << (bitsInInteger - 8*j);
                }

                integers.Add(assembledInteger);
            }

            return integers;
        }
    }
}
