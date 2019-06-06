using System;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Comora;
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
        private const float GlobalScale = 0.7f;
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        private Camera _camera;
        private Picking2D _picking2D;

        public HudViewModel HUDViewModel { get; private set; }

        private Lidar _lidar;
        private SimulatorMap _simulatorMap;

        private VehicleSprite _vehicle;

        public SimulatorGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //IsFixedTimeStep = false;  // Setting this to false will cause next frame to render as soon as the previous is complete (default is true where it will occur at a fixed interval). https://stackoverflow.com/a/8554106/1821428
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 30.0);  // Sets target/max frame rate. Default is 60FPS.
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
            _camera = new Camera(_graphics.GraphicsDevice);
            _picking2D = new Picking2D(_camera);
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

            _simulatorMap = new SimulatorMap(Content, mapName: "RealMapFinite.tmx", scale: Screen.ScaleToHighDPI(1.0f * GlobalScale));

            Texture2D carTexture = Content.Load<Texture2D>("SpriteImages/SensorVehicleTop_300x300");
            _vehicle = new VehicleSprite(GraphicsDevice, carTexture, Screen.ScaleToHighDPIWithoutRounding(0.1f * GlobalScale), Screen.ScaleToHighDPI(1.0f * GlobalScale));

            _vehicle.Position = _simulatorMap.VehicleStartPosition;
            _vehicle.Angle = 4.712f;

            _lidar = new Lidar(_vehicle);

            HUDViewModel = new HudViewModel(_lidar, _vehicle, _camera, _picking2D);

            ((App) Application.Current).AppServiceProvider.InstantiateSimulatedEquipment(_vehicle, _lidar);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float elapsedTimeSinceLastUpdate = (float)gameTime.ElapsedGameTime.TotalSeconds; // Get time elapsed since last Update iteration
            _vehicle.Update(elapsedTimeSinceLastUpdate, WallClearanceOk(0.1f));

            if (DateTime.Now - _lidar.DistanceReadingAge > TimeSpan.FromMilliseconds(100))
            {
                _lidar.Update360(_simulatorMap.Boundaries, Screen.ScaleToHighDPI(1.0f * GlobalScale));
            }

            SetCameraZoom(gameTime);
            _camera.Update(gameTime);
            if (Picking2D.IsPickedUpForMove(_vehicle))
            {
                ScrollCameraIfAtEdge();
            }
            else
            {
                _camera.Position = _vehicle.Position;
            }

            HUDViewModel.RefreshHud(_vehicle);

            base.Update(gameTime);
        }

        private void SetCameraZoom(GameTime gameTime)
        {
            const float defaultZoom = 1f;
            const float minimumZoom = 0.1f;
            const float maximumZoom = 3f;
            const float zoomIncrementPerMillisecond = 0.001f;

            if (Keyboard.GetState().IsKeyDown(Keys.PageUp) && _camera.Zoom < maximumZoom) _camera.Zoom += zoomIncrementPerMillisecond * gameTime.ElapsedGameTime.Milliseconds;
            if (Keyboard.GetState().IsKeyDown(Keys.PageDown) && _camera.Zoom > minimumZoom) _camera.Zoom -= zoomIncrementPerMillisecond * gameTime.ElapsedGameTime.Milliseconds;
            if (Keyboard.GetState().IsKeyDown(Keys.Home)) _camera.Zoom = defaultZoom;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            _spriteBatch.Begin(_camera, samplerState: SamplerState.PointClamp);  // SamplerState.PointClamp removes gaps between tiles when rendering (reccomended)
            _simulatorMap.DrawMap(_spriteBatch);
            _vehicle.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private bool WallClearanceOk(float safetyDistance)
        {
            const float sensorOffsetAbreast = 50 / 2.0f / 100;
            const float sensorOffsetAbeam = 20 / 2.0f / 100;

            if (_lidar.Fwd < sensorOffsetAbreast + safetyDistance) return false;
            if (_lidar.Left < sensorOffsetAbeam + safetyDistance) return false;
            if (_lidar.Right < sensorOffsetAbeam + safetyDistance) return false;
            if (_lidar.Aft < sensorOffsetAbreast) return false;

            return true;
        }

        //TODO: Refactor scrolling into separate class
        const float border = 200;
        const float scrollSpeedReductionFactor = 50;
        private void ScrollCameraIfAtEdge()
        {
            Vector2 mousePosition = Picking2D.MouseLocation();

            if (mousePosition.X < border) _camera.Position = ScrollLeft(mousePosition);
            if (mousePosition.X > Screen.Width - border) _camera.Position = ScrollRight(mousePosition);

            if (mousePosition.Y < border) _camera.Position = ScrollUp(mousePosition);
            if (mousePosition.Y > Screen.Height - border) _camera.Position = ScrollDown(mousePosition);
        }

        private Vector2 ScrollLeft(Vector2 mousePosition)
        {
            Vector2 movementTowardsLeft = new Vector2((mousePosition.X - border) / scrollSpeedReductionFactor, 0);
            return Vector2.Add(_camera.Position, movementTowardsLeft);
        }

        private Vector2 ScrollRight(Vector2 mousePosition)
        {
            Vector2 movementTowardsRight = new Vector2((mousePosition.X + border - Screen.Width) / scrollSpeedReductionFactor, 0);
            return Vector2.Add(_camera.Position, movementTowardsRight);
        }

        private Vector2 ScrollUp(Vector2 mousePosition)
        {
            Vector2 movementUpwards = new Vector2(0, (mousePosition.Y - border) / scrollSpeedReductionFactor);
            return Vector2.Add(_camera.Position, movementUpwards);
        }

        private Vector2 ScrollDown(Vector2 mousePosition)
        {
            Vector2 movementDownwards = new Vector2(0, (mousePosition.Y + border - Screen.Height) / scrollSpeedReductionFactor);
            return Vector2.Add(_camera.Position, movementDownwards);
        }
    }
}
