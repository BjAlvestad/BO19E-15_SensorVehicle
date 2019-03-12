using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SimulatorUwpXaml
{
    public class Hud
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _font;
        private readonly Color _hudColor = Color.White;
        private readonly Camera _camera;

        public Hud(SpriteBatch spriteBatch, SpriteFont fontForHUD, Camera camera)
        {
            _spriteBatch = spriteBatch;
            _font = fontForHUD;
            _camera = camera;

            SetHudsToDefaultPositions();
            //DisplayInformation.GetForCurrentView().DpiChanged += Screen_DpiChanged;
            ApplicationView.GetForCurrentView().VisibleBoundsChanged += Screen_VisibleBoundsChanged;
        }

        //private void Screen_DpiChanged(DisplayInformation sender, object args)
        //{
        //    SetHudsToDefaultPositions();
        //}

        private void Screen_VisibleBoundsChanged(ApplicationView sender, object args)
        {
            SetHudsToDefaultPositions();
        }

        public HudPosition DistanceDataPosition { get; set; }
        public HudPosition VehicleDataPosition { get; set; }
        public HudPosition DebugDataPosition { get; set; }

        public void DrawDistances(Lidar distance)
        {
            _spriteBatch.DrawString(_font, $"^ {distance.Fwd}", DistanceDataPosition.Top, _hudColor);
            _spriteBatch.DrawString(_font, $"{distance.Left} <", DistanceDataPosition.Left, _hudColor);
            _spriteBatch.DrawString(_font, $"> {distance.Right}", DistanceDataPosition.Right, _hudColor);
            _spriteBatch.DrawString(_font, $"V: {distance.Aft}", DistanceDataPosition.Bottom, _hudColor);
        }

        public void DrawVehicleData(VehicleSprite vehicle)
        {
            _spriteBatch.DrawString(_font, $"Direction:\n {vehicle.AngleInDegrees}", VehicleDataPosition.Top, _hudColor);
            _spriteBatch.DrawString(_font, $"Speed Left:\n {vehicle.SpeedLeftWheel}", VehicleDataPosition.Left, _hudColor);
            _spriteBatch.DrawString(_font, $"Speed Right:\n {vehicle.SpeedRightWheel}", VehicleDataPosition.Right, _hudColor);
        }

        public void DrawDebugMessages(string mousePosition, string vehiclePosition)
        {
            _spriteBatch.DrawString(_font, $"Mouse pos. (screen):\n {mousePosition}", DebugDataPosition.Left, _hudColor);
            _spriteBatch.DrawString(_font, $"Vehicle pos (world):\n {vehiclePosition}", DebugDataPosition.Right, _hudColor);
        }

        public void DrawDebugMouseOverObject(SpriteClass sprite)
        {
            string message = Picking2D.IsMouseIntersectingSprite(sprite) ? "Mouse Over:\n  Car" : "Mouse Over:\n  None";
            _spriteBatch.DrawString(_font, message, DebugDataPosition.Top, _hudColor);
        }

        public void SetHudsToDefaultPositions()
        {
            DistanceDataPosition = new HudPosition(new Vector2(150, 150), 75);
            VehicleDataPosition = new HudPosition(new Vector2(Screen.Width - Screen.ScaleToHighDPI(200), Screen.ScaleToHighDPI(100)), (75));
            DebugDataPosition = new HudPosition(new Vector2(Screen.Width - Screen.ScaleToHighDPI(300), Screen.Height - Screen.ScaleToHighDPI(100)), 75);
        }

        public void SetHudsToDefaultPositionsInWorld(Camera camera)
        {
            float widthFromCenter = Screen.Width / 2;
            float heightFromCenter = Screen.Height / 2;

            Vector2 distanceDataOrigin = new Vector2(ScaledAndZoomedPixles(100) - widthFromCenter, ScaledAndZoomedPixles(100) - heightFromCenter);
            Vector2 distanceDataInWorld = Vector2.Zero;
            camera.ToWorld(ref distanceDataOrigin, out distanceDataInWorld);
            
            Vector2 vehicleDataOrigin = new Vector2(widthFromCenter - ScaledAndZoomedPixles(200), ScaledAndZoomedPixles(100) - heightFromCenter);
            Vector2 vehicleDataInWorld = Vector2.Zero;
            camera.ToWorld(ref vehicleDataOrigin, out vehicleDataInWorld);
            
            Vector2 debugDataOrigin = new Vector2(widthFromCenter - ScaledAndZoomedPixles(250), heightFromCenter -ScaledAndZoomedPixles(100));
            Vector2 debugDataInWorld = Vector2.Zero;
            camera.ToWorld(ref debugDataOrigin, out debugDataInWorld);

            DistanceDataPosition = new HudPosition(distanceDataInWorld, 75);
            VehicleDataPosition = new HudPosition(vehicleDataInWorld, 75);
            DebugDataPosition = new HudPosition(debugDataInWorld, 75);
        }

        private float ScaledAndZoomedPixles(float desiredPixelDistance)
        {
            return Screen.ScaleToHighDPI(desiredPixelDistance) * _camera.Zoom;
        }
    }
}
