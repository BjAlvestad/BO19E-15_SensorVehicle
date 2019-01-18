using Microsoft.Xna.Framework;

namespace SimulatorUwpXaml
{
    public struct Tile
    {
        public Rectangle MapRectangle { get; }
        public Rectangle TileSetRectangle { get; }

        public Tile(Rectangle mapRectangle, Rectangle tileSetRectangle)
        {
            MapRectangle = mapRectangle;
            TileSetRectangle = tileSetRectangle;
        }
    }
}
