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

        public VehicleSprite(GraphicsDevice graphicsDevice, Texture2D spriteTexture, float scale) : base(graphicsDevice, spriteTexture, scale)
        {

        }

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

            const int speedScale = 68;
            float linearSpeed = speedScale / TimePerMeter(averagePower) * Math.Sign(averagePower);

            Angle += (float)(elapsedTimeSinceLastUpdate * angularVelocity);  // theta = omega * tid
            Position += new Vector2(elapsedTimeSinceLastUpdate * linearSpeed * (float)Math.Cos(Angle), elapsedTimeSinceLastUpdate * linearSpeed * (float)Math.Sin(Angle));
        }

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
            const double a = 46863230;
            const double b = 2.655837;
            const double c = 0.2627988;
            const double d = 2.089203;

            double pl4Regression = d + (a - b) / (1 + Math.Pow(speedDifferance / c, b));  // Symmetrical sigmoidal regression (for rotation on-the-spot).

            return (float) pl4Regression;
            //return 2126233 + (2.863412 - 2126233) / (1 + Math.Pow(speedDifferance / 4291.27, 3.70461));  // Regression with the data in wrong order
        }

        private double TimeForFullRotationWithMovement(int speedDifferance)
        {
            return 2103.6 * Math.Pow(speedDifferance, -1.258);  // R^2: 0.9551 (TODO: improve formula with more measurements from the car - must be done in large space)
        }

        private float TimePerMeter(float linearWheelPower) // Calculated based on speed in a straight line
        {
            const double a = 416.5587;
            const double b = 1.097031;
            const double c = 8.717592;
            const double d = -8.155351;

            float absoluteLinearWheelPower = Math.Abs(linearWheelPower);
            double pl4Regression = d + (a - b) / (1 + Math.Pow(absoluteLinearWheelPower / c, b));  // Symmetrical sigmoidal regression (for rotation on-the-spot).

            return (float) pl4Regression / 10;
            //return 2126233 + (2.863412 - 2126233) / (1 + Math.Pow(speedDifferance / 4291.27, 3.70461));  // Regression with the data in wrong order
        }
    }
}
