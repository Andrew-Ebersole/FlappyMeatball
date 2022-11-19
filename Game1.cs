//Flappy Meatball
//Andrew Ebersole

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.CompilerServices;

namespace FlappyMeatball
{
    public class Game1 : Game
    {
        //variables yay!
        bool jumpCancel = false;
        Texture2D meatball;
        Texture2D pipe;
        Vector2 ballPosition;
        float ballVelocity;
        bool gameRunning = false;
        bool gameLost = false;
        int points = 0;
        private SpriteFont arial;
        int highScore = 0;
        int score = -1;
        float milisecondCounter = 0;
        long pipeHeight;
        float pipeX;
        Random rng = new Random();
        float spinCounter = 0f;

        //change aspect ratio
        int consoleHeight;
        int consoleWidth;
        
        //uhh this makes the code work
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

        //set the Aspect ratio and others
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            
        }

        //it is important that this is hear i think
        protected override void Initialize()
        {
            // Set window size if running debug (in release it will be fullscreen)
            #region
#if DEBUG
            _graphics.PreferredBackBufferWidth = 200*9/21;
            _graphics.PreferredBackBufferHeight = 200;
            _graphics.ApplyChanges();
#else
			_graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
			_graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
			_graphics.ApplyChanges();
#endif
            #endregion

            consoleWidth = _graphics.PreferredBackBufferWidth;
            consoleHeight = _graphics.PreferredBackBufferHeight;

            ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 4,
            _graphics.PreferredBackBufferHeight / 2);


            // Initializes the custom devcade controls
            Devcade.Input.Initialize();

            base.Initialize();
        }

        //load the meatball image and others
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            meatball = Content.Load<Texture2D>("meatball");
            pipe = Content.Load<Texture2D>("spaghetti");
            _font = Content.Load<SpriteFont>("File");

        }

        /*** Game Logic ***/
        protected override void Update(GameTime gameTime)
        {
            //exits game if escape button pressed
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
                || Keyboard.GetState().IsKeyDown(Keys.Escape)
                || (Devcade.Input.GetButton(1,Devcade.Input.ArcadeButtons.Menu) 
                && Devcade.Input.GetButton(2, Devcade.Input.ArcadeButtons.Menu)))
                Exit();

            var kstate = Keyboard.GetState();
            
            //When space is clicked, jumps and waits until space is let go to jump again
            // haha look at all these controls
            if ((kstate.IsKeyDown(Keys.Space) 
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A1)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A2)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A3)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A4)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B1)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B2)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B3)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B4)) 
                && !jumpCancel)
            {
                ballVelocity = -18;
                jumpCancel = true;
                if (gameRunning == false && !gameLost)
                {
                    gameRunning = true;
                    gameLost = false;
                    milisecondCounter = 0;
                    score = -1;
                }  
            }

            //Checks highscore
            if (score > highScore)
            {
                highScore = score;
            }

            //Allows user to jump again
            if ((kstate.IsKeyUp(Keys.Space)
                && GamePad.GetState(1).IsButtonUp((Buttons)Devcade.Input.ArcadeButtons.A1)
                && GamePad.GetState(1).IsButtonUp((Buttons)Devcade.Input.ArcadeButtons.A2)
                && GamePad.GetState(1).IsButtonUp((Buttons)Devcade.Input.ArcadeButtons.A3)
                && GamePad.GetState(1).IsButtonUp((Buttons)Devcade.Input.ArcadeButtons.A4)
                && GamePad.GetState(1).IsButtonUp((Buttons)Devcade.Input.ArcadeButtons.B1)
                && GamePad.GetState(1).IsButtonUp((Buttons)Devcade.Input.ArcadeButtons.B2)
                && GamePad.GetState(1).IsButtonUp((Buttons)Devcade.Input.ArcadeButtons.B3)
                && GamePad.GetState(1).IsButtonUp((Buttons)Devcade.Input.ArcadeButtons.B4))
                && jumpCancel)
            {
                jumpCancel = false;
            }

            //Starts a new game
            if ((kstate.IsKeyDown(Keys.Space)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A1)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A2)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A3)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A4)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B1)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B2)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B3)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B4))
                && gameLost)
            {
                milisecondCounter = 0;
                score = -1;
                gameLost = false;
                ballPosition.Y = consoleHeight / 2;
            }

            //If meatball hits the ceiling it stops and if  it hits the ground it loses
            if (gameRunning && ballPosition.Y > consoleHeight - meatball.Height*(consoleHeight / 8000f))
            {
                gameRunning = false;
                gameLost = true;
                ballPosition.Y = consoleHeight/2;
                score = -1;
                milisecondCounter = 0;
            }
            else if (ballPosition.Y < meatball.Height*(consoleHeight / 8000f))
            {
                ballPosition.Y = (meatball.Height*(consoleHeight / 8000f));
                ballVelocity = 0;
            }
           
            //Gravity
            if (ballVelocity < 16)
            {
                ballVelocity += (float)(0.06 * gameTime.ElapsedGameTime.TotalMilliseconds);
            }
            else
            {
                ballVelocity = 16;
            }

            //Updates the ball and pipe 
            if (gameRunning)
            {
                ballPosition.Y += ballVelocity * (float)(consoleHeight * 0.00006 * gameTime.ElapsedGameTime.TotalMilliseconds);
                spinCounter += 0.003f * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                pipeX = consoleWidth - ((milisecondCounter * consoleWidth));
            }

            //update score
            milisecondCounter += (float)(0.001 * gameTime.ElapsedGameTime.TotalMilliseconds);
            if (milisecondCounter >= 2 && !gameLost)
            {
                milisecondCounter -= 2;
                score++;
                pipeX = _graphics.PreferredBackBufferWidth;
                pipeHeight = rng.NextInt64(consoleHeight * 4 / 10, consoleHeight * 9 / 10);
            }

            //If the ball position overlaps with pipe the game ends
            if (score >= 0 && pipeX <= consoleWidth/42 && pipeX >= consoleWidth/-4 
                && (ballPosition.Y < pipeHeight - consoleHeight/4.2f || ballPosition.Y > pipeHeight+0.03*consoleHeight))
            {
                gameRunning = false;
                gameLost = true;
                ballPosition.Y = _graphics.PreferredBackBufferHeight - meatball.Height * (consoleHeight / 8000f);
                score = -1;
                milisecondCounter = 0;
            }

            // Updates Devcade Inputs
            Devcade.Input.Update();

            base.Update(gameTime);
        }

        /*** draw objects ***/
        protected override void Draw(GameTime gameTime)
        {
            //background color
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            
            //Displays title screen
            if (gameRunning == false & gameLost == false)
            {
                _spriteBatch.DrawString(_font,
                    "Press" +
                    "\nAnything!" +
                    "\nDont hit" +
                    "\nthe floor",
                    new Vector2((consoleWidth / 2) - consoleWidth/ 2.1f, 0), Color.Black);
                score = -1;

                _spriteBatch.DrawString(_font,
                    $"High " +
                    "\nScore" +
                    $"\n{highScore} pts",
                    new Vector2((consoleWidth / 2) - consoleWidth / 2.1f, consoleHeight - consoleHeight *3 / 10), Color.Black);
            }

            //Displays meatball
            if (gameLost == false)
            {

                _spriteBatch.Draw(
                meatball,
                ballPosition,
                null,
                Color.White,
                spinCounter,
                new Vector2(meatball.Height / 2, meatball.Height / 2),
                new Vector2(consoleHeight/4000f,consoleHeight/4000f),
                SpriteEffects.None,
                0f
                );
            }

            //Displays pipes
            if (score >= 0 && gameLost == false && gameRunning == true)
            {
                _spriteBatch.Draw(
                    pipe,
                    new Vector2(pipeX + (consoleWidth * 0.488f), pipeHeight - consoleHeight * 0.95f),
                    null,
                    Color.White,
                    3.1415f / 2,
                    new Vector2(+50, 0),
                    new Vector2(consoleWidth / 525f, consoleHeight / 2857f),
                    SpriteEffects.None,
                    0f
                    ); ;

                _spriteBatch.Draw(
                   pipe,
                   new Vector2(pipeX + (consoleWidth*0.4761f), pipeHeight + consoleHeight * 0.1f),
                   null,
                   Color.White,
                   3.1415f / 2,
                   new Vector2(+50, 0),
                   new Vector2(consoleWidth/525f, consoleHeight/2857f),
                   SpriteEffects.FlipHorizontally,
                   0f
                   );
            }

            //Displays Score
            if (gameLost == false && gameRunning == true && score >= 0)
            {
                _spriteBatch.DrawString(_font,
                    "Score" +
                    $"\n{score}",
                    new Vector2(consoleWidth / 2 - (consoleWidth * 200/420), 0), Color.Black);
            } else if (gameLost == false && gameRunning == true)
            {
                _spriteBatch.DrawString(_font,
                    "Score" +
                    $"\n0",
                    new Vector2(_graphics.PreferredBackBufferWidth / 2 - (consoleWidth * 200/420), 0), Color.Black);
            }

            //Displays Restart Screen
            if (gameLost)
            {
                _spriteBatch.DrawString(_font,
                    "You lose!" +
                    "\nPress " +
                    "\nAnything" +
                    "\nto restart",
                    new Vector2(_graphics.PreferredBackBufferWidth / 2 - (consoleWidth * 200/420), 
                    _graphics.PreferredBackBufferHeight / 2 - (consoleHeight * 200/1000)), Color.Black);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}