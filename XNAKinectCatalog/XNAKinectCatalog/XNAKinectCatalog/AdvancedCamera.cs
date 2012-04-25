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


namespace XNAKinectCatalog
{

    public interface ICamera { }


    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AdvancedCamera : Microsoft.Xna.Framework.GameComponent, ICamera
    {

        #region props


        protected IInputHandler input;
        private GraphicsDeviceManager graphics;

        protected CameraType _cameraType;

        private Matrix projection;
        private Matrix view;
        protected Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 30.0f);

        public Vector3 cameraTarget = Vector3.Zero;
        private Vector3 cameraUpVector = Vector3.Up;
        public Vector3 transformedReference = Vector3.Zero;

        public Vector3 cameraReference = new Vector3(0.0f, 0.0f, -1.0f);

        public float cameraYaw = 0.0f;
        protected float spinRate = 80.0f;
        public float cameraPitch = 0.0f;

        protected Vector3 movement = Vector3.Zero;
        protected float moveRate = 220.0f;

        public Matrix ViewMatrix { get { return view; } set { view = value; } }
        public Matrix ProjectionMatrix { get { return projection; } }

        public Vector3 Position
        {
            get { return cameraPosition; }
            set { cameraPosition = value; }
        }

        public Vector3 Target { get { return cameraTarget; } }
        public Vector3 Movement { get { return movement; } }
        public Vector3 TransformedReference { get { return transformedReference; } }

        public MovingDirection movingDirection;

        #endregion

        public AdvancedCamera(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            // Add this game component to the game services collection
            game.Services.AddService(typeof(AdvancedCamera), this);
        }



        // ================================================================================================
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            graphics = (GraphicsDeviceManager)Game.Services.GetService(typeof(IGraphicsDeviceManager));
            input = (IInputHandler)Game.Services.GetService(typeof(IInputHandler));

            base.Initialize();

            InitializeCamera();

        }

        // ================================================================================================
        private void InitializeCamera()
        {
            float aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height;

            Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1.0f, 10000.0f, out projection);

            Matrix.CreateLookAt(ref cameraPosition, ref cameraTarget, ref cameraUpVector, out view);
        }

        // ================================================================================================
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            UpdateCameraPosition(gameTime);

            Matrix.CreateLookAt(ref cameraPosition, ref cameraTarget, ref cameraUpVector, out view);

            base.Update(gameTime);
        }

        // ================================================================================================
        public virtual void UpdateCameraPosition(GameTime gameTime)
        {
            // Store the position of the camera before any trans in case it gets invalid (collision)

            // Read Input

            // Delta (time) to control drawing refresh properly
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

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

            // Yaw
            if ((input.PreviousMouseState.X > input.MouseState.X) && (input.MouseState.LeftButton == ButtonState.Pressed))
            {
                cameraYaw += (spinRate * timeDelta);
            }
            else if ((input.PreviousMouseState.X < input.MouseState.X) && (input.MouseState.LeftButton == ButtonState.Pressed))
            {
                cameraYaw -= (spinRate * timeDelta);
            }

            // Pitch
            if ((input.PreviousMouseState.Y > input.MouseState.Y) && (input.MouseState.LeftButton == ButtonState.Pressed))
            {
                cameraPitch += (spinRate * timeDelta);
            }
            else if ((input.PreviousMouseState.Y < input.MouseState.Y) && (input.MouseState.LeftButton == ButtonState.Pressed))
            {
                cameraPitch -= (spinRate * timeDelta);
            }

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

            // Y rot
            Matrix rotationMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(cameraPitch)) * Matrix.CreateRotationY(MathHelper.ToRadians(cameraYaw));
            //Matrix.CreateRotationY(MathHelper.ToRadians(cameraYaw), out rotationMatrix); //Matrix.CreateRotationX(MathHelper.ToRadians(cameraPitch), out rotationMatrix);

            // for the FPS Camera, apply the movement transformation
            if (movement != Vector3.Zero)
            {
                Vector3.Transform(ref movement, ref rotationMatrix, out movement);
                cameraPosition += movement;
            }

            //add in pitch to the rotation
            // rotationMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(cameraPitch)) * rotationMatrix;

            // Create a Vector pointing to the place the camera is facing
            // Vector3 transformedReference;
            Vector3.Transform(ref cameraReference, ref rotationMatrix, out transformedReference);

            // Calculate the Position the camera is looking at
            Vector3.Add(ref cameraPosition, ref transformedReference, out cameraTarget);
        }

        // ================================================================================================
        // ================================================================================================
        // ================================================================================================
        // ================================================================================================
        // ================================================================================================

    } // public class Camera : Microsoft.Xna.Framework.GameComponent, ICamera


}
