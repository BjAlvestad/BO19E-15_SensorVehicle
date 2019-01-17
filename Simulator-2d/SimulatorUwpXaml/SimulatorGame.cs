using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SimulatorUwpXaml
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class SimulatorGame : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        private Hud _hud;


        private VehicleSprite _vehicle;

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
            this.IsMouseVisible = true; 
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

            Texture2D carTexture = Content.Load<Texture2D>("SpriteImages/car_red");
            _vehicle = new VehicleSprite(GraphicsDevice, carTexture, Screen.ScaleToHighDPI(0.205f));
            _vehicle.Position = new Vector2(1500, 300);

            _hud = new Hud(_spriteBatch, Content.Load<SpriteFont>("HUD/HudDistance"));
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
            float elapsedTimeSinceLastUpdate = (float)gameTime.ElapsedGameTime.TotalSeconds; // Get time elapsed since last Update iteration
            _vehicle.Update(elapsedTimeSinceLastUpdate);

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
            _vehicle.Draw(_spriteBatch);

            _hud.DrawVehicleData(_vehicle);
            _hud.DrawDebugMessages($"X: {Mouse.GetState().X}  Y: {Mouse.GetState().Y}", $"{_vehicle.Position}");
            _hud.DrawDebugMouseOverObject(_vehicle);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
