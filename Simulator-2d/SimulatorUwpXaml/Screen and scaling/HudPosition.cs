using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SimulatorUwpXaml
{
    public class HudPosition
    {
        public HudPosition(Vector2 origin, int distancesFromOrigin)
        {
            Origin = origin;
            SetVerticalDistancesFromOrigin(distancesFromOrigin);
            SetHorizontalDistancesFromOrigin(distancesFromOrigin);
        }

        public Vector2 Origin { get; set; }
        public Vector2 Top { get; set; }
        public Vector2 Left { get; set; }
        public Vector2 Right { get; set; }
        public Vector2 Bottom { get; set; }

        public void SetVerticalDistancesFromOrigin(int distanceFromOrigin)
        {
            Vector2 distanceDown = new Vector2(0, distanceFromOrigin);
            Bottom = Origin + distanceDown;
            Top = Origin - distanceDown;
        }

        public void SetHorizontalDistancesFromOrigin(int distanceFromOrigin)
        {
            Vector2 distanceRight = new Vector2(distanceFromOrigin, 0);
            Right = Origin + distanceRight;
            Left = Origin - distanceRight;
        }
    }
}
