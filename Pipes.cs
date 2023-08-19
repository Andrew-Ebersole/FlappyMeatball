using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FlappyMeatball
{
    internal class Pipes
    {
        // --- Fields --- //

        private Vector2 position;
        private Random rng;
        private List<Rectangle> pipes;
        private Texture2D texture;
        private int pipeGap;
        private Rectangle window;
        private Point pipeSize;


        // --- Properties --- //

        /// <summary>
        /// The X and Y position of the top left corner of the bottom pipe
        /// </summary>
        public Vector2 Pos { get { return position; } set { position = value; } }

        /// <summary>
        /// list of all pipes
        /// </summary>
        public List<Rectangle> Rectangle { get { return pipes; } }


        // --- Constructor --- //

        public Pipes(Texture2D texture, Rectangle window)
        {
            this.texture = texture;
            this.window = window;
            position = new Vector2(window.Width, window.Height);
            pipes = new List<Rectangle>();
            pipeGap = (int)(window.Height * 0.5f);
            rng = new Random();
            pipes.Add(new Rectangle((int)position.X,(int)position.Y,(int)(window.Width * 0.2f), (int)(window.Height * 0.8f)));
            pipes.Add(new Rectangle((int)position.X, (int)position.Y - pipes[0].Height - pipeGap, (int)(window.Width * 0.2f), (int)(window.Height * 0.8f)));

        }



        // --- Methods --- //


        public void Update(GameTime gt, float gameSpeed)
        {
            // Change position
            position.X -= (float)gt.ElapsedGameTime.TotalMilliseconds * gameSpeed * 0.3f;

            // Update Rectangles
            pipes[0] = (new Rectangle((int)position.X, (int)position.Y, (int)(window.Width * 0.25f), (int)(window.Height * 0.8f)));
            pipes[1] = new Rectangle((int)position.X, (int)position.Y - pipes[0].Height - pipeGap, (int)(window.Width * 0.25f), (int)(window.Height * 0.8f));
        }

        /// <summary>
        /// Draw the pipes
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture,
                new Rectangle((int)(pipes[0].X + 0.5f * pipes[0].Width), 
                (int)(pipes[0].Y + 0.5f * pipes[0].Height),
                pipes[0].Width, pipes[0].Height),
                null,
                Color.White,
                (float)Math.PI,
                new Vector2(texture.Width / 2, texture.Height / 2),
                SpriteEffects.None,
                0);

            sb.Draw(texture,
                new Rectangle((int)(pipes[1].X + 0.5f * pipes[1].Width),
                (int)(pipes[1].Y + 0.5f * pipes[1].Height),
                pipes[1].Width, pipes[1].Height),
                null,
                Color.White,
                2 * (float)Math.PI,
                new Vector2(texture.Width / 2, texture.Height / 2),
                SpriteEffects.None,
                0);

            // Show hitboxes
            //sb.Draw(texture,
            //    pipes[0],
            //    Color.Red * 0.8f);

            //sb.Draw(texture,
            //    pipes[1],
            //    Color.Yellow * 0.8f);
        }

        public void RespawnPipes(int score)
        {
            // Update gap based on score
            if (pipeGap > (int)(window.Height * 0.3f))
            {
                pipeGap = (int)(window.Height * (0.5f - (score * 0.01f)));
            }
            if (pipeGap < (int)(window.Height * 0.3f))
            {
                pipeGap = (int)(window.Height * 0.3f);
            }

            // Pick random height to set pipes to
            position = 
                new Vector2(window.Width + pipes[0].Width,
                rng.Next((int)(0.1f * window.Height) + pipeGap, (int)(0.9f * window.Height)));
        }
    }
}
