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
            Drive2(elapsedTimeSinceLastUpdate);
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
    }
}
