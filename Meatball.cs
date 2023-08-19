using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlappyMeatball
{
    internal class Meatball
    {
        // --- Fields --- //

        private Vector2 position;
        private Rectangle rectangle;
        private Texture2D texture;
        private float spinCounter;
        private float velocity;
        private Rectangle window;

        

        // --- Properties --- //

        /// <summary>
        /// The X and Y position of the top left corner of the meatball
        /// </summary>
        public Vector2 Pos { get { return position; } set { position = value; } }

        /// <summary>
        /// The rectangle containing width and height as well as ints for the x and y position
        /// </summary>
        public Rectangle Rectangle { get { return rectangle; } }

        /// <summary>
        /// The y velocity of the meatball
        /// </summary>
        public float Velocity { get { return velocity; } set { velocity = value; } }


        // --- Constructor --- //

        /// <summary>
        /// Stores updates and draws the meatball
        /// </summary>
        /// <param name="position"> x,y location of the meatball </param>
        /// <param name="size"> width, height of the meatball</param>
        /// <param name="texture"> texture to use for the meatball</param>
        public Meatball(Vector2 position,int width, int height, Texture2D texture, Rectangle window)
        {
            this.position = position;
            rectangle = new Rectangle((int)position.X, (int)position.Y, height, width);
            this.texture = texture;
            float spinCounter = 0f;
            velocity = 0;
            this.window = window;
        }



        // --- Methods --- //

        public void Update(GameTime gt, float gameSpeed)
        {
            // Spin counter
            spinCounter += (float)gt.ElapsedGameTime.TotalMilliseconds * gameSpeed * 0.001f;

            // Gravity
            if (velocity < 22 * gameSpeed)
            {
                velocity += 0.09f * gameSpeed * (float)gt.ElapsedGameTime.TotalMilliseconds;
            }
            else
            {
                velocity = 22 * gameSpeed;
            }

            // update position based on velocity
            position.Y += velocity * window.Height / 1440;

            // Update rectangle to position
            rectangle.X = (int)position.X;
            rectangle.Y = (int)position.Y;
        }

        /// <summary>
        /// Draw the meatball
        /// </summary>
        /// <param name="sb">Spritebatch</param>
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(
                texture,
                new Rectangle((int)(rectangle.X+0.5*rectangle.Width),(int)(rectangle.Y+0.5*rectangle.Height),
                rectangle.Width,rectangle.Height),
                null,
                Color.White,
                spinCounter,
                new Vector2(texture.Width/2,texture.Height/2),
                SpriteEffects.None,
                0);
        }
    }
}
