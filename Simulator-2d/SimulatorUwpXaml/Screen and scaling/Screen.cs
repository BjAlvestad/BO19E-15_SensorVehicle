using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;

namespace SimulatorUwpXaml
{
    static class Screen
    {
        static Screen()
        {
            RawPixelsPerViewPixel = (float)DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            Height = ScaleToHighDPI((float)ApplicationView.GetForCurrentView().VisibleBounds.Height);
            Width = ScaleToHighDPI((float)ApplicationView.GetForCurrentView().VisibleBounds.Width);

            DisplayInformation.GetForCurrentView().DpiChanged += Screen_DpiChanged;
            ApplicationView.GetForCurrentView().VisibleBoundsChanged += Screen_VisibleBoundsChanged;
        }

        private static void Screen_DpiChanged(DisplayInformation sender, object args)
        {
            RawPixelsPerViewPixel = (float) sender.RawPixelsPerViewPixel;
        }

        private static void Screen_VisibleBoundsChanged(ApplicationView sender, object args)
        {
            Height = ScaleToHighDPI((float)sender.VisibleBounds.Height);
            Width = ScaleToHighDPI((float)sender.VisibleBounds.Width);
        }

        public static float Height { get; set; }
        public static float Width { get; set; }

        public static float RawPixelsPerViewPixel { get; private set; }

        public static float ScaleToHighDPI(float f)
        {
            f *= RawPixelsPerViewPixel;
            return (float)Math.Round(f, 1);
        }
    }
}
