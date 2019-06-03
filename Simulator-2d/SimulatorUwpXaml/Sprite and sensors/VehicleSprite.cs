using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace SimulatorUwpXaml
{
    public class VehicleSprite : SpriteClass
    {
        private int _speedLeftWheel;
        private int _speedRightWheel;
        private float _angle;
        private float _mapScale;

        public float CmTraveledLeftWheel { get; set; }
        public float CmTraveledRightWheel { get; set; }

        public VehicleSprite(GraphicsDevice graphicsDevice, Texture2D spriteTexture, float textureScale, float mapScale) : base(graphicsDevice, spriteTexture, textureScale)
        {
            CarPhysicsRegressionType = RegressionType.SymmetricalSigmoidalPl4;
            _mapScale = mapScale;
        }

        public RegressionType CarPhysicsRegressionType { get; set; }

        /// <summary>
        /// Angle in radians (clockwise rotation). 0 rad is towards right.
        /// </summary>
        public override float Angle
        {
            get => _angle;
            set
            {
                _angle = ValidatedAngle(value);
                AngleInDegrees = _angle * 180 / (float) Math.PI;
            }
        }

        /// <summary>
        /// Angle in degrees (clockwise rotation). 0 degrees is towards right.
        /// </summary>
        public float AngleInDegrees { get; private set; }

        public int SpeedLeftWheel
        {
            get => _speedLeftWheel;
            set => _speedLeftWheel = ValidatedWheelSpeed(value);
        }

        public int SpeedRightWheel
        {
            get => _speedRightWheel;
            set => _speedRightWheel = ValidatedWheelSpeed(value);
        }

        private static float ValidatedAngle(float radAngle)
        {
            if (radAngle < 0)
            {
                return (float) (radAngle + 2 * Math.PI);
            }

            int numberOfRevolutions = (int)Math.Truncate(radAngle / (2 * Math.PI));
            return (float)(radAngle - 2 * Math.PI * numberOfRevolutions);
        }

        private static int ValidatedWheelSpeed(int requestedSpeed)
        {
            if (requestedSpeed > -100 && requestedSpeed < 100)
            {
                return requestedSpeed;
            }
            
            return (Math.Sign(requestedSpeed) == 1) ? 100 : -100;
        }

        protected override void Move(float elapsedTimeSinceLastUpdate)
        {
            float averagePower = CalculateLinearVelocity();

            int speedInner = Math.Min(SpeedLeftWheel, SpeedRightWheel);
            int speedOuter = Math.Max(SpeedLeftWheel, SpeedRightWheel);
            float angularVelocity = CalculateAngularVelocity(speedInner, speedOuter, averagePower);

            const int speedScale = 88;
            float linearSpeed = speedScale / TimePerMeter(averagePower) * Math.Sign(averagePower);

            float angularDisplacement = elapsedTimeSinceLastUpdate * angularVelocity;  // theta = omega * tid
            Angle += angularDisplacement;  

            float linearDisplacement = elapsedTimeSinceLastUpdate * linearSpeed;
            Position += new Vector2(linearDisplacement * (float)Math.Cos(Angle), linearDisplacement * (float)Math.Sin(Angle));

            // Linear displacement gives a smaller distance than what the lidar measurement indicates, however the distance is the same as number of world-pixels.
            // Lidar seems to measure correct distance (according to drawing - we have decided that one pixel should equal 1 cm)
            // Suspect the issue is impreciceness in the mapScaling, but since Lidar
            const float angularScaling = 0.5f;  // TODO: check cm on encoder on real car
            CmTraveledLeftWheel += linearDisplacement / _mapScale   
                                   + (_speedLeftWheel - averagePower) * angularScaling * elapsedTimeSinceLastUpdate;
            CmTraveledRightWheel += linearDisplacement/ _mapScale 
                                    + (_speedRightWheel - averagePower) * angularScaling * elapsedTimeSinceLastUpdate;
        }

        #region VehiclePhysics
        private float CalculateLinearVelocity()
        {
            return (SpeedLeftWheel + SpeedRightWheel) / 2f;
        }

        private float CalculateAngularVelocity(int speedInner, int speedOuter, float linearVelocity)
        {
            if (speedInner == speedOuter) return 0;
            int direction = SpeedLeftWheel < SpeedRightWheel ? -1 : 1;

            double timeForFullRotation = (speedOuter != -speedInner) ? TimeForFullRotationWithMovement(speedOuter - speedInner) : TimeForFullRotationOnTheSpot(speedOuter - speedInner);

            return (float)(2*Math.PI / timeForFullRotation * direction);
        }

        private double TimeForFullRotationOnTheSpot(int speedDifferance) // Calculated based on data when rotating on the spot
        {
            switch (CarPhysicsRegressionType)
            {
                case RegressionType.SymmetricalSigmoidalPl4:
                    return SymmetricalSigmoidalPl4(speedDifferance, 46863230, 2.655837, 0.2627988, 2.089203);  // R^2 = 0.9994
                case RegressionType.Power:
                    return Power(speedDifferance, 51362, -1.86);  // R^2: 0.9931
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private double TimeForFullRotationWithMovement(int speedDifferance)
        {
            //TODO: improve formula with more measurements from the car - must be done in large space
            switch (CarPhysicsRegressionType)
            {
                case RegressionType.SymmetricalSigmoidalPl4:
                    return SymmetricalSigmoidalPl4(speedDifferance, 35582790, 1.822818, 0.01529213, 2.08377);  // R^2: 0.9912
                case RegressionType.Power:
                    return Power(speedDifferance, 2103.6, -1.258);  // R^2: 0.9551
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private float TimePerMeter(float linearWheelPower) // Calculated based on speed in a straight line
        {
            float absoluteLinearWheelPower = Math.Abs(linearWheelPower);

            switch (CarPhysicsRegressionType)
            {
                case RegressionType.SymmetricalSigmoidalPl4:
                    return (float) SymmetricalSigmoidalPl4(absoluteLinearWheelPower, 416.5587, 1.097031, 8.717592, -8.155351) / 10;  // R^2 = 0.9999
                case RegressionType.Power:
                    return (float) Power(absoluteLinearWheelPower, 3235.5, -1.1) / 10;  // R^2: 0.9942
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static double SymmetricalSigmoidalPl4(double variable, double a, double b, double c, double d)
        {
            return d + (a - b) / (1 + Math.Pow(variable / c, b));
        }

        private static double Power(double variable, double a, double b)
        {
            return a * Math.Pow(variable, b);
        }
        #endregion
    }
}
