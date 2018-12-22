using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SimulatorUwpXaml
{
    public class Hud
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _font;

        public Hud(SpriteBatch spriteBatch, SpriteFont fontForHUD)
        {
            _spriteBatch = spriteBatch;
            _font = fontForHUD;

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

        public static HudPosition DistanceDataPosition { get; set; }
        public static HudPosition VehicleDataPosition { get; set; }
        public static HudPosition DebugDataPosition { get; set; }

        //private void DrawDistances(Distance distance) // Distance class not yet created
        //{
        //    spriteBatch.DrawString(font, $"^ {distance.Fwd}", DistanceDataPosition.Top, Color.Black);
        //    spriteBatch.DrawString(font, $"{distance.Left} <", DistanceDataPosition.Left, Color.Black);
        //    spriteBatch.DrawString(font, $"> {distance.Right}", DistanceDataPosition.Right, Color.Black);
        //    spriteBatch.DrawString(font, $"V: {distance.Aft}", DistanceDataPosition.Bottom, Color.Black);
        //}

        public void DrawVehicleData(VehicleSprite vehicle)
        {
            _spriteBatch.DrawString(_font, $"Direction:\n {vehicle.AngleInDegrees}", VehicleDataPosition.Top, Color.Black);
            _spriteBatch.DrawString(_font, $"Speed Left:\n {vehicle.SpeedLeftWheel}", VehicleDataPosition.Left, Color.Black);
            _spriteBatch.DrawString(_font, $"Speed Right:\n {vehicle.SpeedRightWheel}", VehicleDataPosition.Right, Color.Black);
        }

        public void DrawDebugMessages(string mousePosition, string vehiclePosition)
        {
            _spriteBatch.DrawString(_font, $"Mouse position:\n {mousePosition}", DebugDataPosition.Left, Color.Black);
            _spriteBatch.DrawString(_font, $"Vehicle position:\n {vehiclePosition}", DebugDataPosition.Right, Color.Black);
        }

        public void DrawDebugMouseOverObject(SpriteClass sprite)
        {
            string message = Picking2D.IsMouseIntersectingSprite(sprite) ? "Mouse Over:\n  Car" : "Mouse Over:\n  None";
            _spriteBatch.DrawString(_font, message, DebugDataPosition.Top, Color.Black);
        }

        public static void SetHudsToDefaultPositions()
        {
            DistanceDataPosition = new HudPosition(new Vector2(150, 150), 75);
            VehicleDataPosition = new HudPosition(new Vector2(Screen.Width - Screen.ScaleToHighDPI(200), Screen.ScaleToHighDPI(100)), (75));
            DebugDataPosition = new HudPosition(new Vector2(Screen.Width - Screen.ScaleToHighDPI(300), Screen.Height - Screen.ScaleToHighDPI(100)), 75);
        }
    }
}
