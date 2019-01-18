using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace SimulatorUwpXaml
{
    public class SimulatorMap
    {
        private const string BaseMapFolder = "Maps";

        private readonly TmxMap _map;
        private readonly Texture2D _tileset;
        private readonly int _tileWidth;
        private readonly int _tileHeight;
        private int _tilesetTilesWide;
        private int _tilesetTilesHigh;
        private List<Tile> _wallTiles = new List<Tile>();
        private List<Tile> _floorTiles = new List<Tile>();
        public readonly List<BoundingBox> Boundaries = new List<BoundingBox>(); //TODO: Check if this can be better encapsulated / protected against acidental edits from outside.


        public SimulatorMap(ContentManager content, string mapName, float scale)
        {
            _map = new TmxMap($"{content.RootDirectory}/{BaseMapFolder}/{mapName}");
            _tileset = content.Load<Texture2D>($"{BaseMapFolder}/{_map.Tilesets[0].Name}");

            _tileWidth = _map.Tilesets[0].TileWidth;
            _tileHeight = _map.Tilesets[0].TileHeight;
            _tilesetTilesWide = _tileset.Width / _tileWidth;
            _tilesetTilesHigh = _tileset.Height / _tileHeight;

            LoadMap(scale);
        }

        private void LoadMap(float scale)
        {
            for (int j = 0; j < _map.Layers.Count; j++)
            {
                for (var i = 0; i < _map.Layers[j].Tiles.Count; i++)
                {
                    int gid = this._map.Layers[j].Tiles[i].Gid;
                    if (gid != 0)
                    {
                        int tileFrame = gid - 1;
                        int row = tileFrame / (_tileset.Height / _tileHeight);

                        float x = (i % _map.Width) * _map.TileWidth ;
                        float y = (float)Math.Floor(i / (double)_map.Width) * _map.TileHeight;

                        Rectangle tilesetRec = new Rectangle(_tileWidth * tileFrame, _tileHeight * row, _tileWidth, _tileHeight);
                        Rectangle mapRectangle = new Rectangle((int) (x * scale), (int) (y*scale), (int)(_tileWidth*scale), (int)(_tileHeight*scale));

                        if (j == 1)
                            _wallTiles.Add(new Tile(mapRectangle, tilesetRec));
                        else
                        {
                            _floorTiles.Add(new Tile(mapRectangle, tilesetRec));
                        }
                    }
                }
            }

            LoadBoundaries();
        }

        private void LoadBoundaries()
        {
            if (_wallTiles == null)
            {
                return;
            }

            foreach(Tile tile in _wallTiles)
            {
                Vector3 startBoundary = new Vector3(tile.MapRectangle.X, tile.MapRectangle.Y, 0f);
                Vector3 endBoundary =  new Vector3(tile.MapRectangle.X + tile.MapRectangle.Width, tile.MapRectangle.Y + tile.MapRectangle.Height, 0f);
                Boundaries.Add(new BoundingBox(startBoundary, endBoundary));
            }
        }

        public void DrawMap(SpriteBatch spriteBatch)
        {
            foreach (Tile floorTile in _floorTiles)
            {
                spriteBatch.Draw(_tileset, floorTile.MapRectangle, floorTile.TileSetRectangle, Color.White);
            }

            foreach (Tile wallTile in _wallTiles)
            {
                spriteBatch.Draw(_tileset, wallTile.MapRectangle, wallTile.TileSetRectangle, Color.White);
            }
        }
    }
}
