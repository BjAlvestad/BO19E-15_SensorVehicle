using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SimulatorUwpXaml
{
    class SpriteClass
    {
        public Texture2D texture
        {
            get;
        }

        public float PosX
        {
            get;
            set;
        }

        public float PosY
        {
            get;
            set;
        }

        public float Angle
        {
            get;
            set;
        }

        public float SpeedX
        {
            get;
            set;
        }

        public float SpeedY
        {
            get;
            set;
        }

        public float RateOfTurn
        {
            get;
            set;
        }

        public float Scale
        {
            get;
            set;
        }

        public SpriteClass (GraphicsDevice graphicsDevice, Texture2D spriteTexture, float scale)
        {
            this.Scale = scale;
            if (texture == null)
            {
                texture = spriteTexture;
            }
        }

        public void JumpToPosition(float newX, float newY)
        {
            PosX = newX;
            PosY = newY;
        }

        public void Update ()
        {
            this.PosX += this.SpeedX;
            this.PosY += this.SpeedY;
            this.Angle += this.RateOfTurn;
        }

        public void Draw (SpriteBatch spriteBatch)
        {
            Vector2 spritePosition = new Vector2(this.PosX, this.PosY);
            spriteBatch.Draw(texture, spritePosition, null, Color.White, this.Angle, new Vector2(texture.Width/2, texture.Height/2), new Vector2(Scale, Scale), SpriteEffects.None, 0f);
        }
    }
}
