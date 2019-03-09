using System;
using System.Diagnostics;
using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SimulatorUwpXaml
{
    class Picking2D
    {
        private static bool _spritePickedUpForMove;
        private static bool _spritePickedUpForRotate;
        private static Camera _camera;

        public Picking2D(Camera camera)
        {
            _camera = camera;
        }

        public static bool IsPickedUp(SpriteClass sprite)
        {
            return IsPickedUpForMove(sprite) || IsPickedUpForRotate(sprite);
        }

        public static bool IsPickedUpForMove(SpriteClass sprite)
        {
            MouseState mouseState = Mouse.GetState();

            if (IsMouseIntersectingSprite(sprite) && mouseState.LeftButton == ButtonState.Pressed)
            {
                _spritePickedUpForMove = true;
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                _spritePickedUpForMove = false;
            }

            return _spritePickedUpForMove;
        }        
        
        public static bool IsPickedUpForRotate(SpriteClass sprite)
        {
            MouseState mouseState = Mouse.GetState();

            if (IsMouseIntersectingSprite(sprite) && mouseState.RightButton == ButtonState.Pressed)
            {
                _spritePickedUpForRotate = true;
            }
            else if (mouseState.RightButton == ButtonState.Released)
            {
                _spritePickedUpForRotate = false;
            }

            return _spritePickedUpForRotate;
        }

        public static bool IsMouseIntersectingSprite(SpriteClass sprite)   // TEMP: Public for debugging purposes. Can be set to private once program is complete.
        {
            return (MouseLocationInWorld() - sprite.Position).Length() < sprite.Texture.Height * sprite.Scale;
        }

        public static Vector2 MouseLocation()
        {
            Vector2 mouseLocation = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            return mouseLocation;
        }

        public static Vector2 MouseLocationInWorld()
        {
            Vector2 mousePosRelativeScreenCenterOrigo = MouseLocation() - new Vector2(Screen.Width/2, Screen.Height/2);
            Vector2 mouseInWorld = Vector2.Zero;
            _camera.ToWorld(ref mousePosRelativeScreenCenterOrigo, out mouseInWorld);

            return mouseInWorld;
        }

        public static float MouseDirection(SpriteClass sprite)
        {
            Vector2 mouseLocation = MouseLocationInWorld();
            Vector2 direction = mouseLocation - sprite.Position;

            return (float)Math.Atan2(direction.Y, direction.X);
        }

        public void DebugMousePosition(SpriteClass sprite)
        {
            Vector2 spriteOriginalPos = sprite.Position;
            Vector2 spriteScreenPos = Vector2.Zero;
            _camera.ToScreen(ref spriteOriginalPos, out spriteScreenPos);

            Vector2 cameraOriginalPos = _camera.Position;
            Vector2 cameraScreenPos = Vector2.Zero;
            _camera.ToScreen(ref cameraOriginalPos, out cameraScreenPos);

            Vector2 mouseOriginalScreenPos = MouseLocation();
            Vector2 mouseInWorld = Vector2.Zero;
            _camera.ToWorld(ref mouseOriginalScreenPos, out mouseInWorld);

            Debug.WriteLine($"Checked if mouse intersects sprite. Mouse screen: {MouseLocation()}, Mouse world: {MouseLocationInWorld()}. Sprite position: {sprite.Position}. SpriteScreenPos: {spriteScreenPos} Camera pos: {_camera.Position}, Camera screenPos {cameraScreenPos}. Screen width: {Screen.Width}, Height: {Screen.Height}");
        }
    }
}


