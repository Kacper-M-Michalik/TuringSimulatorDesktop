using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TuringCore;
using TuringServer;
using TuringSimulatorDesktop.Input;
using TuringSimulatorDesktop.UI;
using FontStashSharp;
using System.IO;

namespace TuringSimulatorDesktop
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using var game = new TuringSimulatorDesktop.MainWindow();
            game.Run();
        }
    }

    public class MainWindow : Game
    {
        public GraphicsDeviceManager GraphicsManager;
        public View CurrentView;

        SpriteBatch ScreenBatch;

        public MainWindow()
        {
            GraphicsManager = new GraphicsDeviceManager(this);

            GraphicsManager.PreparingDeviceSettings += (object s, PreparingDeviceSettingsEventArgs args) =>
            {
                args.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.IsBorderless = false;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;

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
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            GlobalInterfaceData.BaseWindow = this;
            GlobalInterfaceData.FullscreenViewport = GraphicsDevice.Viewport;

            GlobalInterfaceData.Device = GraphicsDevice;
            GlobalInterfaceData.UIEffect = Content.Load<Effect>("UIShader");

            GlobalInterfaceData.Fonts = new FontSystem();
            //GlobalInterfaceData.Fonts.AddFont(File.ReadAllBytes(@"Fonts/Roboto-Regular.ttf"));
            GlobalInterfaceData.Fonts.AddFont(File.ReadAllBytes(@"C:/Windows/Fonts/arial.ttf"));
            
            GlobalUIRenderer.Setup(GlobalInterfaceData.Device);

            CurrentView = new MainScreenView();
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

            GlobalInterfaceData.Time = gameTime;

            InputManager.Update();

            if (GlobalInterfaceData.UIRequiresRedraw)
            {
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.CornflowerBlue);

                CurrentView.Draw();                
                
                ScreenBatch.Begin();
                DebugManager.Draw(GraphicsDevice, ScreenBatch, gameTime);
                ScreenBatch.End();
                //GlobalInterfaceData.UIRequiresRedraw = false;
                
            }
            else
            {
                //only redraw debug menu
            }
            base.Draw(gameTime);
        }

        public void OnResize(object Sender, EventArgs Args)
        {
            GlobalInterfaceData.FullscreenViewport = new Viewport(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
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