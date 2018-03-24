using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;

namespace MyFirstProject
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // define some constant values for the game states
        const int STATE_SPLASH = 0;
        const int STATE_MISSION = 1;
        const int STATE_GAME = 2;
        const int STATE_MISSIONOVER = 3;

        // Set initial game state to splash
        int gameState = STATE_SPLASH;

        // Declare texture variables
        SpriteFont robotaurFont;
        SpriteFont robotaurFontSmall;

        Texture2D shipTexture;
        Texture2D asteroidTexture;
        Texture2D bulletTexture;
        Texture2D missionFailedTexture;
        Texture2D missionCompleteTexture;
        Texture2D resultTexture;
        Texture2D spaceTexture;
        Texture2D splashTexture;
        Texture2D missionOrdersTexture;
        Texture2D failBackTexture;
        Texture2D explosionTexture;

        SoundEffect laser;
        SoundEffect explosion;
        SoundEffect missionComplete;
        SoundEffect missionFail;
        Song backgroundMusic;

        // Player global variables
        float playerSpeed = 200;
        float playerRotateSpeed = 5;
        Vector2 playerPosition = new Vector2(0, 0);
        Vector2 playerOffset = new Vector2(0, 0);
        float playerAngle = 0;
        bool playerAlive = false;
        float score = 0;
        float finalScore;
        float tempBonusScore;
        int bonusScore;
        const float asteroidScoreValue = 100;
        Vector2 explosionPosition;
        const int explosionTimer = 10;

        // Asteroid global variables
        const float baseAsteroidSpeed = 50;
        float asteroidSpeed = 50;
        Vector2 asteroidOffset = new Vector2(0, 0);
        ArrayList asteroidPositions = new ArrayList();
        ArrayList asteroidVelocities = new ArrayList();
        bool asteroidHit = false;
        int asteroidHitTimer = 0;


        // Bullet global variables
        float bulletSpeed = 400;
        Vector2 bulletPosition = new Vector2(0, 0);
        Vector2 bulletVelocity = new Vector2(0, 0);
        bool bulletAlive = false;
        int ammoUsed = 0;

        // Create global random to use in functions
        Random random = new Random();

        bool win = false;
        int asteroidCount = 0;
        int asteroidCountLimit = 10;
        int levelCount = 1;
        int finalLevel;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            this.graphics.SynchronizeWithVerticalRetrace = true;
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

            // gameState = STATE_GAME;
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
            robotaurFont = Content.Load<SpriteFont>("fonts/Robotaur");
            robotaurFontSmall = Content.Load<SpriteFont>("fonts/RobotaurSmall");

            shipTexture = Content.Load<Texture2D>("images/ship");
            asteroidTexture = Content.Load<Texture2D>("images/rock");
            bulletTexture = Content.Load<Texture2D>("images/beam");
            explosionTexture = Content.Load<Texture2D>("images/explosion");

            missionFailedTexture = Content.Load<Texture2D>("titles/missionFailed");
            missionCompleteTexture = Content.Load<Texture2D>("titles/missionComplete");
            missionOrdersTexture = Content.Load<Texture2D>("titles/missionOrders");

            spaceTexture = Content.Load<Texture2D>("backgrounds/spaceBack");            
            splashTexture = Content.Load<Texture2D>("backgrounds/splashBack");
            failBackTexture = Content.Load<Texture2D>("backgrounds/failBack");

            laser = Content.Load<SoundEffect>("sounds/laser");
            explosion = Content.Load<SoundEffect>("sounds/explosion");
            missionComplete = Content.Load<SoundEffect>("sounds/missonComplete");
            missionFail = Content.Load<SoundEffect>("sounds/missonFail");
            backgroundMusic = Content.Load<Song>("sounds/backgroundMusic");
            MediaPlayer.Volume = 0.2f;
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;

            playerOffset = new Vector2(shipTexture.Width / 2, shipTexture.Height / 2);
            asteroidOffset = new Vector2(asteroidTexture.Width / 2, asteroidTexture.Height / 2);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void StartLevel()
        {
            // Put player in start of screen and set to alive
            playerPosition = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
            playerAlive = true;

            CreateAsteroids();
            ammoUsed = 0;
        }

        private void UpdatePlayer(float deltaTime)
        {
            float currentSpeed = 0;

            if (Keyboard.GetState().IsKeyDown(Keys.Up) == true)
            {
                currentSpeed = playerSpeed * deltaTime;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Down) == true)
            {
                currentSpeed = -playerSpeed * deltaTime;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) == true)
            {
                playerAngle -= playerRotateSpeed * deltaTime;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right) == true)
            {
                playerAngle += playerRotateSpeed * deltaTime;
            }

            Vector2 playerDirection = new Vector2(-(float)Math.Sin(playerAngle), (float)Math.Cos(playerAngle));

            playerDirection.Normalize();

            // Shoot a bullet
            if (Keyboard.GetState().IsKeyDown(Keys.Space) == true && bulletAlive == false)
            {
                bulletPosition = playerPosition;
                bulletVelocity = playerDirection * bulletSpeed;
                bulletAlive = true;
                laser.Play(.2f,0,0);
                ammoUsed++;
            }

            // the Vector2 class also has a normalize function
            Vector2 direction = new Vector2(40, 30);
            direction.Normalize();

            Vector2 playerVelocity = playerDirection * currentSpeed;
            playerPosition += playerVelocity;
        }

        private void CreateAsteroids()
        {
            // Every second level slightly increase speed
            if(levelCount % 2 == 0)
            {
                asteroidSpeed += 15f;
            }
            
            for (int i = 0; i < asteroidCount + levelCount; i++)
            {
                if(i >= asteroidCountLimit)
                {
                    break;
                }
                Vector2 randDirection = new Vector2(random.Next(-100, 100), random.Next(-100, 100));
                randDirection.Normalize();

                Vector2 asteroidPosition = SetAsteroidStartPosition();
                asteroidPositions.Add(asteroidPosition);

                // Every third level point all asteroids at player
                Vector2 velocity;
                if(levelCount % 3 == 0)
                {
                    velocity = (playerPosition - asteroidPosition);
                }
                else
                {
                    velocity = new Vector2(random.Next(-100, 100), random.Next(-100, 100));
                }
                
                velocity.Normalize();
                velocity *= asteroidSpeed;
                asteroidVelocities.Add(velocity);
            }
        }

        private void UpdateAsteroids(float deltaTime)
        {
            for (int asteroidIdx = 0; asteroidIdx < asteroidPositions.Count; asteroidIdx++)
            {
                Vector2 position = (Vector2)asteroidPositions[asteroidIdx];
                Vector2 velocity = (Vector2)asteroidVelocities[asteroidIdx];

                position += velocity * deltaTime;
                asteroidPositions[asteroidIdx] = position;

                if (position.X < 0 && velocity.X < 0 || position.X > graphics.GraphicsDevice.Viewport.Width && velocity.X > 0)
                {
                    velocity.X = -velocity.X;
                    asteroidVelocities[asteroidIdx] = velocity;
                }
                if (position.Y < 0 && velocity.Y < 0 || position.Y > graphics.GraphicsDevice.Viewport.Height && velocity.Y > 0)
                {
                    velocity.Y = -velocity.Y;
                    asteroidVelocities[asteroidIdx] = velocity;
                }
            }
        }

        private Vector2 SetAsteroidStartPosition()
        {
            // Set X value
            int tempX = random.Next(50, 800);
            if(tempX >= 300 && tempX <= 400)
            {
                tempX -= 150;
            }
            else if(tempX >= 400 && tempX <= 500)
            {
                tempX += 150;
            }

            // Set Y value
            int tempY = random.Next(25, 480);
            if (tempY >= 150 && tempY <= 240)
            {
                tempY -= 100;
            }
            else if (tempY >= 240 && tempY <= 330)
            {
                tempY += 100;
            }

            // Create temp vector with generated X,Y values
            Vector2 tempVector = new Vector2(tempX, tempY);
            return tempVector;
        }

        private void UpdateBullets(float deltaTime)
        {
            if (bulletAlive == false)
            {
                return;
            }

            bulletPosition += bulletVelocity * deltaTime;

            if (bulletPosition.X < 0 ||
            bulletPosition.X > graphics.GraphicsDevice.Viewport.Width ||
            bulletPosition.Y < 0 ||
            bulletPosition.Y > graphics.GraphicsDevice.Viewport.Height)
            {
                bulletAlive = false;
            }

        }

        // Circular
        private bool IsColliding(Vector2 position1, float radius1, Vector2 position2, float radius2)
        {
            Vector2 distance = position2 - position1;

            if(distance.Length() < radius1 + radius2)
            {
                explosion.Play(0.3f, 0, 0);
                score += (asteroidScoreValue * levelCount);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // TODO: Add your update logic here
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch(gameState)
            {
                case STATE_SPLASH:
                    UpdateSplashState(deltaTime);
                    break;
                case STATE_MISSION:
                    UpdateMissionState(deltaTime);
                    break;
                case STATE_GAME:
                    UpdateGameState(deltaTime);
                    break;
                case STATE_MISSIONOVER:
                    UpdateMissionOverState(deltaTime);
                    break;
            }

            base.Update(gameTime);

        }

        private void UpdateSplashState(float deltaTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) == true)
            {
                gameState = STATE_MISSION;
            }
        }

        private void DrawSplashState(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(splashTexture, new Vector2(0, 0));

            spriteBatch.DrawString(robotaurFont, "Press Space to start", new Vector2(50, 425), Color.Green);
            spriteBatch.DrawString(robotaurFont, "Press Escape to quit", new Vector2(50, 450), Color.Red);
        }

        private void UpdateMissionState(float deltaTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                gameState = STATE_GAME;
                StartLevel();
            }
        }

        private void DrawMissionState(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spaceTexture, new Vector2(0, 0), null, null, null, 0, null, Color.White);
            spriteBatch.Draw(missionOrdersTexture, new Vector2(0, -25));

            string introText = "A previously unknown asteroid field is moving towards earth.\n\nScientists from around the world agree it will destroy the planet,\n\nif unchecked. The United Nations have chosen you to defend us.\n\nEquipped with the best tech humanity has to offer,\n\nyou have been sent to space to save us all.";
            string helpText = "Space to shoot.\n\nUp and down to move.\n\nRight and left to turn.";

            spriteBatch.DrawString(robotaurFontSmall, introText, new Vector2(50, 70), Color.White);
            spriteBatch.DrawString(robotaurFont, "How To Play", new Vector2(50, 250), Color.White);
            spriteBatch.DrawString(robotaurFontSmall, helpText, new Vector2(50, 280), Color.White);
            spriteBatch.DrawString(robotaurFont, "Level " + levelCount, new Vector2(50, 400), Color.White);
            spriteBatch.DrawString(robotaurFont, "Press Enter to launch", new Vector2(50, 425), Color.Green);
            spriteBatch.DrawString(robotaurFont, "Press Escape to quit", new Vector2(50, 450), Color.Red);
        }

        private void UpdateGameState(float deltaTime)
        {
            UpdatePlayer(deltaTime);
            UpdateAsteroids(deltaTime);
            UpdateBullets(deltaTime);

            Rectangle playerRect = new Rectangle(
            (int)(playerPosition.X - playerOffset.X),
            (int)(playerPosition.Y - playerOffset.Y),
            shipTexture.Bounds.Width,
            shipTexture.Bounds.Height);

            Rectangle bulletRect = new Rectangle(
            (int)bulletPosition.X,
            (int)bulletPosition.Y,
            bulletTexture.Bounds.Width,
            bulletTexture.Bounds.Height);

            // check for bullet collision CIRC
            if (bulletAlive == true)
            {
                for (int asteroidIdx = 0; asteroidIdx < asteroidPositions.Count; asteroidIdx++)
                {
                    Vector2 asteroidPosition = (Vector2)asteroidPositions[asteroidIdx];

                    if(IsColliding(bulletPosition, bulletTexture.Bounds.Width / 2, asteroidPosition, asteroidTexture.Bounds.Width / 2) == true)
                    {
                        explosionPosition = (Vector2)asteroidPositions[asteroidIdx] - asteroidOffset; ;
                        asteroidHit = true;
                        asteroidPositions.RemoveAt(asteroidIdx);
                        asteroidVelocities.RemoveAt(asteroidIdx);
                        break;
                    }
                }
            }

            if (playerAlive == true)
            {
                for (int asteroidIdx = 0; asteroidIdx < asteroidPositions.Count; asteroidIdx++)
                {
                    Vector2 asteroidPosition = (Vector2)asteroidPositions[asteroidIdx];
                    if (IsColliding(playerPosition, shipTexture.Bounds.Width / 2, asteroidPosition, asteroidTexture.Bounds.Width / 2) == true)
                    {
                        win = false;
                        playerAlive = false;
                    }
                }
            }

            // No more asteroids, player completes level
            if(asteroidPositions.Count == 0)
            {
                win = true;
                levelCount++;
                bulletAlive = false;
                asteroidHit = false;

                if (ammoUsed < 10)
                {
                    tempBonusScore = ((10 - ammoUsed) * ((100 * levelCount) * .2f));
                    bonusScore = (int)tempBonusScore;
                    score += bonusScore;
                }

                missionComplete.Play(.2f, 0, 0);
                gameState = STATE_MISSIONOVER;
            }

            // Player dies
            if(playerAlive == false)
            {
                win = false;
                finalLevel = levelCount;
                finalScore = score;
                levelCount = 1;
                score = 0;
                missionFail.Play(.3f, 0, 0);
                asteroidSpeed = 50;
                gameState = STATE_MISSIONOVER;
            }

        }

        private void DrawGameState(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spaceTexture, new Vector2(0, 0), null, null, null, 0, null, Color.White);

            if (bulletAlive == true)
            {
                spriteBatch.Draw(bulletTexture, bulletPosition, null, null, null, 0, null, Color.White);
            }

            if (playerAlive == true)
            {
                spriteBatch.Draw(shipTexture, playerPosition, null, null, playerOffset, playerAngle, null, Color.White);
            }

            if (asteroidHit == true)
            {
                spriteBatch.Draw(explosionTexture, explosionPosition, null, null, null, 0, null, Color.White);
                asteroidHitTimer++;
                if(asteroidHitTimer >= explosionTimer)
                {
                    asteroidHit = false;
                    asteroidHitTimer = 0;
                }
            }

            for (int asteroidIdx = 0; asteroidIdx < asteroidPositions.Count; asteroidIdx++)
            {
                Vector2 position = (Vector2)asteroidPositions[asteroidIdx];
                spriteBatch.Draw(asteroidTexture, position, null, null, asteroidOffset, 0, null, Color.White);
            }

            spriteBatch.DrawString(robotaurFont, "Score " + score, new Vector2(50, 375), Color.White);
            spriteBatch.DrawString(robotaurFont, "Level " + levelCount, new Vector2(50, 400), Color.White);
        }

        private void UpdateMissionOverState(float deltaTime)
        {
            asteroidPositions.Clear();
            asteroidVelocities.Clear();
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                gameState = STATE_GAME;
                StartLevel();
            }
        }

        private void DrawMissionOverState(SpriteBatch spriteBatch)
        {
            string resultText;
            string quitText = "Press Escape to quit";
            string continueText;

            Vector2 resultPosition = new Vector2(0, -25);
            if(win == true)
            {
                spriteBatch.Draw(spaceTexture, new Vector2(0, 0), null, null, null, 0, null, Color.White);
                resultText = "Well done.\n\nThis section of the asteroid field is cleared.\n\nBe careful, it gets rougher ahead.";
                continueText = "Press enter to continue";
                spriteBatch.DrawString(robotaurFont, "Bonus " + bonusScore, new Vector2(50, 350), Color.White);
                spriteBatch.DrawString(robotaurFont, "Score " + score, new Vector2(50, 375), Color.White);
                spriteBatch.DrawString(robotaurFont, "Level " + levelCount, new Vector2(50, 400), Color.White);
                resultTexture = missionCompleteTexture;
            }
            else
            {
                spriteBatch.Draw(failBackTexture, new Vector2(0, 0), null, null, null, 0, null, Color.White);
                resultText = "In these final moments\n\nYour spirit watches\n\nThe asteroids\n\nDestroy your home\n\nYou think of Earth\n\nIt's people\n\nYour family\n\nYou have failed all.";
                continueText = "Press enter to restart";
                spriteBatch.DrawString(robotaurFont, "Score " + finalScore, new Vector2(50, 375), Color.White);
                spriteBatch.DrawString(robotaurFont, "Level " + finalLevel, new Vector2(50, 400), Color.White);
                resultTexture = missionFailedTexture;
            }


            spriteBatch.Draw(resultTexture, resultPosition, null, null, null, 0, null, Color.White);
            spriteBatch.DrawString(robotaurFontSmall, resultText, new Vector2(50, 70), Color.White);
            
            
            spriteBatch.DrawString(robotaurFont, continueText, new Vector2(50, 425), Color.Green);
            spriteBatch.DrawString(robotaurFont, quitText, new Vector2(50, 450), Color.Red);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            switch (gameState)
            {
                case STATE_SPLASH:
                    DrawSplashState(spriteBatch);
                    break;
                case STATE_MISSION:
                    DrawMissionState(spriteBatch);
                    break;
                case STATE_GAME:
                    DrawGameState(spriteBatch);
                    break;
                case STATE_MISSIONOVER:
                    DrawMissionOverState(spriteBatch);
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}


/*
 * TO DO
 * Add multiple bullets
 * Add further bonus based on timespan/duration of level
 * Keep images going off screen - bounce back sooner
 * Make asteroids bounce off eather other at certain level
 * Different text at each level
 * Create installer
*/
