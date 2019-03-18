using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SimulatorUwpXaml
{
    public class HudViewModel : INotifyPropertyChanged
    {
        private readonly SynchronizationContext _uiSyncContext;

        private readonly Lidar _lidar;
        private readonly VehicleSprite _vehicle;
        private readonly Camera _camera;
        private readonly Picking2D _picking;

        public HudViewModel(Lidar lidar, VehicleSprite vehicle, Camera camera, Picking2D picking)
        {
            _uiSyncContext = SynchronizationContext.Current;

            _lidar = lidar;
            _vehicle = vehicle;
            _camera = camera;
            _picking = picking;
        }

        public double DistanceFwd { get; private set; }
        public double DistanceLeft { get; private set; }
        public double DistanceRight { get; private set; }
        public double DistanceAft { get; private set; }

        public int WheelSpeedLeft { get; private set; }
        public int WheelSpeedRight { get; private set; }
        public string VehicleDirection { get; private set; }

        public string MousePosScreenRaw { get; set; }
        public string MousePosScreen { get; set; }
        public string MousePosWorld { get; set; }
        public string VehiclePosWorld { get; set; }

        public string MouseOverObject { get; set; }

        public void StopWheels()
        {
            _vehicle.SpeedLeftWheel = 0;
            _vehicle.SpeedRightWheel = 0;
        }

        public void RefreshHud(params SpriteClass[] spritesInGame)
        {
            RefreshDistances();
            RefreshVehicleData();
            RefreshDebugData();
            RefreshMouseOverObject(spritesInGame);
        }

        private void RefreshDistances()
        {
            const int numberOfDecimals = 2;

            DistanceFwd = Math.Round(_lidar.Fwd, numberOfDecimals);
            DistanceLeft =  Math.Round(_lidar.Left, numberOfDecimals);
            DistanceRight =  Math.Round(_lidar.Right, numberOfDecimals);
            DistanceAft =  Math.Round(_lidar.Aft, numberOfDecimals);

            RaiseSyncedPropertyChanged(nameof(DistanceFwd));
            RaiseSyncedPropertyChanged(nameof(DistanceLeft));
            RaiseSyncedPropertyChanged(nameof(DistanceRight));
            RaiseSyncedPropertyChanged(nameof(DistanceAft));
        }

        private void RefreshVehicleData()
        {
            WheelSpeedLeft = _vehicle.SpeedLeftWheel;
            WheelSpeedRight = _vehicle.SpeedRightWheel;
            VehicleDirection = $"{_vehicle.AngleInDegrees:#.0}";

            RaiseSyncedPropertyChanged(nameof(WheelSpeedLeft));
            RaiseSyncedPropertyChanged(nameof(WheelSpeedRight));
            RaiseSyncedPropertyChanged(nameof(VehicleDirection));
        }

        private void RefreshDebugData()
        {
            Vector2 mouseOnScreenRaw = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            MousePosScreenRaw = $"{VectorToString(mouseOnScreenRaw)}\t(Screen - raw)";
            RaiseSyncedPropertyChanged(nameof(MousePosScreenRaw));

            Vector2 mouseOnScreen = mouseOnScreenRaw - new Vector2(Screen.Width/2, Screen.Height/2);
            MousePosScreen = $"{VectorToString(mouseOnScreen)}\t(Screen - center origo)";
            RaiseSyncedPropertyChanged(nameof(MousePosScreen));

            MousePosWorld = $"{VectorToString(Picking2D.MouseLocationInWorld())}\t(World - from screen center origo)";
            RaiseSyncedPropertyChanged(nameof(MousePosWorld));

            VehiclePosWorld = $"{VectorToString(_vehicle.Position)}\t(World)";
            RaiseSyncedPropertyChanged(nameof(VehiclePosWorld));
        }

        private string VectorToString(Vector2 vector)
        {
            return $"[X:{vector.X:000}, Y:{vector.Y:000}]";
        }

        private void RefreshMouseOverObject(IEnumerable<SpriteClass> sprites)
        {
            bool mouseIntersectsSprite = false;
            foreach (SpriteClass sprite in sprites)
            {
                if (Picking2D.IsMouseIntersectingSprite(sprite)) mouseIntersectsSprite = true;
            }

            MouseOverObject = mouseIntersectsSprite ? "Mouse Over: Sprite" : "Mouse Over: None";
            RaiseSyncedPropertyChanged(nameof(MouseOverObject));
        }

        protected virtual void RaiseSyncedPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (_uiSyncContext == null)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                _uiSyncContext.Post((_) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName)), null);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
    }
}
