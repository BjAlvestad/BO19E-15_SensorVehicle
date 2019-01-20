namespace VehicleEquipment.DistanceMeasurement.Lidar
{
    /// <summary>
    /// <para>The Lidars vertical angle from center</para>
    /// <para>(The enum values correspond to channel number in datablock containing the distance for the named vertical angle)</para>
    /// </summary>
    public enum VerticalAngle
    {
        Down15 = 0,
        Up1 = 1,
        Down13 = 2,
        Up3 = 3,  // Manual claims that channel 3 is Down3, but we assume it's a typo in the manual, and should actually be Up3 (as can be seen from the pattern).
        Down11 = 4,
        Up5 = 5,
        Down9 = 6,
        Up7 = 7,
        Down7 = 8,
        Up9 = 9,
        Down5 = 10,
        Up11 = 11,
        Down3 = 12,
        Up13 = 13,
        Down1 = 14,
        Up15 = 15
    }
}
