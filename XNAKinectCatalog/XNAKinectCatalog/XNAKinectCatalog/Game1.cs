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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        AdvancedCamera camera;
        BasicEffect effect;
        InputHandler input;

        Texture2D texture;

        Vector3 mousePosition;


        Matrix world = Matrix.Identity;


        VertexPositionTexture[] verts;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        int[] indices;

        // ===============================================================================
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //GameSettings.ScreenHeight = gameConfig.PreferredHeight;
            //GameSettings.ScreenWidth = gameConfig.PreferredWidth;

            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 1024;

            //graphics.ToggleFullScreen();
            graphics.ApplyChanges();

            Mouse.SetPosition(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
            
            input = new InputHandler(this);
            Components.Add(input);

        }

        // ===============================================================================
        void SetupIndexBuffer() { 
        
            indices = new int[6];

            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;

        }

        // ===============================================================================
        void CreateBuffers()
        {

            // Initialize vertices
            verts = new VertexPositionTexture[8];
            verts[0] = new VertexPositionTexture(new Vector3(-1, 1, 1), new Vector2(0, 0));
            verts[1] = new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(1, 0));
            verts[2] = new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(0, 1));
            verts[3] = new VertexPositionTexture(new Vector3(1, -1, 1), new Vector2(1, 1));
            verts[4] = new VertexPositionTexture(new Vector3(-1, 1, -1), new Vector2(0, 0));
            verts[5] = new VertexPositionTexture(new Vector3(1, 1, -1), new Vector2(1, 0));
            verts[6] = new VertexPositionTexture(new Vector3(-1, -1, -1), new Vector2(0, 1));
            verts[7] = new VertexPositionTexture(new Vector3(1, -1, -1), new Vector2(1, 1));



            // Set vertex data in VertexBuffer
            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), verts.Length, BufferUsage.None);

            vertexBuffer.SetData(verts);

            //indexBuffer = new IndexBuffer(GraphicsDevice, typeof(int), 3, BufferUsage.WriteOnly);
            //indexBuffer.SetData(indices);

        }

        // ===============================================================================
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // Initialize camera
            camera = new XNAKinectCatalogCamera(this); // Camera(this, new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up);

            Components.Add(camera); 
            
            base.Initialize();
        }

        // ===============================================================================
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            texture = Content.Load<Texture2D>(@"Textures\HL");

            CreateBuffers();

            // Initialize the BasicEffect
            effect = new BasicEffect(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        // ===============================================================================
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        // ===============================================================================
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //// TODO: Add your update logic here
            //// Translation
            //KeyboardState keyboardState = Keyboard.GetState();
            //if (keyboardState.IsKeyDown(Keys.Left))
            //    world *= Matrix.CreateTranslation(-.01f, 0, 0);
            //if (keyboardState.IsKeyDown(Keys.Right))
            //    world *= Matrix.CreateTranslation(.01f, 0, 0);


            base.Update(gameTime);
        }

        // ===============================================================================
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Set object and camera info
            effect.World = world;
            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;
            effect.Texture = texture;
            effect.TextureEnabled = true;

            // Begin effect and draw for each pass
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, verts, 0, 2);
            }


            base.Draw(gameTime);
        }

        // ===============================================================================
    }
}
