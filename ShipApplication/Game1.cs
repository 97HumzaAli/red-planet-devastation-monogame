using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace ShipApplication
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D shipSprite;
        Texture2D redPlanetSprite;
        Texture2D boom;
        Explosion explosion;
        SpriteFont gameFont;
        double shipScale = 0.05f;
        float rotation;
        const float planetRadius = (float)(0.12 * 600) / 2;
        Vector2 view; 
        Vector2 planetPosition = new Vector2(300, 300);
        Vector2 shipPosition;
        Vector2 mousePosition;
        Vector2 speed = new Vector2(1, 1);
        Vector2 scorePosition;
        Vector2 messagePosition;
        Rectangle rectangle;
        double elapseTime;
        double gameCountdown = 60;
        double randomTimer = 0;
        int score;
        string output = "";
        MouseState mouse;
        public Texture2D background;
        public Rectangle srcRect;
        public Vector2 position1, position2;
        public Vector2 speedbg, speedbg2;
        SoundEffect explode;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //ParallaxBG
            Vector2 stage = new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            background = Content.Load<Texture2D>("2ndBackground");
            srcRect = new Rectangle(2000, 0, background.Width-2000, background.Height); ;
            position1 = new Vector2(stage.X- srcRect.Width, 0);
            speedbg = new Vector2(0, 20);
            position2 = new Vector2(0, stage.X - srcRect.Width - 50);
            speedbg2 = new Vector2(0,10);
            //GameSprites
            shipSprite = Content.Load<Texture2D>("Ship");
            shipPosition = new Vector2(view.X / 2, view.Y / 2);
            rectangle = new Rectangle(0, 0, shipSprite.Width, shipSprite.Height);

            redPlanetSprite = Content.Load<Texture2D>("redplanet");
            gameFont = Content.Load<SpriteFont>("gameFont");

            //Explosion stuff
            boom = Content.Load<Texture2D>("explosion");
            explosion = new Explosion(this,_spriteBatch,boom,Vector2.Zero, 3);
            this.Components.Add(explosion);

            //SoundFX
            explode = Content.Load<SoundEffect>("explode");

            view = new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            //Text
            scorePosition = new Vector2(5, _graphics.PreferredBackBufferHeight - 35);
            messagePosition = new Vector2((_graphics.PreferredBackBufferWidth / 2) - 150, (_graphics.PreferredBackBufferHeight / 2) + 50);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            elapseTime = gameTime.ElapsedGameTime.TotalSeconds;
            randomTimer += elapseTime;
            gameCountdown -= elapseTime;
            mouse = Mouse.GetState();

            position1 -= speedbg;
            position2 -= speedbg2;

            if (position1.Y < -srcRect.Height)
            {
                position1.Y = position2.Y + srcRect.Height;
            }

            if (position2.Y < -srcRect.Height)
            {
                position2.Y = position1.Y + srcRect.Height;
            }

            // TODO: Add your update logic here
            if (gameCountdown >= 0)
            {
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    mousePosition = new Vector2(mouse.X, mouse.Y);
                    float xAxisDiff = mousePosition.X - (shipPosition.X);
                    float yAxisDiff = mousePosition.Y - (shipPosition.Y);
                    rotation = (float)Math.Atan2(xAxisDiff, -yAxisDiff);
                    shipPosition.X += xAxisDiff * speed.X * 0.10f;
                    shipPosition.Y += yAxisDiff * speed.Y * 0.10f;
                }

                float shipPlanetDist = Vector2.Distance(planetPosition, shipPosition);
                if (shipPlanetDist<planetRadius) 
                {
                    Vector2 position = new Vector2(planetPosition.X-planetRadius, planetPosition.Y-planetRadius);
                    explosion = new Explosion(this,_spriteBatch, boom, position, 3);
                    Components.Add(explosion);
                    explosion.Position= position;
                    explosion.restart();
                    explode.Play();
                    score+=10;
                    shipScale+=0.001;
                    Random r = new Random();
                    planetPosition.X = r.Next(0, _graphics.PreferredBackBufferWidth);
                    planetPosition.Y = r.Next(0, _graphics.PreferredBackBufferHeight);
                    randomTimer = 0;
                }
                if (randomTimer >= 0.5)
                {
                    Random r = new Random();
                    planetPosition.X = r.Next(0, _graphics.PreferredBackBufferWidth);
                    planetPosition.Y = r.Next(0, _graphics.PreferredBackBufferHeight);
                    randomTimer = 0;
                }

            }
            else
            {
                gameCountdown = 0;
                randomTimer = 0;
                shipPosition.X = view.X/2;
                shipPosition.Y = view.Y/2;
                scorePosition.X = (_graphics.PreferredBackBufferWidth/2)-100;
                scorePosition.Y = _graphics.PreferredBackBufferHeight/2;

                
                if (score <= 70)
                {
                    output = "Better try next time";
                }
                if(score > 70)
                {
                    output = "Winner.";

                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(SpriteSortMode.Deferred);
            _spriteBatch.Draw(background, position1, srcRect, Color.White);
            _spriteBatch.Draw(background, position2, srcRect, Color.White);
            _spriteBatch.Draw(redPlanetSprite, new Vector2(planetPosition.X - planetRadius, planetPosition.Y - planetRadius), null, Color.White, 0f, Vector2.Zero, 0.12f, SpriteEffects.None, 0f);
            _spriteBatch.Draw(shipSprite, new Vector2(shipPosition.X, shipPosition.Y), rectangle, Color.White, rotation, new Vector2(shipSprite.Width/2, 0), (float)shipScale, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(gameFont, "TIMER: "+ Math.Round(gameCountdown, 1).ToString(), new Vector2(5, 0), Color.Red);
            _spriteBatch.DrawString(gameFont, "SCORE: "+ score, scorePosition, Color.White);
            _spriteBatch.DrawString(gameFont, output, messagePosition, Color.White);
            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}