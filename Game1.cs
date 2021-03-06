﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wolf_Runner
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        public Core.Player Player;
        public Core.Enviroment Enviroment;
        public Core.GameManager GameManager;
        public Core.UI UI;
        public bool drawDebug = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            GameManager = new Core.GameManager(this);
            UI = new Core.UI(this);
            Player = new Core.Player(this, 50.0f, 6.5f, 3);
            Enviroment = new Core.Enviroment(this, (this.Window.ClientBounds.Height / 2)+ 150, 9.82f,60.0f);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) && (this.GameManager.GameState == 0 || this.GameManager.GameState == 2))
                Exit();

            // TODO: Add your update logic here

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && (this.GameManager.GameState == 0 || this.GameManager.GameState == 2))
            {
                if(this.GameManager.GameState == 0)
                {
                    this.GameManager.ChangeGameState(1);
                }
                else if (this.GameManager.GameState == 2)
                {
                    this.GameManager.GameRestart();
                }
            }

            GameManager.Tick(gameTime);

            if (this.GameManager.GameState == 1)
            {
                Player.Tick(gameTime);

                Enviroment.Tick(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Enviroment.CycleColor);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            Player.Draw(gameTime, drawDebug);

            Enviroment.Draw(gameTime, drawDebug);


            UI.Draw(gameTime, drawDebug);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
