using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Wolf_Runner
{
    public class Core
    {
        public class GameManager
        {
            public Game1 Game { get; private set; }
            public float Score { get; private set; }
            public int HighScore { get; private set; }
            public int Runs { get; private set; }
            public int GameState { get; private set; }
            // 0 = NotStartedYet (Just opend the game)
            // 1 = Running (The game is running)
            // 2 = GameOver (The player has hit an Obstacle and triggerd GAMEOVER)
            
            public GameManager(Game1 Game)
            {
                this.Game = Game;
                this.Score = 0;
                this.HighScore = 0;
                this.Runs = 0;
                this.GameState = 0;
            }

            public void Tick(GameTime gameTime)
            {
                if (GameState == 1)
                {
                    this.Score += (float)(0.1 * this.Game.Enviroment.TimeMultiplier);
                }
            }

            public void ChangeGameState(int newGameState)
            {
                this.GameState = (newGameState);

                switch (this.GameState)
                {
                    case 0:
                        throw new Exception("ERROR: GameState reserved for startup!");
                    case 1:
                        this.Score = 0;
                        this.Runs++;
                        //RUN GAME
                        break;
                    case 2:
                        //GAMEOVER
                        if (this.Score > this.HighScore)
                        {
                            this.HighScore = (int)this.Score;
                            Console.WriteLine("New HighScore! : " + this.HighScore);
                        }
                        else
                        {
                            Console.WriteLine("Score : " + this.HighScore);
                        }
                        
                        break;
                    default:
                        throw new Exception("ERROR: Invalid GameState!");
                }
            }

            public void GameReset()
            {
                this.HighScore = 0;
                this.Runs = 0;
            }

            public void GameRestart()
            {
                this.Game.Player = new Core.Player(this.Game, 50.0f, 6.5f, 3);
                this.Game.Enviroment = new Core.Enviroment(this.Game, (this.Game.Window.ClientBounds.Height / 2) + 150, 9.82f, 40.0f);
                ChangeGameState(1);
            }
        }
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
            
            public Texture2D FloorTexture { get; private set; }
            public List<Vector2> FloorpiecePositions { get; private set; }
            public Texture2D FloorShelf { get; private set; }
            public Rectangle FloorShelfRectangle { get; private set; }
            public float TimeMultiplier { get; private set; }



            public Color CycleColor { get; private set; }
            public int ColorDay { get; private set; }
            public int ColorNight { get; private set; }
            public Texture2D MoonTexture { get; private set; }
            public Vector2 MoonPosition { get; private set; }
            public Texture2D SunTexture { get; private set; }
            public Vector2 SunPosition { get; private set; }


            public List<Obstacle> Obstacles { get; private set; }
            public List<Texture2D> ObstacleTextures { get; private set; }
            public List<Vector2> ObstacleTextureOffsets { get; private set; }
            public float MinObstacleCooldown { get; private set; }
            public float CurrentObstacleCooldown { get; private set; }


            public Random Random { get; private set; }

            public Enviroment (Game1 Game, int Floor, float GravityConstant, float DayNightCycleTime)
            {
                this.Game = Game;
                this.Floor = Floor;
                this.GravityConstant = GravityConstant;
                this.DayNightCycleTime = DayNightCycleTime;
                this.CurrentTimeOfDay = 0.0f;
                this.DayNightCycleEnabled = true;
                this.IsDay = true;

                this.ColorDay = 180;
                this.ColorNight = 20;
                this.CycleColor = new Color(ColorNight, ColorNight, ColorNight);


                this.MoonTexture = this.Game.Content.Load<Texture2D>("Enviroment/Moon_0");
                this.SunTexture = this.Game.Content.Load<Texture2D>("Enviroment/Sun_0");
                this.MoonPosition = new Vector2(-200, -200);
                this.SunPosition = new Vector2(-200, -200);


                this.FloorTexture = this.Game.Content.Load<Texture2D>("Sprites/Ground/ground_0");
                this.FloorpiecePositions = new List<Vector2>();
                this.TimeMultiplier = 1;

                this.Random = new Random();
                LoadObstacleSystem();
                FloorFirstLoad();

                // Create Collider Rectangle and Texture
                Vector2 Position = new Vector2(0.0f, this.Floor);
                Vector2 Size = new Vector2(this.Game.Window.ClientBounds.Width, 20);

                this.FloorCollider = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

                this.FloorColliderTexture = new Texture2D(this.Game.GraphicsDevice, (int)Size.X, (int)Size.Y);
                Color[] data = new Color[(int)Size.X * (int)Size.Y];
                for (int i = 0; i < data.Length; ++i) data[i] = new Color(0, 50, 250, 30);
                FloorColliderTexture.SetData(data);

                this.FloorShelfRectangle = new Rectangle(0, this.Floor, this.Game.Window.ClientBounds.Width, this.Game.Window.ClientBounds.Height - this.Floor);

                this.FloorShelf = new Texture2D(this.Game.GraphicsDevice, this.Game.Window.ClientBounds.Width, this.Game.Window.ClientBounds.Height - this.Floor);
                Color[] dataG = new Color[this.Game.Window.ClientBounds.Width * (this.Game.Window.ClientBounds.Height - this.Floor)];
                for (int i = 0; i < dataG.Length; ++i) dataG[i] = CycleColor;
                FloorShelf.SetData(dataG);
            }

            public Enviroment(Game1 Game, int Floor, float GravityConstant)
            {
                this.Game = Game;
                this.Floor = Floor;
                this.GravityConstant = GravityConstant;
                this.DayNightCycleEnabled = false;
                this.IsDay = true;

                this.ColorDay = 180;
                this.ColorNight = 20;
                this.CycleColor = new Color(ColorNight, ColorNight, ColorNight);

                this.FloorTexture = this.Game.Content.Load<Texture2D>("Sprites/Ground/ground_0");
                this.FloorpiecePositions = new List<Vector2>();
                this.TimeMultiplier = 1;

                this.Random = new Random();
                LoadObstacleSystem();
                FloorFirstLoad();

                // Create Collider Rectangle and Texture
                Vector2 Position = new Vector2(0.0f, this.Floor);
                Vector2 Size = new Vector2(this.Game.Window.ClientBounds.Width, 20);

                this.FloorCollider = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

                this.FloorColliderTexture = new Texture2D(this.Game.GraphicsDevice, (int)Size.X, (int)Size.Y);
                Color[] data = new Color[(int)Size.X * (int)Size.Y];
                for (int i = 0; i < data.Length; ++i) data[i] = new Color(0, 50, 250, 30);
                FloorColliderTexture.SetData(data);
            }


            public void LoadObstacleSystem()
            {

                this.MinObstacleCooldown = 3.5f;
                this.Obstacles = new List<Obstacle>();
                this.ObstacleTextures = new List<Texture2D>();
                this.ObstacleTextureOffsets = new List<Vector2>();

                this.ObstacleTextures.Add(this.Game.Content.Load<Texture2D>("Sprites/Obstacles/Stone_0"));
                this.ObstacleTextureOffsets.Add(new Vector2(18, 71));

                this.ObstacleTextures.Add(this.Game.Content.Load<Texture2D>("Sprites/Obstacles/Stone_1"));
                this.ObstacleTextureOffsets.Add(new Vector2(24, 96));

                this.ObstacleTextures.Add(this.Game.Content.Load<Texture2D>("Sprites/Obstacles/Stone_2"));
                this.ObstacleTextureOffsets.Add(new Vector2(58, 70));
            }

            public void FloorFirstLoad()
            {
                while (true)
                {
                    if (FloorpiecePositions.Count == 0)
                    {
                        FloorpiecePositions.Add(new Vector2(0, Floor - 2));
                    }
                    else
                    {
                        FloorpiecePositions.Add(new Vector2((this.FloorTexture.Width-2)*FloorpiecePositions.Count, Floor - 2));
                        if (FloorpiecePositions[FloorpiecePositions.Count - 1].X > this.Game.Window.ClientBounds.Width - this.FloorTexture.Width)
                        {
                            break;
                        }
                    }
                }
            }

            public void Tick(GameTime gameTime)
            {
                if (this.Game.GameManager.GameState == 1)
                {
                    if (this.DayNightCycleEnabled)
                    {

                        MoonPosition = new Vector2(this.Game.Window.ClientBounds.Width - MoonTexture.Width, ((this.Game.Window.ClientBounds.Height - 40) * (float)-Math.Sin(Math.PI * (CurrentTimeOfDay / (DayNightCycleTime/2)) + 0.5f * Math.PI)) + this.Game.Enviroment.Floor);
                        SunPosition = new Vector2(this.Game.Window.ClientBounds.Width - SunTexture.Width, ((this.Game.Window.ClientBounds.Height - 40) * (float)Math.Sin(Math.PI * (CurrentTimeOfDay / (DayNightCycleTime/2)) + 0.5f * Math.PI)) + this.Game.Enviroment.Floor);

                        this.CurrentTimeOfDay += (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (this.CurrentTimeOfDay > this.DayNightCycleTime)
                        {
                            this.CurrentTimeOfDay = 0.0f;
                        }


                        int ColorDelta = ColorDay - ColorNight;

                        if (this.CurrentTimeOfDay >= this.DayNightCycleTime / 2)
                        {
                            this.IsDay = false;

                            float colorEditMultiplier = 1 - (CurrentTimeOfDay / (DayNightCycleTime / 2)) % 1;
                            int newColorInt = Convert.ToInt32(ColorNight + (ColorDelta * colorEditMultiplier));


                            this.CycleColor = new Color(newColorInt, newColorInt, newColorInt);
                        }
                        else
                        {
                            this.IsDay = true;

                            float colorEditMultiplier = (CurrentTimeOfDay / (DayNightCycleTime / 2)) % 1;
                            int newColorInt = Convert.ToInt32(ColorNight + (ColorDelta * colorEditMultiplier));



                            this.CycleColor = new Color(newColorInt, newColorInt, newColorInt);
                        }
                    }

                    if (true)
                    {
                        this.TimeMultiplier += (float)(gameTime.ElapsedGameTime.TotalSeconds * 0.001f);
                        bool SpawnObstructed = false;
                        for (int i = 0; i < FloorpiecePositions.Count; i++)
                        {
                            FloorpiecePositions[i] = new Vector2(FloorpiecePositions[i].X - 10 * TimeMultiplier, FloorpiecePositions[i].Y);


                            //Console.WriteLine(FloorpiecePositions[i].X + ":" + FloorpiecePositions[i].Y);


                            if (FloorpiecePositions[i].X >= this.Game.Window.ClientBounds.Width - FloorTexture.Width + 10)
                            {
                                SpawnObstructed = true;
                                //Console.WriteLine("Spawn Obtructed!");
                                break;
                            }
                        }


                        if (!SpawnObstructed)
                        {
                            FloorpiecePositions.Add(new Vector2(this.Game.Window.ClientBounds.Width, Floor - 2));
                            //Console.WriteLine("Added Floor Tile!");
                        }

                        List<Vector2> RemovalList = new List<Vector2>();

                        foreach (var FloorPiecePos in FloorpiecePositions)
                        {
                            if (FloorPiecePos.X < 0 - FloorTexture.Width)
                            {
                                RemovalList.Add(FloorPiecePos);
                            }
                        }

                        foreach (var item in RemovalList)
                        {
                            FloorpiecePositions.Remove(item);
                            //Console.WriteLine("Removed Floor Tile!");
                        }
                    }

                    if (true)
                    {
                        this.CurrentObstacleCooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (CurrentObstacleCooldown > this.MinObstacleCooldown)
                        {
                            if (Random.Next(0, 10) == 0)
                            {
                                int nextObstacle = Random.Next(0, 3);
                                //int nextObstacle = 0;

                                this.Obstacles.Add(new Obstacle(this.Game, this.ObstacleTextures[nextObstacle], this.ObstacleTextureOffsets[nextObstacle]));
                                CurrentObstacleCooldown = 0;

                                this.MinObstacleCooldown = Random.Next(10, 30)/10;
                            }
                        }

                        List<Obstacle> RemovalList = new List<Obstacle>();
                        foreach (var obstacle in Obstacles)
                        {
                            obstacle.Tick(gameTime);

                            if (obstacle.Position.X < (0 - obstacle.Collider.Width))
                            {
                                RemovalList.Add(obstacle);
                            }
                        }

                        foreach (var obstacle in RemovalList)
                        {
                            Obstacles.Remove(obstacle);
                        }
                    }
                }
            }

            public void Draw(GameTime gameTime, bool DebugDraw)
            {
                this.FloorShelf = new Texture2D(this.Game.GraphicsDevice, this.Game.Window.ClientBounds.Width, this.Game.Window.ClientBounds.Height - this.Floor);
                Color[] dataG = new Color[this.Game.Window.ClientBounds.Width * (this.Game.Window.ClientBounds.Height - this.Floor)];
                for (int i = 0; i < dataG.Length; ++i) dataG[i] = CycleColor;
                FloorShelf.SetData(dataG);

                if (this.DayNightCycleEnabled)
                {
                    this.Game.spriteBatch.Draw(this.MoonTexture, this.MoonPosition, Color.White);
                    this.Game.spriteBatch.Draw(this.SunTexture, this.SunPosition, Color.White);
                }

                this.Game.spriteBatch.Draw(this.FloorShelf, FloorShelfRectangle, Color.White);

                for (int i = 0; i < FloorpiecePositions.Count; i++)
                {
                    this.Game.spriteBatch.Draw(this.FloorTexture, FloorpiecePositions[i], Color.White);
                }

                foreach (var obstacle in Obstacles)
                {
                    obstacle.Draw(gameTime, DebugDraw);
                }

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
            public Game1 Game { get; private set; }
            public Texture2D Sprite { get; private set; }
            public Vector2 Position { get; private set; }
            public Vector2 Offset { get; private set; }
            public Rectangle Collider { get; private set; }
            public Texture2D ColliderTexture { get; private set; }

            public Obstacle(Game1 Game, Texture2D Sprite, Vector2 SpriteOffset)
            {
                this.Game = Game;
                this.Sprite = Sprite;
                this.Position = new Vector2(this.Game.Window.ClientBounds.Width, this.Game.Enviroment.Floor - Sprite.Height);
                this.Offset = SpriteOffset;
                this.Collider = new Rectangle(
                    this.Game.Window.ClientBounds.Width + (int)(this.Offset.X / 2),
                    this.Game.Enviroment.Floor - Sprite.Height + (int)this.Offset.Y,
                    this.Sprite.Width - (int)this.Offset.X,
                    this.Sprite.Height - (int)this.Offset.Y);

                this.ColliderTexture = new Texture2D(this.Game.GraphicsDevice, this.Collider.Width, this.Collider.Height);
                Color[] dataG = new Color[this.Collider.Width * this.Collider.Height];
                for (int i = 0; i < dataG.Length; ++i) dataG[i] = new Color(0, 250, 20, 30); ;
                ColliderTexture.SetData(dataG);
            }

            public void Tick(GameTime gameTime)
            {
                this.Position = new Vector2(this.Position.X - 10 * this.Game.Enviroment.TimeMultiplier, this.Position.Y);
                this.Collider = new Rectangle(
                    (int)this.Position.X + (int)(this.Offset.X / 2),
                    this.Collider.Y,
                    this.Sprite.Width - (int)this.Offset.X,
                    this.Sprite.Height - (int)this.Offset.Y);
            }

            public void Draw(GameTime gameTime, bool DrawDebug)
            {
                this.Game.spriteBatch.Draw(this.Sprite, this.Position, Color.White);

                if (DrawDebug)
                {
                    this.Game.spriteBatch.Draw(this.ColliderTexture, this.Collider, Color.White);
                }
            }
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
                this.Position = new Vector2((this.Game.Window.ClientBounds.Width / 3) - 150, (this.Game.Window.ClientBounds.Height / 2) + 40);

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

                this.Collider = new Rectangle((int)(this.Position.X + 5), (int)(this.Position.Y + 32), 118, 58);

                this.ColliderTexture = new Texture2D(Game.GraphicsDevice, 120, 32);
                Color[] data = new Color[120 * 32];
                for (int i = 0; i < data.Length; ++i) data[i] = new Color(0, 50, 250, 30);
                this.ColliderTexture.SetData(data);


                this.GDCollider = new Rectangle((int)(this.Position.X + 5), (int)(this.Position.Y + 32 + 58), 118, 1);

                this.GDColliderTexture = new Texture2D(Game.GraphicsDevice, 120, 1);
                Color[] dataG = new Color[120 * 1];
                for (int i = 0; i < dataG.Length; ++i) dataG[i] = new Color(230, 50, 0, 30);
                this.GDColliderTexture.SetData(dataG);

                this.CurrenForce = 0;
            }

            public void Tick(GameTime gameTime)
            {
                if (this.Game.GameManager.GameState == 1)
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
                }
                else
                {
                    this.YAxis = 0;
                }

                if (!this.GDCollider.Intersects(this.Game.Enviroment.FloorCollider) && !this.Collider.Intersects(this.Game.Enviroment.FloorCollider))
                {
                    this.CurrenForce += this.Game.Enviroment.GravityConstant * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    isGrounded = false;
                    if (YAxis > 0)
                    {
                        this.CurrenForce += this.YAxis * this.JumpForce;
                    }
                }
                else
                {
                    isGrounded = true;
                    this.CurrenForce = 0;
                    if (YAxis < 0)
                    {
                        this.CurrenForce += this.YAxis * this.JumpForce; 
                    }
                }


                

                if (this.Game.GameManager.GameState == 1)
                {
                    if (this.Collider.Intersects(this.Game.Enviroment.FloorCollider))
                    {
                        Position = new Vector2(Position.X, this.Game.Enviroment.Floor - 32 - 58);
                    }
                    else
                    {
                        Position = new Vector2(Position.X, Position.Y + this.CurrenForce);
                    }
                }

                this.Collider = new Rectangle((int)(this.Position.X + 3), (int)(this.Position.Y + 32), 120, 58);
                this.GDCollider = new Rectangle((int)(this.Position.X + 3), (int)(this.Position.Y + 32 + 58), 120, 1);

                foreach (var obstacle in this.Game.Enviroment.Obstacles)
                {
                    if (this.Collider.Intersects(obstacle.Collider) && this.Game.GameManager.GameState == 1)
                    {
                        this.Game.GameManager.ChangeGameState(2);
                    }
                }
            }

            public void Draw(GameTime gameTime, bool DrawDebug)
            {
                if (this.isGrounded)
                {
                    if (this.Game.GameManager.GameState == 1)
                    {
                        CurrentAnimationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }

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

                if (DrawDebug)
                {
                    this.Game.spriteBatch.Draw(this.ColliderTexture, this.Collider, Color.White);
                    this.Game.spriteBatch.Draw(this.GDColliderTexture, this.GDCollider, Color.White);
                }
            }
        }
        
        public class UI
        {
            public Game1 Game { get; private set; }
            public SpriteFont PrimaryFont { get; private set; }
            public SpriteFont SecondaryFont { get; private set; }
            private int OldHighScore { get; set; }

            public UI(Game1 Game)
            {
                this.Game = Game;
                this.PrimaryFont = this.Game.Content.Load<SpriteFont>("Fonts/Standard");
            }

            public void Draw(GameTime gameTime, bool drawDebug)
            {
                if (this.Game.GameManager.GameState == 0)
                {
                    /*
                     * Display Welcome Message!
                     * Basic Instructions.
                     */

                    this.Game.spriteBatch.DrawString(this.PrimaryFont,"Wolf Runner",new Vector2(20, 0),Color.White,0,new Vector2(0,0),3,SpriteEffects.None, 1);
                    this.Game.spriteBatch.DrawString(this.PrimaryFont, "Use W and S to jump and fall.", new Vector2(20, 90), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                    this.Game.spriteBatch.DrawString(this.PrimaryFont, "Press Space to start!", new Vector2(20, 90 + 18), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                    this.Game.spriteBatch.DrawString(this.PrimaryFont, "Press Exit to exit game now.", new Vector2(20, 90 + 18 + 18), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                }
                else if (this.Game.GameManager.GameState == 1)
                {
                    /*
                     * Show current score, highscore and run
                     */

                    this.Game.spriteBatch.DrawString(this.PrimaryFont, "Score: " + (int)this.Game.GameManager.Score, new Vector2(20,  this.Game.Window.ClientBounds.Height - 70), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                    this.Game.spriteBatch.DrawString(this.PrimaryFont, "Highscore: " + this.Game.GameManager.HighScore, new Vector2(20, this.Game.Window.ClientBounds.Height - 70 + 18), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                    this.Game.spriteBatch.DrawString(this.PrimaryFont, "Run: " + this.Game.GameManager.Runs, new Vector2(20, this.Game.Window.ClientBounds.Height - 70 + 18 + 18), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                    this.OldHighScore = this.Game.GameManager.HighScore;
                }
                else if (this.Game.GameManager.GameState == 2)
                {
                    /*
                     * Show score for this run.
                     * Current/New highscore.
                     * Current amount of runs.
                     */

                    if ( this.Game.GameManager.HighScore > this.OldHighScore)
                    {
                        /*
                         * NEW HIGH SCORE!
                         */
                        this.Game.spriteBatch.DrawString(this.PrimaryFont, "NEW HIGHSCORE!", new Vector2(20, 0), Color.White, 0, new Vector2(0, 0), 2, SpriteEffects.None, 1);
                        this.Game.spriteBatch.DrawString(this.PrimaryFont, "Highscore: " + this.Game.GameManager.HighScore.ToString(), new Vector2(20, 40 + 22), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                        this.Game.spriteBatch.DrawString(this.PrimaryFont, "Run: " + this.Game.GameManager.Runs, new Vector2(20, 40 + 22 + 22), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                    }
                    else if (this.Game.GameManager.HighScore - this.Game.GameManager.Score < 10)
                    {
                        /*
                         * So Close!!
                         */
                        this.Game.spriteBatch.DrawString(this.PrimaryFont, "So close!", new Vector2(20, 0), Color.White, 0, new Vector2(0, 0), 2, SpriteEffects.None, 1);
                        this.Game.spriteBatch.DrawString(this.PrimaryFont, "Score: " + ((int)this.Game.GameManager.Score).ToString(), new Vector2(20, 40), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                        this.Game.spriteBatch.DrawString(this.PrimaryFont, "Highscore: " + this.Game.GameManager.HighScore.ToString(), new Vector2(20, 40 + 22), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                        this.Game.spriteBatch.DrawString(this.PrimaryFont, "Run: " + this.Game.GameManager.Runs, new Vector2(20, 40 + 22 + 22), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                    }
                    else
                    {
                        /*
                         * Current Run Compleated.
                         * Score.
                         */
                        this.Game.spriteBatch.DrawString(this.PrimaryFont, "Keep it going.", new Vector2(20, 0), Color.White, 0, new Vector2(0, 0), 2, SpriteEffects.None, 1);
                        this.Game.spriteBatch.DrawString(this.PrimaryFont, "Score: " + ((int)this.Game.GameManager.Score).ToString(), new Vector2(20, 40), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                        this.Game.spriteBatch.DrawString(this.PrimaryFont, "Highscore: "+ this.Game.GameManager.HighScore.ToString(), new Vector2(20, 40 + 22), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                        this.Game.spriteBatch.DrawString(this.PrimaryFont, "Run: " + this.Game.GameManager.Runs, new Vector2(20, 40 + 22 + 22), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                    }

                    /*
                     * Do Run and instruction part here.
                     */
                    
                    this.Game.spriteBatch.DrawString(this.PrimaryFont, "Press Space to start again!", new Vector2(20, 150), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                    this.Game.spriteBatch.DrawString(this.PrimaryFont, "Press Exit to exit game.", new Vector2(20, 150 + 18), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                }
                else
                {
                    //I don't know.
                }
            }
        }
    }
}
