using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication
{
    public class Explosion : DrawableGameComponent
    {
        private SpriteBatch _spriteBatch;
        private Texture2D boom;
        private Vector2 boomPosition;
        private Vector2 boomDimension;
        private List<Rectangle> frames;
        private int frameIndex = -1;

        private int delay;
        private int delayCounter;

        private const int ROW = 5;
        private const int COL = 5;

        public Vector2 Position { get => boomPosition; set => boomPosition = value; }

        public Explosion(Game game, SpriteBatch spriteBatch,
            Texture2D boom,
            Vector2 position,
            int delay) : base(game)
        {
            this._spriteBatch = spriteBatch;
            this.boom = boom;
            this.boomPosition = position;
            this.delay = delay;

            this.boomDimension = new Vector2(boom.Width / COL, boom.Height / ROW);
            hide();
            createFrames();
        }

        public void hide()
        {
            this.Enabled = false;
            this.Visible = false;
        }

        public void restart()
        {
            frameIndex = -1;
            delayCounter = 0;
            this.Enabled = true;
            this.Visible = true;
        }

        private void createFrames()
        {
            frames = new List<Rectangle>();
            for (int i = 0; i < ROW; i++)
            {
                for (int j = 0; j < COL; j++)
                {
                    int x = j * (int)boomDimension.X;
                    int y = i * (int)boomDimension.Y;
                    Rectangle r = new Rectangle(x, y, (int)boomDimension.X, (int)boomDimension.Y);
                    frames.Add(r);
                }
            }

        }


        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred);
            if (frameIndex >= 0)
            {
                //v 4
                _spriteBatch.Draw(boom, boomPosition, frames[frameIndex], Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            delayCounter++;
            if (delayCounter > delay)
            {
                frameIndex++;
                if (frameIndex > ROW * COL - 1)
                {
                    frameIndex = -1;
                    hide();

                }
                delayCounter = 0;
            }


            base.Update(gameTime);
        }
    }
}
