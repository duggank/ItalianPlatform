using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Tile_Engine;

namespace Gemstone_Hunter
{
    public class Mushroom : GameObject
    {
        #region MyCode
        private Vector2 fallSpeed = new Vector2(0, 20);
        private float walkSpeed = 60.0f;
        private bool facingLeft = true;

        public bool Dead;

        public Mushroom(ContentManager content, int cellX, int cellY)
        {

            animations.Add("run",
                new AnimationStrip(
                    content.Load<Texture2D>(
                        @"Textures\Sprites\Mushroom\mushroom"),
                    40,
                    "run",
                    0,
                    1));
            animations["run"].FrameLength = 0.4f;
            animations["run"].LoopAnimation = true;

            frameWidth = 40;
            frameHeight = 40;
            CollisionRectangle = new Rectangle(9, 1, 30, 40);
            worldLocation = new Vector2(
                cellX * TileMap.TileWidth,
                cellY * TileMap.TileHeight);

            enabled = true;

            codeBasedBlocks = true;
            PlayAnimation("run");
            

        }
        public override void Update(GameTime gameTime)
        {
            Vector2 oldLocation = worldLocation;

            if (!Dead)
            {
                velocity = new Vector2(0, velocity.Y);

                Vector2 direction = new Vector2(1, 0);

                if (facingLeft)
                {
                    direction = new Vector2(-1, 0);
                }

                direction *= walkSpeed;
                velocity += direction;
                velocity += fallSpeed;
            }
            base.Update(gameTime);

            if (!Dead)
            {
                if (oldLocation == worldLocation)
                {
                    facingLeft = !facingLeft;
                }
            }
            else
            {
                if (animations[currentAnimation].FinishedPlaying)
                {
                    enabled = false;
                }
            }
        }
        #endregion
    }
}
        