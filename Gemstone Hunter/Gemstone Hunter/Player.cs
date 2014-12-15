using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Tile_Engine;

namespace Gemstone_Hunter
{
    public class Player : GameObject
    {
        private Vector2 fallSpeed = new Vector2(0, 20);
        private float moveScale = 180.0f;
        private bool dead = false;
        private int score = 0;
        private int livesRemaining = 3;
        public bool BigMario = true;

        public bool Dead
        {
            get { return dead; }
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        public int LivesRemaining
        {
            get { return livesRemaining; }
            set { livesRemaining = value; }
        }


        #region Constructor
        public Player(ContentManager content)
        {
                animations.Add("idle",
                    new AnimationStrip(
                        content.Load<Texture2D>(@"Textures\Sprites\Player\Mario"),
                        30,
                        "idle",
                        139,
                        1));
                animations["idle"].LoopAnimation = true;

                animations.Add("run",
                    new AnimationStrip(
                        content.Load<Texture2D>(@"Textures\Sprites\Player\Mario"),
                        34,
                        "run",
                        0,
                        2));
                animations["run"].LoopAnimation = true;

                animations.Add("jump",
                    new AnimationStrip(
                        content.Load<Texture2D>(@"Textures\Sprites\Player\Mario"),
                        34,
                        "jump",
                        102,
                        1));
                float i = animations["jump"].FrameLength;

                animations["jump"].LoopAnimation = false;
                animations["jump"].FrameLength = 0.7f;
                animations["jump"].NextAnimation = "idle";

                animations.Add("die",
                    new AnimationStrip(
                        content.Load<Texture2D>(@"Textures\Sprites\Player\Mario"),
                        34,
                        "die",
                        171,
                        1));
                animations["die"].LoopAnimation = false;


                animations.Add("idleBig",
                    new AnimationStrip(
                        content.Load<Texture2D>(@"Textures\Sprites\Player\marioBig"),
                        32,
                        "idleBig",
                        128,
                        1));
                animations["idleBig"].LoopAnimation = true;

                animations.Add("runBig",
                    new AnimationStrip(
                        content.Load<Texture2D>(@"Textures\Sprites\Player\marioBig"),
                        32,
                        "run",
                        0,
                        3));
                animations["runBig"].LoopAnimation = true;

                animations.Add("jumpBig",
                    new AnimationStrip(
                        content.Load<Texture2D>(@"Textures\Sprites\Player\marioBig"),
                        32,
                        "jumpBig",
                        96,
                        1));
                float k = animations["jumpBig"].FrameLength;

                animations["jumpBig"].LoopAnimation = false;
                animations["jumpBig"].FrameLength = 0.5f;
                animations["jumpBig"].NextAnimation = "idleBig";

                frameWidth = 34;
                frameHeight = 32;
                CollisionRectangle = new Rectangle(9, 1, 28, 32);

                drawDepth = 0.825f;

                enabled = true;
                codeBasedBlocks = false;
                PlayAnimation("idle");

        }
        #endregion

        #region Public Methods
        public override void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.C))
                BigMario = !BigMario;

            frameWidth = 34;
            frameHeight = 32;
            CollisionRectangle = new Rectangle(9, 1, 28, 32);

            if (BigMario)
            {
                frameWidth = 34;
                frameHeight = 64;
                CollisionRectangle = new Rectangle(9, 1, 28, 64);
            }

            if (!Dead)
            {
                string newAnimation = "idle" + (BigMario ? "Big" : "");

                velocity = new Vector2(0, velocity.Y);
                GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
                KeyboardState keyState = Keyboard.GetState();

                if (keyState.IsKeyDown(Keys.Left) ||
                    (gamePad.ThumbSticks.Left.X < -0.3f))
                {
                    flipped = true;
                    newAnimation = "run" + (BigMario ? "Big" : "");
                    velocity = new Vector2(-moveScale, velocity.Y);
                }

                if (keyState.IsKeyDown(Keys.Right) ||
                    (gamePad.ThumbSticks.Left.X > 0.3f))
                {
                    flipped = false;
                    newAnimation = "run" + (BigMario ? "Big" : "");
                    velocity = new Vector2(moveScale, velocity.Y);
                }

                if (keyState.IsKeyDown(Keys.Space) ||
                    (gamePad.Buttons.A == ButtonState.Pressed))
                {
                    if (onGround)
                    {
                        Jump();
                        newAnimation = "jump" + (BigMario ? "Big" : "");
                    }
                }

                if (keyState.IsKeyDown(Keys.Up) ||
                    gamePad.ThumbSticks.Left.Y > 0.3f)
                {
                    checkLevelTransition();
                }

                
                //if (!onGround)
                //    newAnimation = "jump";


                if (currentAnimation == "jump" + (BigMario ? "Big" : ""))
                    newAnimation = "jump" + (BigMario ? "Big" : "");

                if (newAnimation != currentAnimation)
                {
                    PlayAnimation(newAnimation);
                }
            }

            velocity += fallSpeed;

            repositionCamera();

            base.Update(gameTime);
        }

        /*
        public override void Draw(SpriteBatch spriteBatch)
        {
            String temp = currentAnimation;

            if (BigMario)
                currentAnimation += "Big";

            base.Draw(spriteBatch);

            currentAnimation = temp;
        }
        */

        public void Jump()
        {
            velocity.Y = -500;
        }

        public void Kill()
        {
            if (BigMario)
            {
                BigMario = false;
            }
            else
            {
                PlayAnimation("die");
                LivesRemaining--;
                velocity.X = 0;
                dead = true;
            }
        }

        public void Revive()
        {
            PlayAnimation("idle");
            dead = false;
        }

        #endregion

        
        
        
        
        
        #region Helper Methods
        private void repositionCamera()
        {
            int screenLocX = (int)Camera.WorldToScreen(worldLocation).X;

            if (screenLocX > 500)
            {
                Camera.Move(new Vector2(screenLocX - 500, 0));
            }

            if (screenLocX < 200)
            {
                Camera.Move(new Vector2(screenLocX - 200, 0));
            }
        }

        private void checkLevelTransition()
        {
            Vector2 centerCell = TileMap.GetCellByPixel(WorldCenter);
            if (TileMap.CellCodeValue(centerCell).StartsWith("T_"))
            {
                string[] code = TileMap.CellCodeValue(centerCell).Split('_');

                if (code.Length != 4)
                    return;

                LevelManager.LoadLevel(int.Parse(code[1]));

                WorldLocation = new Vector2(
                    int.Parse(code[2]) * TileMap.TileWidth,
                    int.Parse(code[3]) * TileMap.TileHeight);

                LevelManager.RespawnLocation = WorldLocation;

                velocity = Vector2.Zero;
            }
        }

        #endregion


    }
}
