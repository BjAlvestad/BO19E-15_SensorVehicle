﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;

namespace SimulatorUwpXaml
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class SimulatorGame : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        private Screen _screen;

        private string _mapPath;
        private TiledMap _map;   // The tile map   
        private TiledMapRenderer _mapRenderer;  // The renderer for the map

        public SimulatorGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _screen = new Screen();

            _mapPath = "Maps/SimpleTCorridor/SimpleTCorridor";
            _map = Content.Load<TiledMap>(_mapPath);  // Load the compiled map (created with TiledEditor)          
            _mapRenderer = new TiledMapRenderer(GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            _mapRenderer.Update(_map, gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);  // SamplerState.PointClamp removes gaps between tiles when rendering (reccomended)
            _mapRenderer.Draw(_map); 
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
