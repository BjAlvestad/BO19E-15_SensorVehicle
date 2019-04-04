using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace SimulatorUwpXaml
{
    public class VehicleSprite : SpriteClass
    {
        private int _speedLeftWheel;
        private int _speedRightWheel;
        private float _angle;
        private readonly float _wheelBase;

        public VehicleSprite(GraphicsDevice graphicsDevice, Texture2D spriteTexture, float scale) : base(graphicsDevice, spriteTexture, scale)
        {
            _wheelBase = spriteTexture.Height * scale;
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
            Drive4(elapsedTimeSinceLastUpdate);
        }

        #region First attempt at Drive methods
        private void Drive(float elapsedTimeSinceLastUpdate)    // BUG: Formlene er ikke korrekt enda mtp fysikk
        {
            float linearVelocity = (SpeedLeftWheel + SpeedRightWheel)/2f;  // ??? vet ikke om det kan regnes ut slik

            PhysicsTurn(SpeedLeftWheel, SpeedRightWheel, linearVelocity);

            if (linearVelocity > 0.000001 || linearVelocity < 0.000001)
            {
                PhysicsLinearMove(linearVelocity);
            }          
        }

        private void PhysicsTurn(int speedLeft, int speedRight, float linearVelocity)
        {
            float turnRadius = (_wheelBase * linearVelocity) / (speedLeft - speedRight);  // d*SpeedInner / (SpeedOuter - SpeedInner)

            float angularVelocity = linearVelocity / turnRadius;

            float angleToTurn = (linearVelocity / turnRadius); // Skulle også vært med tid

            if (float.IsNaN(angleToTurn))
            {
                angleToTurn = -0.01f * Math.Abs(speedLeft);
            }

            Angle += angleToTurn;
        }

        private void PhysicsLinearMove(float linearVelocity)    // BUG : Bil beveger seg ikke når vi har ulikt fortegn på beltehastighetene.
        {  
            Vector2 directionToDrive = new Vector2((float)Math.Cos(Angle), (float)Math.Sin(Angle));
            Position = float.IsNaN(Angle) ? Position : Position + directionToDrive * linearVelocity;
        }
        #endregion

        #region Second attempt at drive methods
        public void Drive2(float elapsedTimeSinceLastUpdate)
        {
            //const float velocityFactor = 0.025f;
            const float maxAngularVelocity = 5f;

            SpeedLeftWheel = Math.Abs(SpeedLeftWheel) > 100 ? Math.Sign(SpeedLeftWheel) * 100 : SpeedLeftWheel;
            SpeedRightWheel = Math.Abs(SpeedRightWheel) > 100 ? Math.Sign(SpeedRightWheel) * 100 : SpeedRightWheel;

            //speedLeft *= velocityFactor;
            //speedRight *= velocityFactor;

            float linearVelocity = (SpeedLeftWheel + SpeedRightWheel) / 2f;  // ??? vet ikke om det kan regnes ut slik
            float angularVelocity = 0;

            if (SpeedLeftWheel != SpeedRightWheel)
            {
                float turnRadius = (_wheelBase * linearVelocity) / (SpeedLeftWheel - SpeedRightWheel);  // d*SpeedInner / (SpeedOuter - SpeedInner)
                angularVelocity = linearVelocity / turnRadius;
                if (angularVelocity > maxAngularVelocity || float.IsNaN(angularVelocity))
                {
                    angularVelocity = maxAngularVelocity;
                }
            }

            // BUG: Denne if setningen er for å prøve å fikse når vi har turn-radius 0 (snur "on the spot)"
            //if (Math.Abs(speedLeft + speedRight) < 0.0000000000000000001)
            //{
            //    float combinedSpeed = (speedLeft - speedRight);
            //    angularVelocity = Math.Abs(combinedSpeed) > maxAngularVelocity ? Math.Sign(combinedSpeed) * maxAngularVelocity : combinedSpeed;
            //}

           
            Angle += (float)(elapsedTimeSinceLastUpdate * angularVelocity);  // theta = omega * tid

            Position += new Vector2(elapsedTimeSinceLastUpdate * linearVelocity * (float)Math.Cos(Angle), elapsedTimeSinceLastUpdate * linearVelocity * (float)Math.Sin(Angle));

            
            //Vector2 displacedPosition = position;
            //if (float.IsInfinity(turnRadius))
            //{
            //    position += new Vector2(linearVelocity * (float)Math.Cos(angle), linearVelocity * (float)Math.Sin(angle));
            //}
            //else
            //{
            //    position = new Vector2(turnRadius * (float)Math.Cos(angle), turnRadius * (float)Math.Sin(angle));
            //}

            //position = displacedPosition;
        }
        #endregion

        public void Drive4(float elapsedTimeSinceLastUpdate)
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

        private float CalculateTurnRadius(float speedInner, float speedOuter)
        {
            return (_wheelBase * speedInner) / (speedOuter - speedInner) / 3;
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
            return 2103.6 * Math.Pow(speedDifferance, -1.258);
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
