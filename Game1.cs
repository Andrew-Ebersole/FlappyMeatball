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
        int points = 0;
        private SpriteFont arial;
        int highScore = 0;
        int score = -1;
        float milisecondCounter = 0;
        long pipeHeight;
        float pipeX;
        Random rng = new Random();
        float spinCounter = 0f;
        string initials;
        int selectedInitial;
        bool keysPressed;
        public enum Background
        {
            Title,
            Game,
            Lose,
            NewHighscore
        }
        private Background background;

        //change aspect ratio
        int consoleHeight;
        int consoleWidth;
        
        //uhh this makes the code work
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

        Save highscores;
        bool didScoresUpdate;

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
            _graphics.PreferredBackBufferWidth = 1440*9/21;
            _graphics.PreferredBackBufferHeight = 1440;
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

            highscores = new Save();

            // Initializes the custom devcade controls
            Devcade.Input.Initialize();
            background = Background.Title;
            didScoresUpdate = false;
            initials = "AAA";
            selectedInitial = 0;
            keysPressed = false;

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
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A4)) 
                && !jumpCancel)
            {
                ballVelocity = -18;
                jumpCancel = true;
                if (background == Background.Title)
                {
                    background = Background.Game;
                    milisecondCounter = 0;
                    score = -1;
                } 
                if (background == Background.Lose)
                {
                    milisecondCounter = 0;
                    background = Background.Title;
                    ballPosition.Y = consoleHeight / 2;
                }
            }
            if ((kstate.IsKeyDown(Keys.Enter)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B1)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B2)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B3)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B4))
                && !jumpCancel)
            {
                if (background == Background.NewHighscore)
                {
                    // Enter current initials and return to title screen                    
                    background = Background.Title;
                    if (didScoresUpdate != true)
                    {
                        highscores.UpdateHighscores(score, initials);
                        didScoresUpdate = true;
                    }
                }
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


            //If meatball hits the ceiling it stops and if  it hits the ground it loses
            if (background == Background.Game && ballPosition.Y > consoleHeight - meatball.Height*(consoleHeight / 8000f))
            {
                background = Background.Lose;
                ballPosition.Y = consoleHeight/2;
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
            if (background == Background.Game)
            {
                didScoresUpdate = false;
                ballPosition.Y += ballVelocity * (float)(consoleHeight * 0.00006 * gameTime.ElapsedGameTime.TotalMilliseconds);
                spinCounter += 0.003f * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                pipeX = consoleWidth - ((milisecondCounter * consoleWidth));
            }

            //update score
            milisecondCounter += (float)(0.001 * gameTime.ElapsedGameTime.TotalMilliseconds);
            if (milisecondCounter >= 2 && background == Background.Game)
            {
                milisecondCounter -= 2;
                score++;
                pipeX = _graphics.PreferredBackBufferWidth;
                pipeHeight = rng.NextInt64(consoleHeight * 4 / 10, consoleHeight * 9 / 10);
            }

            //If the ball position overlaps with pipe the game ends
            if (background == Background.Game && score >= 0 && pipeX <= consoleWidth/42 && pipeX >= consoleWidth/-4 
                && (ballPosition.Y < pipeHeight - consoleHeight/4.2f || ballPosition.Y > pipeHeight+0.03*consoleHeight))
            {
                background = Background.Lose;
                ballPosition.Y = consoleHeight / 2;
                milisecondCounter = 0;
            }

            if (background == Background.Lose)
            {
                
                // Checks if got a highscore and prompts user to enter initials if so
                if (highscores.checkIfHighscores(score))
                {
                    background = Background.NewHighscore;
                    initials = "AAA";
                    selectedInitial = 0;
                }
            }

            // makes it so holding down key only counts as one press
            if (kstate.IsKeyUp(Keys.Up) && kstate.IsKeyUp(Keys.Down) && kstate.IsKeyUp(Keys.Left) && kstate.IsKeyUp(Keys.Right))
            {
                keysPressed = false;
            }

            // Changes initials when entering new highscore
            if (background == Background.NewHighscore && !keysPressed)
            {
                

                //Move Up or Down
                //Change the character up or down but clamps so it is only letters.
                char changedInitial = initials[selectedInitial];
                
                if (kstate.IsKeyDown(Keys.Up)
                    || Devcade.Input.GetButtonDown(1, Devcade.Input.ArcadeButtons.StickUp)
                    || Devcade.Input.GetButtonDown(2, Devcade.Input.ArcadeButtons.StickUp))
                {
                    keysPressed = true;
                    if (changedInitial != 'A')
                    {
                        changedInitial--;
                    }
                }
                if (kstate.IsKeyDown(Keys.Down)
                    || Devcade.Input.GetButtonDown(1, Devcade.Input.ArcadeButtons.StickDown)
                    || Devcade.Input.GetButtonDown(2, Devcade.Input.ArcadeButtons.StickDown))
                {
                    keysPressed = true;
                    if (changedInitial != 'Z')
                    {
                        changedInitial++;
                    }
                }

                //changed the intials to incorporate the changed letter
                string oldInitials = initials;
                initials = "";
                for (int i = 0; i < 3; i++)
                {
                    if (i == selectedInitial)
                    {
                        initials += changedInitial;
                    } else
                    {
                        initials += oldInitials[i];
                    }
                }

                //Change initials left or right
                if (kstate.IsKeyDown(Keys.Left)
                    || Devcade.Input.GetButtonDown(1, Devcade.Input.ArcadeButtons.StickLeft)
                    || Devcade.Input.GetButtonDown(2, Devcade.Input.ArcadeButtons.StickLeft))
                {
                    keysPressed = true;
                    selectedInitial--;
                }
                if (kstate.IsKeyDown(Keys.Right)
                    || Devcade.Input.GetButtonDown(1, Devcade.Input.ArcadeButtons.StickRight)
                    || Devcade.Input.GetButtonDown(2, Devcade.Input.ArcadeButtons.StickRight))
                {
                    keysPressed = true;
                    selectedInitial++;
                }
                selectedInitial = Math.Clamp(selectedInitial, 0, 2);
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
            if (background == Background.Title)
            {
                _spriteBatch.DrawString(_font,
                    "Press" +
                    "\nAnything!" +
                    "\nDont hit" +
                    "\nthe floor",
                    new Vector2((consoleWidth / 2) - consoleWidth/ 2.1f, 0), Color.Black);
                score = -1;

                _spriteBatch.DrawString(_font,
                    $"{highscores.getInitials(0)}: {highscores.getHighscore(0)}" +
                    $"\n{highscores.getInitials(1)}: {highscores.getHighscore(1)}" +
                    $"\n{highscores.getInitials(2)}: {highscores.getHighscore(2)}" +
                    $"\n{highscores.getInitials(3)}: {highscores.getHighscore(3)}" +
                    $"\n{highscores.getInitials(4)}: {highscores.getHighscore(4)}",
                    new Vector2((consoleWidth / 2) - consoleWidth / 2.1f, consoleHeight - consoleHeight *4 / 10), Color.Black);
            }

            //Displays meatball
            if (background == Background.Title || background == Background.Game)
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
            if (score >= 0 && background == Background.Game)
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
            if (background == Background.Game && score >= 0)
            {
                _spriteBatch.DrawString(_font,
                    "Score" +
                    $"\n{score}",
                    new Vector2(consoleWidth / 2 - (consoleWidth * 200/420), 0), Color.Black);
            } else if (background == Background.Game)
            {
                _spriteBatch.DrawString(_font,
                    "Score" +
                    $"\n0",
                    new Vector2(_graphics.PreferredBackBufferWidth / 2 - (consoleWidth * 200/420), 0), Color.Black);
            }

            //Displays Restart Screen
            if (background == Background.Lose)
            {
                _spriteBatch.DrawString(_font,
                    "You lose!" +
                    "\nPress " +
                    "\nAnything" +
                    "\nto restart",
                    new Vector2(_graphics.PreferredBackBufferWidth / 2 - (consoleWidth * 200/420), 
                    _graphics.PreferredBackBufferHeight / 2 - (consoleHeight * 200/1000)), Color.Black);

            }

            // Display highscores screen
            if (background == Background.NewHighscore)
            {
                // New Highscore
                _spriteBatch.DrawString(_font,
                    "New" +
                    "\nHighscore!",
                    new Vector2(_graphics.PreferredBackBufferWidth / 2 - (consoleWidth * 200 / 420),
                    _graphics.PreferredBackBufferHeight / 2 - (consoleHeight * 300 / 1000)), Color.Black);

                // All Initials

                string singleInitial = "";
                _spriteBatch.DrawString(_font,
                    singleInitial += initials[0],
                    new Vector2(_graphics.PreferredBackBufferWidth / 2 - (consoleWidth * 200 / 420),
                    _graphics.PreferredBackBufferHeight / 2), Color.Black);

                singleInitial = "";
                _spriteBatch.DrawString(_font,
                    singleInitial += initials[1],
                    new Vector2(_graphics.PreferredBackBufferWidth / 2 - (consoleWidth * 150 / 420),
                    _graphics.PreferredBackBufferHeight / 2), Color.Black);
                singleInitial = "";
                _spriteBatch.DrawString(_font,
                    singleInitial += initials[2],
                    new Vector2(_graphics.PreferredBackBufferWidth / 2 - (consoleWidth * 100 / 420),
                    _graphics.PreferredBackBufferHeight / 2), Color.Black);


                string highlightedInitial = "";
                
                // First Initial
                if (selectedInitial == 0)
                {
                    
                    _spriteBatch.DrawString(_font,
                    highlightedInitial += initials[0],
                    new Vector2(_graphics.PreferredBackBufferWidth / 2 - (consoleWidth * 200 / 420),
                    _graphics.PreferredBackBufferHeight / 2), Color.Gold);
                }

                // Second Initial
                if (selectedInitial == 1)
                {
                    _spriteBatch.DrawString(_font,
                    highlightedInitial += initials[1],
                    new Vector2(_graphics.PreferredBackBufferWidth / 2 - (consoleWidth * 150 / 420),
                    _graphics.PreferredBackBufferHeight / 2), Color.Gold);
                }

                // Third Initial
                if (selectedInitial == 2)
                {
                    _spriteBatch.DrawString(_font,
                    highlightedInitial += initials[2],
                    new Vector2(_graphics.PreferredBackBufferWidth / 2 - (consoleWidth * 100 / 420),
                    _graphics.PreferredBackBufferHeight / 2), Color.Gold);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}