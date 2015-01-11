

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace dodel
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public struct dodel
        {
            public Rectangle safe;
            public Texture2D txe;
            public Vector2 speed;
            public void move()
            {
                safe.X += (int)speed.X;
                safe.Y += (int)speed.Y;
            }
            public void draw(SpriteBatch s)
            {
                // error
                // s.Draw(txe, safe, new Rectangle(0, 0, 50, 50), Color.Red, 0F, new Vector2(25, 25), 0, 0F);
                s.Draw(txe, safe, null, Color.Red, 0f, new Vector2(0f, 0f), 0, 0f);
            }
        }
        public struct rectnorm
        {
            public Rectangle safe;
            public Texture2D txe;
            public void draw(SpriteBatch s)
            {
                s.Draw(txe, safe, Color.White);
            }
        }
        public dodel mainball;
        public rectnorm r1;
        public rectnorm r2;
        public rectnorm r3;
        public rectnorm r4;
        public List<rectnorm> rect = new List<rectnorm>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            mainball.safe = new Rectangle(25, 25, 50, 50);
            mainball.speed = new Vector2(10f, 10f);
            r1.safe = new Rectangle(0, 0, 20, 200);
            r2.safe = new Rectangle(10, 50, 20, 200);
            r3.safe = new Rectangle(0, 100, 20, 200);
            r4.safe = new Rectangle(10, 0, 20, 200);
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
            mainball.txe = Content.Load<Texture2D>("dodel");
            r1.txe = Content.Load<Texture2D>("rectnorm");
            r2.txe = Content.Load<Texture2D>("rectnorm");
            r3.txe = Content.Load<Texture2D>("rectnorm");
            r4.txe = Content.Load<Texture2D>("rectnorm");

            rect.Add(r1);
            rect.Add(r2);
            rect.Add(r3);
            rect.Add(r4);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // error
            spriteBatch.Begin();
            mainball.draw(spriteBatch);
            r1.draw(spriteBatch);
            r2.draw(spriteBatch);
            r3.draw(spriteBatch);
            r4.draw(spriteBatch);
            // error : list is not array!
            /*
            for (int i = 0; i < rect.Count; i++)
            {
                rect[i].draw(spriteBatch);
            }
            */
            spriteBatch.End();
            

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }

}

