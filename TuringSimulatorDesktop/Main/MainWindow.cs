using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TuringCore;
using TuringServer;
using TuringSimulatorDesktop.Input;
using TuringSimulatorDesktop.UI;

namespace TuringSimulatorDesktop
{
    public class MainWindow : Game
    {
        public GraphicsDeviceManager GraphicsManager;
        SpriteBatch ScreenBatch;
        public View CurrentView;

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
            ScreenBatch = new SpriteBatch(GraphicsDevice);

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
            
            //LOAD HERE
            SpriteFont MainFont = Content.Load<SpriteFont>("BaseFont");
            GlobalGraphicsData.TextureLookup.Add(TextureLookupKey.StateNodeBackground, Content.Load<Texture2D>("StateNodeBackgroundTest"));

            GlobalGraphicsData.BaseWindow = this;

            GlobalGraphicsData.Device = GraphicsDevice;
            GlobalGraphicsData.Font = MainFont;

            GlobalGraphicsData.TabHeight = 25;

            GlobalMeshRenderer.Setup(GlobalGraphicsData.Device, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);

            CurrentView = new MainScreenView(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
        }

        protected override void Update(GameTime gameTime)
        {      
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            GlobalGraphicsData.Time = gameTime;

            InputManager.Update();

            if (GlobalGraphicsData.UIRequiresRedraw)
            {
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.CornflowerBlue);

                CurrentView.Draw();
                
                ScreenBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.Identity);
                DebugManager.Draw(GraphicsDevice, ScreenBatch);
                ScreenBatch.End();

                //GlobalGraphicsData.UIRequiresRedraw = false;
            }
            else
            {
                //only redraw debug menu
            }
            base.Draw(gameTime);
        }

        public void OnResize(object Sender, EventArgs Args)
        {
            /*
            for (int i = 0; i < GlobalGraphicsData.BackBufferListeners.Count; i++)
            {
                GlobalGraphicsData.BackBufferListeners[i].BackBufferResized(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
            }
            */
            CurrentView.ViewResize(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);            
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            Server.CloseServer();
        }
    }
}