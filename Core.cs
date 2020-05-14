using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wolf_Runner
{
    public class Core
    {
        public class Enviroment
        {
            public float GravityConstant { get; private set; }
            public float DayNightCycleTime { get; private set; }
            public float CurrentTimeOfDay { get; private set; }
            public bool DayNightCycleEnabled { get; set; }
            public bool IsDay { get; private set; }
            public int Floor { get; private set; }

            public Enviroment (int Floor, float GravityConstant, float DayNightCycleTime)
            {
                this.Floor = Floor;
                this.GravityConstant = GravityConstant;
                this.DayNightCycleTime = DayNightCycleTime;
                this.CurrentTimeOfDay = 0.0f;
                this.DayNightCycleEnabled = true;
                this.IsDay = true;
            }

            public Enviroment(int Floor, float GravityConstant)
            {
                this.Floor = Floor;
                this.GravityConstant = GravityConstant;
                this.DayNightCycleEnabled = false;
                this.IsDay = true;
            }

            public void Tick(Game1 game, GameTime gameTime)
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

            public Player(Game1 Game, float Weight, Vector2 AccelerationForce, int MaxHealth)
            {
                this.Game = Game;
                this.Position = new Vector2(this.Game.Window.ClientBounds.Width / 3, this.Game.Window.ClientBounds.Height / 2);

                AnimationTime = 0.5f;
                CurrentAnimationTime = 0;

                Sprites = new List<Texture2D>();
                isGrounded = true;

                Sprites.Add(this.Game.Content.Load<Texture2D>("Sprites/Wolf/Wolf_0"));
                Sprites.Add(this.Game.Content.Load<Texture2D>("Sprites/Wolf/Wolf_1"));
                Sprites.Add(this.Game.Content.Load<Texture2D>("Sprites/Wolf/Wolf_2"));
                Sprites.Add(this.Game.Content.Load<Texture2D>("Sprites/Wolf/Wolf_3"));
            }

            public void Tick(GameTime gameTime)
            {

            }

            public void Draw(GameTime gameTime)
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
