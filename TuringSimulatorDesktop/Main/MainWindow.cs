using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TuringBackend;
using TuringBackend.Systems.VisualProgramming;
using TuringSimulatorDesktop.Input;
using TuringSimulatorDesktop.UI;

namespace TuringSimulatorDesktop
{
    public class MainWindow : Game
    {
        public GraphicsDeviceManager GraphicsManager;
        SpriteBatch spriteBatch;
                
        List<UIElement> CoreElements;
        WindowManager WinManager;

        int ReferenceWidth = 1920;
        int ReferenceHeight = 1080;

        public MainWindow()
        {
            GraphicsManager = new GraphicsDeviceManager(this);

            GraphicsManager.PreparingDeviceSettings += (object s, PreparingDeviceSettingsEventArgs args) =>
            {
                args.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;

            OnResize(this, null);

            CoreElements = new List<UIElement>();
            WinManager = new WindowManager();

            /*
            Renderer = new MeshRenderer(GraphicsDevice);

            List<VertexPositionColor> Verticies = new List<VertexPositionColor>();
            Verticies.Add(new VertexPositionColor(new Vector3(0f, 0f, 0f), Color.Red));
            Verticies.Add(new VertexPositionColor(new Vector3(25f, 25f, 0f), Color.Red));
            Verticies.Add(new VertexPositionColor(new Vector3(50f, 0f, 0f), Color.Red));

            List<int> Indices = new List<int>();
            Indices.Add(0);
            Indices.Add(1);
            Indices.Add(2);

            MeshData Mesh1 = new MeshData(Verticies.ToArray(), Indices.ToArray());
                
            Renderer.AddMesh(Mesh1);
            Renderer.FinaliseMeshAddtionsAndDeletions();
            */
            
            // TODO: use this.Content to load your game content here
            SpriteFont MainFont = Content.Load<SpriteFont>("BaseFont");
            UIElement.TextureLookup.Add(TextureLookupKey.StateNodeBackground, Content.Load<Texture2D>("StateNodeBackgroundTest"));

            GlobalGraphicsData.Device = GraphicsDevice;
            GlobalGraphicsData.Font = MainFont;

            //define start buttosn here?
            //bind here?

            Button AddWindowButton = new Button(UIElement.TextureLookup[TextureLookupKey.StateNodeBackground], new Vector2(0f, 0f));

            AddWindowButton.Clicked += (AddWindowButton) =>
            {
                WinManager.AddWindow();
            };

            CoreElements.Add(AddWindowButton);

        }

        protected override void Update(GameTime gameTime)
        {      
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();

            WinManager.Update();

            if (GlobalGraphicsData.UIRequiresRedraw)
            {
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.CornflowerBlue);

                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.Identity);

                for (int i = 0; i < CoreElements.Count; i++)
                {
                    CoreElements[i].Draw(spriteBatch, null);
                }

                WinManager.Draw(spriteBatch);

                DebugManager.Draw(GraphicsDevice, spriteBatch);
                spriteBatch.End();

                //GraphicsDevice.SetRenderTarget(null);
                //Renderer.Effect.Projection = Matrix.CreateOrthographicOffCenter(0, Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, 0, Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight, 0f, 1f);
                //Renderer.Draw();
            }
            else
            {
                //only redraw debug menu
            }
            base.Draw(gameTime);
        }

        public void OnResize(object Sender, EventArgs Args)
        {
            //UIElement.DrawScaleMatrix = Matrix.CreateScale((float)Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth / (float)ReferenceWidth, (float)Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight / (float)ReferenceHeight, 1f);

            /*
            for (int i = 0; i < UIElement.AllUIElements.Count; i++)
            {
                UIElement.AllUIElements[i].NotifyScreenResize();
            }
            */
        }

    }
}