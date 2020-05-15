using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
            public float JumpForce { get; private set; }
            public float CurrentSpeed { get; private set; }
            public float Weight { get; private set; }
            public int MaxHealth { get; private set; }
            public int CurrentHealth { get; private set; }
            private float AnimationTime { get; set; }
            private float CurrentAnimationTime { get; set; }
            public bool isGrounded { get; private set; }
            public Rectangle Collider { get; private set; }
            public Texture2D ColliderTexture { get; private set; }
            public Rectangle GDCollider { get; private set; }
            public Texture2D GDColliderTexture { get; private set; }


            public float YAxis { get; private set; }
            public float CurrenForce { get; private set; }

            public Player(Game1 Game, float Weight, float JumpForce, int MaxHealth)
            {
                this.Game = Game;
                this.Position = new Vector2((this.Game.Window.ClientBounds.Width / 3) - 150, this.Game.Window.ClientBounds.Height / 2);

                this.Weight = Weight;
                this.JumpForce = JumpForce;
                this.MaxHealth = MaxHealth;
                this.CurrentHealth = this.MaxHealth;

                AnimationTime = 0.5f;
                CurrentAnimationTime = 0;

                Sprites = new List<Texture2D>();
                isGrounded = true;

                Sprites.Add(this.Game.Content.Load<Texture2D>("Sprites/Wolf/Wolf_0"));
                Sprites.Add(this.Game.Content.Load<Texture2D>("Sprites/Wolf/Wolf_1"));
                Sprites.Add(this.Game.Content.Load<Texture2D>("Sprites/Wolf/Wolf_2"));
                Sprites.Add(this.Game.Content.Load<Texture2D>("Sprites/Wolf/Wolf_3"));

                this.Collider = new Rectangle((int)(this.Position.X+3), (int)(this.Position.Y+32), 120, 58);

                this.ColliderTexture = new Texture2D(Game.GraphicsDevice, 120, 32);
                Color[] data = new Color[120 * 32];
                for (int i = 0; i < data.Length; ++i) data[i] = new Color(0, 50, 250, 230);
                this.ColliderTexture.SetData(data);


                this.GDCollider = new Rectangle((int)(this.Position.X + 3), (int)(this.Position.Y + 32 + 58), 120, 5);

                this.GDColliderTexture = new Texture2D(Game.GraphicsDevice, 120, 5);
                Color[] dataG = new Color[120 * 5];
                for (int i = 0; i < dataG.Length; ++i) dataG[i] = new Color(230, 50, 0, 230);
                this.GDColliderTexture.SetData(dataG);

                this.CurrenForce = 0;
            }

            public void Tick(GameTime gameTime)
            {
                if ((Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down)) && !(Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Up)))
                {
                    this.YAxis = 1;
                }
                else if (!(Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down)) && (Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Up)))
                {
                    this.YAxis = -1;                
                }
                else
                {
                    this.YAxis = 0;
                }

                

                if (!this.GDCollider.Intersects(this.Game.Enviroment.FloorCollider) && !this.Collider.Intersects(this.Game.Enviroment.FloorCollider))
                {
                    this.CurrenForce += this.Game.Enviroment.GravityConstant * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (YAxis > 0)
                    {
                        this.CurrenForce += this.YAxis * this.JumpForce;
                    }
                }
                else
                {
                    this.CurrenForce = 0;
                    if (YAxis < 0)
                    {
                        this.CurrenForce += this.YAxis * this.JumpForce;
                    }
                }


                


                if (this.Collider.Intersects(this.Game.Enviroment.FloorCollider))
                {
                    Position = new Vector2(Position.X, this.Game.Enviroment.Floor - 32 - 58);
                }
                else
                {
                    Position = new Vector2(Position.X, Position.Y + this.CurrenForce);
                }

                this.Collider = new Rectangle((int)(this.Position.X + 3), (int)(this.Position.Y + 32), 120, 58);
                this.GDCollider = new Rectangle((int)(this.Position.X + 3), (int)(this.Position.Y + 32 + 58), 120, 5);
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

                    if (DrawDebug)
                    {
                        this.Game.spriteBatch.Draw(this.ColliderTexture, this.Collider, Color.White);
                        this.Game.spriteBatch.Draw(this.GDColliderTexture, this.GDCollider, Color.White);
                    }


                    if (CurrentAnimationTime <= AnimationTime * 0.25)
                    {
                        this.Game.spriteBatch.Draw(Sprites[0], this.Position, Color.White);
                    }
                    else if (CurrentAnimationTime <= AnimationTime * 0.50)
                    {
                        this.Game.spriteBatch.Draw(Sprites[1], this.Position, Color.White);
                    }
                    else if (CurrentAnimationTime <= AnimationTime * 0.75)
                    {
                        this.Game.spriteBatch.Draw(Sprites[2], this.Position, Color.White);
                    }
                    else if (CurrentAnimationTime <= AnimationTime * 1)
                    {
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
