﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SimulatorUwpXaml
{
    public class SpriteClass
    {
        public SpriteClass (GraphicsDevice graphicsDevice, Texture2D spriteTexture, float textureScale)
        {
            this.TextureScale = textureScale;
            if (Texture == null)
            {
                Texture = spriteTexture;
            }
        }

        public Texture2D Texture { get; }

        public float TextureScale { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Speed { get; set; }

        public virtual float Angle { get; set; }

        public float RateOfTurn { get; set; }

        public void Update (float elapsedTimeSinceLastUpdate, bool noObstruction)
        {
            if (Picking2D.IsPickedUpForMove(this))
            {
                Position = Picking2D.MouseLocationInWorld();
            }
            else if (Picking2D.IsPickedUpForRotate(this))
            {
                Angle = Picking2D.MouseDirection(this);
            }
            else
            {
                if(noObstruction) Move(elapsedTimeSinceLastUpdate);
            }
        }

        protected virtual void Move(float elapsedTimeSinceLastUpdate)
        {
            Position = Position + Speed * elapsedTimeSinceLastUpdate;
            this.Angle += this.RateOfTurn * elapsedTimeSinceLastUpdate;
        }

        public void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, this.Angle, new Vector2(Texture.Width/2, Texture.Height/2), new Vector2(TextureScale, TextureScale), SpriteEffects.None, 0f);
        }
    }
}
