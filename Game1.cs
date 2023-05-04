//Flappy Meatball
//Andrew Ebersole

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FlappyMeatball
{
    public class Game1 : Game
    {
        //variables yay!
        bool jumpCancel = false;
        Texture2D pipe;
        float ballVelocity;
        int points = 0;
        private SpriteFont arial;
        int highScore = 0;
        int score = 0;
        float milisecondCounter = 0;
        Random rng = new Random();
        string initials;
        int selectedInitial;
        bool keysPressed;
        private Pipes pipes;
        private float fadeTimer;
        private Texture2D singlecolor;
        private KeyboardState currentKS;
        private KeyboardState previousKS;

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
        
        // Game Speed
        private float gameSpeed;

        // Meatball
        Texture2D meatballTexture;
        private Meatball meatball;

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

            highscores = new Save();

            // Initializes the custom devcade controls
            Devcade.Input.Initialize();
            background = Background.Title;
            didScoresUpdate = false;
            initials = "AAA";
            selectedInitial = 0;
            keysPressed = false;
            gameSpeed = 1f;
            fadeTimer = 0;
            currentKS = Keyboard.GetState();
            previousKS = Keyboard.GetState();

            base.Initialize();
        }

        //load the meatball image and others
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            meatballTexture = Content.Load<Texture2D>("meatball");
            pipe = Content.Load<Texture2D>("spaghetti");
            _font = Content.Load<SpriteFont>("File");

            // Pipes
            pipes = new Pipes(pipe, new Rectangle(0,0,consoleWidth,consoleHeight));

            // Meatball
            meatball = new Meatball(
                new Vector2(_graphics.PreferredBackBufferWidth / 4,_graphics.PreferredBackBufferHeight / 2),
                (int)(consoleHeight / 10f), (int)(consoleHeight / 11f),
                meatballTexture,
                new Rectangle(0,0,consoleWidth,consoleHeight));

            singlecolor = new Texture2D(GraphicsDevice, 1, 1);
            singlecolor.SetData(new[] { Color.White });
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

            currentKS = Keyboard.GetState();

            // Game State Machine
            switch (background)
            {
                case Background.Title: // --- Title ---------------------------------------------------------//
                    gameSpeed = 1;
                    meatball.Pos = new Vector2(consoleWidth * 0.2f, consoleHeight * 0.4f);
                    meatball.Update(gameTime, gameSpeed);

                    if (PressAnyButton()
                        && !jumpCancel)
                    {
                        gameSpeed = 1f ;
                        background = Background.Game;
                        score = 0;
                        meatball.Velocity =  -(float)Math.Sqrt(500 * gameSpeed);
                        jumpCancel = true;
                        pipes.RespawnPipes(score);
                    }
                    break;

                case Background.Game: // --- Game ---------------------------------------------------------//
                    meatball.Update(gameTime, gameSpeed);

                    //If meatball hits the ceiling it stops and if  it hits the ground it loses
                    if (meatball.Rectangle.Intersects(new Rectangle(0, consoleHeight, consoleWidth, consoleHeight)))
                    {
                        background = Background.Lose;
                        fadeTimer = 0;
                        meatball.Pos = new Vector2(meatball.Pos.X, (consoleHeight / 2));
                    }
                    else if (meatball.Rectangle.Intersects(new Rectangle(0, -consoleHeight, consoleWidth, consoleHeight)))
                    {
                        meatball.Pos = new Vector2(meatball.Pos.X, (int)(meatball.Rectangle.Height * (consoleHeight / 8000f)));
                        meatball.Velocity = 0;
                    }

                    didScoresUpdate = false;
                    pipes.Update(gameTime, gameSpeed);
                    if (pipes.Pos.X + pipes.Rectangle[0].Width < 0)
                    {
                        // Update GameSpeed
                        if (gameSpeed < 1.5)
                        {
                            gameSpeed += 0.02f;
                        }
                        else if (gameSpeed < 2)
                        {
                            gameSpeed += 0.005f;
                        }
                        else if (gameSpeed > 2.5)
                        {
                            gameSpeed += 0.0001f;
                        }
                        score++;
                        pipes.RespawnPipes(score);
                    }

                    // check if meatball hit pipe
                    if (DidMeatballCrash())
                    {
                        background = Background.Lose;
                        fadeTimer = 0;
                        meatball.Pos = new Vector2(meatball.Pos.X, consoleHeight / 2);
                    }

                    //When space is clicked, jumps and waits until space is let go to jump again
                    if (PressAnyButton()
                        && !jumpCancel)
                    {
                        meatball.Velocity = -(float)Math.Sqrt(500 * gameSpeed);
                        jumpCancel = true;
                    }

                    break;

                case Background.Lose: // --- Lose ---------------------------------------------------------//
                    fadeTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    // Checks if got a highscore and prompts user to enter initials if so
                    if (fadeTimer > 1000
                        && PressAnyButton())
                    {
                        if (highscores.checkIfHighscores(score))
                        {
                            background = Background.NewHighscore;
                            initials = "JKL";
                            selectedInitial = 0;
                        } else
                        {
                            background = Background.Title;
                        }
                        milisecondCounter = 0;
                        meatball.Pos = new Vector2(meatball.Pos.X, consoleHeight / 2);
                    }
                    break;

                case Background.NewHighscore:  // --- Highscore ---------------------------------------------------------//
                    if (PressAnyButton()
                    && !jumpCancel)
                    {
                        // Enter current initials and return to title screen                    
                        background = Background.Title;
                        if (didScoresUpdate != true)
                        {
                            highscores.UpdateHighscores(score, initials);
                            didScoresUpdate = true;
                        }
                    }

                    // Changes initials when entering new highscore
                    if (!keysPressed)
                    {
                        //Move Up or Down
                        //Change the character up or down but clamps so it is only letters.
                        char changedInitial = initials[selectedInitial];

                        if (currentKS.IsKeyDown(Keys.Up)
                            || Devcade.Input.GetButtonDown(1, Devcade.Input.ArcadeButtons.StickUp)
                            || Devcade.Input.GetButtonDown(2, Devcade.Input.ArcadeButtons.StickUp))
                        {
                            keysPressed = true;
                            if (changedInitial != 'A')
                            {
                                changedInitial--;
                            }
                        }
                        if (currentKS.IsKeyDown(Keys.Down)
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
                            }
                            else
                            {
                                initials += oldInitials[i];
                            }
                        }

                        //Change initials left or right
                        if (currentKS.IsKeyDown(Keys.Left)
                            || Devcade.Input.GetButtonDown(1, Devcade.Input.ArcadeButtons.StickLeft)
                            || Devcade.Input.GetButtonDown(2, Devcade.Input.ArcadeButtons.StickLeft))
                        {
                            keysPressed = true;
                            selectedInitial--;
                        }
                        if (currentKS.IsKeyDown(Keys.Right)
                            || Devcade.Input.GetButtonDown(1, Devcade.Input.ArcadeButtons.StickRight)
                            || Devcade.Input.GetButtonDown(2, Devcade.Input.ArcadeButtons.StickRight))
                        {
                            keysPressed = true;
                            selectedInitial++;
                        }
                        selectedInitial = Math.Clamp(selectedInitial, 0, 2);
                    }

                    break;

            }
           

            //Allows user to jump again
            if (!PressAnyButton()           
                && jumpCancel)
            {
                jumpCancel = false;
            }

           
            

            // makes it so holding down key only counts as one press
            if (currentKS.IsKeyUp(Keys.Up) && currentKS.IsKeyUp(Keys.Down) && currentKS.IsKeyUp(Keys.Left) && currentKS.IsKeyUp(Keys.Right))
            {
                keysPressed = false;
            }

            
            // Updates Devcade Inputs
            Devcade.Input.Update();

            previousKS = currentKS;

            base.Update(gameTime);
        }

        /*** draw objects ***/
        protected override void Draw(GameTime gameTime)
        {
            //background color
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // Game Finite State Machine
            switch (background)
            {
                case Background.Title: // --- Title ---------------------------------------------------------//

                    _spriteBatch.DrawString(_font,
                    "Press" +
                    "\nAnything!" +
                    "\nDont hit" +
                    "\nthe floor",
                    new Vector2((consoleWidth / 2) - consoleWidth / 2.1f, 0), Color.Black);
                    score = 0; 

                    _spriteBatch.DrawString(_font,
                        $"{highscores.getInitials(0)}: {highscores.getHighscore(0)}" +
                        $"\n{highscores.getInitials(1)}: {highscores.getHighscore(1)}" +
                        $"\n{highscores.getInitials(2)}: {highscores.getHighscore(2)}" +
                        $"\n{highscores.getInitials(3)}: {highscores.getHighscore(3)}" +
                        $"\n{highscores.getInitials(4)}: {highscores.getHighscore(4)}",
                        new Vector2((consoleWidth / 2) - consoleWidth / 2.1f,
                        consoleHeight - consoleHeight * 4 / 10), Color.Black);
                    
                    meatball.Draw(_spriteBatch);

                    break;

                case Background.Game: // --- Game ---------------------------------------------------------//

                    // Draw Meatball
                    meatball.Draw(_spriteBatch);

                    // Draw Pipes
                    pipes.Draw(_spriteBatch);

                    //Displays Score
                    if (score >= 0)
                    {
                        _spriteBatch.DrawString(_font,
                            "Score" +
                            $"\n{score}",
                            new Vector2(consoleWidth / 2 - (consoleWidth * 200 / 420), 0), Color.Black);
                    }
                    else
                    {
                        _spriteBatch.DrawString(_font,
                            "Score" +
                            $"\n0",
                            new Vector2(_graphics.PreferredBackBufferWidth / 2 - (consoleWidth * 200 / 420), 0), Color.Black);
                    }

                    break;

                case Background.Lose: // --- Lose ---------------------------------------------------------//

                    // Draw blackout fade
                    _spriteBatch.Draw(singlecolor,
                        new Rectangle(0,0,consoleWidth,consoleHeight),
                        Color.Black * (fadeTimer / 2000f));

                    // draw lose text
                    _spriteBatch.DrawString(_font,
                    "You lose!" +
                    $"\nScore:{score}" +
                    "\nPress " +
                    "\nAnything" +
                    "\nto restart",
                    new Vector2(_graphics.PreferredBackBufferWidth / 2 - (consoleWidth * 200 / 420),
                    _graphics.PreferredBackBufferHeight / 2 - (consoleHeight * 200 / 1000)), Color.White);

                    break;

                case Background.NewHighscore: // --- Highscore ---------------------------------------------//

                    
                    // New Highscore
                    _spriteBatch.DrawString(_font,
                        "New" +
                        "\nHighscore" +
                        $"\n{score}!",
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


                    // Draw highlighted initial
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

                    break;

            }


            // Debug draw meatball hitbox
            //_spriteBatch.Draw(pipe,meatball.Rectangle,Color.Red * 0.9f);
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Returns if any button is pressed
        /// </summary>
        /// <returns></returns>
        public bool PressAnyButton()
        {
            // Check if any of the buttons are pressed
            if (Keyboard.GetState().IsKeyDown(Keys.Space)
                && previousKS.IsKeyUp(Keys.Space)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A1)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A2)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A3)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A4)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B1)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B2)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B3)
                || GamePad.GetState(1).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B4)
                || GamePad.GetState(2).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A1)
                || GamePad.GetState(2).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A2)
                || GamePad.GetState(2).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A3)
                || GamePad.GetState(2).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A4)
                || GamePad.GetState(2).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B1)
                || GamePad.GetState(2).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B2)
                || GamePad.GetState(2).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B3)
                || GamePad.GetState(2).IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B4))
            {
                return true;
            }

            // If not return false
            return false;
        }

        /// <summary>
        /// Return true if meatball hits the pipe
        /// </summary>
        /// <returns></returns>
        private bool DidMeatballCrash()
        {
            // check all pipes to see collisions
            foreach (Rectangle r in pipes.Rectangle)
            {
                if (r.Intersects(meatball.Rectangle))
                {
                    return true;
                }
            }

            // if not collided than don't crash
            return false;
        }
    }
}