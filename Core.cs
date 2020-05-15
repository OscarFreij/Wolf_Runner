using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Wolf_Runner
{
    public class Core
    {
        public class Enviroment
        {
            public Game1 Game { get; private set; }
            public float GravityConstant { get; private set; }
            public float DayNightCycleTime { get; private set; }
            public float CurrentTimeOfDay { get; private set; }
            public bool DayNightCycleEnabled { get; set; }
            public bool IsDay { get; private set; }
            public int Floor { get; private set; }
            public Rectangle FloorCollider { get; private set; }
            public Texture2D FloorColliderTexture { get; private set; }

            public Enviroment (Game1 Game, int Floor, float GravityConstant, float DayNightCycleTime)
            {
                this.Game = Game;
                this.Floor = Floor;
                this.GravityConstant = GravityConstant;
                this.DayNightCycleTime = DayNightCycleTime;
                this.CurrentTimeOfDay = 0.0f;
                this.DayNightCycleEnabled = true;
                this.IsDay = true;


                // Create Collider Rectangle and Texture
                Vector2 Position = new Vector2(0.0f, this.Floor);
                Vector2 Size = new Vector2(this.Game.Window.ClientBounds.Width, 20);

                this.FloorCollider = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

                this.FloorColliderTexture = new Texture2D(this.Game.GraphicsDevice, (int)Size.X, (int)Size.Y);
                Color[] data = new Color[(int)Size.X * (int)Size.Y];
                for (int i = 0; i < data.Length; ++i) data[i] = new Color(0, 50, 250, 230);
                FloorColliderTexture.SetData(data);
            }

            public Enviroment(Game1 Game, int Floor, float GravityConstant)
            {
                this.Game = Game;
                this.Floor = Floor;
                this.GravityConstant = GravityConstant;
                this.DayNightCycleEnabled = false;
                this.IsDay = true;


                // Create Collider Rectangle and Texture
                Vector2 Position = new Vector2(0.0f, this.Floor);
                Vector2 Size = new Vector2(this.Game.Window.ClientBounds.Width, 20);

                this.FloorCollider = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

                this.FloorColliderTexture = new Texture2D(this.Game.GraphicsDevice, (int)Size.X, (int)Size.Y);
                Color[] data = new Color[(int)Size.X * (int)Size.Y];
                for (int i = 0; i < data.Length; ++i) data[i] = new Color(0, 50, 250, 230);
                FloorColliderTexture.SetData(data);
            }

            public void Tick(GameTime gameTime)
            {
                if (this.DayNightCycleEnabled)
                {
                    this.CurrentTimeOfDay += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (this.CurrentTimeOfDay > this.DayNightCycleTime)
                    {
                        this.CurrentTimeOfDay = 0.0f;
                    }

                    if (this.CurrentTimeOfDay >= this.DayNightCycleTime/2)
                    {
                        this.IsDay = false;
                    }
                    else
                    {
                        this.IsDay = true;
                    }
                }
            }

            public void Draw(GameTime gameTime, bool DebugDraw)
            {
                if (DebugDraw)
                {
                    this.Game.spriteBatch.Draw(this.FloorColliderTexture, this.FloorCollider, Color.White);
                }

            }


            public void SetTime(float NewTime)
            {
                if (NewTime > 1 || NewTime < 0)
                {
                    throw new Exception("New time value is not betweene 0 and 1");
                }
                else
                {
                    this.CurrentTimeOfDay = this.DayNightCycleTime * NewTime;
                }
            }

        }
        
        public class Obstacle
        {

        }

        public class Player
        {
            public Game1 Game { get; private set; }
            public List<Texture2D> Sprites { get; private set; }
            public Vector2 Position { get; private set; }
            public Vector2 AccelerationForce { get; private set; }
            public float CurrentSpeed { get; private set; }
            public float Weight { get; private set; }
            public int MaxHealth { get; private set; }
            public int CurrentHealth { get; private set; }
            private float AnimationTime { get; set; }
            private float CurrentAnimationTime { get; set; }
            public bool isGrounded { get; private set; }
            public Rectangle Collider { get; private set; }
            public Texture2D ColliderTexture { get; private set; }

            public Player(Game1 Game, float Weight, Vector2 AccelerationForce, int MaxHealth)
            {
                this.Game = Game;
                this.Position = new Vector2((this.Game.Window.ClientBounds.Width / 3) - 150, this.Game.Window.ClientBounds.Height / 2);

                AnimationTime = 0.5f;
                CurrentAnimationTime = 0;

                Sprites = new List<Texture2D>();
                isGrounded = true;

                Sprites.Add(this.Game.Content.Load<Texture2D>("Sprites/Wolf/Wolf_0"));
                Sprites.Add(this.Game.Content.Load<Texture2D>("Sprites/Wolf/Wolf_1"));
                Sprites.Add(this.Game.Content.Load<Texture2D>("Sprites/Wolf/Wolf_2"));
                Sprites.Add(this.Game.Content.Load<Texture2D>("Sprites/Wolf/Wolf_3"));

                this.Collider = new Rectangle((int)(this.Position.X+3), (int)(this.Position.Y+32), 120, 62);

                this.ColliderTexture = new Texture2D(Game.GraphicsDevice, 120, 32);
                Color[] data = new Color[120 * 32];
                for (int i = 0; i < data.Length; ++i) data[i] = new Color(0, 50, 250, 230);
                ColliderTexture.SetData(data);
            }

            public void Tick(GameTime gameTime)
            {



            }

            public void Draw(GameTime gameTime, bool DrawDebug)
            {
                if (this.isGrounded)
                {
                    CurrentAnimationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (CurrentAnimationTime > AnimationTime)
                    {
                        CurrentAnimationTime = 0.0f;
                    }

                    if (CurrentAnimationTime <= AnimationTime * 0.25)
                    {
                        if (DrawDebug)
                        {
                            this.Game.spriteBatch.Draw(this.ColliderTexture, this.Collider, Color.White);
                        }

                        this.Game.spriteBatch.Draw(Sprites[0], this.Position, Color.White);
                    }
                    else if (CurrentAnimationTime <= AnimationTime * 0.50)
                    {
                        if (DrawDebug)
                        {
                            this.Game.spriteBatch.Draw(this.ColliderTexture, this.Collider, Color.White);
                        }

                        this.Game.spriteBatch.Draw(Sprites[1], this.Position, Color.White);
                    }
                    else if (CurrentAnimationTime <= AnimationTime * 0.75)
                    {
                        if (DrawDebug)
                        {
                            this.Game.spriteBatch.Draw(this.ColliderTexture, this.Collider, Color.White); ;
                        }

                        this.Game.spriteBatch.Draw(Sprites[2], this.Position, Color.White);
                    }
                    else if (CurrentAnimationTime <= AnimationTime * 1)
                    {
                        if (DrawDebug)
                        {
                            this.Game.spriteBatch.Draw(this.ColliderTexture, this.Collider, Color.White);
                        }

                        this.Game.spriteBatch.Draw(Sprites[3], this.Position, Color.White);
                    }
                    else
                    {
                        throw new Exception("Wolf Animation Timer Outside Bounds!");
                    }
                }
                else
                {
                    if (DrawDebug)
                    {
                        this.Game.spriteBatch.Draw(this.ColliderTexture, this.Collider, Color.White);
                    }
                    CurrentAnimationTime = AnimationTime * 0.51f;
                    this.Game.spriteBatch.Draw(Sprites[1], this.Position, Color.White);
                }
            }
        }
        
        public class UI
        {

        }
    }
}
