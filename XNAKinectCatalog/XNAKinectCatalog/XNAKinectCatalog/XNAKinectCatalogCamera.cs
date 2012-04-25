using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XNAKinectCatalog
{
   public  class XNAKinectCatalogCamera : AdvancedCamera
    {

          Game1 _gameInstance;


        // For collision detection
        public float fMinimumCollisionDistance = 1.5f;

        public string CollisionType { get; set; }
        


        const float fMouseRotationSpeed = 0.09f;

        private float _fMovementDelta;
        private float _fStepsDelta;

        public int CollisionCount;
        public string collAgainst = "";

        // to use the translation when colliding against a wall in angle, we have to check whether the previous step collided to keep the "momentum"
        private byte previousStepCollided; // 0 = No, 1 = X, 2 = Z

        // ================================================================================================

       public XNAKinectCatalogCamera (Game1 game) : base(game) { }

       public XNAKinectCatalogCamera(Game1 game, CameraType type)
            : base(game)
        {
            _gameInstance = game;

            // Subscribe this as a game service to be accesible from the entire application
            game.Services.AddService(typeof(XNAKinectCatalogCamera), this);

            _cameraType = type;
        }

        // ================================================================================================
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize ()
        {

            base.Initialize();

        } // public override void Initialize ()

        // ================================================================================================

        public override void UpdateCameraPosition (GameTime gameTime)
        {

            Vector3 v3OriginalCameraPos = cameraPosition;
            Vector3 v3NewCameraPos = cameraPosition;

            // Read Input

            // Delta (time) to control drawing refresh properly
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // The game is running, so the camera can be updated
            //if (GameManager.State.Value.ToString() == "Labyrinth.GameStates.RunningLevelState")
            //{
                // Keyboard // Controller
                // Yaw
                if (input.KeyboardState.IsKeyDown(Keys.Left) || (input.GamePads[0].ThumbSticks.Right.X < 0) || (input.GamePads[0].DPad.Left == ButtonState.Pressed))
                    cameraYaw += (spinRate * timeDelta);

                if (input.KeyboardState.IsKeyDown(Keys.Right) || (input.GamePads[0].ThumbSticks.Right.X > 0) || (input.GamePads[0].DPad.Right == ButtonState.Pressed))
                    cameraYaw -= (spinRate * timeDelta);

                //  Pitch
                if (input.KeyboardState.IsKeyDown(Keys.Down) || (input.GamePads[0].ThumbSticks.Right.Y < 0) || (input.GamePads[0].DPad.Down == ButtonState.Pressed))
                    cameraPitch -= (spinRate * timeDelta);

                if (input.KeyboardState.IsKeyDown(Keys.Up) || (input.GamePads[0].ThumbSticks.Right.Y > 0) || (input.GamePads[0].DPad.Up == ButtonState.Pressed))
                    cameraPitch += (spinRate * timeDelta);

                // Mouse read
#if !XBOX360

                // This calculations are used when debugging to allow the use of the button for mouse movement
                // Yaw
                //if ((input.PreviousMouseState.X > input.MouseState.X) && (input.MouseState.LeftButton == ButtonState.Pressed))
                //    cameraYaw += (spinRate * timeDelta);
                //else if ((input.PreviousMouseState.X < input.MouseState.X) && (input.MouseState.LeftButton == ButtonState.Pressed))
                //    cameraYaw -= (spinRate * timeDelta);

                //// Pitch
                //if ((input.PreviousMouseState.Y > input.MouseState.Y) && (input.MouseState.LeftButton == ButtonState.Pressed))
                //    cameraPitch += (spinRate * timeDelta);
                //else if ((input.PreviousMouseState.Y < input.MouseState.Y) && (input.MouseState.LeftButton == ButtonState.Pressed))
                //    cameraPitch -= (spinRate * timeDelta);
                


                if (input.MouseState != input.OriginalMouseState)
                {

                    // Yaw
                    cameraYaw -= (fMouseRotationSpeed * (input.MouseState.X - input.OriginalMouseState.X));

                    // Pitch
                    cameraPitch -= (fMouseRotationSpeed * (input.MouseState.Y - input.OriginalMouseState.Y));

                    // Mouse.SetPosition(GameSettings.ScreenWidth / 2, GameSettings.ScreenHeight / 2);
                }

                // Mouse.SetPosition(GameSettings.ScreenWidth / 2, GameSettings.ScreenHeight / 2);

            //} // if (GameManager.State.Value.ToString() == "Labyrinth.GameStates.PausedLevelState")

#endif

            // clamp values if gone beyond boundaries
            if (cameraYaw > 360)
                cameraYaw -= 360;

            if (cameraYaw < 0)
                cameraYaw += 360;

            if (cameraPitch > 89)
                cameraPitch = 89;

            if (cameraPitch < -89)
                cameraPitch = -89;

            //update movement (none for this base class)
            movement *= (moveRate * timeDelta);

            // Actual Transformation
            Matrix rotationMatrixCamera;
            Matrix rotationMatrixMovement;

            // X / Y rot
            // This one's for the view (always update all axes)
            // First we mul the RotX then Y
            rotationMatrixCamera = Matrix.CreateRotationX(MathHelper.ToRadians(cameraPitch)) * Matrix.CreateRotationY(MathHelper.ToRadians(cameraYaw));

            // This one's for the spatial movement
            if (_cameraType == CameraType.Type2D)
                // 2D Cam                
                rotationMatrixMovement = Matrix.CreateRotationY(MathHelper.ToRadians(cameraYaw));
            else
                // 3D Cam
                rotationMatrixMovement = Matrix.CreateRotationX(MathHelper.ToRadians(cameraPitch)) * Matrix.CreateRotationY(MathHelper.ToRadians(cameraYaw));


            // for the FPS Camera, apply the movement transformation
            if (movement != Vector3.Zero)
            {
                // player.TotalDistanceTraveled += .1;
                Vector3.Transform(ref movement, ref rotationMatrixMovement, out movement);
                v3NewCameraPos += movement;
            }

            // Create a Vector pointing to the place the camera is facing
            Vector3.Transform(ref base.cameraReference, ref rotationMatrixCamera, out base.transformedReference);

            cameraPosition = v3NewCameraPos;

            // Calculate the Position the camera is looking at
            Vector3.Add(ref cameraPosition, ref transformedReference, out base.cameraTarget);

        } // public override void UpdateCameraPosition (GameTime gameTime)

        // ================================================================================================
        // ================================================================================================
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //reset movement vector
            movement = Vector3.Zero;
            base.movingDirection = MovingDirection.None;

            // Up n Down
            if (input.KeyboardState.IsKeyDown(Keys.Z))
                movement.Y -= 0.1f;

            if (input.KeyboardState.IsKeyDown(Keys.Q))
                movement.Y += 0.1f;

            // Left n Right
            if (input.KeyboardState.IsKeyDown(Keys.A) || (input.GamePads[0].ThumbSticks.Left.X < 0))
            {
                movement.X -= _fMovementDelta;
                base.movingDirection = MovingDirection.West;
            }
            else if (input.KeyboardState.IsKeyDown(Keys.D) || (input.GamePads[0].ThumbSticks.Left.X > 0))
            {
                movement.X += _fMovementDelta;
                base.movingDirection = MovingDirection.East;
            } // if (input.KeyboardState.IsKeyDown(Keys.D) || (input.GamePads[0].ThumbSticks.Left.X > 0))

            // Forward n Back
            if (input.KeyboardState.IsKeyDown(Keys.S) || (input.GamePads[0].ThumbSticks.Left.Y < 0))
            {
                movement.Z += _fMovementDelta;

                switch (base.movingDirection)
                {
                    case MovingDirection.None:
                        base.movingDirection = MovingDirection.South;
                        break;
                    case MovingDirection.East:
                        base.movingDirection = MovingDirection.SouthEast;
                        break;
                    case MovingDirection.West:
                        base.movingDirection = MovingDirection.SouthWest;
                        break;
                }

            }
            else if (input.KeyboardState.IsKeyDown(Keys.W) || (input.GamePads[0].ThumbSticks.Left.Y > 0))
            {
                movement.Z -= _fMovementDelta;

                switch (base.movingDirection)
                {
                    case MovingDirection.None:
                        base.movingDirection = MovingDirection.North;
                        break;
                    case MovingDirection.East:
                        base.movingDirection = MovingDirection.NorthEast;
                        break;
                    case MovingDirection.West:
                        base.movingDirection = MovingDirection.NorthWest;
                        break;
                }

            } // if (input.KeyboardState.IsKeyDown(Keys.W) || (input.GamePads[0].ThumbSticks.Left.Y > 0))

            //make sure we don’t increase speed if pushing up and over (diagonal)
            //if (movement.LengthSquared() != 0)
            //    movement.Normalize();

            base.Update(gameTime);
        }

        // ================================================================================================



    }
}
