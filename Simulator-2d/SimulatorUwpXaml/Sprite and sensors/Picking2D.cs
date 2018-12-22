using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SimulatorUwpXaml
{
    static class Picking2D
    {
        private static bool _spritePickedUpForMove;
        private static bool _spritePickedUpForRotate;

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
            return (MouseLocation() - sprite.Position).Length() < sprite.Texture.Height * sprite.Scale;
        }

        public static Vector2 MouseLocation()
        {
            Vector2 mouseLocation = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            return mouseLocation;
        }

        public static float MouseDirection(SpriteClass sprite)
        {
            Vector2 mouseLocation = MouseLocation();
            Vector2 direction = mouseLocation - sprite.Position;

            return (float)Math.Atan2(direction.Y, direction.X);
        }
    }
}


