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


    // ===========================================================================================
    public interface IInputHandler
    {

        bool WasPressed(int playerIndex, InputHandler.ButtonType buttonType, Keys keys);

        KeyboardHandler KeyboardState { get; }
        GamePadState[] GamePads { get; }
        ButtonHandler ButtonHandler { get; }

#if !XBOX360
        MouseState MouseState { get; }
        MouseState PreviousMouseState { get; }
        MouseState OriginalMouseState { get; }
#endif

    }; // public interface IInputHandler

    // ===========================================================================================
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class InputHandler : Microsoft.Xna.Framework.GameComponent, IInputHandler
    {
        // **********************************************************************************************
        #region Enum

        public enum ButtonType
        {
            A, B, Back, LeftShoulder, LeftStick,
            RightShoulder, RightStick, Start, X, Y
        }

        #endregion

        // **********************************************************************************************
        #region Vars

        private KeyboardHandler keyboard;
        private ButtonHandler gamePadHandler = new ButtonHandler();
        private GamePadState[] gamePads = new GamePadState[4];
        private bool allowsExiting;

#if !XBOX360
        private MouseState mouseState;
        private MouseState prevMouseState;
        private MouseState originalMouseState;
#endif

        #endregion

        // **********************************************************************************************
        #region Constructs

        public InputHandler(Game game) : this(game, false) { }

        public InputHandler(Game game, bool allowsExiting)
            : base(game)
        {

            this.allowsExiting = allowsExiting;

            // Add this game component to the game services collection
            game.Services.AddService(typeof(IInputHandler), this);

            //initialize our member fields
            keyboard = new KeyboardHandler();
            gamePads[0] = GamePad.GetState(PlayerIndex.One);
            gamePads[1] = GamePad.GetState(PlayerIndex.Two);
            gamePads[2] = GamePad.GetState(PlayerIndex.Three);
            gamePads[3] = GamePad.GetState(PlayerIndex.Four);

#if !XBOX360
            Game.IsMouseVisible = false;
            prevMouseState = Mouse.GetState();
            originalMouseState = Mouse.GetState();
#endif

        } // public InputHandler (Game game, bool allowsExiting)

        #endregion

        // **********************************************************************************************
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Keyboars
            keyboard.Update();
            gamePadHandler.Update();

            if (allowsExiting)
            {
                if (keyboard.IsKeyDown(Keys.Escape))
                    Game.Exit();

                // Allows the default game to exit on Xbox360 and Win
                if (gamePadHandler.WasButtonPressed(0, ButtonType.Back))
                    Game.Exit();
            }

            gamePads[0] = GamePad.GetState(PlayerIndex.One);
            gamePads[1] = GamePad.GetState(PlayerIndex.Two);
            gamePads[2] = GamePad.GetState(PlayerIndex.Three);
            gamePads[3] = GamePad.GetState(PlayerIndex.Four);

            // Mouse
#if !XBOX360
            prevMouseState = mouseState;
            mouseState = Mouse.GetState();
#endif

            base.Update(gameTime);

        } // public override void Update (GameTime gameTime)

        #region IInputHandler Members

        // ================================================================================
        public bool WasPressed(int playerIndex, InputHandler.ButtonType buttonType, Keys keys)
        {
            if (gamePadHandler.WasButtonPressed(playerIndex, buttonType) || keyboard.WasKeyPressed(keys))
                return (true);
            else
                return (false);
        }

        // ================================================================================
        public KeyboardHandler KeyboardState
        {
            get { return (keyboard); }
        }

        // ================================================================================
        public ButtonHandler ButtonHandler
        {
            get { return (gamePadHandler); }
        }

        // ================================================================================
        public GamePadState[] GamePads
        {
            get { return (gamePads); }
        }

        // ================================================================================
        public MouseState MouseState
        {
            get { return (mouseState); }
        }

        // ================================================================================
        public MouseState PreviousMouseState
        {
            get { return (prevMouseState); }
        }

        // ================================================================================
        public MouseState OriginalMouseState
        {
            get { return (originalMouseState); }
        }


        #endregion
    } // public class InputHandler : Microsoft.Xna.Framework.GameComponent, IInputHandler

    // ================================================================================
    public class ButtonHandler
    {
        private GamePadState[] prevGamePadsState = new GamePadState[4];
        private GamePadState[] gamePadsState = new GamePadState[4];

        public ButtonHandler()
        {
            prevGamePadsState[0] = GamePad.GetState(PlayerIndex.One);
            prevGamePadsState[1] = GamePad.GetState(PlayerIndex.Two);
            prevGamePadsState[2] = GamePad.GetState(PlayerIndex.Three);
            prevGamePadsState[3] = GamePad.GetState(PlayerIndex.Four);
        }

        public void Update()
        {
            //set our previous state to our new state
            prevGamePadsState[0] = gamePadsState[0];
            prevGamePadsState[1] = gamePadsState[1];
            prevGamePadsState[2] = gamePadsState[2];
            prevGamePadsState[3] = gamePadsState[3];
            //get our new state
            //gamePadsState = GamePad.State .GetState();
            gamePadsState[0] = GamePad.GetState(PlayerIndex.One);
            gamePadsState[1] = GamePad.GetState(PlayerIndex.Two);
            gamePadsState[2] = GamePad.GetState(PlayerIndex.Three);
            gamePadsState[3] = GamePad.GetState(PlayerIndex.Four);
        }

        public bool WasButtonPressed(int playerIndex, InputHandler.ButtonType button)
        {
            int pi = playerIndex;
            switch (button)
            { //start switch
                case InputHandler.ButtonType.A:
                    {
                        return (gamePadsState[pi].Buttons.A == ButtonState.Pressed && prevGamePadsState[pi].Buttons.A == ButtonState.Released);
                    }
                case InputHandler.ButtonType.B:
                    {
                        return (gamePadsState[pi].Buttons.B == ButtonState.Pressed && prevGamePadsState[pi].Buttons.B == ButtonState.Released);
                    }
                case InputHandler.ButtonType.Back:
                    {
                        return (gamePadsState[pi].Buttons.Back == ButtonState.Pressed && prevGamePadsState[pi].Buttons.Back == ButtonState.Released);
                    }
                case InputHandler.ButtonType.LeftShoulder:
                    {
                        return (gamePadsState[pi].Buttons.LeftShoulder == ButtonState.Pressed && prevGamePadsState[pi].Buttons.LeftShoulder == ButtonState.Released);
                    }
                case InputHandler.ButtonType.LeftStick:
                    {
                        return (gamePadsState[pi].Buttons.LeftStick == ButtonState.Pressed && prevGamePadsState[pi].Buttons.LeftStick == ButtonState.Released);
                    }
                case InputHandler.ButtonType.RightShoulder:
                    {
                        return (gamePadsState[pi].Buttons.RightShoulder == ButtonState.Pressed && prevGamePadsState[pi].Buttons.RightShoulder == ButtonState.Released);
                    }
                case InputHandler.ButtonType.RightStick:
                    {
                        return (gamePadsState[pi].Buttons.RightStick == ButtonState.Pressed && prevGamePadsState[pi].Buttons.RightStick == ButtonState.Released);
                    }
                case InputHandler.ButtonType.Start:
                    {
                        return (gamePadsState[pi].Buttons.Start == ButtonState.Pressed && prevGamePadsState[pi].Buttons.Start == ButtonState.Released);
                    }
                case InputHandler.ButtonType.X:
                    {
                        return (gamePadsState[pi].Buttons.X == ButtonState.Pressed && prevGamePadsState[pi].Buttons.X == ButtonState.Released);
                    }
                case InputHandler.ButtonType.Y:
                    {
                        return (gamePadsState[pi].Buttons.Y == ButtonState.Pressed && prevGamePadsState[pi].Buttons.Y == ButtonState.Released);
                    }

                default:
                    throw (new ArgumentException());

            } //end switch
        }

    } // public class ButtonHandler

    // ================================================================================

    public class KeyboardHandler
    {
        private KeyboardState prevKeyboardState;
        private KeyboardState keyboardState;

        public KeyboardHandler()
        {
            prevKeyboardState = Keyboard.GetState();
        }

        public bool IsKeyDown(Keys key)
        {
            return (keyboardState.IsKeyDown(key));
        }

        public bool IsHoldingKey(Keys key)
        {
            return (keyboardState.IsKeyDown(key) && prevKeyboardState.IsKeyDown(key));
        }

        public bool WasKeyPressed(Keys key)
        {
            return (keyboardState.IsKeyDown(key) && prevKeyboardState.IsKeyUp(key));

        }

        public bool HasReleasedKey(Keys key)
        {
            return (keyboardState.IsKeyUp(key) && prevKeyboardState.IsKeyDown(key));
        }

        public void Update()
        {
            //set our previous state to our new state
            prevKeyboardState = keyboardState;
            //get our new state
            keyboardState = Keyboard.GetState();
        }

    } // public class KeyboardHandler

    // ================================================================================

}
