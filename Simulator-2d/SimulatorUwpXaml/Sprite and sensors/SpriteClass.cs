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



        public float Angle
        {
            get;
            set;
        }
        public Vector2 Position { get; set; }

        public Vector2 Speed { get; set; }


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

        public void Update ()
        {
            Position = Position + Speed * elapsedTimeSinceLastUpdate;
            this.Angle += this.RateOfTurn * elapsedTimeSinceLastUpdate;
        }

        public void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, this.Angle, new Vector2(Texture.Width/2, Texture.Height/2), new Vector2(Scale, Scale), SpriteEffects.None, 0f);
        }
    }
}
